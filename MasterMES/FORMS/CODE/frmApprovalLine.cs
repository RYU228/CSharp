using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTreeList;
using DSLibrary;

namespace MasterMES.FORMS.CODE
{
    public partial class frmApprovalLine : frmBase, CLASSES.IButtonAction
    {
        #region 생성자
        public frmApprovalLine()
        {
            InitializeComponent();

            grvApprvLine.OptionsCustomization.AllowSort = false;
            grvApprvLineSub.OptionsCustomization.AllowSort = false;
        }

        #endregion

        #region 오버라이드 이벤트
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

                var initSet = SqlHelper.GetDataSet("xp_ApprovalLine", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                tlsPerson.SetColumns(initSet.Tables[0], "SortKey1,SortKey2", autoColumnWidth:true);
                grcApprvLine.SetColumns(initSet.Tables[1], "ApprvSeq", showGroupPanel: false, showFilterRow: false, true);
                grcApprvLineSub.SetColumns(initSet.Tables[2], "ApprvSubSeq", showGroupPanel: false, showFilterRow: false);

                tlsPerson.SetData(initSet.Tables[0], 0);
                grcApprvLine.SetData(initSet.Tables[1]);

                // 그리드 초기 형식 저장
                DxControl.SaveGridFormat(grvApprvLine, GridFormat.Default);
                DxControl.SaveGridFormat(grvApprvLineSub, GridFormat.Default);

                this.FillGridApprovalLine();
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
        }

        /// <summary>
        /// 신규 버튼
        /// </summary>
        public void NewButtonClick()
        {
            txtApprvSeq.EditValue = null;
            txtApprvTitle.EditValue = null;
            grvApprvLineSub.DeleteAllRows();

            txtApprvTitle.Focus();
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
                var field = new Dictionary<string, bool>()
                {
                    { "ApprvTitle", txtApprvTitle.EditValue.CString() == "" },
                    { "ApprovalLineSub Info", grvApprvLineSub.RowCount <= 0 },
                };
                if (Utils.CheckField(field))
                    return;

                // 결재자의 순번 생성 - 사원 추가 순서대로 순번을 부여하기 위해서 : GridView 입력 -> DataTable -> XML로 전달
                for (int i = 0; i < grvApprvLineSub.RowCount; i++)
                    grvApprvLineSub.SetRowCellValue(i, "ApprvSubSeq", i + 1);

                var startTime = DateTime.Now;
                var logType = ActionLogType.Insert;
                var message = MessageText.OKInsert;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Exist, SqlDbType.Char);
                inParams.Add("@i_PersonID", AppConfig.Login.PersonID, SqlDbType.VarChar);
                inParams.Add("@i_ApprvSeq", txtApprvSeq.EditValue.CShort(), SqlDbType.TinyInt);

                if (SqlHelper.Exist("xp_ApprovalLine", inParams, out string a, out string b))
                {
                    if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionUpdate))
                        return;

