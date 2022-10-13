using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Grid;
using DSLibrary;

namespace MasterMES.FORMS.SYSTEM
{
    public partial class frmGlossary : frmBase, CLASSES.IButtonAction
    {
        #region 생성자
        public frmGlossary()
        {
            InitializeComponent();
        }

        #endregion

        #region 오버라이드 이벤트
        /// <summary>
        /// 화면 활성화시 처리
        /// </summary>
        /// <param name="e"></param>
        //protected override void OnActivated(EventArgs e)
        //{
        //    base.OnActivated(e);

        //    //초기화,신규,저장,삭제,출력 버튼을 활성화
        //    MainButton.ActiveButton("INIT,NEW,SAVE,DELETE,PRINT");
        //}

        /// <summary>
        /// 화면 비활성화시 처리
        /// </summary>
        /// <param name="e"></param>
        //protected override void OnDeactivate(EventArgs e)
        //{
        //    base.OnDeactivate(e);
        //    MainButton.ActiveButton();
        //}

        /// <summary>
        /// 화면이 닫힐때
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            // 해당 폼을 닫을 때 용어정보를 다시 로딩
            Program.GetGlossaryInfo();
        }

        /// <summary>
        /// 폼이 시작화 될때 처리
        /// </summary>
        /// <param name="e"></param>
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            // 셀 가로 병합을 위한 클래스 Instance 생성
            //cellMerge = new MergeCellHelper(grvGlossary);

