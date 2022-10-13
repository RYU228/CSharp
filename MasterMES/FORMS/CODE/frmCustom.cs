using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Grid;
using DSLibrary;

namespace MasterMES.FORMS.CODE
{
    public partial class frmCustom : frmBase, CLASSES.IButtonAction
    {
        #region 생성자
        public frmCustom()
        {
            InitializeComponent();

            // 입력형식 설정
            txtCustomID.SetEditMask(MaskType.RegExpression, @"[0-9]{4}");   // 거래처 코드(4자리)
            txtCustomNo.SetEditMask(MaskType.BizNumber);                    // 사업자번호
            txtPhoneNo1.SetEditMask(MaskType.PhoneFax);                     // 전화번호 1
            txtPhoneNo2.SetEditMask(MaskType.PhoneFax);                     // 전화번호 2
            txtFaxNo.SetEditMask(MaskType.PhoneFax);                        // 팩스번호
        }

        #endregion

        #region 오버라이드 이벤트
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

                var initSet = SqlHelper.GetDataSet("xp_Custom", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                gleCustType.SetData(initSet.Tables[0], "UseClss");      // 거래처 구분
                gleRegion.SetData(initSet.Tables[1], "UseClss");        // 지역
                grcCustom.SetColumns(initSet.Tables[2], "UseClss");     // 거래처 그리드
                grcCharge.SetColumns(initSet.Tables[3], "UseClss");     // 담당자 그리드

                // 담당자 삭제 버튼
                var riChargeDelete = grvCharge.SetRepositoryItemSimpleButton("DeleteButton", RepositoryButtonType.None, "삭제");
                riChargeDelete.Click += RiChargeDelete_Click;

                // 그리드 초기 형식 저장
                DxControl.SaveGridFormat(grvCustom, GridFormat.Default);
                DxControl.SaveGridFormat(grvCharge, GridFormat.Default);

                //this.NewButtonClick();
                this.FillGridCustom();
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
            DxControl.ClearControl(lcgCustom);

            //grcCharge.DataSource = null;

            txtCustomID.Focus();
        }

        /// <summary>
        /// 복사 버튼
        /// </summary>
        public void CopyButtonClick()
        {
            txtCustomID.EditValue = null;

            //try
            //{
            //    var field = new Dictionary<string, bool>()
            //    {
            //        { "CustomID", txtCustomID.EditValue.CString() == "" },
            //    };
            //    if (Utils.CheckField(field))
            //        return;

            //    var startTime = DateTime.Now;

            //    if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionCopy))
            //        return;

            //    Utils.ShowWaitForm(this);
            //    MainButton.CopyButton = false;

            //    var inParams = new DbParamList();
            //    inParams.Add("@i_ProcessID", ProcessType.Copy, SqlDbType.Char);
            //    inParams.Add("@i_CustomID", txtCustomID.EditValue.CString(), SqlDbType.Char);
            //    inParams.Add("@i_InptUser", AppConfig.Login.PersonID, SqlDbType.VarChar);

            //    SqlHelper.Execute("xp_Custom", inParams, out string[] retVal, out string retCode, out string retMsg);

            //    if (retCode != "00")
            //        throw new Exception(retMsg);

            //    this.NewButtonClick();
            //    this.FillGridCustom();

            //    // 신규 생성된 거래처 코드에 해당하는 데이터를 화면에 디스플레이(FocusedRowChanged Event 발생)
            //    grvCustom.FindRowByValue("CustomID", retVal[0]);

            //    SqlHelper.WriteLog(this, ActionLogType.Insert, startTime);
            //    MsgBox.OKMessage(MessageText.OKCopy);
            //}
            //catch (Exception ex)
            //{
            //    MsgBox.ErrorMessage(ex.CString());
            //}
            //finally
            //{
            //    MainButton.CopyButton = true;
            //    Utils.CloseWaitForm();
            //}
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
                    { "CustomID,4", txtCustomID.EditValue.CString() != "" && txtCustomID.EditValue.CString().LengthB() != 4 },
                    { "KCustom", txtKCustom.EditValue.CString() == "" },
                    { "CustType", gleCustType.EditValue.CString() == "" },
                    { "CustomNo", txtCustomNo.EditValue.CString() == "" },
                    { "Chief", txtChief.EditValue.CString() == "" },
                    { "Condition", txtCondition.EditValue.CString() == "" },
                    { "Category", txtCategory.EditValue.CString() == "" },
                    { "Region", gleRegion.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;
                var logType = ActionLogType.Insert;
                var message = MessageText.OKInsert;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Exist, SqlDbType.Char);
                inParams.Add("@i_CustomID", txtCustomID.EditValue.CString(), SqlDbType.Char);

                if (SqlHelper.Exist("xp_Custom", inParams, out _, out _))
                {
                    if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionUpdate))
                        return;

