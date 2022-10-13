using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Grid;
using DSLibrary;
//using Microsoft.Office.Interop.Excel;
//using System.Runtime.InteropServices;       //현재 열려있는 엑셀 확인
//using Excel = Microsoft.Office.Interop.Excel;

namespace MasterMES.FORMS.CODE
{
    public partial class frmPattern : frmBase, CLASSES.IButtonAction
    {
        #region 생성자
        public frmPattern()
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
            try
            {
                var startTime = DateTime.Now;

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Initial, SqlDbType.Char);

                var initSet = SqlHelper.GetDataSet("xp_Pattern", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                // 전체 공정
                //grcProcess.SetColumns(initSet.Tables[0], "ProcessID,Process", false, true, true);
                grcProcess.SetColumns(initSet.Tables[0], "ProcessID,SubSeq", false, true, true);
                grvProcess.SetRowIndicatorWidth();

                // 구분 컬럼 폭 고정
                //grvProcess.FixedColumnWidth("BookmarkFlag", 50);

                // 패턴공정
                grcPattern.SetColumns(initSet.Tables[1], "SubSeq,ProcessID", false, true, true);
                grvPattern.SetRowIndicatorWidth();
                //grvPattern.Columns["SubSeq"].SortMode = DevExpress.XtraGrid.ColumnSortMode.Value;
                //grvPattern.Columns["SubSeq"].SortOrder = DevExpress.Data.ColumnSortOrder.Ascending;

                // 패턴리스트
                grcMain.SetColumns(initSet.Tables[2], "", false, true, true);

                grvMain.SetRowIndicatorWidth();
                grvMain.Columns["PatternID"].SortMode = DevExpress.XtraGrid.ColumnSortMode.Value;
                grvMain.Columns["PatternID"].SortOrder = DevExpress.Data.ColumnSortOrder.Ascending;

                SqlHelper.WriteLog(this, ActionLogType.Select, startTime);

                DxControl.SaveGridFormat(grvProcess, GridFormat.Default);
                DxControl.SaveGridFormat(grvPattern, GridFormat.Default);
                DxControl.SaveGridFormat(grvMain, GridFormat.Default);
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
            this.ViewMainGrid();
        }

        /// <summary>
        /// 신규 버튼
        /// </summary>
        public void NewButtonClick()
        {
            DxControl.ClearControl(lcgPattern);

            this.ViewProcessGrid();

            txtPatternCode.Focus();
        }

        /// <summary>
        /// 복사 버튼
        /// </summary>
        public void CopyButtonClick()
        {
            //try
            //{
            //    var field = new Dictionary<string, bool>()
            //    {
            //        { "ProcessID", txtPatternCode.EditValue.CString() == "" },
            //    };
            //    if (Utils.CheckField(field))
            //        return;

            //    var startTime = DateTime.Now;

            //    if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionCopy))
            //        return;

            //    Utils.ShowWaitForm(this);
            //    MainButton.CopyButton = false;

            //    var inParams = new DbParamList();
            //    inParams.Add("@i_ProcessID", ProcessType.Copy, SqlDbType.Char);
            //    inParams.Add("@i_CustCode", txtPatternCode.EditValue.CString(), SqlDbType.Char);
            //    inParams.Add("@i_InptUser", AppConfig.Login.PersonID, SqlDbType.VarChar);

            //    SqlHelper.Execute("xp_Customer", inParams, out string[] retVal, out string retCode, out string retMsg);

            //    if (retCode != "00")
            //        throw new Exception(retMsg);

            //    this.NewButtonClick();
            //    this.ViewPatternGrid();

            //    // 신규 생성된 거래처 코드에 해당하는 데이터를 화면에 디스플레이(FocusedRowChanged Event 발생)
            //    grvMain.FindRowByValue("CustCode", retVal[0]);

            //    SqlHelper.WriteLog(this, ActionLogType.Insert, startTime);
            //    MsgBox.OKMessage(MessageText.OKCopy);
            //}
            //catch (Exception ex)
            //{
            //    MsgBox.ErrorMessage(ex.CString());
            //}
            //finally
            //{
            //    MainButton.CopyButton = true;
            //    Utils.CloseWaitForm();
            //}
        }

