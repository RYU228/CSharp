using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;


namespace DSLibrary
{
    public partial class frmFindValue : frmBase
    {
        public delegate void FindValueEventHandler(DataRow row);
        public event FindValueEventHandler FindValueEvent;

        #region 전역변수
        private readonly string formText;
        private readonly DataTable sourceData;
        private readonly string hiddenColumns;
        private DataRow selectedRow = null;
        public int rowCount { get; }
        #endregion

        #region 생성자
        public frmFindValue()
        {
            InitializeComponent();
        }

        public frmFindValue(string formText, DataTable sourceData, string hiddenColumns = "UseClss") : this()
        {
            this.formText = formText;
            this.sourceData = sourceData;
            this.hiddenColumns = hiddenColumns;

            this.Text = this.formText;
            gdcFind.SetColumns(this.sourceData, this.hiddenColumns, false, true, true, true);
            rowCount = gdvFind.RowCount;

            gdvFind.DoubleClick += GdvFind_DoubleClick;
            gdvFind.KeyUp += GdvFind_KeyUp;
            btnOk.Click += BtnOk_Click;
            btnCancel.Click += BtnCancel_Click;
        }

        #endregion

        #region 오버라이드 이벤트
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            //this.Text = this.formText;
            //gdcFind.SetColumns(this.sourceData, this.hiddenColumns, false, true, true, true);

            //gdvFind.DoubleClick += GdvFind_DoubleClick;
            //gdvFind.KeyUp += GdvFind_KeyUp;
            //btnOk.Click += BtnOk_Click;
            //btnCancel.Click += BtnCancel_Click;
        }

        #endregion

        #region 폼 컨트롤 이벤트
        /// <summary>
        /// 그리드 더블 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GdvFind_DoubleClick(object sender, EventArgs e)
        {
            var point = (sender as GridView).GridControl.PointToClient(MousePosition);
            var hitInfo = (sender as GridView).CalcHitInfo(point);

            if (hitInfo.HitTest != GridHitTest.Column && gdvFind.FocusedRowHandle >= 0)
            {
                //this.selectedRow = gdvFind.GetDataRow(gdvFind.FocusedRowHandle);
                //this.DialogResult = DialogResult.OK;

                //if (FindValueEvent != null)
                //    FindValueEvent(this.selectedRow);
                //else
                //    MsgBox.ErrorMessage("Event Error!");

                SelectData();
            }
        }

        /// <summary>
        /// Eneter Key 입력
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GdvFind_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //this.selectedRow = gdvFind.GetDataRow(gdvFind.FocusedRowHandle);
                //this.DialogResult = DialogResult.OK;

                //if (FindValueEvent != null)
                //    FindValueEvent(this.selectedRow);
                //else
                //    MsgBox.ErrorMessage("Event Error!");

                SelectData();
            }
        }

        /// <summary>
        /// 확인 버튼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnOk_Click(object sender, EventArgs e)
        {
            //this.selectedRow = gdvFind.GetDataRow(gdvFind.FocusedRowHandle);
            //this.DialogResult = DialogResult.OK;

            //if (FindValueEvent != null)
            //    FindValueEvent(this.selectedRow);
            //else
            //    MsgBox.ErrorMessage("Event Error!");

            SelectData();
        }

        /// <summary>
        /// 취소 버튼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            //this.selectedRow = null;
            //this.DialogResult = DialogResult.OK;

            //if (FindValueEvent != null)
            //    FindValueEvent(this.selectedRow);
            //else
            //    MsgBox.ErrorMessage("Event Error!");

            SelectData(false);
        }

        #endregion

        #region 사용자 함수
        /// <summary>
        /// FindValue 선택 및 취소 시 로직
        /// </summary>
        /// <param name="isSelect"></param>
        public void SelectData(bool isSelect = true)
        {
            if (isSelect == true)
            {
                this.selectedRow = gdvFind.GetDataRow(gdvFind.FocusedRowHandle);
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                this.selectedRow = null;
                this.DialogResult = DialogResult.Cancel;
            }

            // this.DialogResult = DialogResult.OK;

            if (FindValueEvent != null)
                FindValueEvent(this.selectedRow);
            else
                MsgBox.ErrorMessage("Event Error!");
        }
        #endregion
    }
}