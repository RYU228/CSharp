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
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DSLibrary;

namespace MasterMES.FORMS.CHART
{
    public partial class frmProcessResultDate : frmBase, CLASSES.IButtonAction
    {
        public frmProcessResultDate()
        {
            InitializeComponent();
        }

        #region 오버라이드 이벤트
        //protected override void OnActivated(EventArgs e)
        //{
        //    base.OnActivated(e);
        //    MainButton.ActiveButton("INIT,NEW,VIEW");
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
            try
            {
                Utils.ShowWaitForm(this);
                MainButton.InitButton = false;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Initial, SqlDbType.Char);

                var initSet = SqlHelper.GetDataSet("xp_Report_ProcessResultDate", inParams, out string retCode, out string retMsg);
                if (retCode != "00")
                    return;

                gleRoutingCode.SetData(initSet.Tables[0], "", "1");
                sleCustCode.SetData(initSet.Tables[1]);
                sleItemCode.SetData(initSet.Tables[2]);
                grcProcRslt.SetColumns(initSet.Tables[3]);

                grvProcRslt.OptionsBehavior.AutoExpandAllGroups = true;
                grvProcRslt.OptionsView.ShowGroupedColumns = true;

                grvProcRslt.Columns["WorkDate"].GroupIndex = 0;
                grvProcRslt.SetGroupSummaryItem("WorkDate", DevExpress.Data.SummaryItemType.Sum, "일계");
                grvProcRslt.SetGroupSummaryItem("WorkRoll", DevExpress.Data.SummaryItemType.Sum, "{0:N0}");
                grvProcRslt.SetGroupSummaryItem("WorkQty", DevExpress.Data.SummaryItemType.Sum, "{0:N0}");

                grvProcRslt.SetSummaryItem("WorkDate", DevExpress.Data.SummaryItemType.Custom, "합계");
                grvProcRslt.SetSummaryItem("WorkRoll", DevExpress.Data.SummaryItemType.Sum, "{0:N0}");
                grvProcRslt.SetSummaryItem("WorkQty", DevExpress.Data.SummaryItemType.Sum, "{0:N0}");

                DxControl.SaveGridFormat(grvProcRslt, GridFormat.Default);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
            finally
            {
                MainButton.InitButton = true;
                Utils.CloseWaitForm();
            }
        }

        /// <summary>
        /// 조회 버튼
        /// </summary>
        public void ViewButtonClick()
        {
            try
            {
                var startTime = DateTime.Now;

                Utils.ShowWaitForm(this);
                MainButton.ViewButton = false;

                var inParams = new DbParamList();

                inParams.Add("@i_ProcessID", ProcessType.GetList, SqlDbType.Char);
                inParams.Add("@i_StrWorkDate", dteStrWorkDate.EditValue.CDateString("yyyyMMdd"), SqlDbType.Char);
                inParams.Add("@i_EndWorkDate", dteEndWorkDate.EditValue.CDateString("yyyyMMdd"), SqlDbType.Char);
                inParams.Add("@i_CustomID", sleCustCode.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_AriticleID", sleItemCode.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_OrderNo", txtOrderNo.EditValue.CString(), SqlDbType.VarChar);

                var processResultDate = SqlHelper.GetDataTable("xp_Report_ProcessResultDate", inParams, out _, out _);
                grcProcRslt.SetData(processResultDate);

                DxControl.LoadGridFormat(grvProcRslt, GridFormat.Current);  // 데이터 로딩 후 저장된 그리드 형식을 불러옴

                SqlHelper.WriteLog(this, ActionLogType.Select, startTime);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
            finally
            {
                MainButton.ViewButton = true;
                Utils.CloseWaitForm();
            }
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

        #region 폼 컨트롤 이벤트
        /// <summary>
        /// Group Row 설정
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvProcRslt_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            var view = sender as GridView;
            var info = e.Info as GridGroupRowInfo;

            var workDate = view.GetGroupRowValue(e.RowHandle, view.Columns["WorkDate"]).CString().Insert(4, "-").Insert(7, "-");
            info.GroupText = $"작업일 : {workDate}";

            e.Appearance.FontStyleDelta = FontStyle.Bold;
            e.Appearance.ForeColor = Color.Yellow;
        }

        /// <summary>
        /// Group Footer 설정
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvProcRslt_CustomDrawRowFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {
            e.Appearance.FontStyleDelta = FontStyle.Bold;
            e.Appearance.ForeColor = Color.Yellow;
        }

        /// <summary>
        /// Footer Cell의 색상 변경
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvProcRslt_CustomDrawFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {
            e.Appearance.FontStyleDelta = FontStyle.Bold;
            e.Appearance.ForeColor = Color.Yellow;
        }

        /// <summary>
        /// 셀 표시 텍스트 변경
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvProcRslt_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.ListSourceRowIndex >= 0)
            {
                if (new string[] { "WorkDate", "OrderID" }.Contains(e.Column.FieldName))
                {
                    var text = (sender as GridView).GetRowCellValue(e.ListSourceRowIndex, e.Column.FieldName).CString();

                    if (e.Column.FieldName == "WorkDate")
                        e.DisplayText = $"{text.Substring(0, 4)}-{text.Substring(4, 2)}-{text.Substring(6, 2)}";
                    else
                        e.DisplayText = $"{text.Substring(0, 4)}-{text.Substring(4, 2)}-{text.Substring(6, 4)}";
                }
            }
        }

        /// <summary>
        /// 셀 병합
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvProcRslt_CellMerge(object sender, CellMergeEventArgs e)
        {
            var view = sender as GridView;

            if (e.Column.FieldName == "WorkDate")
            {
                var val1 = view.GetRowCellDisplayText(e.RowHandle1, "WorkDate");
                var val2 = view.GetRowCellDisplayText(e.RowHandle2, "WorkDate");
                e.Merge = (val1 == val2);
            }
            else
            {
                e.Column.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            }

            e.Handled = true;
        }

        /// <summary>
        /// 다른 샘플 화면 호출
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCallForm_Click(object sender, EventArgs e)
        {
            var f1 = new frmProcessResultDate1();
            f1.Tag = new FormLog("04110", DateTime.Now);

            Utils.ShowMidChildForm(AppConfig.MainForm, f1);
        }
        #endregion

        #region 사용자 정의 메서드

        #endregion
    }
}