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

namespace MasterMES.FORMS.BUSINESS
{
    public partial class frmReceipt : frmBase, IButtonAction
    {
        #region 생성자
        public frmReceipt()
        {
            InitializeComponent();

            DxControl.ButtonEditEvent += ButtonEditEvent;
            Extensions.ButtonEditEvent += ButtonEditEvent;

            // DateEdit 월 단위로 표시
            dteReceiptMon.SetViewStyle(ViewStyle.MonthView);
            dteSReceiptMon.SetViewStyle(ViewStyle.MonthView);
            dteEReceiptMon.SetViewStyle(ViewStyle.MonthView);

            // 파일 업로드 버튼 생성
            lcgConceptFile.SetHeaderButtons("FileUpload");
            lcgQuotationFile.SetHeaderButtons("FileUpload");
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
            try
            {
                Utils.ShowWaitForm(this);
                MainButton.InitButton = false;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Initial, SqlDbType.Char);

                var initSet = SqlHelper.GetDataSet("xp_Receipt", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                gleEquipType.SetData(initSet.Tables[0], "UseClss");
                gleInterest.SetData(initSet.Tables[1], "UseClss");
                gleReceiptState.SetData(initSet.Tables[2], "UseClss");             
                grcReceipt.SetColumns(initSet.Tables[3], "UseClss");
                grcQuotation.SetColumns(initSet.Tables[4]);
                grcConceptFile.SetColumns(initSet.Tables[5], "GroupSeq", false, false);    // 컨셉도 파일 첨부 그리드
                grcQuotationFile.SetColumns(initSet.Tables[5], "GroupSeq", false, false);  // 견적서 파일 첨부 그리드

                // 컨셉도 파일 다운로드
                var riConceptFileDownload = grvConceptFile.SetRepositoryItemSimpleButton("FileDownload", RepositoryButtonType.Download);
                riConceptFileDownload.Click += RiConceptFileDownload_Click;

                // 컨셉도 파일 삭제
                var riConceptFileDelete = grvConceptFile.SetRepositoryItemSimpleButton("FileDelete", RepositoryButtonType.Delete);
                riConceptFileDelete.Click += RiConceptFileDelete_Click;

                // 견적서 파일 다운로드
                var riQuotationFileDownload = grvQuotationFile.SetRepositoryItemSimpleButton("FileDownload", RepositoryButtonType.Download);
                riQuotationFileDownload.Click += RiQuotationFileDownload_Click;

                // 견적서 파일 삭제
                var riQuotationFileDelete = grvQuotationFile.SetRepositoryItemSimpleButton("FileDelete", RepositoryButtonType.Delete);
                riQuotationFileDelete.Click += RiQuotationFileDelete_Click;

                this.FillGridReceipt();

                // 그리드 초기 형식 저장 - 접수 내역 / 견적 이력 / 컨셉도 첨부 파일 / 견적서 첨부 파일
                DxControl.SaveGridFormat(grvReceipt, GridFormat.Default);
                DxControl.SaveGridFormat(grvQuotation, GridFormat.Default);
                DxControl.SaveGridFormat(grvConceptFile, GridFormat.Default);
                DxControl.SaveGridFormat(grvQuotation, GridFormat.Default);
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
            this.FillGridReceipt();
        }

        /// <summary>
        /// 신규 버튼
        /// </summary>
        public void NewButtonClick()
        {
            DxControl.ClearControl(lcgReceipt);
            gleReceiptState.EditValue = "1";    // '견적中'으로 설정

            txtReceiptID.Focus();
        }

        /// <summary>
        /// 복사 버튼
        /// </summary>
        public void CopyButtonClick()
        {
            try
            {
                var field = new Dictionary<string, bool>()
                {
                    { "ReceiptID", txtReceiptID.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;

                if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionCopy))
                    return;

                Utils.ShowWaitForm(this);
                MainButton.CopyButton = false;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Copy, SqlDbType.Char);
                inParams.Add("@i_ReceiptID", txtReceiptID.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_InptUser", AppConfig.Login.PersonID, SqlDbType.VarChar);

                SqlHelper.Execute("xp_Receipt", inParams, out string[] retVal, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                this.NewButtonClick();
                this.FillGridReceipt();

                // 신규 생성된 거래처 코드에 해당하는 데이터를 화면에 디스플레이(FocusedRowChanged Event 발생)
                grvReceipt.FindRowByValue("ReceiptID", retVal[0]);

                SqlHelper.WriteLog(this, ActionLogType.Insert, startTime);
                MsgBox.OKMessage(MessageText.OKCopy);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
            finally
            {
                MainButton.CopyButton = true;
                Utils.CloseWaitForm();
            }
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
                    { "KCustom", bteCustom.EditValue.CString() == "" },
                    { "CustomID", bteCustom.Tag.CString() == "" },
                    { "CustomCharge", bteCustomCharge.Tag.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;
                var logType = ActionLogType.Insert;
                var message = MessageText.OKInsert;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Exist, SqlDbType.Char);
                inParams.Add("@i_ReceiptID", txtReceiptID.EditValue.CString(), SqlDbType.Char);

                if (SqlHelper.Exist("xp_Receipt", inParams, out _, out _))
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
                inParams.Add("@i_ReceiptMon", dteReceiptMon.EditValue.CDateString(), SqlDbType.Date);
                inParams.Add("@i_ReceiptID", txtReceiptID.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_CustomID", bteCustom.Tag.CString(), SqlDbType.Char);
                inParams.Add("@i_ChargeSeq", bteCustomCharge.Tag.CString(), SqlDbType.TinyInt);
                inParams.Add("@i_EquipType", gleEquipType.EditValue.CString(), SqlDbType.VarChar);
                inParams.Add("@i_Interest", gleInterest.EditValue.CString(), SqlDbType.VarChar);
                inParams.Add("@i_ReceiptState", gleReceiptState.EditValue.CString(), SqlDbType.VarChar);
                inParams.Add("@i_SpecialNote", mmeSpecialNote.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_Remark", mmeRemark.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_InptUser", AppConfig.Login.PersonID, SqlDbType.VarChar);

                SqlHelper.Execute("xp_Receipt", inParams, out string[] retVal, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);
                
                this.FillGridReceipt();
                grvReceipt.FindRowByValue("ReceiptID", retVal[0]);

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
                inParams.Add("@i_ReceiptID", txtReceiptID.EditValue.CString(), SqlDbType.Char);

                SqlHelper.Execute("xp_Receipt", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                this.FillGridReceipt();

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
                    if (bte.Tag.CString() != "")
                    {
                        GetCustomInfo();
                    }
                    break;

                case "bteCustomCharge":
                    if (bteCustom.Tag.CString() == "")
                    {
                        MsgBox.ErrorMessage("거래처를 먼저 선택해 주십시오.");
                        return false;
                    }
                    fdResult = FindData.ShowFindData(FindData.FIND_CUSTOMCHARGE, bte.EditValue.CString(), bteCustom.Tag.CString(), null, null, false, bte);
                    if (bte.Tag.CString() != "")
                    {
                        GetCustomChargeInfo();
                    }
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
        /// 거래처 찾기 삭제 버튼 클릭시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bteCustom_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (e.Button.Tag.CString() == "CLEAR")
                txtAddress.EditValue = null;
        }

        /// <summary>
        /// 거래처 담당자 찾기 버튼 클릭시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bteCustomCharge_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (e.Button.Tag.CString() == "CLEAR")
            {
                txtDutyName.EditValue = null;
                txtCellPhoneNo.EditValue = null;
                txtEMail.EditValue = null;
            }
        }

        /// <summary>
        /// 그리드 행변경시 화면에 정보 표시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvReceipt_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle >= 0)
            {
                var ReceiptID = (sender as GridView).GetFocusedRowCellValue("ReceiptID").CString();
                this.ShowDataReceipt(ReceiptID);
                this.FillGridConceptFile();
                this.FillGridQuotation();
            }
        }

        private void grvReceipt_RowClick(object sender, RowClickEventArgs e)
        {
            var ReceiptID = (sender as GridView).GetFocusedRowCellValue("ReceiptID").CString();
            this.ShowDataReceipt(ReceiptID);
            this.FillGridConceptFile();
            this.FillGridQuotation();
        }

        private void grvReceipt_DoubleClick(object sender, EventArgs e)
        {
            //var field = new Dictionary<string, bool>()
            //{
            //    { "ReceiptID", txtReceiptID.EditValue.CString() == "" },
            //};
            //if (Utils.CheckField(field))
            //    return;

            var ea = e as DXMouseEventArgs;
            GridView view = sender as GridView;
            var info = view.CalcHitInfo(ea.Location);
            var ReceiptID = view.GetFocusedRowCellValue("ReceiptID").CString();
            string ContactDate = "";
            bool isSelected = false;

            if (info.InRow || info.InRowCell)
            {
                if (info.Column.FieldName == "ContactDate1" || info.Column.FieldName == "ContactDetails1")
                {
                    ContactDate = view.GetFocusedRowCellValue("ContactDate1").CDateString("yyyy-MM-dd");
                    isSelected = true;
                }
                else if (info.Column.FieldName == "ContactDate2" || info.Column.FieldName == "ContactDetails2")
                {
                    ContactDate = view.GetFocusedRowCellValue("ContactDate2").CDateString("yyyy-MM-dd");
                    isSelected = true;
                }
                else if (info.Column.FieldName == "ContactDate3" || info.Column.FieldName == "ContactDetails3")
                {
                    ContactDate = view.GetFocusedRowCellValue("ContactDate3").CDateString("yyyy-MM-dd");
                    isSelected = true;
                }

                if(isSelected)
                {
                    // Delegate를 이용한 이벤트 발생으로 데이터 조회
                    using (frmReceiptSales frmSales = new frmReceiptSales(ReceiptID, ContactDate))
                    {
                        frmSales.ChangeReceiptSales += FillGridReceipt;
                        Utils.ShowPopupForm(frmSales);
                    }
                }
            }
        }

        /// <summary>
        /// 견적서 Double Click - 견적서 관리창 Show
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvQuotation_DoubleClick(object sender, EventArgs e)
        {
            var field = new Dictionary<string, bool>()
            {
                { "ReceiptID", txtReceiptID.EditValue.CString() == "" },
            };
            if (Utils.CheckField(field))
                return;

            var ea = e as DXMouseEventArgs;
            GridView view = sender as GridView;
            var info = view.CalcHitInfo(ea.Location);

            var receiptID = txtReceiptID.EditValue.CString();
            var kCustom = bteCustom.Text;
            var quotID = "";

            if (info.InRow || info.InRowCell)
                quotID = view.GetFocusedRowCellValue("QuotID").CString();

            using (frmQuotation frmQuot = new frmQuotation(receiptID, quotID, kCustom))
            {
                frmQuot.ChangeQuotation += FillGridQuotation;
                Utils.ShowPopupForm(frmQuot);
            }
        }

        /// <summary>
        /// 컨셉도 파일 다운로드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RiConceptFileDownload_Click(object sender, EventArgs e)
        {
            var groupName = grvReceipt.GetFocusedRowCellValue("ReceiptID").CString();

            var fileUpDown = new FileUpDown();
            fileUpDown.DownloadAttachFile(this, groupName, grvConceptFile.GetFocusedRowCellValue("GroupSeq").CInt(),
                                                           grvConceptFile.GetFocusedRowCellValue("FileName").CString());
        }

        /// <summary>
        /// 컨셉도 파일 삭제
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RiConceptFileDelete_Click(object sender, EventArgs e)
        {
            var groupName = grvReceipt.GetFocusedRowCellValue("ReceiptID").CString();

            var fileUpDown = new FileUpDown();
            fileUpDown.DeleteAttachFile(this, groupName, grvConceptFile.GetFocusedRowCellValue("GroupSeq").CInt());
            fileUpDown.FillGridAttachFile(this, grcConceptFile, groupName);
        }

        /// <summary>
        /// 견적서 파일 다운로드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RiQuotationFileDownload_Click(object sender, EventArgs e)
        {
            var groupName = $"{grvReceipt.GetFocusedRowCellValue("ReceiptID")}{grvQuotation.GetFocusedRowCellValue("QuotID")}";

            var fileUpDown = new FileUpDown();
            fileUpDown.DownloadAttachFile(this, groupName, grvQuotationFile.GetFocusedRowCellValue("GroupSeq").CInt(),
                                                           grvQuotationFile.GetFocusedRowCellValue("FileName").CString());
        }

        /// <summary>
        /// 견적서 파일 삭제
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RiQuotationFileDelete_Click(object sender, EventArgs e)
        {
            var groupName = $"{grvReceipt.GetFocusedRowCellValue("ReceiptID")}{grvQuotation.GetFocusedRowCellValue("QuotID")}";

            var fileUpDown = new FileUpDown();
            fileUpDown.DeleteAttachFile(this, groupName, grvQuotationFile.GetFocusedRowCellValue("GroupSeq").CInt());
            fileUpDown.FillGridAttachFile(this, grcQuotationFile, groupName);
        }


        /// <summary>
        /// 견적서 그리드 행 클릭시 - 견적서 파일 리스트 조회
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvQuotation_RowClick(object sender, RowClickEventArgs e)
        {
            this.FillGridQuotationFile();
        }

        /// <summary>
        /// 견적 그리드 행 변경시 - 견적서 파일 리스트 조회
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvQuotation_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            this.FillGridQuotationFile();
        }


        /// <summary>
        /// 컨셉도 파일 첨부
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lcgConceptFile_CustomButtonClick(object sender, DevExpress.XtraBars.Docking2010.BaseButtonEventArgs e)
        {
            if (e.Button.Properties.Tag.CString() != "FileUpload")
                return;

            try
            {
                var field = new Dictionary<string, bool>()
                {
                    { "Receipt Info", grvReceipt.FocusedRowHandle < 0 },
                };
                if (Utils.CheckField(field))
                    return;

                var menuID = ((FormLog)this.Tag).MenuID;
                var groupName = grvReceipt.GetFocusedRowCellValue("ReceiptID").CString();
                
                var frmFileDialog = new frmFileDlg(menuID, groupName);
                frmFileDialog.ShowDialog();

                var fileUpDown = new FileUpDown();
                fileUpDown.FillGridAttachFile(this, grcConceptFile, groupName);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 견적서 파일 첨부
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lcgQuotationFile_CustomButtonClick(object sender, DevExpress.XtraBars.Docking2010.BaseButtonEventArgs e)
        {
            if (e.Button.Properties.Tag.CString() != "FileUpload")
                return;

            try
            {
                var field = new Dictionary<string, bool>()
                {
                    { "Receipt Info", grvReceipt.FocusedRowHandle < 0 },
                    { "Quotation Info", grvQuotation.FocusedRowHandle < 0 },
                };
                if (Utils.CheckField(field))
                    return;

                var menuID = ((FormLog)this.Tag).MenuID;
                var groupName = $"{grvReceipt.GetFocusedRowCellValue("ReceiptID")}{grvQuotation.GetFocusedRowCellValue("QuotID")}";
                
                var frmFileDialog = new frmFileDlg(menuID, groupName);
                frmFileDialog.ShowDialog();

                var fileUpDown = new FileUpDown();
                fileUpDown.FillGridAttachFile(this, grcQuotationFile, groupName);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        #endregion

        #region 사용자 정의 메서드
        /// <summary>
        /// 거래처 조회 : Grid
        /// </summary>
        private void FillGridReceipt()
        {
            try
            {
                var startTime = DateTime.Now;

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetList, SqlDbType.Char);
                inParams.Add("@i_SReceiptMon", dteSReceiptMon.EditValue.CDateString(), SqlDbType.Date);
                inParams.Add("@i_EReceiptMon", dteEReceiptMon.EditValue.CDateString(), SqlDbType.Date);

                var table = SqlHelper.GetDataTable("xp_Receipt", inParams, out _, out _);

                grcReceipt.SetData(table);

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
        /// 거래처 조회
        /// </summary>
        private void ShowDataReceipt(string ReceiptID)
        {
            try
            {
                DxControl.ClearControl(lcgReceipt);

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetRow, SqlDbType.Char);
                inParams.Add("@i_ReceiptID", ReceiptID, SqlDbType.Char);

                var row = SqlHelper.GetDataRow("xp_Receipt", inParams, out _, out _);

                if (row != null)
                {
                    dteReceiptMon.EditValue = row["ReceiptMon"];
                    txtReceiptID.EditValue = row["ReceiptID"];
                    bteCustom.EditValue = row["KCustom"];
                    bteCustom.Tag = row["CustomID"];
                    txtAddress.EditValue = row["Address"];
                    bteCustomCharge.EditValue = row["ChargeName"];
                    bteCustomCharge.Tag = row["ChargeSeq"];
                    txtDutyName.EditValue = row["DutyName"];
                    txtCellPhoneNo.EditValue = row["CellPhoneNo"];
                    txtEMail.EditValue = row["EMail"];
                    //dteReceiptDate.EditValue = row["ReceiptDate"].CDateString();
                    gleEquipType.EditValue = row["EquipType"];
                    gleInterest.EditValue = row["Interest"];
                    gleReceiptState.EditValue = row["ReceiptState"];
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
        /// 거래처 정보 조회
        /// </summary>
        private void GetCustomInfo()
        {
            try
            {
                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", "91", SqlDbType.Char);
                inParams.Add("@i_CustomID", bteCustom.Tag.CString(), SqlDbType.Char);

                var row = SqlHelper.GetDataRow("xp_Receipt", inParams, out _, out _);

                if (row != null)
                {
                    txtAddress.EditValue = row["Address"];
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
        /// 거래처 담당자 정보 조회
        /// </summary>
        private void GetCustomChargeInfo()
        {
            try
            {
                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", "92", SqlDbType.Char);
                inParams.Add("@i_CustomID", bteCustom.Tag.CString(), SqlDbType.Char);
                inParams.Add("@i_ChargeSeq", bteCustomCharge.Tag.CString(), SqlDbType.TinyInt);

                var row = SqlHelper.GetDataRow("xp_Receipt", inParams, out _, out _);

                if (row != null)
                {
                    txtDutyName.EditValue = row["DutyName"];
                    txtCellPhoneNo.EditValue = row["CellPhoneNo"];
                    txtEMail.EditValue = row["Email"];
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
        /// 견적서 조회 : GRID
        /// </summary>
        private void FillGridQuotation()
        {
            try
            {
                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetList, SqlDbType.Char);
                inParams.Add("@i_ReceiptID", txtReceiptID.EditValue.CString());

                var table = SqlHelper.GetDataTable("xp_Quotation", inParams, out _, out _);
                grcQuotation.SetData(table);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 컨셉트 파일 리스트 조회
        /// </summary>
        private void FillGridConceptFile()
        {
            var groupName = grvReceipt.GetFocusedRowCellValue("ReceiptID").CString();

            var fileUpDown = new FileUpDown();
            fileUpDown.FillGridAttachFile(this, grcConceptFile, groupName);
        }

        /// <summary>
        /// 견적서 파일 리스트 조회
        /// </summary>
        private void FillGridQuotationFile()
        {
            var groupName = $"{grvReceipt.GetFocusedRowCellValue("ReceiptID")}{grvQuotation.GetFocusedRowCellValue("QuotID")}";

            var fileUpDown = new FileUpDown();
            fileUpDown.FillGridAttachFile(this, grcQuotationFile, groupName);
        }

        #endregion
    }
}
