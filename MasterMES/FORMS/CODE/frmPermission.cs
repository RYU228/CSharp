using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DSLibrary;
using MasterMES.CLASSES;
//using Microsoft.Office.Interop.Excel;
//using System.Runtime.InteropServices;       //현재 열려있는 엑셀 확인
//using Excel = Microsoft.Office.Interop.Excel;

namespace MasterMES.FORMS.CODE
{
    public partial class frmPermission : frmBase, IButtonAction
    {
        #region 생성자
        public frmPermission()
        {
            InitializeComponent();

            DxControl.ButtonEditEvent += ButtonEditEvent;
            Extensions.ButtonEditEvent += ButtonEditEvent;
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

                var initSet = SqlHelper.GetDataSet("xp_Permission", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                // 전체 사원
                //grcProcess.SetColumns(initSet.Tables[0], "ProcessID,Process", false, true, true);
                grcPerson.SetColumns(initSet.Tables[0], "PersonID,SubSeq", false, true, true);
                grvPerson.SetRowIndicatorWidth();

                // 권한 부여된 사원
                grcPermission.SetColumns(initSet.Tables[1], "PersonID,SubSeq", false, true, true);
                grvPermission.SetRowIndicatorWidth();
                //grvPattern.Columns["SubSeq"].SortMode = DevExpress.XtraGrid.ColumnSortMode.Value;
                //grvPattern.Columns["SubSeq"].SortOrder = DevExpress.Data.ColumnSortOrder.Ascending;

                // 권한 리스트
                grcMain.SetColumns(initSet.Tables[2], "", false, true, true);

                grvMain.SetRowIndicatorWidth();
                grvMain.Columns["PermissionID"].SortMode = DevExpress.XtraGrid.ColumnSortMode.Value;
                grvMain.Columns["PermissionID"].SortOrder = DevExpress.Data.ColumnSortOrder.Ascending;

                SqlHelper.WriteLog(this, ActionLogType.Select, startTime);

                DxControl.SaveGridFormat(grvPerson, GridFormat.Default);
                DxControl.SaveGridFormat(grvPermission, GridFormat.Default);
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
            this.FillspdMain();
        }

        /// <summary>
        /// 신규 버튼
        /// </summary>
        public void NewButtonClick()
        {
            DxControl.ClearControl(lcgPattern);

            //this.FillspdPerson();

            txtPermissionID.Focus();
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
            //        { "ProcessID", txtPermissionID.EditValue.CString() == "" },
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
            //    inParams.Add("@i_CustCode", txtPermissionID.EditValue.CString(), SqlDbType.Char);
            //    inParams.Add("@i_InptUser", AppConfig.Login.PersonID, SqlDbType.VarChar);

            //    SqlHelper.Execute("xp_Customer", inParams, out string[] retVal, out string retCode, out string retMsg);

            //    if (retCode != "00")
            //        throw new Exception(retMsg);

            //    this.NewButtonClick();
            //    this.FillspdPermission();

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
                    { "Permission", txtPermission.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;
                var processID = ProcessType.Insert;
                var logType = ActionLogType.Insert;
                var message = MessageText.OKInsert;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Exist, SqlDbType.Char);
                inParams.Add("@i_PermissionID", txtPermissionID.EditValue.CString());

                if (SqlHelper.Exist("xp_Permission", inParams, out _, out _))
                {
                    if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionUpdate))
                        return;

                    processID = ProcessType.Update;
                    logType = ActionLogType.Update;
                    message = MessageText.OKUpdate;
                }

                Utils.ShowWaitForm(this);
                MainButton.SaveButton = false;

                var xmlPermissionSub = (grcPermission.DataSource as DataTable).DefaultView
                                    .ToTable(false, new string[] { "SubSeq", "PersonID" })
                                    .ToXml();

                inParams.Clear();
                inParams.Add("@i_ProcessID", processID, SqlDbType.Char);
                inParams.Add("@i_PermissionID", txtPermissionID.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_Permission", txtPermission.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_UseClss", chkUseClss.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_Remark", mmeRemark.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_InptUser", AppConfig.Login.PersonID, SqlDbType.VarChar);
                inParams.Add("@i_Xml_PermissionSub", xmlPermissionSub, SqlDbType.VarChar);

                SqlHelper.Execute("xp_Permission", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                var permission = txtPermission.EditValue.CString();
                this.FillspdMain();
                grvMain.FindRowByValue("Permission", permission);

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
                    { "PermissionID", txtPermissionID.EditValue.CString() == "" },
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
                inParams.Add("@i_PermissionID", txtPermissionID.EditValue.CString(), SqlDbType.Char);

                SqlHelper.Execute("xp_Permission", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                this.FillspdMain();

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
                var permissionID = (sender as GridView).GetFocusedRowCellValue("PermissionID").CString();
                this.ShowData(permissionID);
                //this.ViewCustomerChargeGrid();
                //this.ViewCustomerExpressGrid();
            }
        }

