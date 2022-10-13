using System;
using System.Drawing;
using DevExpress.Utils.Drawing;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

/// <summary>
/// 그리드 뷰 가로 병합 관련 클래스
/// Download : ICODEBROKER(https://icodebroker.tistory.com/7024)
/// </summary>
namespace DSLibrary
{
    /// <summary>
    /// 병합 셀 그리드 페인터
    /// </summary>
    public class MergeCellGridPainter : GridPainter
    {
        #region Field

        /// <summary>
        /// 커스텀 페인트 여부
        /// </summary>
        private bool isCustomPainting;

        #endregion

        #region 커스텀 페인팅 여부 - IsCustomPainting

        /// <summary>
        /// 커스텀 페인팅 여부
        /// </summary>
        public bool IsCustomPainting
        {
            get { return this.isCustomPainting; }
            set { this.isCustomPainting = value; }
        }

        #endregion

        #region 생성자 - MergeCellGridPainter(gridView)

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="gridView">그리드 뷰</param>
        public MergeCellGridPainter(GridView gridView) : base(gridView)
        {
        }

        #endregion

        #region 병합 셀 그리기 - DrawMergeCell(mergeCell, e)

        /// <summary>
        /// 병합 셀 그리기
        /// </summary>
        /// <param name="mergeCell">병합 셀</param>
        /// <param name="e">이벤트 인자</param>
        public void DrawMergeCell(MergeCell mergeCell, DXPaintEventArgs e)
        {
            int delta = mergeCell.GridColumn1.VisibleIndex - mergeCell.GridColumn2.VisibleIndex;

            if (Math.Abs(delta) > 1)
                return;

            GridViewInfo gridViewInfo = View.GetViewInfo() as GridViewInfo;

            GridCellInfo gridCellInfo1 = gridViewInfo.GetGridCellInfo(mergeCell.RowHandle, mergeCell.GridColumn1);
            GridCellInfo gridCellInfo2 = gridViewInfo.GetGridCellInfo(mergeCell.RowHandle, mergeCell.GridColumn2);

            if (gridCellInfo1 == null || gridCellInfo2 == null)
                return;

            // 추가 : 마우스 클릭시 에러 발생 (ViewInfo = null)
            if (gridCellInfo1.ViewInfo == null)
                return;

            Rectangle targetRectangle = Rectangle.Union(gridCellInfo1.Bounds, gridCellInfo2.Bounds);

            gridCellInfo1.Bounds = targetRectangle;
            gridCellInfo1.CellValueRect = targetRectangle;

            gridCellInfo2.Bounds = targetRectangle;
            gridCellInfo2.CellValueRect = targetRectangle;

            if (delta < 0)
                gridCellInfo1 = gridCellInfo2;

            Rectangle boundRectangle = gridCellInfo1.ViewInfo.Bounds;

            boundRectangle.Width = targetRectangle.Width;
            boundRectangle.Height = targetRectangle.Height;

            gridCellInfo1.ViewInfo.Bounds = boundRectangle;

            GraphicsCache graphicsCache = new GraphicsCache(e);

            gridCellInfo1.ViewInfo.CalcViewInfo(e.Graphics);

            this.isCustomPainting = true;

            gridCellInfo1.Appearance.FillRectangle(graphicsCache, gridCellInfo1.Bounds);

            DrawRowCell(new GridViewDrawArgs(graphicsCache, gridViewInfo, gridViewInfo.ViewRects.Bounds), gridCellInfo1);

            this.isCustomPainting = false;
        }

        #endregion
    }
}