using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Grid;
using DSLibrary;
using MasterMES.CLASSES;

namespace MasterMES.FORMS.PO
{
    public partial class frmPurchaseOrder : frmBase, IButtonAction
    {
        #region 생성자
        public frmPurchaseOrder()
        {
            InitializeComponent();

            lcgProjectInfo.SetHeaderButtons("HideList");
        } 

        #endregion

        #region 오버라이드 이벤트
        /// <summary>
        /// 폼이 보여질 때
        /// </summary>
        /// <param name="e"></param>
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.InitButtonClick();
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
                Utils.ShowWaitForm(this);
                MainButton.InitButton = false;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Initial, SqlDbType.Char);

                var initSet = SqlHelper.GetDataSet("xp_PurchaseOrder", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                tlsProject.SetColumns(initSet.Tables[0]);
                sleVCustomID.SetData(initSet.Tables[1], "UseClss");

                gleDepartID.SetData(initSet.Tables[2], "UseClss");
                glePersonID.SetData(initSet.Tables[3], "PersonInit,DepartID,UseClss");
                glePersonID.SetCascading(gleDepartID, "DepartID", "DepartID");

                sleCustomID.SetData(initSet.Tables[1], "UseClss");
                sleSTIPersonID.SetData(initSet.Tables[4], "PersonID,UseClss");

                gleReqDepartID.SetData(initSet.Tables[2], "UseClss");
                gleReqPersonID.SetData(initSet.Tables[3], "PersonInit,DepartID,UseClss");
                gleReqPersonID.SetCascading(gleReqDepartID, "DepartID", "DepartID");

                grcPurchaseOrderSub.SetColumns(initSet.Tables[5], "MaterialSeq,OrdStatus");

                //grcPurchaseOrderSub.SetColumns(initSet.Tables[2]);

                //var riOrdQty = grvPurchaseOrder.SetRepositoryItemSpinEdit("OrdQty");

                //var riOrdPrc = grvPurchaseOrder.SetRepositoryItemSpinEdit("OrdPrc");

                //var riOrdDate = grvPurchaseOrder.SetRepositoryItemDateEdit("OrdDate", ViewStyle.DateView);
                //riOrdDate.EditValueChanged += RiOrdDate_EditValueChanged;

                //var riExpDelivDate = grvPurchaseOrder.SetRepositoryItemDateEdit("ExpDelivDate", ViewStyle.DateView);

                //var riPORemark = grvPurchaseOrder.SetRepositoryItemTextEdit("PORemark");


                //grvPurchaseOrder.FixedColumnWidth(new Dictionary<string, int>()
                //{
                //    { "OrdQty", 80 },
                //    { "OrdPrc", 80 },
                //    { "OrdDate", 100 },
                //    { "ExpDelivDate", 100 },
                //});
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
        /// 신규 버튼
        /// </summary>
        public void NewButtonClick()
        {
            txtProjectNo.EditValue = null;
            txtReceiptID.EditValue = null;
            txtProjectName.EditValue = null;
            txtContractDate.EditValue = null;
            txtDeliveryDate.EditValue = null;
            txtOrderStatus.EditValue = null;
            speContractAmt.EditValue = 0;
            mmePRemark.EditValue = null;
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
                    // 필수 입력항목 체크
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;
                var logType = ActionLogType.Insert;
                var message = MessageText.OKInsert;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Exist, SqlDbType.Char);
                inParams.Add("@i_ProjectNo", txtProjectNo.EditValue.CString(), SqlDbType.Char);
                // ======================================
                // 파라메터 추가 필요
                // ======================================

                if (SqlHelper.Exist("xp_PurchaseOrder", inParams, out _, out _))
                {
                    if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionUpdate))
                        return;