        private void grvMain_RowClick(object sender, RowClickEventArgs e)
        {
            var permissionID = (sender as GridView).GetFocusedRowCellValue("PermissionID").CString();
            this.ShowData(permissionID);
            //this.ViewCustomerChargeGrid();
            //this.ViewCustomerExpressGrid();
        }

        /// <summary>
        /// 사원 더블클릭
        /// </summary>
        private void grcPerson_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var point = grcPerson.PointToClient(Control.MousePosition);
                var hitInfo = grvPerson.CalcHitInfo(point);

                if (hitInfo.RowHandle < 0)
                    return;

                var sourceRow = grvPerson.GetDataRow(hitInfo.RowHandle);

                if (sourceRow != null && grcPermission.DataSource is System.Data.DataTable targetTable)
                {
                    var rows = targetTable.Select($"PersonID = '{sourceRow["PersonID"]}'");

                    if (rows.Length <= 0)
                    {
                        targetTable.Rows.Add(sourceRow.ItemArray);
                        //grvProcess.SetFocusedRowCellValue("BookmarkFlag", "☆");
                        //this.RefreshSortKey();
                    }
                }

                // SetData 메소드를 호출하여 데이터를 표시하는게 아니므로 RowIndicator의 폭을 별도 설정한다.
                grvPermission.SetRowIndicatorWidth();
                grvPermission.BestFitColumns();
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 선택된 사원 더블클릭
        /// </summary>
        private void grcPermission_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var point = grcPermission.PointToClient(Control.MousePosition);
                var hitInfo = grvPermission.CalcHitInfo(point);

                if (hitInfo.RowHandle < 0)
                    return;

                var sourceRow = grvPermission.GetDataRow(hitInfo.RowHandle);

                grvPermission.DeleteSelectedRows();

                // SetData 메소드를 호출하여 데이터를 표시하는게 아니므로 RowIndicator의 폭을 별도 설정한다.
                grvPermission.SetRowIndicatorWidth();
                grvPermission.BestFitColumns();
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
            grvPerson.GridControl.Focus();
            var index = grvPerson.FocusedRowHandle;

            if (index < 0)
                return;

            var sourceRow = grvPerson.GetDataRow(index);

            if (sourceRow != null && grcPermission.DataSource is System.Data.DataTable targetTable)
            {
                var rows = targetTable.Select($"PersonID = '{sourceRow["PersonID"]}'");

                if (rows.Length <= 0)
                {
                    targetTable.Rows.Add(sourceRow.ItemArray);
                    //grvProcess.DeleteSelectedRows();
                    //sourceRow["SubSeq"] = grvPattern.DataRowCount;
                    //this.RefreshSortKey();
                }
            }

            // SetData 메소드를 호출하여 데이터를 표시하는게 아니므로 RowIndicator의 폭을 별도 설정한다.
            grvPermission.SetRowIndicatorWidth();
            grvPermission.BestFitColumns();
        }

        /// <summary>
        /// 삭제 버튼 클릭
        /// </summary>
        private void btnMinus_Click(object sender, EventArgs e)
        {
            grvPermission.GridControl.Focus();
            var index = grvPermission.FocusedRowHandle;

            if (index < 0)
                return;

            grvPermission.DeleteSelectedRows();

            // SetData 메소드를 호출하여 데이터를 표시하는게 아니므로 RowIndicator의 폭을 별도 설정한다.
            grvPermission.SetRowIndicatorWidth();
            grvPermission.BestFitColumns();
        }

        /// <summary>
        ///  선택된 사원 최상
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFirst_Click(object sender, EventArgs e)
        {
            //grvPermission.GridControl.Focus();
            //var index = grvPermission.FocusedRowHandle;

            //if (index <= 0)
            //    return;

            ////var dt = grvPattern.DataSource as DataTable;
            ////var dr = dt.NewRow();
            ////dt.Rows.InsertAt(dr, 1);

            //var row1 = grvPermission.GetDataRow(index);
            //var row2 = grvPermission.GetDataRow(0);

            //var row = row1.ItemArray;
            //row1.ItemArray = row2.ItemArray;
            //row2.ItemArray = row;

            //grvPermission.FocusedRowHandle = 0;
        }

        /// <summary>
        /// 선택된 사원 순번 상
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrevious_Click(object sender, EventArgs e)
        {
            grvPermission.GridControl.Focus();
            var index = grvPermission.FocusedRowHandle;

            if (index <= 0)
                return;

            var row1 = grvPermission.GetDataRow(index);
            var row2 = grvPermission.GetDataRow(index - 1);

            var row = row1.ItemArray;
            row1.ItemArray = row2.ItemArray;
            row2.ItemArray = row;

            grvPermission.FocusedRowHandle = index - 1;
        }

