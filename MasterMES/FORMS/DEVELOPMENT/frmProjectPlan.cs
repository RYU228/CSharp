using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DSLibrary;
using MasterMES.CLASSES;
using MasterMES.FORMS.ETC;

namespace MasterMES.FORMS.DEVELOPMENT
{
    public partial class frmProjectPlan : frmBase, IButtonAction
    {
        #region 생성자
        public frmProjectPlan()
        {
            InitializeComponent();

            dteSContractMon.SetViewStyle(ViewStyle.MonthView);
            dteEContractMon.SetViewStyle(ViewStyle.MonthView);
        }

        #endregion

        #region 오버라이드 이벤트
        //protected override void OnActivated(EventArgs e)
        //{
        //    base.OnActivated(e);
        //    MainButton.ActiveButton("INIT,NEW,COPY,SAVE,DELETE");
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
            DxControl.ClearControl(lcgSearch);

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
                gleWorkName.SetData(initSet.Tables[1], "UseClss");
                glePerson.SetData(initSet.Tables[2], "UseClss");
                grcPlanList.SetColumns(initSet.Tables[3], "ProjectNo,WorkSeq,PlanType");

                rdgDate.SetGroupItem("P:계획|W:실행");

                DxControl.ClearControl(lcgSearch);
                DxControl.ClearControl(lcgProjectPlan);
                DxControl.ClearControl(lcgProjectPlanList);

                DxControl.SaveGridFormat(grvConfirmOrder, GridFormat.Default);
                DxControl.SaveGridFormat(grvPlanList, GridFormat.Default);

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
            DxControl.ClearControl(lcgProjectPlan);

            gleWorkName.Focus();
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
                    { "ProjectNo", txtProjectNo.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;
                var logType = ActionLogType.Insert;
                var message = MessageText.OKInsert;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Exist, SqlDbType.Char);
                inParams.Add("@i_ProjectNo", txtProjectNo.EditValue.CString().Replace("-", ""), SqlDbType.Char);
                inParams.Add("@i_WorkSeq", txtWorkSeq.EditValue.CInt(), SqlDbType.TinyInt);
                inParams.Add("@i_PlanType", rdgDate.EditValue.CString(), SqlDbType.Char);

                if (SqlHelper.Exist("xp_ProjectPlan", inParams, out _, out _))
                {
                    if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionUpdate))
                        return;

