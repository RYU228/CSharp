using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using DSLibrary;

namespace MasterMES.FORMS.CODE
{
    public partial class frmPartsCode : frmBase, CLASSES.IButtonAction
    {
        public frmPartsCode()
        {
            InitializeComponent();
        }

        #region 오버라이드 이벤트
        //protected override void OnActivated(EventArgs e)
        //{
        //    base.OnActivated(e);
        //    MainButton.ActiveButton("INIT,NEW,SAVE,DELETE");
        //}

        //protected override void OnDeactivate(EventArgs e)
        //{
        //    base.OnDeactivate(e);
        //    MainButton.ActiveButton();
        //}

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.InitButtonClick();
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

                var initSet = SqlHelper.GetDataSet("xp_Parts", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                grcParts.SetColumns(initSet.Tables[0], "UseClss");

                DxControl.SaveGridFormat(grvParts, GridFormat.Default);

                this.NewButtonClick();
                this.ViewPartsGrid();
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
        }

        /// <summary>
        /// 신규 버튼
        /// </summary>
        public void NewButtonClick()
        {
            DxControl.ClearControl(lcgParts);
            txtPartsCode.Focus();
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
                    { "PartsCode,4", txtPartsCode.EditValue.CString().Length != 4 },
                    { "PartsName", txtPartsName.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;
                var logType = ActionLogType.Insert;
                var message = MessageText.OKInsert;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Exist, SqlDbType.Char);
                inParams.Add("@i_PartsCode", txtPartsCode.EditValue.CString(), SqlDbType.Char);

                if (SqlHelper.Exist("xp_Parts", inParams, out _, out _))
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
                inParams.Add("@i_PartsCode", txtPartsCode.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_PartsName", txtPartsName.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_UseClss", chkUseClss.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_Remark", mmeRemark.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_InptUser", AppConfig.Login.PersonID, SqlDbType.VarChar);

                SqlHelper.Execute("xp_Parts", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                var partsCode = txtPartsCode.EditValue.CString();
                this.NewButtonClick();
                this.ViewPartsGrid();
                grvParts.FindRowByValue("PartsCode", partsCode);

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
                    { "PartsCode", txtPartsCode.EditValue.CString() == "" },
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
                inParams.Add("@i_PartsCode", txtPartsCode.EditValue.CString(), SqlDbType.Char);

                SqlHelper.Execute("xp_Parts", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                this.NewButtonClick();
                this.ViewPartsGrid();

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

        #endregion

        #region 폼 컨트롤 이벤트
        private void grvParts_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle >= 0)
                this.ViewPartsRow();
        }

        private void grvParts_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            this.ViewPartsRow();
        }

        #endregion

        #region 사용자 정의 메서드
        /// <summary>
        /// Parts 정보 조회 - Grid
        /// </summary>
        private void ViewPartsGrid()
        {
            try
            {
                var startTime = DateTime.Now;

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetList, SqlDbType.Char);

                var table = SqlHelper.GetDataTable("xp_Parts", inParams, out _, out _);

                grcParts.SetData(table);

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
        private void ViewPartsRow()
        {
            try
            {
                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetRow, SqlDbType.Char);
                inParams.Add("@i_PartsCode", grvParts.GetFocusedRowCellValue("PartsCode").CString(), SqlDbType.Char);

                var row = SqlHelper.GetDataRow("xp_Parts", inParams, out _, out _);

                if (row != null)
                {
                    txtPartsCode.EditValue = row["PartsCode"];
                    txtPartsName.EditValue = row["PartsName"];
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