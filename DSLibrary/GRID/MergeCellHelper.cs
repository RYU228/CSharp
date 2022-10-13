using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.Utils.Drawing;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

/// <summary>
/// 그리드 뷰 가로 병합 관련 클래스
/// Download : ICODEBROKER(https://icodebroker.tistory.com/7024)
/// </summary>
namespace DSLibrary
{
    /// <summary>
    /// 병합 셀 헬퍼
    /// </summary>
    public class MergeCellHelper
    {
        #region Field

        /// <summary>
        /// 그리드 뷰
        /// </summary>
        private GridView gridView;

        /// <summary>
        /// 병합 셀 리스트
        /// </summary>
        private List<MergeCell> mergeCellList = new List<MergeCell>();

        /// <summary>
        /// 병합 셀 그리드 페인터
        /// </summary>
        private MergeCellGridPainter mergeCellGridPainter;

        #endregion

        #region 병합 셀 리스트 - MergeCellList

        /// <summary>
        /// 병합 셀 리스트
        /// </summary>
        public List<MergeCell> MergeCellList
        {
            get { return this.mergeCellList; }
        }

        #endregion

        #region 생성자 - MergeCellHelper(gridView)

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="gridView">그리드 뷰</param>
        public MergeCellHelper(GridView gridView)
        {
            this.gridView = gridView;

            this.mergeCellGridPainter = new MergeCellGridPainter(this.gridView);

            this.gridView.GridControl.Paint += gridControl_Paint;
            this.gridView.CustomDrawCell += gridView_CustomDrawCell;
            this.gridView.CellValueChanged += gridView_CellValueChanged;
        }

        #endregion

        #region 병합 셀 추가하기 - AddMergeCell(rowHandle, gridColumn1, gridColumn2)

        /// <summary>
        /// 병합 셀 추가하기
        /// </summary>
        /// <param name="rowHandle">행 핸들</param>
        /// <param name="gridColumn1">그리드 컬럼 1</param>
        /// <param name="gridColumn2">그리드 컬럼 2</param>
        /// <returns>병합 셀</returns>
        public MergeCell AddMergeCell(int rowHandle, GridColumn gridColumn1, GridColumn gridColumn2)
        {
            MergeCell mergeCell = new MergeCell(rowHandle, gridColumn1, gridColumn2);
            this.mergeCellList.Add(mergeCell);

            return mergeCell;
        }

        #endregion

        #region 병합 셀 추가하기 - AddMergeCell(rowHandle, columnIndex1, columnIndex2, value)

        /// <summary>
        /// 병합 셀 추가하기
        /// </summary>
        /// <param name="rowHandle">행 핸들</param>
        /// <param name="columnIndex1">컬럼 인덱스 1</param>
        /// <param name="columnIndex2">컬럼 인덱스 2</param>
        /// <param name="value">값</param>
        public void AddMergeCell(int rowHandle, int columnIndex1, int columnIndex2, object value)
        {
            MergeCell mergeCell = AddMergeCell(rowHandle, this.gridView.Columns[columnIndex1], this.gridView.Columns[columnIndex2]);
            SetCellValue(mergeCell, value);
        }

        #endregion

        #region 병합 셀 추가하기 - AddMergeCell(rowHandle, gridColumn1, gridColumn2, value)

        /// <summary>
        /// 병합 셀 추가하기
        /// </summary>
        /// <param name="rowHandle">행 핸들</param>
        /// <param name="gridColumn1">그리드 컬럼 1</param>
        /// <param name="gridColumn2">그리드 컬럼 2</param>
        /// <param name="value">값</param>
        public void AddMergeCell(int rowHandle, GridColumn gridColumn1, GridColumn gridColumn2, object value)
        {
            MergeCell mergeCell = AddMergeCell(rowHandle, gridColumn1, gridColumn2);
            SetCellValue(mergeCell, value);
        }

        #endregion

        #region 셀 값 설정하기 - SetCellValue(rowHandle, gridColumn, value)