            // 폼 초기화 설정
            this.InitButtonClick();
        }

        #endregion

        #region 인터페이스 - 공용 버튼 클릭
        /// <summary>
        /// 화면 초기화
        /// </summary>
        public void InitButtonClick()
        {
            try
            {
                // Wait폼 출력 및 초기화버튼을 비활성
                Utils.ShowWaitForm(this);
                MainButton.InitButton = false;

                // 초기화에 필요한 데이터를 조회
                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Initial, SqlDbType.Char);

                // 프로시저 실행
                var initSet = SqlHelper.GetDataSet("xp_Glossary", inParams, out string retCode, out string retMsg);

                // 실행 결과 에러코드가 정상이 아니면 오류 출력
                if (retCode != "00")
                    throw new Exception(retMsg);

                // 폼의 컨트롤 초기화 설정
                gleLanguageCode.AddItem("KOR:한국어|ENG:English|CHN:中文|JPN:にほんご");
                gleHAlignCode.SetData(initSet.Tables[0]);
                gleVAlignCode.SetData(initSet.Tables[1]);
                gleFormatCode.SetData(initSet.Tables[2]);
                grcGlossary.SetColumns(initSet.Tables[3], "UseClss");

                // 화면 입력 컨트롤 값 초기화
                NewButtonClick();

                // 폼 로딩과 동시에 데이터 조회
                ViewGlossaryGrid();

                // 왼쪽 2개 컬럼(0~1)을 고정
                grvGlossary.FrozenColumns(1);

                // 화면 초기화 후 그리드 초기 포맷 저장
                DxControl.SaveGridFormat(grvGlossary, GridFormat.Default);

                // 사용자 정의 그리드 포맷 로딩
                DxControl.LoadGridFormat(grvGlossary, GridFormat.Current);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
            finally
            {
                // 초기화버튼을 활성화하고 Wait폼을 종료합니다.
                MainButton.InitButton = true;
                Utils.CloseWaitForm();
            }
        }

        /// <summary>
        /// 신규 버튼
        /// </summary>
        public void NewButtonClick()
        {
            // 입력 컨트롤 개별로 초기값 설정
            gleLanguageCode.EditValue = "KOR";
            txtGlossaryCode.EditValue = null;
            txtGlossaryName.EditValue = null;
            gleHAlignCode.EditValue = 0;
            gleVAlignCode.EditValue = 0;
            gleFormatCode.EditValue = 0;
            txtFormatText.EditValue = null;
            chkUseClss.EditValue = "0";

            gleLanguageCode.Focus();
        }

        /// <summary>
        /// 저장 버튼
        /// </summary>
        public void SaveButtonClick()
        {
            try
            {
                // 필수 입력 필드값 체크
                var field = new Dictionary<string, bool>()
                {
                    { "LanguageCode", gleLanguageCode.EditValue.CString() == "" },
                    { "GlossaryCode", txtGlossaryCode.EditValue.CString() == "" },
                    { "GlossaryName", txtGlossaryName.EditValue.CString() == "" },
                    { "HAlignCode", gleHAlignCode.EditValue.CString() == "" },
                    { "VAlignCode", gleVAlignCode.EditValue.CString() == "" },
                    { "FormatCode", gleFormatCode.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                // 버튼 클릭 시작 시각
                var startTime = DateTime.Now;

                // 기본 처리 processId를 입력으로 설정(Stored Procedure에서 Merge로 작성한 경우는 불필요)
                var processId = ProcessType.Insert;

                // 기본 처리 LogType을 입력으로 설정
                var logType = ActionLogType.Insert;

                // 완료 메세지 기본값을 입력으로 설정
                var message = MessageText.OKInsert;

                // 키값에 해당하는 데이터가 존재하는지 체크
                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Exist, SqlDbType.Char);
                inParams.Add("@i_LanguageCode", gleLanguageCode.EditValue.CString());
                inParams.Add("@i_GlossaryCode", txtGlossaryCode.EditValue.CString());

                if (SqlHelper.Exist("xp_Glossary", inParams, out _, out _))
                {
                    // 키값에 해당하는 데이터가 존재하면 수정 확인 메세지를 출력
                    if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionUpdate))
                        return;

                    // 업데이트일 경우 처리 processId, logType, message를 수정으로 변경
                    processId = ProcessType.Update;
                    logType = ActionLogType.Update;
                    message = MessageText.OKUpdate;
                }

                // Wait폼 출력 및 저장버튼을 비활성
                Utils.ShowWaitForm(this);
                MainButton.SaveButton = false;

                // 데이터 저장용 파라메터를 생성
                inParams = new DbParamList();
                inParams.Add("@i_ProcessID", processId, SqlDbType.Char);
                inParams.Add("@i_LanguageCode", gleLanguageCode.EditValue.CString(), SqlDbType.VarChar);
                inParams.Add("@i_GlossaryCode", txtGlossaryCode.EditValue.CString(), SqlDbType.VarChar);
                inParams.Add("@i_GlossaryName", txtGlossaryName.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_HAlignCode", gleHAlignCode.EditValue.CString(), SqlDbType.TinyInt);
                inParams.Add("@i_VAlignCode", gleVAlignCode.EditValue.CString(), SqlDbType.TinyInt);
                inParams.Add("@i_FormatCode", gleFormatCode.EditValue.CString(), SqlDbType.TinyInt);
                inParams.Add("@i_FormatText", txtFormatText.EditValue.CString(), SqlDbType.VarChar);
                inParams.Add("@i_UseClss", chkUseClss.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_InptUser", AppConfig.Login.PersonID, SqlDbType.VarChar);

                // 데이터를 저장 (processId에 따라 저장 또는 수정)
                SqlHelper.Execute("xp_Glossary", inParams, out string retCode, out string retMsg);

                // 반환코드 검증 : 00 - 정상, 그외 - 오류
                if (retCode != "00")
                    throw new Exception(retMsg);

                // 행을 찾기 위한 키값을 저장
                var languageCode = gleLanguageCode.EditValue.CString();
                var glossaryCode = txtGlossaryCode.EditValue.CString();

                // 화면 초기화
                this.NewButtonClick();

                // 정상적으로 저장된 경우 그리드를 재조회
                this.ViewGlossaryGrid();

                // 저장한 행 찾기
                grvGlossary.FindRowByValue(new Dictionary<string, object>()
                {
                    { "LanguageCode", languageCode },
                    { "GlossaryCode", glossaryCode },
                });

                // Insert 또는 Update 로그를 기록
                SqlHelper.WriteLog(this, logType, startTime);

                // 성공(입력 또는 수정) 메세지 출력
                MsgBox.OKMessage(message);
            }
            catch (Exception ex)
            {
                // 에러발생시 에러메세지를 출력
                MsgBox.ErrorMessage(ex.CString());
            }
            finally
            {
                // 저장버튼을 활성화 및 Wait폼을 종료
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
                // 필수 입력 필드값 체크
                var field = new Dictionary<string, bool>()
                {
                    { "LanguageCode", gleLanguageCode.EditValue.CString() == "" },
                    { "GlossaryCode", txtGlossaryCode.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                // 삭제 확인 메세지 출력
                if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionDelete))
                    return;

                var startTime = DateTime.Now;

                // Wait폼 출력 및 삭제버튼을 비활성화
                Utils.ShowWaitForm(this);
                MainButton.DeleteButton = false;

                // 삭제 조건에 해당한 파라메터 생성
                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Delete, SqlDbType.Char);
                inParams.Add("@i_Languagecode", gleLanguageCode.EditValue.CString());
                inParams.Add("@i_Glossarycode", txtGlossaryCode.EditValue.CString());

                // 데이터를 삭제
                SqlHelper.Execute("xp_Glossary", inParams, out string retCode, out string retMsg);

                // 반환코드 검증 : 00 - 정상, 그외 - 오류
                if (retCode != "00")
                    throw new Exception(retMsg);

                this.NewButtonClick();
                this.ViewGlossaryGrid();

                // 로그를 기록
                SqlHelper.WriteLog(this, ActionLogType.Delete, startTime);
                MsgBox.OKMessage(MessageText.OKDelete);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
            finally
            {
                // 삭제버튼을 활성화하고 Wait폼을 종료
                MainButton.DeleteButton = true;
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
        /// 출력 버튼
        /// </summary>
        public void PrintButtonClick(int reportIndex)
        {
            try
            {
                MainButton.PrintButton = false;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Print, SqlDbType.Char);
                inParams.Add("@i_LanguageCode", AppConfig.Login.Language, SqlDbType.VarChar);

                var reportSource = SqlHelper.GetDataTable("xp_Glossary", inParams, out string retCode, out string retMsg);

                // 비정상 코드를 반환하면 오류 처리
                if (retCode != "00")
                    throw new Exception(retMsg);

                if (reportIndex == 1)
                {
                    var condition = $"{"LanguageCode".Localization()} : {gleLanguageCode.Properties.GetDisplayText(gleLanguageCode.EditValue)}";
                    var rptPortrait = new REPORTS.DESIGN.RPT_Glossary_Portrait(condition);
                    rptPortrait.Preview(reportSource, true);
                }
                else if (reportIndex == 2)
                {
                    var rptLandscape = new REPORTS.DESIGN.RPT_Glossary_Landscape();
                    rptLandscape.Preview(reportSource, false);
                }
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

        /// <summary>
        /// 복사 버튼
        /// </summary>
        public void CopyButtonClick()
        {
        }

        #endregion

        #region 폼 컨트롤 이벤트
        /// <summary>
        /// 행이 변경될때 용어정보 화면 표시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvGlossary_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle >= 0)
                this.ViewGlossaryRow();
        }

        private void grvGlossary_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            this.ViewGlossaryRow();
        }

        /// <summary>
        /// 가로 병합 예제
        /// HAlignName, VAlignName 컬럼의 값이 같으면 HAlignName 컬럼일 경우는 셀을 병합, VAlignName 일 경우는 셀 정렬을 가운데로 (HAlignName 컬럼에서 정렬하면 적용 안됨)
        /// CustomColumnDisplayText 이벤트에서는 셀의 정렬을 변경할 수 없어 CustomDrawCell 이벤트에서 구현
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void grvGlossary_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        //{
            //if (e.RowHandle >= 0)
            //{
            //    if (e.Column.FieldName == "HAlignName" || e.Column.FieldName == "VAlignName")
            //    {
            //        var view = sender as GridView;

            //        var val1 = view.GetRowCellValue(e.RowHandle, "HAlignName").CString();
            //        var val2 = view.GetRowCellValue(e.RowHandle, "VAlignName").CString();

            //        if (val1.Equals(val2))
            //        {
            //            if (e.Column.FieldName == "HAlignName")
            //                cellMerge.AddMergeCell(e.RowHandle, 4, 5, val1);
            //            else
            //                e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            //        }
            //    }
            //}
        //}

        /// <summary>
        /// GridView CellMerge
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void grvGlossary_CellMerge(object sender, DevExpress.XtraGrid.Views.Grid.CellMergeEventArgs e)
        //{
        //    //GridView view = sender as GridView;

        //    //if (new string[] { "LanguageCode", "LanguageName" }.Contains(e.Column.FieldName))
        //    //{
        //    //    var val1 = view.GetRowCellDisplayText(e.RowHandle1, "LanguageCode");
        //    //    var val2 = view.GetRowCellDisplayText(e.RowHandle2, "LanguageCode");
        //    //    e.Merge = (val1 == val2);
        //    //}
        //    //else
        //    //{
        //    //    e.Column.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
        //    //}

        //    //e.Handled = true;
        //}

        #endregion

        #region 사용자 정의 메서드
        /// <summary>
        /// 용어정보 조회 : Grid
        /// </summary>
        private void ViewGlossaryGrid()
        {
            try
            {
                var startTime = DateTime.Now;

                // Wait폼을 호출
                Utils.ShowWaitForm(this);

                // 조회조건 파라메터를 생성(그리드 조회용)
                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetList, SqlDbType.Char);

                // 데이터 조회
                var table = SqlHelper.GetDataTable("xp_Glossary", inParams, out _, out _);

                // 그리드에 데이터를 표시
                grcGlossary.SetData(table);

                // 조회 Log 기록
                SqlHelper.WriteLog(this, ActionLogType.Select, startTime);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
            finally
            {
                // Wait 폼을 종료
                Utils.CloseWaitForm();
            }
        }

        /// <summary>
        /// 용어정보 조회 : Row
        /// </summary>
        private void ViewGlossaryRow()
        {
            try
            {
                // Wait폼을 호출
                Utils.ShowWaitForm(this);

                // 조회조건 파라메터를 생성합니다.(단일행 조회용)
                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetRow, SqlDbType.Char);
                inParams.Add("@i_LanguageCode", grvGlossary.GetFocusedRowCellValue("LanguageCode").CString());
                inParams.Add("@i_GlossaryCode", grvGlossary.GetFocusedRowCellValue("GlossaryCode").CString());

                // 데이터를 조회합니다.
                var row = SqlHelper.GetDataRow("xp_Glossary", inParams, out _, out _);

                // 컨트롤에 데이터를 표시
                if (row != null)
                {
                    gleLanguageCode.EditValue = row["LanguageCode"];
                    txtGlossaryCode.EditValue = row["GlossaryCode"];
                    txtGlossaryName.EditValue = row["GlossaryName"];
                    gleHAlignCode.EditValue = row["HAlignCode"];
                    gleVAlignCode.EditValue = row["VAlignCode"];
                    gleFormatCode.EditValue = row["FormatCode"];
                    txtFormatText.EditValue = row["FormatText"];
                    chkUseClss.EditValue = row["UseClss"];
                }
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
            finally
            {
                // Wait 폼을 종료
                Utils.CloseWaitForm();
            }
        }
        #endregion
    }
}