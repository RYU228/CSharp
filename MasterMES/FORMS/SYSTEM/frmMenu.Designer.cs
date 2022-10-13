
namespace MasterMES.FORMS.SYSTEM
{
    partial class frmMenu
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.chkUseClss = new DevExpress.XtraEditors.CheckEdit();
            this.txtMenuPath = new DevExpress.XtraEditors.TextEdit();
            this.gleMenuLevel = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit2View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.txtMenuTitle = new DevExpress.XtraEditors.TextEdit();
            this.gleParentID = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.txtMenuID = new DevExpress.XtraEditors.TextEdit();
            this.tlsMenu = new DevExpress.XtraTreeList.TreeList();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcgMenu = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.splitterItem1 = new DevExpress.XtraLayout.SplitterItem();
            this.txtMenuGroup = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkUseClss.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMenuPath.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gleMenuLevel.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit2View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMenuTitle.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gleParentID.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMenuID.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tlsMenu)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcgMenu)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitterItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMenuGroup.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.chkUseClss);
            this.layoutControl1.Controls.Add(this.txtMenuPath);
            this.layoutControl1.Controls.Add(this.gleMenuLevel);
            this.layoutControl1.Controls.Add(this.txtMenuTitle);
            this.layoutControl1.Controls.Add(this.gleParentID);
            this.layoutControl1.Controls.Add(this.txtMenuID);
            this.layoutControl1.Controls.Add(this.tlsMenu);
            this.layoutControl1.Controls.Add(this.txtMenuGroup);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(1793, 507, 650, 400);
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(1298, 768);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // chkUseClss
            // 
            this.chkUseClss.Location = new System.Drawing.Point(104, 213);
            this.chkUseClss.Name = "chkUseClss";
            this.chkUseClss.Properties.Caption = "";
            this.chkUseClss.Size = new System.Drawing.Size(113, 20);
            this.chkUseClss.StyleController = this.layoutControl1;
            this.chkUseClss.TabIndex = 11;
            // 
            // txtMenuPath
            // 
            this.txtMenuPath.Location = new System.Drawing.Point(104, 185);
            this.txtMenuPath.Name = "txtMenuPath";
            this.txtMenuPath.Size = new System.Drawing.Size(113, 24);
            this.txtMenuPath.StyleController = this.layoutControl1;
            this.txtMenuPath.TabIndex = 9;
            // 
            // gleMenuLevel
            // 
            this.gleMenuLevel.Location = new System.Drawing.Point(104, 129);
            this.gleMenuLevel.Name = "gleMenuLevel";
            this.gleMenuLevel.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.gleMenuLevel.Properties.PopupView = this.gridLookUpEdit2View;
            this.gleMenuLevel.Size = new System.Drawing.Size(113, 24);
            this.gleMenuLevel.StyleController = this.layoutControl1;
            this.gleMenuLevel.TabIndex = 8;
            // 
            // gridLookUpEdit2View
            // 
            this.gridLookUpEdit2View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit2View.Name = "gridLookUpEdit2View";
            this.gridLookUpEdit2View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit2View.OptionsView.ShowGroupPanel = false;
            // 
            // txtMenuTitle
            // 
            this.txtMenuTitle.Location = new System.Drawing.Point(104, 101);
            this.txtMenuTitle.Name = "txtMenuTitle";
            this.txtMenuTitle.Size = new System.Drawing.Size(113, 24);
            this.txtMenuTitle.StyleController = this.layoutControl1;
            this.txtMenuTitle.TabIndex = 7;
            // 
            // gleParentID
            // 
            this.gleParentID.Location = new System.Drawing.Point(104, 73);
            this.gleParentID.Name = "gleParentID";
            this.gleParentID.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.gleParentID.Properties.PopupView = this.gridLookUpEdit1View;
            this.gleParentID.Size = new System.Drawing.Size(113, 24);
            this.gleParentID.StyleController = this.layoutControl1;
            this.gleParentID.TabIndex = 6;
            this.gleParentID.QueryPopUp += new System.ComponentModel.CancelEventHandler(this.gleParentID_QueryPopUp);
            // 
            // gridLookUpEdit1View
            // 
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // txtMenuID
            // 
            this.txtMenuID.Location = new System.Drawing.Point(104, 45);
            this.txtMenuID.Name = "txtMenuID";
            this.txtMenuID.Size = new System.Drawing.Size(113, 24);
            this.txtMenuID.StyleController = this.layoutControl1;
            this.txtMenuID.TabIndex = 5;
            // 
            // tlsMenu
            // 
            this.tlsMenu.Location = new System.Drawing.Point(243, 12);
            this.tlsMenu.Name = "tlsMenu";
            this.tlsMenu.Size = new System.Drawing.Size(1043, 744);
            this.tlsMenu.TabIndex = 4;
            this.tlsMenu.RowClick += new DevExpress.XtraTreeList.RowClickEventHandler(this.tlsMenu_RowClick);
            this.tlsMenu.NodeCellStyle += new DevExpress.XtraTreeList.GetCustomNodeCellStyleEventHandler(this.tlsMenu_NodeCellStyle);
            this.tlsMenu.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.tlsMenu_FocusedNodeChanged);
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.lcgMenu,
            this.splitterItem1});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(1298, 768);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.tlsMenu;
            this.layoutControlItem1.Location = new System.Drawing.Point(231, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(1047, 748);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // lcgMenu
            // 
            this.lcgMenu.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem6,
            this.layoutControlItem5,
            this.layoutControlItem4,
            this.layoutControlItem3,
            this.layoutControlItem2,
            this.emptySpaceItem1,
            this.layoutControlItem8,
            this.layoutControlItem7});
            this.lcgMenu.Location = new System.Drawing.Point(0, 0);
            this.lcgMenu.Name = "lcgMenu";
            this.lcgMenu.Size = new System.Drawing.Size(221, 748);
            this.lcgMenu.Text = "Menu Info";
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.txtMenuPath;
            this.layoutControlItem6.Location = new System.Drawing.Point(0, 140);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(197, 28);
            this.layoutControlItem6.Tag = "";
            this.layoutControlItem6.Text = "MenuPath";
            this.layoutControlItem6.TextSize = new System.Drawing.Size(68, 18);
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.gleMenuLevel;
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 84);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(197, 28);
            this.layoutControlItem5.Tag = "Y";
            this.layoutControlItem5.Text = "MenuLevel";
            this.layoutControlItem5.TextSize = new System.Drawing.Size(68, 18);
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.txtMenuTitle;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 56);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(197, 28);
            this.layoutControlItem4.Tag = "Y";
            this.layoutControlItem4.Text = "MenuTitle";
            this.layoutControlItem4.TextSize = new System.Drawing.Size(68, 18);
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.gleParentID;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 28);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(197, 28);
            this.layoutControlItem3.Tag = "Y";
            this.layoutControlItem3.Text = "ParentID";
            this.layoutControlItem3.TextSize = new System.Drawing.Size(68, 18);
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.txtMenuID;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(197, 28);
            this.layoutControlItem2.Tag = "Y";
            this.layoutControlItem2.Text = "MenuID";
            this.layoutControlItem2.TextSize = new System.Drawing.Size(68, 18);
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 192);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(197, 511);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.Control = this.chkUseClss;
            this.layoutControlItem8.Location = new System.Drawing.Point(0, 168);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.Size = new System.Drawing.Size(197, 24);
            this.layoutControlItem8.Text = "UseClss";
            this.layoutControlItem8.TextSize = new System.Drawing.Size(68, 18);
            // 
            // splitterItem1
            // 
            this.splitterItem1.AllowHotTrack = true;
            this.splitterItem1.Location = new System.Drawing.Point(221, 0);
            this.splitterItem1.Name = "splitterItem1";
            this.splitterItem1.Size = new System.Drawing.Size(10, 748);
            // 
            // txtMenuGroup
            // 
            this.txtMenuGroup.Location = new System.Drawing.Point(104, 157);
            this.txtMenuGroup.Name = "txtMenuGroup";
            this.txtMenuGroup.Size = new System.Drawing.Size(113, 24);
            this.txtMenuGroup.StyleController = this.layoutControl1;
            this.txtMenuGroup.TabIndex = 7;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.txtMenuGroup;
            this.layoutControlItem7.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
            this.layoutControlItem7.CustomizationFormText = "MenuTitle";
            this.layoutControlItem7.Location = new System.Drawing.Point(0, 112);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(197, 28);
            this.layoutControlItem7.Text = "MenuGroup";
            this.layoutControlItem7.TextSize = new System.Drawing.Size(68, 18);
            // 
            // frmMenu
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1298, 768);
            this.Controls.Add(this.layoutControl1);
            this.Name = "frmMenu";
            this.Text = "frmMenu";
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chkUseClss.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMenuPath.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gleMenuLevel.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit2View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMenuTitle.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gleParentID.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMenuID.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tlsMenu)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcgMenu)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitterItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMenuGroup.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.TextEdit txtMenuPath;
        private DevExpress.XtraEditors.GridLookUpEdit gleMenuLevel;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit2View;
        private DevExpress.XtraEditors.TextEdit txtMenuTitle;
        private DevExpress.XtraEditors.GridLookUpEdit gleParentID;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraEditors.TextEdit txtMenuID;
        private DevExpress.XtraTreeList.TreeList tlsMenu;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlGroup lcgMenu;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.SplitterItem splitterItem1;
        private DevExpress.XtraEditors.CheckEdit chkUseClss;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private DevExpress.XtraEditors.TextEdit txtMenuGroup;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
    }
}