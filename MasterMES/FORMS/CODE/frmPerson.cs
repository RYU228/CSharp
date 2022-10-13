using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using DSLibrary;

namespace MasterMES.FORMS.CODE
{
    public partial class frmPerson : frmBase, CLASSES.IButtonAction
    {
        #region 생성자
        public frmPerson()
        {
            InitializeComponent();

            txtPersonID.SetEditMask(MaskType.RegExpression, @"[0-9]{1,8}");
            txtCellPhoneNo.SetEditMask(MaskType.CellPhone);
            txtPhoneNo.SetEditMask(MaskType.PhoneFax);
            txtIDCardNo.SetEditMask(MaskType.IDCardNumber);
        }

        #endregion

        #region 오버라이드 이벤트
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            MainButton.ActiveButtons("INIT,NEW,SAVE,DELETE");
        }

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            MainButton.ActiveButtons();
        }

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
                var initSet = SqlHelper.GetDataSet("xp_Person", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                gleDepartID.SetData(initSet.Tables[0], "UseClss");
                gleDutyID.SetData(initSet.Tables[1], "UseClss");
                grcPerson.SetColumns(initSet.Tables[2], "UseClss");

                // 그리드 초기 형식 저장
                DxControl.SaveGridFormat(grvPerson, GridFormat.Default);

                // 폼 로딩과 동시에 데이터를 조회합니다.                
                this.FillGridPerson();
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
            finally
            {
                // 초기화버튼을 활성화하고 Wait폼을 종료합니다.
                MainButton.InitButton = true;
                Utils.CloseWaitForm();
            }
        }

        /// <summary>
        /// 신규 버튼
        /// </summary>
        public void NewButtonClick()
        {
            DxControl.ClearControl(lcgPerson);
            txtPersonID.Focus();
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
                    { "PersonName", txtPersonName.EditValue.CString() == "" },
                    { "PersonInit", txtPersonInit.EditValue.CString() == "" },
                    { "UserID", txtUserID.EditValue.CString() == "" },
                    { "DepartID", gleDepartID.EditValue.CString() == "" },
                    { "DutyID", gleDutyID.EditValue.CString() == "" },
                    { "JoinDate", dteJoinDate.EditValue.CString() == "" },
                    { "EMail", txtEMail.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;
                var logType = ActionLogType.Insert;
                var message = MessageText.OKInsert;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Exist, SqlDbType.Char);
                inParams.Add("@i_PersonID", txtPersonID.EditValue.CString());

                if (SqlHelper.Exist("xp_Person", inParams, out _, out _))
                {
                    if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionUpdate))
                        return;

                    logType = ActionLogType.Update;
                    message = MessageText.OKUpdate;
                }

                inParams.Clear();
                inParams.Add("@i_ProcessID", ProcessType.Merge, SqlDbType.Char);
                inParams.Add("@i_PersonID", txtPersonID.EditValue.CString());
                inParams.Add("@i_PersonName", txtPersonName.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_PersonInit", txtPersonInit.EditValue.CString(), SqlDbType.VarChar);
                inParams.Add("@i_UserID", txtUserID.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_DepartID", gleDepartID.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_DutyID", gleDutyID.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_JoinDate", dteJoinDate.EditValue.CDateString());
                inParams.Add("@i_RetireDate", dteRetireDate.EditValue.CDateString());
                inParams.Add("@i_CellPhoneNo", txtCellPhoneNo.EditValue.CString());
                inParams.Add("@i_PhoneNo", txtPhoneNo.EditValue.CString());
                inParams.Add("@i_IDCardNo", txtIDCardNo.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_Birthday", dteBirthday.EditValue.CDateString());
                inParams.Add("@i_EMail", txtEMail.EditValue.CString());
                inParams.Add("@i_ZipCode", txtZipCode.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_Address1", txtAddress1.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_Address2", txtAddress2.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_UseClss", chkUseClss.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_Remark", mmeRemark.EditValue.CString());
                inParams.Add("@i_InptUser", AppConfig.Login.PersonID);

                SqlHelper.Execute("xp_Person", inParams, out string[] retVal, out string retCode, out string retMsg);

                // 반환코드 검증 : 00 - 정상, 그외 - 오류
                if (retCode != "00")
                    throw new Exception(retMsg);

                this.NewButtonClick();
                this.FillGridPerson();
                grvPerson.FindRowByValue("PersonID", retVal[0]);

                SqlHelper.WriteLog(this, logType, startTime);
                MsgBox.OKMessage(message);
            }
            catch (Exception ex)
            {
                MsgBox.OKMessage(System.Runtime.InteropServices.Marshal.GetExceptionCode().CString());
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
                    { "EmpNo", txtPersonID.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionDelete))
                    return;

                var startTime = DateTime.Now;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Delete, SqlDbType.Char);
                inParams.Add("@i_PersonID", txtPersonID.EditValue.CString());

                SqlHelper.Execute("xp_Person", inParams, out string retCode, out string retMsg);

                // 반환코드 검증 : 00 - 정상, 그외 - 오류
                if (retCode != "00")
                    throw new Exception(retMsg);

                this.NewButtonClick();
                this.FillGridPerson();

                SqlHelper.WriteLog(this, ActionLogType.Delete, startTime);
                MsgBox.OKMessage(MessageText.OKDelete);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 조회 버튼
        /// </summary>
        public void ViewButtonClick()
        {
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
        /// Focused Row가 변경될 때
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvPerson_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle >= 0)
                this.ShowDataPerson();
        }

        private void grvPerson_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            this.ShowDataPerson();
        }

        #endregion

        #region 사용자 정의 메서드
        /// <summary>
        /// 사원정보 조회 : Grid
        /// </summary>
        private void FillGridPerson()
        {
            try
            {
                var startTime = DateTime.Now;
                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetList, SqlDbType.Char);

                var table = SqlHelper.GetDataTable("xp_Person", inParams, out _, out _);
                grcPerson.SetData(table);

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
        /// 사원정보 조회 : Row
        /// </summary>
        private void ShowDataPerson()
        {
            try
            {
                DxControl.ClearControl(lcgPerson);

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetRow, SqlDbType.Char);
                inParams.Add("@i_PersonID", grvPerson.GetFocusedRowCellValue("PersonID").CString(), SqlDbType.VarChar);

                var row = SqlHelper.GetDataRow("xp_Person", inParams, out _, out _);

                if (row != null)
                {
                    txtPersonID.EditValue = row["PersonID"];
                    txtPersonName.EditValue = row["PersonName"];
                    txtPersonInit.EditValue = row["PersonInit"];
                    txtUserID.EditValue = row["UserID"];
                    gleDepartID.EditValue = row["DepartID"];
                    gleDutyID.EditValue = row["DutyID"];
                    dteJoinDate.EditValue = row["JoinDate"];
                    dteRetireDate.EditValue = row["RetireDate"];
                    txtCellPhoneNo.EditValue = row["CellPhoneNo"];
                    txtPhoneNo.EditValue = row["PhoneNo"];
                    txtIDCardNo.EditValue = row["IDCardNo"];
                    dteBirthday.EditValue = row["Birthday"];
                    txtEMail.EditValue = row["EMail"];
                    txtZipCode.EditValue = row["ZipCode"];
                    txtAddress1.EditValue = row["Address1"];
                    txtAddress2.EditValue = row["Address2"];
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