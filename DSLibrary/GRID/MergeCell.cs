using DevExpress.XtraGrid.Columns;

/// <summary>
/// 그리드 뷰 가로 병합 관련 클래스
/// Download : ICODEBROKER(https://icodebroker.tistory.com/7024)
/// </summary>
namespace DSLibrary
{
    /// <summary>
    /// 병합 셀
    /// </summary>
    public class MergeCell
    {
        #region Field

        /// <summary>
        /// 행 핸들
        /// </summary>
        private int rowHandle;

        /// <summary>
        /// 그리그 컬럼 1
        /// </summary>
        private GridColumn gridColumn1;

        /// <summary>
        /// 그리드 컬럼 2
        /// </summary>
        private GridColumn gridColumn2;

        #endregion

        #region 행 핸들 - RowHandle

        /// <summary>
        /// 행 핸들
        /// </summary>
        public int RowHandle
        {
            get { return this.rowHandle; }
            set { this.rowHandle = value; }
        }

        #endregion

        #region 그리드 컬럼 1 - GridColumn1

        /// <summary>
        /// 그리드 컬럼 1
        /// </summary>
        public GridColumn GridColumn1
        {
            get { return this.gridColumn1; }
            set { this.gridColumn1 = value; }
        }

        #endregion

        #region 그리드 컬럼 2 - GridColumn2

        /// <summary>
        /// 그리드 컬럼 2
        /// </summary>
        public GridColumn GridColumn2
        {
            get { return this.gridColumn2; }
            set { this.gridColumn2 = value; }
        }

        #endregion

        #region 생성자 - MergeCell(rowHandle, gridColumn1, gridColumn2)

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="rowHandle">행 핸들</param>
        /// <param name="gridColumn1">그리드 컬럼 1</param>
        /// <param name="gridColumn2">그리드 컬럼 2</param>
        public MergeCell(int rowHandle, GridColumn gridColumn1, GridColumn gridColumn2)
        {
            this.rowHandle = rowHandle;
            this.gridColumn1 = gridColumn1;
            this.gridColumn2 = gridColumn2;
        }

        #endregion
    }
}