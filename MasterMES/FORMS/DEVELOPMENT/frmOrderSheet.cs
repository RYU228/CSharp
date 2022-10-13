using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Grid;
using DSLibrary;
using MasterMES.CLASSES;
using MasterMES.FORMS.ETC;

namespace MasterMES.FORMS.DEVELOPMENT
{
    public partial class frmOrderSheet : frmBase, IButtonAction
    {
        #region 생성자
        public frmOrderSheet()
        {
            InitializeComponent();

            // 파일 업로드 버튼 생성
            lcgOrderSheetFile.SetHeaderButtons("FileUpload");

            // 이미지 위치 설정
            picOrderSheet.Properties.PictureAlignment = ContentAlignment.TopCenter;
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

                var initSet = SqlHelper.GetDataSet("xp_OrderSheet", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                grcProject.SetColumns(initSet.Tables[0]);
                grcOrderSheetFile.SetColumns(initSet.Tables[1], "GroupSeq", false, false); // 오더시트 파일 첨부 그리드
                sleCustomID.SetData(initSet.Tables[2], "UseClss");

                // 오더시트 파일 다운로드
                var riOrderSheetFileDownload = grvOrderSheetFile.SetRepositoryItemSimpleButton("FileDownload", RepositoryButtonType.Download);
                riOrderSheetFileDownload.Click += RiOrderSheetFileDownload_Click;

                // 오더시트 파일 삭제
                var riOrderSheetFileDelete = grvOrderSheetFile.SetRepositoryItemSimpleButton("FileDelete", RepositoryButtonType.Delete);
                riOrderSheetFileDelete.Click += RiOrderSheetFileDelete_Click;

                // 그리드 초기 형식 저장
                DxControl.SaveGridFormat(grvProject, GridFormat.Default);
                DxControl.SaveGridFormat(grvOrderSheetFile, GridFormat.Default);
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
            //txtReceiptID.EditValue = null;
            txtProjectName.EditValue = null;
            //txtContractDate.EditValue = null;
            txtDeliveryDate.EditValue = null;
            //txtOrderStatus.EditValue = null;
            //speContractAmt.EditValue = 0;
            mmeRemark.EditValue = null;
            grvOrderSheetFile.DeleteAllRows();
            picOrderSheet.Image = null;
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

                // 입력사항이 없으므로 메세지를 출력하지 않고 Insert/Update만 구분
                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Exist, SqlDbType.Char);
                inParams.Add("@i_ProjectNo", txtProjectNo.EditValue.CString().Replace("-", ""), SqlDbType.Char);

                if (SqlHelper.Exist("xp_OrderSheet", inParams, out _, out _))
                {
                    logType = ActionLogType.Update;
                    message = MessageText.OKUpdate;
                }

                inParams.Clear();
                inParams.Add("@i_ProcessID", ProcessType.Merge, SqlDbType.Char);
                inParams.Add("@i_ProjectNo", txtProjectNo.EditValue.CString().Replace("-", ""), SqlDbType.Char);
                inParams.Add("@i_InptUser", AppConfig.Login.PersonID, SqlDbType.VarChar);

                SqlHelper.Execute("xp_OrderSheet", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                // 프로젝트 그리드 재조회 및 행 찾기
                var findValue = txtProjectNo.EditValue;
                this.FillGridProject();
                grvProject.FindRowByValue("ProjectNo", findValue);

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
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;

                if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionDelete))
                    return;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Delete, SqlDbType.Char);
                inParams.Add("@i_ProjectNo", txtProjectNo.EditValue.CString().Replace("-", ""), SqlDbType.Char);

