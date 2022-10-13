using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DSLibrary;
using MasterMES.CLASSES;

namespace MasterMES.FORMS.CODE
{
    public partial class frmEquipment : frmBase, CLASSES.IButtonAction
    {
        public frmEquipment()
        {
            InitializeComponent();
        }

        #region 오버라이드 이벤트
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.InitButtonClick();

            DxControl.ButtonEditEvent += ButtonEditEvent;
            Extensions.ButtonEditEvent += ButtonEditEvent;
        }

        #endregion

        #region 인터페이스 - 공용 버튼 클릭
        /// <summary>
        /// 초기화 버튼
        /// </summary>
        public void InitButtonClick()
        {
            try
            {
                Utils.ShowWaitForm(this);
                MainButton.InitButton = false;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Initial, SqlDbType.Char);

                var initSet = SqlHelper.GetDataSet("xp_Equipment", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                gleWarehouseID.SetData(initSet.Tables[0], "WarehouseID,UseClss");
                gleLocationID.SetData(initSet.Tables[1], "WarehouseID,LocationID,UseClss");
                grcEquip.SetColumns(initSet.Tables[2], "UseClss");

                gleLocationID.SetCascading(gleWarehouseID, "WarehouseID", "WarehouseID");

                // 그리드 초기 형식 저장
                DxControl.SaveGridFormat(grvEquip, GridFormat.Default);
                DxControl.LoadGridFormat(grvEquip, GridFormat.Current);

                this.FillGridEquipment();
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
            finally
            {
                MainButton.InitButton = true;
                Utils.CloseWaitForm();
            }
        }

        /// <summary>
        /// 조회 버튼
        /// </summary>
        public void ViewButtonClick()
        {
            this.FillGridEquipment();
        }

        /// <summary>
        /// 신규 버튼
        /// </summary>
        public void NewButtonClick()
        {
            DxControl.ClearControl(lcgEquip);
            txtEquipID.Focus();
        }

        /// <summary>
        /// 복사 버튼
        /// </summary>
        public void CopyButtonClick()
        {
        }

        /// <summary>
        /// 저장 버튼
        /// </summary>
        public void SaveButtonClick()
        {
            try
            {
                var field = new Dictionary<string, bool>()
                {
                    //{ "EquipID,4", txtEquipID.EditValue.CString().Length != 4 },
                    { "EquipName", txtEquipName.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;
                var logType = ActionLogType.Insert;
                var message = MessageText.OKInsert;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Exist, SqlDbType.Char);
                inParams.Add("@i_EquipCode", txtEquipID.EditValue.CString(), SqlDbType.Char);

                if (SqlHelper.Exist("xp_Equipment", inParams, out _, out _))
                {
                    if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionUpdate))
                        return;

                    logType = ActionLogType.Update;
                    message = MessageText.OKUpdate;
                }

                Utils.ShowWaitForm(this);
                MainButton.SaveButton = false;

                inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Merge, SqlDbType.Char);
                inParams.Add("@i_EquipID", txtEquipID.EditValue.CString(), SqlDbType.VarChar);
                inParams.Add("@i_EquipName", txtEquipName.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_ModelName", txtModelName.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_MakerID", bteMakerID.Tag.CString(), SqlDbType.Char);
                inParams.Add("@i_MakeYear", txtMakeYear.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_PurchDate", dtePurchDate.EditValue.CString(), SqlDbType.Date);
                inParams.Add("@i_PurchCustID", btePurchCustID.Tag.CString(), SqlDbType.Char);
                inParams.Add("@i_PurchAmt", spePurchAmt.EditValue.CDouble(), SqlDbType.Decimal);
                inParams.Add("@i_WarehouseID", gleWarehouseID.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_LocationID", gleLocationID.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_UseClss", chkUseClss.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_Remark", mmeRemark.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_InptUser", AppConfig.Login.PersonID, SqlDbType.VarChar);

                SqlHelper.Execute("xp_Equipment", inParams, out string[] retVal, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                var equipID = txtEquipID.EditValue.CString();
                
                this.FillGridEquipment();
                grvEquip.FindRowByValue("EquipID", retVal[0]);

                SqlHelper.WriteLog(this, logType, startTime);

                MsgBox.OKMessage(message);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
            finally
            {
                MainButton.SaveButton = true;
                Utils.CloseWaitForm();
            }
        }

        /// <summary>
        /// 삭제 버튼
        /// </summary>
        public void DeleteButtonClick()
        {
            try
            {
                var field = new Dictionary<string, bool>()
                {
                    { "EquipID", txtEquipID.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionDelete))
                    return;

                var startTime = DateTime.Now;

                Utils.ShowWaitForm(this);
                MainButton.DeleteButton = false;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Delete, SqlDbType.Char);
                inParams.Add("@i_EquipID", txtEquipID.EditValue.CString(), SqlDbType.Char);

                SqlHelper.Execute("xp_Equipment", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                this.FillGridEquipment();

                SqlHelper.WriteLog(this, ActionLogType.Delete, startTime);

                MsgBox.OKMessage(MessageText.OKDelete);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
            finally
            {
                MainButton.DeleteButton = true;
                Utils.CloseWaitForm();
            }
        }

        /// <summary>
        /// 출력 버튼
        /// </summary>
        public void PrintButtonClick(int reportIndex)
        {
        }

        /// <summary>
        /// ButtonEdit 공통 이벤트 (FindData 호출)
        /// </summary>
        /// <param name="sender"></param>
        private bool ButtonEditEvent(object sender)
        {
            ButtonEdit bte = sender as ButtonEdit;
            FindDataResult fdResult = FindDataResult.FIND_NONE;

            switch (bte.Name)
            {
                // 검색 컨트롤
                //case "bteSearchCustom":
                //    fdResult = FindData.ShowFindData(FindData.FIND_CUSTOM, bte.EditValue.CString(), null, null, null, false, bte);
                //    break;

                // 입력 컨트롤
                case "bteMakerID":
                    fdResult = FindData.ShowFindData(FindData.FIND_CUSTOMTYPE, bte.EditValue.CString(), "2", null, null, false, bte);
                    break;

                case "btePurchCustID":
                    fdResult = FindData.ShowFindData(FindData.FIND_CUSTOMTYPE, bte.EditValue.CString(), "2", null, null, false, bte);
                    break;

                    //조건문 및 여러 정보를 가져올 경우 예시
                    //FindData.ShowFindData(FindData.FIND_ORDERSUB, bte.EditValue.CString(), "2022060001", null, null, false, bte, bteStuffWidth, bteWorkWidth);
            }

            // 찾기 성공이면 다음 컨트롤로 포커스 이동, 실패면 포커스 이동 중지
            if (fdResult == FindDataResult.FIND_OK)
                return true;
            else
                return false;
        }

        #endregion

        #region 폼 컨트롤 이벤트
        private void grvEquip_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle >= 0)
            {
                var equipID = (sender as GridView).GetFocusedRowCellValue("EquipID").CString();
                this.ShowDataEquip(equipID);
            }   
        }

        private void grvEquip_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            var equipID = (sender as GridView).GetFocusedRowCellValue("EquipID").CString();
            this.ShowDataEquip(equipID);
        }

