using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGantt;
using DevExpress.XtraGantt.Exceptions;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTreeList.Columns;
using DSLibrary;
using MasterMES.CLASSES;
using MasterMES.FORMS.ETC;

//https://icodebroker.tistory.com/7238 참고

namespace MasterMES.FORMS.DEVELOPMENT
{
    public partial class frmProjectPlanList : frmBase, IButtonAction
    {
        /// <summary>
        /// 스트립 라인 시작일
        /// </summary>
        private DateTime stripLineStartDate = DateTime.Now.Date.AddDays(-10);

        /// <summary>
        /// 스트립 라인 종료일
        /// </summary>
        private DateTime stripLineEndDate = DateTime.Now.Date.AddDays(-5);

        /// <summary>
        /// 스트립 라인 색상
        /// </summary>
        //private Color striplineColor = Color.FromArgb(64, 255, 224, 166);
        private Color striplineColor = Color.FromArgb(64, 255, 255, 255);

        #region 생성자
        public frmProjectPlanList()
        {
            InitializeComponent();

            
        }

        #endregion

        #region 오버라이드 이벤트
        /// <summary>
        /// Form Shown
        /// </summary>
        /// <param name="e"></param>
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

                var initSet = SqlHelper.GetDataSet("xp_ProjectPlan", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                grcConfirmOrder.SetColumns(initSet.Tables[0], autoColumnWidth: true);

                DxControl.ClearControl(lcgSearch);
                DxControl.ClearControl(lcgProjectPlanList);

                DxControl.SaveGridFormat(grvConfirmOrder, GridFormat.Default);

                this.FillGridConfirmOrder();
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
            this.FillGridConfirmOrder();
        }

        /// <summary>
        /// 신규 버튼
        /// </summary>
        public void NewButtonClick()
        {

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

        #region 간트 컨트롤 태스크 커스텀 그리기 - ganttControl_CustomDrawTask(sender, e)

        /// <summary>
        /// 간트 컨트롤 태스크 커스텀 그리기
        /// </summary>
        /// <param name="sender">이벤트 발생자</param>
        /// <param name="e">이벤트 인자</param>
        private void ganttControl_CustomDrawTask(object sender, CustomDrawTaskEventArgs e)
        {
            e.Appearance.BackColor = Color.White;
            //e.Appearance.ProgressColor = Color.Transparent;
        }

        #endregion

        /// <summary>
        /// 컨트롤 설정하기
        /// </summary>
        /// <param name="control">간트 컨트롤</param>
        /// <param name="autoWidth">너비 자동 설정 여부</param>
        /// <param name="splitterPosition">스플리터 위치</param>
        /// <param name="startDate">시작일</param>
        /// <param name="endDate">종료일</param>
        /// <param name="milestoneFont">마일스톤 폰트</param>
        /// <param name="summaryTaskFont">요약 태스크 폰트</param>
        /// <param name="taskFont">태스크 폰트</param>
        private void SetControl
        (
            GanttControl control,
            bool autoWidth,
            int splitterPosition,
            DateTime startDate,
            DateTime endDate
        )
        {
            control.OptionsView.ShowCaption = true;
            control.OptionsView.AutoWidth = autoWidth;
            control.OptionsView.ShowHorzLines = false;
            control.OptionsView.ShowIndicator = false;
            control.OptionsView.ShowFirstLines = false;

            control.SplitterPosition = splitterPosition;

            control.ChartStartDate = startDate;
            control.ChartFinishDate = endDate;

            //control.Appearance.Milestone.Font = milestoneFont;
            //control.Appearance.SummaryTask.Font = summaryTaskFont;
            //control.Appearance.Task.Font = taskFont;
        }

        #region 매핑 설정하기 - SetMapping(control, keyField, parentField, textField, startDateField, endDateField, progressField, predecessorField)

        /// <summary>
        /// 매핑 설정하기
        /// </summary>
        /// <param name="control">컨트롤</param>
        /// <param name="keyField">키 필드</param>
        /// <param name="parentField">부모 필드</param>
        /// <param name="textField">텍스트 필드</param>
        /// <param name="startDateField">시작일 필드</param>
        /// <param name="endDateField">종료일 필드</param>
        /// <param name="progressField">진행률 필드</param>
        /// <param name="predecessorField">이전 태스크 필드</param>
        private void SetMapping
        (
            GanttControl control,
            string keyField,
            string startDateField,
            string endDateField,
            string progressField
        )
        {
            control.TreeListMappings.KeyFieldName = keyField;

            control.ChartMappings.StartDateFieldName = startDateField;
            control.ChartMappings.FinishDateFieldName = endDateField;
            //control.ChartMappings.ProgressFieldName = progressField;
        }

        #endregion
        #region 컬럼 추가하기 - AddColumn(control, name, fieldName, caption, width, visible)

        /// <summary>
        /// 컬럼 추가하기
        /// </summary>
        /// <param name="control">간트 컨트롤</param>
        /// <param name="name">명칭</param>
        /// <param name="fieldName">필드명</param>
        /// <param name="caption">제목</param>
        /// <param name="width">너비</param>
        /// <param name="visible">표시 여부</param>
        /// <returns>인덱스</returns>
        private int AddColumn(GanttControl control, string name, string fieldName, string caption, int width, bool visible)
        {
            TreeListColumn column = new TreeListColumn();

            column.Name = name;
            column.FieldName = fieldName;
            column.Caption = caption;
            column.Width = width;
            column.Visible = visible;

            int index = control.Columns.Add(column);

            return index;
        }

        #endregion
        #region 예외 추가하기 - AddException(control, year, month, day)

        /// <summary>
        /// 예외 추가하기
        /// </summary>
        /// <param name="control">컨트롤</param>
        /// <param name="year">연도</param>
        /// <param name="month">월</param>
        /// <param name="day">일</param>
        /// <returns>처리 결과</returns>
        private bool AddException(GanttControl control, int year, int month, int day)
        {
            YearlyExceptionRule rule = new YearlyExceptionRule();

            rule.DayOfMonth = day;
            rule.Month = (DevExpress.XtraGantt.Scheduling.Month)month;
            rule.StartDate = new DateTime(year, month, day);
            rule.Occurrences = 1;

            bool result = gtcProjectPlan.Exceptions.Add(rule);

            return result;
        }

        #endregion

        #region 폼 컨트롤 이벤트

        /// <summary>
        /// 행 클릭시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvComfirmOrder_RowClick(object sender, RowClickEventArgs e)
        {
            var projectNo = (sender as GridView).GetFocusedRowCellValue("ProjectNo").CString().Replace("-", "");
            this.FillGanttProjectPlan(projectNo);
        }

        /// <summary>
        /// 활성화 행 변경시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvComfirmOrder_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle >= 0)
            {
                var projectNo = (sender as GridView).GetFocusedRowCellValue("ProjectNo").CString().Replace("-", "");
                this.FillGanttProjectPlan(projectNo);
            }
        }
        #endregion

