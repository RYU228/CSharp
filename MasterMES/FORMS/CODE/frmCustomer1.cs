using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Grid;
using DSLibrary;

namespace STI.FORMS.CODE
{
    public partial class frmCustomer : frmBase, CLASSES.IButtonAction
    {
        #region 생성자
        public frmCustomer()
        {
            InitializeComponent();
        }

        #endregion

        #region 오버라이드 이벤트
        //protected override void OnActivated(EventArgs e)
        //{
        //    base.OnActivated(e);
        //    MainButton.ActiveButton("INIT,NEW,COPY,SAVE,DELETE");
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

                var initSet = SqlHelper.GetDataSet("xp_Customer", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                gleCustType.SetData(initSet.Tables[0], "UseClss");
                grcCustomer.SetColumns(initSet.Tables[1], "UseClss");
                grcCharge.SetColumns(initSet.Tables[2], "UseClss", false, false);

                // 담당자 삭제 버튼
                var riChargeDelete = grvCharge.SetRepositoryItemSimpleButton("DeleteButton", RepositoryButtonType.None, "담당자 삭제");
                riChargeDelete.Click += RiChargeDelete_Click;

                this.NewButtonClick();
                this.ViewCustomerGrid();

                DxControl.SaveGridFormat(grvCustomer, GridFormat.Default);
                DxControl.SaveGridFormat(grvCharge, GridFormat.Default);

                DxControl.LoadGridFormat(grvCustomer, GridFormat.Current);
                DxControl.LoadGridFormat(grvCharge, GridFormat.Current);
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
            DxControl.ClearControl(lcgCustomer);

            grcCharge.DataSource = null;

            txtCustCode.Focus();
        }

