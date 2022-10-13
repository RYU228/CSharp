using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DSLibrary;

namespace MasterMES.FORMS.CODE
{
    public partial class frmUserMenu : frmBase, CLASSES.IButtonAction
    {
        #region 생성자
        public frmUserMenu()
        {
            InitializeComponent();

            lcgUserMenu.SetHeaderButtons("CheckAll,UnCheckAll,CheckArea,UnCheckArea");

            grvUserMenu.OptionsSelection.MultiSelect = true;
            grvUserMenu.OptionsView.AllowCellMerge = true;

            // 체크 입력란에 Sort을 허용할 경우, 특정 컬럼이 정렬기준으로 설정되어 있다면, 그 컬럼 체크 값이 변경될때 마다 그리드의 정렬순서도 변경되어 행이 이동함
            grvUserMenu.OptionsCustomization.AllowSort = false;
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
                var initSet = SqlHelper.GetDataSet("xp_UserMenu", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                grcPerson.SetColumns(initSet.Tables[0], "UseClss");
                grcUserMenu.SetColumns(initSet.Tables[1]);

                // 사용권한
                var riMenuClss = grvUserMenu.SetRepositoryItemCheckEdit("MenuClss");
                riMenuClss.EditValueChanged += RiMenuClss_EditValueChanged;

                // 저장권한
                var riSaveClss = grvUserMenu.SetRepositoryItemCheckEdit("SaveClss");
                riSaveClss.EditValueChanged += RiSaveClss_EditValueChanged;

                // 삭제권한
                var riDeleteClss = grvUserMenu.SetRepositoryItemCheckEdit("DeleteClss");
                riDeleteClss.EditValueChanged += RiDeleteClss_EditValueChanged;

                // 출력권한
                var riPrintClss = grvUserMenu.SetRepositoryItemCheckEdit("PrintClss");
                riPrintClss.EditValueChanged += RiPrintClss_EditValueChanged;

                // 그리드 초기 형식 저장
                DxControl.SaveGridFormat(grvPerson, GridFormat.Default);
                DxControl.SaveGridFormat(grvUserMenu, GridFormat.Default);

                this.NewButtonClick();
                this.FillGridPerson();
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.Message);
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
            grcUserMenu.DataSource = null;
        }

        /// <summary>
        /// 저장 버튼
        /// </summary>
        public void SaveButtonClick()
        {
            try
            {
                // 체크된 권한이 있는지 확인한다.
                var rows = (grcUserMenu.DataSource == null) ?
                    null :
                    (grcUserMenu.DataSource as DataTable).DefaultView.ToTable()
                        .Select("MenuClss = '0' OR SaveClss = '0' OR DeleteClss = '0' OR PrintClss = '0'");

                var field = new Dictionary<string, bool>()
                {
                    { "Person Info", grvPerson.FocusedRowHandle < 0 },
                    { "UserMenu Info", rows == null || rows.Length == 0 },
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;
                var logType = ActionLogType.Insert;
                var message = MessageText.OKInsert;
                var personId = grvPerson.GetFocusedRowCellValue("PersonID").CString();

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Exist, SqlDbType.Char);
                inParams.Add("@i_PersonID", personId);

                if (SqlHelper.Exist("xp_UserMenu", inParams, out _, out _))
                {
                    if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionUpdate))
                        return;

                    logType = ActionLogType.Update;
                    message = MessageText.OKUpdate;
                }

                Utils.ShowWaitForm(this);
                MainButton.SaveButton = false;

                var xmlData = (grcUserMenu.DataSource as DataTable).DefaultView
                               .ToTable(false, new string[] { "MenuID", "MenuClss", "SaveClss", "DeleteClss", "PrintClss" })
                               .ToXml();
                ////.Select("SaveClss = 0 OR DeleteClss = 0 OR SelectClss = 0 OR PrintClss = 0")
                ////.CopyToDataTable().ToXml();

                inParams.Clear();
                inParams.Add("@i_ProcessID", ProcessType.Merge, SqlDbType.Char);
                inParams.Add("@i_PersonID", personId);
                inParams.Add("@i_XmlData", xmlData);
                inParams.Add("@i_InptUser", AppConfig.Login.PersonID);

                SqlHelper.Execute("xp_UserMenu", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                this.FillGridUserMenu();

                // 화면 단위 건수는 아님, 전체 데이터가 저장(최초)/수정된 건수
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
                    { "Person Info", grvPerson.FocusedRowHandle < 0 },
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
                inParams.Add("@i_PersonID", grvPerson.GetFocusedRowCellValue("PersonID").CString());

                SqlHelper.Execute("xp_UserMenu", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                this.NewButtonClick();
                this.FillGridUserMenu();

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
        /// 사원 선택에 따른 권한 정보 표시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvPerson_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle >= 0)
                this.FillGridUserMenu();
        }

        private void grvPerson_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            this.FillGridUserMenu();
        }

