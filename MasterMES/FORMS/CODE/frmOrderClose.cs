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
using DSLibrary;

namespace DS_MES.FORMS.CODE
{
    public partial class frmOrderClose : frmBase, CLASSES.IButtonAction
    {
        public frmOrderClose()
        {
            InitializeComponent();

            grvCharge.OptionsCustomization.AllowSort = false;
            grvExpress.OptionsCustomization.AllowSort = false;
        }

        #region 오버라이드 이벤트
        //protected override void OnActivated(EventArgs e)
        //{
        //    base.OnActivated(e);
        //    MainButton.ActiveButton("INIT,NEW,SAVE,DELETE");
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

                var initSet = SqlHelper.GetDataSet("xp_Customer", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                gleCustType.SetData(initSet.Tables[0], "UseClss");
                gleLossType.SetData(initSet.Tables[1], "UseClss");
                gleUsageType.SetData(initSet.Tables[2], "UseClss");
                gleWorkType.SetData(initSet.Tables[3], "UseClss");
                gleConvertType.SetData(initSet.Tables[4], "UseClss");
                glePointType.SetData(initSet.Tables[5], "UseClss");
                gleTaxType.SetData(initSet.Tables[6], "UseClss");
                grcCustomer.SetColumns(initSet.Tables[7], "UseClss");
                grcCharge.SetColumns(initSet.Tables[8], "UseClss", false, false);
                grcExpress.SetColumns(initSet.Tables[9], "UseClss", false, false);

                // 담당자 삭제 버튼
                var riChargeDelete = grvCharge.SetRepositoryItemSimpleButton("DeleteButton", RepositoryButtonType.Delete);
                riChargeDelete.Click += RiChargeDelete_Click;

                // 화물 삭제 버튼
                var riExpressDelete = grvExpress.SetRepositoryItemSimpleButton("DeleteButton", RepositoryButtonType.Delete);
                riExpressDelete.Click += RiExpressDelete_Click;

                this.NewButtonClick();
                this.ViewCustomerGrid();

                DxControl.SaveGridFormat(grvCustomer, GridFormat.Default);
                DxControl.SaveGridFormat(grvCharge, GridFormat.Default);
                DxControl.SaveGridFormat(grvExpress, GridFormat.Default);

                DxControl.LoadGridFormat(grvCustomer, GridFormat.Current);
                DxControl.LoadGridFormat(grvCharge, GridFormat.Current);
                DxControl.LoadGridFormat(grvExpress, GridFormat.Current);
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
        /// <summary>
        /// 그리드 행변경시 화면에 정보 표시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvCustomer_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle >= 0)
            {
                var custCode = (sender as GridView).GetFocusedRowCellValue("CustCode").CString();
                this.ViewCustomerRow(custCode);
                this.ViewCustomerChargeGrid();
                this.ViewCustomerExpressGrid();
            }
        }

        private void grvCustomer_RowClick(object sender, RowClickEventArgs e)
        {
            var custCode = (sender as GridView).GetFocusedRowCellValue("CustCode").CString();
            this.ViewCustomerRow(custCode);
            this.ViewCustomerChargeGrid();
            this.ViewCustomerExpressGrid();
        }

        /// <summary>
        /// 담당자 삭제 버튼 : Row를 삭제함
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RiChargeDelete_Click(object sender, EventArgs e)
        {
            grvCharge.DeleteRow(grvCharge.FocusedRowHandle);

            //try
            //{
            //    var field = new Dictionary<string, bool>()
            //    {
            //        { "CustCode", txtCustCode.EditValue.CString() == "" },
            //    };
            //    if (Utils.CheckField(field))
            //        return;

            //    if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionDelete))
            //        return;

            //    Utils.ShowWaitForm(this);

            //    var inParams = new DbParameterList();
            //    inParams.Add("@i_ProcessID", ProcessType.Delete, SqlDbType.Char);
            //    inParams.Add("@i_CustCode", txtCustCode.EditValue.CString(), SqlDbType.Char);
            //    inParams.Add("@i_ChargeSeq", grvCharge.GetFocusedRowCellValue("ChargeSeq").CInt(), SqlDbType.Int);

            //    SqlHelper.Execute("xp_CustomerCharge", inParams, out string retCode, out string retMsg);

            //    if (retCode != "00")
            //        throw new Exception(retMsg);

            //    this.ViewCustomerChargeGrid();
            //    MsgBox.OKMessage(MessageText.OKDelete);
            //}
            //catch (Exception ex)
            //{
            //    MsgBox.ErrorMessage(ex.CString());
            //}
            //finally
            //{
            //    Utils.CloseWaitForm();
            //}
        }