        /// <summary>
        /// 복사 버튼
        /// </summary>
        public void CopyButtonClick()
        {
            try
            {
                var field = new Dictionary<string, bool>()
                {
                    { "CustCode", txtCustCode.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;

                if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionCopy))
                    return;

                Utils.ShowWaitForm(this);
                MainButton.CopyButton = false;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Copy, SqlDbType.Char);
                inParams.Add("@i_CustCode", txtCustCode.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_InptUser", AppConfig.Login.PersonID, SqlDbType.VarChar);

                SqlHelper.Execute("xp_Customer", inParams, out string[] retVal, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                this.NewButtonClick();
                this.ViewCustomerGrid();

                // 신규 생성된 거래처 코드에 해당하는 데이터를 화면에 디스플레이(FocusedRowChanged Event 발생)
                grvCustomer.FindRowByValue("CustCode", retVal[0]);

                SqlHelper.WriteLog(this, ActionLogType.Insert, startTime);
                MsgBox.OKMessage(MessageText.OKCopy);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
            finally
            {
                MainButton.CopyButton = true;
                Utils.CloseWaitForm();
            }
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
                    { "CustCode,4", txtCustCode.EditValue.CString() != "" && txtCustCode.EditValue.CString().LengthB() != 4 },
                    { "CustName", txtCustName.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;
                var logType = ActionLogType.Insert;
                var message = MessageText.OKInsert;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Exist, SqlDbType.Char);
                inParams.Add("@i_CustCode", txtCustCode.EditValue.CString(), SqlDbType.Char);

                if (SqlHelper.Exist("xp_Customer", inParams, out _, out _))
                {
                    if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionUpdate))
                        return;


                    logType = ActionLogType.Update;
                    message = MessageText.OKUpdate;
                }

                Utils.ShowWaitForm(this);
                MainButton.SaveButton = false;

                inParams.Clear();
                inParams.Add("@i_ProcessID", ProcessType.Merge, SqlDbType.Char);
                inParams.Add("@i_CustCode", txtCustCode.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_CustName", txtCustName.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_CustNameEng", txtCustNameEng.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_CustNameShort", txtCustNameShort.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_CeoName", txtCeoName.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_BizNo", txtBizNo.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_Condition", txtCondition.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_Category", txtCategory.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_CustType", gleCustType.EditValue.CString(), SqlDbType.VarChar);
                inParams.Add("@i_ZipCode", txtZipCode.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_Address1", txtAddress1.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_Address2", txtAddress2.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_RepPhoneNo", txtRepPhoneNo.EditValue.CString(), SqlDbType.VarChar);
                inParams.Add("@i_PhoneNo", txtPhoneNo.EditValue.CString(), SqlDbType.VarChar);
                inParams.Add("@i_FaxNo", txtFaxNo.EditValue.CString(), SqlDbType.VarChar);
                inParams.Add("@i_Homepage", txtHomepage.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_EMail", txtEMail.EditValue.CString(), SqlDbType.VarChar);
                inParams.Add("@i_UseClss", chkUseClss.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_Remark", mmeRemark.EditValue.CString(), SqlDbType.VarChar);
                inParams.Add("@i_InptUser", AppConfig.Login.PersonID, SqlDbType.VarChar);

                SqlHelper.Execute("xp_Customer", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                var custCode = txtCustCode.EditValue.CString();
                this.NewButtonClick();
                this.ViewCustomerGrid();
                grvCustomer.FindRowByValue("CustCode", custCode);

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
                    { "CustCode", txtCustCode.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;

                if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionDelete))
                    return;

                Utils.ShowWaitForm(this);
                MainButton.SaveButton = false;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Delete, SqlDbType.Char);
                inParams.Add("@i_CustCode", txtCustCode.EditValue.CString(), SqlDbType.Char);

                SqlHelper.Execute("xp_Customer", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                this.NewButtonClick();
                this.ViewCustomerGrid();

                SqlHelper.WriteLog(this, ActionLogType.Delete, startTime);
                MsgBox.OKMessage(MessageText.OKInsert);
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
        /// 출력 버튼
        /// </summary>
        public void PrintButtonClick(int reportIndex)
        {
        }

        #endregion

        #region 폼 컨트롤 이벤트
        /// <summary>
        /// 그리드 행변경시 화면에 정보 표시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvCustomer_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle >= 0)
            {
                var custCode = (sender as GridView).GetFocusedRowCellValue("CustCode").CString();
                this.ViewCustomerRow(custCode);
                this.ViewCustomerChargeGrid();
            }
        }

        private void grvCustomer_RowClick(object sender, RowClickEventArgs e)
        {
            var custCode = (sender as GridView).GetFocusedRowCellValue("CustCode").CString();
            this.ViewCustomerRow(custCode);
            this.ViewCustomerChargeGrid();
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
                { "CustCode", txtCustCode.EditValue.CString() == "" },
            };
            if (Utils.CheckField(field))
                return;

            var ea = e as DXMouseEventArgs;
            GridView view = sender as GridView;
            var info = view.CalcHitInfo(ea.Location);

            var custCode = txtCustCode.EditValue.CString();
            var custName = txtCustName.EditValue.CString();
            var chargeSeq = 0;

            if (info.InRow || info.InRowCell)
                chargeSeq = view.GetFocusedRowCellValue("ChargeSeq").CShort();

            // Delegate를 이용한 이벤트 발생으로 데이터 조회
            using (frmCustomer_Charge frmCharge = new frmCustomer_Charge(custCode, custName, chargeSeq))
            {
                //frmCharge.ViewCustomerChargeEvent += new frmCustomer_Charge.ViewCustomerChargeEventHandler(ViewCustomerChargeGrid);
                frmCharge.ChangeCustomerCharge += ViewCustomerChargeGrid;
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
                    { "CustCode", txtCustCode.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionDelete))
                    return;

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Delete, SqlDbType.Char);
                inParams.Add("@i_CustCode", txtCustCode.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_ChargeSeq", grvCharge.GetFocusedRowCellValue("ChargeSeq").CInt(), SqlDbType.Int);

                SqlHelper.Execute("xp_CustomerCharge", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                this.ViewCustomerChargeGrid();
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
        private void ViewCustomerGrid()
        {
            try
            {
                var startTime = DateTime.Now;

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetList, SqlDbType.Char);

                var table = SqlHelper.GetDataTable("xp_Customer", inParams, out _, out _);

                grcCustomer.SetData(table);

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
        /// 거래처 조회 : Row  -> 복사시 생성된 거래처를 화면에 표시하기 위해서 custCode를 Parameter로 받는다.
        /// </summary>
        private void ViewCustomerRow(string custCode)
        {
            try
            {
                DxControl.ClearControl(lcgCustomer);

                var startTime = DateTime.Now;

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetRow, SqlDbType.Char);
                inParams.Add("@i_CustCode", custCode, SqlDbType.Char);

                var row = SqlHelper.GetDataRow("xp_Customer", inParams, out _, out _);

                if (row != null)
                {
                    txtCustCode.EditValue = row["CustCode"];
                    txtCustName.EditValue = row["CustName"];
                    txtCustNameEng.EditValue = row["CustNameEng"];
                    txtCustNameShort.EditValue = row["CustNameShort"];
                    txtCeoName.EditValue = row["CeoName"];
                    txtBizNo.EditValue = row["BizNo"];
                    txtCondition.EditValue = row["Condition"];
                    txtCategory.EditValue = row["Category"];
                    gleCustType.EditValue = row["CustType"];
                    txtZipCode.EditValue = row["ZipCode"];
                    txtAddress1.EditValue = row["Address1"];
                    txtAddress2.EditValue = row["Address2"];
                    txtRepPhoneNo.EditValue = row["RepPhoneNo"];
                    txtPhoneNo.EditValue = row["PhoneNo"];
                    txtFaxNo.EditValue = row["FaxNo"];
                    txtHomepage.EditValue = row["Homepage"];
                    txtEMail.EditValue = row["EMail"];
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
        private void ViewCustomerChargeGrid()
        {
            try
            {
                var inParams = new DbParamList();
                inParams.Add("@i_processID", ProcessType.GetList, SqlDbType.Char);
                inParams.Add("@i_CustCode", txtCustCode.EditValue.CString(), SqlDbType.Char);

                var table = SqlHelper.GetDataTable("xp_CustomerCharge", inParams, out _, out _);
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