        /// <summary>
        /// 선택된 사원 순번 하
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNext_Click(object sender, EventArgs e)
        {
            grvPermission.GridControl.Focus();
            var index = grvPermission.FocusedRowHandle;

            if (index >= grvPermission.DataRowCount - 1)
                return;

            var row1 = grvPermission.GetDataRow(index);
            var row2 = grvPermission.GetDataRow(index + 1);

            var row = row1.ItemArray;
            row1.ItemArray = row2.ItemArray;
            row2.ItemArray = row;

            grvPermission.FocusedRowHandle = index + 1;
        }

        /// <summary>
        /// 선택된 공정패턴 순번 최하
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLast_Click(object sender, EventArgs e)
        {
            //grvPermission.GridControl.Focus();
            //var index = grvPermission.FocusedRowHandle;

            //if (index >= grvPermission.DataRowCount-1)
            //    return;

            //var row1 = grvPermission.GetDataRow(index);
            //var row2 = grvPermission.GetDataRow(grvPermission.DataRowCount-1);

            //var row = row1.ItemArray;
            //row1.ItemArray = row2.ItemArray;
            //row2.ItemArray = row;

            //grvPermission.FocusedRowHandle = grvPermission.DataRowCount-1;
        }

        private bool ButtonEditEvent(object sender)
        {
            ButtonEdit bte = sender as ButtonEdit;
            FindDataResult fdResult = FindDataResult.FIND_NONE;

            switch (bte.Name)
            {
                // 입력 컨트롤
                case "bteMenu":
                    fdResult = FindData.ShowFindData(FindData.FIND_MENU, bte.EditValue.CString(), null, null, null, false, bte);
                    break;

                case "bteOrderID":
                    fdResult = FindData.ShowFindData(FindData.FIND_ORDERID, bte.EditValue.CString(), null, null, null, false, bte);
                    break;

                    //조건문 및 여러 정보를 가져올 경우 예시
                    //FindData.ShowFindData(FindData.FIND_ORDERSUB, bte.EditValue.CString(), "2022060001", null, null, false, bte, bteStuffWidth, bteWorkWidth);
            }

            // 찾기 성공이면 다음 컨트롤로 포커스 이동, 실패면 포커스 이동 중지
            if (fdResult == FindDataResult.FIND_OK)
                return true;
            else
                return false;
        }
        #endregion

        #region 사용자 정의 메서드
        /// <summary>
        /// 패턴 조회 : Row
        /// </summary>
        private void ShowData(string PermissionID)
        {
            try
            {
                //DxControl.ClearControl(lcgPattern);

                var startTime = DateTime.Now;

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetRow, SqlDbType.Char);
                inParams.Add("@i_PermissionID", PermissionID, SqlDbType.Char);

                var row = SqlHelper.GetDataRow("xp_Permission", inParams, out _, out _);

                if (row != null)
                {
                    txtPermissionID.EditValue = row["PermissionID"];
                    txtPermission.EditValue = row["Permission"];
                    chkUseClss.EditValue = row["UseClss"];
                    mmeRemark.EditValue = row["Remark"];
                }

                FillspdPermission();
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
        /// 권한 리스트 조회 : Main Grid
        /// </summary>
        private void FillspdMain()
        {
            try
            {
                var startTime = DateTime.Now;

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetList, SqlDbType.Char);
                //inParams.Add("@i_PermissionID", grvMain.GetFocusedRowCellValue("PatternID").CString(), SqlDbType.Char);

                //var table = SqlHelper.GetDataTable("xp_Permission", inParams, out _, out _);
                var dataSet = SqlHelper.GetDataSet("xp_Permission", inParams, out string retCode, out string retMsg);

                //grcMain.SetColumns(initSet.Tables[0], "UseClss");
                //grcMain.SetData(table);
                grcMain.SetData(dataSet.Tables[2]);

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
        /// 전체 사원 조회 : Grid
        /// </summary>
        private void FillspdPerson()
        {
            try
            {
                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetList, SqlDbType.Char);

                var initSet = SqlHelper.GetDataSet("xp_Permission", inParams, out string retCode, out string retMsg);

                //grcProcess.SetColumns(initSet.Tables[0], "ProcessID,SubSeq", false, true, true);
                grcPerson.SetData(initSet.Tables[0]);
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
        /// 선택된 사원 조회 : Grid
        /// </summary>
        private void FillspdPermission()
        {
            try
            {
                var inParams = new DbParamList();
                inParams.Add("@i_processID", ProcessType.GetList, SqlDbType.Char);
                inParams.Add("@i_PermissionID", txtPermissionID.EditValue.CString(), SqlDbType.Char);

                var initSet = SqlHelper.GetDataSet("xp_Permission", inParams, out string retCode, out string retMsg);

                //grcPattern.SetColumns(initSet.Tables[1], "ProcessID,SubSeq", false, true, true);
                grcPermission.SetData(initSet.Tables[1]);
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
