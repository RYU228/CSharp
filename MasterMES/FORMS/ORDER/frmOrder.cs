using System;
using System.Data;
using System.Drawing;
using System.Linq;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DSLibrary;

namespace MasterMES.FORMS.ORDER
{
    public partial class frmOrder : frmBase, CLASSES.IButtonAction
    {
        public frmOrder()
        {
            InitializeComponent();
            //new GridRowSelection(grvColor);
        }

        #region 오버라이드 이벤트
        //protected override void OnActivated(EventArgs e)
        //{
        //    base.OnActivated(e);
        //    MainButton.ActiveButton("INIT,VIEW,NEW,SAVE,DELETE");
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
                var initSet = SqlHelper.GetDataSet("xp_Order", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                gleCloseClss.SetData(initSet.Tables[0], "", "0");
                gleUnitCostClss.SetData(initSet.Tables[1]);
                grcCustom.SetColumns(initSet.Tables[2], "CustomID", showGroupPanel: false, true, true);

                grvOrder.SetColumns(autoColumnWidth: true);
                grvColor.SetColumns("", false, autoColumnWidth: true);
                grvDayOrder.SetColumns("GroupingID", false, autoColumnWidth: true);

                // 색상별 주문내역 집계
                grvColor.SetSummaryItem("Color", DevExpress.Data.SummaryItemType.Count, "합계(건수={0:N0}건)", "Title");
                grvColor.SetSummaryItem("ColorRoll", DevExpress.Data.SummaryItemType.Sum, "{0:N0}");
                grvColor.SetSummaryItem("ColorQty", DevExpress.Data.SummaryItemType.Sum, "{0:N0}");
                grvColor.SetSummaryItem("StuffInRoll", DevExpress.Data.SummaryItemType.Sum, "{0:N0}");
                grvColor.SetSummaryItem("StuffInQty", DevExpress.Data.SummaryItemType.Sum, "{0:N0}");
                grvColor.SetSummaryItem("PassRoll", DevExpress.Data.SummaryItemType.Sum, "{0:N0}");
                grvColor.SetSummaryItem("PassQty", DevExpress.Data.SummaryItemType.Sum, "{0:N0}");
                grvColor.SetSummaryItem("DefectRoll", DevExpress.Data.SummaryItemType.Sum, "{0:N0}");
                grvColor.SetSummaryItem("DefectQty", DevExpress.Data.SummaryItemType.Sum, "{0:N0}");
                grvColor.SetSummaryItem("OutRoll", DevExpress.Data.SummaryItemType.Sum, "{0:N0}");
                grvColor.SetSummaryItem("OutQty", DevExpress.Data.SummaryItemType.Sum, "{0:N0}");

                // 일자별 주문내역 집계
                // 그룹행을 자동 펼침으로 설정
                grvDayOrder.OptionsBehavior.AutoExpandAllGroups = true;
                // 그룹값을 그룹 Footer가 아닌 그룹행의 컬럼으로 표시하도록 설정
                grvDayOrder.OptionsBehavior.AlignGroupSummaryInGroupRow = DevExpress.Utils.DefaultBoolean.True;

                grvDayOrder.Columns["GroupingID"].GroupIndex = 0;
                //grvDayOrder.SetGroupSummaryItem("InCustom", DevExpress.Data.SummaryItemType.Custom, "합계");
                grvDayOrder.SetGroupSummaryItem("InRoll", DevExpress.Data.SummaryItemType.Sum, "{0:N0}");
                grvDayOrder.SetGroupSummaryItem("InQty", DevExpress.Data.SummaryItemType.Sum, "{0:N0}");
                grvDayOrder.SetGroupSummaryItem("PassRoll", DevExpress.Data.SummaryItemType.Sum, "{0:N0}");
                grvDayOrder.SetGroupSummaryItem("PassQty", DevExpress.Data.SummaryItemType.Sum, "{0:N0}");
                grvDayOrder.SetGroupSummaryItem("DefectRoll", DevExpress.Data.SummaryItemType.Sum, "{0:N0}");
                grvDayOrder.SetGroupSummaryItem("DefectQty", DevExpress.Data.SummaryItemType.Sum, "{0:N0}");
                grvDayOrder.SetGroupSummaryItem("OutRoll", DevExpress.Data.SummaryItemType.Sum, "{0:N0}");
                grvDayOrder.SetGroupSummaryItem("OutQty", DevExpress.Data.SummaryItemType.Sum, "{0:N0}");

                ViewCustomGrid();

                // 거래처
                DxControl.SaveGridFormat(grvCustom, GridFormat.Default);
                DxControl.LoadGridFormat(grvCustom, GridFormat.Current);

                // 수주
                DxControl.SaveGridFormat(grvOrder, GridFormat.Default);
                DxControl.LoadGridFormat(grvOrder, GridFormat.Current);

                // 색상
                DxControl.SaveGridFormat(grvColor, GridFormat.Default);
                DxControl.LoadGridFormat(grvColor, GridFormat.Current);

                // 일자별 집계
                DxControl.SaveGridFormat(grvDayOrder, GridFormat.Default);
                DxControl.LoadGridFormat(grvDayOrder, GridFormat.Current);

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
            grcOrder.DataSource = null;
            grcColor.DataSource = null;
            grcDayOrder.DataSource = null;

            ViewCustomGrid();
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
        /// 거래처 그리드 행변경
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvCustom_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle >= 0)
                ViewOrderGrid();
        }

