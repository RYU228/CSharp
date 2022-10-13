using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DSLibrary;
using DevExpress.XtraEditors;
using System.Data;
using System.IO;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid;
using System.Drawing;

namespace MasterMES.CLASSES
{
    public class FileUpDown
    {
        /// <summary>
        /// 첨부파일 다운로드
        /// </summary>
        /// <param name="form">폼 이름</param>
        /// <param name="groupName">첨부파일 그룹</param>
        /// <param name="groupSeq">첨부파일 순번</param>
        /// <param name="fileName">파일명</param>
        public void DownloadAttachFile(Form form, string groupName, int groupSeq, string fileName)
        {
            var dlgFolderBrowser = new XtraFolderBrowserDialog();

            try
            {
                //if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionDownload))
                //    return;

                if (DialogResult.OK == dlgFolderBrowser.ShowDialog())
                {
                    var menuID = Utils.GetMenuID(form);
                    var filePath = dlgFolderBrowser.SelectedPath;

                    var inParams = new DbParamList();
                    inParams.Add("@i_ProcessID", ProcessType.GetRow, SqlDbType.Char);
                    inParams.Add("@i_MenuID", menuID, SqlDbType.VarChar);
                    inParams.Add("@i_GroupName", groupName, SqlDbType.NVarChar);
                    inParams.Add("@i_GroupSeq", groupSeq, SqlDbType.Int);

                    var row = SqlHelper.GetDataRow("xp_FileStorage", inParams, out string retCode, out string retMsg);

                    if (retCode != "00")
                        throw new Exception(retMsg);

                    byte[] bytes = (byte[])row["FileData"];
                    File.WriteAllBytes(Path.Combine(filePath, fileName), bytes);

                    MsgBox.OKMessage(MessageText.OKDownload);

                    // 다운로드 위치를 탐색기로 열기
                    System.Diagnostics.Process.Start(filePath);
                }
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 첨부파일 삭제
        /// </summary>
        /// <param name="form">폼 이름</param>
        /// <param name="groupName">첨부파일 그룹</param>
        /// <param name="groupSeq">첨부파일 순번</param>
        public void DeleteAttachFile(Form form, string groupName, int groupSeq)
        {
            try
            {
                if (DialogResult.No == MsgBox.QuestionMessage(MessageText.QuestionDelete))
                    return;

                var menuID = Utils.GetMenuID(form);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Delete, SqlDbType.Char);
                inParams.Add("@i_MenuID", menuID, SqlDbType.VarChar);
                inParams.Add("@i_GroupName", groupName, SqlDbType.NVarChar);
                inParams.Add("@i_GroupSeq", groupSeq, SqlDbType.Int);

                var row = SqlHelper.GetDataRow("xp_FileStorage", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                MsgBox.OKMessage(MessageText.OKDelete);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 첨부파일 리스트를 그리드에 조회
        /// </summary>
        /// <param name="form">폼 이름</param>
        /// <param name="grid">첨부파일 리스트를 표시할 그리드 컨트롤</param>
        /// <param name="groupName">첨부파일 그룹</param>
        public void FillGridAttachFile(Form form, GridControl grid, string groupName)
        {
            try
            {
                var menuID = Utils.GetMenuID(form);

                // 조회조건 파라메터를 생성
                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetList, SqlDbType.Char);
                inParams.Add("@i_MenuID", menuID, SqlDbType.VarChar);
                inParams.Add("@i_GroupName", groupName, SqlDbType.NVarChar);

                var dataSet = SqlHelper.GetDataSet("xp_FileStorage", inParams, out _, out _);

                grid.SetData(dataSet.Tables[0]);

                // 저장된 그리드 형식이 있다면 적용
                DxControl.LoadGridFormat(grid.MainView as GridView, GridFormat.Current);
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 지정된 파일을 다운로드 후 (열기:Option)
        /// </summary>
        /// <param name="form"></param>
        /// <param name="groupName"></param>
        /// <param name="groupSeq"></param>
        /// <param name="fileName"></param>
        /// <param name="isOpen"></param>
        /// <returns></returns>
        public string SaveAttachFile(Form form, string groupName, int groupSeq, string fileName, bool isOpen = false)
        {
            try
            {
                var menuID = Utils.GetMenuID(form);
                var path = Application.StartupPath + "\\Temp";

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                var fullPath = Path.Combine(path, fileName);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetRow, SqlDbType.Char);
                inParams.Add("@i_MenuID", menuID, SqlDbType.VarChar);
                inParams.Add("@i_GroupName", groupName, SqlDbType.NVarChar);
                inParams.Add("@i_GroupSeq", groupSeq, SqlDbType.Int);

                var row = SqlHelper.GetDataRow("xp_FileStorage", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                byte[] bytes = (byte[])row["FileData"];
                File.WriteAllBytes(fullPath, bytes);

                if (isOpen)
                    System.Diagnostics.Process.Start(fullPath);

                return fullPath;

            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
                return string.Empty;
            }
        }

        /// <summary>
        /// 데이터베이싀 이미지 컬럼 데이터를 PictureEdit에 표시합니다.
        /// </summary>
        /// <param name="form"></param>
        /// <param name="groupName"></param>
        /// <param name="groupSeq"></param>
        /// <param name="picBox"></param>
        public void ImageDataToControl(Form form, string groupName, int groupSeq, PictureEdit picBox)
        {
            try
            {
                Utils.ShowWaitForm(form);

                var menuID = Utils.GetMenuID(form);

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.GetRow, SqlDbType.Char);
                inParams.Add("@i_MenuID", menuID, SqlDbType.VarChar);
                inParams.Add("@i_GroupName", groupName, SqlDbType.NVarChar);
                inParams.Add("@i_GroupSeq", groupSeq, SqlDbType.Int);

                var row = SqlHelper.GetDataRow("xp_FileStorage", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                if (row != null && row["FileData"] != null)
                {
                    byte[] bytes = (byte[])row["FileData"];
                    MemoryStream ms = new MemoryStream(bytes);

                    Image image = Image.FromStream(ms);

                    picBox.Image = (Image)image.Clone();
                }
                else
                {
                    picBox.Image = null;
                }
            }
            catch (ArgumentException)
            {
                MsgBox.ErrorMessage("이미지 파일 형식이 아닙니다.");
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
    }
}
