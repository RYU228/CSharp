using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using DSLibrary;
using MasterMES.CLASSES;

namespace MasterMES.FORMS.CODE
{
    public partial class frmLocation : frmBase, IButtonAction
    {
        #region 생성자
        public frmLocation()
        {
            InitializeComponent();

            txtWarehouseID.SetEditMask(MaskType.RegExpression, @"[0-9A-Z]{1}");
            txtBinID.Properties.MaxLength = 15;
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

                var initSet = SqlHelper.GetDataSet("xp_Warehouse", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                tlsLocation.SetColumns(initSet.Tables[0], autoColumnWidth:true);

                this.ClearWarehouse();
                this.ClearLocation();
                this.ClearBin();

                this.FillTreeLocation();

                tacLocation.SelectedTabPage = tapWarehouse;
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
            if (tacLocation.SelectedTabPage == tapWarehouse)
            {
                this.ClearWarehouse();
                txtWarehouseID.Focus();
            }
            else if (tacLocation.SelectedTabPage == tapLocation)
            {
                this.ClearLocation();
                txtLocationName.Focus();
            }
            else if (tacLocation.SelectedTabPage == tapBin)
            {
                this.ClearBin();
                txtBinID.Focus();
            }
        }

        /// <summary>
        /// 저장 버튼
        /// </summary>
        public void SaveButtonClick()
        {
            if (tacLocation.SelectedTabPage == tapWarehouse)
                this.SaveWarehouse();
            else if (tacLocation.SelectedTabPage == tapLocation)
                this.SaveLocation();
            else if (tacLocation.SelectedTabPage == tapBin)
                this.SaveBin();
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
            if (tacLocation.SelectedTabPage == tapWarehouse)
                this.DeleteWarehouse();
            else if (tacLocation.SelectedTabPage == tapLocation)
                this.DeleteLocation();
            else if (tacLocation.SelectedTabPage == tapBin)
                this.DeleteBin();
        }

        /// <summary>
        /// 조회 버튼
        /// </summary>
        public void ViewButtonClick()
        {
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
        private void tlsLocation_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            this.ShowData(e.Node.Level, e.Node["KeyField"].CString());
        }

        /// <summary>
        /// 노드 클릭시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tlsLocation_RowClick(object sender, DevExpress.XtraTreeList.RowClickEventArgs e)
        {
            this.ShowData(e.Node.Level, e.Node["KeyField"].CString());
        }

        #endregion

        #region 사용자 정의 메서드 - 창고
        /// <summary>
        /// 창고 입력란 초기화
        /// </summary>
        private void ClearWarehouse()
        {
            txtWarehouseID.EditValue = null;
            txtWarehouseName.EditValue = null;
            chkWUseClss.EditValue = "0";
            mmeWRemark.EditValue = null;
        }

        /// <summary>
        /// 창고 저장
        /// </summary>
        private void SaveWarehouse()
        {
            try
            {
                var field = new Dictionary<string, bool>()
                {
                    { "WarehouseID,1", txtWarehouseID.EditValue.CString() == "" },
                    { "WarehouseName", txtWarehouseName.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;
                var logType = ActionLogType.Insert;
                var message = MessageText.OKInsert;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Exist, SqlDbType.Char);
                inParams.Add("@i_WarehouseID", txtWarehouseID.EditValue.CString());

                if (SqlHelper.Exist("xp_Warehouse", inParams, out _, out _))
                {
                    if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionUpdate))
                        return;

                    logType = ActionLogType.Update;
                    message = MessageText.OKUpdate;
                }

                inParams.Clear();
                inParams.Add("@i_ProcessID", ProcessType.Merge, SqlDbType.Char);
                inParams.Add("@i_WarehouseID", txtWarehouseID.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_WarehouseName", txtWarehouseName.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_UseClss", chkWUseClss.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_Remark", mmeWRemark.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_InptUser", AppConfig.Login.PersonID, SqlDbType.VarChar);

                SqlHelper.Execute("xp_Warehouse", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                var findKey = txtWarehouseID.EditValue;
                this.ClearWarehouse();

                this.FillTreeLocation();
                tlsLocation.FindNodeByValue(findKey);

                SqlHelper.WriteLog(this, logType, startTime);
                MsgBox.OKMessage(message);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 창고 삭제
        /// </summary>
        private void DeleteWarehouse()
        {
            try
            {
                var field = new Dictionary<string, bool>()
                {
                    { "WarehouseID", txtWarehouseID.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;

                if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionDelete))
                    return;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Delete, SqlDbType.Char);
                inParams.Add("@i_WarehouseID", txtWarehouseID.EditValue.CString(), SqlDbType.Char);

                SqlHelper.Execute("xp_Warehouse", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                this.ClearWarehouse();
                this.FillTreeLocation();

                SqlHelper.WriteLog(this, ActionLogType.Delete, startTime);
                MsgBox.OKMessage(MessageText.OKDelete);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 창고 조회 (1개 행)
        /// </summary>
        private void ShowDataWarehoues(string warehouseID)
        {
            try
            {
                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetRow, SqlDbType.Char);
                inParams.Add("@i_WarehouseID", warehouseID, SqlDbType.Char);

                var row = SqlHelper.GetDataRow("xp_Warehouse", inParams, out _, out _);

                if (row != null)
                {
                    txtWarehouseID.EditValue = row["WarehouseID"];
                    txtWarehouseName.EditValue = row["WarehouseName"];
                    chkWUseClss.EditValue = row["UseClss"];
                    mmeWRemark.EditValue = row["Remark"];

                    // 하위 데이터 저장을 위해서 상위값 출력
                    txtLWarehouseID.EditValue = $"{row["WarehouseID"]} : {row["WarehouseName"]}";                    
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

        #region 사용자 정의 메서드 - 위치
        /// <summary>
        /// 위치 입력란 초기화
        /// </summary>
        private void ClearLocation()
        {
            txtLocationID.EditValue = null;
            txtLocationName.EditValue = null;
            chkLUseClss.EditValue = "0";
            mmeLRemark.EditValue = null;
        }

        /// <summary>
        /// 위치 저장
        /// </summary>
        private void SaveLocation()
        {
            try
            {
                var field = new Dictionary<string, bool>()
                {
                    { "WarehouseID", txtLWarehouseID.EditValue.CString() == "" },
                    { "LocationName", txtLocationName.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;
                var logType = ActionLogType.Insert;
                var message = MessageText.OKInsert;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Exist, SqlDbType.Char);
                inParams.Add("@i_WarehouseID", txtLWarehouseID.EditValue.CString().Substring(0, 1));
                inParams.Add("@i_LocationID", txtLocationID.EditValue.CString());

                if (SqlHelper.Exist("xp_Location", inParams, out _, out _))
                {
                    if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionUpdate))
                        return;

                    logType = ActionLogType.Update;
                    message = MessageText.OKUpdate;
                }

                inParams.Clear();
                inParams.Add("@i_ProcessID", ProcessType.Merge, SqlDbType.Char);
                inParams.Add("@i_WarehouseID", txtLWarehouseID.EditValue.CString().Substring(0, 1), SqlDbType.Char);
                inParams.Add("@i_LocationID", txtLocationID.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_LocationName", txtLocationName.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_UseClss", chkLUseClss.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_Remark", mmeLRemark.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_InptUser", AppConfig.Login.PersonID, SqlDbType.VarChar);

                SqlHelper.Execute("xp_Location", inParams, out string[] retVal, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                var findKey = txtLWarehouseID.EditValue.CString().Substring(0, 1) + retVal[0];
                this.ClearLocation();

                this.FillTreeLocation();
                tlsLocation.FindNodeByValue(findKey);

                SqlHelper.WriteLog(this, logType, startTime);
                MsgBox.OKMessage(message);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 위치 삭제
        /// </summary>
        private void DeleteLocation()
        {
            try
            {
                var field = new Dictionary<string, bool>()
                {
                    { "WarehouseID", txtLWarehouseID.EditValue.CString() == "" },
                    { "LocationID", txtLocationID.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;

                if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionDelete))
                    return;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Delete, SqlDbType.Char);
                inParams.Add("@i_WarehouseID", txtLWarehouseID.EditValue.CString().Substring(0, 1), SqlDbType.Char);
                inParams.Add("@i_LocationID", txtLocationID.EditValue.CString(), SqlDbType.Char);

                SqlHelper.Execute("xp_Location", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                this.ClearLocation();
                this.FillTreeLocation();

                SqlHelper.WriteLog(this, ActionLogType.Delete, startTime);
                MsgBox.OKMessage(MessageText.OKDelete);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 위치 조회 (1개 행)
        /// </summary>
        private void ShowDataLocation(string warehouseID, string LocationID)
        {
            try
            {
                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetRow, SqlDbType.Char);
                inParams.Add("@i_WarehouseID", warehouseID, SqlDbType.Char);
                inParams.Add("@i_LocationID", LocationID, SqlDbType.Char);

                var row = SqlHelper.GetDataRow("xp_Location", inParams, out _, out _);

                if (row != null)
                {
                    txtLWarehouseID.EditValue = row["WarehouseID"];
                    txtLocationID.EditValue = row["LocationID"];
                    txtLocationName.EditValue = row["LocationName"];
                    chkLUseClss.EditValue = row["UseClss"];
                    mmeLRemark.EditValue = row["Remark"];

                    // 하위 데이터 저장을 위해서 상위값 출력
                    txtBWarehouseID.EditValue = row["WarehouseID"]; // ID + Name
                    txtBLocationID.EditValue = $"{row["LocationID"]} : {row["LocationName"]}";
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

        #region 사용자 정의 메서드 - BIN
        /// <summary>
        /// Bin 입력란 초기화
        /// </summary>
        private void ClearBin()
        {
            txtBinID.EditValue = null;
            txtBinName.EditValue = null;
            chkBUseClss.EditValue = "0";
            mmeBRemark.EditValue = null;
        }

        /// <summary>
        /// Bin정보 저장
        /// </summary>
        private void SaveBin()
        {
            try
            {
                var field = new Dictionary<string, bool>()
                {
                    { "WarehouseID", txtBWarehouseID.EditValue.CString() == "" },
                    { "LocationID", txtBLocationID.EditValue.CString() == "" },
                    { "BinID", txtBinID.EditValue.CString() == "" },
                    { "BinName", txtBinName.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;
                var logType = ActionLogType.Insert;
                var message = MessageText.OKInsert;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Exist, SqlDbType.Char);
                inParams.Add("@i_WarehouseID", txtBWarehouseID.EditValue.CString().Substring(0, 1), SqlDbType.Char);
                inParams.Add("@i_LocationID", txtBLocationID.EditValue.CString().Substring(0, 2), SqlDbType.Char);
                inParams.Add("@i_BinID", txtBinID.EditValue.CString(), SqlDbType.NVarChar);

                if (SqlHelper.Exist("xpBin", inParams, out _, out _))
                {
                    if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionUpdate))
                        return;

                    logType = ActionLogType.Update;
                }

                inParams.Clear();
                inParams.Add("@i_ProcessID", ProcessType.Merge, SqlDbType.Char);
                inParams.Add("@i_WarehouseID", txtBWarehouseID.EditValue.CString().Substring(0, 1), SqlDbType.Char);
                inParams.Add("@i_LocationID", txtBLocationID.EditValue.CString().Substring(0, 2), SqlDbType.Char);
                inParams.Add("@i_BinID", txtBinID.EditValue.CString(), SqlDbType.VarChar);
                inParams.Add("@i_BinName", txtBinName.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_UseClss", chkBUseClss.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_Remark", mmeBRemark.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_InptUser", AppConfig.Login.PersonID, SqlDbType.VarChar);

                SqlHelper.Execute("xp_Bin", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                var findKey = txtBWarehouseID.EditValue.CString().Substring(0, 1) + 
                              txtBLocationID.EditValue.CString().Substring(0, 2) + txtBinID.EditValue.CString();
                this.ClearBin();

                this.FillTreeLocation();
                tlsLocation.FindNodeByValue(findKey);

                SqlHelper.WriteLog(this, logType, startTime);
                MsgBox.OKMessage(message);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// BIN 삭제
        /// </summary>
        private void DeleteBin()
        {
            try
            {
                var field = new Dictionary<string, bool>()
                {
                    { "WarehouseID", txtBWarehouseID.EditValue.CString() == "" },
                    { "LocationID", txtBLocationID.EditValue.CString() == "" },
                    { "BinID", txtBinID.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;

                if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionDelete))
                    return;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Delete, SqlDbType.Char);
                inParams.Add("@i_WarehouseID", txtBWarehouseID.EditValue.CString().Substring(0, 1), SqlDbType.Char);
                inParams.Add("@i_LocationID", txtBLocationID.EditValue.CString().Substring(0, 2), SqlDbType.Char);
                inParams.Add("@i_BinID", txtBinID.EditValue.CString());

                SqlHelper.Execute("xp_Bin", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                this.ClearBin();
                this.FillTreeLocation();

                SqlHelper.WriteLog(this, ActionLogType.Delete, startTime);
                MsgBox.OKMessage(MessageText.OKDelete);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// Bin 조회 (1개 행)
        /// </summary>
        private void ShowDataBin(string warehouseID, string LocationID, string binID)
        {
            try
            {
                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetRow, SqlDbType.Char);
                inParams.Add("@i_WarehouseID", warehouseID, SqlDbType.Char);
                inParams.Add("@i_LocationID", LocationID, SqlDbType.Char);
                inParams.Add("@i_BinID", binID, SqlDbType.VarChar);

                var row = SqlHelper.GetDataRow("xp_Bin", inParams, out _, out _);

                if (row != null)
                {
                    txtBWarehouseID.EditValue = row["WarehouseID"];
                    txtBLocationID.EditValue = row["LocationID"];
                    txtBinID.EditValue = row["BinID"];
                    txtBinName.EditValue = row["BinName"];
                    chkBUseClss.EditValue = row["UseClss"];
                    mmeBRemark.EditValue = row["Remark"];
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

        #region 사용자 정의 메서드 - Fill TreeList, ShowData
        /// <summary>
        /// 트리리스트 조회(Warehouse - Location - Bin)
        /// </summary>
        private void FillTreeLocation()
        {
            try
            {
                var startTime = DateTime.Now;

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetList, SqlDbType.Char);

                var table = SqlHelper.GetDataTable("xp_Warehouse", inParams, out _, out _);

                tlsLocation.SetData(table, 2);

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
        /// 선택된 TreeList의 Level에 따라 데이터를 표시함.
        /// </summary>
        /// <param name="keyValue"></param>
        private void ShowData(int level, string keyValue)
        {
            // 상위 키는 신규 버튼 클릭시와 조회시 초기화를 구분하기 위해서 여기서 값을 초기화 하낟.
            this.ClearWarehouse();

            txtLWarehouseID.EditValue = null;
            this.ClearLocation();

            txtBWarehouseID.EditValue = null;
            txtBLocationID.EditValue = null;
            this.ClearBin();

            switch (level)
            {
                case 0:
                    this.ShowDataWarehoues(keyValue.Substring(0, 1));
                    tacLocation.SelectedTabPage = tapWarehouse;
                    break;

                case 1:
                    this.ShowDataLocation(keyValue.Substring(0, 1), keyValue.Substring(1, 2));
                    tacLocation.SelectedTabPage = tapLocation;
                    break;

                case 2:
                    this.ShowDataBin(keyValue.Substring(0, 1), keyValue.Substring(1, 2), keyValue.Substring(3));
                    tacLocation.SelectedTabPage = tapBin;
                    break;
            }
        }
        #endregion
    }
}