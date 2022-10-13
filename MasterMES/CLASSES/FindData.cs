using DevExpress.XtraEditors;
using DSLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MasterMES.CLASSES
{
    #region 열거형 선언
    /// <summary>
    /// FindData Result
    /// </summary>
    public enum FindDataResult
    {
        /// <summary> 찾기 성공 </summary>
        FIND_OK,
        /// <summary> 찾는 내용이 없음 </summary>
        FIND_NONE,
        /// <summary> 찾는 내용이 없고 새로 등록 </summary>
        FIND_NONE_ADDNEW,
        /// <summary> 조회 조건 오류 </summary>
        FIND_ERROR,
        /// <summary> 찾기 취소 </summary>
        FIND_CANCEL,
    }

    #endregion

    public static class FindData
    {
        // FindKey (저장프로시져와 값을 동일하게 세팅해야 됨)
        public const string FIND_ORDER = "01";
        public const string FIND_CUSTOM = "02";
        public const string FIND_ARTICLE = "03";
        public const string FIND_WORKNAME = "04";
        public const string FIND_WIDTH = "05";
        public const string FIND_MENU = "06";
        public const string FIND_ORDERID = "07";
        public const string FIND_CUSTOMCHARGE = "08";
        public const string FIND_CUSTOMTYPE = "09";

        public const string FIND_ORDERSUB = "90";


        private static string findKey;      // 찾고자 하는 Key 값
        private static object[] controls;   // 검색한 내용을 적용할 컨트롤 배열

        /// <summary>
        /// frmFiindValue 설정
        /// </summary>
        /// <param name="fk">Find Key : 찾고자 하는 Key 값</param>
        /// <param name="fs">Find String : 해당 string을 포함한 내용을 검색</param>
        /// <param name="ws1">조건문1</param>
        /// <param name="ws2">조건문2</param>
        /// <param name="ws3">조건문3</param>
        /// <param name="con">적용할 컨트롤 배열</param>
        public static FindDataResult ShowFindData(string fk, string fs, string ws1, string ws2, string ws3, bool isNewData, params object[] con)
        {
            FindDataResult fdResult;

            if (fk == "")
            {
                MsgBox.ErrorMessage("조회 조건이 명확하지 않습니다.");
                return FindDataResult.FIND_ERROR;
            }

            findKey = fk;
            controls = new object[con.Length];
            controls = con;

            var inParams = new DbParamList();
            inParams.Add("@i_ProcessID", fk, SqlDbType.Char);
            inParams.Add("@i_FindString", fs, SqlDbType.NVarChar);
            inParams.Add("@i_WhereString1", ws1, SqlDbType.NVarChar);
            inParams.Add("@i_WhereString2", ws2, SqlDbType.NVarChar);
            inParams.Add("@i_WhereString3", ws3, SqlDbType.NVarChar);

            var table = SqlHelper.GetDataTable("xp_FindData", inParams, out _, out _);
            frmFindValue frmFind;
            frmFind = new frmFindValue("Find Data", table);
            frmFind.FindValueEvent += GetData;

            if (frmFind.rowCount == 0)
                fdResult = FindDataResult.FIND_NONE;
            else
            {
                if (frmFind.DialogResult == DialogResult.OK)
                    fdResult = FindDataResult.FIND_OK;
                else
                    fdResult = FindDataResult.FIND_CANCEL;
            }

            if (fdResult == FindDataResult.FIND_NONE)
            {
                if (isNewData == false || fs == "")
                {
                    MsgBox.ErrorMessage("검색 결과가 없습니다.");
                    (controls[0] as ButtonEdit).EditValue = "";
                    (controls[0] as ButtonEdit).Tag = "";
                }
                else
                {
                    if (MsgBox.QuestionMessage("검색 결과가 없습니다. 새로 생성하시겠습니까?") == DialogResult.Yes)
                    {
                        fdResult = FindDataResult.FIND_NONE_ADDNEW;
                    }
                    else
                    {
                        (controls[0] as ButtonEdit).EditValue = "";
                        (controls[0] as ButtonEdit).Tag = "";
                    }
                }
            }
            else
            {
                if (frmFind.rowCount == 1)
                    frmFind.SelectData();
                else
                    frmFind.ShowDialog();
            }

            return fdResult;
        }

        /// <summary>
        /// 검색한 내용을 각 컨트롤에 적용
        /// </summary>
        /// <param name="row">검색 DataRow</param>
        public static void GetData(DataRow row)
        {
            if (row == null)
                return;

            switch (findKey)
            {
                case FIND_ORDER:
                    (controls[0] as ButtonEdit).EditValue = row["OrderNo"];
                    (controls[0] as ButtonEdit).Tag = row["OrderID"];
                    break;

                case FIND_CUSTOM:
                    (controls[0] as ButtonEdit).EditValue = row["KCustom"];
                    (controls[0] as ButtonEdit).Tag = row["CustomID"];
                    if (controls.Length > 1) (controls[1] as ButtonEdit).EditValue = row["CustomNo"];
                    if (controls.Length > 2) (controls[2] as ButtonEdit).EditValue = row["Chief"];
                    break;

                case FIND_ARTICLE:
                    (controls[0] as ButtonEdit).EditValue = row["Article"];
                    (controls[0] as ButtonEdit).Tag = row["ArticleID"];
                    break;

                case FIND_WORKNAME:
                    (controls[0] as ButtonEdit).EditValue = row["WorkName"];
                    (controls[0] as ButtonEdit).Tag = row["WorkID"];
                    break;

                case FIND_WIDTH:
                    (controls[0] as ButtonEdit).EditValue = row["Width"];
                    (controls[0] as ButtonEdit).Tag = row["WidthID"];
                    break;

                case FIND_ORDERSUB:
                    (controls[0] as ButtonEdit).EditValue = row["Article"];
                    (controls[0] as ButtonEdit).Tag = row["ArticleID"];
                    if (controls.Length > 1) (controls[1] as ButtonEdit).EditValue = row["DesignNo"];
                    if (controls.Length > 2) (controls[2] as ButtonEdit).EditValue = row["Color"];
                    break;

                case FIND_MENU:
                    (controls[0] as ButtonEdit).EditValue = row["MenuTitle"];
                    (controls[0] as ButtonEdit).Tag = row["MenuID"];
                    break;

                case FIND_ORDERID:
                    (controls[0] as ButtonEdit).EditValue = row["OrderID"];
                    (controls[0] as ButtonEdit).Tag = row["OrderID"];
                    break;

                case FIND_CUSTOMCHARGE:
                    (controls[0] as ButtonEdit).EditValue = row["ChargeName"];
                    (controls[0] as ButtonEdit).Tag = row["ChargeSeq"];
                    if (controls.Length > 1) (controls[1] as ButtonEdit).EditValue = row["DutyName"];
                    if (controls.Length > 2) (controls[2] as ButtonEdit).EditValue = row["CellPhoneNo"];
                    break;

                case FIND_CUSTOMTYPE:
                    (controls[0] as ButtonEdit).EditValue = row["KCustom"];
                    (controls[0] as ButtonEdit).Tag = row["CustomID"];
                    break;
            }
        }
    }
}
