
namespace MasterMES.FORMS.CHART
{
    partial class frmProcessResultDate1
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
            this.grcProcRslt = new DevExpress.XtraGrid.GridControl();
            this.grvProcRslt = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.txtOrderNo = new DevExpress.XtraEditors.TextEdit();
            this.sleItemCode = new DevExpress.XtraEditors.SearchLookUpEdit();
            this.searchLookUpEdit2View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.sleCustCode = new DevExpress.XtraEditors.SearchLookUpEdit();
            this.searchLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gleRoutingCode = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.dteEndWorkDate = new DevExpress.XtraEditors.DateEdit();
            this.dteStrWorkDate = new DevExpress.XtraEditors.DateEdit();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grcProcRslt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvProcRslt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOrderNo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sleItemCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEdit2View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sleCustCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gleRoutingCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteEndWorkDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteEndWorkDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteStrWorkDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteStrWorkDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.grcProcRslt);
            this.layoutControl1.Controls.Add(this.txtOrderNo);
            this.layoutControl1.Controls.Add(this.sleItemCode);
            this.layoutControl1.Controls.Add(this.sleCustCode);
            this.layoutControl1.Controls.Add(this.gleRoutingCode);
            this.layoutControl1.Controls.Add(this.dteEndWorkDate);
            this.layoutControl1.Controls.Add(this.dteStrWorkDate);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(1798, 868);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // grcProcRslt
            // 
            this.grcProcRslt.Location = new System.Drawing.Point(12, 81);
            this.grcProcRslt.MainView = this.grvProcRslt;
            this.grcProcRslt.Name = "grcProcRslt";
            this.grcProcRslt.Size = new System.Drawing.Size(1774, 775);
            this.grcProcRslt.TabIndex = 10;
            this.grcProcRslt.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grvProcRslt});
            // 
            // grvProcRslt
            // 
            this.grvProcRslt.GridControl = this.grcProcRslt;
            this.grvProcRslt.Name = "grvProcRslt";
            this.grvProcRslt.OptionsView.AllowCellMerge = true;
            this.grvProcRslt.CellMerge += new DevExpress.XtraGrid.Views.Grid.CellMergeEventHandler(this.grvProcRslt_CellMerge);
            this.grvProcRslt.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.grvProcRslt_RowStyle);
            // 
            // txtOrderNo
            // 
            this.txtOrderNo.Location = new System.Drawing.Point(1281, 45);
            this.txtOrderNo.MaximumSize = new System.Drawing.Size(200, 0);
            this.txtOrderNo.MinimumSize = new System.Drawing.Size(200, 0);
            this.txtOrderNo.Name = "txtOrderNo";
            this.txtOrderNo.Size = new System.Drawing.Size(200, 20);
            this.txtOrderNo.StyleController = this.layoutControl1;
            this.txtOrderNo.TabIndex = 9;
            // 
            // sleItemCode
            // 
            this.sleItemCode.Location = new System.Drawing.Point(995, 45);
            this.sleItemCode.MaximumSize = new System.Drawing.Size(200, 0);
            this.sleItemCode.MinimumSize = new System.Drawing.Size(200, 0);
            this.sleItemCode.Name = "sleItemCode";
            this.sleItemCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.sleItemCode.Properties.PopupView = this.searchLookUpEdit2View;
            this.sleItemCode.Size = new System.Drawing.Size(200, 20);
            this.sleItemCode.StyleController = this.layoutControl1;
            this.sleItemCode.TabIndex = 8;
            // 
            // searchLookUpEdit2View
            // 
            this.searchLookUpEdit2View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.searchLookUpEdit2View.Name = "searchLookUpEdit2View";
            this.searchLookUpEdit2View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.searchLookUpEdit2View.OptionsView.ShowGroupPanel = false;
            // 
            // sleCustCode
            // 
            this.sleCustCode.Location = new System.Drawing.Point(709, 45);
            this.sleCustCode.MaximumSize = new System.Drawing.Size(200, 0);
            this.sleCustCode.MinimumSize = new System.Drawing.Size(200, 0);
            this.sleCustCode.Name = "sleCustCode";
            this.sleCustCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.sleCustCode.Properties.PopupView = this.searchLookUpEdit1View;
            this.sleCustCode.Size = new System.Drawing.Size(200, 20);
            this.sleCustCode.StyleController = this.layoutControl1;
            this.sleCustCode.TabIndex = 7;
            // 
            // searchLookUpEdit1View
            // 
            this.searchLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.searchLookUpEdit1View.Name = "searchLookUpEdit1View";
            this.searchLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.searchLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // gleRoutingCode
            // 
            this.gleRoutingCode.Location = new System.Drawing.Point(473, 45);
            this.gleRoutingCode.MaximumSize = new System.Drawing.Size(150, 0);
            this.gleRoutingCode.MinimumSize = new System.Drawing.Size(150, 0);
            this.gleRoutingCode.Name = "gleRoutingCode";
            this.gleRoutingCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.gleRoutingCode.Properties.PopupView = this.gridLookUpEdit1View;
            this.gleRoutingCode.Size = new System.Drawing.Size(150, 20);
            this.gleRoutingCode.StyleController = this.layoutControl1;
            this.gleRoutingCode.TabIndex = 6;
            // 
            // gridLookUpEdit1View
            // 
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // dteEndWorkDate
            // 
            this.dteEndWorkDate.EditValue = null;
            this.dteEndWorkDate.Location = new System.Drawing.Point(257, 45);
            this.dteEndWorkDate.MaximumSize = new System.Drawing.Size(130, 0);
            this.dteEndWorkDate.MinimumSize = new System.Drawing.Size(130, 0);
            this.dteEndWorkDate.Name = "dteEndWorkDate";
            this.dteEndWorkDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dteEndWorkDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dteEndWorkDate.Size = new System.Drawing.Size(130, 20);
            this.dteEndWorkDate.StyleController = this.layoutControl1;
            this.dteEndWorkDate.TabIndex = 5;
            // 
            // dteStrWorkDate
            // 
            this.dteStrWorkDate.EditValue = null;
            this.dteStrWorkDate.Location = new System.Drawing.Point(106, 45);
            this.dteStrWorkDate.MaximumSize = new System.Drawing.Size(130, 0);
            this.dteStrWorkDate.MinimumSize = new System.Drawing.Size(130, 0);
            this.dteStrWorkDate.Name = "dteStrWorkDate";
            this.dteStrWorkDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dteStrWorkDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dteStrWorkDate.Size = new System.Drawing.Size(130, 20);
            this.dteStrWorkDate.StyleController = this.layoutControl1;
            this.dteStrWorkDate.TabIndex = 4;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlGroup1,
            this.layoutControlItem7});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(1798, 868);
            this.Root.TextVisible = false;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.layoutControlItem4,
            this.layoutControlItem5,
            this.layoutControlItem6,
            this.emptySpaceItem2});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(1778, 69);
            this.layoutControlGroup1.Text = "Query Cond";
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.dteStrWorkDate;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(216, 24);
            this.layoutControlItem1.Text = "WorkDate";
            this.layoutControlItem1.TextSize = new System.Drawing.Size(70, 14);
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.dteEndWorkDate;
            this.layoutControlItem2.Location = new System.Drawing.Point(216, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(151, 24);
            this.layoutControlItem2.Text = "∼";
            this.layoutControlItem2.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
            this.layoutControlItem2.TextSize = new System.Drawing.Size(12, 14);
            this.layoutControlItem2.TextToControlDistance = 5;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.gleRoutingCode;
            this.layoutControlItem3.Location = new System.Drawing.Point(367, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(236, 24);
            this.layoutControlItem3.Text = "RoutingCode";
            this.layoutControlItem3.TextSize = new System.Drawing.Size(70, 14);
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.sleCustCode;
            this.layoutControlItem4.Location = new System.Drawing.Point(603, 0);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(286, 24);
            this.layoutControlItem4.Text = "CustCode";
            this.layoutControlItem4.TextSize = new System.Drawing.Size(70, 14);
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.sleItemCode;
            this.layoutControlItem5.Location = new System.Drawing.Point(889, 0);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(286, 24);
            this.layoutControlItem5.Text = "ItemCode";
            this.layoutControlItem5.TextSize = new System.Drawing.Size(70, 14);
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.txtOrderNo;
            this.layoutControlItem6.Location = new System.Drawing.Point(1175, 0);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(286, 24);
            this.layoutControlItem6.Text = "OrderNo";
            this.layoutControlItem6.TextSize = new System.Drawing.Size(70, 14);
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(1461, 0);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(293, 24);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.grcProcRslt;
            this.layoutControlItem7.Location = new System.Drawing.Point(0, 69);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(1778, 779);
            this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem7.TextVisible = false;
            // 
            // frmProcessResultDate1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1798, 868);
            this.Controls.Add(this.layoutControl1);
            this.Name = "frmProcessResultDate1";
            this.Text = "frmProcessResultDate1";
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grcProcRslt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvProcRslt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOrderNo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sleItemCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEdit2View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sleCustCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gleRoutingCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteEndWorkDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteEndWorkDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteStrWorkDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteStrWorkDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.TextEdit txtOrderNo;
        private DevExpress.XtraEditors.SearchLookUpEdit sleItemCode;
        private DevExpress.XtraGrid.Views.Grid.GridView searchLookUpEdit2View;
        private DevExpress.XtraEditors.SearchLookUpEdit sleCustCode;
        private DevExpress.XtraGrid.Views.Grid.GridView searchLookUpEdit1View;
        private DevExpress.XtraEditors.GridLookUpEdit gleRoutingCode;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraEditors.DateEdit dteEndWorkDate;
        private DevExpress.XtraEditors.DateEdit dteStrWorkDate;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraGrid.GridControl grcProcRslt;
        private DevExpress.XtraGrid.Views.Grid.GridView grvProcRslt;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
    }
}