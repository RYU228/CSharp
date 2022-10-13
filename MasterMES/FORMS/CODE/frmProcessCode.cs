using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Behaviors;
using DevExpress.Utils.DragDrop;
using DevExpress.XtraGrid.Views.Grid;
using DSLibrary;

namespace MasterMES.FORMS.CODE
{
    public partial class frmProcessCode : frmBase, CLASSES.IButtonAction
    {
        private BehaviorManager behaviorManager;

        public frmProcessCode()
        {
            InitializeComponent();

            behaviorManager = new BehaviorManager();

            // BehaviorManager 초기화 : 좌 그리드 → 우 그리드로 데이터 이동
            behaviorManager.Attach<DragDropBehavior>(grvCopy, behavior =>
            {
                behavior.Properties.AllowDrop = true;
                behavior.Properties.InsertIndicatorVisible = true;
                behavior.Properties.PreviewVisible = true;
                behavior.DragDrop += Behavior_DragDropLeftToRight;
            });

            // BehaviorManager 초기화 : 우 그리드 → 좌 그리드로 데이터 이동
            behaviorManager.Attach<DragDropBehavior>(grvGroup, behavior =>
            {
                behavior.Properties.AllowDrop = true;
                behavior.Properties.InsertIndicatorVisible = true;
                behavior.Properties.PreviewVisible = true;
                behavior.DragDrop += Behavior_DragDropRightToLeft;
            });

            grvGroup.DoubleClick += GrvGroup_DoubleClick;
            grvCopy.DoubleClick += GrvCopy_DoubleClick;
        }

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

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Initial, SqlDbType.Char);

                var table = SqlHelper.GetDataTable("xp_WorkingGroup", inParams, out _, out string retMsg);

                grcGroup.SetColumns(table);
                grcCopy.SetColumns(table);


                this.ViewGroupGrid();

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
        /// 좌 그리드 --> 우 그리드 (우 그리드에 동일한 행이 없을때만 추가함, 여기는 추가 행추가 명령이 포함됨. 좌->우, 우->좌 방법이 왜 틀린지 모르겠음.????????)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Behavior_DragDropLeftToRight(object sender, DragDropEventArgs e)
        {
            try
            {
                if (e.Target == e.Source)
                    return;

                var index = (e.Data as int[]);
                var sourceRow = grvGroup.GetDataRow(index[0]);
                var targetTable = grcCopy.DataSource as DataTable;

                if (sourceRow != null && targetTable != null)
                {
                    // 우 그리드에 동일한 데이터(GruopCode)가 있는지 확인
                    var rows = targetTable.Select($"GroupCode = '{sourceRow["GroupCode"]}'");

                    if (rows.Length <= 0)
                        targetTable.Rows.Add(sourceRow.ItemArray);

                    e.Handled = true;
                }

                // 우 그리드 정렬
                targetTable.DefaultView.Sort = "GroupCode ASC";

                // SetData 메소드를 호출하여 데이터를 표시하는게 아니므로 RowIndicator의 폭을 별도 설정한다.
                grvCopy.SetRowIndicatorWidth();
                grvCopy.BestFitColumns();
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 우 그리드 --> 좌 그리드 (좌 그리드에 행 추가 작업 없음?, 동일한 행이 존재하면 삭제하면 자동 추가됨 ?)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Behavior_DragDropRightToLeft(object sender, DragDropEventArgs e)
        {
            try
            {
                if (e.Target == e.Source)
                    return;

                var index = (e.Data as int[]);
                var sourceRow = grvCopy.GetDataRow(index[0]);

                if (sourceRow != null)
                {
                    if (sourceRow != null)
                    {
                        (grcCopy.DataSource as DataTable).Rows.Remove(sourceRow);
                    e.Handled = true;
                    }
                }

                grvGroup.SetRowIndicatorWidth();
                grvGroup.BestFitColumns();
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 좌 그리드 더블 클릭시 우 그리드로 해당 행 이동
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GrvGroup_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                Point point = grcGroup.PointToClient(Control.MousePosition);
                DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hitInfo = grvGroup.CalcHitInfo(point);

                if (hitInfo.RowHandle < 0)
                    return;

                var sourceRow = grvGroup.GetDataRow(hitInfo.RowHandle);
                var targetTable = grcCopy.DataSource as DataTable;

                if (sourceRow != null && targetTable != null)
                {
                    var rows = targetTable.Select($"GroupCode = '{sourceRow["GroupCode"]}'");

                    if (rows.Length <= 0)
                        targetTable.Rows.Add(sourceRow.ItemArray);
                }

                // 우 그리드 정렬
                targetTable.DefaultView.Sort = "GroupCode ASC";

                // SetData 메소드를 호출하여 데이터를 표시하는게 아니므로 RowIndicator의 폭을 별도 설정한다.
                grvCopy.SetRowIndicatorWidth();

                grvCopy.BestFitColumns();
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 우 그리드 더블 클릭시 우 그리드 행 삭제 => 좌 그리드는 우 그리드로 행이동 후 삭제하지 않으므로 별도 작업 필요 없음
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GrvCopy_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                //DXMouseEventArgs ea = e as DXMouseEventArgs;
                //GridView view = sender as GridView;
                //GridHitInfo hitInfo = view.CalcHitInfo(ea.Location);

                // 위 3Line을 1Line으로 통합
                var hitInfo = (sender as GridView).CalcHitInfo((e as DXMouseEventArgs).Location);

                if (hitInfo.RowHandle < 0)
                    return;

                grvCopy.DeleteRow(hitInfo.RowHandle);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 찾기 폼을 호출 합니다. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bteGroupName_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (e.Button.Tag.CString() == "FIND")
            {
                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", "81", SqlDbType.Char);

                var table = SqlHelper.GetDataTable("xp_WorkingGroup", inParams, out _, out _);

                var frmGroup = new frmFindValue("Find WorkingGroup", table, "UseClss");
                frmGroup.FindValueEvent += FrmGroup_FindValueEvent;
                frmGroup.ShowDialog();
            }
            else
            {
                bteGroupName.EditValue = null;
                txtGruopCode.EditValue = null;
            }
        }

        /// <summary>
        /// 찾기 폼에서 선택된 행을 화면표시합니다.
        /// </summary>
        /// <param name="row">선택된 행(DataRow Type)</param>
        private void FrmGroup_FindValueEvent(DataRow row)
        {
            if (row == null)
                return;

            bteGroupName.EditValue = row["GroupName"];
            txtGruopCode.EditValue = row["GroupCode"];
        }
        #endregion

        #region 사용자 정의 메소드
        private void ViewGroupGrid()
        {
            try
            {
                Utils.ShowWaitForm(this);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetList, SqlDbType.Char);

                var table = SqlHelper.GetDataTable("xp_WorkingGroup", inParams, out _, out string retMsg);

                grcGroup.SetData(table);
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