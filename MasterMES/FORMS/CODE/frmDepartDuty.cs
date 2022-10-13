using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using DSLibrary;

namespace MasterMES.FORMS.CODE
{
    public partial class frmDepartDuty : frmBase, CLASSES.IButtonAction
    {
        #region 생성자
        public frmDepartDuty()
        {
            InitializeComponent();

            tacOrg.SelectedTabPage = tapDepart;

            txtDepartID.SetEditMask(MaskType.RegExpression, @"[0-9]{4}");
            txtDutyID.SetEditMask(MaskType.RegExpression, @"[0-9]{4}");
        }

        #endregion

        #region 오버라이드 이벤트
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            InitButtonClick();
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
                var initSet = SqlHelper.GetDataSet("xp_Depart", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                grcDepart.SetColumns(initSet.Tables[0], "UseClss");
                grcDuty.SetColumns(initSet.Tables[1], "UseClss");

                // 입력란 초기화
                this.ClearDeaprt();
                this.ClearDuty();

                // 그리드 초기 형식 저장
                DxControl.SaveGridFormat(grvDepart, GridFormat.Default);
                DxControl.SaveGridFormat(grvDuty, GridFormat.Default);

                FillGridDepart();
                FillGridDuty();
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
        /// 신규 버튼
        /// </summary>
        public void NewButtonClick()
        {
            if (tacOrg.SelectedTabPage == tapDepart)
            {
                this.ClearDeaprt();
                txtDepartID.Focus();
            }
            else
            {
                this.ClearDuty();
                txtDutyID.Focus();
            }
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
            if (tacOrg.SelectedTabPage == tapDepart)
                this.SaveDepart();
            else
                this.SaveDuty();
        }

        /// <summary>
        /// 삭제 버튼
        /// </summary>
        public void DeleteButtonClick()
        {
            if (tacOrg.SelectedTabPage == tapDepart)
                this.DeleteDepart();
            else
                this.DeleteDuty();
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
        private void grvDepart_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle >= 0)
                this.ShowDataDepart();
        }

        private void grvDepart_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            this.ShowDataDepart();
        }

