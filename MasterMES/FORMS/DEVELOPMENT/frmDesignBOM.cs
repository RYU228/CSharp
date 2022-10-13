using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Grid;
using DSLibrary;
using MasterMES.CLASSES;

namespace MasterMES.FORMS.DEVELOPMENT
{
    public partial class frmDesignBOM : frmBase, IButtonAction
    {
        #region 생성자
        public frmDesignBOM()
        {
            InitializeComponent();

            rdgMaterialType.SetGroupItem("01:Production|02:Purchase");
        }

        #endregion

        #region 오버라이드 이벤트
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

                var initSet = SqlHelper.GetDataSet("xp_DesignBOM", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                sleVCustomID.SetData(initSet.Tables[7], "UseClss");

                grcProject.SetColumns(initSet.Tables[0]);
                glePartCode.SetData(initSet.Tables[1], "UseClss");
                sleMaterialID.SetData(initSet.Tables[2], "UseClss");

                // 재질 선택
                grcQom.SetColumns(initSet.Tables[3], "MiddleCode,UseClss", false, true, true, false);
                grvQom.OptionsView.ShowColumnHeaders = false;
                grvQom.FixedColumnWidth("CheckColumn", 50);
                var riQomSelected = grvQom.SetRepositoryItemCheckEdit("CheckColumn");
                riQomSelected.EditValueChanged += RiQomSelected_EditValueChanged;

                // 가공 선택
                grcWorkType.SetColumns(initSet.Tables[4], "MiddleCode,UseClss", false, true, true, false);
                grvWorkType.OptionsView.ShowColumnHeaders = false;
                grvWorkType.FixedColumnWidth("CheckColumn", 50);
                var riWorkTypeSelected = grvWorkType.SetRepositoryItemCheckEdit("CheckColumn");
                riWorkTypeSelected.EditValueChanged += RiWorkTypeSelected_EditValueChanged;

                // 후처리 선택
                grcPostProcess.SetColumns(initSet.Tables[5], "MiddleCode,UseClss", false, true, true, false);
                grvPostProcess.OptionsView.ShowColumnHeaders = false;
                grvPostProcess.FixedColumnWidth("CheckColumn", 50);
                var riPostProcessSelected = grvPostProcess.SetRepositoryItemCheckEdit("CheckColumn");
                riPostProcessSelected.EditValueChanged += RiPostProcessSelected_EditValueChanged;

                gleUnit.SetData(initSet.Tables[6], "UseClss");
                sleCustomID.SetData(initSet.Tables[7], "UseClss");
                glePaintColor.SetData(initSet.Tables[8], "UseClss");
                grcDesignBOM.SetColumns(initSet.Tables[9]);

                this.NewButtonClick();

                // 그리드 초기 형식 저장
                DxControl.SaveGridFormat(grvProject, GridFormat.Default);
                DxControl.SaveGridFormat(grvDesignBOM, GridFormat.Default);
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
            txtMaterialSeq.EditValue = null;
            rdgMaterialType.EditValue = "01";
            glePartCode.EditValue = null;
            txtMapNo.EditValue = null;
            sleMaterialID.EditValue = null;
            txtMaterialName.EditValue = null;
            txtSpec.EditValue = null;
            grvQom.SetValueRange(0, grvQom.RowCount - 1, new string[] { "CheckColumn" }, "1");
            grvWorkType.SetValueRange(0, grvWorkType.RowCount - 1, new string[] { "CheckColumn" }, "1");
            grvPostProcess.SetValueRange(0, grvPostProcess.RowCount - 1, new string[] { "CheckColumn" }, "1");
            glePaintColor.EditValue = null;
            gleUnit.EditValue = null;
            speReqQty.EditValue = 0;
            txtMakerName.EditValue = null;
            sleCustomID.EditValue = null;
            mmeRRemark.EditValue = null;

            glePaintColor.Enabled = false;
        }

