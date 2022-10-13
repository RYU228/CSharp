using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraTab;
using DevExpress.XtraTab.ViewInfo;
using DevExpress.LookAndFeel;
using DSLibrary;

namespace MasterMES
{
    public partial class frmMain : frmBase
    {
        #region 인스턴스 변수
        // Table형식 즐겨찾기의 최대 컬럼 수
        private const int BOOKMARK_MAX_COL = 10;

        // Flag값 : 상단 메뉴 열림 여부, 상단 즐겨찾기 열림 여부, 상단 즐겨찾기 수정 모드 여부
        private bool isMenuOpen = false;
        private bool isBookmarkOpen = false;
        private bool isBookmarkEditMode = false;

        // Accordion 즐겨찾기시 선택된 요소를 저장 (실제 처리는 PopUp 메뉴 클릭시 발생하므로)
        private AccordionControlElement selectedElement;

        // 활성화된 Document와 Form을 저장 : 부동 문서에 대한 처리를 위해서 
        private BaseDocument activeDocument;
        private Form activeForm;

        // 폼별 리포트 리스트, 폼별 사용자 권한을 저장 => 폼이 활성화 될때 리포트 및 권한 설정
        private DataTable report;
        private DataTable permission;

        #endregion

        #region 생성자
        public frmMain()
        {
            InitializeComponent();

            layoutMenu.AutoSize = true;
            layoutTopBookmark.AutoSize = true;
            layoutMidBookmark.AutoSize = true;

            // Accordion Menu 최소화시 그룹 버튼 클릭할 때 표시되는 메뉴의 높이를 메뉴에 맞게 조정
            acdMainMenu.OptionsMinimizing.PopupFormAutoHeightMode = AccordionPopupFormAutoHeightMode.FitContent;

            // 공용 처리버튼 이벤트 등록(모든 버튼은 동일한 이벤트를 처리한다.)
            btnInit.Click += ActionButton_Click;
            btnView.Click += ActionButton_Click;
            btnNew.Click += ActionButton_Click;
            btnCopy.Click += ActionButton_Click;
            btnSave.Click += ActionButton_Click;
            btnDelete.Click += ActionButton_Click;
            btnPrint.Click += ActionButton_Click;
            btnClose.Click += ActionButton_Click;

            // Skin Loading - 나중에 삭제
            foreach (SkinContainer skin in SkinManager.Default.Skins)
                cmbSkins.Properties.Items.Add(skin.SkinName);
        }

        #endregion

        #region SimpleButton - 사용여부 결정 필요

        /// <summary>
        /// 상단 공용버튼 클릭시 각 버튼의 기능을 처리 한다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ActionButton_Click(object sender, EventArgs e)
        {
            // 활성화된 Document가 없으면 프로그램을 종료하고, 있으면 해당 탭(폼)을 종료한다
            //if (domMain.View.ActiveDocument == null)
            //{
            //    if ((sender as SimpleButton).Name == "btnClose")
            //    {
            //        Close();
            //        return;
            //    }
            //}

            // 버튼 처리 메서드를 호출한다.(클릭된 버튼의 Name을 전달)
            ButtonClicked((sender as SimpleButton).Name);
        }

