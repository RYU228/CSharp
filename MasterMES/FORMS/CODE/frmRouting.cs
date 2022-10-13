using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using DSLibrary;

namespace MasterMES.FORMS.CODE
{
    public partial class frmRouting : frmBase, CLASSES.IButtonAction
    {
        #region 생성자
        public frmRouting()
        {
            InitializeComponent();

            lcgRouting.SetHeaderButtons("NEW,SAVE,DELETE");
            lcgEquipment.SetHeaderButtons("NEW,SAVE,DELETE");

            speTubeCount.SetEditMask(MaskType.Numeric);
            speBuyAmount.SetEditMask(MaskType.Numeric);
        }

        #endregion

        #region 오버라이드 이벤트
        //protected override void OnActivated(EventArgs e)
        //{
        //    base.OnActivated(e);
        //    MainButton.ActiveButton("INIT");
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
            try
            {
                Utils.ShowWaitForm(this);
                MainButton.InitButton = false;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Initial, SqlDbType.Char);
                var initSet = SqlHelper.GetDataSet("xp_Routing", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                tlsRouting.SetColumns(initSet.Tables[0], "UseClss");

                this.NewEquipment();
                this.NewRouting();  // 최초 txtRoutingCode에 포커스를 위치시키기 위해 설비->공정 순으로 추기화

                // 폼 로딩과 동시에 데이터를 조회합니다.                
                this.ViewRoutingEquipmentTreeList();
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
        /// 공정 정보 관리
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lcgRouting_CustomButtonClick(object sender, DevExpress.XtraBars.Docking2010.BaseButtonEventArgs e)
        {
            switch (e.Button.Properties.Tag.CString())
            {
                case "NEW":
                    this.NewEquipment();
                    this.NewRouting();
                    break;

                case "SAVE":
                    this.SaveRouting();
                    break;

                case "DELETE":
                    this.DeleteRouting();
                    break;
            }
        }

        /// <summary>
        /// 설비정보 관리
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lcgEquipment_CustomButtonClick(object sender, DevExpress.XtraBars.Docking2010.BaseButtonEventArgs e)
        {
            switch (e.Button.Properties.Tag.CString())
            {
                case "NEW":
                    this.NewEquipment();
                    break;

                case "SAVE":
                    this.SaveEquipment();
                    break;

                case "DELETE":
                    this.DeleteEquipment();
                    break;
            }
        }

        /// <summary>
        /// 활성화 Node가 별경될 때
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tlsRoutng_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            if (e.Node.Level == 0)
            {
                this.ViewRoutingRow(e.Node["KeyField"].CString());
            }
            else
            {
                this.ViewEquipmentRow(e.Node["KeyField"].CString().Substring(0, 4),
                                      e.Node["KeyField"].CString().Substring(4, 4));
            }
        }

        /// <summary>
        /// Node가 클릭 될때
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tlsRouting_RowClick(object sender, DevExpress.XtraTreeList.RowClickEventArgs e)
        {
            if (e.Node.Level == 0)
            {
                this.ViewRoutingRow(e.Node["KeyField"].CString());
            }
            else
            {
                this.ViewEquipmentRow(e.Node["KeyField"].CString().Substring(0, 4),
                                      e.Node["KeyField"].CString().Substring(4, 4));
            }
        }

        /// <summary>
        /// TreeList Row 색상변경(Routing Row : Level 0)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tlsRoutng_NodeCellStyle(object sender, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            if (e.Node.Level == 0)
            {
                e.Appearance.ForeColor = Color.DeepSkyBlue;
                e.Appearance.FontStyleDelta = FontStyle.Bold;
            }
        }

        #endregion