        #region 사용자 정의 메서드

        /// <summary>
        /// 수주확정 리스트 조회
        /// </summary>
        private void FillGridConfirmOrder()
        {
            try
            {
                var startTime = DateTime.Now;

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetList, SqlDbType.Char);
                inParams.Add("@i_PlanStartDate", dteSContractMon.EditValue.CDateString(), SqlDbType.Date);
                inParams.Add("@i_PlanEndDate", dteEContractMon.EditValue.CDateString(), SqlDbType.Date);

                var table = SqlHelper.GetDataTable("xp_ProjectPlan", inParams, out _, out _);

                grcConfirmOrder.SetData(table);

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
        /// 프로젝트 플랜 조회
        /// </summary>
        private void FillGanttProjectPlan(string projectNo)
        {
            try
            {
                Utils.ShowWaitForm(this);

                var startTime = DateTime.Now;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", "92", SqlDbType.Char);
                inParams.Add("@i_ProjectNo", projectNo, SqlDbType.Char);

                var row = SqlHelper.GetDataRow("xp_ProjectPlan", inParams, out _, out _);

                if (row != null && row["PlanStartDate"].LengthB() != 0)
                {
                    //SetControl(this.gtcProjectPlan, false, 550, new DateTime(2022, 9, 1), new DateTime(2022, 9, 30));
                    gtcProjectPlan.Columns.Clear();

                    SetControl(this.gtcProjectPlan, false, 550, Convert.ToDateTime(row["PlanStartDate"]), Convert.ToDateTime(row["PlanEndDate"]));

                    AddColumn(this.gtcProjectPlan, "WorkName", "WorkName", "업무명", 100, true);
                    AddColumn(this.gtcProjectPlan, "ChargeName", "ChargeName", "담당자", 80, true);
                    AddColumn(this.gtcProjectPlan, "WorkDetails", "WorkDetails", "업무내용", 120, true);
                    AddColumn(this.gtcProjectPlan, "WorkRemark", "WorkRemark", "비고", 150, true);
                    AddColumn(this.gtcProjectPlan, "StartDate", "StartDate", "시작일", 100, true);
                    AddColumn(this.gtcProjectPlan, "EndDate", "EndDate", "종료일", 100, true);
                    AddColumn(this.gtcProjectPlan, "Progress2", "Progress2", "진행률", 50, true);

                    //AddException(this.ganttControl1, 2022, 9, 16);

                    SetMapping(this.gtcProjectPlan, "ID", "StartDate", "EndDate", "Progress");

                    this.gtcProjectPlan.CustomDrawTask += ganttControl_CustomDrawTask;

                    inParams.Clear();
                    inParams.Add("@i_ProcessID", "82", SqlDbType.Char);
                    inParams.Add("@i_ProjectNo", projectNo, SqlDbType.Char);

                    var table = SqlHelper.GetDataTable("xp_ProjectPlan", inParams, out _, out _);

                    this.gtcProjectPlan.DataSource = Task.GetList(table);

                    SqlHelper.WriteLog(this, ActionLogType.Select, startTime);
                }
                else
                {
                    DxControl.ClearControl(lcgProjectPlanList);
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

            

            //try
            //{
            //    var startTime = DateTime.Now;

            //    var inParams = new DbParamList();
            //    inParams.Add("@i_ProcessID", "82", SqlDbType.Char);
            //    inParams.Add("@i_ProjectNo", projectNo, SqlDbType.Char);

            //    var table = SqlHelper.GetDataTable("xp_ProjectPlan", inParams, out _, out _);

            //    this.gtcProjectPlan.DataSource = Task.GetList(table);

            //    //SqlHelper.WriteLog(this, ActionLogType.Select, startTime);
            //}
            //catch (Exception ex)
            //{
            //    MsgBox.ErrorMessage(ex.CString());
            //}
            //finally
            //{
            //    Utils.CloseWaitForm();
            //}

            this.gtcProjectPlan.ExpandAll();
        }
        #endregion
    }
}