        /// <summary>
        /// 저장 버튼
        /// </summary>
        public void SaveButtonClick()
        {
            try
            {
                var field = new Dictionary<string, bool>()
                {
                    { "Pattern", txtPattern.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;
                var processID = ProcessType.Insert;
                var logType = ActionLogType.Insert;
                var message = MessageText.OKInsert;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Exist, SqlDbType.Char);
                inParams.Add("@i_PatternCode", txtPatternCode.EditValue.CString());

                if (SqlHelper.Exist("xp_Pattern", inParams, out _, out _))
                {
                    if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionUpdate))
                        return;

                    processID = ProcessType.Update;
                    logType = ActionLogType.Update;
                    message = MessageText.OKUpdate;
                }

                Utils.ShowWaitForm(this);
                MainButton.SaveButton = false;

                var xmlPatternSub = (grcPattern.DataSource as DataTable).DefaultView
                                    .ToTable(false, new string[] { "SubSeq", "ProcessID", "Process" })
                                    .ToXml();

                inParams.Clear();
                inParams.Add("@i_ProcessID", processID, SqlDbType.Char);
                inParams.Add("@i_PatternCode", txtPatternCode.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_Pattern", txtPattern.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_Remark", mmeRemark.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_InptUser", AppConfig.Login.PersonID, SqlDbType.VarChar);
                inParams.Add("@i_Xml_PatternSub", xmlPatternSub, SqlDbType.VarChar);

                SqlHelper.Execute("xp_Pattern", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                var pattern = txtPattern.EditValue.CString();
                this.ViewMainGrid();
                grvMain.FindRowByValue("Pattern", pattern);

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
                var field = new Dictionary<string, bool>()
                {
                    { "ProcessID", txtPatternCode.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;

                if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionDelete))
                    return;

                Utils.ShowWaitForm(this);
                MainButton.SaveButton = false;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Delete, SqlDbType.Char);
                inParams.Add("@i_PatternCode", txtPatternCode.EditValue.CString(), SqlDbType.Char);

                SqlHelper.Execute("xp_Pattern", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                this.ViewMainGrid();

                SqlHelper.WriteLog(this, ActionLogType.Delete, startTime);
                MsgBox.OKMessage(MessageText.OKInsert);
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
        /// 출력 버튼
        /// </summary>
        public void PrintButtonClick(int reportIndex)
        {
            //Excel.Application oExcel = (Excel.Application)Marshal.GetActiveObject("Excel.Application");

            //Excel.Application oExcel;
            //Excel.Workbook oExcelBook;
            //Excel.Worksheet oExcelSheet;

            //oExcel = new Excel.Application();
            //oExcel.Visible = true;

            //oExcelBook = oExcel.Workbooks.Open("C:\\test.xls");
        }

        #endregion

        #region 폼 컨트롤 이벤트
        /// <summary>
        /// 그리드 행변경시 화면에 정보 표시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvMain_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle >= 0)
            {
                var patternCode = (sender as GridView).GetFocusedRowCellValue("PatternID").CString();
                this.ViewMainRow(patternCode);
                //this.ViewCustomerChargeGrid();
                //this.ViewCustomerExpressGrid();
            }
        }

        private void grvMain_RowClick(object sender, RowClickEventArgs e)
        {
            var patternCode = (sender as GridView).GetFocusedRowCellValue("PatternID").CString();
            this.ViewMainRow(patternCode);
            //this.ViewCustomerChargeGrid();
            //this.ViewCustomerExpressGrid();
        }

        /// <summary>
        /// 공정 더블클릭
        /// </summary>
        private void grcProcess_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var point = grcProcess.PointToClient(Control.MousePosition);
                var hitInfo = grvProcess.CalcHitInfo(point);

                if (hitInfo.RowHandle < 0)
                    return;

                var sourceRow = grvProcess.GetDataRow(hitInfo.RowHandle);

                if (sourceRow != null && grcPattern.DataSource is System.Data.DataTable targetTable)
                {
                    var rows = targetTable.Select($"ProcessID = '{sourceRow["ProcessID"]}'");

                    if (rows.Length <= 0)
                    {
                        targetTable.Rows.Add(sourceRow.ItemArray);
                        //grvProcess.SetFocusedRowCellValue("BookmarkFlag", "☆");
                        //this.RefreshSortKey();
                    }
                }

                // SetData 메소드를 호출하여 데이터를 표시하는게 아니므로 RowIndicator의 폭을 별도 설정한다.
                grvPattern.SetRowIndicatorWidth();
                grvPattern.BestFitColumns();
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 선택된 공정 더블클릭
        /// </summary>
        private void grcPattern_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var point = grcPattern.PointToClient(Control.MousePosition);
                var hitInfo = grvPattern.CalcHitInfo(point);

                if (hitInfo.RowHandle < 0)
                    return;

                var sourceRow = grvPattern.GetDataRow(hitInfo.RowHandle);

                grvPattern.DeleteSelectedRows();

                // SetData 메소드를 호출하여 데이터를 표시하는게 아니므로 RowIndicator의 폭을 별도 설정한다.
                grvPattern.SetRowIndicatorWidth();
                grvPattern.BestFitColumns();
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 추가 버튼 클릭
        /// </summary>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            grvProcess.GridControl.Focus();
            var index = grvProcess.FocusedRowHandle;

            if (index < 0)
                return;

            var sourceRow = grvProcess.GetDataRow(index);

            if (sourceRow != null && grcPattern.DataSource is System.Data.DataTable targetTable)
            {
                var rows = targetTable.Select($"ProcessID = '{sourceRow["ProcessID"]}'");

                if (rows.Length <= 0)
                {
                    targetTable.Rows.Add(sourceRow.ItemArray);
                    //grvProcess.DeleteSelectedRows();
                    //sourceRow["SubSeq"] = grvPattern.DataRowCount;
                    //this.RefreshSortKey();
                }
            }

            // SetData 메소드를 호출하여 데이터를 표시하는게 아니므로 RowIndicator의 폭을 별도 설정한다.
            grvPattern.SetRowIndicatorWidth();
            grvPattern.BestFitColumns();
        }

        /// <summary>
        /// 삭제 버튼 클릭
        /// </summary>
        private void btnMinus_Click(object sender, EventArgs e)
        {
            grvPattern.GridControl.Focus();
            var index = grvPattern.FocusedRowHandle;

            if (index < 0)
                return;

            grvPattern.DeleteSelectedRows();

            // SetData 메소드를 호출하여 데이터를 표시하는게 아니므로 RowIndicator의 폭을 별도 설정한다.
            grvPattern.SetRowIndicatorWidth();
            grvPattern.BestFitColumns();
        }

        /// <summary>
        ///  선택된 공정패턴 최상
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFirst_Click(object sender, EventArgs e)
        {
            grvPattern.GridControl.Focus();
            var index = grvPattern.FocusedRowHandle;

            if (index <= 0)
                return;

            //var dt = grvPattern.DataSource as DataTable;
            //var dr = dt.NewRow();
            //dt.Rows.InsertAt(dr, 1);

            var row1 = grvPattern.GetDataRow(index);
            var row2 = grvPattern.GetDataRow(0);

            var row = row1.ItemArray;
            row1.ItemArray = row2.ItemArray;
            row2.ItemArray = row;

            grvPattern.FocusedRowHandle = 0;
        }

        /// <summary>
        /// 선택된 공정패턴 순번 상
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrevious_Click(object sender, EventArgs e)
        {
            grvPattern.GridControl.Focus();
            var index = grvPattern.FocusedRowHandle;

            if (index <= 0)
                return;

            var row1 = grvPattern.GetDataRow(index);
            var row2 = grvPattern.GetDataRow(index-1);

            var row = row1.ItemArray;
            row1.ItemArray = row2.ItemArray;
            row2.ItemArray = row;

            grvPattern.FocusedRowHandle = index -1;
        }

        /// <summary>
        /// 선택된 공정패턴 순번 하
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNext_Click(object sender, EventArgs e)
        {
            grvPattern.GridControl.Focus();
            var index = grvPattern.FocusedRowHandle;

            if (index >= grvPattern.DataRowCount - 1)
                return;

            var row1 = grvPattern.GetDataRow(index);
            var row2 = grvPattern.GetDataRow(index + 1);

            var row = row1.ItemArray;
            row1.ItemArray = row2.ItemArray;
            row2.ItemArray = row;

            grvPattern.FocusedRowHandle = index + 1;
        }

        /// <summary>
        /// 선택된 공정패턴 순번 최하
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLast_Click(object sender, EventArgs e)
        {
            grvPattern.GridControl.Focus();
            var index = grvPattern.FocusedRowHandle;

            if (index >= grvPattern.DataRowCount-1)
                return;

            var row1 = grvPattern.GetDataRow(index);
            var row2 = grvPattern.GetDataRow(grvPattern.DataRowCount-1);

            var row = row1.ItemArray;
            row1.ItemArray = row2.ItemArray;
            row2.ItemArray = row;

            grvPattern.FocusedRowHandle = grvPattern.DataRowCount-1;
        }

        #endregion

        #region 사용자 정의 메서드
        /// <summary>
        /// 패턴 조회 : Row
        /// </summary>
        private void ViewMainRow(string patternCode)
        {
            try
            {
                //DxControl.ClearControl(lcgPattern);

                var startTime = DateTime.Now;

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetRow, SqlDbType.Char);
                inParams.Add("@i_PatternCode", patternCode, SqlDbType.Char);

                var row = SqlHelper.GetDataRow("xp_Pattern", inParams, out _, out _);

                if (row != null)
                {
                    txtPatternCode.EditValue = row["PatternID"];
                    txtPattern.EditValue = row["Pattern"];
                    chkUseClss.EditValue = row["UseClss"];
                    mmeRemark.EditValue = row["Remark"];
                }

                ViewPatternGrid();
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
        /// 등록된 패턴 조회 : Main Grid
        /// </summary>
        private void ViewMainGrid()
        {
            try
            {
                var startTime = DateTime.Now;

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetList, SqlDbType.Char);
                //inParams.Add("@i_PatternCode", grvMain.GetFocusedRowCellValue("PatternID").CString(), SqlDbType.Char);

                //var table = SqlHelper.GetDataTable("xp_Pattern", inParams, out _, out _);
                var dataSet = SqlHelper.GetDataSet("xp_Pattern", inParams, out string retCode, out string retMsg);

                //grcMain.SetColumns(initSet.Tables[0], "UseClss");
                //grcMain.SetData(table);
                grcMain.SetData(dataSet.Tables[1]);

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
        /// 공정 조회 : Grid
        /// </summary>
        private void ViewProcessGrid()
        {
            try
            {
                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetList, SqlDbType.Char);

                var initSet = SqlHelper.GetDataSet("xp_Pattern", inParams, out string retCode, out string retMsg);

                //grcProcess.SetColumns(initSet.Tables[0], "ProcessID,SubSeq", false, true, true);
                grcProcess.SetData(initSet.Tables[0]);
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
        /// 선택된 패턴 공정 조회 : Grid
        /// </summary>
        private void ViewPatternGrid()
        {
            try
            {
                var inParams = new DbParamList();
                inParams.Add("@i_processID", ProcessType.GetList, SqlDbType.Char);
                inParams.Add("@i_PatternCode", txtPatternCode.EditValue.CString(), SqlDbType.Char);

                var initSet = SqlHelper.GetDataSet("xp_Pattern", inParams, out string retCode, out string retMsg);

                //grcPattern.SetColumns(initSet.Tables[1], "ProcessID,SubSeq", false, true, true);
                grcPattern.SetData(initSet.Tables[2]);
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
