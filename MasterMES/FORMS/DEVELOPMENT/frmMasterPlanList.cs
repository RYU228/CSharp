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

namespace MasterMES.FORMS.DEVELOPMENT
{
    public partial class frmMasterPlanList : frmBase, IButtonAction
    {
        /// <summary>
        /// ID 해시 세트
        /// </summary>
        HashSet<int> idHashSet = new HashSet<int> { 2, 3, 4 };

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
        private Color striplineColor = Color.FromArgb(64, 255, 224, 166);

        /// <summary>
        /// 데드 라인 날짜
        /// </summary>
        private DateTime deadLineDate = new DateTime(2020, 2, 29);

        #region 생성자
        public frmMasterPlanList()
        {
            InitializeComponent();

            SetControl(this.ganttControl1, false, 550, new DateTime(2020, 2, 1), new DateTime(2020, 4, 30));

            AddColumn(this.ganttControl1, "Name", "Name", "업무명", 100, true);
            AddColumn(this.ganttControl1, "Charger", "Charger", "담당자", 80, true);
            AddColumn(this.ganttControl1, "Work", "Work", "업무내용", 120, true);
            AddColumn(this.ganttControl1, "Remark", "Remark", "비고", 150, true);
            AddColumn(this.ganttControl1, "StartDate", "StartDate", "시작일", 100, true);
            AddColumn(this.ganttControl1, "EndDate", "EndDate", "종료일", 100, true);
            AddColumn(this.ganttControl1, "Progress", "Progress", "진행률", 50, true);
            AddColumn(this.ganttControl1, "Progress2", "Progress2", "업무별진척도", 50, true);
            AddColumn(this.ganttControl1, "Progress3", "Progress3", "진척도", 50, true);
            AddColumn(this.ganttControl1, "Progress4", "Progress4", "가중치(부서별)", 50, true);
            AddColumn(this.ganttControl1, "Progress5", "Progress5", "가중치(전체)", 50, true);

            AddException(this.ganttControl1, 2020, 3, 6);

            SetMapping(this.ganttControl1, "ID", "ParentID", "Name", "StartDate", "EndDate", "Progress2", "PrevioudIDList");

            //this.ganttControl1.CustomDrawTask += ganttControl_CustomDrawTask;
            this.ganttControl1.CustomDrawTaskDependency += ganttControl_CustomDrawTaskDependency;
            //this.ganttControl1.CustomTaskDisplayText += ganttControl_CustomTaskDisplayText;
            this.ganttControl1.CustomDrawTimescaleColumn += ganttControl_CustomDrawTimescaleColumn;

            this.ganttControl1.DataSource = Task.GetList();

            this.ganttControl1.ExpandAll();
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

        #region 간트 컨트롤 태스크 커스텀 그리기 - ganttControl_CustomDrawTask(sender, e)

        /// <summary>
        /// 간트 컨트롤 태스크 커스텀 그리기
        /// </summary>
        /// <param name="sender">이벤트 발생자</param>
        /// <param name="e">이벤트 인자</param>
        private void ganttControl_CustomDrawTask(object sender, CustomDrawTaskEventArgs e)
        {
            int id = Convert.ToInt32(e.Node.GetValue("ID"));

            if (this.idHashSet.Contains(id))
            {
                e.Appearance.BackColor = Color.Black;
                e.Appearance.ProgressColor = Color.Red;
            }

            //if (e.Node.Id == 3)
            //{
            //    TimeSpan startOffset = TimeSpan.FromDays(2);
            //    TimeSpan endOffset = TimeSpan.FromDays(2);

            //    e.DrawShape(e.Info.StartDate + startOffset, e.Info.FinishDate - endOffset);

            //    e.DrawRightText();

            //    e.Handled = true;
            //}
        }

        #endregion
        #region 간트 컨트롤 태스크 텍스트 커스텀 표시하기 - ganttControl_CustomTaskDisplayText(sender, e)

        /// <summary>
        /// 간트 컨트롤 태스크 텍스트 커스텀 표시하기
        /// </summary>
        /// <param name="sender">이벤트 발생자</param>
        /// <param name="e">이벤트 인자</param>
        private void ganttControl_CustomTaskDisplayText(object sender, CustomTaskDisplayTextEventArgs e)
        {
            int id = Convert.ToInt32(e.Node.GetValue("ID"));

            if (idHashSet.Contains(id))
            {
                e.RightText = "High priority";
            }
            else
            {
                e.RightText = string.Empty;
                e.LeftText = "Normal priority";
            }
        }

        #endregion
        #region 간트 컨트롤 태스크 의존도 커스텀 그리기 - ganttControl_CustomDrawTaskDependency(sender, e)

        /// <summary>
        /// 간트 컨트롤 태스크 의존도 커스텀 그리기
        /// </summary>
        /// <param name="sender">이벤트 발생자</param>
        /// <param name="e">이벤트 인자</param>
        private void ganttControl_CustomDrawTaskDependency(object sender, CustomDrawTaskDependencyEventArgs e)
        {
            int previousID = Convert.ToInt32(e.PredecessorNode.GetValue("ID"));
            int id = Convert.ToInt32(e.SuccessorNode.GetValue("ID"));

            if (idHashSet.Contains(previousID) && idHashSet.Contains(id))
            {
                e.Appearance.BackColor = Color.Blue;
            }
        }

        #endregion
        #region 간트 컨트롤 타임 스케일 컬럼 커스텀 그리기 - ganttControl_CustomDrawTimescaleColumn(sender, e)

        /// <summary>
        /// 간트 컨트롤 타임 스케일 컬럼 커스텀 그리기
        /// </summary>
        /// <param name="sender">이벤트 발생자</param>
        /// <param name="e">이벤트 인자</param>
        private void ganttControl_CustomDrawTimescaleColumn(object sender, CustomDrawTimescaleColumnEventArgs e)
        {
            GanttTimescaleColumn column = e.Column;

            if (column.StartDate <= this.deadLineDate && column.FinishDate >= this.deadLineDate)
            {
                DrawDeadLine(e);
            }

            DrawStripLine(e);

            e.Handled = true;
        }

        #endregion

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
            string parentField,
            string textField,
            string startDateField,
            string endDateField,
            string progressField,
            string predecessorField
        )
        {
            control.TreeListMappings.KeyFieldName = keyField;
            control.TreeListMappings.ParentFieldName = parentField;
            //control.ChartMappings.TextFieldName = textField;

            control.ChartMappings.StartDateFieldName = startDateField;
            control.ChartMappings.FinishDateFieldName = endDateField;
            //control.ChartMappings.ProgressFieldName = progressField;
            //control.ChartMappings.PredecessorsFieldName = predecessorField;
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

            bool result = ganttControl1.Exceptions.Add(rule);

            return result;
        }

