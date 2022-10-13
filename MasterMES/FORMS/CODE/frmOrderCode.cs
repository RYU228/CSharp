using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DSLibrary;

namespace DS_MES.FORMS.CODE
{
    public partial class frmOrderCode : frmBase, CLASSES.IButtonAction
    {
        public frmOrderCode()
        {
            InitializeComponent();
            grvInput.OptionsCustomization.AllowSort = false;
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

                var initSet = SqlHelper.GetDataSet("xp_OrderCode", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    return;

                grcOrd.SetColumns(initSet.Tables[0], "", false, true, true);
                grcInput.SetColumns(initSet.Tables[1], "", false);

                NewButtonClick();
                dteStrOrdDate.SetDefaultDate();
                dteEndOrdDate.SetDefaultDate();

                // 컬럼 폭 고정
                grvInput.FixedColumnWidth(new Dictionary<string, int>()
                {
                    { "DeleteButton", 40 },
                    { "FindButton", 40 },
                    { "ItemCode", 80 },
                    { "ItemName", 250 },
                    { "OrdPrice", 100 },
                    { "OrdQty", 80 },
                    { "OrdAmt", 120 },
                });

                // Repository Item Setting
                // 행삭제 버튼
                var riDeleteRow = grvInput.SetRepositoryItemSimpleButton("DeleteButton", RepositoryButtonType.Delete);
                riDeleteRow.Click += RiDeleteRow_Click;

                // 품목코드
                var riItemCode = grvInput.SetRepositoryItemSimpleButton("FindButton", RepositoryButtonType.Find);
                riItemCode.ButtonClick += RiItemCode_Click;

                // 발주 단가
                var riOrdPrice = grvInput.SetRepositoryItemSpinEdit("OrdPrice"); //, "N2");
                riOrdPrice.EditValueChanged += RiOrdPrice_EditValueChanged;

                // 발주 수량
                var riOrdQty = grvInput.SetRepositoryItemSpinEdit("OrdQty"); //, "N1");
                riOrdQty.EditValueChanged += RiOrdQty_EditValueChanged;

                // 발주 구분
                var riOrdType = grvInput.SetRepositoryItemGridLookUpEdit("OrdType", initSet.Tables[2], "MiddleCode", "MiddleName", "UseClss");

                // 비고
                var riRemark = grvInput.SetRepositoryItemTextEdit("Remark");
                riRemark.EditValueChanged += RiRemark_EditValueChanged;
                riRemark.Leave += RiRemark_Leave;

                DxControl.SaveGridFormat(grvOrd, GridFormat.Default);
                DxControl.SaveGridFormat(grvInput, GridFormat.Default);

                DxControl.LoadGridFormat(grvOrd, GridFormat.Current);
                DxControl.LoadGridFormat(grvInput, GridFormat.Current);
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
                grvInput.DeleteAllRows();

                Utils.ShowWaitForm(this);
                MainButton.ViewButton = false;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetList, SqlDbType.Char);
                inParams.Add("@i_StrOrdDate", dteStrOrdDate.EditValue.CDateString(), SqlDbType.Date);
                inParams.Add("@i_EndOrdDate", dteEndOrdDate.EditValue.CDateString(), SqlDbType.Date);

                grcOrd.SetData(SqlHelper.GetDataTable("xp_OrderCode", inParams, out _, out _));

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
            dteOrdDate.SetDefaultDate();
            grvInput.DeleteAllRows();

            dteOrdDate.Focus();
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
            try
            {
                // 품목이 입력되지 않은 행이 존재하는지
                var errorItemCode = (grcInput.DataSource as DataTable).DefaultView.ToTable().Select("ItemCode IS NULL OR ItemCode = ''");

                // 단가가 0인 행이 존재하는지
                var errorOrdPrice = (grcInput.DataSource as DataTable).DefaultView.ToTable().Select("OrdPrice = 0");

                // 수량이 0인 행이 존재하는지
                var errorOrdQty = (grcInput.DataSource as DataTable).DefaultView.ToTable().Select("OrdQty = 0");

                var field = new Dictionary<string, bool>()
                {
                    { "Order Info", grvInput.RowCount == 0 },
                    { "ItemCode", errorItemCode != null && errorItemCode.Length > 0 },
                    { "OrdPrice", errorOrdPrice != null && errorOrdPrice.Length > 0 },
                    { "OrdQty", errorOrdQty != null && errorOrdQty.Length > 0 }
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;
                var logType = ActionLogType.Insert;
                var message = MessageText.OKInsert;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Exist, SqlDbType.Char);
                inParams.Add("@i_OrdDate", dteOrdDate.EditValue.CDateString(), SqlDbType.Date);

                if (SqlHelper.Exist("xp_OrderCode", inParams, out _, out _))
                {
                    if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionUpdate))
                        return;

                    logType = ActionLogType.Update;
                    message = MessageText.OKUpdate;
                }

