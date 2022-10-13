
namespace MasterMES.FORMS.CODE
{
    partial class frmUserMenu
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
            this.grcUserMenu = new DevExpress.XtraGrid.GridControl();
            this.grvUserMenu = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.grcPerson = new DevExpress.XtraGrid.GridControl();
            this.grvPerson = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.splitterItem1 = new DevExpress.XtraLayout.SplitterItem();
            this.lcgPerson = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcgUserMenu = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grcUserMenu)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvUserMenu)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grcPerson)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvPerson)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitterItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcgPerson)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcgUserMenu)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.grcUserMenu);
            this.layoutControl1.Controls.Add(this.grcPerson);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(1747, 507, 650, 400);
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(1298, 768);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // grcUserMenu
            // 
            this.grcUserMenu.Location = new System.Drawing.Point(407, 45);
            this.grcUserMenu.MainView = this.grvUserMenu;
            this.grcUserMenu.Name = "grcUserMenu";
            this.grcUserMenu.Size = new System.Drawing.Size(867, 699);
            this.grcUserMenu.TabIndex = 5;
            this.grcUserMenu.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grvUserMenu});
            // 
            // grvUserMenu
            // 
            this.grvUserMenu.GridControl = this.grcUserMenu;
            this.grvUserMenu.Name = "grvUserMenu";
            this.grvUserMenu.CellMerge += new DevExpress.XtraGrid.Views.Grid.CellMergeEventHandler(this.grvUserMenu_CellMerge);
            this.grvUserMenu.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.grvUserMenu_ShowingEditor);
            // 
            // grcPerson
            // 
            this.grcPerson.Location = new System.Drawing.Point(24, 45);
            this.grcPerson.MainView = this.grvPerson;
            this.grcPerson.Name = "grcPerson";
            this.grcPerson.Size = new System.Drawing.Size(345, 699);
            this.grcPerson.TabIndex = 4;
            this.grcPerson.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grvPerson});
            // 
            // grvPerson
            // 
            this.grvPerson.GridControl = this.grcPerson;
            this.grvPerson.Name = "grvPerson";
            this.grvPerson.RowClick += new DevExpress.XtraGrid.Views.Grid.RowClickEventHandler(this.grvPerson_RowClick);
            this.grvPerson.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.grvPerson_FocusedRowChanged);
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.splitterItem1,
            this.lcgPerson,
            this.lcgUserMenu});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(1298, 768);
            this.Root.TextVisible = false;
            // 
            // splitterItem1
            // 
            this.splitterItem1.AllowHotTrack = true;
            this.splitterItem1.Location = new System.Drawing.Point(373, 0);
            this.splitterItem1.Name = "splitterItem1";
            this.splitterItem1.Size = new System.Drawing.Size(10, 748);
            // 
            // lcgPerson
            // 
            this.lcgPerson.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1});
            this.lcgPerson.Location = new System.Drawing.Point(0, 0);
            this.lcgPerson.Name = "lcgPerson";
            this.lcgPerson.Size = new System.Drawing.Size(373, 748);
            this.lcgPerson.Text = "Person Info";
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.grcPerson;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(349, 703);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // lcgUserMenu
            // 
            this.lcgUserMenu.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem2});
            this.lcgUserMenu.Location = new System.Drawing.Point(383, 0);
            this.lcgUserMenu.Name = "lcgUserMenu";
            this.lcgUserMenu.Size = new System.Drawing.Size(895, 748);
            this.lcgUserMenu.Text = "UserMenu Info";
            this.lcgUserMenu.CustomButtonClick += new DevExpress.XtraBars.Docking2010.BaseButtonEventHandler(this.lcgUserMenu_CustomButtonClick);
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.grcUserMenu;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(871, 703);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // frmUserMenu
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1298, 768);
            this.Controls.Add(this.layoutControl1);
            this.Name = "frmUserMenu";
            this.Text = "frmUserMenu";
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grcUserMenu)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvUserMenu)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grcPerson)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvPerson)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitterItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcgPerson)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcgUserMenu)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraGrid.GridControl grcUserMenu;
        private DevExpress.XtraGrid.Views.Grid.GridView grvUserMenu;
        private DevExpress.XtraGrid.GridControl grcPerson;
        private DevExpress.XtraGrid.Views.Grid.GridView grvPerson;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.SplitterItem splitterItem1;
        private DevExpress.XtraLayout.LayoutControlGroup lcgPerson;
        private DevExpress.XtraLayout.LayoutControlGroup lcgUserMenu;
    }
}