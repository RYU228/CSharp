
namespace DS_MES.FORMS.CODE
{
    partial class frmOrderCode
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
            this.grcOrd = new DevExpress.XtraGrid.GridControl();
            this.grvOrd = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.dteEndOrdDate = new DevExpress.XtraEditors.DateEdit();
            this.dteStrOrdDate = new DevExpress.XtraEditors.DateEdit();
            this.dteOrdDate = new DevExpress.XtraEditors.DateEdit();
            this.btnDeleteRow = new DevExpress.XtraEditors.SimpleButton();
            this.btnAppendRow = new DevExpress.XtraEditors.SimpleButton();
            this.grcInput = new DevExpress.XtraGrid.GridControl();
            this.grvInput = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcgOrd = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.splitterItem1 = new DevExpress.XtraLayout.SplitterItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grcOrd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvOrd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteEndOrdDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteEndOrdDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteStrOrdDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteStrOrdDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteOrdDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteOrdDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grcInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcgOrd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitterItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.grcOrd);
            this.layoutControl1.Controls.Add(this.dteEndOrdDate);
            this.layoutControl1.Controls.Add(this.dteStrOrdDate);
            this.layoutControl1.Controls.Add(this.dteOrdDate);
            this.layoutControl1.Controls.Add(this.btnDeleteRow);
            this.layoutControl1.Controls.Add(this.btnAppendRow);
            this.layoutControl1.Controls.Add(this.grcInput);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(1590, 507, 650, 400);
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(1298, 768);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // grcOrd
            // 
            this.grcOrd.Location = new System.Drawing.Point(24, 101);
            this.grcOrd.MainView = this.grvOrd;
            this.grcOrd.Name = "grcOrd";
            this.grcOrd.Size = new System.Drawing.Size(167, 643);
            this.grcOrd.TabIndex = 11;
            this.grcOrd.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grvOrd});
            // 
            // grvOrd
            // 
            this.grvOrd.GridControl = this.grcOrd;
            this.grvOrd.Name = "grvOrd";
            this.grvOrd.RowClick += new DevExpress.XtraGrid.Views.Grid.RowClickEventHandler(this.grvOrd_RowClick);
            this.grvOrd.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.grvOrd_FocusedRowChanged);
            // 
            // dteEndOrdDate
            // 
            this.dteEndOrdDate.EditValue = null;
            this.dteEndOrdDate.Location = new System.Drawing.Point(86, 73);
            this.dteEndOrdDate.Name = "dteEndOrdDate";
            this.dteEndOrdDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dteEndOrdDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dteEndOrdDate.Size = new System.Drawing.Size(105, 24);
            this.dteEndOrdDate.StyleController = this.layoutControl1;
            this.dteEndOrdDate.TabIndex = 10;
            this.dteEndOrdDate.Tag = "D";
            // 
            // dteStrOrdDate
            // 
            this.dteStrOrdDate.EditValue = null;
            this.dteStrOrdDate.Location = new System.Drawing.Point(86, 45);
            this.dteStrOrdDate.Name = "dteStrOrdDate";
            this.dteStrOrdDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dteStrOrdDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dteStrOrdDate.Size = new System.Drawing.Size(105, 24);
            this.dteStrOrdDate.StyleController = this.layoutControl1;
            this.dteStrOrdDate.TabIndex = 9;
            this.dteStrOrdDate.Tag = "D-6";
            // 
            // dteOrdDate
            // 
            this.dteOrdDate.EditValue = null;
            this.dteOrdDate.Location = new System.Drawing.Point(279, 12);
            this.dteOrdDate.MaximumSize = new System.Drawing.Size(140, 0);
            this.dteOrdDate.MinimumSize = new System.Drawing.Size(140, 0);
            this.dteOrdDate.Name = "dteOrdDate";
            this.dteOrdDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dteOrdDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dteOrdDate.Size = new System.Drawing.Size(140, 24);
            this.dteOrdDate.StyleController = this.layoutControl1;
            this.dteOrdDate.TabIndex = 7;
            this.dteOrdDate.Tag = "D";
            // 
            // btnDeleteRow
            // 
            this.btnDeleteRow.Location = new System.Drawing.Point(1226, 12);
            this.btnDeleteRow.MaximumSize = new System.Drawing.Size(60, 0);
            this.btnDeleteRow.MinimumSize = new System.Drawing.Size(60, 0);
            this.btnDeleteRow.Name = "btnDeleteRow";
            this.btnDeleteRow.Size = new System.Drawing.Size(60, 23);
            this.btnDeleteRow.StyleController = this.layoutControl1;
            this.btnDeleteRow.TabIndex = 6;
            this.btnDeleteRow.Text = "행삭제";
            this.btnDeleteRow.Click += new System.EventHandler(this.btnDeleteRow_Click);
            // 
            // btnAppendRow
            // 
            this.btnAppendRow.Location = new System.Drawing.Point(1162, 12);
            this.btnAppendRow.MaximumSize = new System.Drawing.Size(60, 0);
            this.btnAppendRow.MinimumSize = new System.Drawing.Size(60, 0);
            this.btnAppendRow.Name = "btnAppendRow";
            this.btnAppendRow.Size = new System.Drawing.Size(60, 23);
            this.btnAppendRow.StyleController = this.layoutControl1;
            this.btnAppendRow.TabIndex = 5;
            this.btnAppendRow.Text = "행추가";
            this.btnAppendRow.Click += new System.EventHandler(this.btnAppendRow_Click);
            // 
            // grcInput
            // 
            this.grcInput.Location = new System.Drawing.Point(217, 40);
            this.grcInput.MainView = this.grvInput;
            this.grcInput.Name = "grcInput";
            this.grcInput.Size = new System.Drawing.Size(1069, 716);
            this.grcInput.TabIndex = 4;
            this.grcInput.Tag = "AR";
            this.grcInput.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grvInput});
            // 
            // grvInput
            // 
            this.grvInput.GridControl = this.grcInput;
            this.grvInput.Name = "grvInput";
            this.grvInput.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.grvInput_RowCellStyle);
            this.grvInput.InitNewRow += new DevExpress.XtraGrid.Views.Grid.InitNewRowEventHandler(this.grvInput_InitNewRow);
            this.grvInput.KeyUp += new System.Windows.Forms.KeyEventHandler(this.grvInput_KeyUp);
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.layoutControlItem4,
            this.lcgOrd,
            this.splitterItem1,
            this.emptySpaceItem1});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(1298, 768);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.grcInput;
            this.layoutControlItem1.Location = new System.Drawing.Point(205, 28);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(1073, 720);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.btnAppendRow;
            this.layoutControlItem2.Location = new System.Drawing.Point(1150, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(64, 28);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.btnDeleteRow;
            this.layoutControlItem3.Location = new System.Drawing.Point(1214, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(64, 28);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.dteOrdDate;
            this.layoutControlItem4.Location = new System.Drawing.Point(205, 0);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(206, 28);
            this.layoutControlItem4.Tag = "Y";
            this.layoutControlItem4.Text = "OrdDate";
            this.layoutControlItem4.TextSize = new System.Drawing.Size(50, 18);
            // 
            // lcgOrd
            // 
            this.lcgOrd.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem6,
            this.layoutControlItem7,
            this.layoutControlItem5});
            this.lcgOrd.Location = new System.Drawing.Point(0, 0);
            this.lcgOrd.Name = "lcgOrd";
            this.lcgOrd.Size = new System.Drawing.Size(195, 748);
            this.lcgOrd.Text = "Order Info";
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.dteStrOrdDate;
            this.layoutControlItem6.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(171, 28);
            this.layoutControlItem6.Tag = "Y";
            this.layoutControlItem6.Text = "OrdDate";
            this.layoutControlItem6.TextSize = new System.Drawing.Size(50, 18);
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.dteEndOrdDate;
            this.layoutControlItem7.Location = new System.Drawing.Point(0, 28);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(171, 28);
            this.layoutControlItem7.Text = "∼";
            this.layoutControlItem7.TextSize = new System.Drawing.Size(50, 18);
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.grcOrd;
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 56);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(171, 647);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // splitterItem1
            // 
            this.splitterItem1.AllowHotTrack = true;
            this.splitterItem1.Location = new System.Drawing.Point(195, 0);
            this.splitterItem1.Name = "splitterItem1";
            this.splitterItem1.Size = new System.Drawing.Size(10, 748);
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(411, 0);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(739, 28);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // frmOrderCode
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1298, 768);
            this.Controls.Add(this.layoutControl1);
            this.Name = "frmOrderCode";
            this.Text = "frmOrderCode";
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grcOrd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvOrd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteEndOrdDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteEndOrdDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteStrOrdDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteStrOrdDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteOrdDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteOrdDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grcInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcgOrd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitterItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraGrid.GridControl grcInput;
        private DevExpress.XtraGrid.Views.Grid.GridView grvInput;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.SimpleButton btnDeleteRow;
        private DevExpress.XtraEditors.SimpleButton btnAppendRow;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraEditors.DateEdit dteEndOrdDate;
        private DevExpress.XtraEditors.DateEdit dteStrOrdDate;
        private DevExpress.XtraEditors.DateEdit dteOrdDate;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlGroup lcgOrd;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraLayout.SplitterItem splitterItem1;
        private DevExpress.XtraGrid.GridControl grcOrd;
        private DevExpress.XtraGrid.Views.Grid.GridView grvOrd;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
    }
}