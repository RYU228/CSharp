using System;

namespace MasterMES.REPORTS.DESIGN
{
    public partial class RPT_Quotation : DevExpress.XtraReports.UI.XtraReport
    {
        public RPT_Quotation()
        {
            InitializeComponent();
            //xrTableCell200.Text = "1234";
            //xrTableCell200.RowSpan = 7;
        }

        private void RPT_Quotation_AfterPrint(object sender, EventArgs e)
        {
            //xrTableCell62.RowSpan = 2;
        }

        private void DetailReport_AfterPrint(object sender, EventArgs e)
        {
            //xrTableCell62.RowSpan = 3;
        }

        private void Detail1_AfterPrint(object sender, EventArgs e)
        {
            //xrTableCell62.RowSpan = 4;
        }

        private void DetailReport_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //xrTableCell62.Text = "1234";
        }
    }
}