        /// <summary>
        /// 저장 버튼
        /// </summary>
        public void SaveButtonClick()
        {
            try
            {
                // 재질/내용, 가공, 후처리 체크여부
                var qmChecked = (grcQom.DataSource as DataTable).DefaultView.ToTable().Select("CheckColumn = '0'");
                var wtChecked = (grcWorkType.DataSource as DataTable).DefaultView.ToTable().Select("CheckColumn = '0'");
                var ppChecked = (grcPostProcess.DataSource as DataTable).DefaultView.ToTable().Select("CheckColumn = '0'");

                var field = new Dictionary<string, bool>()
                {
                    { "ProjectNo", txtProjectNo.EditValue.CString() == "" },
                    { "PartCode", glePartCode.EditValue == null },
                    { "Qom Info", qmChecked == null || qmChecked.Length == 0 },
                    { "WorkType Info", wtChecked == null || wtChecked.Length == 0 },
                    { "PostProcess Info", ppChecked == null || ppChecked.Length == 0 },
                    { "ReqQty", speReqQty.EditValue.CInt() == 0 },
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;
                var logType = ActionLogType.Insert;
                var message = MessageText.OKInsert;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Exist, SqlDbType.Char);
                inParams.Add("@i_ProjectNo", txtProjectNo.EditValue.CString().Replace("-", ""), SqlDbType.Char);
                inParams.Add("@i_MaterialSeq", txtMaterialSeq.EditValue.CInt(), SqlDbType.Int);

                if (SqlHelper.Exist("xp_DesignBOM", inParams, out _, out _))
                {
                    if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionUpdate))
                        return;

                    logType = ActionLogType.Update;
                    message = MessageText.OKUpdate;
                }

                // xml data 생성
                var qmXmlData = (grcQom.DataSource as DataTable).DefaultView.ToTable(false, new string[] { "CheckColumn", "MiddleCode" })
                                                                .Select("CheckColumn = '0'").CopyToDataTable().ToXml();
                var wtXmlData = (grcWorkType.DataSource as DataTable).DefaultView.ToTable(false, new string[] { "CheckColumn", "MiddleCode" })
                                                                     .Select("CheckColumn = '0'").CopyToDataTable().ToXml();
                var ppXmlData = (grcPostProcess.DataSource as DataTable).DefaultView.ToTable(false, new string[] { "CheckColumn", "MiddleCode" })
                                                                        .Select("CheckColumn = '0'").CopyToDataTable().ToXml();

                inParams.Clear();
                inParams.Add("@i_ProcessID", ProcessType.Merge, SqlDbType.Char);
                inParams.Add("@i_ProjectNo", txtProjectNo.EditValue.CString().Replace("-", ""), SqlDbType.Char);
                inParams.Add("@i_MaterialSeq", txtMaterialSeq.EditValue.CInt(), SqlDbType.Int);
                inParams.Add("@i_MaterialType", rdgMaterialType.EditValue.CString(), SqlDbType.VarChar);
                inParams.Add("@i_PartCode", glePartCode.EditValue.CString(), SqlDbType.VarChar);
                inParams.Add("@i_MapNo", txtMapNo.EditValue.CString(), SqlDbType.VarChar);

                if (rdgMaterialType.EditValue.CString() == "01")    // 제작품
                {
                    inParams.Add("@i_MaterialID", string.Empty, SqlDbType.VarChar);
                    inParams.Add("@i_MaterialName", txtMaterialName.EditValue.CString(), SqlDbType.VarChar);
                    inParams.Add("@i_Spec", txtSpec.EditValue.CString(), SqlDbType.VarChar);
                }
                else // 구매품
                {
                    inParams.Add("@i_MaterialID", sleMaterialID.EditValue.CString(), SqlDbType.VarChar);
                    inParams.Add("@i_MaterialName", string.Empty, SqlDbType.VarChar);
                    inParams.Add("@i_Spec", string.Empty, SqlDbType.VarChar);
                }

                inParams.Add("@i_QMXmlData", qmXmlData, SqlDbType.VarChar);
                inParams.Add("@i_WTXmlData", wtXmlData, SqlDbType.VarChar);
                inParams.Add("@i_PPXmlData", ppXmlData, SqlDbType.VarChar);

