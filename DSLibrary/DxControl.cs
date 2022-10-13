using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Menu;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraLayout;
using DevExpress.XtraPrinting;
using DevExpress.XtraTab;
using DevExpress.XtraTreeList;

/// <summary>
/// DevExpress Control 초기화 클래스
/// </summary>
namespace DSLibrary
{
    #region 열거형 선언
    /// <summary>
    /// GridView 포맷 저장 형식
    /// </summary>
    public enum GridFormat
    {
        /// <summary> 기본 설정값 </summary>
        Default,
        /// <summary> 현재 설정값 </summary>
        Current,
    }

    #endregion

    public class DxControl
    {
        #region 클래스 변수 선언
        public delegate bool BEEventHandler(object sender);
        public static event BEEventHandler ButtonEditEvent;
        #endregion

        #region 폼 컨트롤 초기화(모양 및 공통 속성)
        /// <summary>
        /// 폼의 모든 컨트롤을 초기화 디자인을 설정한다.
        /// </summary>
        /// <param name="controls">Control.ControlCollection 개체</param>
        public static void InitialControl(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                if (control.HasChildren)
                    InitialControl(control.Controls);

                #region LayoutControl
                if (control is LayoutControl layoutControl)
                {
                    layoutControl.AllowCustomization = false;
                    layoutControl.OptionsFocus.AllowFocusGroups = false;
                    layoutControl.OptionsFocus.EnableAutoTabOrder = false;
                    layoutControl.Appearance.DisabledLayoutItem.ForeColor = SystemColors.WindowText;
                    layoutControl.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);

                    foreach (BaseLayoutItem item in layoutControl.Items)
                    {
                        if (item is LayoutControlGroup layoutGroup)
                        {
                            layoutGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
                            layoutGroup.ExpandButtonVisible = true;

                            if (layoutGroup.TextVisible)
                            {
                                layoutGroup.AppearanceGroup.FontStyleDelta = FontStyle.Bold;
                                layoutGroup.HeaderButtonsLocation = GroupElementLocation.AfterText;
                                layoutGroup.Text = layoutGroup.Text.Localization();
                            }

                            foreach (DevExpress.XtraEditors.ButtonPanel.BaseButton baseButton in layoutGroup.CustomHeaderButtons)
                            {
                                baseButton.Caption = baseButton.Caption.Localization();
                            }
                        }
                        else if (item is LayoutControlItem layoutItem)
                        {
                            layoutItem.AppearanceItemCaption.TextOptions.HAlignment = HorzAlignment.Far;

                            // 텍스트가 공백이 아니고 Visible 상태이면
                            if (layoutItem.Text.Trim() != "" && layoutItem.TextVisible)
                            {
                                //layoutItem.Text = layoutItem.Text.Localization();

                                // LayoutItem의 Tag 속성값이 "Y" 이면 필수 입력을 의미하며 텍스트의 전경색을 변경
                                if (layoutItem.Tag.CString().ToUpper() == "Y")
                                    layoutItem.AppearanceItemCaption.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Critical;
                                //{
                                //layoutItem.AppearanceItemCaption.Options.UseTextOptions = true;
                                //layoutItem.AppearanceItemCaption.FontStyleDelta = FontStyle.Underline;
                                //}

                                // Control이 SpinEdit이면 용어집 정보와 같이 세팅해준다.
                                if (layoutItem.Control is SpinEdit spinEdit)
                                    spinEdit.SetMask(layoutItem.Text);

                                layoutItem.Text = layoutItem.Text.Localization();
                            }
                        }
                    }
                }

                #endregion

                #region GridControl/GridView
                else if (control is GridControl grid)
                {
                    var view = grid.MainView as GridView;
                    grid.DataSource = null;

                    view.ColumnPanelRowHeight = 23;
                    view.RowHeight = 20;
                    view.VertScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always;

                    view.Appearance.HeaderPanel.Options.UseTextOptions = true;
                    view.Appearance.HeaderPanel.TextOptions.HAlignment = HorzAlignment.Center;
                    view.Appearance.HeaderPanel.TextOptions.WordWrap = WordWrap.Wrap;
                    view.Appearance.VertLine.Options.UseBackColor = true;
                    view.Appearance.VertLine.BackColor = UserColors.LineColor;
                    view.Appearance.HorzLine.Options.UseBackColor = true;
                    view.Appearance.HorzLine.BackColor = UserColors.LineColor;
                    view.OptionsSelection.EnableAppearanceFocusedCell = false;
                    view.OptionsSelection.EnableAppearanceHideSelection = false;
                    view.OptionsNavigation.UseTabKey = false;
                    view.OptionsView.BestFitMaxRowCount = 35;

                    #region GridView - 컬럼 고정 단축 메뉴 추가 (컬럼 고정과 밴드 고정이 처리가 달라 별도로 처리)
                    if (view.GetType() == typeof(GridView))
                    {
                        view.PopupMenuShowing += (s, e) =>
                        {
                            if (e.Menu == null)
                                return;

                            // 컬럼고정(없음, 좌측, 우측) 항목을 추가 (그리드 컬럼 헤드 영역 클릭시 표시됨)
                            if (e.MenuType == GridMenuType.Column)
                            {
                                var colMenu = e.Menu as GridViewColumnMenu;

                                DXMenuItem item = null;
                                item = new DXMenuItem("열 고정 해제") { BeginGroup = true };
                                item.ImageOptions.ImageUri.Uri = "GridLines;Size16x16;Colored";
                                item.Click += (sender1, e1) => { colMenu.Column.Fixed = FixedStyle.None; };
                                e.Menu.Items.Add(item);

                                item = new DXMenuItem("열을 좌측에 고정");
                                item.ImageOptions.ImageUri.Uri = "AlignVerticalLeft;Size16x16;Colored";
                                item.Click += (sender1, e1) => { colMenu.Column.Fixed = FixedStyle.Left; };
                                e.Menu.Items.Add(item);

                                item = new DXMenuItem("열을 우측에 고정");
                                item.ImageOptions.ImageUri.Uri = "AlignVerticalRight;Size16x16;Colored";
                                item.Click += (sender1, e1) => { colMenu.Column.Fixed = FixedStyle.Right; };
                                e.Menu.Items.Add(item);
                            }
                        };
                    }
                    #endregion

                    else if (view.GetType() == typeof(BandedGridView))
                    {
                        var bandView = view as BandedGridView;
                        bandView.Appearance.BandPanel.Options.UseTextOptions = true;
                        //bandView.Appearance.BandPanel.FontStyleDelta = FontStyle.Bold;
                        bandView.Appearance.BandPanel.TextOptions.HAlignment = HorzAlignment.Center;
                        bandView.OptionsView.ShowBands = false;
                        bandView.OptionsView.ShowColumnHeaders = false;
                        bandView.OptionsView.ShowGroupPanel = false;
                        bandView.OptionsBehavior.Editable = false;
                        bandView.OptionsCustomization.AllowChangeColumnParent = false;
                        bandView.OptionsCustomization.AllowBandMoving = false;

                        #region BandedGridView - 컬럼 고정 단축 메뉴 추가
                        bandView.PopupMenuShowing += (s, e) =>
                        {
                            if (e.Menu == null)
                                return;

                            // 컬럼고정(없음, 좌측, 우측) 항목을 추가 (밴드 영역 클릭시 표시됨, 하위 밴드 고정 불가)
                            if (e.MenuType == GridMenuType.Column)
                            {
                                var colMenu = e.Menu as GridViewBandMenu;

                                DXMenuItem item = null;
                                item = new DXMenuItem("열 고정 해제") { BeginGroup = true };
                                item.ImageOptions.ImageUri.Uri = "GridLines;Size16x16;Colored";
                                item.Click += (sender1, e1) => { colMenu.Band.Fixed = FixedStyle.None; };
                                e.Menu.Items.Add(item);

                                item = new DXMenuItem("열을 좌측에 고정");
                                item.ImageOptions.ImageUri.Uri = "AlignVerticalLeft;Size16x16;Colored";
                                item.Click += (sender1, e1) => { colMenu.Band.Fixed = FixedStyle.Left; };
                                e.Menu.Items.Add(item);

                                item = new DXMenuItem("열을 우측에 고정");
                                item.ImageOptions.ImageUri.Uri = "AlignVerticalRight;Size16x16;Colored";
                                item.Click += (sender1, e1) => { colMenu.Band.Fixed = FixedStyle.Right; };
                                e.Menu.Items.Add(item);
                            }
                        };

                        #endregion
                    }

                    // 데이터가 없을 경우 "No Data" 표시
                    view.CustomDrawEmptyForeground += (s, e) =>
                    {
                        var text = new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center
                        };

                        e.Appearance.Font = new Font("IBM Plex Sans KR", 11, FontStyle.Regular);
                        e.Appearance.DrawString(e.Cache, "[ No Data ]", e.Bounds, e.Cache.GetSolidBrush(Color.LightSlateGray), text);
                    };

