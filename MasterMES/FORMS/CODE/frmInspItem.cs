using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using DSLibrary;
using MasterMES.CLASSES;

namespace MasterMES.FORMS.CODE
{
    public partial class frmInspItem : frmBase, IButtonAction
    {
        #region 생성자
        public frmInspItem()
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

        #region 인터페이스 - 공용버튼 클릭
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

                var initSet = SqlHelper.GetDataSet("xp_InspItem", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                tlsInspItem.SetColumns(initSet.Tables[0], autoColumnWidth:true);

                this.ClearInsp();
                this.ClearInspSub();

                this.FillTreeInspItem();

                tacInspItem.SelectedTabPage = tapInsp;
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
            if (tacInspItem.SelectedTabPage == tapInsp)
            {
                this.ClearInsp();
                txtInspName.Focus();
            }
            else if (tacInspItem.SelectedTabPage == tapInspSub)
            {
                this.ClearInspSub();
                txtInspSubName.Focus();
            }
        }

        /// <summary>
        /// 저장 버튼
        /// </summary>
        public void SaveButtonClick()
        {
            if (tacInspItem.SelectedTabPage == tapInsp)
                this.SaveInsp();
            else if (tacInspItem.SelectedTabPage == tapInspSub)
                this.SaveInspSub();
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
            if (tacInspItem.SelectedTabPage == tapInsp)
                this.DeleteInsp();
            else if (tacInspItem.SelectedTabPage == tapInspSub)
                this.DeleteInspSub();
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
        /// <param name="reportIndex"></param>
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
        private void tlsInspItem_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            this.ShowData(e.Node.Level, e.Node["KeyField"].CString());
        }

        /// <summary>
        /// 노트 클릭시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tlsInspItem_RowClick(object sender, DevExpress.XtraTreeList.RowClickEventArgs e)
        {
            this.ShowData(e.Node.Level, e.Node["KeyField"].CString());
        }

        #endregion

        #region 사용자 정의 메서드 - 검사항목
        /// <summary>
        /// 검사항목 입력란 초기화
        /// </summary>
        private void ClearInsp()
        {
            txtInspID.EditValue = null;
            txtInspName.EditValue = null;
            chkUseClss.EditValue = "0";
            mmeRemark.EditValue = null;
        }