                inParams.Add("@i_PaintColor", glePaintColor.EditValue.CString(), SqlDbType.VarChar);
                inParams.Add("@i_Unit", gleUnit.EditValue.CString(), SqlDbType.VarChar);
                inParams.Add("@i_ReqQty", speReqQty.EditValue.CInt(), SqlDbType.Int);
                inParams.Add("@i_MakerName", txtMakerName.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_CustomID", sleCustomID.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_Remark", mmeRRemark.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_InptUser", AppConfig.Login.PersonID, SqlDbType.VarChar);

                SqlHelper.Execute("xp_DesignBOM", inParams, out string[] retVal, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                this.FillGridDesignBOM(txtProjectNo.EditValue.CString().Replace("-", ""));
                grvDesignBOM.FindRowByValue("MaterialSeq", retVal[0].CInt());

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
                    { "MaterialSeq", txtMaterialSeq.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;

                if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionDelete))
                    return;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Delete, SqlDbType.Char);
                inParams.Add("@i_ProjectNo", txtProjectNo.EditValue.CString().Replace("-", ""), SqlDbType.Char);
                inParams.Add("@i_MaterialSeq", txtMaterialSeq.EditValue.CInt(), SqlDbType.Int);

                SqlHelper.Execute("xp_DesignBOM", inParams, out string retCode, out string retMsg);

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
            this.NewProjectInfo();
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
        /// 제작품/구매품에 따른 입력컨트롤 활성화
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdgMaterialType_EditValueChanged(object sender, EventArgs e)
        {
            sleMaterialID.Enabled = false;
            txtMaterialName.Enabled = false;
            txtSpec.Enabled = false;

            sleMaterialID.EditValue = null;
            txtMaterialName.EditValue = null;
            txtSpec.EditValue = null;

            switch (rdgMaterialType.EditValue.CString())
            {
                case "01":  // 제작품
                    txtMaterialName.Enabled = true;
                    txtSpec.Enabled = true;
                    break;

                case "02":  // 구매품
                    sleMaterialID.Enabled = true;
                    break;
            }
        }

        /// <summary>
        /// 구매품 선택시 자재명과 규격 표시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sleMaterialID_EditValueChanged(object sender, EventArgs e)
        {
            // 구매품목 선택시 자재명, 규격 표시
            if (rdgMaterialType.EditValue.CString() == "02")
            {
                var row = (sleMaterialID.Properties.GetRowByKeyValue(sleMaterialID.EditValue) as DataRowView).Row;
                txtMaterialName.EditValue = row["MaterialName"];
                txtSpec.EditValue = row["Spec"];
            }
        }

        /// <summary>
        /// 재질 선택 변경시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RiQomSelected_EditValueChanged(object sender, EventArgs e)
        {
            if (grvQom.PostEditor())
                grvQom.UpdateCurrentRow();
        }

        /// <summary>
        /// 가공 선택 변경시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RiWorkTypeSelected_EditValueChanged(object sender, EventArgs e)
        {
            if (grvWorkType.PostEditor())
                grvWorkType.UpdateCurrentRow();
        }

        /// <summary>
        /// 후처리 선택 변경시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RiPostProcessSelected_EditValueChanged(object sender, EventArgs e)
        {
            if (grvPostProcess.PostEditor())
                grvPostProcess.UpdateCurrentRow();

            // 도색이 선택되면 도색색상을 활성화
            glePaintColor.Enabled = (grvPostProcess.GetFocusedRowCellValue("CheckColumn").CString() == "0");
            glePaintColor.EditValue = null;
        }

        /// <summary>
        /// 프로젝트 그리드 행 변경시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvProject_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle >= 0)
            {
                var projectNo = (sender as GridView).GetFocusedRowCellValue("ProjectNo").CString().Replace("-", "");
                this.ShowDataProject(projectNo);
                this.FillGridDesignBOM(projectNo);
            }
        }

