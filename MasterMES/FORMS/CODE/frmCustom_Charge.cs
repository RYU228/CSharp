using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using DSLibrary;

namespace MasterMES.FORMS.CODE
{
    public partial class frmCustom_Charge : frmBase
    {
        public delegate void ViewCustomChargeEventHandler();
        //public event ViewCustomChargeEventHandler ViewCustomChargeEvent;
        public ViewCustomChargeEventHandler ChangeCustomCharge;

        private string CustomID;
        private string KCustom;
        private int chargeSeq;

        #region 생성자
        public frmCustom_Charge()
        {
            InitializeComponent();

            txtCellPhoneNo.SetEditMask(MaskType.CellPhone);
            txtPhoneNo.SetEditMask(MaskType.PhoneFax);
            txtFaxNo.SetEditMask(MaskType.PhoneFax);
        }

        public frmCustom_Charge(string CustomID, string KCustom, int chargeSeq) : this()
        {
            this.CustomID = CustomID;
            this.KCustom = KCustom;
            this.chargeSeq = chargeSeq;
        }

        #endregion

        #region 오버라이드 이벤트
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (ChangeCustomCharge != null)
                ChangeCustomCharge();
            else
                MsgBox.ErrorMessage("Event Error!");
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            txtCustomID.EditValue = this.CustomID;
            txtKCustom.EditValue = this.KCustom;
            txtChargeSeq.EditValue = this.chargeSeq;

            if (txtChargeSeq.EditValue.CString() != "")
                ShowDataCustomCharge();
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
            DxControl.ClearControl(lcgCustomCharge);
        }

        /// <summary>
        /// 저장 버튼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveCustomCharge();
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
        private void SaveCustomCharge()
        {
            try
            {
                var field = new Dictionary<string, bool>()
                {
                    { "CustomID", txtCustomID.EditValue.CString() == "" },
                    { "Charge", txtChargeName.EditValue.CString() == "" },
                    { "CellPhoneNo", txtCellPhoneNo.EditValue.CString() == "" },
                    { "PhoneNo", txtPhoneNo.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                var message = MessageText.OKInsert;

                var inParams = new DbParamList();
                inParams.Add("@i_processID", ProcessType.Exist, SqlDbType.Char);
                inParams.Add("@i_CustomID", txtCustomID.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_ChargeSeq", txtChargeSeq.EditValue.CShort(), SqlDbType.TinyInt);

                if (SqlHelper.Exist("xp_CustomCharge", inParams, out _, out _))
                {
                    if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionUpdate))
                        return;

                    message = MessageText.OKUpdate;
                }

                Utils.ShowWaitForm(this);
                btnSave.Enabled = false;

                inParams.Clear();
                inParams.Add("@i_ProcessID", ProcessType.Merge, SqlDbType.Char);
                inParams.Add("@i_CustomID", txtCustomID.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_ChargeSeq", txtChargeSeq.EditValue.CShort(), SqlDbType.TinyInt);
                inParams.Add("@i_ChargeName", txtChargeName.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_DepartName", txtDepartName.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_DutyName", txtDutyName.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_CellPhoneNo", txtCellPhoneNo.EditValue.CString(), SqlDbType.VarChar);
                inParams.Add("@i_PhoneNo", txtPhoneNo.EditValue.CString(), SqlDbType.VarChar);
                inParams.Add("@i_FaxNo", txtFaxNo.EditValue.CString(), SqlDbType.VarChar);
                inParams.Add("@i_EMail", txtEMail.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_UseClss", chkUseClss.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_Remark", mmeRemark.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_InptUser", AppConfig.Login.PersonID, SqlDbType.VarChar);

                SqlHelper.Execute("xp_CustomCharge", inParams, out string retCode, out string retMsg);

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
        private void ShowDataCustomCharge()
        {
            try
            {
                DxControl.ClearControl(lcgCustomCharge);

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_processID", ProcessType.GetRow, SqlDbType.Char);
                inParams.Add("@i_CustomID", txtCustomID.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_ChargeSeq", this.chargeSeq, SqlDbType.TinyInt);

                var row = SqlHelper.GetDataRow("xp_CustomCharge", inParams, out _, out _);

                if (row != null)
                {
                    txtCustomID.EditValue = row["CustomID"];
                    txtChargeSeq.EditValue = row["ChargeSeq"];
                    txtChargeName.EditValue = row["ChargeName"];
                    txtDepartName.EditValue = row["DepartName"];
                    txtDutyName.EditValue = row["DutyName"];
                    txtCellPhoneNo.EditValue = row["CellPhoneNo"];
                    txtPhoneNo.EditValue = row["PhoneNo"];
                    txtFaxNo.EditValue = row["FaxNo"];
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

        #endregion
    }
}