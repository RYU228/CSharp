using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DSLibrary;

namespace MasterMES.FORMS.SYSTEM
{
    public partial class frmMenu : frmBase, CLASSES.IButtonAction
    {
        #region 생성자
        public frmMenu()
        {
            InitializeComponent();
        }

        #endregion

        #region 오버라이드 이벤트
        //protected override void OnActivated(EventArgs e)
        //{
        //    base.OnActivated(e);
        //    MainButton.ActiveButton();
        //    MainButton.ActiveButton("INIT,NEW,SAVE,DELETE");
        //}

        //protected override void OnDeactivate(EventArgs e)
        //{
        //    base.OnDeactivate(e);
        //    //MainButton.ActiveButton();
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
                var initSet = SqlHelper.GetDataSet("xp_Menu", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                gleMenuLevel.SetData(initSet.Tables[0]);
                tlsMenu.SetColumns(initSet.Tables[1], "UseClss", false, false);

                // QueryPopUp 이벤트가 발생하지 않은 상태에서 TreeList의 노드를 클릭할 경우 ParentID 값이 표시되지 않음
                // 그래서 최초 한번은 강제로 데이터를 설정함 => 즉 상위 메뉴값을 컨트롤에 설정함
                this.SetDataParentID();

                this.NewButtonClick();
                this.ViewMainMenuTreeList();
            }
            catch (Exception ex)
            {
                var text = MessageText.OKInsert;

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
            DxControl.ClearControl(lcgMenu);
            txtMenuID.Focus();
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
                    { "MenuID", txtMenuID.EditValue.CString() == "" },
                    { "ParentID", gleParentID.EditValue.CString() == "" },
                    { "MenuTitle", txtMenuID.EditValue.CString() == "" },
                    { "MenuLevel", gleMenuLevel.EditValue.CString() == ""},
                    { "MenuPath", gleMenuLevel.EditValue.CString() == "2" && txtMenuPath.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;
                var processId = ProcessType.Insert;     // 기본 처리 processId를 입력으로 설정
                var logType = ActionLogType.Insert;     // 기본 처리 LogType을 입력으로 설정
                var message = MessageText.OKInsert;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Exist, SqlDbType.Char);
                inParams.Add("@i_MenuID", txtMenuID.EditValue.CString());

                if (SqlHelper.Exist("xp_Menu", inParams, out _, out _))
                {
                    if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionUpdate))
                        return;

                    processId = ProcessType.Update;
                    logType = ActionLogType.Update;
                    message = MessageText.OKUpdate;
                }

                Utils.ShowWaitForm(this);
                MainButton.SaveButton = false;

                inParams = new DbParamList();
                inParams.Add("@i_ProcessID", processId, SqlDbType.Char);
                inParams.Add("@i_MenuID", txtMenuID.EditValue.CString());
                inParams.Add("@i_ParentID", gleParentID.EditValue.CString());
                inParams.Add("@i_MenuTitle", txtMenuTitle.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_MenuLevel", gleMenuLevel.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_MenuGroup", txtMenuGroup.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_MenuPath", txtMenuPath.EditValue.CString());
                inParams.Add("@i_UseClss", chkUseClss.EditValue.CString());

                SqlHelper.Execute("xp_Menu", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                var menuID = txtMenuID.EditValue.CString();
                this.NewButtonClick();
                this.ViewMainMenuTreeList();
                tlsMenu.FindNodeByValue(menuID);

                // 로그를 기록합니다
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
                    { "MenuID", txtMenuID.EditValue.CString() == "" },
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
                inParams.Add("@i_MenuID", txtMenuID.EditValue.CString());

                SqlHelper.Execute("xp_Menu", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                this.NewButtonClick();
                this.ViewMainMenuTreeList();

                // 로그를 기록합니다
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

        /// <summary>
        /// 복사 버튼
        /// </summary>
        public void CopyButtonClick()
        {
        }

        #endregion

        #region 폼 컨트롤 이벤트
        /// <summary>
        /// 상위 메뉴의 경우 신규 등록된 데이터를 보여주기 위해서 Popup 이벤트에서 처리한다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gleParentID_QueryPopUp(object sender, CancelEventArgs e)
        {
            this.SetDataParentID();
        }

        /// <summary>
        /// Node 변경시 신규 Node의 정보를 화면에 표시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tlsMenu_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            this.ViewMenuRow(e.Node["MenuID"].CString());
        }

        private void tlsMenu_RowClick(object sender, DevExpress.XtraTreeList.RowClickEventArgs e)
        {
            this.ViewMenuRow(e.Node["MenuID"].CString());
        }

        /// <summary>
        /// 0 레벨 노드 색상 변경
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tlsMenu_NodeCellStyle(object sender, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            if (e.Node.Level == 0)
            {
                e.Appearance.ForeColor = Color.DeepSkyBlue;
                e.Appearance.FontStyleDelta = FontStyle.Bold;
            }
        }

        #endregion

        #region 사용자 정의 메서드
        /// <summary>
        /// TreeList에 메뉴정보를 조회합니다.
        /// </summary>
        /// <param name="logFlag"></param>
        private void ViewMainMenuTreeList()
        {
            try
            {
                var startTime = DateTime.Now;

                // Wait폼을 호출합니다.
                Utils.ShowWaitForm(this);

                // 조회조건 파라메터를 생성합니다.(그리드 조회용)
                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetList, SqlDbType.Char);

                // 데이터를 조회합니다.
                var table = SqlHelper.GetDataTable("xp_Menu", inParams, out _, out _);

                // 그리드에 데이터를 디스플레이합니다.
                tlsMenu.SetData(table, 0);

                SqlHelper.WriteLog(this, ActionLogType.Select, startTime);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
            finally
            {
                // Wait 폼을 종료합니다.
                Utils.CloseWaitForm();
            }
        }

        /// <summary>
        /// 선택된 노드의 정보를 화면에 표시합니다.
        /// </summary>
        /// <param name="menuID"></param>
        private void ViewMenuRow(string menuID)
        {
            try
            {
                DxControl.ClearControl(lcgMenu);

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetRow, SqlDbType.Char);
                inParams.Add("@i_MenuID", menuID);

                var row = SqlHelper.GetDataRow("xp_Menu", inParams, out _, out _);

                if (row != null)
                {
                    txtMenuID.EditValue = row["MenuID"];
                    gleParentID.EditValue = row["ParentID"];
                    txtMenuTitle.EditValue = row["MenuTitle"];
                    gleMenuLevel.EditValue = row["MenuLevel"];
                    txtMenuGroup.EditValue = row["MenuGroup"];
                    txtMenuPath.EditValue = row["MenuPath"];
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

        /// <summary>
        /// 상위 메뉴 ID 정보 조회
        /// </summary>
        private void SetDataParentID()
        {
            try
            {
                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", "81", SqlDbType.Char);

                var table = SqlHelper.GetDataTable("xp_Menu", inParams, out _, out _);
                gleParentID.SetData(table, "MenuLevel");
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        #endregion
    }
}