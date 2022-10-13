using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using DevExpress.XtraPrinting.Preview;
using DevExpress.XtraReports.UI;
using DSLibrary;

namespace MasterMES.REPORTS.DESIGN
{
    public partial class RPT_Glossary_Portrait : DevExpress.XtraReports.UI.XtraReport
    {
        public RPT_Glossary_Portrait(string condition)
        {
            InitializeComponent();

            // 조회조건 표시 (호출시 지역화해서 넘김)
            xrlblCond01.Text = condition;

            // 리포트 타이틀 지역화
            xrlblTitle.Text = xrlblTitle.Text.Localization();

            // 페이지 헤더 지역화
            for (int i = 0; i < xrTable1.Rows[0].Cells.Count; i++)
                xrTable1.Rows[0].Cells[i].Text = xrTable1.Rows[0].Cells[i].Text.Localization();
        }
    }
}
