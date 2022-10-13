using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DSLibrary;
using MasterMES.CLASSES;

namespace MasterMES.FORMS.SAMPLE
{
    public partial class frmSampleOrder : frmBase, IButtonAction
    {
        #region 생성자
        public frmSampleOrder()
        {
            InitializeComponent();

            DxControl.ButtonEditEvent += ButtonEditEvent;
            Extensions.ButtonEditEvent += ButtonEditEvent;
        }
        #endregion

        #region 오버라이드 이벤트
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            InitButtonClick();
        }
        #endregion

        #region 인터페이스 - 공용 버튼 클릭
        /// <summary>
        /// 초기화 버튼
        /// </summary>
        public void InitButtonClick()
        {
            // Grid 초기화
            InitGrid();

            // 검색 컨트롤 초기화
            dteSearchDateS.Tag = "M-1";
            dteSearchDateS.SetDefaultDate();
            dteSearchDateE.Tag = "D";
            dteSearchDateE.SetDefaultDate();
            txtSearchOrderNo.EditValue = "";
            txtSearchOrderNo.Tag = "";
            bteSearchCustom.EditValue = "";
            bteSearchCustom.Tag = "";

            // 입력 컨트롤 초기화
            ClearData();

            // 조회
            ViewButtonClick();
        }

        /// <summary>
        /// 조회 버튼
        /// </summary>
        public void ViewButtonClick()
        {
            FillGridOrder();
        }

        /// <summary>
        /// 신규 버튼
        /// </summary>
        public void NewButtonClick()
        {
            // 입력 컨트롤 초기화
            ClearData();
        }

        /// <summary>
        /// 복사 버튼
        /// </summary>
        public void CopyButtonClick()
        {
            txtOrderID.EditValue = "";
            txtOrderNo.EditValue = "";
        }

        /// <summary>
        /// 저장 버튼
        /// </summary>
        public void SaveButtonClick()
        {
            SaveData();
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
        /// grvOrderList의 Row 포커스 변경 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvOrderList_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle >= 0)
                ShowData();
        }

        /// <summary>
        /// grvOrderList의 Row 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvOrderList_RowClick(object sender, RowClickEventArgs e)
        {
            ShowData();
        }