        /// <summary>
        ///  거래처 그리드 행 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvCustom_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            ViewOrderGrid();
        }

        /// <summary>
        /// 주문 그리드 행 변경
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvOrder_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle >= 0)
                ViewColorAndDayGird();
        }

        /// <summary>
        /// 주문 그리드 행 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvOrder_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            ViewColorAndDayGird();
        }

        /// <summary>
        /// 색상별 주문내역 그리드 - 품명 셀 병합
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvColor_CellMerge(object sender, CellMergeEventArgs e)
        {
            var view = sender as BandedGridView;
            if (e.Column.FieldName == "Article")
            {
                var value1 = view.GetRowCellValue(e.RowHandle1, "Article");
                var value2 = view.GetRowCellValue(e.RowHandle2, "Article");
                e.Merge = (value1 == value2);
            }
            else
            {
                e.Column.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            }
        }

        /// <summary>
        /// 셀 병합 후 EvenRow 및 행 선택을 설정
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvColor_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            var view = sender as GridView;
            var grid = (sender as GridView).GridControl;

            // 품목 이후 컬럼이면
            if (e.Column.VisibleIndex > view.Columns["Article"].VisibleIndex)
            {
                // 짝수행 배경색
                //if (e.RowHandle % 2 == 0)
                //    e.Appearance.BackColor = DevExpress.Skins.GridSkins.GetSkin(grid.LookAndFeel)["GridOddRow"].Color.BackColor;

                // 선택행 전경/배경색
                if (e.RowHandle == view.FocusedRowHandle)
                {
                    e.Appearance.BackColor = DevExpress.Skins.CommonSkins.GetSkin(grid.LookAndFeel.ActiveLookAndFeel).Colors.GetColor("Highlight");
                    e.Appearance.ForeColor = DevExpress.Skins.CommonSkins.GetSkin(grid.LookAndFeel.ActiveLookAndFeel).Colors.GetColor("HighlightText");
                }
            }
        }

        /// <summary>
        /// 색상별 주문 그리드 - Footer 글자색 변경 및 진하게
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvColor_CustomDrawFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {
            var summaryItem = e.Info.SummaryItem;

            e.Appearance.FontStyleDelta = FontStyle.Bold;
            if (summaryItem.Tag.CString() == "Title")
                e.Appearance.ForeColor = Color.Red;
            else
                e.Appearance.ForeColor = Color.Blue;
        }

        /// <summary>
        /// 일자별 주문 그리드 - 날짜 형식 지정 및 값이 0 인 셀은 공백으로 표시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvDayOrder_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {

            try
            {
                if (e.ListSourceRowIndex >= 0)
                {
                    var view = (sender as GridView);
                    string text = string.Empty;

                    if (e.Column.FieldName == "OrderDate")
                    {
                        text = view.GetRowCellValue(e.ListSourceRowIndex, e.Column.FieldName).CString();
                        e.DisplayText = $"{text.Substring(4, 2)}-{text.Substring(6, 2)}";
                    }
                    else if (new string[] { "InRoll", "InQty", "PassRoll", "PassQty", "DefectRoll", "DefectQty", "OutRoll", "OutQty" }.Contains(e.Column.FieldName))
                    {
                        if (view.GetRowCellValue(e.ListSourceRowIndex, e.Column.FieldName).CInt() == 0)
                            e.DisplayText = "";
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// 일자별 주문 그리드 - 그룹행 텍스트 표시 및 글꼴 지정
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvDayOrder_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            var view = sender as GridView;
            var info = e.Info as GridGroupRowInfo;
            //info.GroupText = $"일자별 주문내역 : (건수 = {view.GetChildRowCount(e.RowHandle)}, 입고수량계 : {view.GetGroupRowValue(e.RowHandle, view.Columns["InQty"]).CString()})";
            info.GroupText = $"합계:(건수={view.GetChildRowCount(e.RowHandle)})";

            e.Appearance.FontStyleDelta = FontStyle.Bold;
            e.Appearance.ForeColor = Color.Red;
        }

        /// <summary>
        /// 일자별 주문 그리드 - 그룹행 셀의 글꼴 지정
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvDayOrder_CustomDrawGroupRowCell(object sender, DevExpress.XtraGrid.Views.Base.RowGroupRowCellEventArgs e)
        {
            e.Appearance.FontStyleDelta = FontStyle.Bold;
            e.Appearance.ForeColor = Color.Blue;
        }

        /// <summary>
        /// 일자별 주문 그리드 - 그룹 Footer 글자색 변경 및 진하게
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvDayOrder_CustomDrawRowFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {
            e.Appearance.FontStyleDelta = FontStyle.Bold;
            e.Appearance.ForeColor = Color.Blue;
        }

        #endregion

        #region 사용자 정의 메서드
        /// <summary>
        /// 거래처 리스트 조회
        /// </summary>
        private void ViewCustomGrid()
        {
            try
            {
                var startTime = DateTime.Now;

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetList, SqlDbType.Char);
                inParams.Add("@i_StrAcptDate", dteStrAcptDate.EditValue.CDateString("yyyyMMdd"), SqlDbType.Char);
                inParams.Add("@i_EndAcptDate", dteEndAcptDate.EditValue.CDateString("yyyyMMdd"), SqlDbType.Char);
                inParams.Add("@i_Custom", txtShortCustom.EditValue.CString(), SqlDbType.VarChar);
                inParams.Add("@i_Article", txtArticle.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_OrderNo", txtOrderNo.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_CloseClss", gleCloseClss.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_UnitCostClss", gleUnitCostClss.EditValue.CString(), SqlDbType.Char);

                var table = SqlHelper.GetDataTable("xp_Order", inParams, out _, out _);

                grcCustom.SetData(table);

                SqlHelper.WriteLog(this, ActionLogType.Select, startTime);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
            finally
            {
                Utils.CloseWaitForm();
            }
        }

        /// <summary>
        /// 수주 리스트 조회
        /// </summary>
        private void ViewOrderGrid()
        {
            try
            {
                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", "81", SqlDbType.Char);
                inParams.Add("@i_CustomID", grvCustom.GetFocusedRowCellValue("CustomID").CString(), SqlDbType.Char);
                inParams.Add("@i_StrAcptDate", dteStrAcptDate.EditValue.CDateString("yyyyMMdd"), SqlDbType.Char);
                inParams.Add("@i_EndAcptDate", dteEndAcptDate.EditValue.CDateString("yyyyMMdd"), SqlDbType.Char);
                inParams.Add("@i_Custom", txtShortCustom.EditValue.CString(), SqlDbType.VarChar);
                inParams.Add("@i_Article", txtArticle.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_OrderNo", txtOrderNo.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_CloseClss", gleCloseClss.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_UnitCostClss", gleUnitCostClss.EditValue.CString(), SqlDbType.Char);

                var table = SqlHelper.GetDataTable("xp_Order", inParams, out _, out _);

                grcOrder.SetData(table);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.ToString());
            }
            finally
            {
                Utils.CloseWaitForm();
            }
        }

        /// <summary>
        /// 색상별 / 일자별 오더
        /// </summary>
        private void ViewColorAndDayGird()
        {
            try
            {
                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", "82", SqlDbType.Char);
                inParams.Add("@i_OrderID", grvOrder.GetFocusedRowCellValue("OrderID").CString(), SqlDbType.Char);

                var dataSet = SqlHelper.GetDataSet("xp_Order", inParams, out _, out _);

                grcColor.SetData(dataSet.Tables[0]);
                grcDayOrder.SetData(dataSet.Tables[1]);

                if (grvDayOrder.RowCount > 0)
                    grvDayOrder.FocusedRowHandle = 0;
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.ToString());
            }
            finally
            {
                Utils.CloseWaitForm();
            }
        }

        #endregion
    }
}