                    logType = ActionLogType.Update;
                    message = MessageText.OKUpdate;
                }

                inParams.Clear();
                inParams.Add("@i_ProcessID", ProcessType.Merge, SqlDbType.Char);
                inParams.Add("@i_ProjectNo", txtProjectNo.EditValue.CString(), SqlDbType.Char);
                // ======================================
                // 파라메터 추가 필요
                // ======================================

                SqlHelper.Execute("xp_PurchaseOrder", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

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
                    { "ProjectNo", txtProjectNo.EditValue.CString() == "" },
                    // 필수 입력항목 체크
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;

                if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionDelete))
                    return;

                var inParams = new DbParamList();
                inParams.Add("ProcessID", ProcessType.Delete, SqlDbType.Char);
                inParams.Add("ProjectNo", txtProjectNo.EditValue.CString(), SqlDbType.Char);

                SqlHelper.Execute("xp_PurchaseOrder", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

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
            this.NewButtonClick();
            grvPurchaseOrderSub.DeleteAllRows();
            this.FillGridProject();
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
        /// 좌측 프로젝트 리스트 보이기/감추기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lcgProjectInfo_CustomButtonClick(object sender, DevExpress.XtraBars.Docking2010.BaseButtonEventArgs e)
        {
            switch (e.Button.Properties.Tag.CString().Trim())
            {
                case "HideList":
                    lcgProjectInfo.CustomHeaderButtons[0].Properties.Caption = $"    {"ShowList".Localization()}    ";
                    lcgProjectInfo.CustomHeaderButtons[0].Properties.Tag = "ShowList";
                    lcgProjectList.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    break;

                case "ShowList":
                    lcgProjectInfo.CustomHeaderButtons[0].Properties.Caption = $"    {"HideList".Localization()}    ";
                    lcgProjectInfo.CustomHeaderButtons[0].Properties.Tag = "HideList";
                    lcgProjectList.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    break;
            }
        }

        /// <summary>
        /// TreeList RowClick시 발주서 번호에 해당하는 프로젝트 정보 및 발주정보를 화면에 표시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tlsProject_RowClick(object sender, DevExpress.XtraTreeList.RowClickEventArgs e)
        {
            if (e.Node.Level == 1)
            {
                var projectNo = e.Node.GetValue("ProjectNo").CString();
                this.ShowDataProject(projectNo);
            }
        }

        /// <summary>
        /// TreeList Node 변경시 발주서 번호에 해당하는 프로젝트 정보 및 발주정보를 화면에 표시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tlsProject_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
           if (e.Node.Level == 1)
            {
                var projectNo = e.Node.GetValue("ProjectNo").CString();
                this.ShowDataProject(projectNo);
            }
        }

        /// <summary>
        /// 품의서 부서 변경시 담당자 그리드 값 초기화
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gleDepartID_EditValueChanged(object sender, EventArgs e)
        {
            glePersonID.EditValue = null;
        }

        /// <summary>
        /// 요청부서 변경시 담당자 그리드 초기화
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gleReqDepartID_EditValueChanged(object sender, EventArgs e)
        {
            gleReqPersonID.EditValue = null;
        }



        ///// <summary>
        ///// 프로젝트 그리드 행 변경시
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void grvProejct_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        //{
        //    if (e.FocusedRowHandle >= 0)
        //    {
        //        var projectNo = (sender as GridView).GetFocusedRowCellValue("ProjectNo").CString();
        //        this.ShowDataProject(projectNo);
        //        this.FillGridPurchaseOrder(projectNo);
        //    }
        //}

        ///// <summary>
        ///// 프로젝트 그리드 행 클릭시
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void grvProejct_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        //{
        //    var projectNo = (sender as GridView).GetFocusedRowCellValue("ProjectNo").CString();
        //    this.ShowDataProject(projectNo);
        //    this.FillGridPurchaseOrder(projectNo);
        //}

        /// <summary>
        /// 입력 컬럼 색상 변경
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvPurchaseOrderSub_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            if (e.RowHandle >= 0)
            {
                if (new string[] { "OrdQty", "OrdPrc", "OrdDate", "ExpDelivDate", "PORemark" }.Contains(e.Column.FieldName))
                {
                    e.Appearance.ForeColor = Color.White;
                    e.Appearance.BackColor = UserColors.EditableColor;
                }
            }
        }

        ///// <summary>
        ///// 발주 일자
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void RiOrdDate_EditValueChanged(object sender, EventArgs e)
        //{
        //    if (grvPurchaseOrderSub.PostEditor())
        //        grvPurchaseOrderSub.UpdateCurrentRow();

        //    grvPurchaseOrderSub.BestFitColumns();
        //}

        #endregion

        #region 사용자 정의 메소드
        /// <summary>
        /// 프로젝트별 발주서 리스트 조회
        /// </summary>
        private void FillGridProject()
        {
            try
            {
                Utils.ShowWaitForm(this);

                var startTime = DateTime.Now;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", "80", SqlDbType.Char);
                inParams.Add("@i_SContractDate", dteSContractDate.EditValue.CDateString());
                inParams.Add("@i_EContractDate", dteEContractDate.EditValue.CDateString());
                inParams.Add("@i_VCustomID", sleVCustomID.EditValue.CString());

                var table = SqlHelper.GetDataTable("xp_PurchaseOrder", inParams, out _, out _);

                tlsProject.SetData(table);
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
        /// 프로젝트 정보 화면 표시
        /// </summary>
        /// <param name="projectNo"></param>
        private void ShowDataProject(string projectNo)
        {
            try
            {
                this.NewButtonClick();

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetRow, SqlDbType.Char);
                inParams.Add("@i_ProjectNo", projectNo, SqlDbType.Char);

                var row = SqlHelper.GetDataRow("xp_PurchaseOrder", inParams, out _, out _);

                if (row != null)
                {
                    txtProjectNo.EditValue = row["ProjectNo"];
                    txtReceiptID.EditValue = row["ReceiptID"];
                    txtProjectName.EditValue = row["ProjectName"];
                    txtContractDate.EditValue = row["ContractDate"].CDateString();
                    txtDeliveryDate.EditValue = row["DeliveryDate"].CDateString();
                    txtOrderStatus.EditValue = row["OrderStatus"].CString();
                    speContractAmt.EditValue = row["ContractAmt"];
                    mmePRemark.EditValue = row["Remark"];
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
        /// 발주서 및 발주 품목 리스트 조회
        /// </summary>
        private void ShowDataPurchaseOrder(string poNo)
        {
            try
            {
                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", "91", SqlDbType.Char);
                inParams.Add("@i_ProjectNo", poNo, SqlDbType.Char);

                var ds = SqlHelper.GetDataSet("xp_PurchaseOrder", inParams, out _, out _);

                if (ds != null)
                {
                    if (ds.Tables[0].Rows != null && ds.Tables[0].Rows.Count > 0)
                    {
                        var row = ds.Tables[0].Rows[0];

                        // 품의서 정보
                        txtPONo.EditValue = row["PONo"];
                        txtPOStatus.EditValue = row["POStatus"];
                        dteApprvDate.EditValue = row["ApprvDate"];
                        dteHopeDate.EditValue = row["HopeDate"];
                        gleDepartID.EditValue = row["DepartID"];
                        glePersonID.EditValue = row["PersonID"];
                        txtAppModel.EditValue = row["AppModel"];
                        txtPOTitle.EditValue = row["POTitle"];
                        mmePOReason.EditValue = row["POReason"];
                        mmeRemark.EditValue = row["Remark"];

                        // 발주서 정보
                        sleCustomID.EditValue = row["CustomID"];
                        dteDueDate.EditValue = row["DueDate"];
                        txtPayTerms.EditValue = row["PayTerms"];
                        txtDeliveryLoc.EditValue = row["DeliveryLoc"];
                        sleSTIPersonID.EditValue = row["STIPersonID"];
                        txtSupplyPerson.EditValue = row["SupplyPerson"];
                        txtSupplyContact.EditValue = row["SupplyContact"];
                        speNegoAmt.EditValue = row["NegoAmt"];
                        gleReqDepartID.EditValue = row["ReqDepartID"];
                        gleReqPersonID.EditValue = row["ReqPersonID"];
                    }
                }

                // 발주 품목 내역
                grcPurchaseOrderSub.SetData(ds.Tables[1]);
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