                    logType = ActionLogType.Update;
                    message = MessageText.OKUpdate;
                }

                Utils.ShowWaitForm(this);
                MainButton.SaveButton = false;
                
                inParams.Clear();
                inParams.Add("@i_ProcessID", ProcessType.Merge, SqlDbType.Char);
                inParams.Add("@i_ProjectNo", txtProjectNo.EditValue.CString().Replace("-", ""), SqlDbType.Char);
                inParams.Add("@i_WorkSeq", txtWorkSeq.EditValue.CInt(), SqlDbType.TinyInt);
                inParams.Add("@i_WorkName", gleWorkName.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_PersonID", glePerson.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_WorkDetails", txtWorkDetails.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_WorkRemark", txtRemark.EditValue.CDecimal(), SqlDbType.NVarChar);
                inParams.Add("@i_PlanType", rdgDate.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_PlanStartDate", dtePlanSDate.EditValue.CDateString());
                inParams.Add("@i_PlanEndDate", dtePlanEDate.EditValue.CDateString());
                inParams.Add("@i_WorkRate", txtWorkRate.EditValue.CDecimal(), SqlDbType.Decimal);
                inParams.Add("@i_PartWeighting", txtPartWeighting.EditValue.CDecimal(), SqlDbType.Decimal);
                inParams.Add("@i_AllWeighting", txtAllWeighting.EditValue.CDecimal(), SqlDbType.Decimal);
                inParams.Add("@i_InptUser", AppConfig.Login.PersonID, SqlDbType.VarChar);

                SqlHelper.Execute("xp_ProjectPlan", inParams, out string[] retVal, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                this.FillGridPlanList();
                //grvReceipt.FindRowByValue("ReceiptID", retVal[0]);
                //ShowDataConfirmOrder(retVal[0]);

                SqlHelper.WriteLog(this, logType, startTime);
                MsgBox.OKMessage(message);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
            finally
            {
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
                var field = new Dictionary<string, bool>()
                {
                    { "ProjectNo", txtProjectNo.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;

                if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionDelete))
                    return;

                Utils.ShowWaitForm(this);
                MainButton.SaveButton = false;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Delete, SqlDbType.Char);
                inParams.Add("@i_ProjectNo", txtProjectNo.EditValue.CString().Replace("-", ""), SqlDbType.Char);

                SqlHelper.Execute("xp_ProjectPlan", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                //this.FillGridConfirmOrder();
                DxControl.ClearControl(lcgProjectPlan);

                SqlHelper.WriteLog(this, ActionLogType.Delete, startTime);
                MsgBox.OKMessage(MessageText.OKInsert);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
            finally
            {
                MainButton.SaveButton = true;
                Utils.CloseWaitForm();
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
        /// 활성화 행 변경시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvComfirmOrder_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle >= 0)
            {
                var projectNo = (sender as GridView).GetFocusedRowCellValue("ProjectNo").CString().Replace("-", "");
                this.ShowDataConfirmOrder(projectNo);
                this.FillGridPlanList();
            }
        }

        /// <summary>
        /// 행 클릭시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvComfirmOrder_RowClick(object sender, RowClickEventArgs e)
        {
            var projectNo = (sender as GridView).GetFocusedRowCellValue("ProjectNo").CString().Replace("-", "");
            this.ShowDataConfirmOrder(projectNo);
            this.FillGridPlanList();
        }

        /// <summary>
        /// 활성화 행 변경시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvPlanList_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            var projectNo = (sender as GridView).GetFocusedRowCellValue("ProjectNo").CString().Replace("-", "");
            var workSeq = (sender as GridView).GetFocusedRowCellValue("WorkSeq").CInt();
            var planType = (sender as GridView).GetFocusedRowCellValue("PlanType").CString();
            this.ShowDataMasterPlan(projectNo, workSeq, planType);
        }

        /// <summary>
        /// 행 클릭시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvPlanList_RowClick(object sender, RowClickEventArgs e)
        {
            var projectNo = (sender as GridView).GetFocusedRowCellValue("ProjectNo").CString().Replace("-", "");
            var workSeq = (sender as GridView).GetFocusedRowCellValue("WorkSeq").CInt();
            var planType = (sender as GridView).GetFocusedRowCellValue("PlanType").CString();
            this.ShowDataMasterPlan(projectNo, workSeq, planType);
        }

        #endregion

        #region 사용자 정의 메서드
        /// <summary>
        /// 수주확정 리스트 조회
        /// </summary>
        private void FillGridConfirmOrder()
        {
            //DxControl.ClearControl(lcgSearch);
            DxControl.ClearControl(lcgProjectPlan);
            DxControl.ClearControl(lcgProjectPlanList);

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
        /// 마스터플랜 리스트 조회
        /// </summary>
        private void FillGridPlanList()
        {
            DxControl.ClearControl(lcgProjectPlan);
            DxControl.ClearControl(lcgProjectPlanList);

            try
            {
                var startTime = DateTime.Now;

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", "81", SqlDbType.Char);
                inParams.Add("@i_ProjectNo", txtProjectNo.EditValue.CString().Replace("-", ""), SqlDbType.Char);

                var table = SqlHelper.GetDataTable("xp_ProjectPlan", inParams, out _, out _);

                grcPlanList.SetData(table);

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
        /// 수주확정정보 조회
        /// </summary>
        private void ShowDataConfirmOrder(string projectNo)
        {
            try
            {
                DxControl.ClearControl(lcgConfirmOrder);

                var startTime = DateTime.Now;

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", "91", SqlDbType.Char);
                inParams.Add("@i_ProjectNo", projectNo, SqlDbType.Char);

                var row = SqlHelper.GetDataRow("xp_ProjectPlan", inParams, out _, out _);

                if (row != null)
                {
                    txtProjectNo.EditValue = row["ProjectNo"];
                    dteDeliveryDate.EditValue = row["DeliveryDate"];
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
        /// 마스터플랜 정보 조회
        /// </summary>
        private void ShowDataMasterPlan(string projectNo, int workSeq, string planType)
        {
            try
            {
                DxControl.ClearControl(lcgProjectPlan);

                var startTime = DateTime.Now;

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetRow, SqlDbType.Char);
                inParams.Add("@i_ProjectNo", projectNo, SqlDbType.Char);
                inParams.Add("@i_WorkSeq", workSeq, SqlDbType.TinyInt);
                inParams.Add("@i_PlanType", planType, SqlDbType.Char);

                var row = SqlHelper.GetDataRow("xp_ProjectPlan", inParams, out _, out _);

                if (row != null)
                {
                    txtWorkSeq.EditValue = row["WorkSeq"];
                    gleWorkName.EditValue = row["WorkName"];
                    glePerson.EditValue = row["ChargeName"];
                    txtWorkDetails.EditValue = row["WorkDetails"];
                    txtRemark.EditValue = row["WorkRemark"];
                    rdgDate.EditValue = row["PlanType"];
                    dtePlanSDate.EditValue = row["PlanStartDate"];
                    dtePlanEDate.EditValue = row["PlanEndDate"];
                    txtWorkRate.EditValue = row["WorkRate"];
                    //txtWorkProgress.EditValue = row["WorkProgress"];
                    txtProgress.EditValue = row["Progress"];
                    txtPartWeighting.EditValue = row["PartWeighting"];
                    txtAllWeighting.EditValue = row["AllWeighting"];
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
