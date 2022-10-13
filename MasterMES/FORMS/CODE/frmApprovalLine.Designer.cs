
namespace MasterMES.FORMS.CODE
{
    partial class frmApprovalLine
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
            this.txtApprvSeq = new DevExpress.XtraEditors.TextEdit();
            this.grcApprvLineSub = new DevExpress.XtraGrid.GridControl();
            this.grvApprvLineSub = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.grcApprvLine = new DevExpress.XtraGrid.GridControl();
            this.grvApprvLine = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.txtApprvTitle = new DevExpress.XtraEditors.TextEdit();
            this.tlsPerson = new DevExpress.XtraTreeList.TreeList();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlGroup3 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtApprvSeq.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grcApprvLineSub)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvApprvLineSub)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grcApprvLine)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvApprvLine)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtApprvTitle.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tlsPerson)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.AllowCustomization = false;
            this.layoutControl1.Controls.Add(this.txtApprvSeq);
            this.layoutControl1.Controls.Add(this.grcApprvLineSub);
            this.layoutControl1.Controls.Add(this.grcApprvLine);
            this.layoutControl1.Controls.Add(this.txtApprvTitle);
            this.layoutControl1.Controls.Add(this.tlsPerson);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(874, 493, 846, 400);
            this.layoutControl1.OptionsFocus.AllowFocusGroups = false;
            this.layoutControl1.OptionsFocus.EnableAutoTabOrder = false;
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(1489, 896);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // txtApprvSeq
            // 
            this.txtApprvSeq.Enabled = false;
            this.txtApprvSeq.Location = new System.Drawing.Point(525, 37);
            this.txtApprvSeq.Name = "txtApprvSeq";
            this.txtApprvSeq.Size = new System.Drawing.Size(50, 24);
            this.txtApprvSeq.StyleController = this.layoutControl1;
            this.txtApprvSeq.TabIndex = 8;
            // 
            // grcApprvLineSub
            // 
            this.grcApprvLineSub.Location = new System.Drawing.Point(464, 472);
            this.grcApprvLineSub.MainView = this.grvApprvLineSub;
            this.grcApprvLineSub.Name = "grcApprvLineSub";
            this.grcApprvLineSub.Size = new System.Drawing.Size(468, 396);
            this.grcApprvLineSub.TabIndex = 7;
            this.grcApprvLineSub.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grvApprvLineSub});
            // 
            // grvApprvLineSub
            // 
            this.grvApprvLineSub.GridControl = this.grcApprvLineSub;
            this.grvApprvLineSub.Name = "grvApprvLineSub";
            this.grvApprvLineSub.DoubleClick += new System.EventHandler(this.grvApprvLineSub_DoubleClick);
            // 
            // grcApprvLine
            // 
            this.grcApprvLine.Location = new System.Drawing.Point(452, 65);
            this.grcApprvLine.MainView = this.grvApprvLine;
            this.grcApprvLine.Name = "grcApprvLine";
            this.grcApprvLine.Size = new System.Drawing.Size(492, 370);
            this.grcApprvLine.TabIndex = 6;
            this.grcApprvLine.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grvApprvLine});
            // 
            // grvApprvLine
            // 
            this.grvApprvLine.GridControl = this.grcApprvLine;
            this.grvApprvLine.Name = "grvApprvLine";
            this.grvApprvLine.RowClick += new DevExpress.XtraGrid.Views.Grid.RowClickEventHandler(this.grvApprvLine_RowClick);
            this.grvApprvLine.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.grvApprvLine_FocusedRowChanged);
            // 
            // txtApprvTitle
            // 
            this.txtApprvTitle.Location = new System.Drawing.Point(652, 37);
            this.txtApprvTitle.Name = "txtApprvTitle";
            this.txtApprvTitle.Size = new System.Drawing.Size(292, 24);
            this.txtApprvTitle.StyleController = this.layoutControl1;
            this.txtApprvTitle.TabIndex = 5;
            // 
            // tlsPerson
            // 
            this.tlsPerson.Location = new System.Drawing.Point(16, 37);
            this.tlsPerson.Name = "tlsPerson";
            this.tlsPerson.Size = new System.Drawing.Size(408, 843);
            this.tlsPerson.TabIndex = 4;
            this.tlsPerson.TreeLevelWidth = 30;
            this.tlsPerson.DoubleClick += new System.EventHandler(this.tlsPerson_DoubleClick);
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlGroup1,
            this.layoutControlGroup3,
            this.emptySpaceItem1});
            this.Root.Name = "Root";
            this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
            this.Root.Size = new System.Drawing.Size(1489, 896);
            this.Root.TextVisible = false;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.layoutControlGroup2,
            this.layoutControlItem5});
            this.layoutControlGroup1.Location = new System.Drawing.Point(436, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(520, 892);
            this.layoutControlGroup1.Text = "ApprovalLine Info";
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.txtApprvTitle;
            this.layoutControlItem2.Location = new System.Drawing.Point(127, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(369, 28);
            this.layoutControlItem2.Text = "ApprvTitle";
            this.layoutControlItem2.TextSize = new System.Drawing.Size(61, 18);
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.grcApprvLine;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 28);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(496, 374);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlGroup2
            // 
            this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem4});
            this.layoutControlGroup2.Location = new System.Drawing.Point(0, 402);
            this.layoutControlGroup2.Name = "layoutControlGroup2";
            this.layoutControlGroup2.Size = new System.Drawing.Size(496, 445);
            this.layoutControlGroup2.Text = "ApprovalLineSub Info";
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.grcApprvLineSub;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(472, 400);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.txtApprvSeq;
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(127, 28);
            this.layoutControlItem5.Text = "ApprvSeq";
            this.layoutControlItem5.TextSize = new System.Drawing.Size(61, 18);
            // 
            // layoutControlGroup3
            // 
            this.layoutControlGroup3.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1});
            this.layoutControlGroup3.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup3.Name = "layoutControlGroup3";
            this.layoutControlGroup3.Size = new System.Drawing.Size(436, 892);
            this.layoutControlGroup3.Text = "Person Info";
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.tlsPerson;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(412, 847);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(956, 0);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(529, 892);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // frmApprovalLine
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1489, 896);
            this.Controls.Add(this.layoutControl1);
            this.KeyPreview = true;
            this.Name = "frmApprovalLine";
            this.Text = "frmApprovalLine";
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtApprvSeq.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grcApprvLineSub)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvApprvLineSub)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grcApprvLine)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvApprvLine)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtApprvTitle.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tlsPerson)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraGrid.GridControl grcApprvLineSub;
        private DevExpress.XtraGrid.Views.Grid.GridView grvApprvLineSub;
        private DevExpress.XtraGrid.GridControl grcApprvLine;
        private DevExpress.XtraGrid.Views.Grid.GridView grvApprvLine;
        private DevExpress.XtraEditors.TextEdit txtApprvTitle;
        private DevExpress.XtraTreeList.TreeList tlsPerson;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup3;
        private DevExpress.XtraEditors.TextEdit txtApprvSeq;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
    }
}