                SqlHelper.Execute("xp_OrderSheet", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                // 프로젝트 그리드 재조회 및 행 찾기
                var findValue = txtProjectNo.EditValue;
                this.FillGridProject();
                grvProject.FindRowByValue("ProjectNo", findValue);

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
        /// 프로젝트 그리드 행 변경시 - 프로젝트 정보 화면 표시 및 오더시트 첨부파일 조회
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvProject_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle >= 0)
            {
                var projectNo = (sender as GridView).GetFocusedRowCellValue("ProjectNo").CString().Replace("-", "");
                this.ShowDataProject(projectNo);
                this.FillGridOrderSheetFile();
            }
        }

        /// <summary>
        /// 프로젝트 그리드 행 클릭시 - 프로젝트 정보 화면 표시 및 오더시트 첨부파일 조회
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvProject_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            var projectNo = (sender as GridView).GetFocusedRowCellValue("ProjectNo").CString().Replace("-", "");
            this.ShowDataProject(projectNo);
            this.FillGridOrderSheetFile();
        }

        /// <summary>
        /// 오더 시트 파일 첨부
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lcgOrderSheetFile_CustomButtonClick(object sender, DevExpress.XtraBars.Docking2010.BaseButtonEventArgs e)
        {
            if (e.Button.Properties.Tag.CString() != "FileUpload")
                return;

            try
            {
                var field = new Dictionary<string, bool>()
                {
                    { "ProjectNo", txtProjectNo.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                var menuID = Utils.GetMenuID(this);
                var groupName = txtProjectNo.EditValue.CString().Replace("-", "");

                var frmFileDialog = new frmFileDlg(menuID, groupName);
                frmFileDialog.ShowDialog();

                (new FileUpDown()).FillGridAttachFile(this, grcOrderSheetFile, groupName);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 오더시트 첨부파일 다운로드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RiOrderSheetFileDownload_Click(object sender, EventArgs e)
        {
            var fileUpDown = new FileUpDown();
            fileUpDown.DownloadAttachFile(this,
                                          txtProjectNo.EditValue.CString().Replace("-", ""),
                                          grvOrderSheetFile.GetFocusedRowCellValue("GroupSeq").CInt(),
                                          grvOrderSheetFile.GetFocusedRowCellValue("FileName").CString());
        }

        /// <summary>
        /// 오더시트 첨부파일 삭제
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RiOrderSheetFileDelete_Click(object sender, EventArgs e)
        {
            var fileUpDown = new FileUpDown();
            fileUpDown.DeleteAttachFile(this,
                                        txtProjectNo.EditValue.CString().Replace("-", ""),
                                        grvOrderSheetFile.GetFocusedRowCellValue("GroupSeq").CInt());
            fileUpDown.FillGridAttachFile(this, grcOrderSheetFile, txtProjectNo.EditValue.CString().Replace("-", ""));
        }

        /// <summary>
        /// 그림파일 화면 표시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvOrderSheetFile_DoubleClick(object sender, EventArgs e)
        {
            var view = sender as GridView;
            var info = view.CalcHitInfo(view.GridControl.PointToClient(Control.MousePosition));

            if ((info.InRow || info.InRowCell) && info.RowHandle >= 0)
            {
                var fileUpDown = new FileUpDown();
                fileUpDown.ImageDataToControl(this,
                                              txtProjectNo.EditValue.CString().Replace("-", ""),
                                              grvOrderSheetFile.GetFocusedRowCellValue("GroupSeq").CInt(),
                                              picOrderSheet);
            }
        }
        
        #endregion

        #region 사용자 정의 메소드
        /// <summary>
        /// 계약 기간내 프로젝트 리스트 조회 - 그리드
        /// </summary>
        private void FillGridProject()
        {
            try
            {
                var startTime = DateTime.Now;

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetList, SqlDbType.Char);
                inParams.Add("@i_SContractDate", dteSContractDate.EditValue.CDateString());
                inParams.Add("@i_EContractDate", dteEContractDate.EditValue.CDateString());
                inParams.Add("@i_CustomID", sleCustomID.EditValue.CString());

                var table = SqlHelper.GetDataTable("xp_OrderSheet", inParams, out _, out _);

                grcProject.SetData(table);
                
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
        /// 프로젝트 정보 화면 표시 - 행
        /// </summary>
        private void ShowDataProject(string projectNo)
        {
            try
            {
                this.NewButtonClick();

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetRow, SqlDbType.Char);
                inParams.Add("@i_ProjectNo", projectNo, SqlDbType.Char);

                var row = SqlHelper.GetDataRow("xp_OrderSheet", inParams, out _, out _);

                if (row != null)
                {
                    txtProjectNo.EditValue = row["ProjectNo"];
                    //txtReceiptID.EditValue = row["ReceiptID"];
                    txtProjectName.EditValue = row["ProjectName"];
                    //txtContractDate.EditValue = row["ContractDate"].CDateString();
                    txtDeliveryDate.EditValue = row["DeliveryDate"].CDateString();
                    //txtOrderStatus.EditValue = row["OrderStatus"];
                    //speContractAmt.EditValue = row["ContractAmt"];
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
        /// 오더시트 첨부파일 리스트 조회
        /// </summary>
        private void FillGridOrderSheetFile()
        {
            (new FileUpDown()).FillGridAttachFile(this,
                                                  grcOrderSheetFile,
                                                  txtProjectNo.EditValue.CString().Replace("-", ""));
        }

        #endregion
    }
}