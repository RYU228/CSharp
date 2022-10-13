using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DSLibrary;

namespace MasterMES.FORMS.BUSINESS
{
    public partial class frmQuotation : frmBase
    {
        public delegate void ViewQuotationEventHandler();
        public ViewQuotationEventHandler ChangeQuotation;

        private string receiptID;
        private string quotID;
        private string kCustom;

        #region 생성자
        public frmQuotation()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자 추가
        /// </summary>
        /// <param name="receiptID"></param>
        /// <param name="quotID"></param>
        /// <param name="kCustom"></param>
        public frmQuotation(string receiptID, string quotID, string kCustom) : this()
        {
            this.receiptID = receiptID;
            this.quotID = quotID;
            this.kCustom = kCustom;
        }

        #endregion

        #region 오버라이드 이벤트
        /// <summary>
        /// 폼이 보여질 때
        /// </summary>
        /// <param name="e"></param>
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            txtReceiptID.EditValue = this.receiptID;
            txtKCustom.EditValue = this.kCustom;
            txtQuotID.EditValue = this.quotID;

            this.InitQuotation();
        }

        /// <summary>
        /// 폼이 닫길때 - 이벤트 호출
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (ChangeQuotation != null)
                ChangeQuotation();
            else
                MsgBox.ErrorMessage("Event Error!");
        }

        #endregion

        #region 인터페이스 - 공용버튼 클릭
        /// <summary>
        /// 초기화 버튼
        /// </summary>
        public void InitQuotation()
        {
            try
            {
                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Initial, SqlDbType.Char);

                var initSet = SqlHelper.GetDataSet("xp_Quotation", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                grcQuotItem.SetColumns(initSet.Tables[0], "QuotSeq");
                grvQuotItem.FixedColumnWidth(new Dictionary<string, int>()
                {
                    { "ItemText", 250 },
                    { "ItemQty", 100 },
                    { "ItemPrice", 100 },
                    { "ItemAmount", 100 },
                    { "Remark", 250 },
                });

                // 항목
                var riItemText = grvQuotItem.SetRepositoryItemTextEdit("ItemText");
                riItemText.EditValueChanged += RiItemText_EditValueChanged;

                // 수량
                var riItemQty = grvQuotItem.SetRepositoryItemSpinEdit("ItemQty");
                riItemQty.EditValueChanged += RiItemQty_EditValueChanged;

                // 단가
                var riItemPrice = grvQuotItem.SetRepositoryItemSpinEdit("ItemPrice");
                riItemPrice.EditValueChanged += RiItemPrice_EditValueChanged;

                // 비고
                var riRemark = grvQuotItem.SetRepositoryItemTextEdit("Remark");
                riRemark.EditValueChanged += RiRemark_EditValueChanged;

                // 견적서 및 견적서 상세내역 조회
                this.ShowDataQuotation();

                // 그리드 초기 형식 저장
                DxControl.SaveGridFormat(grvQuotItem, GridFormat.Default);
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
        /// 신규 버튼
        /// </summary>
        public void NewQuotation()
        {
            dteQuotDate.SetDefaultDate();
            txtChargeName.EditValue = null;
            spePaymentDays.EditValue = 0;
            speValidityDays.EditValue = 0;
            speDepositRate.EditValue = 0;
            speMiddleRate.EditValue = 0;
            speFinalRate.EditValue = 0;
            txtPaymentTerms.EditValue = null;
            speManCnt.EditValue = 0;
            speTakeDays.EditValue = 0;
            speWageRate.EditValue = 0;
            speExpense.EditValue = 0;
            txtLaborRemark.EditValue = null;
            txtExpenseRemark.EditValue = null;
            txtQuotRemark.EditValue = null;
            mmeSpecialNote.EditValue = null;
        }

        #endregion

        #region 폼 컨트롤 이벤트
        /// <summary>
        /// 엔터키 입력시 다음 입력 컬럼으로 이동
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvQuotItem_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                (sender as GridView).MoveCellFocus();
        }

        /// <summary>
        /// 입력 컬럼 색상 변경
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvQuotItem_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            if (e.RowHandle >= 0)
            {
                if (new string[] { "ItemText", "ItemQty", "ItemPrice", "Remark" }.Contains(e.Column.FieldName))
                {
                    e.Appearance.ForeColor = Color.White;
                    e.Appearance.BackColor = UserColors.EditableColor;
                }
            }
        }

        /// <summary>
        /// 그리드 행 추가
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddRow_Click(object sender, EventArgs e)
        {
            grvQuotItem.AddNewRow();

            if (grvQuotItem.PostEditor())
                grvQuotItem.UpdateCurrentRow();

            grvQuotItem.Focus();
            grvQuotItem.FocusedColumn = grvQuotItem.Columns["ItemText"];
        }

        /// <summary>
        /// 그리드 행 삭제 : 행만 삭제 - 삭제후 저장해야 변경사항 저장됨
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelRow_Click(object sender, EventArgs e)
        {
            grvQuotItem.DeleteRow(grvQuotItem.FocusedRowHandle);
        }

        /// <summary>
        /// 신규 추가 행에 대한 컬럼값 초기화
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvQuotItem_InitNewRow(object sender, InitNewRowEventArgs e)
        {
            var view = sender as GridView;

            view.SetRowCellValue(e.RowHandle, "ItemText", null);
            view.SetRowCellValue(e.RowHandle, "ItemQty", 0);
            view.SetRowCellValue(e.RowHandle, "ItemPrice", 0);
            view.SetRowCellValue(e.RowHandle, "ItemAmount", 0);
            view.SetRowCellValue(e.RowHandle, "Remark", null);
        }

        /// <summary>
        /// 자재명
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RiItemText_EditValueChanged(object sender, EventArgs e)
        {
            grvQuotItem.SetFocusedRowCellValue("ItemText", (sender as TextEdit).EditValue);

            if (grvQuotItem.PostEditor())
                grvQuotItem.UpdateCurrentRow();
        }

        /// <summary>
        /// 단가 입력
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RiItemPrice_EditValueChanged(object sender, EventArgs e)
        {
            grvQuotItem.SetRowCellValue(grvQuotItem.FocusedRowHandle, "ItemPrice", (sender as SpinEdit).EditValue);

            if (grvQuotItem.PostEditor())
                grvQuotItem.UpdateCurrentRow();

            double itemAmount = grvQuotItem.GetRowCellValue(grvQuotItem.FocusedRowHandle, "ItemQty").CDouble() *
                                    grvQuotItem.GetRowCellValue(grvQuotItem.FocusedRowHandle, "ItemPrice").CDouble();

            grvQuotItem.SetRowCellValue(grvQuotItem.FocusedRowHandle, "ItemAmount", itemAmount);
        }

        /// <summary>
        /// 수량 입력
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RiItemQty_EditValueChanged(object sender, EventArgs e)
        {
            grvQuotItem.SetRowCellValue(grvQuotItem.FocusedRowHandle, "ItemQty", (sender as SpinEdit).EditValue);

            if (grvQuotItem.PostEditor())
                grvQuotItem.UpdateCurrentRow();

            double itemAmount = grvQuotItem.GetRowCellValue(grvQuotItem.FocusedRowHandle, "ItemQty").CDouble() *
                                    grvQuotItem.GetRowCellValue(grvQuotItem.FocusedRowHandle, "ItemPrice").CDouble();

            grvQuotItem.SetRowCellValue(grvQuotItem.FocusedRowHandle, "ItemAmount", itemAmount);
        }

        /// <summary>
        /// 비고
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RiRemark_EditValueChanged(object sender, EventArgs e)
        {
            grvQuotItem.SetFocusedRowCellValue("Remark", (sender as TextEdit).EditValue);

            if (grvQuotItem.PostEditor())
                grvQuotItem.UpdateCurrentRow();
        }

        /// <summary>
        /// 견적번호 항목 삭제 : 견적 개정시 삭제후 수정 -> 저장
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQutoIDDelete_Click(object sender, EventArgs e)
        {
            txtQuotID.EditValue = null;
        }

        /// <summary>
        /// 저장 버튼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            this.SaveQuotation();
        }

        /// <summary>
        /// 삭제 버튼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            this.DeleteQuotation();
        }

        /// <summary>
        /// 출력 버튼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, EventArgs e)
        {
            this.PrintQuotation();
        }

        /// <summary>
        /// 닫기 버튼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region 사용자 정의 메소드
        /// <summary>
        /// 견적 정보 조회
        /// </summary>
        private void ShowDataQuotation()
        {
            try
            {
                this.NewQuotation();

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetRow, SqlDbType.Char);
                inParams.Add("@i_ReceiptID", txtReceiptID.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_QuotID", txtQuotID.EditValue.CString(), SqlDbType.Char);

                var retSet = SqlHelper.GetDataSet("xp_Quotation", inParams, out _, out _);

                if (retSet.Tables[0] != null && retSet.Tables[0].Rows.Count > 0)
                {
                    var row = retSet.Tables[0].Rows[0];

                    dteQuotDate.EditValue = row["QuotDate"];
                    txtChargeName.EditValue = row["ChargeName"];
                    txtQuotTitle.EditValue = row["QuotTitle"];
                    spePaymentDays.EditValue = row["PaymentDays"];
                    speValidityDays.EditValue = row["ValidityDays"];
                    speDepositRate.EditValue = row["DepositRate"];
                    speMiddleRate.EditValue = row["MiddleRate"];
                    speFinalRate.EditValue = row["FinalRate"];
                    txtPaymentTerms.EditValue = row["PaymentTerms"];
                    speManCnt.EditValue = row["ManCnt"];
                    speTakeDays.EditValue = row["TakeDays"];
                    speWageRate.EditValue = row["WageRate"];
                    speExpense.EditValue = row["Expense"];
                    txtLaborRemark.EditValue = row["LaborRemark"];
                    txtExpenseRemark.EditValue = row["ExpenseRemark"];
                    txtQuotRemark.EditValue = row["QuotRemark"];
                    mmeSpecialNote.EditValue = row["SpecialNote"];
                }

                grcQuotItem.SetData(retSet.Tables[1]);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 견적서 저장
        /// </summary>
        private void SaveQuotation()
        {
            try
            {
                var field = new Dictionary<string, bool>()
                {
                    { "ReceiptID", txtReceiptID.EditValue.CString() == "" },
                    { "ChargeName", txtChargeName.EditValue.CString() == "" },
                    { "QuotTitle", txtQuotTitle.EditValue.CString() == "" },
                    { "QuotItem Info", grvQuotItem.RowCount <= 0 },
                };
                if (Utils.CheckField(field))
                    return;

                var processId = ProcessType.Insert;
                var message = MessageText.OKInsert;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Exist, SqlDbType.Char);
                inParams.Add("@i_ReceiptID", txtReceiptID.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_QuotID", txtQuotID.EditValue.CString(), SqlDbType.Char);

                if (SqlHelper.Exist("xp_Quotation", inParams, out _, out _))
                {
                    if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionUpdate))
                        return;

                    processId = ProcessType.Update;
                    message = MessageText.OKUpdate;
                }

                Utils.ShowWaitForm(this);
                btnSave.Enabled = false;

                // 견적 내역의 순번을 생성 - 입력된 순서대로 번호를 부여하기 위해서 생성 -> DataTable -> XML로 전달
                for (int i = 0; i < grvQuotItem.RowCount; i++)
                    grvQuotItem.SetRowCellValue(i, "QuotSeq", i + 1);

                var xmlData = (grcQuotItem.DataSource as DataTable).DefaultView
                              .ToTable(false, new string[] { "QuotSeq", "ItemText", "ItemQty", "ItemPrice", "Remark" })
                              .ToXml();

                inParams.Clear();
                inParams.Add("@i_ProcessID", processId, SqlDbType.Char);
                inParams.Add("@i_ReceiptID", txtReceiptID.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_QuotID", txtQuotID.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_QuotDate", dteQuotDate.EditValue.CDateString(), SqlDbType.Date);
                inParams.Add("@i_ChargeName", txtChargeName.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_QuotTitle", txtQuotTitle.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_PaymentDays", spePaymentDays.EditValue.CShort(), SqlDbType.SmallInt);
                inParams.Add("@i_ValidityDays", speValidityDays.EditValue.CShort(), SqlDbType.SmallInt);
                inParams.Add("@i_DepositRate", speDepositRate.EditValue.CDouble(), SqlDbType.Decimal);
                inParams.Add("@i_MiddleRate", speMiddleRate.EditValue.CDouble(), SqlDbType.Decimal);
                inParams.Add("@i_FinalRate", speFinalRate.EditValue.CDouble(), SqlDbType.Decimal);
                inParams.Add("@i_PaymentTerms", txtPaymentTerms.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_ManCnt", speManCnt.EditValue.CShort(), SqlDbType.SmallInt);
                inParams.Add("@i_TakeDays", speTakeDays.EditValue.CDouble(), SqlDbType.Decimal);
                inParams.Add("@i_WageRate", speWageRate.EditValue.CInt(), SqlDbType.Int);
                inParams.Add("@i_Expense", speExpense.EditValue.CDouble(), SqlDbType.Decimal);
                inParams.Add("@i_LaborRemark", txtLaborRemark.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_ExpenseRemark", txtExpenseRemark.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_QuotRemark", txtQuotRemark.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_SpecialNote", mmeSpecialNote.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_XmlData", xmlData, SqlDbType.VarChar);
                inParams.Add("@i_InptUser", AppConfig.Login.PersonID, SqlDbType.VarChar);

                SqlHelper.Execute("xp_Quotation", inParams, out string[] retVal, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                txtQuotID.EditValue = retVal[0];

                this.ShowDataQuotation();

                MsgBox.OKMessage(message);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
            finally
            {
                btnSave.Enabled = true;
                Utils.CloseWaitForm();
            }
        }

        /// <summary>
        /// 견적서 삭제
        /// </summary>
        private void DeleteQuotation()
        {
            try
            {
                var field = new Dictionary<string, bool>()
                {
                    { "ReceiptID", txtReceiptID.EditValue.CString() == "" },
                    { "QuotID", txtQuotID.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionDelete))
                    return;

                Utils.ShowWaitForm(this);
                btnDelete.Enabled = false;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Delete, SqlDbType.Char);
                inParams.Add("@i_ReceiptID", txtReceiptID.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_QuotID", txtQuotID.EditValue.CString(), SqlDbType.Char);

                SqlHelper.Execute("xp_Quotation", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                NewQuotation();
                grvQuotItem.DeleteAllRows();

                MsgBox.OKMessage(MessageText.OKDelete);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
            finally
            {
                btnDelete.Enabled = true;
                Utils.CloseWaitForm();
            }
        }

        /// <summary>
        /// 견적서 출력
        /// </summary>
        private void PrintQuotation()
        {
            try
            {
                var field = new Dictionary<string, bool>()
                {
                    { "ReceiptID", txtReceiptID.EditValue.CString() == "" },
                    { "QuotID", txtQuotID.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                btnPrint.Enabled = false;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Print, SqlDbType.Char);
                inParams.Add("@i_ReceiptID", txtReceiptID.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_QuotID", txtQuotID.EditValue.CString(), SqlDbType.Char);

                var reportSource = SqlHelper.GetDataTable("xp_Quotation", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                var rptQuotation = new REPORTS.DESIGN.RPT_Quotation();
                rptQuotation.Preview(reportSource, true);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
            finally
            {
                MainButton.PrintButton = true;
            }
        }

        #endregion
    }
}