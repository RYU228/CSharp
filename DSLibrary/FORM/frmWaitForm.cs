using System;
using DevExpress.XtraWaitForm;
using System.Drawing;

namespace DSLibrary
{
    public partial class frmWaitForm : WaitForm
    {
        /// <summary>
        /// 생성자
        /// </summary>
        public frmWaitForm()
        {
            InitializeComponent();
            //this.progressPanel.AutoHeight = true;
            
            this.progressPanel.AutoSize = true;
            this.ShowOnTopMode = ShowFormOnTopMode.AboveParent;// ShowFormOnTopMode.AboveAll;

            if (AppConfig.App.SkinType == "D")
            {
                progressPanel.ForeColor = Color.White;
                progressPanel.AppearanceCaption.ForeColor = Color.White;
                progressPanel.AppearanceDescription.ForeColor = Color.White;
                progressPanel.BackColor = Color.FromArgb(78,81,97);
                panelControl1.BackColor = Color.FromArgb(59, 62, 77);
            }
            else
            {
                progressPanel.ForeColor = Color.Black;
                progressPanel.AppearanceCaption.ForeColor = Color.Black;
                progressPanel.AppearanceDescription.ForeColor = Color.Black;
                progressPanel.BackColor = Color.FromArgb(235,236,239);
                panelControl1.BackColor = Color.FromArgb(200, 201, 208);
            }
        }

        #region Overrides
        /// <summary>
        /// 제목 설정하기
        /// </summary>
        /// <param name="caption"></param>
        public override void SetCaption(string caption)
        {
            base.SetCaption(caption);
            this.progressPanel.Caption = caption;
        }

        /// <summary>
        /// 설명 설정하기
        /// </summary>
        /// <param name="description">설명</param>
        public override void SetDescription(string description)
        {
            base.SetDescription(description);
            this.progressPanel.Description = description;
        }

        /// <summary>
        /// 명령 처리하기
        /// </summary>
        /// <param name="enumeration">열거형</param>
        /// <param name="argument">인자</param>
        public override void ProcessCommand(Enum cmd, object arg)
        {
            base.ProcessCommand(cmd, arg);
        }

        #endregion

        //public enum WaitFormCommand
        //{
        //}
    }
}