                    view.CustomDrawRowIndicator += (s, e) =>
                    {
                        if (e.Info.IsRowIndicator && e.RowHandle >= 0)
                            e.Info.DisplayText = (e.RowHandle + 1).ToString();

                        //e.Info.ImageIndex = -1;
                        //e.Painter.DrawObject(e.Info);
                    };

                    grid.TabStop = false;

                    // ContextMenu 사용 시 스킨 적용 오류로 PopupMenu로 변경함
                    view.PopupMenuShowing += GridView_PopupMenuShowing;
                }
                #endregion

                #region TreeList
                else if (control is TreeList tree)
                {
                    tree.DataSource = null;
                    tree.ParentFieldName = "ParentField";
                    tree.KeyFieldName = "KeyField";

                    tree.Appearance.HeaderPanel.Options.UseTextOptions = true;
                    tree.Appearance.HeaderPanel.TextOptions.HAlignment = HorzAlignment.Center;
                    tree.Appearance.HeaderPanel.TextOptions.WordWrap = WordWrap.Wrap;
                    tree.Appearance.VertLine.Options.UseBackColor = true;
                    tree.Appearance.HorzLine.Options.UseBackColor = true;
                    tree.Appearance.VertLine.BackColor = UserColors.LineColor;
                    tree.Appearance.HorzLine.BackColor = UserColors.LineColor;
                    tree.OptionsView.ShowIndicator = false;
                    tree.OptionsView.ShowHorzLines = true;
                    tree.OptionsView.ShowVertLines = true;
                    tree.OptionsView.AutoWidth = false;
                    tree.OptionsBehavior.Editable = false;
                    tree.OptionsBehavior.ReadOnly = true;
                    tree.OptionsBehavior.AllowExpandOnDblClick = true;
                    tree.ShowButtonMode = ShowButtonModeEnum.ShowOnlyInEditor;
                    tree.OptionsSelection.EnableAppearanceFocusedCell = false;
                    tree.OptionsView.ShowAutoFilterRow = false;
                    tree.OptionsNavigation.UseTabKey = false;
                    tree.VertScrollVisibility = ScrollVisibility.Auto;
                    tree.OptionsView.TreeLineStyle = DevExpress.XtraTreeList.LineStyle.Solid;
                    tree.ColumnPanelRowHeight = 22;
                    tree.RowHeight = 20;
                    tree.BestFitColumns();

                    tree.CustomDrawEmptyArea += (s, e) =>
                    {
                        var text = new StringFormat()
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center
                        };

                        e.Appearance.Font = new Font("IBM Plex Sans KR", 11, FontStyle.Regular);

                        // 데이터가 조회되어도 문자가 표시되어 데이터가 있는 경우는 동일한 위치에 공백을 출력한다.
                        var dispText = ((s as TreeList).Nodes.Count < 1) ? "[ No Data ]" : "";
                        e.Appearance.DrawString(e.Cache, dispText, e.Bounds, e.Cache.GetSolidBrush(Color.LightSlateGray), text);

                        e.Handled = true;
                    };
                }
                #endregion

                #region Simple Button
                else if (control is SimpleButton simpleButton)
                {
                    if (simpleButton.Text.Trim() != "")
                    {
                        simpleButton.Text = simpleButton.Text.Localization();
                        simpleButton.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
                        simpleButton.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
                        simpleButton.ImageOptions.ImageToTextAlignment = ImageAlignToText.LeftCenter;
                        simpleButton.ImageOptions.ImageToTextIndent = 5;

                        simpleButton.Appearance.Options.UseForeColor = true;
                        simpleButton.Appearance.Options.UseFont = true;
                        //simpleButton.Appearance.ForeColor = Color.White;
                        simpleButton.Appearance.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.ControlText;
                        simpleButton.AppearanceDisabled.Options.UseForeColor = true;
                        simpleButton.AppearanceDisabled.ForeColor = Color.Silver;
                        simpleButton.AppearanceHovered.Options.UseForeColor = true;
                        simpleButton.AppearanceHovered.ForeColor = Color.FromArgb(237, 125, 49);
                        simpleButton.AppearanceHovered.FontStyleDelta = FontStyle.Bold;
                        simpleButton.AppearanceHovered.BorderColor = Color.FromArgb(78, 81, 97);
                        simpleButton.AppearancePressed.Options.UseForeColor = true;
                        simpleButton.AppearancePressed.ForeColor = Color.FromArgb(187, 100, 4);
                        simpleButton.AppearancePressed.BorderColor = Color.FromArgb(78, 81, 97);
                        simpleButton.AppearancePressed.BackColor = Color.FromArgb(78, 81, 97);
                    }

                    // 행추가(+), 행삭제(+) 버튼의 Font를 설정한다.
                    if (simpleButton.Name.Contains("btnAddRow") || simpleButton.Name.Contains("btnDelRow"))
                    {
                        var buttonFont = new Font("IBM Plex Sans KR", 18, FontStyle.Bold);
                        simpleButton.Appearance.Font = buttonFont;
                        simpleButton.AppearanceHovered.Font = buttonFont;
                    }
                }
                #endregion

                #region XtraTabControl
                else if (control is XtraTabControl tabControl)
                {
                    tabControl.TabPages.ToList().ForEach(tp => { tp.Text = tp.Text.Localization(); });

                    for (int i = 0; i < tabControl.CustomHeaderButtons.Count; i++)
                    {
                        //tabControl.CustomHeaderButtons[i].Appearance.ForeColor = Color.White;
                        tabControl.CustomHeaderButtons[i].Appearance.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.ControlText;
                        //tabControl.CustomHeaderButtons[i].Caption = $"  {tabControl.CustomHeaderButtons[i].Caption.Localization().Trim()}  ";
                    }
                }
                #endregion

                #region BaseEdit
                else if (control is BaseEdit baseEdit)
                {
                    baseEdit.BorderStyle = BorderStyles.UltraFlat;
                    baseEdit.EnterMoveNextControl = true;
                    baseEdit.Properties.NullText = string.Empty;
                    baseEdit.Properties.AppearanceDisabled.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.WindowText; // Color.White;
                    baseEdit.Properties.AppearanceDisabled.BackColor = SystemColors.ControlDark;
                    //baseEdit.Properties.AppearanceDisabled.BorderColor = Color.LightGray;

                    if (!baseEdit.Enabled)
                        baseEdit.BorderStyle = BorderStyles.NoBorder;

                    if (baseEdit.Properties.ReadOnly)
                    {
                        baseEdit.TabStop = false;

                        // BorderStyles.UltraFlat일 경우 ReadOnly 컨트롤의 Border가 표시 안됨 => BackColor 설정
                        baseEdit.Properties.Appearance.Options.UseBackColor = true;
                        baseEdit.Properties.Appearance.BackColor = Color.FromArgb(94, 94, 94);
                    }

                    // --------------------------------------------------------------
                    // ButtonEdit
                    // --------------------------------------------------------------
                    if (baseEdit.GetType() == typeof(ButtonEdit))
                    {
                        var buttonEdit = baseEdit as ButtonEdit;
                        buttonEdit.Properties.Buttons.Clear();
                        buttonEdit.Properties.AppearanceReadOnly.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.WindowText;
                        buttonEdit.Properties.AppearanceReadOnly.BackColor = Color.FromArgb(152, 152, 152); // DevExpress.LookAndFeel.DXSkinColors.ForeColors.DisabledText;
                        buttonEdit.Properties.Buttons.Add(new EditorButton(ButtonPredefines.Glyph));
                        buttonEdit.Properties.Buttons[0].ImageOptions.ImageUri.Uri = "Zoom;Size16x16;Colored";
                        buttonEdit.Properties.Buttons[0].Tag = "FIND";
                        buttonEdit.Properties.Buttons.Add(new EditorButton(ButtonPredefines.Glyph));
                        buttonEdit.Properties.Buttons[1].ImageOptions.ImageUri.Uri = "RemovePivotField;Size16x16;Colored";
                        buttonEdit.Properties.Buttons[1].Tag = "CLEAR";

                        buttonEdit.ButtonClick += (s, e) =>
                        {
                            if (e.Button.Tag.CString() == "CLEAR")
                            {
                                (s as ButtonEdit).EditValue = null;
                                (s as ButtonEdit).Tag = null;
                            }
                            else if (e.Button.Tag.CString() == "FIND")
                            {
                                if (ButtonEditEvent != null)
                                    ButtonEditEvent(s);
                                else
                                    MsgBox.ErrorMessage("Event Error!");
                            }
                        };

                        buttonEdit.KeyDown += (s, e) =>
                        {
                            if (e.KeyCode == Keys.Enter)
                            {
                                if (ButtonEditEvent != null)
                                {
                                    //ButtonEditEvent(s);

                                    if (ButtonEditEvent(s) == false)
                                        e.Handled = true;
                                }
                                else
                                    MsgBox.ErrorMessage("Event Error!");
                            }
                        };
                    }

                    // --------------------------------------------------------------
                    // SpinEdit
                    // --------------------------------------------------------------
                    //else if (baseEdit.GetType() == typeof(SpinEdit))
                    //{
                    //    var spinEdit = control as SpinEdit;
                    //}

                    // --------------------------------------------------------------
                    // LookUpEdit
                    // --------------------------------------------------------------
                    else if (baseEdit.GetType() == typeof(LookUpEdit))
                    {
                        var lookUpEdit = baseEdit as LookUpEdit;

                        lookUpEdit.Properties.ShowFooter = false;
                        lookUpEdit.Properties.DropDownItemHeight = 20;
                        lookUpEdit.Properties.DropDownRows = 12;
                        lookUpEdit.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;

                        lookUpEdit.Properties.AppearanceDropDownHeader.Options.UseTextOptions = true;
                        lookUpEdit.Properties.AppearanceDropDownHeader.TextOptions.HAlignment = HorzAlignment.Center;
                        lookUpEdit.Properties.AppearanceDropDownHeader.FontStyleDelta = FontStyle.Bold;

                        lookUpEdit.Properties.Buttons.Clear();
                        lookUpEdit.Properties.Buttons.Add(new EditorButton(ButtonPredefines.Glyph));
                        lookUpEdit.Properties.Buttons[0].ImageOptions.ImageUri.Uri = "Table;Size16x16;Colored";
                        lookUpEdit.Properties.Buttons[0].Tag = "FIND";
                        lookUpEdit.Properties.Buttons.Add(new EditorButton(ButtonPredefines.Glyph));
                        lookUpEdit.Properties.Buttons[1].ImageOptions.ImageUri.Uri = "RemovePivotField;Size16x16;Colored";
                        lookUpEdit.Properties.Buttons[1].Tag = "CLEAR";

                        lookUpEdit.ButtonClick += (s, e) =>
                        {
                            if (e.Button.Tag.CString() == "CLEAR")
                                (s as LookUpEdit).EditValue = null;
                        };

                        // [*** 테스트 필요 ***] Enter Key 입력시 목록을 DropDown 시킴
                        lookUpEdit.KeyDown += (s, e) =>
                        {
                            if (e.KeyCode == Keys.Enter)
                            {
                                var gle = s as GridLookUpEdit;
                                gle.ShowPopup();
                                e.Handled = true;
                            }
                        };

                        // [*** 테스트 필요 ***] DropDown된 목록에서 항목 선택 후 Enter Key 입력시 다음 Control로 Focus 이동
                        lookUpEdit.KeyPress += (s, e) =>
                        {
                            if (e.KeyChar == (char)Keys.Enter)
                                SendKeys.Send("{TAB}");
                        };
                    }

                    // --------------------------------------------------------------
                    // GridLookUpEdit
                    // --------------------------------------------------------------
                    else if (baseEdit.GetType() == typeof(GridLookUpEdit))
                    {
                        var gridLookUpEdit = baseEdit as GridLookUpEdit;
                        gridLookUpEdit.Properties.ShowFooter = false;
                        gridLookUpEdit.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
                        gridLookUpEdit.Properties.View.RowHeight = 20;
                        gridLookUpEdit.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;

                        gridLookUpEdit.Properties.View.Appearance.HeaderPanel.Options.UseTextOptions = true;
                        gridLookUpEdit.Properties.View.Appearance.HeaderPanel.TextOptions.HAlignment = HorzAlignment.Center;
                        gridLookUpEdit.Properties.View.Appearance.HeaderPanel.FontStyleDelta = FontStyle.Bold;
                        //gridLookUpEdit.Properties.View.Appearance.HeaderPanel.ForeColor = Color.DarkBlue;
                        gridLookUpEdit.Properties.View.Appearance.VertLine.Options.UseBackColor = true;
                        gridLookUpEdit.Properties.View.Appearance.HorzLine.Options.UseBackColor = true;
                        gridLookUpEdit.Properties.View.Appearance.VertLine.BackColor = UserColors.LineColor;
                        gridLookUpEdit.Properties.View.Appearance.HorzLine.BackColor = UserColors.LineColor;
                        gridLookUpEdit.Properties.View.OptionsBehavior.Editable = false;
                        gridLookUpEdit.Properties.View.OptionsBehavior.AutoExpandAllGroups = true;
                        //gridLookUpEdit.Properties.View.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
                        gridLookUpEdit.Properties.View.OptionsView.ColumnAutoWidth = true;
                        gridLookUpEdit.Properties.View.OptionsView.ShowColumnHeaders = true;
                        gridLookUpEdit.Properties.View.OptionsView.ShowIndicator = false;
                        gridLookUpEdit.Properties.View.OptionsView.ShowAutoFilterRow = true;
                        gridLookUpEdit.Properties.View.OptionsView.ShowHorizontalLines = DefaultBoolean.False;
                        //gridLookUpEdit.Properties.View.OptionsView.ShowVerticalLines = DefaultBoolean.False;

                        gridLookUpEdit.Properties.Buttons.Clear();
                        gridLookUpEdit.Properties.Buttons.Add(new EditorButton(ButtonPredefines.Glyph));
                        gridLookUpEdit.Properties.Buttons[0].ImageOptions.ImageUri.Uri = "Table;Size16x16;Colored";
                        gridLookUpEdit.Properties.Buttons[0].Tag = "FIND";
                        gridLookUpEdit.Properties.Buttons.Add(new EditorButton(ButtonPredefines.Glyph));
                        gridLookUpEdit.Properties.Buttons[1].ImageOptions.ImageUri.Uri = "RemovePivotField;Size16x16;Colored";
                        gridLookUpEdit.Properties.Buttons[1].Tag = "CLEAR";

                        gridLookUpEdit.ButtonClick += (s, e) =>
                        {
                            if (e.Button.Tag.CString() == "CLEAR")
                                (s as GridLookUpEdit).EditValue = null;
                        };

                        // Enter Key 입력시 목록을 DropDown 시킴
                        gridLookUpEdit.KeyDown += (s, e) =>
                        {
                            if (e.KeyCode == Keys.Enter)
                            {
                                var gle = s as GridLookUpEdit;
                                gle.ShowPopup();
                                e.Handled = true;
                            }
                        };

                        // DropDown된 목록에서 항목 선택 후 Enter Key 입력시 다음 Control로 Focus 이동
                        gridLookUpEdit.Properties.View.KeyPress += (s, e) =>
                        {
                            if (e.KeyChar == (char)Keys.Enter)
                                SendKeys.Send("{TAB}");
                        };
                    }

                    // --------------------------------------------------------------
                    // SearchLookUpEdit
                    // --------------------------------------------------------------
                    else if (baseEdit.GetType() == typeof(SearchLookUpEdit))
                    {
                        var searchLookUpEdit = baseEdit as SearchLookUpEdit;
                        searchLookUpEdit.Properties.ShowFooter = false;
                        searchLookUpEdit.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
                        //searchLookUpEdit.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
                        //searchLookUpEdit.Properties.PopupFormSize = new Size(900, 350); // 초기 창 크기

                        searchLookUpEdit.Properties.View.Appearance.HeaderPanel.Options.UseTextOptions = true;
                        searchLookUpEdit.Properties.View.Appearance.HeaderPanel.FontStyleDelta = FontStyle.Bold;
                        searchLookUpEdit.Properties.View.Appearance.HeaderPanel.TextOptions.HAlignment = HorzAlignment.Center;
                        searchLookUpEdit.Properties.View.Appearance.VertLine.Options.UseBackColor = true;
                        searchLookUpEdit.Properties.View.Appearance.HorzLine.Options.UseBackColor = true;
                        searchLookUpEdit.Properties.View.Appearance.VertLine.BackColor = UserColors.LineColor;
                        searchLookUpEdit.Properties.View.Appearance.HorzLine.BackColor = UserColors.LineColor;
                        searchLookUpEdit.Properties.View.OptionsBehavior.AutoExpandAllGroups = true;
                        //searchLookUpEdit.Properties.View.OptionsBehavior.Editable = false;
                        searchLookUpEdit.Properties.View.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
                        searchLookUpEdit.Properties.View.OptionsView.ShowAutoFilterRow = true;
                        searchLookUpEdit.Properties.View.OptionsView.ColumnAutoWidth = true;

                        searchLookUpEdit.Properties.Buttons.Clear();
                        searchLookUpEdit.Properties.Buttons.Add(new EditorButton(ButtonPredefines.Glyph));
                        searchLookUpEdit.Properties.Buttons[0].ImageOptions.ImageUri.Uri = "Zoom;Size16x16;Colored";
                        searchLookUpEdit.Properties.Buttons[0].Tag = "FIND";
                        searchLookUpEdit.Properties.Buttons.Add(new EditorButton(ButtonPredefines.Glyph));
                        searchLookUpEdit.Properties.Buttons[1].ImageOptions.ImageUri.Uri = "RemovePivotField;Size16x16;Colored";
                        searchLookUpEdit.Properties.Buttons[1].Tag = "CLEAR";

                        searchLookUpEdit.ButtonClick += (s, e) =>
                        {
                            if (e.Button.Tag.CString() == "CLEAR")
                                (s as SearchLookUpEdit).EditValue = null;
                        };

                        // [*** 테스트 필요 ***] Enter Key 입력시 목록을 DropDown 시킴
                        searchLookUpEdit.KeyDown += (s, e) =>
                        {
                            if (e.KeyCode == Keys.Enter)
                            {
                                var sle = s as SearchLookUpEdit;
                                sle.ShowPopup();
                                e.Handled = true;
                            }
                        };

                        // [*** 테스트 필요 ***] DropDown된 목록에서 항목 선택 후 Enter Key 입력시 다음 Control로 Focus 이동
                        searchLookUpEdit.Properties.View.KeyPress += (s, e) =>
                        {
                            if (e.KeyChar == (char)Keys.Enter)
                                SendKeys.Send("{TAB}");
                        };
                    }

                    // --------------------------------------------------------------
                    // RadioGroup
                    // --------------------------------------------------------------
                    else if (baseEdit.GetType() == typeof(RadioGroup))
                    {
                        var radioGroup = baseEdit as RadioGroup;
                        //radioGroup.AutoSizeInLayoutControl = true;
                        radioGroup.Properties.AllowMouseWheel = false;
                    }
                    // --------------------------------------------------------------
                    // DateEdit
                    // --------------------------------------------------------------
                    else if (baseEdit.GetType() == typeof(DateEdit))
                    {
                        var dateEdit = baseEdit as DateEdit;
                        //dateEdit.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
                        dateEdit.Properties.ShowToday = true;
                        //dateEdit.Properties.DrawCellLines = true;

                        // 필수 입력 항목 빈값 입력 불가
                        if (dateEdit.Tag != null)
                            dateEdit.Properties.AllowNullInput = DefaultBoolean.False;

                        // 사용자 정의 버튼 추가(날짜 선택, 초기화)
                        dateEdit.Properties.Buttons.Clear();
                        dateEdit.Properties.Buttons.Add(new EditorButton(ButtonPredefines.Glyph));
                        dateEdit.Properties.Buttons[0].ImageOptions.ImageUri.Uri = "aDateOccurring;Size16x16;Colored";
                        dateEdit.Properties.Buttons[0].Tag = "FIND";
                        dateEdit.Properties.Buttons.Add(new EditorButton(ButtonPredefines.Glyph));
                        dateEdit.Properties.Buttons[1].ImageOptions.ImageUri.Uri = "RemovePivotField;Size16x16;Colored";
                        dateEdit.Properties.Buttons[1].Tag = "CLEAR";

                        dateEdit.ButtonClick += (s, e) =>
                        {
                            if (e.Button.Tag.CString() == "CLEAR")
                                (s as DateEdit).SetDefaultDate();
                        };

                        dateEdit.SetDefaultDate();
                    }

                    // --------------------------------------------------------------
                    // CheckedComboBoxEdit
                    // --------------------------------------------------------------
                    else if (baseEdit.GetType() == typeof(CheckedComboBoxEdit))
                    {
                        var checkedComboBoxEdit = baseEdit as CheckedComboBoxEdit;
                        checkedComboBoxEdit.Properties.DropDownRows = 15;

                        checkedComboBoxEdit.Properties.Buttons.Clear();
                        checkedComboBoxEdit.Properties.Buttons.Add(new EditorButton(ButtonPredefines.Glyph));
                        checkedComboBoxEdit.Properties.Buttons[0].ImageOptions.ImageUri.Uri = "ListBullets;Size16x16;Colored";
                        checkedComboBoxEdit.Properties.Buttons[0].Tag = "Find";
                        checkedComboBoxEdit.Properties.Buttons.Add(new EditorButton(ButtonPredefines.Glyph));
                        checkedComboBoxEdit.Properties.Buttons[1].ImageOptions.ImageUri.Uri = "RemovePivotField;Size16x16;Colored";
                        checkedComboBoxEdit.Properties.Buttons[1].Tag = "CLEAR";

                        checkedComboBoxEdit.ButtonClick += (s, e) =>
                        {
                            if (e.Button.Tag.CString() == "CLEAR")
                                (s as CheckedComboBoxEdit).SetEditValue(null);
                        };
                    }

                    // --------------------------------------------------------------
                    // CheckEdit
                    // --------------------------------------------------------------
                    else if (baseEdit.GetType() == typeof(CheckEdit))
                    {
                        var checkEdit = baseEdit as CheckEdit;
                        checkEdit.BorderStyle = BorderStyles.NoBorder;
                        //checkEdit.BorderStyle = BorderStyles.NoBorder;
                        //checkEdit.Properties.Appearance.BackColor = Color.White;
                        checkEdit.Properties.ValueChecked = "0";
                        checkEdit.Properties.ValueUnchecked = "1";
                        checkEdit.Properties.AllowGrayed = false;
                        checkEdit.Properties.AutoHeight = true;

                        checkEdit.EditValue = (checkEdit.Name.Contains("UseClss")) ? "0" : "1";
                    }

                    // --------------------------------------------------------------
                    // MemoEdit
                    // --------------------------------------------------------------
                    else if (baseEdit.GetType() == typeof(MemoEdit))
                    {
                        var memoEdit = baseEdit as MemoEdit;

                        memoEdit.EnterMoveNextControl = false;  // MemoEdit는 기능 해제
                    }

                    baseEdit.Enter += (s, e) => { baseEdit.SelectAll(); };
                }
                #endregion

                #region Chart Control
                else if (control is ChartControl chart)
                {
                    chart.DataSource = null;
                    //chart.Legend.Visibility = DefaultBoolean.False;

                    // Title Font and Localization
                    foreach (ChartTitle title in chart.Titles)
                    {
                        title.Font = new Font(AppConfig.App.FontName, 18.0f, FontStyle.Bold);
                        title.Text = title.Text.Localization();
                    }

                    Font chartFont = new Font(AppConfig.App.FontName, AppConfig.App.FontSize);

                    // Legend Font
                    chart.Legend.Font = chartFont;

                    // Series Label Font
                    foreach (Series series in chart.Series)
                        series.Label.Font = chartFont;

                    // Crosshair Font
                    chart.CrosshairOptions.GroupHeaderTextOptions.Font = chartFont;
                    chart.CrosshairOptions.CrosshairLabelTextOptions.Font = chartFont;

                    // Set XYDiagram : 가로/세로축, Label Font/Color
                    if (chart.Diagram is XYDiagram diagram)
                    {
                        diagram.AxisX.Title.Visibility = DevExpress.Utils.DefaultBoolean.True;
                        diagram.AxisX.Title.Font = new Font(AppConfig.App.FontName, AppConfig.App.FontSize, FontStyle.Bold);
                        //diagram.AxisX.Title.TextColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Critical;
                        diagram.AxisX.Label.Font = new Font(AppConfig.App.FontName, AppConfig.App.FontSize);

                        diagram.AxisY.Title.Visibility = DevExpress.Utils.DefaultBoolean.True;
                        diagram.AxisY.Title.Font = new Font(AppConfig.App.FontName, AppConfig.App.FontSize, FontStyle.Bold);
                        //diagram.AxisY.Title.TextColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Critical;
                        diagram.AxisY.Label.Font = new Font(AppConfig.App.FontName, AppConfig.App.FontSize);

                        foreach (SecondaryAxisY  secondaryAxisY in diagram.SecondaryAxesY)
                        {
                            secondaryAxisY.Title.Visibility = DevExpress.Utils.DefaultBoolean.True;
                            secondaryAxisY.Title.Font = new Font(AppConfig.App.FontName, AppConfig.App.FontSize, FontStyle.Bold);
                            //secondaryAxisY.Title.TextColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Critical;
                            secondaryAxisY.Label.Font = new Font(AppConfig.App.FontName, AppConfig.App.FontSize);
                        }
                    }
                } 
                #endregion
            }
        }

        #endregion

        #region 폼 컨트롤 입력값 초기화
        /// <summary>
        /// 폼 전체 컨트롤의 입력값을 초기화한다.
        /// </summary>
        /// <param name="controls">Control.ControlCollection 개체</param>
        public static void ClearControl(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                if (control.HasChildren)
                    ClearControl(control.Controls);

                if (control is GridControl grid)
                {
                    grid.DataSource = null;
                }
                else if (control is TreeList tree)
                {
                    tree.DataSource = null;
                }
                else if (control is BaseEdit baseEdit)
                {
                    if (baseEdit.GetType() == typeof(DateEdit))
                    {
                        (baseEdit as DateEdit).SetDefaultDate();
                    }
                    else if (baseEdit.GetType() == typeof(CheckEdit))
                    {
                        var checkEdit = baseEdit as CheckEdit;
                        checkEdit.EditValue = (checkEdit.Name.Contains("UseClss")) ? "0" : "1";
                    }
                    else if (baseEdit.GetType() == typeof(SpinEdit))
                    {
                        (baseEdit as SpinEdit).EditValue = 0;
                    }
                    else
                    {
                        baseEdit.EditValue = null;
                    }
                }
            }
        }

        /// <summary>
        /// LayoutControlGroup 內 컨트롤의 입력값을 초기화한다.
        /// </summary>
        /// <param name="layoutGroup">LayoutControlGroup 컨트롤</param>
        public static void ClearControl(LayoutControlGroup layoutGroup)
        {
            foreach (BaseLayoutItem item in layoutGroup.Items)
            {
                if (item is LayoutControlGroup loGroup)
                {
                    ClearControl(loGroup);
                }
                else if (item is LayoutControlItem layoutItem)
                {
                    if (layoutItem.Control is GridControl grid)
                    {
                        grid.DataSource = null;
                    }
                    else if (layoutItem.Control is TreeList tree)
                    {
                        tree.DataSource = null;
                    }
                    else if (layoutItem.Control is BaseEdit baseEdit)
                    {
                        if (baseEdit.GetType() == typeof(DateEdit))
                        {
                            (baseEdit as DateEdit).SetDefaultDate();
                        }
                        else if (baseEdit.GetType() == typeof(CheckEdit))
                        {
                            var checkEdit = baseEdit as CheckEdit;
                            checkEdit.EditValue = (checkEdit.Name.Contains("UseClss")) ? "0" : "1";
                        }
                        else if (baseEdit.GetType() == typeof(SpinEdit))
                        {
                            (baseEdit as SpinEdit).EditValue = 0;
                        }
                        else
                        {
                            baseEdit.EditValue = null;
                        }
                    }
                }
            }
        }
        #endregion

        #region GridView PopupMenu
        /// <summary>
        /// Grid에서 PopupMenu 호출 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// /// <param name="e"></param>
        /// <returns></returns>
        private static void GridView_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.HitTest == GridHitTest.RowCell)
                e.Menu = CreateGridViewMenu(sender as GridView);
        }

        /// <summary>
        /// GridControl의 PopupMenu를 설정한다.
        /// </summary>
        /// <param name="grid">GridControl 컨트롤</param>
        /// <returns></returns>
        private static GridViewMenu CreateGridViewMenu(GridView view)
        {
            var menu = new GridViewMenu(view);

            var mnuItemInitialFormat = new DXMenuItem();   // 그리드 형식 초기화
            var mnuItemExportFormat = new DXMenuItem();    // 그리드 형식 내보내기
            var mnuItemImportFormat = new DXMenuItem();    // 그리드 형식 가져오기
            var mnuItemExportExcel = new DXMenuItem();     // 그리드 엑셀 내보내기
            var mnuItemCopyCell = new DXMenuItem();        // 그리드 셀값 복사
            var mnuItemCopyRow = new DXMenuItem();         // 그리드 행값 복사
            var mnuItemCopyGrid = new DXMenuItem();        // 그리드 전체 복사
            var mnuItemAddRows = new DXMenuItem();         // 신규 행 추가

            mnuItemInitialFormat.Caption = "Initialize Grid Format".Localization();
            mnuItemInitialFormat.ImageOptions.Image = Properties.Resources.Refresh16x16;
            mnuItemInitialFormat.BeginGroup = true;
            mnuItemInitialFormat.Tag = "InitFormat";
            menu.Items.Add(mnuItemInitialFormat);

            mnuItemExportFormat.Caption = "Save Grid Format".Localization();
            mnuItemExportFormat.ImageOptions.Image = Properties.Resources.Save16x16;
            mnuItemExportFormat.Tag = "SaveFormat";
            menu.Items.Add(mnuItemExportFormat);

            mnuItemImportFormat.Caption = "Load Grid Format".Localization();
            mnuItemImportFormat.ImageOptions.Image = Properties.Resources.Open16x16;
            mnuItemImportFormat.Tag = "LoadFormat";
            menu.Items.Add(mnuItemImportFormat);

            mnuItemExportExcel.Caption = "Export To Grid Excel".Localization();
            mnuItemExportExcel.ImageOptions.Image = Properties.Resources.Excel16x16;
            mnuItemExportExcel.BeginGroup = true;
            mnuItemExportExcel.Tag = "ExportExcel";
            menu.Items.Add(mnuItemExportExcel);

            mnuItemCopyCell.Caption = "Copy Cell".Localization();
            mnuItemCopyCell.ImageOptions.Image = Properties.Resources.CopyCell16x16;
            mnuItemCopyCell.BeginGroup = true;
            mnuItemCopyCell.Tag = "CopyCell";
            menu.Items.Add(mnuItemCopyCell);

            mnuItemCopyRow.Caption = "Copy Row".Localization();
            mnuItemCopyRow.ImageOptions.Image = Properties.Resources.CopyRow16x16;
            mnuItemCopyRow.Tag = "CopyRow";
            menu.Items.Add(mnuItemCopyRow);

            mnuItemCopyGrid.Caption = "Copy Grid".Localization();
            mnuItemCopyGrid.ImageOptions.Image = Properties.Resources.CopyGrid16x16;
            mnuItemCopyGrid.Tag = "CopyGrid";
            menu.Items.Add(mnuItemCopyGrid);

            if (view.GridControl.Tag.CString() == "AR")
            {
                mnuItemAddRows.Caption = "AddRow".Localization();
                mnuItemAddRows.ImageOptions.Image = Properties.Resources.RowInsert16x16;
                mnuItemAddRows.BeginGroup = true;
                mnuItemAddRows.Tag = "AddRow";
                menu.Items.Add(mnuItemAddRows);
            }

            menu.ItemClick += Menu_ItemClick;

            return menu;
        }

        /// <summary>
        /// GridView의 사용자 정의 단축 메뉴의 항목을 클릭할 때 각 메뉴의 처리
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Menu_ItemClick(object sender, DXMenuItemEventArgs e)
        {
            string downloadFileName;
            string text;
            string fileName;

            try
            {
                var view = (sender as GridViewMenu).View;

                view.OptionsPrint.ShowPrintExportProgress = true;
                view.OptionsPrint.AutoWidth = false;

                switch (e.Item.Tag.CString())
                {
                    case "InitFormat":  // 그리드 형식 초기화
                        LoadGridFormat(view, GridFormat.Default);

                        // 초기화값은 데이터가 없는 상태에서 형식만 저장되었으므로 다시 한번 최적의 넓이로 조정한다.
                        // 일부화면은 데이터 출력 후 저장되는 경우도 있음. 마스터 화면의 경우 데이터 조회 후 그리드 형식 저장
                        view.BestFitColumns();

                        // 그리드 초기화시 IndicatorWidth 값이 초기화 된다. 그래서 재설정한다.
                        //view.IndicatorWidth = 48;
                        //Graphics gr = Graphics.FromHwnd(view.GridControl.Handle);
                        //SizeF size = gr.MeasureString(view.RowCount.ToString(), view.PaintAppearance.Row.GetFont());
                        //view.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + DevExpress.XtraGrid.Views.Grid.Drawing.GridPainter.Indicator.ImageSize.Width + 10;
                        view.SetRowIndicatorWidth();

                        // 초기화시 기존에 저장된 현재 형식을 삭제 한다. (초기화 후 저장하지 않으면 다음에 기존 형식대로 또 읽어오는 문제 해결)
                        fileName = $"{Application.StartupPath}\\GridCurrent\\{view.GridControl.FindForm().Name}_{view.Name}.xml";
                        if (File.Exists(fileName))
                            File.Delete(fileName);

                        break;

                    case "SaveFormat":  // 그리드 형식 저장
                        SaveGridFormat(view, GridFormat.Current);
                        break;

                    case "LoadFormat":  // 그리드 형식 불러오기
                        fileName = $"{Application.StartupPath}\\GridCurrent\\{view.GridControl.FindForm().Name}_{view.Name}.xml";
                        if (File.Exists(fileName))
                            LoadGridFormat(view, GridFormat.Current);
                        else
                            MsgBox.WarningMessage("MSG_NOT_EXIST_GRIDFORMAT".Localization());

                        break;

                    case "ExportExcel": // 엑셀 내보내기
                        if (view.RowCount <= 0)
                        {
                            MsgBox.WarningMessage("MSG_DATA_NOT_EXIST".Localization(), "Excel Download");
                            return;
                        }

                        downloadFileName = GetFileName("xlsx", "xlsx files(*.xlsx)|*.xlsx");

                        if (downloadFileName != string.Empty)
                        {
                            var xlsxOptions = new XlsxExportOptionsEx() { ExportType = DevExpress.Export.ExportType.WYSIWYG };
                            view.OptionsPrint.UsePrintStyles = true;
                            view.OptionsPrint.AutoWidth = false;
                            view.OptionsPrint.EnableAppearanceEvenRow = false;
                            view.OptionsPrint.EnableAppearanceOddRow = false;

                            view.AppearancePrint.HeaderPanel.Font = new Font(view.Appearance.HeaderPanel.Font, FontStyle.Bold);
                            view.AppearancePrint.HeaderPanel.Options.UseTextOptions = true;
                            view.AppearancePrint.HeaderPanel.TextOptions.VAlignment = VertAlignment.Center;

                            view.ExportToXlsx(downloadFileName, xlsxOptions);
                        }

                        if (File.Exists(downloadFileName))
                        {
                            try
                            {
                                System.Diagnostics.Process.Start(downloadFileName);
                            }
                            catch (Exception)
                            {
                                string msg = $"{"MSG_ERROR_FILE_OPEN".Localization()}\r\n\r\nPath: {downloadFileName}";
                                MsgBox.ErrorMessage(msg);
                            }
                        }

                        break;

                    case "CopyCell":    // 셀 복사
                        if (string.IsNullOrEmpty(view.GetRowCellValue(view.FocusedRowHandle, view.FocusedColumn).CString()))
                            return;

                        Clipboard.SetText(view.GetRowCellValue(view.FocusedRowHandle, view.FocusedColumn).CString());

                        break;

                    case "CopyRow":     // 행 복사
                        text = string.Empty;
                        var row = view.GetDataRow(view.FocusedRowHandle);

                        if (row == null)
                            return;

                        // 헤더 복사
                        for (int i = 0; i < view.Columns.Count; i++)
                            text += view.Columns[i].Caption + "\t";

                        text += "\r\n";

                        // 행 내용 복사
                        for (int i = 0; i < view.Columns.Count; i++)
                            text += row[i].CString() + "\t";

                        text += "\r\n";

                        Clipboard.SetText(text);
                        break;

                    case "CopyGrid":    // 그리드 전체 복사
                        if (view.RowCount > 0)
                        {
                            text = string.Empty;
                            var table = (view.GridControl.DataSource as DataTable);

                            // 헤더 복사
                            for (int j = 0; j < table.Columns.Count; j++)
                                text += view.Columns[j].Caption + "\t";

                            text += "\r\n";

                            // 데이터 복사
                            for (int i = 0; i < table.Rows.Count; i++)
                            {
                                for (int j = 0; j < table.Columns.Count; j++)
                                    text += table.Rows[i][j].CString() + "\t";

                                text += "\r\n";
                            }

                            Clipboard.SetText(text);
                        }
                        break;

                    case "AddRow":
                        var args = new XtraInputBoxArgs
                        {
                            Caption = "AddRow".Localization(),
                            Prompt = "MSG_ADD_ROWS".Localization(),
                            DefaultButtonIndex = 0,
                            DefaultResponse = 1
                        };

                        var editor = new SpinEdit();
                        editor.Properties.BorderStyle = BorderStyles.HotFlat;
                        editor.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
                        editor.Properties.Mask.EditMask = "N0";
                        editor.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
                        args.Editor = editor;

                        var inputValue = XtraInputBox.Show(args).CString();

                        if (inputValue != "")
                        {
                            for (int i = 0; i < Convert.ToInt32(inputValue); i++)
                            {
                                view.AddNewRow();
                                view.BestFitColumns();
                                view.UpdateCurrentRow();
                            }
                        }
                        break;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region 그리드 포맷 처리를 위한 사용자 정의 함수
        /// <summary>
        /// 그리드를 엑셀 유형으로 저장할 때 사용할 파일명을 생성한다.
        /// </summary>
        /// <param name="fileExt"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        private static string GetFileName(string fileExt, string filter)
        {
            var dialog = new SaveFileDialog
            {
                Filter = filter,
                RestoreDirectory = true,
                FileName = string.Format("{0}.{1}", DateTime.Now.ToString("yyyyMMddHHmmss"), fileExt)
            };

            return (DialogResult.OK == dialog.ShowDialog()) ? dialog.FileName : string.Empty;
        }

        /// <summary>
        /// GridView의 형식을 XML 형태로 저장한다.(To XML File)
        /// </summary>
        /// <param name="view">GridView 컨트롤</param>
        /// <param name="formatType">Default(기본 형식), Current(현재 형식)</param>
        /// <returns></returns>
        public static bool SaveGridFormat(GridView view, GridFormat formatType = GridFormat.Default)
        {
            var path = Application.StartupPath;

            if (formatType == GridFormat.Default)
                path += "\\GridDefault";
            else
                path += "\\GridCurrent";

            // path += (formatType == GridFormat.Default) ? "\\GridDefault" : "\\GridCurrent";

            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                var fileName = $"{path}\\{view.GridControl.FindForm().Name}_{view.Name}.xml";
                if (File.Exists(fileName))
                    File.Delete(fileName);

                view.SaveLayoutToXml(fileName, OptionsLayoutBase.FullLayout);

                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// GridView의 형식을 로딩합니다.(from XML File)
        /// </summary>
        /// <param name="view">GridView 컨트롤</param>
        /// <param name="formatType">Default(기본 형식), Current(현재 형식)</param>
        /// <returns></returns>
        public static bool LoadGridFormat(GridView view, GridFormat formatType = GridFormat.Default)
        {
            var path = Application.StartupPath;

            if (formatType == GridFormat.Default)
                path += "\\GridDefault";
            else
                path += "\\GridCurrent";

            // path += (formatType == GridFormat.Default) ? "\\GridDefault" : "\\GridCurrent";

            try
            {
                if (!Directory.Exists(path))
                    return false;

                var fielName = $"{path}\\{view.GridControl.FindForm().Name}_{view.Name}.xml";
                view.RestoreLayoutFromXml(fielName, OptionsLayoutBase.FullLayout);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion
    }
}