        /// <summary>
        /// 프로젝트 그리드 행 클릭시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvProject_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            var projectNo = (sender as GridView).GetFocusedRowCellValue("ProjectNo").CString().Replace("-", "");
            this.ShowDataProject(projectNo);
            this.FillGridDesignBOM(projectNo);
        }

        /// <summary>
        /// BOM(소요자재) 그리드 행 변경시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvDesignBOM_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle >= 0)
            {
                this.ShowDataDesignBOM(txtProjectNo.EditValue.CString().Replace("-", ""),
                                      (sender as GridView).GetFocusedRowCellValue("MaterialSeq").CInt());
            }
        }

        /// <summary>
        /// BOM(소요자재) 그리드 행 클릭시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvDesignBOM_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            this.ShowDataDesignBOM(txtProjectNo.EditValue.CString().Replace("-", ""),
                                  (sender as GridView).GetFocusedRowCellValue("MaterialSeq").CInt());
        }

        #endregion

        #region 사용자 정의 메서드
        /// <summary>
        /// 프로젝트 정보 표시란 초기화
        /// </summary>
        private void NewProjectInfo()
        {
            txtProjectNo.EditValue = null;
            //txtReceiptID.EditValue = null;
            txtProjectName.EditValue = null;
            //txtContractDate.EditValue = null;
            txtDeliveryDate.EditValue = null;
            //txtOrderStatus.EditValue = null;
            //speContractAmt.EditValue = null;
            mmePRemark.EditValue = null;

            grvDesignBOM.DeleteAllRows();
        }

        /// <summary>
        /// 계약 기간내 프로젝트 리스트 조회 - 그리드
        /// </summary>
        private void FillGridProject()
        {
            try
            {
                Utils.ShowWaitForm(this);

                var startTime = DateTime.Now;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", "81", SqlDbType.Char);
                inParams.Add("@i_SContractDate", dteSContractDate.EditValue.CDateString());
                inParams.Add("@i_EContractDate", dteEContractDate.EditValue.CDateString());
                inParams.Add("@i_VCustomID", sleVCustomID.EditValue.CString());

                var table = SqlHelper.GetDataTable("xp_DesignBOM", inParams, out _, out string retMsg);

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
        /// <param name="projectNo"></param>
        private void ShowDataProject(string projectNo)
        {
            try
            {
                this.NewButtonClick();

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", "91", SqlDbType.Char);
                inParams.Add("@i_ProjectNo", projectNo, SqlDbType.Char);

                var row = SqlHelper.GetDataRow("xp_DesignBOM", inParams, out _, out _);

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
            finally
            {
                Utils.CloseWaitForm();
            }
        }

        /// <summary>
        /// BOM(소요자재) 조회 - 그리드
        /// </summary>
        /// <param name="projectNo"></param>
        private void FillGridDesignBOM(string projectNo)
        {
            try
            {
                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetList, SqlDbType.Char);
                inParams.Add("@i_ProjectNo", projectNo, SqlDbType.Char);

                var table = SqlHelper.GetDataTable("xp_DesignBOM", inParams, out _, out _);

                grcDesignBOM.SetData(table);
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
        /// BOM(소요자재) 조회 - 행
        /// </summary>
        /// <param name="projectNo"></param>
        /// <param name="materialSeq"></param>
        private void ShowDataDesignBOM(string projectNo, int materialSeq)
        {
            try
            {
                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetRow, SqlDbType.Char);
                inParams.Add("@i_ProjectNo", projectNo, SqlDbType.Char);
                inParams.Add("@i_MaterialSeq", materialSeq, SqlDbType.Int);

                var ds = SqlHelper.GetDataSet("xp_DesignBOM", inParams, out _, out _);

                if (ds != null)
                {
                    if (ds.Tables[0].Rows != null && ds.Tables[0].Rows.Count > 0)
                    {
                        var row = ds.Tables[0].Rows[0];

                        txtMaterialSeq.EditValue = row["MaterialSeq"];
                        rdgMaterialType.EditValue = row["MaterialType"];
                        glePartCode.EditValue = row["PartCode"];
                        txtMapNo.EditValue = row["MapNo"];

                        // 제작품
                        if (row["MaterialType"].CString() == "01")
                        {
                            txtMaterialName.EditValue = row["MaterialName"];
                            txtSpec.EditValue = row["Spec"];
                        }
                        else
                        {
                            sleMaterialID.EditValue = row["MaterialID"];
                        }

                        glePaintColor.EditValue = row["PaintColor"];
                        glePaintColor.Enabled = (row["PaintColor"].CString() != "");

                        gleUnit.EditValue = row["Unit"];
                        speReqQty.EditValue = row["ReqQty"];
                        txtMakerName.EditValue = row["MakerName"];
                        sleCustomID.EditValue = row["CustomID"];
                        mmeRRemark.EditValue = row["Remark"];
                    }

                    grcQom.SetData(ds.Tables[1]);
                    grcWorkType.SetData(ds.Tables[2]);
                    grcPostProcess.SetData(ds.Tables[3]);
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