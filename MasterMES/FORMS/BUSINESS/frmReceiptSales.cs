using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using DSLibrary;

namespace MasterMES.FORMS.BUSINESS
{
    public partial class frmReceiptSales : frmBase
    {
        public delegate void ViewReceiptSalesEventHandler();
        //public event ViewCustomChargeEventHandler ViewCustomChargeEvent;
        public ViewReceiptSalesEventHandler ChangeReceiptSales;

        private string ReceiptID;
        private string ContactDate;

        #region 생성자
        public frmReceiptSales()
        {
            InitializeComponent();
        }

        public frmReceiptSales(string ReceiptID, string ContactDate) : this()
        {
            this.ReceiptID = ReceiptID;
            this.ContactDate = ContactDate;
        }

        #endregion

        #region 오버라이드 이벤트
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (ChangeReceiptSales != null)
                ChangeReceiptSales();
            else
                MsgBox.ErrorMessage("Event Error!");
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            txtReceiptID.EditValue = this.ReceiptID;
            dteContactDate.EditValue = this.ContactDate;
            //DateTime test = new DateTime(2022, 8, 12);
            //dteContactDate.DateTime = test;

            if (dteContactDate.EditValue.CString() != "")
            {
                ShowDataReceiptSales();
                mmeContactDetail.Select();
            }
            else
                btnNew.PerformClick();
        }

        #endregion

        #region 폼 컨트롤 이벤트
        /// <summary>
        /// 신규 버튼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNew_Click(object sender, EventArgs e)
        {
            DxControl.ClearControl(lcgReceiptSales);
        }

        /// <summary>
        /// 저장 버튼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveDataReceiptSales();
        }

        /// <summary>
        /// 종료 버튼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

        #region 사용자 정의 메서드
        /// <summary>
        /// 거래처 담당자 저장 : Popup폼은 메뉴 ID가 존재하지 않으므로 Action Log를 남기지 않는다.
        /// </summary>
        private void SaveDataReceiptSales()
        {
            try
            {
                var field = new Dictionary<string, bool>()
                {
                    { "ReceiptID", txtReceiptID.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                var message = MessageText.OKInsert;

                var inParams = new DbParamList();
                inParams.Add("@i_processID", ProcessType.Exist, SqlDbType.Char);
                inParams.Add("@i_ReceiptID", txtReceiptID.EditValue.CString(), SqlDbType.Char);

                if (SqlHelper.Exist("xp_ReceiptSales", inParams, out _, out _))
                {
                    if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionUpdate))
                        return;

                    message = MessageText.OKUpdate;
                }

                Utils.ShowWaitForm(this);
                btnSave.Enabled = false;

                inParams.Clear();
                inParams.Add("@i_ProcessID", ProcessType.Merge, SqlDbType.Char);
                inParams.Add("@i_ReceiptID", txtReceiptID.EditValue.CString(), SqlDbType.Char);
                //inParams.Add("@i_ContactDate", dteContactDate.EditValue.CDateString("yyyyMMdd"), SqlDbType.Date);
                inParams.Add("@i_ContactDate", dteContactDate.EditValue.CString(), SqlDbType.Date);
                inParams.Add("@i_ContactDetails", mmeContactDetail.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_InptUser", AppConfig.Login.PersonID, SqlDbType.VarChar);

                SqlHelper.Execute("xp_ReceiptSales", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                btnNew.PerformClick();
                MsgBox.OKMessage(message);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
            finally
            {
                btnSave.Enabled = true;
                Utils.CloseWaitForm();
            }
        }

        /// <summary>
        /// 거래처 담당자 정보 조회 : Row
        /// </summary>
        private void ShowDataReceiptSales()
        {
            try
            {
                ///DxControl.ClearControl(lcgReceiptSales);

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_processID", ProcessType.GetRow, SqlDbType.Char);
                inParams.Add("@i_ReceiptID", txtReceiptID.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_ContactDate", dteContactDate.EditValue.CString(), SqlDbType.Date);

                var row = SqlHelper.GetDataRow("xp_ReceiptSales", inParams, out _, out _);

                if (row != null)
                {
                    dteContactDate.EditValue = row["ContactDate"];
                    mmeContactDetail.EditValue = row["ContactDetails"];
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