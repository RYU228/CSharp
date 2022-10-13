using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Grid;
using DSLibrary;

namespace MasterMES.FORMS.CODE
{
    public partial class frmMiddleCode : frmBase, CLASSES.IButtonAction
    {
        #region 생성자
        public frmMiddleCode()
        {
            InitializeComponent();

            txtMiddleCode.SetEditMask(MaskType.RegExpression, @"[a-zA-Z0-9]{1,3}");
        }

        #endregion

        #region 오버라이드 이벤트
        /// <summary>
        /// 폼 활성화
        /// </summary>
        /// <param name="e"></param>
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            MainButton.ActiveButtons("INIT,NEW,SAVE,DELETE");
        }

        /// <summary>
        /// 폼 비활성화
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            MainButton.ActiveButtons();
        }

        /// <summary>
        /// 폼이 보여질 때
        /// </summary>
        /// <param name="e"></param>
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            Utils.ShowWaitForm(this);
            MainButton.InitButton = false;

            this.InitButtonClick();

            MainButton.InitButton = true;
            Utils.CloseWaitForm();
        }


        #endregion

        #region 인터페이스 - 공용버튼 클릭
        /// <summary>
        /// 초기화 버튼
        /// </summary>
        public void InitButtonClick()
        {
            try
            {
                // 입력 컨트롤 초기화
                this.NewButtonClick();

                // 초기화 데이터 조회
                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Initial, SqlDbType.VarChar);

                var initSet = SqlHelper.GetDataSet("xp_MiddleCode", inParams, out string retCode, out string retMsg);
                if (retCode != "00")
                    throw new Exception(retMsg);

                // 데이터 화면 표시
                grcLargeCode.SetColumns(initSet.Tables[0]);     // 그리드 형식 지정 및 데이터 표시
                grcMiddleCode.SetColumns(initSet.Tables[1]);    

                // 그리드 초기 형식 저장
                DxControl.SaveGridFormat(grvLargeCode, GridFormat.Default);
                DxControl.SaveGridFormat(grvMiddleCode, GridFormat.Default);

                // 그리드 컨트롤.SetColumn에서 데이터 까지 표시한 경우는 .SetData를 호출하지 않으므로 별도 그리드 형식을 로딩한다.
                DxControl.LoadGridFormat(grvLargeCode, GridFormat.Current);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 신규 버튼
        /// </summary>
        public void NewButtonClick()
        {
            txtMiddleCode.EditValue = null;
            txtMiddleName.EditValue = null;
            chkUseClss.EditValue = "0";

            txtMiddleCode.Focus();
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
                    { "LargeCode", txtLargeCode.EditValue.CString() == "" },
                    { "MiddleCode", txtMiddleCode.EditValue.CString() == "" },
                    { "MiddleName", txtMiddleName.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;
                var logType = ActionLogType.Insert;
                var message = MessageText.OKInsert;

                // 데이터 존재여부 체크
                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Exist, SqlDbType.VarChar);
                inParams.Add("@i_LargeCode", txtLargeCode.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_MiddleCode", txtMiddleCode.EditValue.CString(), SqlDbType.VarChar);

                if (SqlHelper.Exist("xp_MiddleCode", inParams, out _, out _))
                {
                    if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionUpdate))
                        return;

                    logType = ActionLogType.Update;
                    message = MessageText.OKUpdate;
                }

                // Wait폼 출력 및 버튼 비활성화 ==> 메인 화면에서 처리

                inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Merge, SqlDbType.Char);
                inParams.Add("@i_LargeCode", txtLargeCode.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_MiddleCode", txtMiddleCode.EditValue.CString(), SqlDbType.VarChar);
                inParams.Add("@i_MiddleName", txtMiddleName.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_UserDefVal1", txtUserDefVal1.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_UserDefVal2", txtUserDefVal2.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_UseClss", chkUseClss.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_InptUser", AppConfig.Login.PersonID, SqlDbType.VarChar);

                SqlHelper.Execute("xp_MiddleCode", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                // 행을 찾기위한 키값
                var middleCode = txtMiddleCode.EditValue.CString();

                this.NewButtonClick();
                this.FillGridMiddleCode(txtLargeCode.EditValue.CString());

                grvMiddleCode.FindRowByValue("MiddleCode", middleCode);

                // 로그 기록
                SqlHelper.WriteLog(this, logType, startTime);

                MsgBox.OKMessage(message);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 복사 버튼
        /// </summary>
        public void CopyButtonClick()
        {
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
                    { "LargeCode", txtLargeCode.EditValue.CString() == "" },
                    { "MiddleCode", txtMiddleCode.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionDelete))
                    return;

                // Wait폼 출력 및 버튼 비활성화 ==> 메인 화면에서 처리

                var startTime = DateTime.Now;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Delete, SqlDbType.Char);
                inParams.Add("@i_LargeCode", txtLargeCode.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_MiddleCode", txtMiddleCode.EditValue.CString(), SqlDbType.VarChar);

                SqlHelper.Execute("xp_MiddleCode", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                // 행을 찾기위한 키값
                var middleCode = txtMiddleCode.EditValue.CString();

                this.NewButtonClick();
                this.FillGridMiddleCode(txtLargeCode.EditValue.CString());

                grvMiddleCode.FindRowByValue("MiddleCode", middleCode);

                // 로그 기록
                SqlHelper.WriteLog(this, ActionLogType.Delete, startTime);
                MsgBox.OKMessage(MessageText.OKDelete);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
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
        /// <param name="reportIndex"></param>
        public void PrintButtonClick(int reportIndex)
        {
        }
        #endregion

        #region 폼 컨트롤 이벤트
        /// <summary>
        /// 대분류 코드 - 선택행 변경시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvLargeCode_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle >= 0)
            {
                var view = sender as GridView;

                txtLargeCode.EditValue = view.GetFocusedRowCellValue("LargeCode");
                txtLargeName.EditValue = view.GetFocusedRowCellValue("LargeName");

                // 대분류 코드에 해당하는 중분류 리스트 조회
                this.FillGridMiddleCode(txtLargeCode.EditValue.CString());
            }
        }

        /// <summary>
        /// 대분류 코드 - 행 클릭시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvLargeCode_RowClick(object sender, RowClickEventArgs e)
        {
            var view = sender as GridView;

            txtLargeCode.EditValue = view.GetFocusedRowCellValue("LargeCode");
            txtLargeName.EditValue = view.GetFocusedRowCellValue("LargeName");

            // 대분류 코드에 해당하는 중분류 리스트 조회
            this.FillGridMiddleCode(txtLargeCode.EditValue.CString());
        }

        /// <summary>
        /// 중분류 코드 - 선택행 변경시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvMiddleCode_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle >= 0)
            {
                var view = sender as GridView;
                this.ShowDataMiddleCode(view.GetFocusedRowCellValue("MiddleCode").CString());
            }
        }

        /// <summary>
        /// 중분류 코드 - 행 클릭시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvMiddleCode_RowClick(object sender, RowClickEventArgs e)
        {
            var view = sender as GridView;
            this.ShowDataMiddleCode(view.GetFocusedRowCellValue("MiddleCode").CString());
        }

        #endregion

        #region 사용자 정의 메소드
        /// <summary>
        /// 중분류 조회 - 그리드
        /// </summary>
        private void FillGridMiddleCode(string largeCode)
        {
            try
            {
                var startTime = DateTime.Now;
                Utils.ShowWaitForm(this);

                txtMiddleCode.EditValue = null;
                txtMiddleName.EditValue = null;
                chkUseClss.EditValue = "0";

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetList, SqlDbType.VarChar);
                inParams.Add("@i_LargeCode", largeCode, SqlDbType.Char);

                var table = SqlHelper.GetDataTable("xp_MiddleCode", inParams, out _, out _);
                grcMiddleCode.SetData(table);

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
        /// 중분류 조회 - 행
        /// </summary>
        private void ShowDataMiddleCode(string middleCode)
        {
            try
            {
                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetRow, SqlDbType.VarChar);
                inParams.Add("@i_LargeCode", txtLargeCode.EditValue.CString());
                inParams.Add("@i_MiddleCode", middleCode);

                var row = SqlHelper.GetDataRow("xp_MiddleCode", inParams, out _, out _);

                if (row != null)
                {
                    txtMiddleCode.EditValue = row["MiddleCode"];
                    txtMiddleName.EditValue = row["MiddleName"];
                    txtUserDefVal1.EditValue = row["UserDefVal1"];
                    txtUserDefVal2.EditValue = row["UserDefVal2"];
                    chkUseClss.EditValue = row["UseClss"];
                }
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