        /// <summary>
        /// 메뉴 그룹 셀 병합
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvUserMenu_CellMerge(object sender, CellMergeEventArgs e)
        {
            if (e.Column.FieldName == "MenuGroup")
            {
                var view = sender as GridView;
                var val1 = view.GetRowCellDisplayText(e.RowHandle1, "MenuGroup").CString();
                var val2 = view.GetRowCellDisplayText(e.RowHandle2, "MenuGroup").CString();

                e.Merge = (val1 == val2);
            }
            else
            {
                e.Column.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            }

            e.Handled = true;
        }

        #endregion

        #region RepositoryItem 이벤트
        /// <summary>
        /// 사원 선택
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RiPersonChk_EditValueChanged(object sender, EventArgs e)
        {
            if (grvPerson.PostEditor())
                grvPerson.UpdateCurrentRow();
        }

        /// <summary>
        /// 사용권한 : 화면에 대한 권한이 체크될 경우, 화면/저장/삭제/출력 모두를 체크함
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RiMenuClss_EditValueChanged(object sender, EventArgs e)
        {
            grvUserMenu.SetValueRange(grvUserMenu.FocusedRowHandle,
                                      grvUserMenu.FocusedRowHandle,
                                      new string[] { "MenuClss", "SaveClss", "DeleteClss", "PrintClss" },
                                      (sender as CheckEdit).EditValue.CString());

            if (grvUserMenu.PostEditor())
                grvUserMenu.UpdateCurrentRow();
        }

        /// <summary>
        /// 저장 권한
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RiSaveClss_EditValueChanged(object sender, EventArgs e)
        {
            if (grvUserMenu.PostEditor())
                grvUserMenu.UpdateCurrentRow();
        }

        /// <summary>
        /// 삭제 권한
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RiDeleteClss_EditValueChanged(object sender, EventArgs e)
        {
            if (grvUserMenu.PostEditor())
                grvUserMenu.UpdateCurrentRow();
        }

        /// <summary>
        /// 출력 권한
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RiPrintClss_EditValueChanged(object sender, EventArgs e)
        {
            if (grvUserMenu.PostEditor())
                grvUserMenu.UpdateCurrentRow();
        }

        /// <summary>
        /// 화면 사용 여부에 따라 상세 권한 값의 변경/변경 불가하도록 함
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvUserMenu_ShowingEditor(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var view = sender as GridView;
            var menuClss = view.GetFocusedRowCellValue("MenuClss").CString();

            if (new string[] { "SaveClss", "DeleteClss", "PrintClss" }.Contains(view.FocusedColumn.FieldName))
            {
                if (menuClss == "1")    // 사용 컬럼이 미사용이면 상세 권한 값 변경 불가
                    e.Cancel = true;
            }
        }

        /// <summary>
        /// 전체선택, 전체해제, 영역선택, 영역해제 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lcgUserMenu_CustomButtonClick(object sender, DevExpress.XtraBars.Docking2010.BaseButtonEventArgs e)
        {
            string[] fields = { "MenuClss", "SaveClss", "DeleteClss", "PrintClss" };

            switch (e.Button.Properties.Tag.CString())
            {
                // 전체 선택 : 사용란 전체 체크 및 권한 전체 체크
                case "CheckAll":
                    grvUserMenu.SetValueRange(0, grvUserMenu.RowCount - 1, fields, "0");
                    break;

                // 전체 해제 : 사용란 전체 해제 및 권한 전체 해제
                case "UnCheckAll":
                    grvUserMenu.SetValueRange(0, grvUserMenu.RowCount - 1, fields, "1");
                    break;

                // 영역 선택 : 사용란 영역 체크 및 권한 영역 체크
                case "CheckArea":
                    grvUserMenu.SetValueRange(fields, "0");

                    break;

                // 영역 해제 : 사용란 영역 해제 및 권한 영역 해제
                case "UnCheckArea":
                    grvUserMenu.SetValueRange(fields, "1");
                    break;
            }
        }

        #endregion

        #region 사용자 정의 메서드
        /// <summary>
        /// 사원 리스트 조회 : 그리드 -> 사용 로그 기록하지 않음
        /// </summary>
        private void FillGridPerson()
        {
            try
            {
                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", "81", SqlDbType.Char);

                var table = SqlHelper.GetDataTable("xp_UserMenu", inParams, out _, out _);
                grcPerson.SetData(table);
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
        /// 특정 사원의 권한 정보 조회 : 그리드
        /// </summary>
        private void FillGridUserMenu()
        {
            try
            {
                grcUserMenu.DataSource = null;

                var startTime = DateTime.Now;

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetList, SqlDbType.Char);
                inParams.Add("@i_PersonID", grvPerson.GetFocusedRowCellValue("PersonID").CString(), SqlDbType.VarChar);
                inParams.Add("@i_LanguageCode", AppConfig.Login.Language, SqlDbType.VarChar);

                var table = SqlHelper.GetDataTable("xp_UserMenu", inParams, out _, out _);
                grcUserMenu.SetData(table);

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

        #endregion
    }
}