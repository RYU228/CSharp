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
    public partial class frmMasterPlan : frmBase, IButtonAction
    {
        #region 생성자
        public frmMasterPlan()
        {
            InitializeComponent();

            dteSContractMon.SetViewStyle(ViewStyle.MonthView);
            dteEContractMon.SetViewStyle(ViewStyle.MonthView);

            DxControl.ButtonEditEvent += ButtonEditEvent;
            Extensions.ButtonEditEvent += ButtonEditEvent;
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

                var initSet = SqlHelper.GetDataSet("xp_ConfirmOrder", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                tlsConfirmOrder.SetColumns(initSet.Tables[0], autoColumnWidth: true);

                DxControl.ClearControl(lcgSearch);
                DxControl.ClearControl(lcgMasterPlan);
                DxControl.ClearControl(lcgMasterPlanList);

                this.FillTreeConfirmOrder();
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
            this.FillTreeConfirmOrder();
        }

        /// <summary>
        /// 신규 버튼
        /// </summary>
        public void NewButtonClick()
        {
            DxControl.ClearControl(lcgMasterPlan);

            txtPlanName.Focus();
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
                    { "PlanName", txtPlanName.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;
                var logType = ActionLogType.Insert;
                var message = MessageText.OKInsert;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Exist, SqlDbType.Char);
                inParams.Add("@i_ReceiptID", txtPlanName.EditValue.CString(), SqlDbType.Char);

                if (SqlHelper.Exist("xp_ConfirmOrder", inParams, out _, out _))
                {
                    if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionUpdate))
                        return;

                    logType = ActionLogType.Update;
                    message = MessageText.OKUpdate;
                }

                Utils.ShowWaitForm(this);
                MainButton.SaveButton = false;

                //현재 수주확정여부 저장 시 수주확정상태('0')으로 저장
                inParams.Clear();
                inParams.Add("@i_ProcessID", ProcessType.Merge, SqlDbType.Char);
                inParams.Add("@i_ReceiptID", txtReceiptID.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_ContractDate", dteContractDate.EditValue.CDateString());
                inParams.Add("@i_DeliveryDate", dteDeliveryDate.EditValue.CDateString());
                inParams.Add("@i_ContractAmt", txtContractAmt.EditValue.CDecimal(), SqlDbType.Decimal);
                inParams.Add("@i_OrderStatus", "0", SqlDbType.Char);
                inParams.Add("@i_Remark", mmeOrderRemark.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_InptUser", AppConfig.Login.PersonID, SqlDbType.VarChar);

                SqlHelper.Execute("xp_ConfirmOrder", inParams, out string[] retVal, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                //this.FillTreeConfirmOrder();
                //grvReceipt.FindRowByValue("ReceiptID", retVal[0]);
                ShowDataConfirmOrder(retVal[0]);

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
                    { "ReceiptID", txtReceiptID.EditValue.CString() == "" },
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
                inParams.Add("@i_ProjectNo", txtProjectNo.EditValue.CString(), SqlDbType.Char);

                SqlHelper.Execute("xp_ConfirmOrder", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                //this.FillTreeConfirmOrder();
                DxControl.ClearControl(lcgMasterPlan);

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
            //try
            //{
            //    MainButton.PrintButton = false;

            //    var inParams = new DbParamList();
            //    inParams.Add("@i_ProcessID", ProcessType.Print, SqlDbType.Char);
            //    inParams.Add("@i_LanguageCode", "LANKOR", SqlDbType.VarChar);

            //    var reportSource = SqlHelper.GetDataTable("xp_Glossary", inParams, out string retCode, out string retMsg);

            //    // 비정상 코드를 반환하면 오류 처리
            //    if (retCode != "00")
            //        throw new Exception(retMsg);

            //    if (reportIndex == 1)   // 단일 리포트 이면 해당 If 문은 필요하지 않음
            //    {
            //        var condition = $"{"LanguageCode".Localization()} : " + $"{gleLanguageCode.Properties.GetDisplayText(gleLanguageCode.EditValue)}";
            //        var rptPortrait = new REPORTS.DESIGN.RPT_Glossary_Portrait(condition);
            //        rptPortrait.Preview(reportSource, true);
            //    }
            //    else if (reportIndex == 2)
            //    {
            //        var rptLandscape = new REPORTS.DESIGN.RPT_Glossary_Landscape();
            //        rptLandscape.Preview(reportSource, false);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MsgBox.ErrorMessage(ex.CString());
            //}
            //finally
            //{
            //    MainButton.PrintButton = true;
            //}

        }

        /// <summary>
        /// ButtonEdit 공통 이벤트 (FindData 호출)
        /// </summary>
        /// <param name="sender"></param>
        private bool ButtonEditEvent(object sender)
        {
            ButtonEdit bte = sender as ButtonEdit;
            FindDataResult fdResult = FindDataResult.FIND_NONE;

            switch (bte.Name)
            {
                // 검색 컨트롤
                //case "bteSearchCustom":
                //    fdResult = FindData.ShowFindData(FindData.FIND_CUSTOM, bte.EditValue.CString(), null, null, null, false, bte);
                //    break;

                // 입력 컨트롤
                case "bteCustom":
                    fdResult = FindData.ShowFindData(FindData.FIND_CUSTOM, bte.EditValue.CString(), null, null, null, false, bte);
                    //if (bte.Tag.CString() != "")
                    //{
                    //    GetCustomInfo();
                    //}
                    break;

                    //조건문 및 여러 정보를 가져올 경우 예시
                    //FindData.ShowFindData(FindData.FIND_ORDERSUB, bte.EditValue.CString(), "2022060001", null, null, false, bte, bteStuffWidth, bteWorkWidth);
            }

            // 찾기 성공이면 다음 컨트롤로 포커스 이동, 실패면 포커스 이동 중지
            if (fdResult == FindDataResult.FIND_OK)
                return true;
            else
                return false;
        }

        #endregion

        #region 폼 컨트롤 이벤트

        /// <summary>
        /// 첨부파일 업로드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFileUpload_Click(object sender, EventArgs e)
        {
            var field = new Dictionary<string, bool>()
            {
                { "ProjectNo", txtProjectNo.CString() == "" },
            };
            if (Utils.CheckField(field))
                return;

            var menuID = ((FormLog)this.Tag).MenuID;
            var groupName = $"{txtProjectNo.EditValue.CString()}";
            var frmFileDialog = new frmFileDlg(menuID, groupName);

            frmFileDialog.ShowDialog();

            var fileUpDown = new FileUpDown();
            fileUpDown.FillGridAttachFile(this, grcAttachFile, groupName);
        }

        /// <summary>
        /// 첨부파일 삭제
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RiFileDelete_Click(object sender, EventArgs e)
        {
            var groupName = $"{txtProjectNo.EditValue.CString()}";

            var fileUpDown = new FileUpDown();
            fileUpDown.DeleteAttachFile(this, groupName, grvAttachFile.GetFocusedRowCellValue("GroupSeq").CInt());
            fileUpDown.FillGridAttachFile(this, grcAttachFile, groupName);
        }

        /// <summary>
        /// 첨부파일 다운로드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RiFileDownload_Click(object sender, EventArgs e)
        {
            var groupName = $"{txtProjectNo.EditValue.CString()}";

            var fileUpDown = new FileUpDown();
            fileUpDown.DownloadAttachFile(this, groupName, grvAttachFile.GetFocusedRowCellValue("GroupSeq").CInt(),
                                                           grvAttachFile.GetFocusedRowCellValue("FileName").CString());
        }

        /// <summary>
        /// 첨부파일 더블클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvAttachFile_DoubleClick(object sender, EventArgs e)
        {
            var view = sender as GridView;
            var info = view.CalcHitInfo(view.GridControl.PointToClient(Control.MousePosition));

            if ((info.InRow || info.InRowCell) && info.RowHandle >= 0)
            {
                var groupName = $"{txtProjectNo.EditValue.CString()}";

                var fileUpDown = new FileUpDown();
                fileUpDown.DownloadAttachFileOpen(this, groupName, grvAttachFile.GetFocusedRowCellValue("GroupSeq").CInt(),
                                                                   grvAttachFile.GetFocusedRowCellValue("FileName").CString());
            }
        }

        /// <summary>
        /// 활성화 노드 변경시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tlsConfirmOrder_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            this.ShowDataReceipt(e.Node["KeyField"].CString());
            this.ShowDataConfirmOrder(e.Node["KeyField"].CString());
        }

        /// <summary>
        /// 노드 클릭시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tlsConfirmOrder_RowClick(object sender, DevExpress.XtraTreeList.RowClickEventArgs e)
        {
            this.ShowDataReceipt(e.Node["KeyField"].CString());
            this.ShowDataConfirmOrder(e.Node["KeyField"].CString());
        }
        #endregion

        #region 사용자 정의 메서드
        /// <summary>
        /// 접수 리스트 조회
        /// </summary>
        private void FillTreeConfirmOrder()
        {
            DxControl.ClearControl(lcgReceipt);
            DxControl.ClearControl(lcgMasterPlan);
            DxControl.ClearControl(lcgMasterPlanList);

            try
            {
                var startTime = DateTime.Now;

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", "92", SqlDbType.Char);
                inParams.Add("@i_SReceiptMon", dteSContractMon.EditValue.CDateString(), SqlDbType.Date);
                inParams.Add("@i_EReceiptMon", dteEContractMon.EditValue.CDateString(), SqlDbType.Date);
                inParams.Add("@i_CustomID", bteCustom.Tag.CString(), SqlDbType.Char);

                var table = SqlHelper.GetDataTable("xp_ConfirmOrder", inParams, out _, out _);

                tlsConfirmOrder.SetData(table, 1);

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
        /// 접수정보 조회
        /// </summary>
        private void ShowDataReceipt(string receiptID)
        {
            try
            {
                DxControl.ClearControl(lcgReceipt);

                var startTime = DateTime.Now;

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", "91", SqlDbType.Char);
                inParams.Add("@i_ReceiptID", receiptID, SqlDbType.Char);

                var row = SqlHelper.GetDataRow("xp_ConfirmOrder", inParams, out _, out _);

                if (row != null)
                {
                    txtReceiptID.EditValue = row["ReceiptID"];
                    txtCustomCharge.EditValue = row["ChargeName"];
                    txtDutyName.EditValue = row["DutyName"];
                    txtCellPhoneNo.EditValue = row["CellPhoneNo"];
                    txtEMail.EditValue = row["EMail"];
                    txtEquipType.EditValue = row["EquipType"];
                    txtInterest.EditValue = row["Interest"];
                    //txtReceiptState.EditValue = row["ReceiptState"];
                    mmeSpecialNote.EditValue = row["SpecialNote"];
                    mmeRemark.EditValue = row["Remark"];
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
        /// 수주확정정보 조회
        /// </summary>
        private void ShowDataConfirmOrder(string receiptID)
        {
            try
            {
                DxControl.ClearControl(lcgMasterPlan);

                var startTime = DateTime.Now;

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetRow, SqlDbType.Char);
                inParams.Add("@i_ReceiptID", receiptID, SqlDbType.Char);

                var row = SqlHelper.GetDataRow("xp_ConfirmOrder", inParams, out _, out _);

                if (row != null)
                {
                    txtProjectNo.EditValue = row["ProjectNo"];
                    dteContractDate.EditValue = row["ContractDate"];
                    dteDeliveryDate.EditValue = row["DeliveryDate"];
                    txtContractAmt.EditValue = row["ContractAmt"];
                    txtOrderStatus.EditValue = row["OrderStatus"];
                    mmeOrderRemark.EditValue = row["Remark"];
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

            this.FillGridFile();
        }

        /// <summary>
        /// 파일 리스트 조회
        /// </summary>
        private void FillGridFile()
        {
            var groupName = $"{txtProjectNo.EditValue.CString()}";

            var fileUpDown = new FileUpDown();
            fileUpDown.FillGridAttachFile(this, grcAttachFile, groupName);
        }


        #endregion

        
    }
}