        /// <summary>
        /// grvColorList KeyUp 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvColorList_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                (sender as GridView).MoveCellFocus();
        }

        /// <summary>
        /// grvColorList 입력 활성화 표시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grvColorList_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            if (e.RowHandle >= 0)
            {
                if (new string[] { "Article", "DesignNo", "Color", "ColorRoll", "ColorQty", "UnitPrice", "Remark" }.Contains(e.Column.FieldName))
                {
                    e.Appearance.ForeColor = Color.White;
                    e.Appearance.BackColor = UserColors.EditableColor;
                }
            }
        }

        /// <summary>
        /// LayoutControlGroup CustomHeader Button Click Event (행추가, 행삭제, 위로이동, 아래로이동)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lcgColorList_CustomButtonClick(object sender, DevExpress.XtraBars.Docking2010.BaseButtonEventArgs e)
        {
            int index;
            DataRow row1, row2;
            object[] row;

            switch (e.Button.Properties.Tag.CString())
            {
                case "AddRow":
                    grvColorList.AddNewRow();
                    grvColorList.BestFitColumns();
                    grvColorList.UpdateCurrentRow();
                    break;

                case "DeleteRow":
                    grvColorList.DeleteRow(grvColorList.FocusedRowHandle);
                    break;

                case "MoveUp":
                    grvColorList.GridControl.Focus();
                    index = grvColorList.FocusedRowHandle;

                    if (index <= 0)
                        return;

                    row1 = grvColorList.GetDataRow(index);
                    row2 = grvColorList.GetDataRow(index - 1);

                    row = row1.ItemArray;
                    row1.ItemArray = row2.ItemArray;
                    row2.ItemArray = row;

                    grvColorList.FocusedRowHandle = index - 1;
                    break;

                case "MoveDown":
                    grvColorList.GridControl.Focus();
                    index = grvColorList.FocusedRowHandle;

                    if (index >= grvColorList.DataRowCount - 1)
                        return;

                    row1 = grvColorList.GetDataRow(index);
                    row2 = grvColorList.GetDataRow(index + 1);

                    row = row2.ItemArray;
                    row2.ItemArray = row1.ItemArray;
                    row1.ItemArray = row;

                    grvColorList.FocusedRowHandle = index + 1;
                    break;
            }
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
                case "bteSearchCustom":
                    fdResult = FindData.ShowFindData(FindData.FIND_CUSTOM, bte.EditValue.CString(), null, null, null, false, bte);
                    break;

                // 입력 컨트롤
                case "bteCustom":
                    fdResult = FindData.ShowFindData(FindData.FIND_CUSTOM, bte.EditValue.CString(), null, null, null, false, bte);
                    break;

                case "bteWorkName":
                    fdResult = FindData.ShowFindData(FindData.FIND_WORKNAME, bte.EditValue.CString(), null, null, null, false, bte);
                    break;

                case "bteStuffWidth":
                    fdResult = FindData.ShowFindData(FindData.FIND_WIDTH, bte.EditValue.CString(), null, null, null, false, bte);
                    break;

                case "bteWorkWidth":
                    fdResult = FindData.ShowFindData(FindData.FIND_WIDTH, bte.EditValue.CString(), null, null, null, true, bte);

                    if (fdResult == FindDataResult.FIND_NONE_ADDNEW)
                    {
                        // AddNew Save Code

                        bte.Tag = "NewCode"; // New Code
                        fdResult = FindDataResult.FIND_OK;
                    }
                    break;

                // 그리드 컨트롤 - Grid에 속한 ButtonEdit일 경우 [GridView명칭_Column명칭]으로 조회한다.
                case "grvColorList_Article":
                    fdResult = FindData.ShowFindData(FindData.FIND_ARTICLE, bte.EditValue.CString(), null, null, null, false, bte);
                    
                    if (fdResult == FindDataResult.FIND_OK)
                        grvColorList.SetFocusedRowCellValue("ArticleID", bte.Tag.CString());
                    else if (fdResult == FindDataResult.FIND_NONE)
                        grvColorList.SetFocusedRowCellValue("ArticleID", "");
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

        #region 사용자 정의 함수
        /// <summary>
        /// Grid 초기화
        /// </summary>
        private void InitGrid()
        {
            try
            {
                // Wait폼 출력 및 초기화버튼을 비활성
                Utils.ShowWaitForm(this);

                // 초기화에 필요한 데이터를 조회
                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Initial, SqlDbType.Char);

                // 프로시저 실행
                var initSet = SqlHelper.GetDataSet("xp_TestOrder", inParams, out string retCode, out string retMsg);

                // 실행 결과 에러코드가 정상이 아니면 오류 출력
                if (retCode != "00")
                    throw new Exception(retMsg);

                // 각 그리드 초기화
                grcOrderList.SetColumns(initSet.Tables[0]);
                //grcColorList.SetColumns(initSet.Tables[1], "OrderID,OrderSeq,SortSeq,ArticleID", true, true, true);
                grcColorList.SetColumns(initSet.Tables[1], "OrderID,OrderSeq,SortSeq", true, true, true);

                // Repository Item Setting
                var riArticle = grvColorList.SetRepositoryItemButtonEdit("Article");
                var riDesignNo = grvColorList.SetRepositoryItemTextEdit("DesignNo");
                var riColor = grvColorList.SetRepositoryItemTextEdit("Color");
                var riColorRoll = grvColorList.SetRepositoryItemSpinEdit("ColorRoll");
                var riColorQty = grvColorList.SetRepositoryItemSpinEdit("ColorQty");
                var riUnitPrice = grvColorList.SetRepositoryItemSpinEdit("UnitPrice");
                var riRemark = grvColorList.SetRepositoryItemTextEdit("Remark");


                // 화면 초기화 후 그리드 초기 포맷 저장
                DxControl.SaveGridFormat(grvOrderList, GridFormat.Default);
                DxControl.SaveGridFormat(grvColorList, GridFormat.Default);

                // 사용자 정의 그리드 포맷 로딩
                DxControl.LoadGridFormat(grvOrderList, GridFormat.Current);
                DxControl.SaveGridFormat(grvColorList, GridFormat.Current);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
            finally
            {
                // Wait폼을 종료
                Utils.CloseWaitForm();
            }
        }

        /// <summary>
        /// 각 컨트롤 기본 정보 세팅
        /// </summary>
        private void InitControl()
        {
            //speChunkRate.SetMask("ChunkRate");
            //speLossRate.SetMask("LossRate");

            gleOrderForm.AddItem("01:내수|02:수출");
            gleOrderClss.AddItem("01:임가공|02:판매|03:재고정리");
            gleUnitClss.AddItem("01:YDS|02:MTS");
            glePriceClss.AddItem("01:KRW|02:USD");

            gleRollClss.AddItem("02:Lot별 절번호|01:Color별 절번호");
            gleRollNoClss.AddItem("1:출력함|0:출력안함");
            gleExamDateClss.AddItem("1:출력함|0:출력안함");
            gleLabel.AddItem("01:White|02:Buyer");
            gleBand.AddItem("01:White|02:Buyer");
        }

        /// <summary>
        /// 입력 컨트롤 초기화
        /// </summary>
        private void ClearData()
        {
            InitControl();
            
            txtOrderID.EditValue = "";
            txtOrderNo.EditValue = "";
            txtTagOrder.EditValue = "";
            txtTagRemark.EditValue = "";

            speChunkRate.EditValue = "";
            speLossRate.EditValue = "";

            mmeRemark.EditValue = "";
            mmeMemo.EditValue = "";

            bteCustom.EditValue = "";
            bteCustom.Tag = "";
            bteWorkName.EditValue = "";
            bteWorkName.Tag = "";
            bteStuffWidth.EditValue = "";
            bteStuffWidth.Tag = "";
            bteWorkWidth.EditValue = "";
            bteWorkWidth.Tag = "";

            dteAcptDate.Tag = "D";
            dteAcptDate.SetDefaultDate();
            dteDvlyDate.EditValue = null;

            gleOrderForm.EditValue = "01";
            gleOrderClss.EditValue = "01";
            gleUnitClss.EditValue = "01";
            glePriceClss.EditValue = "01";

            gleRollClss.EditValue = "02";
            gleRollNoClss.EditValue = "1";
            gleExamDateClss.EditValue = "1";
            gleLabel.EditValue = "01";
            gleBand.EditValue = "01";

            grvColorList.DeleteAllRows();
        }

        /// <summary>
        /// grcOrderList 조회
        /// </summary>
        private void FillGridOrder()
        {
            try
            {
                // Wait 폼을 호출
                Utils.ShowWaitForm(this);

                // 조회조건 파라메터를 생성
                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetList, SqlDbType.Char);
                inParams.Add("@i_SearchDateS", dteSearchDateS.EditValue.CDateString("yyyyMMdd"), SqlDbType.Char);
                inParams.Add("@i_SearchDateE", dteSearchDateE.EditValue.CDateString("yyyyMMdd"), SqlDbType.Char);
                inParams.Add("@i_SearchOrderNo", txtSearchOrderNo.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_SearchCustomID", bteSearchCustom.Tag.CString(), SqlDbType.Char);

                var table = SqlHelper.GetDataTable("xp_TestOrder", inParams, out _, out _);
                grcOrderList.SetData(table);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
            finally
            {
                // Wait 폼을 종료
                Utils.CloseWaitForm();
            }
        }

        /// <summary>
        /// 입력 정보 조회
        /// </summary>
        private void ShowData()
        {
            try
            {
                // Wait 폼을 호출
                Utils.ShowWaitForm(this);

                // 조회조건 파라메터를 생성
                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", "81", SqlDbType.Char);
                inParams.Add("@i_SearchOrderID", grvOrderList.GetFocusedRowCellValue("OrderID").CString(), SqlDbType.Char);

                var dataSet = SqlHelper.GetDataSet("xp_TestOrder", inParams, out _, out _);

                var row = dataSet.Tables[0].Rows[0];

                if (row != null)
                {
                    txtOrderID.EditValue = row["OrderID"];
                    txtOrderNo.EditValue = row["OrderNo"];
                    txtTagOrder.EditValue = row["TagOrderNo"];
                    txtTagRemark.EditValue = row["TagRemark"];

                    speChunkRate.EditValue = row["ChunkRate"];
                    speLossRate.EditValue = row["LossRate"];

                    mmeRemark.EditValue = row["Remark"];
                    mmeMemo.EditValue = row["Memo"];

                    bteCustom.EditValue = row["KCustom"];
                    bteCustom.Tag = row["CustomID"];
                    bteWorkName.EditValue = row["WorkName"];
                    bteWorkName.Tag = row["WorkID"];
                    bteStuffWidth.EditValue = row["StuffWidth"];
                    bteStuffWidth.Tag = row["StuffWidthID"];
                    bteWorkWidth.EditValue = row["WorkWidth"];
                    bteWorkWidth.Tag = row["WorkWidthID"];

                    dteAcptDate.EditValue = row["AcptDate"].CDateString();
                    dteDvlyDate.EditValue = row["DvlyDate"].CDateString();

                    gleOrderForm.EditValue = row["OrderForm"];
                    gleOrderClss.EditValue = row["OrderClss"];
                    gleUnitClss.EditValue = row["UnitClss"];
                    glePriceClss.EditValue = row["PriceClss"];

                    gleRollClss.EditValue = row["RollFlag"];
                    gleRollNoClss.EditValue = row["RollNoFlag"];
                    gleExamDateClss.EditValue = row["ExamDateFlag"];
                    gleLabel.EditValue = row["LabelID"];
                    gleBand.EditValue = row["BandID"];
                }


                grcColorList.SetData(dataSet.Tables[1]);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
            finally
            {
                // Wait 폼을 종료
                Utils.CloseWaitForm();
            }
        }

        /// <summary>
        /// 입력 정보 저장
        /// </summary>
        private void SaveData()
        {
            try
            {
                var errorArticle = (grcColorList.DataSource as DataTable).DefaultView.ToTable().Select("ArticleID IS NULL or ArticleID = ''");

                // 필수 입력 필드값 체크
                var field = new Dictionary<string, bool>()
                {
                    { "OrderNo", txtOrderNo.EditValue.CString() == "" },
                    { "CustomID", bteCustom.Tag.CString() == "" },
                    { "ColorList", grvColorList.RowCount == 0 },
                    { "ArticleID", errorArticle != null && errorArticle.Length > 0 }
                };
                if (Utils.CheckField(field))
                    return;

                // 버튼 클릭 시작 시각
                var startTime = DateTime.Now;

                // 기본 처리 processId를 입력으로 설정(Stored Procedure에서 Merge로 작성한 경우는 불필요)
                var processId = ProcessType.Insert;

                // 기본 처리 LogType을 입력으로 설정
                var logType = ActionLogType.Insert;

                // 완료 메세지 기본값을 입력으로 설정
                var message = MessageText.OKInsert;

                // 키값에 해당하는 데이터가 존재하는지 체크
                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Exist, SqlDbType.Char);
                inParams.Add("@i_OrderID", txtOrderID.EditValue.CString());

                if (SqlHelper.Exist("xp_TestOrder", inParams, out _, out _))
                {
                    // 키값에 해당하는 데이터가 존재하면 수정 확인 메세지를 출력
                    if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionUpdate))
                        return;

                    // 업데이트일 경우 처리 processId, logType, message를 수정으로 변경
                    processId = ProcessType.Update;
                    logType = ActionLogType.Update;
                    message = MessageText.OKUpdate;
                }

                // Wait폼 출력 및 저장버튼을 비활성
                Utils.ShowWaitForm(this);
                MainButton.SaveButton = false;

                // XML 설정
                var xmlOrderSub = (grcColorList.DataSource as DataTable).DefaultView
                                    .ToTable(false, new string[] { "OrderSeq", "SortSeq", "ArticleID", "DesignNO", "Color", "ColorRoll", "ColorQty", "UnitPrice", "Remark" })
                                    .ToXml();

                // 데이터 저장용 파라메터를 생성
                inParams = new DbParamList();
                inParams.Add("@i_ProcessID", processId, SqlDbType.Char);
                inParams.Add("@i_OrderID", txtOrderID.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_OrderNo", txtOrderNo.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_CustomID", bteCustom.Tag.CString(), SqlDbType.Char);
                inParams.Add("@i_WorkID", bteWorkName.Tag.CString(), SqlDbType.Char);
                inParams.Add("@i_StuffWidthID", bteStuffWidth.Tag.CString(), SqlDbType.Char);
                inParams.Add("@i_WorkWidthID", bteWorkWidth.Tag.CString(), SqlDbType.Char);
                inParams.Add("@i_ChunkRate", speChunkRate.EditValue.CString(), SqlDbType.Float);
                inParams.Add("@i_LossRate", speLossRate.EditValue.CString(), SqlDbType.Float);
                inParams.Add("@i_OrderForm", gleOrderForm.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_OrderClss", gleOrderClss.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_AcptDate", dteAcptDate.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_DvlyDate", dteDvlyDate.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_UnitClss", gleUnitClss.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_PriceClss", glePriceClss.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_TagOrderNo", txtTagOrder.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_TagRemark", txtTagRemark.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_RollFlag", gleRollClss.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_RollNoFlag", gleRollNoClss.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_ExamDateFlag", gleExamDateClss.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_LabelID", gleLabel.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_BandID", gleBand.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_Remark", mmeRemark.EditValue.CString(), SqlDbType.VarChar);
                inParams.Add("@i_Memo", mmeMemo.EditValue.CString(), SqlDbType.VarChar);
                inParams.Add("@i_OrderRoll", "0", SqlDbType.Int);
                inParams.Add("@i_OrderQty", "0", SqlDbType.Int);
                inParams.Add("@i_ColorCnt", "0", SqlDbType.Int);
                inParams.Add("@i_InptUser", AppConfig.Login.PersonID, SqlDbType.Char);
                inParams.Add("@i_UpdtUser", AppConfig.Login.PersonID, SqlDbType.Char);
                inParams.Add("@i_Xml_OrderSub", xmlOrderSub, SqlDbType.VarChar);


                // 데이터를 저장 (processId에 따라 저장 또는 수정)
                SqlHelper.Execute("xp_TestOrder", inParams, out string retCode, out string retMsg);

                // 반환코드 검증 : 00 - 정상, 그외 - 오류
                if (retCode != "00")
                    throw new Exception(retMsg);

                // 행을 찾기 위한 키값을 저장
                var orderID = txtOrderID.EditValue.CString();

                // 재검색
                ViewButtonClick();

                // 저장한 행 찾기
                grvOrderList.FindRowByValue(new Dictionary<string, object>()
                {
                    { "OrderID", orderID },
                });

                // Insert 또는 Update 로그를 기록
                SqlHelper.WriteLog(this, logType, startTime);

                // 성공(입력 또는 수정) 메세지 출력
                MsgBox.OKMessage(message);
            }
            catch (Exception ex)
            {
                // 에러발생시 에러메세지를 출력
                MsgBox.ErrorMessage(ex.CString());
            }
            finally
            {
                // 저장버튼을 활성화 및 Wait폼을 종료
                MainButton.SaveButton = true;
                Utils.CloseWaitForm();
            }
        }


        #endregion
    }
}