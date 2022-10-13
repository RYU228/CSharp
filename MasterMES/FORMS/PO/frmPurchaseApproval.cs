using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTreeList.Nodes;
using DSLibrary;
using MasterMES.CLASSES;
using MasterMES.ETC;
using MasterMES.FORMS.ETC;

namespace MasterMES.FORMS.PO
{
    public partial class frmPurchaseApproval : frmBase, IButtonAction
    {
        private DataTable matlTable = null;

        #region 생성자
        public frmPurchaseApproval()
        {
            InitializeComponent();

            lcgProjectInfo.SetHeaderButtons("HideList");
            lcgQuotFileInfo.SetHeaderButtons("FileUpload");
            lcgPurchaseApprovalSub.SetHeaderButtons("AddRow");
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

                var initSet = SqlHelper.GetDataSet("xp_PurchaseApproval", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                sleVCustomID.SetData(initSet.Tables[0], "UseClss");
                tlsProject.SetColumns(initSet.Tables[1]);

                grcQuotationFile.SetColumns(initSet.Tables[2], "GroupSeq", false, false);

                // 견적서 첨부파일 다운로드
                var riQuotationFileDownload = grvQuotationFile.SetRepositoryItemSimpleButton("FileDownload", RepositoryButtonType.Download);
                riQuotationFileDownload.Click += RiQuotationFileDownload_Click;

                // 견적서 첨부파일 삭제
                var riQuotationFileDelete = grvQuotationFile.SetRepositoryItemSimpleButton("FileDelete", RepositoryButtonType.Delete);
                riQuotationFileDelete.Click += RiQuotationFileDelete_Click;

                gleDepartID.SetData(initSet.Tables[3], "DepartInit,UseClss");
                glePersonID.SetData(initSet.Tables[4], "PersonInit,DepartID,UseClss");
                glePersonID.SetCascading(gleDepartID, "DepartID", "DepartID");
                gleCurrCode.SetData(initSet.Tables[5], "UseClss");

                grcPurchaseApprovalSub.SetColumns(initSet.Tables[6], "MaterialSeq");
                grvPurchaseApprovalSub.FixedColumnWidth(new Dictionary<string, int>()
                {
                    { "DeleteRow", 60 },
                    { "FindMatl", 60 },
                    { "MaterialName", 230 },
                    { "Spec", 250 },
                    { "Unit", 70 },
                    { "OrdQty", 70 },
                    { "OrdPrc", 120 },
                    { "OrdAmt", 140 },
                    { "Remark", 300 },
                });

                // 자재검색을 위한 리스트 (찾기 버튼 클릭시 자재 검색용 데이터 소스
                matlTable = initSet.Tables[7];

                // 행삭제 버튼
                var riDeleteRow = grvPurchaseApprovalSub.SetRepositoryItemSimpleButton("DeleteRow", RepositoryButtonType.Delete, "Delete".Localization());
                riDeleteRow.Click += RiDeleteRow_Click;

                // 자재 찾기 버튼
                var riFindMatl = grvPurchaseApprovalSub.SetRepositoryItemSimpleButton("FindMatl", RepositoryButtonType.Find, "FindMatl".Localization());
                riFindMatl.Click += RiFindMatl_Click;

                // 자재명
                var riMaterialName = grvPurchaseApprovalSub.SetRepositoryItemTextEdit("MaterialName");
                riMaterialName.EditValueChanged += RiMaterialName_EditValueChanged;

                // 규격
                var riSpec = grvPurchaseApprovalSub.SetRepositoryItemTextEdit("Spec");
                riSpec.EditValueChanged += RiSpec_EditValueChanged;

                // 단위
                var riUnit = grvPurchaseApprovalSub.SetRepositoryItemGridLookUpEdit("Unit", initSet.Tables[8], "MiddleCode", "MiddleName", "UseClss");

                // 발주수량
                var riOrdQty = grvPurchaseApprovalSub.SetRepositoryItemSpinEdit("OrdQty");
                riOrdQty.EditValueChanged += RiOrdQty_EditValueChanged;

                // 발주단가
                var riOrdPrc = grvPurchaseApprovalSub.SetRepositoryItemSpinEdit("OrdPrc");
                riOrdPrc.EditValueChanged += RiOrdPrc_EditValueChanged;

                // 비고
                var riRemark = grvPurchaseApprovalSub.SetRepositoryItemTextEdit("Remark");
                riRemark.EditValueChanged += RiRemark_EditValueChanged;

                // 그리드 초기 형식 저장
                DxControl.SaveGridFormat(grvQuotationFile, GridFormat.Default);
                DxControl.SaveGridFormat(grvPurchaseApprovalSub, GridFormat.Default);
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
            txtPOAprvNo.EditValue = null;
            dtePOAprvDate.SetDefaultDate();
            gleDepartID.EditValue = null;
            glePersonID.EditValue = null;
            txtApplyModel.EditValue = null;
            txtPOAprvTitle.EditValue = null;
            dteDesiredDate.EditValue = null;
            speNegoAmt.EditValue = 0;
            txtPOStatus.EditValue = null;
            txtDocNo.EditValue = null;
            gleCurrCode.EditValue = null;
            mmePOAprvReason.EditValue = null;
            mmeRemark.EditValue = null;

            grvQuotationFile.DeleteAllRows();
            grvPurchaseApprovalSub.DeleteAllRows();
        }

        /// <summary>
        /// 저장 버튼
        /// </summary>
        public void SaveButtonClick()
        {
            try
            {
                var departInit = (gleDepartID.Properties.GetRowByKeyValue(gleDepartID.EditValue) as DataRowView).Row["DepartInit"].CString();
                var personInit = (glePersonID.Properties.GetRowByKeyValue(glePersonID.EditValue) as DataRowView).Row["PersonInit"].CString();

                var field = new Dictionary<string, bool>()
                {
                    { "ProjectNo", txtProjectNo.EditValue.CString() == "" },
                    { "POAprvDate", dtePOAprvDate.EditValue.CString() == "" },
                    { "DepartID", gleDepartID.EditValue.CString() == "" },
                    { "DepartInit", departInit == "" },
                    { "PersonID", glePersonID.EditValue.CString() == "" },
                    { "PersonInit", personInit == "" },
                    { "POAprvTitle", txtPOAprvTitle.EditValue.CString() == "" },
                    { "CurrCode", gleCurrCode.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;
                var logType = ActionLogType.Insert;
                var message = MessageText.OKInsert;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Exist, SqlDbType.Char);
                inParams.Add("@i_POAprvNo", txtPOAprvNo.EditValue.CString(), SqlDbType.Char);

                if (SqlHelper.Exist("xp_PurchaseAprroval", inParams, out _, out _))
                {
                    if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionUpdate))
                        return;

                    logType = ActionLogType.Update;
                    message = MessageText.OKUpdate;
                }

                // 자재 정보 XmlData 생성 : 자재 순번 생성 후 XML 생성
                for (int i = 0; i < grvPurchaseApprovalSub.RowCount; i++)
                    grvPurchaseApprovalSub.SetRowCellValue(i, "MaterialSeq", i + 1);

                var xmlData = (grcPurchaseApprovalSub.DataSource as DataTable).DefaultView
                                .ToTable(false, new string[] { "MaterialSeq", "MaterialID", "MaterialName", "Spec", "Unit", "OrdQty", "OrdPrc", "OrdAmt", "Remark" })
                                .ToXml();

                inParams.Clear();
                inParams.Add("@i_ProcessID", ProcessType.Merge, SqlDbType.Char);
                inParams.Add("@i_POAprvNo", txtPOAprvNo.EditValue.CString().Replace("-", ""), SqlDbType.Char);
                inParams.Add("@i_ProjectNo", txtProjectNo.EditValue.CString().Replace("-", ""), SqlDbType.Char);
                inParams.Add("@i_POAprvDate", dtePOAprvDate.EditValue.CDateString(), SqlDbType.Date);
                inParams.Add("@i_DepartID", gleDepartID.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_PersonID", glePersonID.EditValue.CString(), SqlDbType.VarChar);
                inParams.Add("@i_ApplyModel", txtApplyModel.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_DesiredDate", dteDesiredDate.EditValue.CDateString(), SqlDbType.Date);
                inParams.Add("@i_POAprvTitle", txtPOAprvTitle.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_NegoAmt", speNegoAmt.EditValue.CDouble(), SqlDbType.Decimal);
                inParams.Add("@i_CurrCode", gleCurrCode.EditValue.CString(), SqlDbType.VarChar);
                inParams.Add("@i_POAprvReason", mmePOAprvReason.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_Remark", mmeRemark.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_DepartInit", departInit, SqlDbType.VarChar);
                inParams.Add("@i_PersonInit", personInit, SqlDbType.VarChar);
                inParams.Add("@i_XmlData", xmlData, SqlDbType.VarChar);
                inParams.Add("@i_InptUser", AppConfig.Login.UserID, SqlDbType.VarChar);

                SqlHelper.Execute("xp_PurchaseApproval", inParams, out string[] retVal, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                this.NewButtonClick();

                this.FillTreeProject();
                tlsProject.FindNodeByValue(retVal[0]);
                //this.ShowDataPurchaseApproval(retVal[0]);

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
        }

        /// <summary>
        /// 조회 버튼
        /// </summary>
        public void ViewButtonClick()
        {
            this.NewButtonClick();

            this.FillTreeProject();
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
        /// 좌측 프로젝트-발주품의 리스트 보이기/감추기
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
                    lcgQuotFileInfo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    break;

                case "ShowList":
                    lcgProjectInfo.CustomHeaderButtons[0].Properties.Caption = $"    {"HideList".Localization()}    ";
                    lcgProjectInfo.CustomHeaderButtons[0].Properties.Tag = "HideList";
                    lcgProjectList.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lcgQuotFileInfo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    break;
            }
        }

        /// <summary>
        /// 발주 품의서별 견적서 첨부 (발주 품의서 번호가 필요하며므로 데이터 저장 후 파일 첨부)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lcgQuotFileInfo_CustomButtonClick(object sender, DevExpress.XtraBars.Docking2010.BaseButtonEventArgs e)
        {
            if (e.Button.Properties.Tag.CString() != "FileUpload")
                return;

            try
            {
                var field = new Dictionary<string, bool>()
                {
                    { "ProjectNo", txtProjectNo.EditValue.CString() == "" },
                    { "POAprvNo", txtPOAprvNo.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                var menuID = Utils.GetMenuID(this);
                var groupName = $"{txtProjectNo.EditValue.CString().Replace("-", "")}-{txtPOAprvNo.EditValue.CString().Replace("-", "")}";

                var frmFileDialog = new frmFileDlg(menuID, groupName);
                frmFileDialog.ShowDialog();

                (new FileUpDown()).FillGridAttachFile(this, grcQuotationFile, groupName);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 견적서 첨부파일 다운로드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RiQuotationFileDownload_Click(object sender, EventArgs e)
        {
            var fileUpDown = new FileUpDown();
            var groupName = $"{txtProjectNo.EditValue.CString().Replace("-", "")}-{txtPOAprvNo.EditValue.CString().Replace("-", "")}";

            fileUpDown.DownloadAttachFile(this,
                                          groupName,
                                          grvQuotationFile.GetFocusedRowCellValue("GroupSeq").CInt(),
                                          grvQuotationFile.GetFocusedRowCellValue("FileName").CString());
        }

        /// <summary>
        /// 견적서 첨부파일 삭제
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RiQuotationFileDelete_Click(object sender, EventArgs e)
        {
            var fileUpDown = new FileUpDown();
            var groupName = $"{txtProjectNo.EditValue.CString().Replace("-", "")}-{txtPOAprvNo.EditValue.CString().Replace("-", "")}";

            fileUpDown.DeleteAttachFile(this,
                                        groupName,
                                        grvQuotationFile.GetFocusedRowCellValue("GroupSeq").CInt());
            fileUpDown.FillGridAttachFile(this, grcQuotationFile, groupName);
        }

        /// <summary>
        /// 부서변경시 담당자 컨트롤 초기화
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gleDepartID_EditValueChanged(object sender, EventArgs e)
        {
            glePersonID.EditValue = null;
        }

        /// <summary>
        /// TreeList 행 클릭시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tlsProject_RowClick(object sender, DevExpress.XtraTreeList.RowClickEventArgs e)
        {
            this.ShowDataProjectPurchaseApproval(e.Node, e.Node.ParentNode);
        }

        /// <summary>
        /// TreeList 행 변경시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tlsProject_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            this.ShowDataProjectPurchaseApproval(e.Node, e.Node.ParentNode);
        }

        /// <summary>
        /// 그리드 행추가 하기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lcgPurchaseApprovalSub_CustomButtonClick(object sender, DevExpress.XtraBars.Docking2010.BaseButtonEventArgs e)
        {
            grvPurchaseApprovalSub.AddNewRow();

            if (grvPurchaseApprovalSub.PostEditor())
                grvPurchaseApprovalSub.UpdateCurrentRow();

            grvPurchaseApprovalSub.Focus();
            grvPurchaseApprovalSub.FocusedColumn = grvPurchaseApprovalSub.Columns["MaterialName"];
        }

        /// <summary>
        /// 추가된 행 초기화
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvPurchaseApprovalSub_InitNewRow(object sender, DevExpress.XtraGrid.Views.Grid.InitNewRowEventArgs e)
        {
            var view = sender as GridView;
            view.SetRowCellValue(e.RowHandle, "MasterialSeq", 0);
            view.SetRowCellValue(e.RowHandle, "MaterialID", null);
            view.SetRowCellValue(e.RowHandle, "MaterialName", null);
            view.SetRowCellValue(e.RowHandle, "Spec", null);
            view.SetRowCellValue(e.RowHandle, "OrdQty", 0);
            view.SetRowCellValue(e.RowHandle, "OrdPrc", 0);
            view.SetRowCellValue(e.RowHandle, "OrdAmt", 0);
            view.SetRowCellValue(e.RowHandle, "Remark", null);
        }

        /// <summary>
        /// 입력 컬럼 색상 변경
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvPurchaseApprovalSub_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            if (e.RowHandle >= 0)
            {
                if (new string[] { "MaterialName", "Spec", "Unit", "OrdQty", "OrdPrc", "Remark" }.Contains(e.Column.FieldName))
                {
                    e.Appearance.ForeColor = Color.White;
                    e.Appearance.BackColor = UserColors.EditableColor;
                }
            }
        }

        /// <summary>
        /// 행 삭제 버튼 클릭시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RiDeleteRow_Click(object sender, EventArgs e)
        {
            grvPurchaseApprovalSub.DeleteRow(grvPurchaseApprovalSub.FocusedRowHandle);
        }

        /// <summary>
        /// 자재 찾기 버튼 클릭시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RiFindMatl_Click(object sender, EventArgs e)
        {
            var frmFind = new frmFindValue("Material", matlTable);
            frmFind.FindValueEvent += FrmFind_FindValueEvent;
            frmFind.ShowDialog();

            if (grvPurchaseApprovalSub.PostEditor())
                grvPurchaseApprovalSub.UpdateCurrentRow();
        }

        /// <summary>
        /// 자재 찾기에서 선택된 자재에 대한 정보를 그리드에 표시
        /// </summary>
        /// <param name="row"></param>
        private void FrmFind_FindValueEvent(DataRow row)
        {
            if (row != null)
            {
                grvPurchaseApprovalSub.SetFocusedRowCellValue("MaterialID", row["MaterialID"]);
                grvPurchaseApprovalSub.SetFocusedRowCellValue("MaterialName", row["MaterialName"]);
                grvPurchaseApprovalSub.SetFocusedRowCellValue("Spec", row["Spec"]);

                grvPurchaseApprovalSub.FocusedColumn = grvPurchaseApprovalSub.Columns["OrdQty"];
            }
        }

        /// <summary>
        /// 자재명
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RiMaterialName_EditValueChanged(object sender, EventArgs e)
        {
            grvPurchaseApprovalSub.SetFocusedRowCellValue("MaterialName", (sender as TextEdit).EditValue);

            if (grvPurchaseApprovalSub.PostEditor())
                grvPurchaseApprovalSub.UpdateCurrentRow();
        }

        /// <summary>
        /// Spec
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RiSpec_EditValueChanged(object sender, EventArgs e)
        {
            grvPurchaseApprovalSub.SetFocusedRowCellValue("Spec", (sender as TextEdit).EditValue);

            if (grvPurchaseApprovalSub.PostEditor())
                grvPurchaseApprovalSub.UpdateCurrentRow();
        }

        /// <summary>
        /// 발주 수량
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RiOrdQty_EditValueChanged(object sender, EventArgs e)
        {
            grvPurchaseApprovalSub.SetFocusedRowCellValue("OrdQty", (sender as SpinEdit).EditValue);

            if (grvPurchaseApprovalSub.PostEditor())
                grvPurchaseApprovalSub.UpdateCurrentRow();

            var ordAmt = grvPurchaseApprovalSub.GetFocusedRowCellValue("OrdQty").CDouble() *
                            grvPurchaseApprovalSub.GetFocusedRowCellValue("OrdPrc").CDouble();

            grvPurchaseApprovalSub.SetFocusedRowCellValue("OrdAmt", ordAmt);
        }

        /// <summary>
        /// 발주 단가
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RiOrdPrc_EditValueChanged(object sender, EventArgs e)
        {
            grvPurchaseApprovalSub.SetFocusedRowCellValue("OrdPrc", (sender as SpinEdit).EditValue);

            if (grvPurchaseApprovalSub.PostEditor())
                grvPurchaseApprovalSub.UpdateCurrentRow();

            var ordAmt = grvPurchaseApprovalSub.GetFocusedRowCellValue("OrdQty").CDouble() *
                            grvPurchaseApprovalSub.GetFocusedRowCellValue("OrdPrc").CDouble();

            grvPurchaseApprovalSub.SetFocusedRowCellValue("OrdAmt", ordAmt);
        }

        /// <summary>
        /// 비고
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RiRemark_EditValueChanged(object sender, EventArgs e)
        {
            grvPurchaseApprovalSub.SetFocusedRowCellValue("Remark", (sender as TextEdit).EditValue);

            if (grvPurchaseApprovalSub.PostEditor())
                grvPurchaseApprovalSub.UpdateCurrentRow();
        }

        /// <summary>
        /// 그리드 셀에서 엔터키 입력시 다음 입력 컬럼으로 이동
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvPurchaseApprovalSub_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                (sender as GridView).MoveCellFocus();
        }

        #region 폼컨트롤 이벤트 - 결재 관련 버튼     

        /// <summary>
        /// 기안(결재상신) 버튼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDraft_Click(object sender, EventArgs e)
        {
            var frmSetAprv = new frmSetApproval();
            frmSetAprv.ShowDialog();
        }

        /// <summary>
        /// 기안취소 버튼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancelDraft_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 결재 버튼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnApproval_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 기각 버튼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReject_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #endregion

        #region 사용자 정의 메서드
        /// <summary>
        /// 프로젝트별 발주 품의서 리스트 조회
        /// </summary>
        private void FillTreeProject()
        {
            try
            {
                var startTime = DateTime.Now;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetList, SqlDbType.Char);
                inParams.Add("@i_SContractDate", dteSContractDate.EditValue.CDateString());
                inParams.Add("@i_EContractDate", dteEContractDate.EditValue.CDateString());
                inParams.Add("@i_VCustomID", sleVCustomID.EditValue.CString());

                var table = SqlHelper.GetDataTable("xp_PurchaseApproval", inParams, out _, out _);

                tlsProject.SetData(table, 0);

                tlsProject.Columns["ProjectNo"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;

                SqlHelper.WriteLog(this, ActionLogType.Select, startTime);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// TreeList 행 클릭 또는 변경시 프로젝트 정보 및 발주 품의서 정보 조회
        /// </summary>
        /// <param name="selectedNode"></param>
        /// <param name="parentNode"></param>
        private void ShowDataProjectPurchaseApproval(TreeListNode selectedNode, TreeListNode parentNode)
        {
            try
            {
                Utils.ShowWaitForm(this);

                string projectNo;

                switch (selectedNode.Level)
                {
                    case 0:
                        projectNo = selectedNode.GetValue("KeyField").CString();

                        this.ShowDataProject(projectNo);
                        break;

                    case 1:
                        projectNo = parentNode.GetValue("KeyField").CString();
                        string poAprvNo = selectedNode.GetValue("KeyField").CString();

                        this.ShowDataProject(projectNo);
                        this.ShowDataPurchaseApproval(poAprvNo);
                        this.FillGridQuotationFile();
                        break;
                }
            }
            finally
            {
                Utils.CloseWaitForm();
            }
        }

        /// <summary>
        /// 프로젝트 정보 조회
        /// </summary>
        private void ShowDataProject(string projectNo)
        {
            try
            {
                this.NewButtonClick();

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetRow, SqlDbType.Char);
                inParams.Add("@i_ProjectNo", projectNo, SqlDbType.Char);

                var row = SqlHelper.GetDataRow("xp_PurchaseApproval", inParams, out _, out string retMsg);

                if (row != null)
                {
                    txtProjectNo.EditValue = row["ProjectNo"];
                    //txtReceiptID.EditValue = row["ReceiptID"];
                    txtProjectName.EditValue = row["ProjectName"];
                    //txtContractDate.EditValue = row["ContractDate"].CDateString();
                    txtDeliveryDate.EditValue = row["DeliveryDate"].CDateString();
                    //txtOrderStatus.EditValue = row["OrderStatus"];
                    //speContractAmt.EditValue = row["ContractAmt"];
                    mmePRemark.EditValue = row["Remark"];
                }
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 발주 품의서 정보 조회
        /// </summary>
        private void ShowDataPurchaseApproval(string poAprvNo)
        {
            try
            {
                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", "91", SqlDbType.Char);
                inParams.Add("@i_POAprvNo", poAprvNo, SqlDbType.Char);

                var ds = SqlHelper.GetDataSet("xp_PurchaseApproval", inParams, out _, out _);

                if (ds != null)
                {
                    if (ds.Tables[0].Rows != null && ds.Tables[0].Rows.Count > 0)
                    {
                        var row = ds.Tables[0].Rows[0];

                        // 발주품의서 정보
                        txtPOAprvNo.EditValue = row["POAprvNo"];
                        dtePOAprvDate.EditValue = row["POAprvDate"];
                        gleDepartID.EditValue = row["DepartID"];
                        glePersonID.EditValue = row["PersonID"];
                        txtApplyModel.EditValue = row["ApplyModel"];
                        txtPOAprvTitle.EditValue = row["POAprvTitle"];
                        dteDesiredDate.EditValue = row["DesiredDate"];
                        speNegoAmt.EditValue = row["NegoAmt"];
                        gleCurrCode.EditValue = row["CurrCode"];
                        mmePOAprvReason.EditValue = row["POAprvReason"];
                        mmeRemark.EditValue = row["Remark"];
                        txtDocNo.EditValue = row["DocNO"];
                        txtPOStatus.EditValue = row["POStatus"];
                    }
                }

                // 발주 품목 내역
                grcPurchaseApprovalSub.SetData(ds.Tables[1]);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 견적서 첨부파일 조회
        /// </summary>
        private void FillGridQuotationFile()
        {
            var groupName = $"{txtProjectNo.EditValue.CString().Replace("-", "")}-{txtPOAprvNo.EditValue.CString().Replace("-", "")}";
            (new FileUpDown()).FillGridAttachFile(this, grcQuotationFile, groupName);
        }

        #endregion
    }
}