        /// <summary>
        /// 화물 삭제 버튼 : Row 삭제
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RiExpressDelete_Click(object sender, EventArgs e)
        {
            grvExpress.DeleteRow(grvExpress.FocusedRowHandle);

            //try
            //{
            //    var field = new Dictionary<string, bool>()
            //    {
            //        { "CustCode", txtCustCode.EditValue.CString() == "" },
            //    };
            //    if (Utils.CheckField(field))
            //        return;

            //    if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionDelete))
            //        return;

            //    Utils.ShowWaitForm(this);

            //    var inParams = new DbParameterList();
            //    inParams.Add("@i_ProcessID", ProcessType.Delete, SqlDbType.Char);
            //    inParams.Add("@i_CustCode", txtCustCode.EditValue.CString(), SqlDbType.Char);
            //    inParams.Add("@i_ExpressSeq", grvExpress.GetFocusedRowCellValue("ExpressSeq").CInt(), SqlDbType.Int);

            //    SqlHelper.Execute("xp_CustomerExpress", inParams, out string retCode, out string retMsg);

            //    if (retCode != "00")
            //        throw new Exception(retMsg);

            //    this.ViewCustomerExpressGrid();
            //    MsgBox.OKMessage(MessageText.OKDelete);
            //}
            //catch (Exception ex)
            //{
            //    MsgBox.ErrorMessage(ex.CString());
            //}
            //finally
            //{
            //    Utils.CloseWaitForm();
            //}
        }

        /// <summary>
        /// 담당자 그리드 행추가
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddRowCharge_Click(object sender, EventArgs e)
        {
            grvCharge.AddNewRow();
            grvCharge.BestFitColumns();
            grvCharge.UpdateCurrentRow();
        }

        /// <summary>
        /// 담당자 그리드 행삭제
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelRowCharge_Click(object sender, EventArgs e)
        {
            grvCharge.DeleteRow(grvCharge.FocusedRowHandle);
        }

        /// <summary>
        /// 화물 그리드 행추가
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddRowExpress_Click(object sender, EventArgs e)
        {
            grvExpress.AddNewRow();
            grvExpress.BestFitColumns();
            grvExpress.UpdateCurrentRow();
        }

        /// <summary>
        /// 화물 그리드 행삭제
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelRowExpress_Click(object sender, EventArgs e)
        {
            grvExpress.DeleteRow(grvExpress.FocusedRowHandle);
        }

        #endregion

        #region 사용자 정의 메서드
        /// <summary>
        /// 거래처 조회 : Grid
        /// </summary>
        private void ViewCustomerGrid()
        {
            try
            {
                var startTime = DateTime.Now;

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetList, SqlDbType.Char);

                var table = SqlHelper.GetDataTable("xp_Customer", inParams, out _, out _);

                grcCustomer.SetData(table);

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
        /// 거래처 조회 : Row  -> 복사시 생성된 거래처를 화면에 표시하기 위해서 custCode를 Parameter로 받는다.
        /// </summary>
        private void ViewCustomerRow(string custCode)
        {
            try
            {
                DxControl.ClearControl(lcgCustomer);

                var startTime = DateTime.Now;

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetRow, SqlDbType.Char);
                inParams.Add("@i_CustCode", custCode, SqlDbType.Char);

                var row = SqlHelper.GetDataRow("xp_Customer", inParams, out _, out _);

                if (row != null)
                {
                    txtCustCode.EditValue = row["CustCode"];
                    txtCustName.EditValue = row["CustName"];
                    txtCustNameEng.EditValue = row["CustNameEng"];
                    txtCustNameShort.EditValue = row["CustNameShort"];
                    txtCeoName.EditValue = row["CeoName"];
                    txtBizNo.EditValue = row["BizNo"];
                    txtCondition.EditValue = row["Condition"];
                    txtCategory.EditValue = row["Category"];
                    gleCustType.EditValue = row["CustType"];
                    txtZipCode.EditValue = row["ZipCode"];
                    txtAddress1.EditValue = row["Address1"];
                    txtAddress2.EditValue = row["Address2"];
                    txtRepPhoneNo.EditValue = row["RepPhoneNo"];
                    txtPhoneNo.EditValue = row["PhoneNo"];
                    txtFaxNo.EditValue = row["FaxNo"];
                    txtHomepage.EditValue = row["Homepage"];
                    txtEMail.EditValue = row["EMail"];
                    gleLossType.EditValue = row["LossType"];
                    gleUsageType.EditValue = row["UsageType"];
                    gleWorkType.EditValue = row["WorkType"];
                    gleConvertType.EditValue = row["ConvertType"];
                    glePointType.EditValue = row["PointType"];
                    gleTaxType.EditValue = row["TaxType"];
                    chkUseClss.EditValue = row["UseClss"];
                    txtRemark.EditValue = row["Remark"];
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

        /// <summary>
        /// 거래처 담당자 조회 : Grid
        /// </summary>
        private void ViewCustomerChargeGrid()
        {
            try
            {
                var inParams = new DbParamList();
                inParams.Add("@i_processID", ProcessType.GetList, SqlDbType.Char);
                inParams.Add("@i_CustCode", txtCustCode.EditValue.CString(), SqlDbType.Char);

                var table = SqlHelper.GetDataTable("xp_CustomerCharge", inParams, out _, out _);
                grcCharge.SetData(table);
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
        /// 화물 조회 : Grid
        /// </summary>
        private void ViewCustomerExpressGrid()
        {
            try
            {
                var inParams = new DbParamList();
                inParams.Add("@i_processID", ProcessType.GetList, SqlDbType.Char);
                inParams.Add("@i_CustCode", txtCustCode.EditValue.CString(), SqlDbType.Char);

                var table = SqlHelper.GetDataTable("xp_CustomerExpress", inParams, out _, out _);
                grcExpress.SetData(table);
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