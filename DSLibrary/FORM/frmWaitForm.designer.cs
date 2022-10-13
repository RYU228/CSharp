
namespace DSLibrary {
    partial class frmWaitForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.progressPanel = new DevExpress.XtraWaitForm.ProgressPanel();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(62)))), ((int)(((byte)(77)))));
            this.panelControl1.Appearance.Options.UseBackColor = true;
            this.panelControl1.AutoSize = true;
            this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.panelControl1.Controls.Add(this.progressPanel);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            this.panelControl1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.panelControl1.MinimumSize = new System.Drawing.Size(320, 120);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Padding = new System.Windows.Forms.Padding(5);
            this.panelControl1.Size = new System.Drawing.Size(320, 120);
            this.panelControl1.TabIndex = 4;
            // 
            // progressPanel
            // 
            this.progressPanel.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(94)))), ((int)(((byte)(113)))));
            this.progressPanel.Appearance.Font = new System.Drawing.Font("IBM Plex Sans KR", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.progressPanel.Appearance.ForeColor = System.Drawing.Color.White;
            this.progressPanel.Appearance.Options.UseBackColor = true;
            this.progressPanel.Appearance.Options.UseFont = true;
            this.progressPanel.Appearance.Options.UseForeColor = true;
            this.progressPanel.AppearanceCaption.Font = new System.Drawing.Font("IBM Plex Sans KR", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.progressPanel.AppearanceCaption.ForeColor = System.Drawing.Color.White;
            this.progressPanel.AppearanceCaption.Options.UseFont = true;
            this.progressPanel.AppearanceCaption.Options.UseForeColor = true;
            this.progressPanel.AppearanceDescription.Font = new System.Drawing.Font("IBM Plex Sans KR", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.progressPanel.AppearanceDescription.ForeColor = System.Drawing.Color.White;
            this.progressPanel.AppearanceDescription.Options.UseFont = true;
            this.progressPanel.AppearanceDescription.Options.UseForeColor = true;
            this.progressPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.progressPanel.Caption = "메세지";
            this.progressPanel.ContentAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.progressPanel.Description = "처리중입니다. 잠시만 기다려주세요....";
            this.progressPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressPanel.FrameCount = 28000;
            this.progressPanel.Location = new System.Drawing.Point(7, 7);
            this.progressPanel.Margin = new System.Windows.Forms.Padding(0);
            this.progressPanel.MinimumSize = new System.Drawing.Size(306, 106);
            this.progressPanel.Name = "progressPanel";
            this.progressPanel.Padding = new System.Windows.Forms.Padding(5);
            this.progressPanel.Size = new System.Drawing.Size(306, 106);
            this.progressPanel.TabIndex = 5;
            this.progressPanel.WaitAnimationType = DevExpress.Utils.Animation.WaitingAnimatorType.Line;
            // 
            // frmWaitForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(320, 120);
            this.Controls.Add(this.panelControl1);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(320, 120);
            this.Name = "frmWaitForm";
            this.ShowOnTopMode = DevExpress.XtraWaitForm.ShowFormOnTopMode.AboveAll;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraWaitForm.ProgressPanel progressPanel;
    }
}