        #region 사용자 정의 메서드 - Routing
        /// <summary>
        /// 공정별 설비 조회 : TreeList
        /// </summary>
        private void ViewRoutingEquipmentTreeList()
        {
            try
            {
                var startTime = DateTime.Now;

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetList, SqlDbType.Char);

                var table = SqlHelper.GetDataTable("xp_Routing", inParams, out _, out _);
                tlsRouting.SetData(table, 0);

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
        /// 공정 입력란 초기화
        /// </summary>
        private void NewRouting()
        {
            DxControl.ClearControl(lcgRouting);
            txtRoutingCode.Focus();
        }

        /// <summary>
        /// 공정 저장
        /// </summary>
        private void SaveRouting()
        {
            try
            {
                var field = new Dictionary<string, bool>()
                {
                    { "RoutingCode,4", txtRoutingCode.EditValue.CString().LengthB() != 4 },
                    { "RoutingName", txtRoutingName.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;
                var logType = ActionLogType.Insert;
                var message = MessageText.OKInsert;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Exist, SqlDbType.Char);
                inParams.Add("@i_RoutingCode", txtRoutingCode.EditValue.CString(), SqlDbType.Char);

                if (SqlHelper.Exist("xp_Routing", inParams, out _, out _))
                {
                    if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionUpdate))
                        return;

                    logType = ActionLogType.Update;
                    message = MessageText.OKUpdate;
                }

                Utils.ShowWaitForm(this);
                lcgRouting.CustomHeaderButtons[1].Properties.Enabled = false;

                inParams.Clear();
                inParams.Add("@i_ProcessID", ProcessType.Merge, SqlDbType.Char);
                inParams.Add("@i_RoutingCode", txtRoutingCode.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_RoutingName", txtRoutingName.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_UseClss", chkRUseClss.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_Remark", mmeRRemark.EditValue.CString(), SqlDbType.VarChar);
                inParams.Add("@i_InptUser", AppConfig.Login.PersonID, SqlDbType.VarChar);

                SqlHelper.Execute("xp_Routing", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                var routingCode = txtRoutingCode.EditValue.CString();
                this.NewRouting();
                this.ViewRoutingEquipmentTreeList();
                tlsRouting.FindNodeByValue(routingCode);

                SqlHelper.WriteLog(this, logType, startTime);
                MsgBox.OKMessage(message);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
            finally
            {
                lcgRouting.CustomHeaderButtons[1].Properties.Enabled = true;
                Utils.CloseWaitForm();
            }
        }

        /// <summary>
        /// 공정 삭제(설비 내역도 삭제됨)
        /// </summary>
        private void DeleteRouting()
        {
            try
            {
                var field = new Dictionary<string, bool>()
                {
                    { "RoutingCode", txtRoutingCode.EditValue.CString().LengthB() != 4 },
                };
                if (Utils.CheckField(field))
                    return;

                if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionDelete))
                    return;

                var startTime = DateTime.Now;

                Utils.ShowWaitForm(this);
                lcgRouting.CustomHeaderButtons[2].Properties.Enabled = false;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Delete, SqlDbType.Char);
                inParams.Add("@i_RoutingCode", txtRoutingCode.EditValue.CString(), SqlDbType.Char);

                SqlHelper.Execute("xp_Routing", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                this.ViewRoutingEquipmentTreeList();
                this.NewRouting();

                SqlHelper.WriteLog(this, ActionLogType.Delete, startTime);
                MsgBox.OKMessage(MessageText.OKDelete);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
            finally
            {
                lcgRouting.CustomHeaderButtons[2].Properties.Enabled = true;
                Utils.CloseWaitForm();
            }
        }

        /// <summary>
        /// 공정 조회 : Row
        /// </summary>
        private void ViewRoutingRow(string routingCode)
        {
            try
            {
                DxControl.ClearControl(lcgRouting);
                DxControl.ClearControl(lcgEquipment);

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetRow, SqlDbType.Char);
                inParams.Add("@i_RoutingCode", routingCode, SqlDbType.Char);

                var row = SqlHelper.GetDataRow("xp_Routing", inParams, out _, out _);

                if (row != null)
                {
                    txtRoutingCode.EditValue = row["RoutingCode"];
                    txtRoutingName.EditValue = row["RoutingName"];
                    chkRUseClss.EditValue = row["UseClss"];
                    mmeRRemark.EditValue = row["Remark"];
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

        #region 사용자 정의 메서드 - Equipment
        /// <summary>
        /// 설비 입력란 초기화
        /// </summary>
        private void NewEquipment()
        {
            DxControl.ClearControl(lcgEquipment);
            txtEquipCode.Focus();
        }

        /// <summary>
        /// 설비 저장
        /// </summary>
        private void SaveEquipment()
        {
            try
            {
                var field = new Dictionary<string, bool>()
                {
                    { "RoutingCode", txtRoutingCode.EditValue.CString() == "" },
                    { "EquipCode,4", txtEquipCode.EditValue.CString().LengthB() != 4 },
                    { "EquipName", txtEquipName.EditValue.CString() == "" },
                    { "EquipNo", txtEquipNo.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                var startTime = DateTime.Now;
                var logType = ActionLogType.Insert;
                var message = MessageText.OKInsert;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Exist, SqlDbType.Char);
                inParams.Add("@i_RoutingCode", txtRoutingCode.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_EquipCode", txtEquipCode.EditValue.CString(), SqlDbType.Char);

                if (SqlHelper.Exist("xp_Equipment", inParams, out _, out _))
                {
                    if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionUpdate))
                        return;

                    logType = ActionLogType.Update;
                    message = MessageText.OKUpdate;
                }

                Utils.ShowWaitForm(this);
                lcgEquipment.CustomHeaderButtons[1].Properties.Enabled = false;

                inParams.Clear();
                inParams.Add("@i_ProcessID", ProcessType.Merge, SqlDbType.Char);
                inParams.Add("@i_RoutingCode", txtRoutingCode.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_EquipCode", txtEquipCode.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_EquipName", txtEquipName.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_EquipNo", txtEquipNo.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_EquipMaker", txtEquipMaker.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_EquipKind", txtEquipKind.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_TubeCount", speTubeCount.EditValue.CShort(), SqlDbType.SmallInt);
                inParams.Add("@i_BuyAmount", speBuyAmount.EditValue.CDouble(), SqlDbType.Decimal);
                inParams.Add("@i_BuyYear", txtBuyYear.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_UseClss", chkEUseClss.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_Remark", mmeERemark.EditValue.CString(), SqlDbType.NVarChar);
                inParams.Add("@i_InptUser", AppConfig.Login.PersonID, SqlDbType.VarChar);

                SqlHelper.Execute("xp_Equipment", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                var equipCode = $"{txtRoutingCode.EditValue}{txtEquipCode.EditValue}";
                this.NewRouting();
                this.ViewRoutingEquipmentTreeList();
                tlsRouting.FindNodeByValue(equipCode);

                SqlHelper.WriteLog(this, logType, startTime);
                MsgBox.OKMessage(message);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
            finally
            {
                lcgEquipment.CustomHeaderButtons[1].Properties.Enabled = true;
                Utils.CloseWaitForm();
            }
        }

        /// <summary>
        /// 설비 삭제
        /// </summary>
        private void DeleteEquipment()
        {
            try
            {
                var field = new Dictionary<string, bool>()
                {
                    { "RoutingCode", txtRoutingCode.EditValue.CString() == "" },
                    { "EquipCode", txtEquipCode.EditValue.CString() == "" },
                };
                if (Utils.CheckField(field))
                    return;

                if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionDelete))
                    return;

                var startTime = DateTime.Now;

                Utils.ShowWaitForm(this);
                lcgEquipment.CustomHeaderButtons[2].Properties.Enabled = false;

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Delete, SqlDbType.Char);
                inParams.Add("@i_RoutingCode", txtRoutingCode.EditValue.CString(), SqlDbType.Char);
                inParams.Add("@i_EquipCode", txtEquipCode.EditValue.CString(), SqlDbType.Char);

                SqlHelper.Execute("xp_Equipment", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                this.NewRouting();
                this.ViewRoutingEquipmentTreeList();

                SqlHelper.WriteLog(this, ActionLogType.Delete, startTime);
                MsgBox.OKMessage(MessageText.OKDelete);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
            finally
            {
                lcgEquipment.CustomHeaderButtons[2].Properties.Enabled = true;
                Utils.CloseWaitForm();
            }
        }

        /// <summary>
        /// 설비 조회 : Row
        /// </summary>
        private void ViewEquipmentRow(string routingCode, string equipCode)
        {
            try
            {
                DxControl.ClearControl(lcgEquipment);

                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetRow, SqlDbType.Char);
                inParams.Add("@i_RoutingCode", routingCode, SqlDbType.Char);
                inParams.Add("@i_EquipCode", equipCode, SqlDbType.Char);

                var row = SqlHelper.GetDataRow("xp_Equipment", inParams, out _, out _);

                if (row != null)
                {
                    txtRoutingCode.EditValue = row["RoutingCode"];
                    txtEquipCode.EditValue = row["EquipCode"];
                    txtEquipName.EditValue = row["EquipName"];
                    txtEquipNo.EditValue = row["EquipNo"];
                    txtEquipMaker.EditValue = row["EquipMaker"];
                    txtEquipKind.EditValue = row["EquipKind"];
                    speTubeCount.EditValue = row["TubeCount"];
                    speBuyAmount.EditValue = row["BuyAmount"];
                    txtBuyYear.EditValue = row["BuyYear"];
                    chkEUseClss.EditValue = row["UseClss"];
                    mmeERemark.EditValue = row["Remark"];
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