        /// <summary>
        /// 검사항목 저장
        /// </summary>
        private void SaveInsp()
        {
            try
            {
                var field = new Dictionary<string, bool>()
                {
                    { "InspName", txtInspName.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;
                var logType = ActionLogType.Insert;
                var message = MessageText.OKInsert;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Exist, SqlDbType.Char);
                inParams.Add("@i_InspID", txtInspID.EditValue.CString(), SqlDbType.Char);

                if (SqlHelper.Exist("xp_InspItem", inParams, out _, out _))
                {
                    if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionUpdate))
                        return;

                    logType = ActionLogType.Update;
                    message = MessageText.OKUpdate;
                }

                inParams.Clear();
                inParams.Add("@i_ProcessID", ProcessType.Merge, SqlDbType.Char);
                inParams.Add("@i_InspID", txtInspID.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_InspName", txtInspName.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_UseClss", chkUseClss.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_Remark", mmeRemark.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_InptUser", AppConfig.Login.PersonID, SqlDbType.VarChar);

                SqlHelper.Execute("xp_InspItem", inParams, out string[] retVal, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                this.ClearInsp();
                this.FillTreeInspItem();
                tlsInspItem.FindNodeByValue(retVal[0]);

                SqlHelper.WriteLog(this, logType, startTime);
                MsgBox.OKMessage(message);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 검사항목 삭제
        /// </summary>
        private void DeleteInsp()
        {
            try
            {
                var field = new Dictionary<string, bool>()
                {
                    { "InspID", txtInspID.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;

                if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionDelete))
                    return;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Delete, SqlDbType.Char);
                inParams.Add("@i_InspID", txtInspID.EditValue.CString(), SqlDbType.Char);

                SqlHelper.Execute("xp_InspItem", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                this.ClearInsp();
                this.FillTreeInspItem();

                SqlHelper.WriteLog(this, ActionLogType.Delete, startTime);
                MsgBox.OKMessage(MessageText.OKDelete);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 검사항목 조회(1개 행)
        /// </summary>
        /// <param name="inspID"></param>
        private void ShowDataInsp(string inspID)
        {
            try
            {
                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetRow, SqlDbType.Char);
                inParams.Add("@i_InspID", inspID, SqlDbType.Char);

                var row = SqlHelper.GetDataRow("xp_InspItem", inParams, out _, out _);

                if (row != null)
                {
                    txtInspID.EditValue = row["InspID"];
                    txtInspName.EditValue = row["InspName"];
                    chkUseClss.EditValue = row["UseClss"];
                    mmeRemark.EditValue = row["Remark"];

                    // 하위 데이터 처리를 위해서 상위값 출력
                    txtSInspID.EditValue = $"{row["InspID"]} : {row["InspName"]}";
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

        #region 사용자 정의 메서드 - 검사내용
        /// <summary>
        /// 검사내용 입력란 초기화
        /// </summary>
        private void ClearInspSub()
        {
            txtInspSubID.EditValue = null;
            txtInspSubName.EditValue = null;
            txtSpec.EditValue = null;
            chkSUseClss.EditValue = "0";
            mmeSRemark.EditValue = null;
        }

        /// <summary>
        /// 검사내용 저장
        /// </summary>
        private void SaveInspSub()
        {
            try
            {
                var field = new Dictionary<string, bool>()
                {
                    { "InspID", txtSInspID.EditValue.CString() == "" },
                    { "InspSubName", txtInspSubName.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;
                var logType = ActionLogType.Insert;
                var message = MessageText.OKInsert;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Exist, SqlDbType.Char);
                inParams.Add("@i_InspID", txtSInspID.EditValue.CString().Substring(0, 2), SqlDbType.Char);
                inParams.Add("@i_InspSubID", txtInspSubID.EditValue.CString(), SqlDbType.Char);

                if (SqlHelper.Exist("xp_InspItemSub", inParams, out _, out _))
                {
                    if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionUpdate))
                        return;

                    logType = ActionLogType.Update;
                    message = MessageText.OKUpdate;
                }

                inParams.Clear();
                inParams.Add("@i_ProcessID", ProcessType.Merge, SqlDbType.Char);
                inParams.Add("@i_InspID", txtSInspID.EditValue.CString().Substring(0, 2), SqlDbType.Char);
                inParams.Add("@i_InspSubID", txtInspSubID.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_InspSubName", txtInspSubName.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_Spec", txtSpec.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_UseClss", chkSUseClss.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_Remark", mmeSRemark.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_InptUser", AppConfig.Login.PersonID, SqlDbType.VarChar);

                SqlHelper.Execute("xp_InspItemSub", inParams, out string[] retVal, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                var findKey = txtSInspID.EditValue.CString().Substring(0, 2) + retVal[0];
                this.ClearInspSub();

                this.FillTreeInspItem();
                tlsInspItem.FindNodeByValue(findKey);

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
        private void DeleteInspSub()
        {
            try
            {
                var field = new Dictionary<string, bool>()
                {
                    { "InspID", txtSInspID.EditValue.CString() == "" },
                    { "InspSubID", txtInspSubID.EditValue.CString() == "" }
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;

                if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionDelete))
                    return;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Delete, SqlDbType.Char);
                inParams.Add("@i_InspID", txtSInspID.EditValue.CString().Substring(0, 2), SqlDbType.Char);
                inParams.Add("@i_InspSubID", txtInspSubID.EditValue.CString(), SqlDbType.Char);

                SqlHelper.Execute("xp_InspItemSub", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                this.ClearInspSub();
                this.FillTreeInspItem();

                SqlHelper.WriteLog(this, ActionLogType.Delete, startTime);
                MsgBox.OKMessage(MessageText.OKDelete);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 검사내용 조회(1개 행)
        /// </summary>
        /// <param name="inspID"></param>
        /// <param name="inspSubID"></param>
        private void ShowDataInspSub(string inspID, string inspSubID)
        {
            try
            {
                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetRow, SqlDbType.Char);
                inParams.Add("@i_InspID", inspID, SqlDbType.Char);
                inParams.Add("@i_InspSubID", inspSubID, SqlDbType.Char);

                var row = SqlHelper.GetDataRow("xp_InspItemSub", inParams, out _, out _);

                if (row != null)
                {
                    txtSInspID.EditValue = row["InspID"];           // ID + Name
                    txtInspSubID.EditValue = row["InspSubID"];
                    txtInspSubName.EditValue = row["InspSubName"];
                    txtSpec.EditValue = row["Spec"];
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

        #region 사용자 정의 메서드 - FillTreeList, ShowData
        /// <summary>
        /// 검사항목/내용 조회(TreeList)
        /// </summary>
        private void FillTreeInspItem()
        {
            try
            {
                var startTime = DateTime.Now;

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("i_ProcessID", ProcessType.GetList, SqlDbType.Char);

                var table = SqlHelper.GetDataTable("xp_InspItem", inParams, out _, out _);

                tlsInspItem.SetData(table, 0);

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
            this.ClearInsp();

            txtSInspID.EditValue = null;
            this.ClearInspSub();

            switch (level)
            {
                case 0:
                    this.ShowDataInsp(keyValue.Substring(0, 2));
                    tacInspItem.SelectedTabPage = tapInsp;
                    break;

                case 1:
                    this.ShowDataInspSub(keyValue.Substring(0, 2), keyValue.Substring(2, 2));
                    tacInspItem.SelectedTabPage = tapInspSub;
                    break;
            }
        }
        #endregion
    }
}