
namespace MasterMES.FORMS.SALES
{
    partial class frmReviewCon
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
            this.components = new System.ComponentModel.Container();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.mmeRemark = new DevExpress.XtraEditors.MemoEdit();
            this.txtAddress = new DevExpress.XtraEditors.TextEdit();
            this.txtReceiptID = new DevExpress.XtraEditors.TextEdit();
            this.grcReceipt = new DevExpress.XtraGrid.GridControl();
            this.grvReceipt = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.bteCustom = new DevExpress.XtraEditors.ButtonEdit();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lcgReceipt = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem28 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem16 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.splitterItem1 = new DevExpress.XtraLayout.SplitterItem();
            this.behaviorManager1 = new DevExpress.Utils.Behaviors.BehaviorManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mmeRemark.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAddress.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtReceiptID.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grcReceipt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvReceipt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bteCustom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcgReceipt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem28)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem16)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitterItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.behaviorManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.AllowCustomization = false;
            this.layoutControl1.Controls.Add(this.mmeRemark);
            this.layoutControl1.Controls.Add(this.txtAddress);
            this.layoutControl1.Controls.Add(this.txtReceiptID);
            this.layoutControl1.Controls.Add(this.grcReceipt);
            this.layoutControl1.Controls.Add(this.bteCustom);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(1747, 507, 650, 400);
            this.layoutControl1.OptionsFocus.AllowFocusGroups = false;
            this.layoutControl1.OptionsFocus.EnableAutoTabOrder = false;
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(1489, 896);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // mmeRemark
            // 
            this.mmeRemark.Location = new System.Drawing.Point(73, 114);
            this.mmeRemark.Name = "mmeRemark";
            this.mmeRemark.Properties.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(94)))), ((int)(((byte)(94)))));
            this.mmeRemark.Properties.Appearance.Options.UseBorderColor = true;
            this.mmeRemark.Size = new System.Drawing.Size(341, 56);
            this.mmeRemark.StyleController = this.layoutControl1;
            this.mmeRemark.TabIndex = 7;
            // 
            // txtAddress
            // 
            this.txtAddress.EnterMoveNextControl = true;
            this.txtAddress.Location = new System.Drawing.Point(73, 86);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.txtAddress.Properties.ReadOnly = true;
            this.txtAddress.Size = new System.Drawing.Size(341, 24);
            this.txtAddress.StyleController = this.layoutControl1;
            this.txtAddress.TabIndex = 3;
            this.txtAddress.TabStop = false;
            // 
            // txtReceiptID
            // 
            this.txtReceiptID.Location = new System.Drawing.Point(73, 30);
            this.txtReceiptID.Name = "txtReceiptID";
            this.txtReceiptID.Properties.ReadOnly = true;
            this.txtReceiptID.Size = new System.Drawing.Size(341, 24);
            this.txtReceiptID.StyleController = this.layoutControl1;
            this.txtReceiptID.TabIndex = 0;
            // 
            // grcReceipt
            // 
            this.grcReceipt.Location = new System.Drawing.Point(433, 4);
            this.grcReceipt.MainView = this.grvReceipt;
            this.grcReceipt.Name = "grcReceipt";
            this.grcReceipt.Size = new System.Drawing.Size(1052, 888);
            this.grcReceipt.TabIndex = 4;
            this.grcReceipt.TabStop = false;
            this.grcReceipt.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grvReceipt});
            // 
            // grvReceipt
            // 
            this.grvReceipt.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.grvReceipt.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.grvReceipt.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.grvReceipt.Appearance.HorzLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.grvReceipt.Appearance.HorzLine.Options.UseBackColor = true;
            this.grvReceipt.Appearance.VertLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.grvReceipt.Appearance.VertLine.Options.UseBackColor = true;
            this.grvReceipt.ColumnPanelRowHeight = 23;
            this.grvReceipt.GridControl = this.grcReceipt;
            this.grvReceipt.Name = "grvReceipt";
            this.grvReceipt.OptionsNavigation.UseTabKey = false;
            this.grvReceipt.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.grvReceipt.OptionsSelection.EnableAppearanceHideSelection = false;
            this.grvReceipt.OptionsView.BestFitMaxRowCount = 35;
            this.grvReceipt.RowHeight = 20;
            this.grvReceipt.VertScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always;
            // 
            // bteCustom
            // 
            this.bteCustom.Location = new System.Drawing.Point(73, 58);
            this.bteCustom.Name = "bteCustom";
            this.bteCustom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.bteCustom.Size = new System.Drawing.Size(341, 24);
            this.bteCustom.StyleController = this.layoutControl1;
            this.bteCustom.TabIndex = 1;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lcgReceipt,
            this.layoutControlItem1,
            this.splitterItem1});
            this.Root.Name = "Root";
            this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
            this.Root.Size = new System.Drawing.Size(1489, 896);
            this.Root.TextVisible = false;
            // 
            // lcgReceipt
            // 
            this.lcgReceipt.AppearanceGroup.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.lcgReceipt.AppearanceGroup.Options.UseFont = true;
            this.lcgReceipt.HeaderButtonsLocation = DevExpress.Utils.GroupElementLocation.AfterText;
            this.lcgReceipt.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem2,
            this.emptySpaceItem1,
            this.layoutControlItem28,
            this.layoutControlItem16,
            this.layoutControlItem7});
            this.lcgReceipt.Location = new System.Drawing.Point(0, 0);
            this.lcgReceipt.Name = "lcgReceipt";
            this.lcgReceipt.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
            this.lcgReceipt.Size = new System.Drawing.Size(419, 892);
            this.lcgReceipt.Text = "ReviewCon Info";
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.txtReceiptID;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(409, 28);
            this.layoutControlItem2.Tag = "";
            this.layoutControlItem2.Text = "관리번호";
            this.layoutControlItem2.TextSize = new System.Drawing.Size(52, 18);
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.AppearanceItemCaption.Options.UseTextOptions = true;
            this.emptySpaceItem1.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 144);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(409, 717);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem28
            // 
            this.layoutControlItem28.Control = this.mmeRemark;
            this.layoutControlItem28.Location = new System.Drawing.Point(0, 84);
            this.layoutControlItem28.MaxSize = new System.Drawing.Size(0, 60);
            this.layoutControlItem28.MinSize = new System.Drawing.Size(111, 60);
            this.layoutControlItem28.Name = "layoutControlItem28";
            this.layoutControlItem28.Size = new System.Drawing.Size(409, 60);
            this.layoutControlItem28.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem28.Text = "리뷰내용";
            this.layoutControlItem28.TextSize = new System.Drawing.Size(52, 18);
            // 
            // layoutControlItem16
            // 
            this.layoutControlItem16.Control = this.bteCustom;
            this.layoutControlItem16.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
            this.layoutControlItem16.CustomizationFormText = "Custom";
            this.layoutControlItem16.Location = new System.Drawing.Point(0, 28);
            this.layoutControlItem16.Name = "layoutControlItem16";
            this.layoutControlItem16.OptionsTableLayoutItem.RowIndex = 2;
            this.layoutControlItem16.Size = new System.Drawing.Size(409, 28);
            this.layoutControlItem16.Tag = "Y";
            this.layoutControlItem16.Text = "KCustom";
            this.layoutControlItem16.TextSize = new System.Drawing.Size(52, 18);
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.txtAddress;
            this.layoutControlItem7.Location = new System.Drawing.Point(0, 56);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(409, 28);
            this.layoutControlItem7.Text = "프로젝트";
            this.layoutControlItem7.TextSize = new System.Drawing.Size(52, 18);
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem1.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem1.Control = this.grcReceipt;
            this.layoutControlItem1.Location = new System.Drawing.Point(429, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(1056, 892);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // splitterItem1
            // 
            this.splitterItem1.AllowHotTrack = true;
            this.splitterItem1.Location = new System.Drawing.Point(419, 0);
            this.splitterItem1.Name = "splitterItem1";
            this.splitterItem1.Size = new System.Drawing.Size(10, 892);
            // 
            // frmReviewCon
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1489, 896);
            this.Controls.Add(this.layoutControl1);
            this.KeyPreview = true;
            this.Name = "frmReviewCon";
            this.Text = "frmReviewCon";
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mmeRemark.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAddress.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtReceiptID.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grcReceipt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvReceipt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bteCustom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcgReceipt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem28)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem16)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitterItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.behaviorManager1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.TextEdit txtReceiptID;
        private DevExpress.XtraGrid.GridControl grcReceipt;
        private DevExpress.XtraGrid.Views.Grid.GridView grvReceipt;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlGroup lcgReceipt;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraEditors.TextEdit txtAddress;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraEditors.MemoEdit mmeRemark;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem28;
        private DevExpress.XtraEditors.ButtonEdit bteCustom;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem16;
        private DevExpress.Utils.Behaviors.BehaviorManager behaviorManager1;
        private DevExpress.XtraLayout.SplitterItem splitterItem1;
    }
}