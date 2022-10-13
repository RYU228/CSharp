using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DSLibrary;
using MasterMES.CLASSES;

namespace MasterMES.FORMS.QUALITY
{
    public partial class frmNgResult : frmBase, IButtonAction
    {
        #region 생성자
        public frmNgResult()
        {
            InitializeComponent();
        }

        #endregion

        #region 오버라이드 이벤트
        //protected override void OnActivated(EventArgs e)
        //{
        //    base.OnActivated(e);
        //    MainButton.ActiveButton("INIT,NEW,COPY,SAVE,DELETE");
        //}

        //protected override void OnDeactivate(EventArgs e)
        //{
        //    base.OnDeactivate(e);
        //    MainButton.ActiveButton();
        //}

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.InitButtonClick();
        }

        #endregion

        #region 인터페이스 - 공용 버튼 클릭
        /// <summary>
        /// 초기화 버튼
        /// </summary>
        public void InitButtonClick()
        {
            
        }

        /// <summary>
        /// 조회 버튼
        /// </summary>
        public void ViewButtonClick()
        {
            
        }

        /// <summary>
        /// 신규 버튼
        /// </summary>
        public void NewButtonClick()
        {
            
        }

        /// <summary>
        /// 복사 버튼
        /// </summary>
        public void CopyButtonClick()
        {
            
        }

        /// <summary>
        /// 저장 버튼
        /// </summary>
        public void SaveButtonClick()
        {
            
        }

        /// <summary>
        /// 삭제 버튼
        /// </summary>
        public void DeleteButtonClick()
        {
            
        }

        /// <summary>
        /// 출력 버튼
        /// </summary>
        public void PrintButtonClick(int reportIndex)
        {

        }

        #endregion

    }
}
