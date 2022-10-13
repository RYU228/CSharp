
namespace MasterMES.FORMS.CODE
{
    partial class frmPartsCode
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
            this.mmeRemark = new DevExpress.XtraEditors.MemoEdit();
            this.chkUseClss = new DevExpress.XtraEditors.CheckEdit();
            this.txtPartsName = new DevExpress.XtraEditors.TextEdit();
            this.txtPartsCode = new DevExpress.XtraEditors.TextEdit();
            this.grcParts = new DevExpress.XtraGrid.GridControl();
            this.grvParts = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcgParts = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mmeRemark.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkUseClss.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPartsName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPartsCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grcParts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvParts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcgParts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.mmeRemark);
            this.layoutControl1.Controls.Add(this.chkUseClss);
            this.layoutControl1.Controls.Add(this.txtPartsName);
            this.layoutControl1.Controls.Add(this.txtPartsCode);
            this.layoutControl1.Controls.Add(this.grcParts);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(1306, 771);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // mmeRemark
            // 
            this.mmeRemark.Location = new System.Drawing.Point(94, 117);
            this.mmeRemark.Name = "mmeRemark";
            this.mmeRemark.Size = new System.Drawing.Size(284, 56);
            this.mmeRemark.StyleController = this.layoutControl1;
            this.mmeRemark.TabIndex = 8;
            // 
            // chkUseClss
            // 
            this.chkUseClss.Location = new System.Drawing.Point(94, 93);
            this.chkUseClss.Name = "chkUseClss";
            this.chkUseClss.Properties.Caption = "";
            this.chkUseClss.Size = new System.Drawing.Size(284, 20);
            this.chkUseClss.StyleController = this.layoutControl1;
            this.chkUseClss.TabIndex = 7;
            // 
            // txtPartsName
            // 
            this.txtPartsName.Location = new System.Drawing.Point(94, 69);
            this.txtPartsName.Name = "txtPartsName";
            this.txtPartsName.Size = new System.Drawing.Size(284, 20);
            this.txtPartsName.StyleController = this.layoutControl1;
            this.txtPartsName.TabIndex = 6;
            // 
            // txtPartsCode
            // 
            this.txtPartsCode.Location = new System.Drawing.Point(94, 45);
            this.txtPartsCode.Name = "txtPartsCode";
            this.txtPartsCode.Size = new System.Drawing.Size(284, 20);
            this.txtPartsCode.StyleController = this.layoutControl1;
            this.txtPartsCode.TabIndex = 5;
            // 
            // grcParts
            // 
            this.grcParts.Location = new System.Drawing.Point(394, 12);
            this.grcParts.MainView = this.grvParts;
            this.grcParts.Name = "grcParts";
            this.grcParts.Size = new System.Drawing.Size(900, 747);
            this.grcParts.TabIndex = 4;
            this.grcParts.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grvParts});
            // 
            // grvParts
            // 
            this.grvParts.GridControl = this.grcParts;
            this.grvParts.Name = "grvParts";
            this.grvParts.RowClick += new DevExpress.XtraGrid.Views.Grid.RowClickEventHandler(this.grvParts_RowClick);
            this.grvParts.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.grvParts_FocusedRowChanged);
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.lcgParts});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(1306, 771);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.grcParts;
            this.layoutControlItem1.Location = new System.Drawing.Point(382, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(904, 751);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // lcgParts
            // 
            this.lcgParts.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.layoutControlItem4,
            this.layoutControlItem5,
            this.emptySpaceItem2});
            this.lcgParts.Location = new System.Drawing.Point(0, 0);
            this.lcgParts.Name = "lcgParts";
            this.lcgParts.Size = new System.Drawing.Size(382, 751);
            this.lcgParts.Text = "Parts Info";
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.txtPartsCode;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(358, 24);
            this.layoutControlItem2.Tag = "Y";
            this.layoutControlItem2.Text = "PartsCode";
            this.layoutControlItem2.TextSize = new System.Drawing.Size(58, 14);
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.txtPartsName;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 24);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(358, 24);
            this.layoutControlItem3.Tag = "Y";
            this.layoutControlItem3.Text = "PartsName";
            this.layoutControlItem3.TextSize = new System.Drawing.Size(58, 14);
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.chkUseClss;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 48);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(358, 24);
            this.layoutControlItem4.Text = "UseClss";
            this.layoutControlItem4.TextSize = new System.Drawing.Size(58, 14);
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.mmeRemark;
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 72);
            this.layoutControlItem5.MaxSize = new System.Drawing.Size(0, 60);
            this.layoutControlItem5.MinSize = new System.Drawing.Size(84, 60);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(358, 60);
            this.layoutControlItem5.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem5.Text = "Remark";
            this.layoutControlItem5.TextSize = new System.Drawing.Size(58, 14);
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(0, 132);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(358, 574);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // frmPartsCode
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1306, 771);
            this.Controls.Add(this.layoutControl1);
            this.Name = "frmPartsCode";
            this.Text = "frmPartsCode";
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mmeRemark.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkUseClss.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPartsName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPartsCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grcParts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvParts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcgParts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraEditors.MemoEdit mmeRemark;
        private DevExpress.XtraEditors.CheckEdit chkUseClss;
        private DevExpress.XtraEditors.TextEdit txtPartsName;
        private DevExpress.XtraEditors.TextEdit txtPartsCode;
        private DevExpress.XtraGrid.GridControl grcParts;
        private DevExpress.XtraGrid.Views.Grid.GridView grvParts;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlGroup lcgParts;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
    }
}