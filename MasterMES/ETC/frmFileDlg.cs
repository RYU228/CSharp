using DSLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace MasterMES.FORMS.ETC
{
    public partial class frmFileDlg : frmBase
    {
        #region 전역변수
        private string menuID;      // 파일관리 화면
        private string groupName;   // 파일관리 화면 중 구분을 위한 명칭 (하나의 폼에서 두가지 파일 관리를 할 경우 대비)
        #endregion

        #region 생성자
        public frmFileDlg(string menuID, string groupName)
        {
            InitializeComponent();

            this.menuID = menuID;
            this.groupName = groupName;

            InitFileDialog();
            InitGrid();
        }

        #endregion

        #region 오버라이드 이벤트
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            ShowFileDialog();
        }

        #endregion

        #region 폼 컨트롤 이벤트
        /// <summary>
        /// 파일 열기 버튼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFileOpen_Click(object sender, EventArgs e)
        {
            ShowFileDialog();
        }

        /// <summary>
        /// 확인 버튼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOk_Click(object sender, EventArgs e)
        {
            SaveData();
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// 취소 버튼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// 파일 리스트에서 행 삭제
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RiFileDelete_Click(object sender, EventArgs e)
        {
            grvFileList.DeleteRow(grvFileList.FocusedRowHandle);
        }

        #endregion

        #region 사용자 정의 함수
        /// <summary>
        /// FileDialog 초기화
        /// </summary>
        private void InitFileDialog()
        {
            dlgOpenFile.FileName = "";
            dlgOpenFile.Multiselect = true; // 다중 선택 여부
            dlgOpenFile.CheckFileExists = true; // 존재하지 않는 파일 이름을 지정할 때 대화 상자에 경고가 표시되는지 여부
            dlgOpenFile.CheckPathExists = true; // 존재하지 않는 경로를 지정할 때 대화 상자에 경고가 표시되는지 여부
            dlgOpenFile.Filter = "All files (*.*)|*.*"; // 필터설정
            dlgOpenFile.FilterIndex = 1; // 기본 필터 인덱스
        }

        /// <summary>
        /// Grid 초기화
        /// </summary>
        private void InitGrid()
        {
            try
            {
                // 초기화에 필요한 데이터를 조회
                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Initial, SqlDbType.Char);

                // 프로시저 실행
                var initSet = SqlHelper.GetDataSet("xp_FileStorage", inParams, out string retCode, out string retMsg);

                // 실행 결과 에러코드가 정상이 아니면 오류 출력
                if (retCode != "00")
                    throw new Exception(retMsg);

                // 각 그리드 초기화
                //grcFileList.SetColumns(initSet.Tables[0], "FileType,FilePath");
                grcFileList.SetColumns(initSet.Tables[0], showGroupPanel: false);

                // Repository Item Setting
                var riFileDelete = grvFileList.SetRepositoryItemSimpleButton("FileDelete", RepositoryButtonType.Delete);
                riFileDelete.Click += RiFileDelete_Click;

                // 화면 초기화 후 그리드 초기 포맷 저장
                DxControl.SaveGridFormat(grvFileList, GridFormat.Default);

                // 사용자 정의 그리드 포맷 로딩
                DxControl.LoadGridFormat(grvFileList, GridFormat.Current);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// FileDialog 실행 및 선택된 파일 내용 Grid에 적용
        /// </summary>
        private void ShowFileDialog()
        {
            if (dlgOpenFile.ShowDialog() == DialogResult.OK)
            {
                foreach (String file in dlgOpenFile.FileNames)
                {
                    try
                    {
                        var fileinfo = new FileInfo(file);

                        grvFileList.AddNewRow();
                        grvFileList.BestFitColumns();
                        grvFileList.UpdateCurrentRow();

                        grvFileList.SetFocusedRowCellValue("FileName", fileinfo.Name);                          // 파일명
                        grvFileList.SetFocusedRowCellValue("FileType", fileinfo.Extension.Replace(".", ""));    // 파일확장자명
                        grvFileList.SetFocusedRowCellValue("FileSize", fileinfo.Length);                        // 파일크기
                        grvFileList.SetFocusedRowCellValue("FilePath", fileinfo.FullName);                      // 파일경로
                    }
                    catch (Exception ex)
                    {
                        MsgBox.ErrorMessage(ex.CString());
                    }
                }
            }
        }

        /// <summary>
        /// 파일 저장
        /// </summary>
        private void SaveData()
        {
            try
            {
                var errorFileName = (grcFileList.DataSource as DataTable).DefaultView.ToTable().Select("FileName = ''");
                var errorFilePath = (grcFileList.DataSource as DataTable).DefaultView.ToTable().Select("FilePath = ''");

                // 필수 입력 필드값 체크 (파일명 또는 경로가 공백인게 존재하는지)
                var field = new Dictionary<string, bool>()
                {
                    { "FileName", errorFileName != null && errorFileName.Length > 0 },
                    { "FilePath", errorFilePath != null && errorFilePath.Length > 0 }
                };
                if (Utils.CheckField(field))
                    return;

                // Wait폼 출력
                Utils.ShowWaitForm(this);

                // 선택된 파일 갯수만큼 반복
                for (int i = 0; i < grvFileList.RowCount; i++)
                {
                    // 파일을 바이너리로 변환
                    var fileinfo = new FileInfo(grvFileList.GetRowCellValue(i, "FilePath").ToString());
                    Stream fs = fileinfo.OpenRead();
                    BinaryReader br = new BinaryReader(fs);
                    byte[] bytes = br.ReadBytes((Int32)fs.Length);

                    // 데이터 저장용 파라메터를 생성
                    var inParams = new DbParamList();
                    inParams.Add("@i_ProcessID", ProcessType.Insert, SqlDbType.Char);
                    inParams.Add("@i_MenuID", menuID, SqlDbType.Char);
                    inParams.Add("@i_GroupName", groupName, SqlDbType.Char);
                    inParams.Add("@i_FileType", grvFileList.GetRowCellValue(i, "FileType"), SqlDbType.VarChar);
                    inParams.Add("@i_FileName", grvFileList.GetRowCellValue(i, "FileName"), SqlDbType.VarChar);
                    inParams.Add("@i_FileSize", grvFileList.GetRowCellValue(i, "FileSize"), SqlDbType.Int);
                    inParams.Add("@i_FileData", bytes, SqlDbType.VarBinary);
                    inParams.Add("@i_InptUser", AppConfig.Login.PersonID, SqlDbType.Char);
                    inParams.Add("@i_UpdtUser", AppConfig.Login.PersonID, SqlDbType.Char);

                    // 데이터를 저장 (processId에 따라 저장 또는 수정)
                    SqlHelper.Execute("xp_FileStorage", inParams, out string retCode, out string retMsg);

                    // 반환코드 검증 : 00 - 정상, 그외 - 오류
                    if (retCode != "00")
                        throw new Exception(retMsg);
                }

                // Wait폼을 종료
                Utils.CloseWaitForm();

                // 성공(입력 또는 수정) 메세지 출력
                MsgBox.OKMessage(MessageText.OKInsert);
            }
            catch (Exception ex)
            {
                // 에러발생시 에러메세지를 출력
                MsgBox.ErrorMessage(ex.CString());
            }
        }
        #endregion
    }
}