                Utils.ShowWaitForm(this);
                MainButton.SaveButton = false;

                var xmlData = (grcInput.DataSource as DataTable).DefaultView
                              .ToTable(false, new string[] { "ItemCode", "OrdPrice", "OrdQty", "OrdAmt", "OrdType", "Remark" })
                              .ToXml();

                inParams.Clear();
                inParams.Add("@i_ProcessID", ProcessType.Insert, SqlDbType.Char);
                inParams.Add("@i_OrdDate", dteOrdDate.EditValue.CDateString(), SqlDbType.Date);
                inParams.Add("@i_XmlData", xmlData, SqlDbType.VarChar);
                inParams.Add("@i_InptUser", AppConfig.Login.PersonID, SqlDbType.VarChar);

                SqlHelper.Execute("xp_OrderCode", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                var ordDate = grvOrd.GetFocusedRowCellValue("OrdDate");
                this.ViewButtonClick();
                grvOrd.FindRowByValue("OrdDate", ordDate);

                SqlHelper.WriteLog(this, logType, startTime);

                MsgBox.OKMessage(message);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
            finally
            {
                MainButton.SaveButton = true;
                Utils.CloseWaitForm();
            }
        }

        /// <summary>
        /// 삭제 버튼
        /// </summary>
        public void DeleteButtonClick()
        {
            try
            {
                var startTime = DateTime.Now;

                Utils.ShowWaitForm(this);
                MainButton.DeleteButton = false;

                if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionDelete))
                    return;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Delete, SqlDbType.Char);
                inParams.Add("@i_OrdDate", dteOrdDate.EditValue.CDateString(), SqlDbType.Date);

                SqlHelper.Execute("xp_OrderCode", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                SqlHelper.WriteLog(this, ActionLogType.Delete, startTime);
                this.ViewButtonClick();

                MsgBox.OKMessage(MessageText.OKDelete);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
            finally
            {
                MainButton.DeleteButton = true;
                Utils.CloseWaitForm();
            }
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
        /// 행 추가 버튼 클릭 (Simple Button)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAppendRow_Click(object sender, EventArgs e)
        {
            grvInput.AddNewRow();
            grvInput.BestFitColumns();
            grvInput.UpdateCurrentRow();
        }

        /// <summary>
        /// 행 삭제 버튼 클릭(Simple Button)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteRow_Click(object sender, EventArgs e)
        {
            // 입력항목이 비어 있으면 그냥 삭제, 입력값이 있으면 사용자 확인 후 삭제
            if (grvInput.GetFocusedRowCellValue("ItemCode").CString() == "" &&
                    grvInput.GetFocusedRowCellValue("OrdPrc").CDouble() == 0d &&
                    grvInput.GetFocusedRowCellValue("OrdQty").CDouble() == 0d &&
                    grvInput.GetFocusedRowCellValue("OrdType").CString() == "" &&
                    grvInput.GetFocusedRowCellValue("Remark").CString() == "")
                grvInput.DeleteRow(grvInput.FocusedRowHandle);
            else
            {
                if (DialogResult.Yes == MsgBox.QuestionMessage(MessageText.QuestionDeleteRow))
                    grvInput.DeleteRow(grvInput.FocusedRowHandle);
            }
        }

        /// <summary>
        /// 신규 행이 추가될때
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvInput_InitNewRow(object sender, InitNewRowEventArgs e)
        {
            var view = sender as GridView;

            view.SetRowCellValue(e.RowHandle, "ItemCode", null);
            view.SetRowCellValue(e.RowHandle, "ItemName", null);
            view.SetRowCellValue(e.RowHandle, "OrdPrice", 0);
            view.SetRowCellValue(e.RowHandle, "OrdQty", 0);
            view.SetRowCellValue(e.RowHandle, "OrdAmt", 0);
            view.SetRowCellValue(e.RowHandle, "OrdType", null);
            view.SetRowCellValue(e.RowHandle, "Remark", null);

            view.SetRowIndicatorWidth();
        }

        /// <summary>
        /// 입력가능 컬럼 배경색 변경
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvInput_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            if (e.RowHandle >= 0)
            {
                if (new string[] { "OrdPrice", "OrdQty", "OrdType", "Remark" }.Contains(e.Column.FieldName))
                {
                    e.Appearance.ForeColor = Color.White;
                    e.Appearance.BackColor = UserColors.EditableColor;
                }
            }
        }

