using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using DSLibrary;

namespace MasterMES.FORMS.SYSTEM
{
    public partial class frmMiddleCode : frmBase, CLASSES.IButtonAction
    {
        #region 생성자
        public frmMiddleCode()
        {
            InitializeComponent();

            txtMiddleCode.Properties.MaxLength = 7;
        }

        #endregion

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
                // Wait폼 출력 및 초기화버튼을 비활성
                Utils.ShowWaitForm(this);
                MainButton.InitButton = false;

                // 초기화에 필요한 데이터를 조회
                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Initial, SqlDbType.Char);

                var initSet = SqlHelper.GetDataSet("xp_MIddleCode", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                // 폼의 컨트롤 초기화 설정
                gleLargeCode.SetData(initSet.Tables[0]);
                tlsCode.SetColumns(initSet.Tables[1], "UseClss");

                this.NewButtonClick();
                this.ViewCodeTreeList();
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
            DxControl.ClearControl(lcgCode);
            txtMiddleCode.Focus();
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
                    { "LargeCode", gleLargeCode.EditValue.CString() == "" },
                    { "MiddleCode", txtMiddleCode.EditValue.CString() == "" },
                    { "MiddleName", txtMiddleName.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;
                var logType = ActionLogType.Insert;
                var message = MessageText.OKInsert;


                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Exist, SqlDbType.Char);
                inParams.Add("@i_MiddleCode", $"{gleLargeCode.EditValue}{txtMiddleCode.EditValue}", SqlDbType.VarChar);

                if (SqlHelper.Exist("xp_MiddleCode", inParams, out _, out _))
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
                inParams.Add("@i_MiddleCode", $"{gleLargeCode.EditValue}{txtMiddleCode.EditValue}", SqlDbType.VarChar);
                inParams.Add("@i_MiddleName", txtMiddleName.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_LargeCode", gleLargeCode.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_UserDefVal1", txtUserDefVal1.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_UserDefVal2", txtUserDefVal2.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_UserDefVal3", txtUserDefVal3.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_UseClss", chkUseClss.EditValue.CString(), SqlDbType.VarChar);
                inParams.Add("@i_InptUser", AppConfig.Login.PersonID, SqlDbType.NVarChar);

                SqlHelper.Execute("xp_MiddleCode", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                var middleCode = $"{gleLargeCode.EditValue}{txtMiddleCode.EditValue}";
                this.NewButtonClick();
                this.ViewCodeTreeList();
                tlsCode.FindNodeByValue(middleCode);

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
                    { "MiddleCode", txtMiddleCode.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;

                if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionDelete))
                    return;

                Utils.ShowWaitForm(this);
                MainButton.DeleteButton = false;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Delete, SqlDbType.Char);
                inParams.Add("@i_MiddleCode", $"{gleLargeCode.EditValue}{txtMiddleCode.EditValue}", SqlDbType.VarChar);

                SqlHelper.Execute("xp_MiddleCode", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                this.NewButtonClick();
                this.ViewCodeTreeList();

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
        /// <summary>
        /// 0레벨 노드 Font Style 변경
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tlsCode_NodeCellStyle(object sender, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            if (e.Node.Level == 0)
            {
                e.Appearance.ForeColor = Color.DeepSkyBlue;
                e.Appearance.FontStyleDelta = FontStyle.Bold;
            }
        }

        /// <summary>
        /// TreeList 활성화 Node 변경시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tlsCode_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            DxControl.ClearControl(lcgCode);

            if (e.Node.Level == 0)
                gleLargeCode.EditValue = e.Node["CodeID"].CString();
            else
                this.ViewCodeRow($"{e.Node.ParentNode["CodeID"]}{e.Node["CodeID"]}");
        }

        private void tlsCode_RowClick(object sender, DevExpress.XtraTreeList.RowClickEventArgs e)
        {
            DxControl.ClearControl(lcgCode);

            if (e.Node.Level == 0)
                gleLargeCode.EditValue = e.Node["CodeID"].CString();
            else
                this.ViewCodeRow($"{e.Node.ParentNode["CodeID"]}{e.Node["CodeID"]}");
        }

        #endregion

        #region 사용자 정의 메서드
        /// <summary>
        /// 공통코드 조회 : TreeList
        /// </summary>
        private void ViewCodeTreeList()
        {
            try
            {
                var startTime = DateTime.Now;

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetList, SqlDbType.Char);

                var table = SqlHelper.GetDataTable("xp_MiddleCode", inParams, out _, out _);
                tlsCode.SetData(table, 0);

                SqlHelper.WriteLog(this, ActionLogType.Select, startTime);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
            finally
            {
                // Wait 폼을 종료
                Utils.CloseWaitForm();
            }
        }

        /// <summary>
        /// 선택된 중분류 정보를 화면에 표시
        /// </summary>
        /// <param name="middleCode"></param>
        private void ViewCodeRow(string middleCode)
        {
            try
            {
                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetRow, SqlDbType.Char);
                inParams.Add("@i_MiddleCode", middleCode);

                var row = SqlHelper.GetDataRow("xp_MiddleCode", inParams, out _, out _);

                if (row != null)
                {
                    txtMiddleCode.EditValue = row["MiddleCode"].CString().Substring(3);
                    txtMiddleName.EditValue = row["MiddleName"];
                    gleLargeCode.EditValue = row["LargeCode"];
                    txtUserDefVal1.EditValue = row["UserDefVal1"];
                    txtUserDefVal2.EditValue = row["UserDefVal2"];
                    txtUserDefVal3.EditValue = row["UserDefVal3"];
                    chkUseClss.EditValue = row["UseClss"];
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