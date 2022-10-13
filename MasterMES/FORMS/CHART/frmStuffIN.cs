using System;
using System.Data;
using System.Drawing;
using DevExpress.XtraCharts;
using DSLibrary;

namespace MasterMES.FORMS.CHART
{
    public partial class frmStuffIN : frmBase, CLASSES.IButtonAction
    {
        public frmStuffIN()
        {
            InitializeComponent();
            //layoutControl1.BestFit();
        }

        #region 오버라이드 이벤트
        //protected override void OnActivated(EventArgs e)
        //{
        //    base.OnActivated(e);
        //    MainButton.ActiveButton("INIT,VIEW");
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
        }

        /// <summary>
        /// 조회 버튼
        /// </summary>
        public void ViewButtonClick()
        {
            try
            {
                Utils.ShowWaitForm(this);
                MainButton.ViewButton = false;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetList, SqlDbType.Char);

                DataTable table = SqlHelper.GetDataTable("xp_StuffIN", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                cht2DBar.DataSource = table;
                cht2DBar.PaletteName = "Chameleon";

                // Axis의 제목 설정 -> 속성창에서 설정해도 됨
                ((XYDiagram)cht2DBar.Diagram).AxisX.Title.Text = "월(月)";
                ((XYDiagram)cht2DBar.Diagram).AxisY.Title.Text = "수량";

                // Series1
                cht2DBar.Series[0].SetSeries("MonthOfYear", "InQty", ScaleType.Auto, ScaleType.Numerical, "입고수량", true);
                cht2DBar.Series[0].SetBarViewStyle(FillMode.Gradient, Color.SteelBlue, Color.Beige);

                // Series2
                cht2DBar.Series[1].SetSeries("MonthOfYear", "OutQty", ScaleType.Auto, ScaleType.Numerical, "출고수량", true);
                //cht2DBar.Series[1].SetBarViewStyle(FillMode.Gradient, Color.BlueViolet, Color.Beige);

                // Series3
                cht2DBar.Series[2].SetSeries("MonthOfYear", "IORate", seriesName: "입고:출고");
                cht2DBar.Series[2].SetLineViewStyle(MarkerKind.Diamond, 10, true);

                // Secondary AxisY에 대한 설정 (제목 설정 및 Seies[2]의 AxisY는 SecondaryAxesY로 설정) -> 속성창에서 설정해도 됨
                ((XYDiagram)cht2DBar.Diagram).SecondaryAxesY[0].Title.Text = "입고대비 출고율";
                ((LineSeriesView)cht2DBar.Series[2].View).AxisY = ((XYDiagram)cht2DBar.Diagram).SecondaryAxesY[0];

                // Secondary Aixs Y의 문자 표시 형식
                ((XYDiagram)cht2DBar.Diagram).SecondaryAxesY[0].Label.TextPattern = "{V:0%}";

                // Series[2](Line) Label의 문자 표시 형식
                cht2DBar.Series[2].Label.TextPattern = "{V:0.0%}";

                // 범례 표시 -> 속성창에서 설정해도 됨
                cht2DBar.Legend.Visibility = DevExpress.Utils.DefaultBoolean.True;

                // 두개의 파라메터가 모두 true이면 생략해도 동일한 결과
                cht2DBar.SetCrosshair(true, true);

                DisplayChartControl2(table);
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

        private void DisplayChartControl2(DataTable source)
        {
            chartControl2.DataSource = source;
            chartControl2.PaletteName = "Chameleon";

            chartControl2.Series[0].SetSeries("MonthOfYear", "InQty", ScaleType.Auto, ScaleType.Numerical, "입고수량", true);
            chartControl2.Series[1].SetSeries("MonthOfYear", "OutQty", ScaleType.Auto, ScaleType.Numerical, "출고수량", true);
            chartControl2.Series[2].SetSeries("MonthOfYear", "IORate", seriesName: "입고:출고");
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

        #region 폼 컨트롤 이벤트

        #endregion

        #region 사용자 정의 메서드

        #endregion
    }
}