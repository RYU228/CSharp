using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Behaviors;
using DevExpress.Utils.DragDrop;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DSLibrary;

namespace MasterMES.FORMS.SYSTEM
{
    /// <summary>
    /// 환경설정 변경 내용 - 메인폼 적용시 이벤트 인자값
    /// </summary>
    public enum ChangeConfig
    {
        SkinType,
        MainMenu,
        Bookmark,
        AccodionMode
    }

    public partial class frmConfig : frmBase, CLASSES.IButtonAction
    {
        public delegate void ChangeConfigEventHandler(ChangeConfig change);
        public event ChangeConfigEventHandler ChangeConfigEvent;

        private BehaviorManager behaviorManager;

        #region 생성자
        public frmConfig()
        {
            InitializeComponent();

            rdgSkinType.SetGroupItem("D:DarkSkin|L:LightSkin");
            rdgMenuType.SetGroupItem("H:HorizonType|V:VerticalType");
            rdgAccordionMode.SetGroupItem("I:InlineType|O:OverlayType");

            // 메뉴표시 위치 및 글꼴 설정값 표시
            rdgSkinType.EditValue = AppConfig.App.SkinType;
            rdgMenuType.EditValue = AppConfig.App.MenuType;
            rdgAccordionMode.EditValue = AppConfig.App.AccordionMode;
            bteFont.EditValue = $"{AppConfig.App.FontName}, {AppConfig.App.FontSize}pt";

            // 자동으로 검색, 삭제 버튼이 생성됨, 삭제 버튼이 불필요해서 삭제 함
            bteFont.Properties.Buttons.Remove(bteFont.Properties.Buttons[1]);

            behaviorManager = new BehaviorManager(this.components);

            // BehaviorManager 초기화 : 좌 그리드 → 우 그리드로 데이터 이동(grvBookmark에 DragDrop시 발생)
            behaviorManager.Attach<DragDropBehavior>(grvBookmark, behavior =>
            {
                behavior.Properties.AllowDrop = true;
                behavior.Properties.InsertIndicatorVisible = true;
                behavior.Properties.PreviewVisible = true;
                behavior.DragDrop += Behavior_DragDropLeftToRight;
            });

            // BehaviorManager 초기화 : 우 그리드 → 좌 그리드로 데이터 이동(grvMenu에 DragDrop시 발생)
            behaviorManager.Attach<DragDropBehavior>(grvMenu, behavior =>
            {
                behavior.Properties.AllowDrop = true;
                behavior.Properties.InsertIndicatorVisible = true;
                behavior.Properties.PreviewVisible = true;
                behavior.DragDrop += Behavior_DragDropRightToLeft;
            });
        }

        #endregion

        #region 오버라이드 이벤트
        //protected override void OnActivated(EventArgs e)
        //{
        //    base.OnActivated(e);

        //    MainButton.ActiveButton();
        //    MainButton.ActiveButton("INIT,SAVE,DELETE");
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
                var startTime = DateTime.Now;

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Initial, SqlDbType.Char);
                inParams.Add("@i_PersonID", AppConfig.Login.PersonID);

                var initSet = SqlHelper.GetDataSet("xp_Config", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                // 전체 메뉴
                grcMenu.SetColumns(initSet.Tables[0], "MenuID,MenuPath,SortKey", false, true, true);
                grvMenu.SetRowIndicatorWidth();

                // 구분 컬럼 폭 고정
                grvMenu.FixedColumnWidth("BookmarkFlag", 50);

                // 즐겨찾기
                grcBookmark.SetColumns(initSet.Tables[1], "BookmarkFlag,MenuID,MenuPath,SortKey", false, true, true);
                grvBookmark.SetRowIndicatorWidth();
                grvBookmark.Columns["SortKey"].SortMode = DevExpress.XtraGrid.ColumnSortMode.Value;
                grvBookmark.Columns["SortKey"].SortOrder = DevExpress.Data.ColumnSortOrder.Ascending;

                SqlHelper.WriteLog(this, ActionLogType.Select, startTime);

                DxControl.SaveGridFormat(grvMenu);
                DxControl.SaveGridFormat(grvBookmark);

                DxControl.LoadGridFormat(grvMenu);
                DxControl.LoadGridFormat(grvBookmark);
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
                var startTime = DateTime.Now;

                var logType = ActionLogType.Insert;
                var message = MessageText.OKInsert;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Exist, SqlDbType.Char);
                inParams.Add("@i_PersonID", AppConfig.Login.PersonID, SqlDbType.VarChar);