        /// <summary>
        /// 셀 값 설정하기
        /// </summary>
        /// <param name="rowHandle">행 핸들</param>
        /// <param name="gridColumn">그리드 컬럼</param>
        /// <param name="value">값</param>
        public void SetCellValue(int rowHandle, GridColumn gridColumn, object value)
        {
            if (this.gridView.GetRowCellValue(rowHandle, gridColumn) != value)
                this.gridView.SetRowCellValue(rowHandle, gridColumn, value);
        }

        #endregion

        #region 셀 값 설정하기 - SetCellValue(mergeCell, value)

        /// <summary>
        /// 셀 값 설정하기
        /// </summary>
        /// <param name="mergeCell">병합 셀</param>
        /// <param name="value">값</param>
        public void SetCellValue(MergeCell mergeCell, object value)
        {
            if (mergeCell != null)
            {
                SetCellValue(mergeCell.RowHandle, mergeCell.GridColumn1, value);
                SetCellValue(mergeCell.RowHandle, mergeCell.GridColumn2, value);
            }
        }

        #endregion

        #region 그리드 컨트롤 페인트시 처리하기 - gridControl_Paint(sender, e)

        /// <summary>
        /// 그리드 컨트롤 페인트시 처리하기
        /// </summary>
        /// <param name="sender">이벤트 발생자</param>
        /// <param name="e">이벤트 인자</param>
        private void gridControl_Paint(object sender, PaintEventArgs e)
        {
            DrawMergeCell(new DXPaintEventArgs(e));
        }

        #endregion

        #region 그리드 뷰 셀 커스텀 그리기 - gridView_CustomDrawCell(sender, e)

        /// <summary>
        /// 그리드 뷰 셀 커스텀 그리기
        /// </summary>
        /// <param name="sender">이벤트 발생자</param>
        /// <param name="e">이벤트 인자</param>
        private void gridView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            if (IsMergeCell(e.RowHandle, e.Column))
                e.Handled = !this.mergeCellGridPainter.IsCustomPainting;
        }

        #endregion

        #region 그리드 뷰 셀 값 변경시 처리하기 - gridView_CellValueChanged(sender, e)

        /// <summary>
        /// 그리드 뷰 셀 값 변경시 처리하기
        /// </summary>
        /// <param name="sender">이벤트 발생자</param>
        /// <param name="e">이벤트 인자</param>
        private void gridView_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            SetCellValue(GetMergeCell(e.RowHandle, e.Column), e.Value);
        }

        #endregion

        #region 병합 셀 그리기 - DrawMergeCell(e)

        /// <summary>
        /// 병합 셀 그리기
        /// </summary>
        /// <param name="e">이벤트 인자</param>
        private void DrawMergeCell(DXPaintEventArgs e)
        {
            foreach (MergeCell mergeCell in this.mergeCellList)
                this.mergeCellGridPainter.DrawMergeCell(mergeCell, e);
        }

        #endregion

        #region 병합 셀 구하기 - GetMergeCell(rowHandle, gridColumn)

        /// <summary>
        /// 병합 셀 구하기
        /// </summary>
        /// <param name="rowHandle">행 번호</param>
        /// <param name="gridColumn">그리드 컬럼</param>
        /// <returns>병합 셀</returns>
        private MergeCell GetMergeCell(int rowHandle, GridColumn gridColumn)
        {
            foreach (MergeCell mergeCell in this.mergeCellList)
            {
                if (mergeCell.RowHandle == rowHandle && (gridColumn == mergeCell.GridColumn1 || gridColumn == mergeCell.GridColumn2))
                    return mergeCell;
            }

            return null;
        }

        #endregion

        #region 병합 셀 여부 구하기 - IsMergeCell(rowHandle, gridColumn)

        /// <summary>
        /// 병합 셀 여부 구하기
        /// </summary>
        /// <param name="rowHandle">행 번호</param>
        /// <param name="gridColumn">그리드 컬럼</param>
        /// <returns>병합 셀 여부</returns>
        private bool IsMergeCell(int rowHandle, GridColumn gridColumn)
        {
            return GetMergeCell(rowHandle, gridColumn) != null;
        }

        #endregion
    }
}