                    logType = ActionLogType.Update;
                    message = MessageText.OKUpdate;
                }

                inParams.Clear();
                inParams.Add("@i_ProcessID", ProcessType.Merge, SqlDbType.Char);
                inParams.Add("@i_CustomID", txtCustomID.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_KCustom", txtKCustom.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_ECustom", txtECustom.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_ShortCustom", txtShortCustom.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_CustType", gleCustType.EditValue.CString(), SqlDbType.VarChar);
                inParams.Add("@i_CustomNo", txtCustomNo.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_Chief", txtChief.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_Condition", txtCondition.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_Category", txtCategory.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_ZipCode", txtZipCode.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_Address1", txtAddress1.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_Address2", txtAddress2.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_Region", gleRegion.EditValue.CString(), SqlDbType.VarChar);
                inParams.Add("@i_PhoneNo1", txtPhoneNo1.EditValue.CString(), SqlDbType.VarChar);
                inParams.Add("@i_PhoneNo2", txtPhoneNo2.EditValue.CString(), SqlDbType.VarChar);
                inParams.Add("@i_FaxNo", txtFaxNo.EditValue.CString(), SqlDbType.VarChar);
                inParams.Add("@i_EMail", txtEMail.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_HomePage", txtHomepage.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_UseClss", chkUseClss.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_Remark", mmeRemark.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_InptUser", AppConfig.Login.PersonID, SqlDbType.VarChar);

                SqlHelper.Execute("xp_Custom", inParams, out string[] retVal, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                this.NewButtonClick();
                this.FillGridCustom();

                // 신규 또는 수정된 거래처 코드에 해당하는 데이터를 화면에 디스플레이(FocusedRowChanged Event 발생)
                grvCustom.FindRowByValue("CustomID", retVal[0]);

                SqlHelper.WriteLog(this, logType, startTime);
                MsgBox.OKMessage(message);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
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
                    { "CustomID", txtCustomID.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;

                if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionDelete))
                    return;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Delete, SqlDbType.Char);
                inParams.Add("@i_CustomID", txtCustomID.EditValue.CString(), SqlDbType.Char);

                SqlHelper.Execute("xp_Custom", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                this.NewButtonClick();
                this.FillGridCustom();

                SqlHelper.WriteLog(this, ActionLogType.Delete, startTime);
                MsgBox.OKMessage(MessageText.OKDelete);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
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
        /// <summary>
        /// 그리드 행 변경시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvCustom_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle >= 0)
            {
                var CustomID = (sender as GridView).GetFocusedRowCellValue("CustomID").CString();
                this.ShowDataCustom(CustomID);
                this.FillGirdCharge();
            }
        }

        /// <summary>
        /// 그리드 행 클릭시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvCustom_RowClick(object sender, RowClickEventArgs e)
        {
            var CustomID = (sender as GridView).GetFocusedRowCellValue("CustomID").CString();
            this.ShowDataCustom(CustomID);
            this.FillGirdCharge();
        }