                if (SqlHelper.Exist("xp_Config", inParams, out _, out _))
                {
                    if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionUpdate))
                        return;

                    logType = ActionLogType.Update;
                    message = MessageText.OKUpdate;
                }

                Utils.ShowWaitForm(this);
                MainButton.SaveButton = false;

                DataTable bookmark = (grcBookmark.DataSource as DataTable).DefaultView.ToTable(false, new string[] { "MenuID", "SortKey" });

                inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Insert, SqlDbType.Char);
                inParams.Add("@i_PersonID", AppConfig.Login.PersonID, SqlDbType.VarChar);
                inParams.Add("@i_SkinType", rdgSkinType.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_MenuType", rdgMenuType.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_AccordionMode", rdgAccordionMode.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_FontName", AppConfig.App.FontName, SqlDbType.NVarChar);
                inParams.Add("@i_FontSize", AppConfig.App.FontSize, SqlDbType.Decimal);
                inParams.Add("@i_ConfigBookmark", bookmark, SqlDbType.Structured);

                SqlHelper.Execute("xp_Config", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                ChangeConfigEvent(ChangeConfig.Bookmark);

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
                if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionDelete))
                    return;

                var startTime = DateTime.Now;

                Utils.ShowWaitForm(this);
                MainButton.DeleteButton = false;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Delete, SqlDbType.Char);
                inParams.Add("@i_PersonID", AppConfig.Login.PersonID, SqlDbType.VarChar);

                SqlHelper.Execute("xp_Config", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                grvBookmark.DeleteAllRows();

                ChangeConfigEvent(ChangeConfig.Bookmark);

                this.InitButtonClick();

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

        #region 폼 컨트롤 이벤트 - 일반
        /// <summary>
        /// 스킨 타입 변경
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdgSkinType_SelectedIndexChanged(object sender, EventArgs e)
        {
            AppConfig.App.SkinType = rdgSkinType.EditValue.CString();
            ChangeConfigEvent(ChangeConfig.SkinType);
        }

        /// <summary>
        /// 메뉴표시 위치 변경
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdgMenuType_SelectedIndexChanged(object sender, EventArgs e)
        {
            AppConfig.App.MenuType = rdgMenuType.EditValue.CString();
            ChangeConfigEvent(ChangeConfig.MainMenu);
        }

        /// <summary>
        /// AccodionControl 표시방법 변경
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdgAccordionMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            AppConfig.App.AccordionMode = rdgAccordionMode.EditValue.CString();
            ChangeConfigEvent(ChangeConfig.AccodionMode);
        }

        /// <summary>
        /// 프로그램 글꼴 변경
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bteFont_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            var fontDialog = new FontDialog
            {
                Font = WindowsFormsSettings.DefaultFont
            };

            if (DialogResult.OK == fontDialog.ShowDialog())
            {
                Utils.ShowWaitForm(this);

                WindowsFormsSettings.DefaultFont = fontDialog.Font;

                AppConfig.App.FontName = fontDialog.Font.Name;
                AppConfig.App.FontSize = fontDialog.Font.Size;
                bteFont.EditValue = $"{fontDialog.Font.Name}, {fontDialog.Font.Size}pt";

                Utils.CloseWaitForm();
            }
        }

        #endregion

        #region 폼 컨트롤 이벤트 - DranDrop, Double Click : 좌우 그리드 행 이동
        /// <summary>
        /// 메뉴 그리드 항목을 즐겨찾기 항목으로 추가
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Behavior_DragDropLeftToRight(object sender, DragDropEventArgs e)
        {
            try
            {
                if (e.Target == e.Source)
                    return;

                var index = (e.Data as int[]);
                var sourceRow = grvMenu.GetDataRow(index[0]);

                if (sourceRow != null && grcBookmark.DataSource is DataTable targetTable)
                {
                    // 우 그리드에 동일한 데이터(GroupCode)가 있는지 확인
                    var rows = targetTable.Select($"MenuID = '{sourceRow["MenuID"]}'");

                    // 우 그리드에 동일한 데이터가 존재하지 않으면 행추가
                    if (rows.Length <= 0)
                    {
                        targetTable.Rows.Add(sourceRow.ItemArray);
                        grvMenu.SetFocusedRowCellValue("BookmarkFlag", "☆");
                        this.RefreshSortKey();
                    }

                    e.Handled = true;
                }

                grvBookmark.SetRowIndicatorWidth();
                grvBookmark.BestFitColumns();
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 즐겨찾기 그리드에서 항목 삭제
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Behavior_DragDropRightToLeft(object sender, DragDropEventArgs e)
        {
            try
            {
                if (e.Target == e.Source)
                    return;

                var index = (e.Data as int[]);
                var sourceRow = grvBookmark.GetDataRow(index[0]);
                var targetTable = grcMenu.DataSource as DataTable;

                if (sourceRow != null)
                {
                    // 즐겨찾기 표시 삭제
                    var targetRow = targetTable.Select($"MenuID = '{sourceRow["MenuID"]}'");
                    var rowIndex = targetTable.Rows.IndexOf(targetRow[0]);
                    grvMenu.SetRowCellValue(rowIndex, "BookmarkFlag", "");

                    // Bookmark 그리드에서 행 삭제
                    (grcBookmark.DataSource as DataTable).Rows.Remove(sourceRow);

                    this.RefreshSortKey();

                    e.Handled = true;
                }

                grvMenu.SetRowIndicatorWidth();
                grvMenu.BestFitColumns();
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 메뉴 그리드 항목을 즐겨찾기 항목으로 추가
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvMenu_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var point = grcMenu.PointToClient(Control.MousePosition);
                var hitInfo = grvMenu.CalcHitInfo(point);

                if (hitInfo.RowHandle < 0)
                    return;

                var sourceRow = grvMenu.GetDataRow(hitInfo.RowHandle);

                if (sourceRow != null && grcBookmark.DataSource is DataTable targetTable)
                {
                    var rows = targetTable.Select($"MenuID = '{sourceRow["MenuID"]}'");

                    if (rows.Length <= 0)
                    {
                        targetTable.Rows.Add(sourceRow.ItemArray);
                        grvMenu.SetFocusedRowCellValue("BookmarkFlag", "☆");
                        this.RefreshSortKey();
                    }
                }

                // SetData 메소드를 호출하여 데이터를 표시하는게 아니므로 RowIndicator의 폭을 별도 설정한다.
                grvBookmark.SetRowIndicatorWidth();
                grvBookmark.BestFitColumns();
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 즐겨찾기 그리드에서 항목 삭제
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvBookmark_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var hitInfo = (sender as GridView).CalcHitInfo((e as DXMouseEventArgs).Location);

                if (hitInfo.RowHandle < 0)
                    return;

                var targetTable = grcMenu.DataSource as DataTable;

                // 즐겨찾기 표시 삭제
                var targetRow = targetTable.Select($"MenuID = '{grvBookmark.GetRowCellValue(hitInfo.RowHandle, "MenuID")}'");
                var rowIndex = targetTable.Rows.IndexOf(targetRow[0]);
                grvMenu.SetRowCellValue(rowIndex, "BookmarkFlag", "");

                grvBookmark.DeleteRow(hitInfo.RowHandle);

                this.RefreshSortKey();
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        #endregion

        #region 폼 컨트롤 이벤트 - 즐겨찾기 행 상하 이동
        private void btnFirst_Click(object sender, EventArgs e)
        {
            grvBookmark.GridControl.Focus();
            var index = grvBookmark.FocusedRowHandle;

            if (index <= 0)
                return;

            grvBookmark.SetFocusedRowCellValue("SortKey", 0);
            grvBookmark.MoveFirst();

            this.RefreshSortKey();

        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            grvBookmark.GridControl.Focus();
            var index = grvBookmark.FocusedRowHandle;

            if (index <= 0)
                return;

            var row1 = grvBookmark.GetDataRow(index);
            var row2 = grvBookmark.GetDataRow(index - 1);

            var sortKey1 = row1["SortKey"];
            var sortKey2 = row2["SortKey"];

            row1["SortKey"] = sortKey2;
            row2["SortKey"] = sortKey1;

            grvBookmark.FocusedRowHandle = index - 1;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            grvBookmark.GridControl.Focus();
            var index = grvBookmark.FocusedRowHandle;

            if (index >= grvBookmark.DataRowCount - 1)
                return;

            var row1 = grvBookmark.GetDataRow(index);
            var row2 = grvBookmark.GetDataRow(index + 1);

            var sortKey1 = row1["SortKey"];
            var sortKey2 = row2["SortKey"];

            row1["SortKey"] = sortKey2;
            row2["SortKey"] = sortKey1;

            grvBookmark.FocusedRowHandle = index + 1;
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            grvBookmark.GridControl.Focus();
            var index = grvBookmark.FocusedRowHandle;

            if (index >= grvBookmark.DataRowCount - 1)
                return;

            grvBookmark.SetFocusedRowCellValue("SortKey", 999);
            grvBookmark.MoveLast();

            this.RefreshSortKey();
        }

        #endregion

        #region 사용자 정의 메서드
        /// <summary>
        /// 즐겨찾기 그리드 상하 이동후 행번호 재부여
        /// </summary>
        private void RefreshSortKey()
        {
            for (int i = 0; i < grvBookmark.RowCount; i++)
                grvBookmark.SetRowCellValue(i, "SortKey", i + 1);
        }
        #endregion

        
    }
}