using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DSLibrary;
using MasterMES.CLASSES;

namespace MasterMES.FORMS.SALES
{
    public partial class frmReviewCon : frmBase, IButtonAction
    {
        #region 생성자
        public frmReviewCon()
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
            //try
            //{
            //    MainButton.PrintButton = false;

            //    var inParams = new DbParamList();
            //    inParams.Add("@i_ProcessID", ProcessType.Print, SqlDbType.Char);
            //    inParams.Add("@i_LanguageCode", "LANKOR", SqlDbType.VarChar);

            //    var reportSource = SqlHelper.GetDataTable("xp_Glossary", inParams, out string retCode, out string retMsg);

            //    // 비정상 코드를 반환하면 오류 처리
            //    if (retCode != "00")
            //        throw new Exception(retMsg);

            //    if (reportIndex == 1)   // 단일 리포트 이면 해당 If 문은 필요하지 않음
            //    {
            //        var condition = $"{"LanguageCode".Localization()} : " + $"{gleLanguageCode.Properties.GetDisplayText(gleLanguageCode.EditValue)}";
            //        var rptPortrait = new REPORTS.DESIGN.RPT_Glossary_Portrait(condition);
            //        rptPortrait.Preview(reportSource, true);
            //    }
            //    else if (reportIndex == 2)
            //    {
            //        var rptLandscape = new REPORTS.DESIGN.RPT_Glossary_Landscape();
            //        rptLandscape.Preview(reportSource, false);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MsgBox.ErrorMessage(ex.CString());
            //}
            //finally
            //{
            //    MainButton.PrintButton = true;
            //}

        }

        #endregion

    }
}