                    logType = ActionLogType.Update;
                    message = MessageText.OKUpdate;
                }

                var xmlData = (grcApprvLineSub.DataSource as DataTable).DefaultView
                              .ToTable(false, new string[] { "ApprvSubSeq", "ApprvPersonID" })
                              .ToXml();

                inParams.Clear();
                inParams.Add("@i_ProcessID", ProcessType.Merge, SqlDbType.Char);
                inParams.Add("@i_PersonID", AppConfig.Login.PersonID, SqlDbType.VarChar);
                inParams.Add("@i_ApprvSeq", txtApprvSeq.EditValue.CShort(), SqlDbType.TinyInt);
                inParams.Add("@i_ApprvTitle", txtApprvTitle.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_XmlData", xmlData, SqlDbType.VarChar);

                SqlHelper.Execute("xp_ApprovalLine", inParams, out string[] retVal, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                this.NewButtonClick();

                this.FillGridApprovalLine();

                // 해당 메서드 타입으로는 숫자 값이 검색이 안됨. 원인 모르겠음
                // grvApprvLine.FindRowByValue("ApprvSeq", retVal[0]);
                grvApprvLine.FindRowByValue(new Dictionary<string, object>() { { "ApprvSeq", retVal[0] } });

                SqlHelper.WriteLog(this, logType, startTime);
                MsgBox.OKMessage(message);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
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
                    { "ApprvSeq", txtApprvSeq.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;

                if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionDelete))
                    return;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Delete, SqlDbType.Char);
                inParams.Add("@i_PersonID", AppConfig.Login.PersonID, SqlDbType.VarChar);
                inParams.Add("@i_ApprvSeq", txtApprvSeq.EditValue.CShort(), SqlDbType.TinyInt);

                SqlHelper.Execute("xp_ApprovalLine", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                this.NewButtonClick();
                this.FillGridApprovalLine();

                SqlHelper.WriteLog(this, ActionLogType.Delete, startTime);
                MsgBox.OKMessage(MessageText.OKDelete);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
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
        /// TreeList에서 사원 더블 클릭시 결재자 정보 그리드에 행 추가
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tlsPerson_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var tree = sender as TreeList;
                var info = tree.CalcHitInfo(tree.PointToClient(MousePosition));

                if (info.Node == null || info.Node.Level == 0)
                    return;

                var personID = info.Node.GetValue("KeyField").CString().Substring(4);

                // 이미 추가된 결재자인지 체크한다.
                if (grvApprvLineSub.RowCount > 0)
                {
                    var rows = (grvApprvLineSub.DataSource as DataView).ToTable().Select($"ApprvPersonID = '{personID}'");
                    if (rows != null && rows.Length > 0)
                    {
                        MsgBox.WarningMessage("MSG_EXISTS_PERSON".Localization());
                        return;
                    }
                }

                // 사원에 대한 정보 조회
                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", "91", SqlDbType.Char);
                inParams.Add("@i_PersonID", personID, SqlDbType.VarChar);

                var row = SqlHelper.GetDataRow("xp_ApprovalLine", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                if (row != null)
                {
                    grvApprvLineSub.AddNewRow();
                    grvApprvLineSub.FocusedRowHandle = DevExpress.XtraGrid.GridControl.NewItemRowHandle;

                    grvApprvLineSub.SetFocusedRowCellValue("ApprvSubSeq", 0);
                    grvApprvLineSub.SetFocusedRowCellValue("DepartName", row["DepartName"]);
                    grvApprvLineSub.SetFocusedRowCellValue("DutyName", row["DutyName"]);
                    grvApprvLineSub.SetFocusedRowCellValue("ApprvPersonID", row["ApprvPersonID"]);
                    grvApprvLineSub.SetFocusedRowCellValue("PersonName", row["PersonName"]);
                }

                if (grvApprvLineSub.PostEditor())
                    grvApprvLineSub.UpdateCurrentRow();

                grvApprvLineSub.BestFitColumns();
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 그리드 행 변경시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvApprvLine_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle >= 0)
                this.ShowDataApprovalLine(sender, e.FocusedRowHandle);
        }

        /// <summary>
        /// 그리드 행 클릭시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvApprvLine_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            this.ShowDataApprovalLine(sender, e.RowHandle);
        }

        /// <summary>
        /// 기존 사원 삭제
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvApprvLineSub_DoubleClick(object sender, EventArgs e)
        {
            var view = sender as GridView;
            var info = view.CalcHitInfo(view.GridControl.PointToClient(Control.MousePosition));

            if ((info.InRow || info.InRowCell) && info.RowHandle >= 0)
                view.DeleteRow(info.RowHandle);
        }

        #endregion

        #region 사용자 정의 메서드
        /// <summary>
        /// 결재선 그리드
        /// </summary>
        private void FillGridApprovalLine()
        {
            try
            {
                var startTime = DateTime.Now;

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetList, SqlDbType.Char);
                inParams.Add("@i_PersonID", AppConfig.Login.PersonID, SqlDbType.VarChar);

                var table = SqlHelper.GetDataTable("xp_ApprovalLine", inParams, out _, out _);

                grcApprvLine.SetData(table);

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
        /// 결재자 그리드
        /// </summary>
        /// <param name="apprvSub"></param>
        private void FillGridApprovalLineSub(int apprvSeq)
        {
            try
            {
                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", "81", SqlDbType.Char);
                inParams.Add("@i_PersonID", AppConfig.Login.PersonID, SqlDbType.VarChar);
                inParams.Add("@i_ApprvSeq", apprvSeq, SqlDbType.TinyInt);

                var table = SqlHelper.GetDataTable("xp_ApprovalLine", inParams, out _, out _);

                grcApprvLineSub.SetData(table);
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
        /// 결재선 정보 화면 표시 및 결재자 그리드 정보 표시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="rowHandle"></param>
        private void ShowDataApprovalLine(object sender, int rowHandle)
        {
            var view = sender as GridView;
            var apprvSeq = view.GetRowCellValue(rowHandle, "ApprvSeq").CShort();

            txtApprvSeq.EditValue = apprvSeq;
            txtApprvTitle.EditValue = view.GetRowCellValue(rowHandle, "ApprvTitle").CString();

            this.FillGridApprovalLineSub(apprvSeq);
        }

        #endregion
    }
}
