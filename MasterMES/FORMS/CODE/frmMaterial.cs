using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Grid;
using DSLibrary;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;       //현재 열려있는 엑셀 확인
using Excel = Microsoft.Office.Interop.Excel;
using DevExpress.XtraEditors;
using MasterMES.CLASSES;

namespace MasterMES.FORMS.CODE
{
    public partial class frmMaterial : frmBase, IButtonAction
    {
        #region 생성자
        public frmMaterial()
        {
            InitializeComponent();

            speProperStock.SetEditMask(MaskType.Numeric, "N0");

            DxControl.ButtonEditEvent += ButtonEditEvent;
            Extensions.ButtonEditEvent += ButtonEditEvent;
        }

        #endregion

        #region 오버라이드 이벤트
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            MainButton.ActiveButtons("INIT,NEW,COPY,SAVE,DELETE");
        }

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            MainButton.ActiveButtons();
        }

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

                var initSet = SqlHelper.GetDataSet("xp_Material", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                gleUnit.SetData(initSet.Tables[0], "UseClss");
                gleMaterialType.SetData(initSet.Tables[1], "UseClss");
                grcMaterial.SetColumns(initSet.Tables[2], "UseClss");

                // 그리드 초기 형식 저장
                DxControl.SaveGridFormat(grvMaterial, GridFormat.Default);

                this.FillGridMaterial();
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
            DxControl.ClearControl(lcgMaterial);
            bteMainCustom.Tag = null;

            txtMaterialID.Focus();
        }

        /// <summary>
        /// 복사 버튼
        /// </summary>
        public void CopyButtonClick()
        {
            //try
            //{
            //    var field = new Dictionary<string, bool>()
            //    {
            //        { "MaterialID", txtMaterialID.EditValue.CString() == "" },
            //    };
            //    if (Utils.CheckField(field))
            //        return;

            //    var startTime = DateTime.Now;

            //    if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionCopy))
            //        return;

            //    Utils.ShowWaitForm(this);
            //    MainButton.CopyButton = false;

            //    var inParams = new DbParamList();
            //    inParams.Add("@i_ProcessID", ProcessType.Copy, SqlDbType.Char);
            //    inParams.Add("@i_MaterialID", txtMaterialID.EditValue.CString(), SqlDbType.Char);
            //    inParams.Add("@i_InptUser", AppConfig.Login.PersonID, SqlDbType.VarChar);

            //    SqlHelper.Execute("xp_Material", inParams, out string[] retVal, out string retCode, out string retMsg);

            //    if (retCode != "00")
            //        throw new Exception(retMsg);

            //    this.NewButtonClick();
            //    this.FillGridMaterial();

            //    // 신규 생성된 거래처 코드에 해당하는 데이터를 화면에 디스플레이(FocusedRowChanged Event 발생)
            //    grvMaterial.FindRowByValue("MaterialID", retVal[0]);

            //    SqlHelper.WriteLog(this, ActionLogType.Insert, startTime);
            //    MsgBox.OKMessage(MessageText.OKCopy);
            //}
            //catch (Exception ex)
            //{
            //    MsgBox.ErrorMessage(ex.CString());
            //}
            //finally
            //{
            //    MainButton.CopyButton = true;
            //    Utils.CloseWaitForm();
            //}
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
                    { "MaterialID", txtMaterialID.EditValue.CString() == "" },
                    { "MaterialName", txtMaterialName.EditValue.CString() == "" },
                    { "Spec", txtSpec.EditValue.CString() == "" },
                    { "Unit", gleUnit.EditValue.CString() == "" },
                    //{ "ProperStock", speProperStock.EditValue.CInt() == 0 },
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;
                var logType = ActionLogType.Insert;
                var message = MessageText.OKInsert;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Exist, SqlDbType.Char);
                inParams.Add("@i_MaterialID", txtMaterialID.EditValue.CString(), SqlDbType.Char);

                if (SqlHelper.Exist("xp_Material", inParams, out _, out _))
                {
                    if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionUpdate))
                        return;

                    logType = ActionLogType.Update;
                    message = MessageText.OKUpdate;
                }

                inParams.Clear();
                inParams.Add("@i_ProcessID", ProcessType.Merge, SqlDbType.Char);
                inParams.Add("@i_MaterialID", txtMaterialID.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_MaterialName", txtMaterialName.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_MaterialType", gleMaterialType.EditValue.CString(), SqlDbType.VarChar);
                inParams.Add("@i_Spec", txtSpec.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_DetailSpec", txtDetailSpec.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_Unit", gleUnit.EditValue.CString(), SqlDbType.VarChar);
                inParams.Add("@i_ProperStock", speProperStock.EditValue.CInt(), SqlDbType.Int);
                inParams.Add("@i_MainCustom", bteMainCustom.Tag.CString(), SqlDbType.Char);
                inParams.Add("@i_UseClss", chkUseClss.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_Remark", mmeRemark.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_InptUser", AppConfig.Login.PersonID, SqlDbType.VarChar);

                SqlHelper.Execute("xp_Material", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                var custCode = txtMaterialID.EditValue.CString();
                this.NewButtonClick();  // bteMainCustom의 tag값 초기화 필요
                this.FillGridMaterial();
                grvMaterial.FindRowByValue("MaterialID", custCode);

                SqlHelper.WriteLog(this, logType, startTime);
                MsgBox.OKMessage(message);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
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
                    { "MaterialID", txtMaterialID.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;

                if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionDelete))
                    return;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Delete, SqlDbType.Char);
                inParams.Add("@i_MaterialID", txtMaterialID.EditValue.CString(), SqlDbType.Char);

                SqlHelper.Execute("xp_Material", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                this.NewButtonClick();
                this.FillGridMaterial();

                SqlHelper.WriteLog(this, ActionLogType.Delete, startTime);
                MsgBox.OKMessage(MessageText.OKInsert);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 출력 버튼
        /// </summary>
        public void PrintButtonClick(int reportIndex)
        {
            ////Excel.Application oExcel = (Excel.Application)Marshal.GetActiveObject("Excel.Application");
            //Excel.Application oExcel;
            //Excel.Workbook oExcelBook;
            ////Excel.Worksheet oExcelSheet;

            //oExcel = new Excel.Application();
            //oExcel.Visible = true;

            //oExcelBook = oExcel.Workbooks.Open("C:\\test.xls");
        }

        #endregion

        #region 폼 컨트롤 이벤트
        /// <summary>
        /// 그리드 행변경시 화면에 정보 표시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvReceipt_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle >= 0)
            {
                var materialID = (sender as GridView).GetFocusedRowCellValue("MaterialID").CString();
                this.ShowDataMaterial(materialID);
            }
        }

        private void grvReceipt_RowClick(object sender, RowClickEventArgs e)
        {
            var materialID = (sender as GridView).GetFocusedRowCellValue("MaterialID").CString();
            this.ShowDataMaterial(materialID);
        }

        #endregion

        #region 사용자 정의 메서드

        /// <summary>
        /// 데이터 검색 이벤트 
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        private bool ButtonEditEvent(object sender)
        {
            ButtonEdit bte = sender as ButtonEdit;
            FindDataResult fdResult = FindDataResult.FIND_NONE;

            switch (bte.Name)
            {
                case "bteMainCustom":
                    fdResult = FindData.ShowFindData(FindData.FIND_CUSTOM, bte.EditValue.CString(), null, null, null, false, bte);
                    break;
            }

            return (fdResult == FindDataResult.FIND_OK);
        }

        /// <summary>
        /// 거래처 조회 : Grid
        /// </summary>
        private void FillGridMaterial()
        {
            try
            {
                var startTime = DateTime.Now;
                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetList, SqlDbType.Char);

                var table = SqlHelper.GetDataTable("xp_Material", inParams, out _, out _);

                grcMaterial.SetData(table);

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
        private void ShowDataMaterial(string custCode)
        {
            try
            {
                DxControl.ClearControl(lcgMaterial);

                var startTime = DateTime.Now;
                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetRow, SqlDbType.Char);
                inParams.Add("@i_MaterialID", custCode, SqlDbType.Char);

                var row = SqlHelper.GetDataRow("xp_Material", inParams, out _, out _);

                if (row != null)
                {
                    txtMaterialID.EditValue = row["MaterialID"];
                    txtMaterialName.EditValue = row["MaterialName"];
                    gleMaterialType.EditValue = row["MaterialType"];
                    txtSpec.EditValue = row["Spec"];
                    txtDetailSpec.EditValue = row["DetailSpec"];
                    gleUnit.EditValue = row["Unit"];
                    speProperStock.EditValue = row["ProperStock"];
                    bteMainCustom.Tag = row["MainCustom"];
                    bteMainCustom.EditValue = row["KCustom"];
                    chkUseClss.EditValue = row["UseClss"];
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
        #endregion
    }
}
