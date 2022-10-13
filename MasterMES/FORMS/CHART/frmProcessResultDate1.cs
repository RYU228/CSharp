using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DSLibrary;

namespace MasterMES.FORMS.CHART
{
    public partial class frmProcessResultDate1 : frmBase, CLASSES.IButtonAction
    {
        public frmProcessResultDate1()
        {
            InitializeComponent();
        }

        #region 오버라이드 이벤트
        //protected override void OnActivated(EventArgs e)
        //{
        //    base.OnActivated(e);
        //    MainButton.ActiveButton("INIT,NEW,VIEW");
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
                Utils.ShowWaitForm(this);
                MainButton.InitButton = false;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Initial, SqlDbType.Char);

                var initSet = SqlHelper.GetDataSet("xp_Report_ProcessResultDate", inParams, out string retCode, out string retMsg);
                if (retCode != "00")
                    return;

                gleRoutingCode.SetData(initSet.Tables[0], "", "1");
                sleCustCode.SetData(initSet.Tables[1]);
                sleItemCode.SetData(initSet.Tables[2]);
                grcProcRslt.SetColumns(initSet.Tables[3], "NO1");

                DxControl.SaveGridFormat(grvProcRslt, GridFormat.Default);
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
            try
            {
                var startTime = DateTime.Now;

                Utils.ShowWaitForm(this);
                MainButton.ViewButton = false;

                var inParams = new DbParamList();

                inParams.Add("@i_ProcessID", "81", SqlDbType.Char);
                inParams.Add("@i_StrWorkDate", dteStrWorkDate.EditValue.CDateString("yyyyMMdd"), SqlDbType.Char);
                inParams.Add("@i_EndWorkDate", dteEndWorkDate.EditValue.CDateString("yyyyMMdd"), SqlDbType.Char);
                inParams.Add("@i_CustomID", sleCustCode.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_AriticleID", sleItemCode.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_OrderNo", txtOrderNo.EditValue.CString(), SqlDbType.VarChar);

                var processResultDate = SqlHelper.GetDataTable("xp_Report_ProcessResultDate", inParams, out _, out _);
                grcProcRslt.SetData(processResultDate);

                DxControl.LoadGridFormat(grvProcRslt, GridFormat.Current);  // 데이터 로딩 후 저장된 그리드 형식을 불러옴

                SqlHelper.WriteLog(this, ActionLogType.Select, startTime);
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

        private void grvProcRslt_RowStyle(object sender, RowStyleEventArgs e)
        {
            var view = sender as GridView;

            switch (view.GetRowCellValue(e.RowHandle, "NO1").CString())
            {
                case "2":
                    e.Appearance.FontStyleDelta = FontStyle.Bold;
                    e.Appearance.ForeColor = Color.White;
                    e.Appearance.BackColor = Color.FromArgb(48, 74, 30);
                    break;

                case "3":
                    e.Appearance.FontStyleDelta = FontStyle.Bold;
                    e.Appearance.ForeColor = Color.White;
                    e.Appearance.BackColor = Color.FromArgb(132, 60, 12);
                    break;
            }
        }

        private void grvProcRslt_CellMerge(object sender, CellMergeEventArgs e)
        {
            var view = sender as GridView;

            if (e.Column.FieldName == "WorkDate")
            {
                var val1 = view.GetRowCellDisplayText(e.RowHandle1, "WorkDate");
                var val2 = view.GetRowCellDisplayText(e.RowHandle2, "WorkDate");
                e.Merge = (val1 == val2);
            }
            else
            {
                e.Column.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            }

            e.Handled = true;
        }
    }
}