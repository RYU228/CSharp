
namespace MasterMES.FORMS.CODE
{
    partial class frmProcessCode
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
            this.txtGruopCode = new DevExpress.XtraEditors.TextEdit();
            this.bteGroupName = new DevExpress.XtraEditors.ButtonEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.grcCopy = new DevExpress.XtraGrid.GridControl();
            this.grvCopy = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.grcGroup = new DevExpress.XtraGrid.GridControl();
            this.grvGroup = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.splitterItem1 = new DevExpress.XtraLayout.SplitterItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtGruopCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bteGroupName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grcCopy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvCopy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grcGroup)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvGroup)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitterItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.txtGruopCode);
            this.layoutControl1.Controls.Add(this.bteGroupName);
            this.layoutControl1.Controls.Add(this.labelControl2);
            this.layoutControl1.Controls.Add(this.labelControl1);
            this.layoutControl1.Controls.Add(this.grcCopy);
            this.layoutControl1.Controls.Add(this.grcGroup);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(1725, 507, 650, 400);
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(1298, 768);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // txtGruopCode
            // 
            this.txtGruopCode.Location = new System.Drawing.Point(1049, 724);
            this.txtGruopCode.Name = "txtGruopCode";
            this.txtGruopCode.Properties.ReadOnly = true;
            this.txtGruopCode.Size = new System.Drawing.Size(225, 20);
            this.txtGruopCode.StyleController = this.layoutControl1;
            this.txtGruopCode.TabIndex = 9;
            // 
            // bteGroupName
            // 
            this.bteGroupName.Location = new System.Drawing.Point(744, 724);
            this.bteGroupName.Name = "bteGroupName";
            this.bteGroupName.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.bteGroupName.Size = new System.Drawing.Size(225, 20);
            this.bteGroupName.StyleController = this.layoutControl1;
            this.bteGroupName.TabIndex = 8;
            this.bteGroupName.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bteGroupName_ButtonClick);
            // 
            // labelControl2
            // 
            this.labelControl2.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl2.Location = new System.Drawing.Point(656, 12);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(630, 14);
            this.labelControl2.StyleController = this.layoutControl1;
            this.labelControl2.TabIndex = 7;
            this.labelControl2.Text = "행을 끌어 놓거나 더블클릭하면 반대 그리드에 행이 추가 됩니다. (복사본은 삭제됩니다.)";
            // 
            // labelControl1
            // 
            this.labelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl1.Location = new System.Drawing.Point(12, 12);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(630, 14);
            this.labelControl1.StyleController = this.layoutControl1;
            this.labelControl1.TabIndex = 6;
            this.labelControl1.Text = "행을 끌어 놓거나 더블클릭하면 반대 그리드에 행이 추가 됩니다. (원본은 삭제되지 않습니다.)";
            // 
            // grcCopy
            // 
            this.grcCopy.Location = new System.Drawing.Point(656, 30);
            this.grcCopy.MainView = this.grvCopy;
            this.grcCopy.Name = "grcCopy";
            this.grcCopy.Size = new System.Drawing.Size(630, 657);
            this.grcCopy.TabIndex = 5;
            this.grcCopy.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grvCopy});
            // 
            // grvCopy
            // 
            this.grvCopy.GridControl = this.grcCopy;
            this.grvCopy.Name = "grvCopy";
            // 
            // grcGroup
            // 
            this.grcGroup.Location = new System.Drawing.Point(12, 30);
            this.grcGroup.MainView = this.grvGroup;
            this.grcGroup.Name = "grcGroup";
            this.grcGroup.Size = new System.Drawing.Size(630, 726);
            this.grcGroup.TabIndex = 4;
            this.grcGroup.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grvGroup});
            // 
            // grvGroup
            // 
            this.grvGroup.GridControl = this.grcGroup;
            this.grvGroup.Name = "grvGroup";
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.splitterItem1,
            this.layoutControlItem3,
            this.layoutControlItem4,
            this.layoutControlGroup1});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(1298, 768);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.grcGroup;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 18);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(634, 730);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.grcCopy;
            this.layoutControlItem2.Location = new System.Drawing.Point(644, 18);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(634, 661);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // splitterItem1
            // 
            this.splitterItem1.AllowHotTrack = true;
            this.splitterItem1.Location = new System.Drawing.Point(634, 0);
            this.splitterItem1.Name = "splitterItem1";
            this.splitterItem1.Size = new System.Drawing.Size(10, 748);
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.labelControl1;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(634, 18);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.labelControl2;
            this.layoutControlItem4.Location = new System.Drawing.Point(644, 0);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(634, 18);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem5,
            this.layoutControlItem6});
            this.layoutControlGroup1.Location = new System.Drawing.Point(644, 679);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(634, 69);
            this.layoutControlGroup1.Text = "FindValue 예제";
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.bteGroupName;
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(305, 24);
            this.layoutControlItem5.Text = "GroupName";
            this.layoutControlItem5.TextSize = new System.Drawing.Size(64, 14);
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.txtGruopCode;
            this.layoutControlItem6.Location = new System.Drawing.Point(305, 0);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(305, 24);
            this.layoutControlItem6.Text = "GroupCode";
            this.layoutControlItem6.TextSize = new System.Drawing.Size(64, 14);
            // 
            // frmProcessCode
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1298, 768);
            this.Controls.Add(this.layoutControl1);
            this.Name = "frmProcessCode";
            this.Text = "frmProcessCode";
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtGruopCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bteGroupName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grcCopy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvCopy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grcGroup)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvGroup)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitterItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraGrid.GridControl grcCopy;
        private DevExpress.XtraGrid.Views.Grid.GridView grvCopy;
        private DevExpress.XtraGrid.GridControl grcGroup;
        private DevExpress.XtraGrid.Views.Grid.GridView grvGroup;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.SplitterItem splitterItem1;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraEditors.TextEdit txtGruopCode;
        private DevExpress.XtraEditors.ButtonEdit bteGroupName;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
    }
}