using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Grid;
using DSLibrary;
using MasterMES.CLASSES;
using MasterMES.FORMS.ETC;

namespace MasterMES.FORMS.MATLIN
{
    public partial class frmMaterialIn : frmBase, IButtonAction
    {
        #region 생성자
        public frmMaterialIn()
        {
            InitializeComponent();

            // 조회조건 - 월단위로 표시
            dteSContractDate.SetViewStyle(ViewStyle.MonthView);
            dteEContractDate.SetViewStyle(ViewStyle.MonthView);

            // 이미지 위치 설정
            picMaterialIn.Properties.PictureAlignment = ContentAlignment.TopCenter;
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

                var initSet = SqlHelper.GetDataSet("xp_MaterialIn", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                tlsPO.SetColumns(initSet.Tables[0], "PONo", autoColumnWidth: true);
                grcPOSub.SetColumns(initSet.Tables[1], "MaterialSeq");

                // 자재입고
                var riMaterialInIn = grvPOSub.SetRepositoryItemSimpleButton("MaterialIn", RepositoryButtonType.Add, "입고");
                riMaterialInIn.Click += RiMaterialInIn_Click;

                // 자재취소
                var riMaterialInCancel = grvPOSub.SetRepositoryItemSimpleButton("MaterialCancel", RepositoryButtonType.Delete, "취소");
                riMaterialInCancel.Click += RiMaterialInCancel_Click;

                // 자재사진첨부
                var riMaterialInImg = grvPOSub.SetRepositoryItemSimpleButton("MaterialImg", RepositoryButtonType.Find, "첨부");
                riMaterialInImg.Click += RiMaterialInImg_Click;

                // 발주서 상세 그리드
                DxControl.SaveGridFormat(grvPOSub, GridFormat.Default);
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
                inParams.Add("@i_ProjectNo", txtProjectNo.EditValue.CString(), SqlDbType.Char);

                if (SqlHelper.Exist("xp_MaterialIn", inParams, out _, out _))
                {
                    logType = ActionLogType.Update;
                    message = MessageText.OKUpdate;
                }

                inParams.Clear();
                inParams.Add("@i_ProcessID", ProcessType.Merge, SqlDbType.Char);
                inParams.Add("@i_ProjectNo", txtProjectNo.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_InptUser", AppConfig.Login.PersonID, SqlDbType.VarChar);

                SqlHelper.Execute("xp_MaterialIn", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                // 프로젝트 그리드 재조회 및 행 찾기
                this.FillTreePO();
                //grvPO.FindRowByValue("ProjectNo", txtProjectNo.EditValue);

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
                inParams.Add("@i_ProjectNo", txtProjectNo.EditValue.CString(), SqlDbType.Char);

                SqlHelper.Execute("xp_MaterialIn", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                // 프로젝트 그리드 재조회 및 행 찾기
                this.FillTreePO();
                //grvPO.FindRowByValue("ProjectNo", txtProjectNo.EditValue);

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
            this.FillTreePO();
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
        /// 활성화 노드 변경시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tlsPO_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            this.ShowDataPO(e.Node["PONo"].CString());
            this.FillTreePOSub();
        }

        /// <summary>
        /// 노드 클릭시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tlsPO_RowClick(object sender, DevExpress.XtraTreeList.RowClickEventArgs e)
        {
            this.ShowDataPO(e.Node["PONo"].CString());
            this.FillTreePOSub();
        }

        /// <summary>
        /// 자재입고 입고
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RiMaterialInIn_Click(object sender, EventArgs e)
        {
            if (grvPOSub.GetFocusedRowCellValue("InStatus").CString() == "입고") return;

            var poNo = txtPONo.EditValue.CString();
            var materialSeq = grvPOSub.GetFocusedRowCellValue("MaterialSeq").CInt();
            var ordQty = grvPOSub.GetFocusedRowCellValue("OrdQty").CInt();

            this.SaveDataMaterialIn(poNo, materialSeq, ordQty, "2");
            this.FillTreePO();
        }

        /// <summary>
        /// 자재입고 취소
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RiMaterialInCancel_Click(object sender, EventArgs e)
        {
            if (grvPOSub.GetFocusedRowCellValue("InStatus").CString() == "취소") return;

            var poNo = txtPONo.EditValue.CString();
            var materialSeq = grvPOSub.GetFocusedRowCellValue("MaterialSeq").CInt();

            this.SaveDataMaterialIn(poNo, materialSeq, 0, "3");
            this.FillTreePO();
        }

        /// <summary>
        /// 자재입고 첨부
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RiMaterialInImg_Click(object sender, EventArgs e)
        {
            if (grvPOSub.GetFocusedRowCellValue("InStatus").CString() == "미입고" || grvPOSub.GetFocusedRowCellValue("InStatus").CString() == "취소")
            {
                MsgBox.ErrorMessage("입고처리 후 사진첨부가 가능합니다.");
                return;
            }

            try
            {
                var field = new Dictionary<string, bool>()
                {
                    { "PONo", txtPONo.EditValue.CString() == "" },
                    { "MaterialSeq", grvPOSub.GetFocusedRowCellValue("MaterialSeq").CInt() == 0 },
                };
                if (Utils.CheckField(field))
                    return;

                var menuID = Utils.GetMenuID(this);
                var groupName = txtPONo.EditValue.CString();

                var frmFileDialog = new frmFileDlg(menuID, groupName);
                frmFileDialog.ShowDialog();

                var fileUpDown = new FileUpDown();
                fileUpDown.ImageDataToControl(this,
                                              txtPONo.EditValue.CString(),
                                              grvPOSub.GetFocusedRowCellValue("MaterialSeq").CInt(),
                                              picMaterialIn);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 자재 발주 상세 행 변경시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvPOSub_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle >= 0)
            {
                var fileUpDown = new FileUpDown();
                fileUpDown.ImageDataToControl(this,
                                              txtPONo.EditValue.CString(),
                                              grvPOSub.GetFocusedRowCellValue("MaterialSeq").CInt(),
                                              picMaterialIn);
            }
        }

        /// <summary>
        /// 자재 발주 상세 행 클릭시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvPOSub_RowClick(object sender, RowClickEventArgs e)
        {
            var fileUpDown = new FileUpDown();
            fileUpDown.ImageDataToControl(this,
                                          txtPONo.EditValue.CString(),
                                          grvPOSub.GetFocusedRowCellValue("MaterialSeq").CInt(),
                                          picMaterialIn);
        }
        
        #endregion

        #region 사용자 정의 메소드
        /// <summary>
        /// 발주서 리스트 조회 - 그리드
        /// </summary>
        private void FillTreePO()
        {
            try
            {
                var startTime = DateTime.Now;

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetList, SqlDbType.Char);
                //inParams.Add("@i_SContractDate", dteSContractDate.EditValue.CDateString());
                //inParams.Add("@i_EContractDate", dteEContractDate.EditValue.CDateString());

                var table = SqlHelper.GetDataTable("xp_MaterialIn", inParams, out _, out _);

                tlsPO.SetData(table, 1);

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
        /// 발주서 상세 리스트 조회 - 그리드
        /// </summary>
        private void FillTreePOSub()
        {
            try
            {
                var startTime = DateTime.Now;

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", "81", SqlDbType.Char);
                inParams.Add("@i_PONo", txtPONo.EditValue.CString());

                var table = SqlHelper.GetDataTable("xp_MaterialIn", inParams, out _, out _);

                grcPOSub.SetData(table);

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
        /// 발주서 정보 화면 표시 - 행
        /// </summary>
        private void ShowDataPO(string poNo)
        {
            try
            {
                this.NewButtonClick();

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetRow, SqlDbType.Char);
                inParams.Add("@i_PONo", poNo, SqlDbType.Char);

                var row = SqlHelper.GetDataRow("xp_MaterialIn", inParams, out _, out _);

                if (row != null)
                {
                    txtPONo.EditValue = row["PONo"];
                    txtProjectNo.EditValue = row["ProjectNo"];
                    txtKCustom.EditValue = row["KCustom"];
                    txtCreatePerson.EditValue = row["CreatePerson"];
                    //txtCreatePerson.EditValue = row["DueDate"].CDateString();
                    txtDueDate.EditValue = Convert.ToDateTime(row["DueDate"]).ToString("yyyy년 MM월 dd일");
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

        
        private void SaveDataMaterialIn(string poNo, int materialSeq, int ordQty, string inStatus)
        {
            try
            {
                var field = new Dictionary<string, bool>()
                {
                    { "PONo", poNo == "" },
                    { "MaterialSeq", materialSeq == 0 },
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;
                var logType = ActionLogType.Insert;
                var message = MessageText.OKInsert;

                //var inParams = new DbParamList();
                //inParams.Add("@i_ProcessID", ProcessType.Exist, SqlDbType.Char);
                //inParams.Add("@i_ProjectNo", txtProjectNo.EditValue.CString(), SqlDbType.Char);

                //if (SqlHelper.Exist("xp_MaterialIn", inParams, out _, out _))
                //{
                //    logType = ActionLogType.Update;
                //    message = MessageText.OKUpdate;
                //}

                //inParams.Clear();

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Merge, SqlDbType.Char);
                inParams.Add("@i_PONo", poNo, SqlDbType.Char);
                inParams.Add("@i_MaterialSeq", materialSeq, SqlDbType.Int);
                inParams.Add("@i_MaterialID", grvPOSub.GetFocusedRowCellValue("MaterialID").CString(), SqlDbType.VarChar);
                inParams.Add("@i_MaterialName", grvPOSub.GetFocusedRowCellValue("MaterialName").CString(), SqlDbType.NVarChar);
                inParams.Add("@i_Spec", grvPOSub.GetFocusedRowCellValue("Spec").CString(), SqlDbType.NVarChar);
                inParams.Add("@i_InQty", ordQty, SqlDbType.Int);
                inParams.Add("@i_InStatus", inStatus, SqlDbType.Char);
                inParams.Add("@i_InptUser", AppConfig.Login.PersonID, SqlDbType.VarChar);

                SqlHelper.Execute("xp_MaterialIn", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                // 프로젝트 그리드 재조회 및 행 찾기
                this.FillTreePO();
                //grvPO.FindRowByValue("PONo", txtPONo.EditValue);

                SqlHelper.WriteLog(this, logType, startTime);
                MsgBox.OKMessage(message);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        #endregion
    }
}