        #endregion

        #region 데드 라인 그리기 - DrawDeadLine(e)

        /// <summary>
        /// 데드 라인 그리기
        /// </summary>
        /// <param name="e">이벤트 인자</param>
        private void DrawDeadLine(CustomDrawTimescaleColumnEventArgs e)
        {
            e.DrawBackground();

            float x = (float)e.GetPosition(this.deadLineDate);

            float width = 4;

            RectangleF rectangle = new RectangleF(x, e.Column.Bounds.Y, width, e.Column.Bounds.Height);

            e.Cache.FillRectangle(Color.Red, rectangle);

            e.DrawHeader();
        }

        #endregion
        #region 스트립 라인 그리기 - DrawStripLine(e)

        /// <summary>
        /// 스트립 라인 그리기
        /// </summary>
        /// <param name="e">이벤트 인자</param>
        private void DrawStripLine(CustomDrawTimescaleColumnEventArgs e)
        {
            float startPoint = (float)Math.Max(e.GetPosition(this.stripLineStartDate), e.Column.Bounds.Left);
            float endPoint = (float)Math.Min(e.GetPosition(this.stripLineEndDate), e.Column.Bounds.Right);

            e.DrawBackground();

            RectangleF rectangle = new RectangleF
            (
                startPoint,
                e.Column.Bounds.Y,
                endPoint - startPoint,
                e.Column.Bounds.Height
            );

            if (rectangle.Width > 0)
            {
                e.Cache.FillRectangle(striplineColor, rectangle);
            }

            e.DrawHeader();
        }

        #endregion
    }
}