        private void gleWarehouseID_EditValueChanged(object sender, EventArgs e)
        {
            gleLocationID.EditValue = null;
        }

        #endregion

        #region 사용자 정의 메서드
        /// <summary>
        /// Parts 정보 조회 - Grid
        /// </summary>
        private void FillGridEquipment()
        {
            try
            {
                var startTime = DateTime.Now;

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetList, SqlDbType.Char);

                var table = SqlHelper.GetDataTable("xp_Equipment", inParams, out _, out _);

                grcEquip.SetData(table);

                SqlHelper.WriteLog(this, ActionLogType.Select, startTime);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
            finally
            {
                Utils.CloseWaitForm();
            }
        }

        /// <summary>
        /// Parts 정보 조회 - Row
        /// </summary>
        private void ShowDataEquip(string equipID)
        {
            try
            {
                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetRow, SqlDbType.Char);
                inParams.Add("@i_EquipID", equipID, SqlDbType.VarChar);

                var row = SqlHelper.GetDataRow("xp_Equipment", inParams, out _, out _);

                if (row != null)
                {
                    txtEquipID.EditValue = row["EquipID"];
                    txtEquipName.EditValue = row["EquipName"];
                    txtModelName.EditValue = row["ModelName"];
                    bteMakerID.Tag = row["MakerID"];
                    bteMakerID.EditValue = row["MakerName"];
                    txtMakeYear.EditValue = row["MakeYear"];
                    dtePurchDate.EditValue = row["PurchDate"];
                    btePurchCustID.Tag = row["PurchCustID"];
                    btePurchCustID.EditValue = row["PurchCustName"];
                    spePurchAmt.EditValue = row["PurchAmt"];
                    gleWarehouseID.EditValue = row["WarehouseID"];
                    gleLocationID.EditValue = row["LocationID"];
                    chkUseClss.EditValue = row["UseClss"];
                    mmeRemark.EditValue = row["Remark"];
                }

            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
            finally
            {
                Utils.CloseWaitForm();
            }
        }
        #endregion
    }
}