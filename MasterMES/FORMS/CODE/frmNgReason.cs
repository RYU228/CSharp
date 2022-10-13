using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using DSLibrary;

namespace MasterMES.FORMS.CODE
{
    public partial class frmNgReason : frmBase, CLASSES.IButtonAction
    {
        #region 생성자
        public frmNgReason()
        {
            InitializeComponent();
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

                var initSet = SqlHelper.GetDataSet("xp_NgRsn", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                tlsNgReason.SetColumns(initSet.Tables[0], autoColumnWidth:true);

                this.ClearNgRsn();
                this.ClearNgRsnSub();

                this.FillTreeNgReason();

                tacNgReason.SelectedTabPage = tapNgRsn;
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
            if (tacNgReason.SelectedTabPage == tapNgRsn)
            {
                this.ClearNgRsn();
                txtNgRsnName.Focus();
            }
            else if (tacNgReason.SelectedTabPage == tapNgRsnSub)
            {
                this.ClearNgRsnSub();
                txtNgRsnSubName.Focus();
            }
        }

        /// <summary>
        /// 저장 버튼
        /// </summary>
        public void SaveButtonClick()
        {
            if (tacNgReason.SelectedTabPage == tapNgRsn)
                this.SaveNgRsn();
            else if (tacNgReason.SelectedTabPage == tapNgRsnSub)
                this.SaveNgRsnSub();
        }

        /// <summary>
        /// 복사 버튼
        /// </summary>
        public void CopyButtonClick()
        {
        }

        /// <summary>
        /// 삭제 버튼
        /// </summary>
        public void DeleteButtonClick()
        {
            if (tacNgReason.SelectedTabPage == tapNgRsn)
                this.DeleteNgRsn();
            else if (tacNgReason.SelectedTabPage == tapNgRsnSub)
                this.DeleteNgRsnSub();
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
        /// 활성화 노드 변경시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tlsNgReason_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            this.ShowData(e.Node.Level, e.Node["KeyField"].CString());
        }

        /// <summary>
        /// 노드 클릭시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tlsNgReason_RowClick(object sender, DevExpress.XtraTreeList.RowClickEventArgs e)
        {
            this.ShowData(e.Node.Level, e.Node["KeyField"].CString());
        }

        #endregion

        #region 사용자 정의 메서드 - 불량항목
        /// <summary>
        /// 불량항목 입력란 초기화
        /// </summary>
        private void ClearNgRsn()
        {
            txtNgRsnID.EditValue = null;
            txtNgRsnName.EditValue = null;
            chkRUseClss.EditValue = "0";
            mmeRRemark.EditValue = null;
        }

        /// <summary>
        /// 불량항목 저장
        /// </summary>
        private void SaveNgRsn()
        {
            try
            {
                var field = new Dictionary<string, bool>()
                {
                    { "NgRnsName", txtNgRsnName.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;
                var logType = ActionLogType.Insert;
                var message = MessageText.OKInsert;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Exist, SqlDbType.Char);
                inParams.Add("@i_NgRsnID", txtNgRsnID.EditValue.CString(), SqlDbType.Char);

                if (SqlHelper.Exist("xp_NgRsn", inParams, out _, out _))
                {
                    if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionUpdate))
                        return;

                    logType = ActionLogType.Update;
                    message = MessageText.OKUpdate;
                }

                inParams.Clear();
                inParams.Add("@i_ProcessID", ProcessType.Merge, SqlDbType.Char);
                inParams.Add("@i_NgRsnID", txtNgRsnID.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_NgRsnName", txtNgRsnName.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_UseClss", chkRUseClss.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_Remark", mmeRRemark.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_InptUser", AppConfig.Login.PersonID, SqlDbType.VarChar);

                SqlHelper.Execute("xp_NgRsn", inParams, out string[] retVal, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                this.ClearNgRsn();
                this.FillTreeNgReason();
                tlsNgReason.FindNodeByValue(retVal[0]);

                SqlHelper.WriteLog(this, logType, startTime);
                MsgBox.OKMessage(message);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 불량항목 삭제
        /// </summary>
        private void DeleteNgRsn()
        {
            try
            {
                var field = new Dictionary<string, bool>()
                {
                    { "NgRsnID", txtNgRsnID.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;

                if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionDelete))
                    return;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Delete, SqlDbType.Char);
                inParams.Add("@i_NgRsnID", txtNgRsnID.EditValue.CString(), SqlDbType.Char);

                SqlHelper.Execute("xp_NgRsn", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                this.ClearNgRsn();
                this.FillTreeNgReason();

                SqlHelper.WriteLog(this, ActionLogType.Delete, startTime);
                MsgBox.OKMessage(MessageText.OKDelete);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 불량항목 조회 (1개 행)
        /// </summary>
        /// <param name="ngRsnID"></param>
        private void ShowDataNgRsn(string ngRsnID)
        {
            try
            {
                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetRow, SqlDbType.Char);
                inParams.Add("@i_NgRsnID", ngRsnID, SqlDbType.Char);

                var row = SqlHelper.GetDataRow("xp_NgRsn", inParams, out _, out _);

                if (row != null)
                {
                    txtNgRsnID.EditValue = row["NgRsnID"];
                    txtNgRsnName.EditValue = row["NgRsnName"];
                    chkRUseClss.EditValue = row["UseClss"];
                    mmeRRemark.EditValue = row["Remark"];

                    // 하위 데이터 처리를 위해서 상위값 출력
                    txtSNgRsnID.EditValue = $"{row["NgRsnID"]} : {row["NgRsnName"]}";
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

        #region 사용자 정의 메서드 - 불량내용
        /// <summary>
        /// 불량내용 입력란 초기화
        /// </summary>
        private void ClearNgRsnSub()
        {
            txtNgRsnSubID.EditValue = null;
            txtNgRsnSubName.EditValue = null;
            chkSUseClss.EditValue = "0";
            mmeSRemark.EditValue = null;
        }

        /// <summary>
        /// 불량내용 저장
        /// </summary>
        private void SaveNgRsnSub()
        {
            try
            {
                var field = new Dictionary<string, bool>()
                {
                    { "NgRsnID", txtSNgRsnID.EditValue.CString() == "" },
                    { "NgRsnSubName", txtNgRsnSubName.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;
                var logType = ActionLogType.Insert;
                var message = MessageText.OKInsert;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Exist, SqlDbType.Char);
                inParams.Add("@i_NgRsnID", txtSNgRsnID.EditValue.CString().Substring(0, 2), SqlDbType.Char);
                inParams.Add("@i_NgRsnSubID", txtNgRsnSubID.EditValue.CString(), SqlDbType.Char);

                if (SqlHelper.Exist("xp_NgRsnSub", inParams, out _, out _))
                {
                    if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionUpdate))
                        return;

                    logType = ActionLogType.Update;
                    message = MessageText.OKUpdate;
                }

                inParams.Clear();
                inParams.Add("@i_ProcessID", ProcessType.Merge, SqlDbType.Char);
                inParams.Add("@i_NgRsnID", txtSNgRsnID.EditValue.CString().Substring(0, 2), SqlDbType.Char);
                inParams.Add("@i_NgRsnSubID", txtNgRsnSubID.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_NgRsnSubName", txtNgRsnSubName.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_UseClss", chkSUseClss.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_Remark", mmeSRemark.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_InptUser", AppConfig.Login.PersonID, SqlDbType.VarChar);

                SqlHelper.Execute("xp_NgRsnSub", inParams, out string[] retVal, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                var findKey = txtSNgRsnID.EditValue.CString().Substring(0, 2) + retVal[0];
                this.ClearNgRsnSub();

                this.FillTreeNgReason();
                tlsNgReason.FindNodeByValue(findKey);

                SqlHelper.WriteLog(this, logType, startTime);
                MsgBox.OKMessage(message);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 불량내용 삭제
        /// </summary>
        private void DeleteNgRsnSub()
        {
            try
            {
                var field = new Dictionary<string, bool>()
                {
                    { "NgRsnID", txtSNgRsnID.EditValue.CString() == "" },
                    { "NgRsnSubID", txtNgRsnSubID.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;

                if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionDelete))
                    return;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Delete, SqlDbType.Char);
                inParams.Add("@i_NgRsnID", txtSNgRsnID.EditValue.CString().Substring(0, 2), SqlDbType.Char);
                inParams.Add("@i_NgRsnSubID", txtNgRsnSubID.EditValue.CString(), SqlDbType.Char);

                SqlHelper.Execute("xp_NgRsnSub", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                this.ClearNgRsn();
                this.FillTreeNgReason();

                SqlHelper.WriteLog(this, ActionLogType.Delete, startTime);
                MsgBox.OKMessage(MessageText.OKDelete);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CDateString());
            }
        }

        /// <summary>
        /// 불량내용 조회 (1개 행)
        /// </summary>
        /// <param name="ngRsnID"></param>
        /// <param name="ngRsnSubID"></param>
        private void ShowDataNgRsnSub(string ngRsnID, string ngRsnSubID)
        {
            try
            {
                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetRow, SqlDbType.Char);
                inParams.Add("@i_NgRsnID", ngRsnID, SqlDbType.Char);
                inParams.Add("@i_NgRsnSubID", ngRsnSubID, SqlDbType.Char);

                var row = SqlHelper.GetDataRow("xp_NgRsnSub", inParams, out _, out _);

                if (row != null)
                {
                    txtSNgRsnID.EditValue = row["NgRsnID"];             // ID + Name
                    txtNgRsnSubID.EditValue = row["NgRsnSubID"];
                    txtNgRsnSubName.EditValue = row["NgRsnSubName"];
                    chkSUseClss.EditValue = row["UseClss"];
                    mmeSRemark.EditValue = row["Remark"];
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

        #region 사용자 정의 메서드 - Fill TreeList, ShowData
        /// <summary>
        /// 트리리스트 조회 (NgRsn - NgRsnSub)
        /// </summary>
        private void FillTreeNgReason()
        {
            try
            {
                var startTime = DateTime.Now;

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetList, SqlDbType.Char);

                var table = SqlHelper.GetDataTable("xp_NgRsn", inParams, out _, out _);

                tlsNgReason.SetData(table, 0);

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
        /// 선택된 TreeList의 Level에 따라 데이터를 표시함
        /// </summary>
        /// <param name="level"></param>
        /// <param name="keyValue"></param>
        private void ShowData(int level, string keyValue)
        {
            this.ClearNgRsn();

            txtSNgRsnID.EditValue = null;
            this.ClearNgRsnSub();

            switch (level)
            {
                case 0:
                    this.ShowDataNgRsn(keyValue.Substring(0, 2));
                    tacNgReason.SelectedTabPage = tapNgRsn;
                    break;

                case 1:
                    this.ShowDataNgRsnSub(keyValue.Substring(0, 2), keyValue.Substring(2, 2));
                    tacNgReason.SelectedTabPage = tapNgRsnSub;
                    break;
            }
        }
        #endregion
    }
}