        /// <summary>
        /// 공용버튼 클릭 또는 단축키 입력시 각각 기능을 처리한다.(단 해당 버튼이 활성화 된 경우에 한함)
        /// </summary>
        /// <param name="keyValue"></param>
        private void ButtonClicked(string keyValue)
        {
            // 닫기 버튼 클릭의 경우 활성화된 문서가 없으면 프로그램을 종료한다.
            if (keyValue == "E" || keyValue == "btnClose")
            {
                //if (domMain.View.ActiveDocument == null)
                if (activeDocument == null)
                {
                    this.Close();
                    return;
                }
            }

            if (activeForm == null)
                return;

            try
            {
                switch (keyValue)
                {
                    case "btnInit":
                    case "I":
                        if (btnInit.Enabled)
                            ((CLASSES.IButtonAction)activeForm).InitButtonClick();
                        break;

                    case "btnCopy":
                    case "C":
                        if (btnCopy.Enabled)
                            ((CLASSES.IButtonAction)activeForm).CopyButtonClick();
                        break;

                    case "btnView":
                    case "F":
                        if (btnView.Enabled)
                            ((CLASSES.IButtonAction)activeForm).ViewButtonClick();
                        break;

                    case "btnNew":
                    case "N":
                        if (btnNew.Enabled)
                            ((CLASSES.IButtonAction)activeForm).NewButtonClick();
                        break;

                    case "btnSave":
                    case "S":
                        if (btnSave.Enabled)
                            ((CLASSES.IButtonAction)activeForm).SaveButtonClick();
                        break;

                    case "btnDelete":
                    case "D":
                        if (btnDelete.Enabled)
                            ((CLASSES.IButtonAction)activeForm).DeleteButtonClick();
                        break;

                    case "btnPrint":
                    case "P":
                        if (btnPrint.Enabled)
                        {
                            if (pumPrintReport.ItemLinks.Count > 0)
                                pumPrintReport.ShowPopup(MousePosition);
                            else
                                ((CLASSES.IButtonAction)activeForm).PrintButtonClick(1);
                        }
                        break;

                    case "btnClose":
                    case "E":
                        if (btnClose.Enabled)
                            domMain.View.Controller.Close(domMain.View.ActiveDocument);
                        break;
                }
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        #endregion

        #region 폼 이벤트
        /// <summary>
        /// 로그인, 메인 메뉴 생성, 권한에 따른 메뉴 Loading, 즐겨찾기 Loading) 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            try
            {
                // 메인 폼을 그리는 동안 숨김
                this.Hide();

                // 정상 로그인 후
                if (DialogResult.OK == new frmLogin().ShowDialog())
                {
                    // 스킨 적용
                    if (AppConfig.App.SkinType == "D")
                        UserLookAndFeel.Default.SkinName = "Duson Dark";
                    else
                        UserLookAndFeel.Default.SkinName = "Duson Light";

                    // 창 최대화
                    this.WindowState = FormWindowState.Maximized;

                    // 용어집 로딩
                    Program.GetGlossaryInfo();

                    // 메인화면 디자인 설정
                    DxControl.InitialControl(this.Controls);

                    // 공용 버튼 처리를 위한 Ribbon Control 설정
                    MainButton.MainRibbon = this.rbcMain;

                    // TabControl CustomHeaderButton 지역화
                    foreach (DevExpress.XtraTab.Buttons.CustomHeaderButton button in tacMain.CustomHeaderButtons)
                        button.Caption = button.Caption.Localization();

                    // Ribbon Control BarButtonItem 설정
                    var shortcut = new Dictionary<string, string>()
                    {
                        { "bbiInit", "(&I)" }, { "bbiView", "(&F)" }, { "bbiNew", "(&N)" }, { "bbiCopy", "(&C)" },
                        { "bbiSave", "(&S)" }, { "bbiDelete", "(&D)" }, { "bbiPrint", "(&P)" }, { "bbiClose", "(&E)" }
                    };

                    rbcMain.Items.ToList().ForEach(item =>
                    {
                        if (item.GetType() == typeof(BarButtonItem))
                        {
                            var button = item as BarButtonItem;
                            //button.ItemAppearance.Normal.ForeColor = Color.White;
                            button.ItemAppearance.Normal.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.ControlText;
                            button.ItemAppearance.Disabled.ForeColor = Color.Silver;
                            button.ItemAppearance.Hovered.Options.UseForeColor = true;
                            button.ItemAppearance.Hovered.ForeColor = Color.FromArgb(237, 125, 49);
                            button.ItemAppearance.Hovered.FontStyleDelta = FontStyle.Bold;
                            button.ItemAppearance.Pressed.ForeColor = Color.FromArgb(187, 100, 4);
                            //button.ItemAppearance.Pressed.BorderColor = Color.FromArgb(78, 81, 97);
                            //button.ItemAppearance.Pressed.BackColor = Color.FromArgb(78, 81, 97);

                            button.Caption = $" {button.Name.Replace("bbi", "").Localization()}{shortcut[button.Name]}";
                        }
                    });

                    // 팝업메뉴 색상 변경
                    pumPrintReport.SetPopupMenu();
                    pumMidBookmark.SetPopupMenu();
                    pumBookmarkAddDel.SetPopupMenu();

                    //// Accordion Control 즐겨찾기 추가 삭제용 PopupMenu
                    pumBookmarkAddDel.LinksPersistInfo.Clear();
                    var item1 = new BarButtonItem() { Caption = $"{  "Add Bookmark".Localization()}  ", Name = "addBookmarkItem" };
                    var item2 = new BarButtonItem() { Caption = $"{  "Remove Bookmark".Localization()}  ", Name = "removeBookmarkItem" };
                    pumBookmarkAddDel.AddItems(new BarItem[] { item1, item2 });

                    // 상단 메뉴 즐겨찾기 수정 버튼 초기화
                    this.MenuCustomeHeaderButton(false, true, false);

                    // 초기 버튼 설정(전체 비활성화)
                    MainButton.ActiveButtons();

                    // MDI 폼 기록
                    AppConfig.MainForm = this;

                    // Action 로그 기록하기 위한 tag값(MenuID, 시작시각)을 설정한다
                    this.Tag = new FormLog("00000", DateTime.Now);

                    // 메뉴 및 즐겨찾기 생성
                    this.CreateMenuAndBookmark(FORMS.SYSTEM.ChangeConfig.MainMenu);

                    // 권한 및 리포트 리스트 조회(폼 활성화시 적용을 위해 테이블을 조회해 놓는다)
                    this.GetFormInformation();

                    //임시로 즐겨 찾기를 활성화 합니다.
                    //acdMainMenu.ActiveGroup = acdMainMenu.Elements["Bookmark"];

                    // 스킨에 따른 색상 설정
                    this.SetSkinControlColor();

                    // 메인폼 표시
                    this.Show();

                    // Accordion Control DisplayMode 설정 (this.Show() 메서드 전에 변경하니 ScrollBar가 생성되며 Text 영역을 침범함
                    acdMainMenu.OptionsHamburgerMenu.DisplayMode = (AppConfig.App.AccordionMode == "O") ?
                                                                    AccordionControlDisplayMode.Overlay :
                                                                    AccordionControlDisplayMode.Inline;
                }
                else
                {
                    // 로그인 실패 Tag 설정
                    this.Tag = "CancelLogin";
                    Close();
                }
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// App, 열린폼의 로그를 기록 후 프로그램을 종료한다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmMdiMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.Tag.CString() == "CancelLogin")
            {
                //로그인 취소시 폼 종료이면 메세지 없이 바로 종료한다.
                Dispose();
            }
            else
            {
                // 로그인 성공후 폼 종료이면 메세지를 출력하고 열려있는 모든 Document(품)의 사용로그를 기록하고 종료한다.
                if (DialogResult.Yes == MsgBox.Show("MSG_APPLICATION_EXIT".Localization(), "", MessageType.QuestionD2))
                {
                    // X버튼 종료시 모든 활성화된 폼의 사용로그를 기록한다.
                    foreach (var doc in tbwMain.Documents)
                    {
                        var formStartTime = (doc.Form.Tag as FormLog).StartTime;
                        SqlHelper.WriteLog(doc.Form, ActionLogType.Form, formStartTime);
                    }

                    // App 사용 로그를 기록한다.
                    var appStartTime = (this.Tag as FormLog).StartTime;
                    SqlHelper.WriteLog(this, ActionLogType.Form, appStartTime);
                    Dispose();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        /// <summary>
        /// 단축키 입력시 각 단축키의 기능을 처리한다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmMdiMain_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Alt)
            {
                // 버튼 처리 메서드를 호출한다.(입력된 단축키의 KeyCode값을 전달)
                CommonButtonClick(e.KeyCode.CString().ToUpper());
            }
        }

        /// <summary>
        /// 부동 폼에 대한 단축키 적용
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ActiveForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Alt)
            {
                // 버튼 처리 메서드를 호출한다.(입력된 단축키의 KeyCode값을 전달)
                CommonButtonClick(e.KeyCode.CString().ToUpper());
            }
        }

        #endregion

        #region 폼 컨트롤 이벤트 - Ribbon Control
        /// <summary>
        /// RibbonControl의 BarButtonItem 클릭시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbcMain_ItemClick(object sender, ItemClickEventArgs e)
        {
            CommonButtonClick(e.Item.Name);
        }

        /// <summary>
        /// 단축키 또는 RibbonControl의 BarButtonItem을 클릭할때 버튼에 해당하는 기능 처리
        /// </summary>
        /// <param name="keyValue"></param>
        private void CommonButtonClick(string keyValue)
        {
            // 닫기 버튼 클릭의 경우 활성화된 문서가 없으면 프로그램을 종료한다.
            if (keyValue == "E" || keyValue == "bbiClose")
            {
                if (activeDocument == null)
                {
                    this.Close();
                    return;
                }
            }

            if (activeForm == null)
                return;

            BarButtonItem btn = null;
            bool isEnabled = false;

            try
            {
                switch (keyValue)
                {
                    case "bbiInit":
                    case "I":
                        if (bbiInit.Enabled)
                        {
                            // Wait폼 출력 및 공용 버튼을 비활성
                            Utils.ShowWaitForm(this);
                            isEnabled = true;
                            btn = bbiInit;
                            btn.Enabled = false;
                            ((CLASSES.IButtonAction)activeForm).InitButtonClick();
                        }
                        break;

                    case "bbiCopy":
                    case "C":
                        if (bbiCopy.Enabled)
                        {
                            // Wait폼 출력 및 공용 버튼을 비활성
                            Utils.ShowWaitForm(this);
                            isEnabled = true;
                            btn = bbiCopy;
                            btn.Enabled = false;
                            ((CLASSES.IButtonAction)activeForm).CopyButtonClick();
                        }
                        break;

                    case "bbiView":
                    case "F":
                        if (bbiView.Enabled)
                        {
                            // Wait폼 출력 및 공용 버튼을 비활성
                            Utils.ShowWaitForm(this);
                            isEnabled = true;
                            btn = bbiView;
                            btn.Enabled = false;
                            ((CLASSES.IButtonAction)activeForm).ViewButtonClick();
                        }
                        break;

                    case "bbiNew":
                    case "N":
                        if (bbiNew.Enabled)
                        {
                            // Wait폼 출력 및 공용 버튼을 비활성
                            Utils.ShowWaitForm(this);
                            isEnabled = true;
                            btn = bbiNew;
                            btn.Enabled = false;
                            ((CLASSES.IButtonAction)activeForm).NewButtonClick();
                        }
                        break;

                    case "bbiSave":
                    case "S":
                        if (bbiSave.Enabled)
                        {
                            // Wait폼 출력 및 공용 버튼을 비활성
                            Utils.ShowWaitForm(this);
                            isEnabled = true;
                            btn = bbiSave;
                            btn.Enabled = false;
                            ((CLASSES.IButtonAction)activeForm).SaveButtonClick();
                        }
                        break;

                    case "bbiDelete":
                    case "D":
                        if (bbiDelete.Enabled)
                        {
                            // Wait폼 출력 및 공용 버튼을 비활성
                            Utils.ShowWaitForm(this);
                            isEnabled = true;
                            btn = bbiDelete;
                            btn.Enabled = false;
                            ((CLASSES.IButtonAction)activeForm).DeleteButtonClick();
                        }
                        break;

                    case "bbiPrint":
                    case "P":
                        if (bbiPrint.Enabled)
                        {
                            if (pumPrintReport.ItemLinks.Count > 0)
                                pumPrintReport.ShowPopup(MousePosition);
                            else
                            {
                                // Wait폼 출력 및 공용 버튼을 비활성
                                Utils.ShowWaitForm(this);
                                isEnabled = true;
                                btn = bbiPrint;
                                btn.Enabled = false;
                                ((CLASSES.IButtonAction)activeForm).PrintButtonClick(1);
                            }
                        }
                        break;

                    case "bbiClose":
                    case "E":
                        if (bbiClose.Enabled)
                            domMain.View.Controller.Close(activeDocument);

                        break;
                }
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
            finally
            {
                // 공용 버튼을 활성화하고 Wait폼을 종료
                if (isEnabled == true)
                    btn.Enabled = true;
                Utils.CloseWaitForm();
            }
        }

        #endregion

        #region 폼 컨트롤 이벤트 - Accordion Control
        /// <summary>
        /// Accordion Control 그룹항목 시작 부분에 붉은색 막대를 그린다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void acdMainMenu_CustomDrawElement(object sender, CustomDrawElementEventArgs e)
        {
            if (e.Element.Style == ElementStyle.Group)
            {
                e.DrawHeaderBackground();
                e.DrawContextButtons();
                e.DrawExpandCollapseButton();
                e.DrawText();
                e.DrawImage();

                var point = e.ObjectInfo.HeaderBounds.Location;
                point.Y += 1;

                //e.Graphics.FillRectangle(Brushes.Red, new Rectangle(point, new Size(7, e.ObjectInfo.HeaderBounds.Height - 1)));
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(255, 102, 213, 134)), new Rectangle(point, new Size(7, e.ObjectInfo.HeaderBounds.Height - 1)));
                e.Handled = true;
            }
        }

        /// <summary>
        /// AccordionControl Element 클릭시(메뉴 클릭시)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void acdMainMenu_ElementClick(object sender, ElementClickEventArgs e)
        {
            if (e.Element.Style == ElementStyle.Item)
            {
                if (e.MouseButton == MouseButtons.Left)
                {
                    if (e.Element.Tag.CString() == "RemoteAssistance")
                        this.StartRemoteAssistance();
                    else if (e.Element.Tag.CString() == "Logout")
                        this.OnLoad(null);
                    else
                        CallMdiChildForm(e.Element.Tag.CString(), e.Element.Name);
                }
                else if (e.MouseButton == MouseButtons.Right)
                {
                    // 시스템관리 메뉴는 즐겨찾기 할 수 없습니다.
                    if (e.Element.Name.Substring(0, 2) == "99")
                        return;

                    // 즐겨찾기 추가/삭제할 Element를 변수에 저장
                    selectedElement = e.Element;

                    // 즐겨찾기 추가/삭제 단축 메뉴 표시 - 즐겨찾기 그룹이면 => 추가:Visible=false, 아니면 => 추가:Visible=true ===> 즐겨찾기 삭제.Visible = !즐겨찾기 추가.Visible
                    pumBookmarkAddDel.ItemLinks[0].Visible = (e.Element.OwnerElement.Name == "BookmarkGroup") ? false : true;
                    pumBookmarkAddDel.ItemLinks[1].Visible = !pumBookmarkAddDel.ItemLinks[0].Visible;

                    pumBookmarkAddDel.ShowPopup(MousePosition);
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Accordion Control의 상태가 변경될때 Group Element의 Font를 변경한다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void acdMainMenu_StateChanged(object sender, EventArgs e)
        {
            Font groupFont;

            try
            {
                if (acdMainMenu.OptionsMinimizing.State == AccordionControlState.Normal)
                    groupFont = new Font(AppConfig.App.FontName, AppConfig.App.FontSize + 2, FontStyle.Bold);
                else
                    groupFont = new Font(AppConfig.App.FontName, AppConfig.App.FontSize, FontStyle.Regular);

                acdMainMenu.Appearance.Group.Normal.Font = groupFont;
                acdMainMenu.Appearance.Group.Hovered.Font = groupFont;
                acdMainMenu.Appearance.Group.Pressed.Font = groupFont;
            }
            catch { }
        }

        #endregion

        #region 폼 컨트롤 이벤트 - DocumentManager - TabView Control
        /// <summary>
        /// TabView의 문서가 비활성화 될때 공용버튼을 초기화 한다. (마지막 문서 비활성화시 대비)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbwMain_DocumentClosed(object sender, DocumentEventArgs e)
        {
            if (tbwMain.ActiveDocument == null)
                MainButton.ActiveButtons();
        }

        #endregion

        #region 폼 컨트롤 이벤트 - XtraTabControl
        /// <summary>
        /// 탭 페이지를 클릭할때 보이기/숨기기 실행
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tacMain_MouseClick(object sender, MouseEventArgs e)
        {
            var tabControl = sender as XtraTabControl;
            var point = MousePosition;

            var info = tabControl.CalcHitInfo(tabControl.PointToClient(point));
            if (info.HitTest == XtraTabHitTest.PageHeader)
            {
                if (info.Page == tapMenu)
                {
                    isMenuOpen = !isMenuOpen;
                    isBookmarkOpen = false;
                    tacMain.Height = isMenuOpen ? layoutMenu.Height + 30 : 30;
                }
                else
                {
                    isBookmarkOpen = !isBookmarkOpen;
                    isMenuOpen = false;
                    tacMain.Height = isBookmarkOpen ? layoutTopBookmark.Height + 30 : 30;
                }
            }
        }

        /// <summary>
        /// TabControl의 닫기 버튼 클릭시 상단메뉴 숨기기/AccordionControl 보이기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tacMain_CustomHeaderButtonClick(object sender, DevExpress.XtraTab.ViewInfo.CustomHeaderButtonEventArgs e)
        {
            switch (e.Button.Tag.CString())
            {
                case "InitBookmark":
                    this.InitBookmark();
                    break;

                case "EditBookmark":
                    isBookmarkEditMode = true;
                    tacMain.SelectedTabPage = tapMenu;
                    this.EditBookmark();
                    break;

                case "SaveBookmark":
                    this.SaveBookmark();
                    break;
            }
        }

        #endregion

        #region 폼 컨트롤 이벤트 - Document Manager
        /// <summary>
        /// 폼별 공용 버튼 및 리포트 설정
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void domMain_DocumentActivate(object sender, DevExpress.XtraBars.Docking2010.Views.DocumentEventArgs e)
        {
            BaseDocument dockDocument = domMain.View.ActiveDocument;
            BaseDocument floatDocument = domMain.View.ActiveFloatDocument;

            if (floatDocument != null)
            {
                activeForm = floatDocument.Form;
                activeDocument = floatDocument;
            }
            else if (dockDocument != null)
            {
                activeForm = dockDocument.Form;
                activeDocument = dockDocument;
            }
            else if (domMain.View.Documents.Count == 1) // 한개의 탭만 활성화시 강제로 activeDocument 설정
            {
                activeForm = domMain.View.Documents[0].Form;
                activeDocument = domMain.View.Documents[0];
            }
            else if (domMain.View.Documents.Count == 0) // 프로그램 종료시 activeDocument 사용
            {
                activeForm = null;
                activeDocument = null;
            }

            MainButton.ActiveButtons();
            pumPrintReport.ItemLinks.Clear();

            if (activeForm != null)
            {
                // 활성화된 폼의 단축키를 처리하기 위해서 KeyPreview 속성 및 KeyUp 이벤트를 처리한다.
                activeForm.KeyPreview = true;
                activeForm.KeyUp -= ActiveForm_KeyUp;
                activeForm.KeyUp += ActiveForm_KeyUp;

                // 환경설정은 권한에 상관없이 저장 버튼만 활성화 한다.
                if (activeForm.Name == "frmConfig")
                {
                    bbiSave.Enabled = true;
                    return;
                }

                bbiInit.Enabled = true;
                bbiView.Enabled = true;

                // 권한 설정
                var formPermission = this.permission.Select($"MenuID = '{((FormLog)activeForm.Tag).MenuID}'");
                if (formPermission != null && formPermission.Length > 0)
                {
                    bbiNew.Enabled = formPermission[0]["SaveClss"].CString() == "0";
                    bbiCopy.Enabled = formPermission[0]["SaveClss"].CString() == "0";
                    bbiSave.Enabled = formPermission[0]["SaveClss"].CString() == "0";
                    bbiDelete.Enabled = formPermission[0]["DeleteClss"].CString() == "0";
                    bbiPrint.Enabled = formPermission[0]["PrintClss"].CString() == "0";

                    if (bbiPrint.Enabled)
                    {
                        var formReport = this.report.Select($"FormName = '{activeForm.Name}'", "ReportSeq ASC");

                        if (formReport != null && formReport.Length > 0)
                        {
                            for (int i = 0; i < formReport.Length; i++)
                            {
                                pumPrintReport.AddItem(new BarButtonItem()
                                {
                                    Caption = $"  {formReport[i]["ReportName"].CString()  }",
                                    Id = i,
                                    Name = $"btn{formReport[i]["FormName"]}",
                                    Tag = formReport[i]["ReportSeq"].CString()
                                });
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region 폼 컨트롤 이벤트 - PopupMenu
        /// <summary>
        /// PopupMenu 클릭시 처리(중앙즐겨찾기, 출력버튼, 즐겨찾기 추가/삭제)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bmgMain_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.Link.LinkedObject == pumMidBookmark)
            {
                this.CallMdiChildForm(e.Item.Tag.CString(), e.Item.Name.Replace("bkMid", ""));
            }
            else if (e.Link.LinkedObject == pumPrintReport)
            {
                ((CLASSES.IButtonAction)activeForm).PrintButtonClick(e.Item.Id + 1);
            }
            else if (e.Link.LinkedObject == pumBookmarkAddDel)
            {
                if (e.Item.Name == "addBookmarkItem")
                    this.AddBookmark();
                else if (e.Item.Name == "removeBookmarkItem")
                    this.RemoveBookmark();
            }
        }

        #endregion

        #region 사용자 정의 메서드 - 즐겨찾기 추가/삭제
        /// <summary>
        /// Accordion 컨트롤에서 즐겨찾기 추가할 경우
        /// 1. 즐겨찾기 테이블에 데이터 입력, 2. Accordion Control, Top, Middle 즐겨찾기 다시 로딩
        /// </summary>
        private void AddBookmark()
        {
            try
            {
                if (selectedElement == null)
                    return;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", "31", SqlDbType.Char);
                inParams.Add("@i_PersonID", AppConfig.Login.PersonID, SqlDbType.VarChar);
                inParams.Add("@i_MenuID", selectedElement.Name, SqlDbType.VarChar);
                inParams.Add("@i_SortKey", 999, SqlDbType.SmallInt);
                inParams.Add("@i_InptUser", AppConfig.Login.UserID, SqlDbType.VarChar);

                var count = SqlHelper.Execute("xp_Bookmark", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                // Bookmark 다시 Loading
                this.CreateBookmark();
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// Accordion 컨트롤에서 즐겨찾기 추가할 경우
        /// 1. 즐겨찾기 테이블에 데이터 삭제, 2. Accordion Control, Top, Middle 즐겨찾기 다시 로딩
        /// </summary>
        private void RemoveBookmark()
        {
            try
            {
                if (selectedElement == null)
                    return;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", "71", SqlDbType.Char);
                inParams.Add("@i_PersonID", AppConfig.Login.PersonID, SqlDbType.VarChar);
                inParams.Add("@i_MenuID", selectedElement.Name, SqlDbType.VarChar);

                var count = SqlHelper.Execute("xp_Bookmark", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                // Bookmark 다시 Loading
                this.CreateBookmark();
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }
        #endregion

        #region 사용자 정의 메서드 - 메뉴/즐겨찾기 생성

        /// <summary>
        /// 메뉴 표시 위치에 따라 메뉴 및 즐겨찾기를 생성
        /// </summary>
        private void CreateMenuAndBookmark(FORMS.SYSTEM.ChangeConfig change)
        {
            try
            {
                if (change == FORMS.SYSTEM.ChangeConfig.SkinType)
                {
                    Utils.ShowWaitForm(this, "Skin Change...");

                    pumPrintReport.SetPopupMenu();
                    pumMidBookmark.SetPopupMenu();
                    pumBookmarkAddDel.SetPopupMenu();

                    if (AppConfig.App.SkinType == "D")
                        UserLookAndFeel.Default.SkinName = "Duson Dark";
                    else
                        UserLookAndFeel.Default.SkinName = "Duson Light";

                    this.SetSkinControlColor();
                }
                else if (change == FORMS.SYSTEM.ChangeConfig.AccodionMode)
                {
                    // Accordion Control DisplayMode 설정(환경설정 변경시)
                    acdMainMenu.OptionsHamburgerMenu.DisplayMode = (AppConfig.App.AccordionMode == "O") ?
                                                                    AccordionControlDisplayMode.Overlay :
                                                                    AccordionControlDisplayMode.Inline;
                }
                else
                {
                    Utils.ShowWaitForm(this, "Loading the menu...");

                    // 메뉴의 생성 여부 
                    if (change == FORMS.SYSTEM.ChangeConfig.MainMenu)
                        this.CreateMenu();

                    this.CreateBookmark();

                    if (AppConfig.App.MenuType == "H")
                    {
                        tacMain.Visible = true;
                        tacMain.Height = 30;
                        tacMain.SelectedTabPage = tapMenu;

                        acdMainMenu.Visible = false;

                        isMenuOpen = false;         // 탭페이지 펼침 여부
                        isBookmarkOpen = false;     // 탭페이지 펼침 여부
                    }
                    else
                    {
                        tacMain.Visible = false;
                        this.MenuCustomeHeaderButton(false, true, false);   // 즐겨찾기 수정 버튼 초기화
                        this.ChangeMenuButtonState(true);                   // 즐겨찾기 수정 모드 초기화

                        acdMainMenu.Visible = true;
                        acdMainMenu.OptionsMinimizing.State = AccordionControlState.Normal;
                    }
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
        /// 메뉴를 생성합니다.
        /// </summary>
        private void CreateMenu()
        {
            try
            {
                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", "82", SqlDbType.Char);
                inParams.Add("@i_PersonID", AppConfig.Login.PersonID, SqlDbType.VarChar);

                var menuSet = SqlHelper.GetDataSet("xp_UserMenu", inParams, out string retCode, out string retMsg);

                if (menuSet == null)
                    return;

                if (retCode == "00")
                {
                    if (AppConfig.App.MenuType == "H")
                        this.CreateTableMenu(menuSet);
                    else
                        this.CreateAccordionMenu(menuSet);
                }
                else
                {
                    throw new Exception(retMsg);
                }
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 메인 메뉴 테이블의 정보를 이용 주 메뉴(Accordion Control)를 생성한다.
        /// </summary>
        private void CreateAccordionMenu(DataSet menuSet)
        {
            try
            {
                var menuList = menuSet.Tables[1];
                acdMainMenu.Elements.Clear();

                // 메인메뉴 그룹
                var acdGroup = new AccordionControlElement(ElementStyle.Group) { Name = "MainMenu", Text = "메인메뉴" };
                acdGroup.ImageOptions.ImageUri.Uri = "contentarrangeinrows;Size32x32;Colored";// Properties.Resources.Tools_32x32;
                acdMainMenu.Elements.Add(acdGroup);

                // 북마크 그룹
                acdGroup = new AccordionControlElement(ElementStyle.Group) { Name = "Bookmark", Text = "즐겨찾기" };
                acdGroup.ImageOptions.ImageUri.Uri = "Feature;Size32x32;Colored";// Properties.Resources.Bookmark_32x32;
                acdMainMenu.Elements.Add(acdGroup);

                for (int i = 0; i < menuList.Rows.Count; i++)
                {
                    if (menuList.Rows[i]["MenuLevel"].ToString() == "1")
                    {
                        //서브 메뉴 그룹
                        acdMainMenu.Elements["MainMenu"].Elements.Add(new AccordionControlElement()
                        {
                            Name = menuList.Rows[i]["MenuID"].CString(),
                            Text = $" {menuList.Rows[i]["MenuID"].Localization()}",
                            Expanded = true,
                            Style = ElementStyle.Group,
                        });
                    }
                    else
                    {
                        // 메뉴 항목 : 상위 메뉴 숨김, 하위 메뉴 표시인 경우를 방지하기 위한 조건 추가
                        if (acdMainMenu.Elements["MainMenu"].Elements[$"{menuList.Rows[i]["ParentID"]}"] != null)
                        {
                            acdMainMenu.Elements["MainMenu"].Elements[$"{menuList.Rows[i]["ParentID"]}"].Elements.Add(new AccordionControlElement()
                            {
                                Name = menuList.Rows[i]["MenuID"].CString(),
                                Text = $" {menuList.Rows[i]["MenuPath"].Localization()}",
                                Style = ElementStyle.Item,
                                Tag = menuList.Rows[i]["FormPath"]
                            });
                        }
                    }
                }

                var fontFamily = acdMainMenu.Appearance.Group.Normal.Font.FontFamily;
                var fontSize = acdMainMenu.Appearance.Group.Normal.Font.Size + 2;
                var fontStyle = FontStyle.Bold; // | FontStyle.Italic;

                acdMainMenu.Appearance.Group.Normal.ForeColor = Color.FromArgb(255, 102, 213, 134);
                acdMainMenu.Appearance.Group.Normal.Font = new Font(fontFamily, fontSize, fontStyle);
                acdMainMenu.Appearance.Group.Hovered.Font = new Font(fontFamily, fontSize, fontStyle);
                acdMainMenu.Appearance.Group.Pressed.Font = new Font(fontFamily, fontSize, fontStyle);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 상단 버튼 메뉴 생성
        /// </summary>
        private void CreateTableMenu(DataSet menuSet)
        {
            try
            {
                // 존재하는 LayoutControlItem 모두 삭제
                this.ClearLayoutControlItem(layoutMenu);

                //var colCnt = (menuSet.Tables[0].Rows[0]["ColCnt"].CInt() < 15) ? 15 : menuSet.Tables[0].Rows[0]["ColCnt"].CInt();
                var colCnt = menuSet.Tables[0].Rows[0]["ColCnt"].CInt();

                for (int i = 0; i < colCnt; i++)
                {
                    var sizeType = (i == 0) ? SizeType.Absolute : SizeType.Percent;     // 첫번째 컬럼 폭 고정 (해상도에 따른 시작 넓이를 고정)
                    layoutMenu.Root.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition()
                    {
                        SizeType = SizeType.Percent,
                        Width = 100D
                    });
                }

                for (int i = 0; i < menuSet.Tables[0].Rows[0]["RowCnt"].CInt(); i++)
                    layoutMenu.Root.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.AutoSize });

                // Menu TableLayout에 버튼을 생성합니다.
                foreach (DataRow row in menuSet.Tables[1].Rows)
                {
                    var button = new SimpleButton();
                    button.Appearance.Options.UseTextOptions = true;
                    button.Appearance.TextOptions.HAlignment = HorzAlignment.Near;

                    // 메뉴 그룹과 메뉴 항목의 용어집 기준이 틀려서. (메뉴 그룹은 폼이 없기때문에 ID로, 항목은 폼이 있기 때문에 Path로)
                    var menuText = (row["RowIndex"].CInt() == 0) ? row["MenuID"].Localization() : row["MenuPath"].Localization();

                    this.AddLayoutButton(layoutMenu,
                                         button,
                                         $"btn{row["MenuID"]}",
                                         menuText,
                                         row["FormPath"].CString(),
                                         row["RowIndex"].CInt(),
                                         row["ColumnIndex"].CInt());

                    // 메뉴 그룹 버튼이면 글자 크기를 크게, 비활성화 한다.
                    if (row["RowIndex"].CInt() == 0)
                    {
                        button.Appearance.Options.UseForeColor = true;
                        button.Font = new Font(button.Font.Name, button.Font.Size + 2, FontStyle.Bold);
                        //button.AppearanceDisabled.ForeColor = Color.White;
                        button.Enabled = false;
                        button.Text = $"[{button.Text}]";
                    }
                    else
                    {
                        // 클릭 이벤트를 등록합니다.
                        button.Click += TableMenuButton_Click;
                    }
                }
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        #endregion

        #region 사용자 정의 메서드 - 즐겨찾기 생성
        /// <summary>
        /// 즐겨 찾기를 생성합니다.
        /// </summary>
        private void CreateBookmark()
        {
            try
            {
                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", "81", SqlDbType.Char);
                inParams.Add("@i_PersonID", AppConfig.Login.PersonID, SqlDbType.VarChar);

                var table = SqlHelper.GetDataTable("xp_Bookmark", inParams, out string retCode, out string retMsg);

                if (retCode == "00")
                {
                    this.CreateMiddleTableBookmark(table);

                    if (AppConfig.App.MenuType == "H")
                        this.CreateTopTableBookmark(table);
                    else
                        this.CreateAccordionBookmark(table);
                }
                else
                {
                    throw new Exception(retMsg);
                }
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// AccordionControl에 즐겨찾기를 생성합니다.
        /// </summary>
        /// <param name="table"></param>
        private void CreateAccordionBookmark(DataTable table)
        {
            try
            {
                acdMainMenu.Elements["Bookmark"].Elements.Clear();

                //서브 메뉴 그룹
                acdMainMenu.Elements["Bookmark"].Elements.Add(new AccordionControlElement()
                {
                    Name = "BookmarkGroup",
                    Text = "즐겨찾기",
                    Expanded = true,
                    Style = ElementStyle.Group,
                });

                foreach (DataRow row in table.Rows)
                {
                    acdMainMenu.Elements["Bookmark"].Elements["BookmarkGroup"].Elements.Add(new AccordionControlElement()
                    {
                        Name = row["MenuID"].CString(),
                        Text = row["MenuPath"].Localization(),
                        Style = ElementStyle.Item,
                        Tag = row["FormPath"].CString()
                    });
                }
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 상단 즐겨찾기를 생성합니다.
        /// </summary>
        /// <param name="table"></param>
        private void CreateTopTableBookmark(DataTable table)
        {
            this.ClearLayoutControlItem(layoutTopBookmark);

            var maxRow = Math.Ceiling((table.Rows.Count / BOOKMARK_MAX_COL.CDouble()));

            for (int i = 0; i < BOOKMARK_MAX_COL + 1; i++)
            {
                //var sizeType = (i == 0) ? SizeType.Absolute : SizeType.Percent;     // 첫번째 컬럼 폭 고정 (해상도에 따른 시작 넓이를 고정)
                layoutTopBookmark.Root.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    SizeType = SizeType.Percent,
                    Width = 100d
                });
            }

            for (int i = 0; i < maxRow; i++)
                layoutTopBookmark.Root.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.AutoSize });

            for (int i = 0; i < table.Rows.Count; i++)
            {
                var button = new SimpleButton();
                button.Appearance.Options.UseTextOptions = true;
                button.Appearance.TextOptions.HAlignment = HorzAlignment.Near;

                this.AddLayoutButton(layoutTopBookmark,
                                     button,
                                     $"bkTop{table.Rows[i]["MenuID"]}",
                                     table.Rows[i]["MenuPath"].Localization(),
                                     table.Rows[i]["FormPath"].CString(),
                                     table.Rows[i]["RowIndex"].CInt(),
                                     table.Rows[i]["ColumnIndex"].CInt() + 1);

                button.Click += TopBookmarkButton_Click;
            }
        }

        /// <summary>
        /// 중앙 즐겨찾기를 생성합니다.
        /// </summary>
        /// <param name="table"></param>
        private void CreateMiddleTableBookmark(DataTable table)
        {
            try
            {
                // 존재하는 LayoutControlItem 모두 삭제
                this.ClearLayoutControlItem(layoutMidBookmark);

                // Clear Popup Menu
                pumMidBookmark.ItemLinks.Clear();

                // 즐겨찾기 개수에 따라 버튼 크기가 달라지므로 무조건 TableLayout 컬럼수는 고정한다.
                for (int i = 0; i < BOOKMARK_MAX_COL; i++)
                {
                    layoutMidBookmark.Root.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition()
                    {
                        SizeType = SizeType.Percent,
                        Width = 100d
                    });
                }

                // 중앙 즐겨찾기는 행이 한개 임
                layoutMidBookmark.Root.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.AutoSize });

                for (int i = 0; i < table.Rows.Count; i++)
                {
                    var button = new SimpleButton();

                    if (i < BOOKMARK_MAX_COL)
                    {
                        this.AddLayoutButton(layoutMidBookmark,
                                             button,
                                             $"bkMid{table.Rows[i]["MenuID"]}",
                                             table.Rows[i]["MenuPath"].Localization().Trim(),
                                             table.Rows[i]["FormPath"].CString(),
                                             0,
                                             i);

                        button.Click += MidBookmarkButton_Click;
                    }
                    else
                    {
                        // DropDown Button 생성
                        if (i == BOOKMARK_MAX_COL)
                        {
                            layoutMidBookmark.Root.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition()
                            {
                                SizeType = SizeType.Absolute,
                                Width = 40d
                            });

                            // 메뉴 Button 설정
                            this.AddLayoutButton(layoutMidBookmark, button, "CallContextMenu", "▼", "", 0, i);
                            button.Click += MidBookmarkButton_Click;
                        }

                        pumMidBookmark.AddItem(new BarButtonItem()
                        {
                            Caption = $"  {table.Rows[i]["MenuPath"].Localization()  }",
                            Id = i,
                            Name = $"bkMid{table.Rows[i]["MenuID"]}",
                            Tag = table.Rows[i]["FormPath"]
                        });
                    }

                    // 중앙 즐겨찾기는 기본 버튼의 색상이 틀림
                    //button.ForeColor = Color.Coral;
                    button.ForeColor = Color.FromArgb(255, 100, 40);
                    button.AppearanceHovered.ForeColor = Color.Red;
                    button.AppearancePressed.ForeColor = Color.FromArgb(192, 0, 0);
                }
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 상단 즐겨찾기 버튼 클릭시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TopBookmarkButton_Click(object sender, EventArgs e)
        {
            var button = sender as SimpleButton;
            this.CallMdiChildForm(button.Tag.CString(), button.Name.Replace("bkTop", ""));
        }

        /// <summary>
        /// 중앙 즐겨찾기 버튼 클릭시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MidBookmarkButton_Click(object sender, EventArgs e)
        {
            var button = sender as SimpleButton;

            if (button.Name == "CallContextMenu")
                pumMidBookmark.ShowPopup(MousePosition);
            else
                this.CallMdiChildForm(button.Tag.CString(), button.Name.Replace("bkMid", ""));
        }

        #endregion

        #region 사용자 정의 메서드 - 상단 즐겨찾기 관리 (초기화, 수정, 저장)
        /// <summary>
        /// 등록된 모든 즐겨찾기를 삭제합니다.
        /// </summary>
        private void InitBookmark()
        {
            try
            {
                Utils.ShowWaitForm(this);
                tacMain.CustomHeaderButtons[0].Enabled = false;

                // 등록된 즐겨찾기를 조회합니다.
                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Delete, SqlDbType.Char);
                inParams.Add("@i_PersonID", AppConfig.Login.PersonID, SqlDbType.VarChar);

                var table = SqlHelper.GetDataTable("xp_Bookmark", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                this.MenuCustomeHeaderButton(false, true, false);
                this.ChangeMenuButtonState(true);
                this.ShowBookmarkImage();
                this.CreateBookmark();
                acdMainMenu.Visible = false;
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
            finally
            {
                tacMain.CustomHeaderButtons[0].Enabled = true;
                Utils.CloseWaitForm();
            }
        }

        /// <summary>
        /// 즐겨찾기 수정 시작
        /// </summary>
        private void EditBookmark()
        {
            try
            {
                Utils.ShowWaitForm(this);
                tacMain.CustomHeaderButtons[1].Enabled = false;

                // TabControl의 CusotomHeader Button Visible 속성 변경
                this.MenuCustomeHeaderButton(true, false, true);

                // 상단 메뉴 버튼들을 즐겨찾기 수정 모드(색상 변경)로 변경
                this.ChangeMenuButtonState(false);

                // 즐겨찾기 등록된 메뉴에 대해 이미지 표시
                this.ShowBookmarkImage();

                isMenuOpen = true;
                isBookmarkOpen = false;
                tacMain.Height = layoutMenu.Height + 30;
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
            finally
            {
                tacMain.CustomHeaderButtons[1].Enabled = true;
                Utils.CloseWaitForm();
            }
        }

        /// <summary>
        /// 즐겨찾기 저장
        /// </summary>
        private void SaveBookmark()
        {
            try
            {
                Utils.ShowWaitForm(this);
                tacMain.CustomHeaderButtons[2].Enabled = false;

                var table = new DataTable();
                table.Columns.Add("MenuID", typeof(string));
                table.Columns.Add("IsBookmark", typeof(string));

                foreach (BaseLayoutItem item in layoutMenu.Items)
                {
                    if (item is LayoutControlItem li && li.Control is SimpleButton button)
                    {
                        // 즐겨찾기 표시 여부에 따라 DataTable에 행 추가 
                        // Stroed Procedure 내부에서 기존행은 정렬순서를 그대로, 신규찾기는 999로 행을 생성한 후 정렬순서, 메뉴ID로 정렬순서를 재 설정한다.
                        if (string.IsNullOrEmpty(button.ImageOptions.ImageUri.Uri))
                            table.Rows.Add(button.Name.Replace("btn", ""), "N");
                        else
                            table.Rows.Add(button.Name.Replace("btn", ""), "Y");
                    }
                }

                var xmlData = table.ToXml();

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Insert, SqlDbType.Char);
                inParams.Add("@i_PersonID", AppConfig.Login.PersonID, SqlDbType.VarChar);
                inParams.Add("@i_XmlData", xmlData, SqlDbType.VarChar);

                var count = SqlHelper.Execute("xp_Bookmark", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                this.MenuCustomeHeaderButton(false, true, false);
                this.ChangeMenuButtonState(true);
                this.CreateBookmark();
                acdMainMenu.Visible = false;
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
            finally
            {
                tacMain.CustomHeaderButtons[2].Enabled = true;
                Utils.CloseWaitForm();
            }
        }

        /// <summary>
        /// TabControl의 사용자 버튼 Visible 속성을 설정합니다.
        /// </summary>
        /// <param name="isInit"></param>
        /// <param name="isEdit"></param>
        /// <param name="isSave"></param>
        private void MenuCustomeHeaderButton(bool isInit, bool isEdit, bool isSave)
        {
            tacMain.CustomHeaderButtons[0].Visible = isInit;
            tacMain.CustomHeaderButtons[1].Visible = isEdit;
            tacMain.CustomHeaderButtons[2].Visible = isSave;
        }

        /// <summary>
        /// 상단 메뉴 버튼의 즐겨찾기 이미지를 표시합니다.
        /// </summary>
        private void ShowBookmarkImage()
        {
            try
            {
                // 등록된 즐겨찾기를 조회합니다.
                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetList, SqlDbType.Char);
                inParams.Add("@i_PersonID", AppConfig.Login.PersonID, SqlDbType.VarChar);

                var table = SqlHelper.GetDataTable("xp_Bookmark", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                // 등록된 즐겨찾기에 해당하는 버튼에 이미지를 표시합니다.
                if (table != null && table.Rows.Count > 0)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        var button = (this.Controls.Find($"btn{row["MenuID"]}", true)[0] as SimpleButton);
                        button.ImageOptions.ImageUri.Uri = "Feature;Size16x16;Colored;";
                        button.ImageOptions.ImageToTextAlignment = ImageAlignToText.RightCenter;
                    }
                }
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 상단 메뉴 버튼의 속성을 변경합니다.(일반모드 / 즐겨찾기 수정모드)
        /// </summary>
        /// <param name="isNormalState"></param>
        private void ChangeMenuButtonState(bool isNormalState)
        {
            // 상단 메뉴의 버튼 속성을 변경합니다.
            foreach (BaseLayoutItem item in layoutMenu.Items)
            {
                // 상단 메뉴 그룹 버튼은 속성을 변경하지 않는다.
                if (item.OptionsTableLayoutItem.RowIndex > 0)
                {
                    if (item is LayoutControlItem li && li.Control is SimpleButton button)
                    {
                        button.ImageOptions.ImageUri.Uri = "";

                        if (isNormalState)
                        {
                            button.Enabled = true;
                            //button.ForeColor = Color.White;
                            button.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.ControlText;
                            button.AppearanceHovered.ForeColor = Color.FromArgb(200, 200, 200);
                            button.AppearancePressed.ForeColor = Color.FromArgb(200, 200, 200);
                            button.AppearanceDisabled.ForeColor = Color.Silver;
                            isBookmarkEditMode = false;
                        }
                        else
                        {
                            // 상단 버튼 메뉴 : 즐겨찾기 수정시 시스템 설정은 비활성화 한다.
                            if (button.Name.Substring(3, 2) == "99")
                            {
                                button.Enabled = false;
                            }
                            else
                            {
                                if (AppConfig.App.SkinType == "D")
                                {
                                    button.ForeColor = Color.LightGreen;
                                    button.AppearanceHovered.ForeColor = Color.FromArgb(3, 198, 171);
                                    button.AppearancePressed.ForeColor = Color.FromArgb(3, 198, 171);
                                    button.AppearanceDisabled.ForeColor = Color.FromArgb(147, 180, 131);
                                }
                                else
                                {
                                    button.ForeColor = Color.Green;
                                    button.AppearanceHovered.ForeColor = Color.FromArgb(3, 198, 171);
                                    button.AppearancePressed.ForeColor = Color.FromArgb(3, 198, 171);
                                    button.AppearanceDisabled.ForeColor = Color.FromArgb(147, 180, 131);
                                }

                                isBookmarkEditMode = true;
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region 사용자 정의 메서드 - MDI 자식 폼 호출
        /// <summary>
        /// 상단 메뉴 버튼 클릭시 Mdi 자식폼을 호출한다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TableMenuButton_Click(object sender, EventArgs e)
        {
            var button = sender as SimpleButton;

            try
            {
                // 즐겨찾기 수정 모드 일 경우 이미지 표시/숨김을 반복하고 폼 호출은 하지 않습니다.
                if (isBookmarkEditMode)
                {
                    // 시스템관리 메뉴는 즐겨찾기 할 수 없습니다.
                    if (button.Name.Substring(3, 2) == "99")
                        return;

                    if (string.IsNullOrEmpty(button.ImageOptions.ImageUri.Uri))
                        button.ImageOptions.ImageUri.Uri = "Feature;Size16x16;Colored;";
                    else
                        button.ImageOptions.ImageUri.Uri = "";
                }
                else
                {
                    if (button.Tag.CString() == "RemoteAssistance")
                        this.StartRemoteAssistance();
                    else if (button.Tag.CString() == "Logout")
                        this.OnLoad(null);
                    else
                        this.CallMdiChildForm(button.Tag.CString(), button.Name.Replace("btn", ""));

                    // 상단 메뉴 닫기
                    isMenuOpen = false;
                    isBookmarkOpen = false;
                    tacMain.Height = 30;
                }
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// Mdi 자식폼을 호출합니다.
        /// </summary>
        /// <param name="formName"></param>
        /// <param name="menuID"></param>
        private void CallMdiChildForm(string formName, string menuID)
        {
            try
            {
                // 해당하는 폼을 호출 합니다.
                string formLocation = $"{System.Reflection.Assembly.GetEntryAssembly().GetName().Name}.FORMS.";

                // 폼 위치 + 폼명으로 폼 개체 인스턴스를 생성한다.
                var selectedForm = Activator.CreateInstance(Type.GetType(formLocation + formName)) as XtraForm;

                // 폼의 Tag 값을 설정한다.(MenuID, 시작 시각)
                selectedForm.Tag = new FormLog(menuID, DateTime.Now);

                // 환경설정 폼을 호출한 경우 이벤트 등록 : 환경설정 폼이 닫힐 경우 이벤트 발생 -> MDI 폼에서 이벤트 처리
                if (selectedForm.Name == "frmConfig")
                {
                    // 이벤트 등록
                    var fConfig = selectedForm as FORMS.SYSTEM.frmConfig;
                    fConfig.ChangeConfigEvent += new FORMS.SYSTEM.frmConfig.ChangeConfigEventHandler(CreateMenuAndBookmark);
                }

                // 폼을 호출한다.
                Utils.ShowMidChildForm(this, selectedForm);

                isMenuOpen = !isMenuOpen;
                isBookmarkOpen = false;
                tacMain.Height = 30;
            }
            catch (ArgumentNullException)
            {
                string message = $"{"MSG_ERROR_FORM_OPEN".Localization()}\r\n\r\n" +
                                 $"Title : {((formName.IndexOf('.') > 0) ? formName.Split('.')[1].Localization() : formName.Localization())}\r\n" +
                                 $"ID    : {formName}";
                MsgBox.ErrorMessage(message);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        #endregion

        #region 사용자 정의 메서드 - LayoutControlItem 삭제, LayoutControlItem/Button 추가
        /// <summary>
        /// LayoutControl의 Item을 삭제합니다.
        /// </summary>
        /// <param name="layoutControl"></param>
        private void ClearLayoutControlItem(LayoutControl layoutControl)
        {
            layoutControl.BeginUpdate();

            foreach (BaseLayoutItem item in layoutControl.Items)
            {
                if (item is LayoutControlItem li)
                {
                    li.Control.Dispose();
                    li.Dispose();
                }
            }

            // TableLayout 행열 삭제
            layoutControl.Root.OptionsTableLayoutGroup.ColumnDefinitions.Clear();
            layoutControl.Root.OptionsTableLayoutGroup.RowDefinitions.Clear();

            layoutControl.EndUpdate();
        }

        /// <summary>
        /// TableLayoutControl에 버튼을 추가합니다.
        /// </summary>
        /// <param name="layoutControl"></param>
        /// <param name="button"></param>
        /// <param name="name"></param>
        /// <param name="text"></param>
        /// <param name="tag"></param>
        /// <param name="rowIndex"></param>
        /// <param name="colIndex"></param>
        private void AddLayoutButton(LayoutControl layoutControl, SimpleButton button, string name, string text, string tag, int rowIndex, int colIndex)
        {
            // 메뉴 Button 설정
            button.Name = name;
            button.Text = text;
            //button.ForeColor = Color.White;
            button.Tag = tag;
            button.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            button.Font = new Font(button.Font.Name, button.Font.Size);
            button.AppearanceHovered.ForeColor = Color.FromArgb(200, 200, 200);
            button.AppearancePressed.ForeColor = Color.FromArgb(250, 250, 250);
            button.AppearanceDisabled.ForeColor = Color.Silver;
            button.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
            button.ImageOptions.ImageToTextAlignment = ImageAlignToText.RightCenter;

            // LayoutControlItem 설정
            var layoutItem = new LayoutControlItem() { TextVisible = false };
            layoutItem.Control = button;
            layoutItem.OptionsTableLayoutItem.RowIndex = rowIndex;
            layoutItem.OptionsTableLayoutItem.RowSpan = 1;
            layoutItem.OptionsTableLayoutItem.ColumnIndex = colIndex;
            layoutItem.OptionsTableLayoutItem.ColumnSpan = 1;

            // Layout Table에 Item 추가
            layoutControl.Root.AddItem(layoutItem);
        }

        #endregion

        #region 사용자 정의 메서드 - 권한 및 리포트 리스트 조회
        /// <summary>
        /// 권한 및 리포트 리스트를 조회
        /// </summary>
        private void GetFormInformation()
        {
            try
            {
                // 공용버튼 권한정보
                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", "83", SqlDbType.Char);
                inParams.Add("@i_PersonID", AppConfig.Login.PersonID);
                this.permission = SqlHelper.GetDataTable("xp_UserMenu", inParams, out _, out _);

                // 폼별 리포트 형식
                inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetList, SqlDbType.Char);
                this.report = SqlHelper.GetDataTable("xp_FormReport", inParams, out _, out string retMsg);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        #endregion

        #region 사용자 메서드 - 원격지원 실행

        /// <summary>
        /// 원격지원 실행
        /// </summary>
        private void StartRemoteAssistance()
        {
            var filePath = "";

            if (Environment.Is64BitOperatingSystem)
                filePath = @"C:\Program Files (x86)\seetrol\client\SeetrolClient.exe";
            else
                filePath = @"C:\Program Files\seetrol\client\SeetrolClient.exe";

            if (File.Exists(filePath))
                Process.Start(filePath);
            else
                Process.Start(@"http://www.dusontec.com/Remote/Remote.html");
        }

        #endregion

        #region 사용자 메서드 - 스킨에 따른 색상 설정

        /// <summary>
        /// 스킨에 따른 색상 설정
        /// </summary>
        private void SetSkinControlColor()
        {
            if (AppConfig.App.SkinType == "D")
            {
                tacMain.BackColor = Color.FromArgb(78, 81, 97);
                layoutControl2.BackColor = Color.FromArgb(78, 81, 97);
            }
            else
            {
                tacMain.BackColor = Color.FromArgb(235, 236, 239);
                layoutControl2.BackColor = Color.FromArgb(235, 236, 239);
            }
        }

        #endregion

        #region ##### Skin 적용 - 나중에 삭제 #####
        private void cmbSkins_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DevExpress.LookAndFeel.UserLookAndFeel.Default.SkinName = (sender as ComboBoxEdit).Text;
            }
            catch { }
        }
        #endregion
    }
}