        private void grvDuty_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle >= 0)
                this.ShowDataDuty();
        }

        private void grvDuty_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            this.ShowDataDuty();
        }

        #endregion

        #region 사용자 정의 메서드 - 부서관리
        private void ClearDeaprt()
        {
            txtDepartID.EditValue = null;
            txtDepartName.EditValue = null;
            txtDepartInit.EditValue = null;
            chkDeUseClss.EditValue = "0";
            mmeDeRemark.EditValue = null;
        }

        /// <summary>
        /// 부서 저장
        /// </summary>
        private void SaveDepart()
        {
            try
            {
                var field = new Dictionary<string, bool>()
                {
                    { "DepartID,4", txtDepartID.EditValue.CString() != "" && txtDepartID.EditValue.CString().LengthB() != 4 },
                    { "DepartName", txtDepartName.EditValue.CString() == "" },
                    { "DepartInit", txtDepartInit.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;
                var logType = ActionLogType.Insert;     // 기본 처리 LogType을 입력으로 설정
                var message = MessageText.OKInsert;     // Stored Procedure에서 Insert, Update 분리 여부에 따라 설정 필요

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Exist, SqlDbType.Char);
                inParams.Add("@i_DepartID", txtDepartID.EditValue.CString(), SqlDbType.Char);

                if (SqlHelper.Exist("xp_Depart", inParams, out _, out _))
                {
                    if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionUpdate))
                        return;

                    logType = ActionLogType.Update;
                    message = MessageText.OKUpdate;
                }

                inParams.Clear();
                inParams.Add("@i_ProcessID", ProcessType.Merge, SqlDbType.Char);
                inParams.Add("@i_DepartID", txtDepartID.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_DepartName", txtDepartName.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_DepartInit", txtDepartInit.EditValue.CString(), SqlDbType.VarChar);
                inParams.Add("@i_UseClss", chkDeUseClss.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_Remark", mmeDeRemark.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_InptUser", AppConfig.Login.PersonID, SqlDbType.VarChar);

                SqlHelper.Execute("xp_Depart", inParams, out string[] retVal, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                this.NewButtonClick();
                this.FillGridDepart();
                grvDepart.FindRowByValue("DepartID", retVal[0]);

                SqlHelper.WriteLog(this, logType, startTime);
                MsgBox.OKMessage(message);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 부서 삭제
        /// </summary>
        private void DeleteDepart()
        {
            try
            {
                var field = new Dictionary<string, bool>()
                {
                    { "DepartID", txtDepartID.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionDelete))
                    return;

                var startTime = DateTime.Now;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Delete, SqlDbType.Char);
                inParams.Add("@i_DepartID", txtDepartID.EditValue.CString(), SqlDbType.Char);

                SqlHelper.Execute("xp_Depart", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                this.NewButtonClick();
                this.FillGridDepart();

                SqlHelper.WriteLog(this, ActionLogType.Delete, startTime);
                MsgBox.OKMessage(MessageText.OKDelete);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 부서 조회 : Grid
        /// </summary>
        private void FillGridDepart()
        {
            try
            {
                var startTime = DateTime.Now;

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetList, SqlDbType.Char);

                var table = SqlHelper.GetDataTable("xp_Depart", inParams, out _, out _);
                grcDepart.SetData(table);

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
        /// 부서 조회 : Row
        /// </summary>
        private void ShowDataDepart()
        {
            try
            {
                this.ClearDeaprt();

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetRow, SqlDbType.Char);
                inParams.Add("@i_DepartID", grvDepart.GetFocusedRowCellValue("DepartID").CString());

                var row = SqlHelper.GetDataRow("xp_Depart", inParams, out _, out _);

                if (row != null)
                {
                    txtDepartID.EditValue = row["DepartID"];
                    txtDepartName.EditValue = row["DepartName"];
                    txtDepartInit.EditValue = row["DepartInit"];
                    chkDeUseClss.EditValue = row["UseClss"];
                    mmeDeRemark.EditValue = row["Remark"];
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

        #region 사용자 정의 메서드 - 직위 관리
        private void ClearDuty()
        {
            txtDutyID.EditValue = null;
            txtDutyName.EditValue = null;
            chkDuUseClss.EditValue = "0";
            mmeDuRemark.EditValue = null;
        }

        /// <summary>
        /// 직위 저장
        /// </summary>
        private void SaveDuty()
        {
            try
            {
                var field = new Dictionary<string, bool>()
                {
                    { "DutyID,4", txtDutyID.EditValue.CString() != "" && txtDutyID.EditValue.CString().LengthB() != 4 },
                    { "DutyName", txtDutyName.EditValue.CString() == "" }
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;
                var logType = ActionLogType.Insert;     // 기본 처리 LogType을 입력으로 설정
                var message = MessageText.OKInsert;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Exist, SqlDbType.Char);
                inParams.Add("@i_DutyID", txtDutyID.EditValue.CString(), SqlDbType.Char);

                if (SqlHelper.Exist("xp_Duty", inParams, out _, out _))
                {
                    if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionUpdate))
                        return;

                    logType = ActionLogType.Update;
                    message = MessageText.OKUpdate;
                }

                inParams.Clear();
                inParams.Add("@i_ProcessID", ProcessType.Merge, SqlDbType.Char);
                inParams.Add("@i_DutyID", txtDutyID.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_DutyName", txtDutyName.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_UseClss", chkDuUseClss.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_Remark", mmeDuRemark.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_InptUser", AppConfig.Login.PersonID, SqlDbType.VarChar);

                SqlHelper.Execute("xp_Duty", inParams, out string[] retVal, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                this.NewButtonClick();
                this.FillGridDuty();
                grvDuty.FindRowByValue("DutyID", retVal[0]);

                SqlHelper.WriteLog(this, logType, startTime);
                MsgBox.OKMessage(message);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 직위 삭제
        /// </summary>
        private void DeleteDuty()
        {
            try
            {
                var field = new Dictionary<string, bool>() 
                {
                    { "DutyID", txtDutyID.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionDelete))
                    return;

                var startTime = DateTime.Now;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Delete, SqlDbType.Char);
                inParams.Add("@i_DutyID", txtDutyID.EditValue.CString(), SqlDbType.Char);

                SqlHelper.Execute("xp_Duty", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                NewButtonClick();
                FillGridDuty();

                SqlHelper.WriteLog(this, ActionLogType.Delete, startTime);
                MsgBox.OKMessage(MessageText.OKDelete);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 직위 조회 : Grid
        /// </summary>
        /// <param name="logFlag"></param>
        private void FillGridDuty()
        {
            try
            {
                var startTime = DateTime.Now;

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetList, SqlDbType.Char);

                var table = SqlHelper.GetDataTable("xp_Duty", inParams, out _, out _);
                grcDuty.SetData(table);

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
        /// 직위 조회 : Row
        /// </summary>
        private void ShowDataDuty()
        {
            try
            {
                this.ClearDuty();

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetRow, SqlDbType.Char);
                inParams.Add("@i_DutyID", grvDuty.GetFocusedRowCellValue("DutyID").CString());

                var row = SqlHelper.GetDataRow("xp_Duty", inParams, out _, out _);

                if (row != null)
                {
                    txtDutyID.EditValue = row["DutyID"];
                    txtDutyName.EditValue = row["DutyName"];
                    chkDuUseClss.EditValue = row["UseClss"];
                    mmeDuRemark.EditValue = row["Remark"];
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