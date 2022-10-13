using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DSLibrary;

namespace MasterMES.ETC
{
    public partial class frmSetApproval : frmBase
    {
        #region 생성자
        public frmSetApproval()
        {
            InitializeComponent();
        }

        #endregion

        #region 오버라이드 이벤트
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.InitSetApproval();
        }

        #endregion

        #region 폼 컨트롤 이벤트

        #endregion

        #region 사용자 정의 메소드
        /// <summary>
        /// 화면 초기화
        /// </summary>
        private void InitSetApproval()
        {

        }
        #endregion


    }
}