        /// <summary>
        /// 행 삭제 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RiDeleteRow_Click(object sender, EventArgs e)
        {
            if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionDeleteRow))
                return;

            grvInput.DeleteRow(grvInput.FocusedRowHandle);
        }

        /// <summary>
        /// 품목 찾기 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RiItemCode_Click(object sender, EventArgs e)
        {
            var inParams = new DbParamList();
            inParams.Add("@i_ProcessID", "82", SqlDbType.Char);

            var item = SqlHelper.GetDataTable("xp_OrderCode", inParams, out _, out _);

            var findItem = new frmFindValue("Search Item", item);
            findItem.FindValueEvent += FindItem_FindValueEvent;
            Utils.ShowPopupForm(findItem);
        }

        /// <summary>
        /// 품목 검색
        /// </summary>
        /// <param name="row"></param>
        private void FindItem_FindValueEvent(DataRow row)
        {
            try
            {
                if (row == null)
                    return;

                grvInput.SetFocusedRowCellValue("ItemCode", row["ItemCode"]);
                grvInput.SetFocusedRowCellValue("ItemName", row["ItemName"]);

                grvInput.UpdateCurrentRow();
                grvInput.BestFitColumns();

                grvInput.FocusedColumn = grvInput.Columns["OrdPrice"];
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 단가 입력
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RiOrdPrice_EditValueChanged(object sender, EventArgs e)
        {
            grvInput.SetFocusedRowCellValue("OrdPrice", (sender as SpinEdit).EditValue);

            if (grvInput.PostEditor())
                grvInput.UpdateCurrentRow();

            var ordAmt = grvInput.GetFocusedRowCellValue("OrdPrice").CDouble() * grvInput.GetFocusedRowCellValue("OrdQty").CDouble();
            grvInput.SetFocusedRowCellValue("OrdAmt", ordAmt);
        }

        /// <summary>
        /// 수량 입력
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RiOrdQty_EditValueChanged(object sender, EventArgs e)
        {
            grvInput.SetFocusedRowCellValue("OrdQty", (sender as SpinEdit).EditValue);

            if (grvInput.PostEditor())
                grvInput.UpdateCurrentRow();

            var ordAmt = grvInput.GetFocusedRowCellValue("OrdPrice").CDouble() * grvInput.GetFocusedRowCellValue("OrdQty").CDouble();
            grvInput.SetFocusedRowCellValue("OrdAmt", ordAmt);
        }

        /// <summary>
        /// 비고 입력
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RiRemark_EditValueChanged(object sender, EventArgs e)
        {
            grvInput.SetFocusedRowCellValue("Remark", (sender as SpinEdit).EditValue);

            if (grvInput.PostEditor())
                grvInput.UpdateCurrentRow();
        }

        /// <summary>
        /// 비고란 입력후 Focus가 벗어날때 컬럼 폭 재조정
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RiRemark_Leave(object sender, EventArgs e)
        {
            grvInput.BestFitColumns();
        }

        /// <summary>
        /// 입력 그리드에 엔터키 입력시 다음 임력 컬럼으로 이동
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                (sender as GridView).MoveCellFocus(); // "OrdPrice,OrdQty,OrdType,Remark");
        }

        /// <summary>
        /// 그리드 행 포커스가 변경될 때
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvOrd_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle >= 0)
                ViewOrderCodeRow();
        }

        /// <summary>
        /// 그리드 행을 클릭할 때
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvOrd_RowClick(object sender, RowClickEventArgs e)
        {
            ViewOrderCodeRow();
        }

        #endregion

        #region 사용자 정의 메서드
        /// <summary>
        /// 일자에 해당하는 발주 조회
        /// </summary>
        private void ViewOrderCodeRow()
        {
            try
            {
                Utils.ShowWaitForm(this);

                dteOrdDate.EditValue = grvOrd.GetFocusedRowCellValue("OrdDate");

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", "81", SqlDbType.Char);
                inParams.Add("@i_OrdDate", grvOrd.GetFocusedRowCellValue("OrdDate"), SqlDbType.Date);

                grcInput.SetData(SqlHelper.GetDataTable("xp_OrderCode", inParams, out _, out _));
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
        #endregion
    }
}