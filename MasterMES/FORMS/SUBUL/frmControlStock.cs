using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DSLibrary;

namespace MasterMES.FORMS.SUBUL
{
    public partial class frmControlStock : frmBase, CLASSES.IButtonAction
    {
        public frmControlStock()
        {
            InitializeComponent();

            grvStock.OptionsCustomization.AllowSort = false;
            dtpStockYear.SetViewStyle(ViewStyle.YearView);
        }

        #region 오버라이드 이벤트
        //protected override void OnActivated(EventArgs e)
        //{
        //    base.OnActivated(e);
        //    MainButton.ActiveButton("INIT,NEW,SAVE,DELETE");
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

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Initial, SqlDbType.Char);

                var initSet = SqlHelper.GetDataSet("xp_ControlStock", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                grcStock.SetColumns(initSet.Tables[0], "", false, true, true);

                // 거래처, 품목
                var riCust = grvStock.SetRepositoryItemSearchLookUpEdit("CustName", initSet.Tables[1], "CustCode", "CustName");
                //var riItem = grvStock.SetRepositoryItemSearchLookUpEdit("ItemName", initSet.Tables[2], "ItemCode", "ItemName");

                var riItem = grvStock.SetRepositoryItemButtonEdit("ItemName");
                riItem.ButtonClick += RiItem_ButtonClick;

                // StockRoll
                var riStockRoll = grvStock.SetRepositoryItemSpinEdit("StockRoll"); //, "N0");
                riStockRoll.EditValueChanged += RiStockRoll_EditValueChanged;

                // StockQty
                var riStockQty = grvStock.SetRepositoryItemSpinEdit("StockQty"); //, "N2");
                riStockQty.EditValueChanged += RiStockQty_EditValueChanged;

                // Remark
                var riRemark = grvStock.SetRepositoryItemTextEdit("Remark");
                riRemark.EditValueChanged += RiRemark_EditValueChanged; ;
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
            MsgBox.OKMessage(grvStock.GetRowCellValue(grvStock.FocusedRowHandle, "Remark").CString());
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
            if (grvStock.RowCount <= 0)
            {
                MsgBox.OKMessage("데이터가 존재하지 않습니다");
                return;
            }

            memXml.EditValue = (grcStock.DataSource as DataTable).DefaultView.ToTable().Select("ChgYoN = 'Y'").CopyToDataTable()
                                .DefaultView.ToTable(false, new string[] { "CustName", "ItemName", "StockRoll", "StockQty", "Remark" })
                                .ToXml();


            memXml.EditValue = (grcStock.DataSource as DataTable).DefaultView
                               .ToTable(false, new string[] { "CustName", "ItemName", "StockRoll", "StockQty", "Remark", "ChgYoN" })
                               .Select("ChgYoN = 'Y'")
                               .CopyToDataTable().ToXml();
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

        private void RiItem_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (e.Button.Tag.CString() == "FIND")
                MsgBox.Show((sender as ButtonEdit).EditValue.CString());
        }


        private void RiStockRoll_EditValueChanged(object sender, EventArgs e)
        {
            grvStock.SetFocusedRowCellValue("StockRoll", (sender as SpinEdit).EditValue);

            if (grvStock.PostEditor())
                grvStock.UpdateCurrentRow();
        }

        private void RiStockQty_EditValueChanged(object sender, EventArgs e)
        {
            grvStock.SetFocusedRowCellValue("StockQty", (sender as SpinEdit).EditValue);

            if (grvStock.PostEditor())
                grvStock.UpdateCurrentRow();
        }


        private void RiRemark_EditValueChanged(object sender, EventArgs e)
        {
            if (grvStock.PostEditor())
                grvStock.UpdateCurrentRow();
        }

        private void grvStock_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            GridView gridView = sender as GridView;
            if (e.Column.FieldName != "ChgYoN")
                gridView.SetRowCellValue(e.RowHandle, "ChgYoN", "Y");
        }

        private void grvStock_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                (sender as GridView).MoveCellFocus(); // "CustName,ItemName,StockRoll,StockQty,Remark");            
        }

        private void grvStock_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            if (e.RowHandle >= 0)
            {
                if (new string[] { "CustName", "ItemName", "StockRoll", "StockQty", "Remark" }.Contains(e.Column.FieldName))
                {
                    e.Appearance.ForeColor = Color.White;
                    e.Appearance.BackColor = UserColors.EditableColor;
                }
            }
        }

        #endregion

        #region 사용자 정의 메서드

        #endregion

        private void btnAddRow_Click(object sender, EventArgs e)
        {
            grvStock.AddNewRow();
            grvStock.BestFitColumns();
            grvStock.UpdateCurrentRow();

            grvStock.SetFocusedRowCellValue("StockQty", 0);
        }

        private void btnDelRow_Click(object sender, EventArgs e)
        {
            grvStock.DeleteRow(grvStock.FocusedRowHandle);
        }
    }
}