        /// <summary>
        /// 담당자 등록 창 호출 : 생성자를 이용 팝업폼에 데이터 전달, 이벤트 이용 데이터 재 조회
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvCharge_DoubleClick(object sender, EventArgs e)
        {
            var field = new Dictionary<string, bool>()
            {
                { "CustomID", txtCustomID.EditValue.CString() == "" },
            };
            if (Utils.CheckField(field))
                return;

            var ea = e as DXMouseEventArgs;
            GridView view = sender as GridView;
            var info = view.CalcHitInfo(ea.Location);

            var CustomID = txtCustomID.EditValue.CString();
            var KCustom = txtKCustom.EditValue.CString();
            var chargeSeq = 0;

            if (info.InRow || info.InRowCell)
                chargeSeq = view.GetFocusedRowCellValue("ChargeSeq").CShort();

            // Delegate를 이용한 이벤트 발생으로 데이터 조회
            using (frmCustom_Charge frmCharge = new frmCustom_Charge(CustomID, KCustom, chargeSeq))
            {
                //frmCharge.ViewCustomChargeEvent += new frmCustom_Charge.ViewCustomChargeEventHandler(FillGirdCharge);
                frmCharge.ChangeCustomCharge += FillGirdCharge;
                Utils.ShowPopupForm(frmCharge);
            }
        }

        /// <summary>
        /// 담당자 삭제 버튼 : Row를 삭제함
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RiChargeDelete_Click(object sender, EventArgs e)
        {
            try
            {
                var field = new Dictionary<string, bool>()
                {
                    { "CustomID", txtCustomID.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionDelete))
                    return;

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Delete, SqlDbType.Char);
                inParams.Add("@i_CustomID", txtCustomID.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_ChargeSeq", grvCharge.GetFocusedRowCellValue("ChargeSeq").CInt(), SqlDbType.Int);

                SqlHelper.Execute("xp_CustomCharge", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                this.FillGirdCharge();
                MsgBox.OKMessage(MessageText.OKDelete);
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

        #region 사용자 정의 메서드
        /// <summary>
        /// 거래처 조회 : Grid
        /// </summary>
        private void FillGridCustom()
        {
            try
            {
                var startTime = DateTime.Now;

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetList, SqlDbType.Char);

                var table = SqlHelper.GetDataTable("xp_Custom", inParams, out _, out _);

                grcCustom.SetData(table);

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
        /// 거래처 조회 : Row  -> 복사시 생성된 거래처를 화면에 표시하기 위해서 CustomID를 Parameter로 받는다.
        /// </summary>
        private void ShowDataCustom(string customID)
        {
            try
            {
                DxControl.ClearControl(lcgCustom);

                var startTime = DateTime.Now;

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetRow, SqlDbType.Char);
                inParams.Add("@i_CustomID", customID, SqlDbType.Char);

                var row = SqlHelper.GetDataRow("xp_Custom", inParams, out _, out _);

                if (row != null)
                {
                    txtCustomID.EditValue = row["CustomID"];
                    txtKCustom.EditValue = row["KCustom"];
                    txtECustom.EditValue = row["ECustom"];
                    txtShortCustom.EditValue = row["ShortCustom"];
                    gleCustType.EditValue = row["CustType"];
                    txtCustomNo.EditValue = row["CustomNo"];
                    txtChief.EditValue = row["Chief"];
                    txtCondition.EditValue = row["Condition"];
                    txtCategory.EditValue = row["Category"];
                    txtZipCode.EditValue = row["ZipCode"];
                    txtAddress1.EditValue = row["Address1"];
                    txtAddress2.EditValue = row["Address2"];
                    gleRegion.EditValue = row["Region"];
                    txtPhoneNo1.EditValue = row["PhoneNo1"];
                    txtPhoneNo2.EditValue = row["PhoneNo2"];
                    txtFaxNo.EditValue = row["FaxNo"];
                    txtEMail.EditValue = row["EMail"];
                    txtHomepage.EditValue = row["Homepage"];
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

        /// <summary>
        /// 거래처 담당자 조회 : Grid
        /// </summary>
        private void FillGirdCharge()
        {
            try
            {
                var inParams = new DbParamList();
                inParams.Add("@i_processID", ProcessType.GetList, SqlDbType.Char);
                inParams.Add("@i_CustomID", txtCustomID.EditValue.CString(), SqlDbType.Char);

                var table = SqlHelper.GetDataTable("xp_CustomCharge", inParams, out _, out _);
                grcCharge.SetData(table);
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
