using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;  // 참조 : System.Web.Extensions
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraLayout;
using DevExpress.XtraReports.UI;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using Microsoft.VisualBasic;

/// <summary>
/// 확장메서드 클래스
/// </summary>
namespace DSLibrary
{
    #region 열거형 선언
    /// <summary>
    /// TextEdit의 Mask 형식
    /// </summary>
    public enum MaskType
    {
        /// <summary>년월일(yyyy-MM-dd)</summary>
        Date,
        /// <summary>년월일 시분(yyyy-MM-dd HH:mm)</summary>
        DateMinute,
        /// <summary>년월일 시분초(yyyy-MM-dd HH:mm:ss)</summary>
        DateTime,
        /// <summary>시분 (HH:mm)</summary>
        Time,
        /// <summary>사업자등록번호(NNN-NN-NNNNN)</summary>
        BizNumber,
        /// <summary>주민등록번호(NNNNNN-NNNNNNN)</summary>
        IDCardNumber,
        /// <summary>숫자형식(상세형식은 사용자가 정의)</summary>
        Numeric,
        /// <summary>전화번호, 팩스번호</summary>
        PhoneFax,
        /// <summary>휴대폰번호</summary>
        CellPhone,
        /// <summary>아이피 주소</summary>
        IP,
        /// <summary>사용자 정의 형식</summary>
        RegExpression,
    };

    /// <summary>
    /// DateEdit의 형식
    /// </summary>
    public enum ViewStyle
    {
        /// <summary>月 형태 표시</summary>
        DateView,
        /// <summary>月 형태 표시</summary>
        MonthView,
        /// <summary>年 형태 표시</summary>
        YearView,
    };

    /// <summary>
    /// Repository ButtonEdit의 Button Type
    /// </summary>
    public enum RepositoryButtonType
    {
        /// <summary>없음</summary>
        None,
        /// <summary>검색 버튼</summary>
        Add,
        /// <summary>행 삭제 버튼</summary>
        Delete,
        /// <summary>검색 버튼</summary>
        Find,
        /// <summary>다운로드 버튼</summary>
        Download,
    }
    #endregion

    public static class Extensions
    {
        #region 클래스 변수 선언
        public delegate bool BEEventHandler(object sender);
        public static event BEEventHandler ButtonEditEvent;
        #endregion

        #region 용어집 테이블의 컬럼명 정의
        /// <summary>
        /// 용어집 테이블 컬럼 정의
        /// </summary>
        private const string keyField = "GlossaryCode";
        private const string captionField = "GlossaryName";
        private const string hAlignField = "HAlignCode";
        private const string vAlignField = "VAlignCode";
        private const string typeField = "FormatCode";
        private const string typeStringField = "FormatText";

        #endregion

        #region 형 변환 또는 값 변환 관련 메서드
        /// <summary>
        /// object 형을 string 형으로 변환한다.
        /// </summary>
        /// <param name="value">변환 대상 값</param>
        /// <param name="isTrim">결과값 Trim() 처리(true:Yes, false:No)</param>
        /// <returns></returns>
        public static string CString(this object value, bool isTrim = false)
        {
            if (value == null || value == DBNull.Value)
                return string.Empty;
            else
                return isTrim ? value.ToString().Trim() : value.ToString();
        }

        /// <summary>
        /// object 형을 DateTime Format의 string 형으로 변환한다.
        /// </summary>
        /// <param name="value">변환 대상 값</param>
        /// <param name="dateFormat">반환 형식(ex. yyyy-MM-dd HH:mm:ss)</param>
        /// <returns></returns>
        public static string CDateString(this object value, string dateFormat = "yyyy-MM-dd")
        {
            if (value.CString().Length == 8)
            {
                var temp = value.CString();
                value = $"{temp.Substring(0, 4)}-{temp.Substring(4, 2)}-{temp.Substring(6, 2)}";
            }

            if (value == null || value == DBNull.Value || Information.IsDate(value) == false)
                return string.Empty;

            var dateTime = DateTime.Parse(value.ToString());
            if (dateTime.ToString("yyyyMMdd") == "00010101")
                return string.Empty;
            else
                return dateTime.ToString(dateFormat);
        }

        /// <summary>
        /// object 형을 Int 형으로 변환한다.
        /// </summary>
        /// <param name="value">변환 대상 값</param>
        /// <returns></returns>
        public static int CInt(this object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return 0;
            }
            else
            {
                var s = value.ToString();

                if (s != null)
                {
                    if (s.ToLower() == "true" || s == "예" || s.ToLower() == "yes")
                        return 1;
                    else if (s.ToLower() == "false" || s == "아니오" || s.ToLower() == "no")
                        return 0;

                    // s값이 정수형태가 아니면 FormatException 발생 => Double형으로 변환 후 Int32로 변환
                    return Convert.ToInt32(Convert.ToDouble(s.Replace(",", "")));
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// object 형을 Short(Int16) 형으로 변환한다.
        /// </summary>
        /// <param name="value">변환 대상 값</param>
        /// <returns></returns>
        public static short CShort(this object value)
        {
            if (value == null || value == DBNull.Value || Information.IsNumeric(value) == false)
                return 0;

            return Convert.ToInt16(Convert.ToDouble(value.ToString().Replace(",", "")));
        }

        /// <summary>
        /// object 형을 Long(Int64) 형으로 변환한다.
        /// </summary>
        /// <param name="value">변환 대상 값</param>
        /// <returns></returns>
        public static long CLong(this object value)
        {
            if (value == null || value == DBNull.Value || Information.IsNumeric(value) == false)
                return 0;

            return Convert.ToInt64(Convert.ToDouble(value.ToString().Replace(",", "")));
        }

        /// <summary>
        /// object 형을 Float 형으로 변환한다.
        /// </summary>
        /// <param name="value">변환 대상 값</param>
        /// <returns></returns>
        public static float CFloat(this object value)
        {
            if (value == null || value == DBNull.Value || Information.IsNumeric(value) == false)
                return 0f;

            return Convert.ToSingle(value.ToString().Replace(",", ""));
        }

        /// <summary>
        /// object 형을 Double 형으로 변환한다.
        /// </summary>
        /// <param name="value">변환 대상 값</param>
        /// <returns></returns>
        public static double CDouble(this object value)
        {
            if (value == null || value == DBNull.Value || Information.IsNumeric(value) == false)
                return 0d;

            return Convert.ToDouble(value.ToString().Replace(",", ""));
        }

        /// <summary>
        /// object 형을 Decimal 형으로 변환한다.
        /// </summary>
        /// <param name="value">변환 대상 값</param>
        /// <returns></returns>
        public static decimal CDecimal(this object value)
        {
            if (value == null || value == DBNull.Value || Information.IsNumeric(value) == false)
                return 0M;

            return Convert.ToDecimal(value.ToString().Replace(",", ""));
        }

        /// <summary>
        /// 문자열에서 영숫자만 추출하여 문자열 형태로 반환한다.
        /// </summary>
        /// <param name="value">대상 문자열</param>
        /// <returns></returns>
        public static string CAlphaNumeric(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            else
                // '\W'           : [^a-zA-Z0-9] 영문자(대/소), 숫자 이외 문자를 의미
                // @"[ㄱ-ㅎ가-힣]" : 한글,  @"[ㄱ-ㅎ가-힣]" : 한글 이외 문자를 의미
                return Regex.Replace(Regex.Replace(value, @"\W", ""), @"[ㄱ-ㅎ가-힣]", "");
        }
        #endregion

        #region 데이터 형 체크 관련 메서드
        /// <summary>
        /// 값의 숫자 형식 여부를 반환한다.
        /// </summary>
        /// <param name="value">대상 값</param>
        /// <returns></returns>
        public static bool IsNumeric(this object value)
        {
            return Information.IsNumeric(value);
        }

        /// <summary>
        /// 값의 날짜 형식 여부를 반환한다.
        /// </summary>
        /// <param name="value">대상 값</param>
        /// <returns></returns>
        public static bool IsDate(this object value)
        {
            return Information.IsDate(value);
        }

        /// <summary>
        /// 주민등록번호(Resident registration number)의 유효성을 검사한다.
        /// </summary>
        /// <param name="idNumber">주민등록번호('-' 문자 포함/미포함 가능)</param>
        /// <returns></returns>
        public static bool IsIDNumber(this string idNumber)
        {
            idNumber = idNumber.Replace(" ", "").Replace("-", "");

            if (idNumber.Length != 13)
                return false;

            var sum = 0;
            for (int i = 0; i < idNumber.Length - 1; i++)
            {
                var c = idNumber[i];

                //숫자로 이루어져 있는가?
                if (!char.IsNumber(c))
                {
                    return false;
                }
                else
                {
                    if (i < idNumber.Length)
                        sum += int.Parse(c.ToString()) * ((i % 8) + 2); //지정된 숫자로 각 자리를 나눈 후 더한다.
                }
            }

            // 검증코드와 결과 값이 같은가?
            if (!((((11 - (sum % 11)) % 10).ToString()) == ((idNumber[idNumber.Length - 1]).ToString())))
                return false;

            return true;
        }

        #endregion

        #region 날짜 관련 메서드

        /// <summary>
        /// 월의 1일자를 반환한다.
        /// </summary>
        /// <param name="date">기준일자</param>
        /// <returns></returns>
        public static DateTime FirstOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        /// <summary>
        /// 월의 마지막 일자를 반환한다.
        /// </summary>
        /// <param name="date">기준일자</param>
        /// <returns></returns>
        public static DateTime LastOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1);
        }

        /// <summary>
        /// 기준일자가 속한 주의 월요일을 반환한다.
        /// </summary>
        /// <param name="date">기준일자</param>
        /// <returns></returns>
        public static DateTime MondayOfWeek(this DateTime date)
        {
            var today = (int)date.DayOfWeek;
            return date.AddDays((double)((today == 0) ? -6 : 1 - today));
        }

        /// <summary>
        /// 기준일자가 속한 주의 일요일을 반환한다.
        /// </summary>
        /// <param name="date">기준일자</param>
        /// <returns></returns>
        public static DateTime SundayOfWeek(this DateTime date)
        {
            var today = (int)date.DayOfWeek;
            return date.AddDays((double)((today == 0) ? 0 : 7 - today));
        }

        #endregion

        #region 문자열 관련 메서드
        /// <summary>
        /// 문자열의 오른쪽부터 지정된 길이만큼 문자를 반환한다.
        /// </summary>
        /// <param name="value">대상 문자열</param>
        /// <param name="length">반환 문자열 길이</param>
        /// <returns></returns>
        public static string Right(this string value, int length)
        {
            if (length <= 0)
                return string.Empty;

            if (value.Length < length)
                length = value.Length;

            return value.Substring(value.Length - length); //, length);
        }

        /// <summary>
        /// 문자열 길이(Byte 단위)를 반환한다.
        /// </summary>
        /// <param name="value">대상 문자열</param>
        /// <returns></returns>
        public static int LengthB(this object value)
        {
            return Encoding.Default.GetByteCount(value.CString());
        }

        /// <summary>
        /// 문자열에 한글 포함 여부를 반환한다.
        /// </summary>
        /// <param name="value">대상 문자열</param>
        /// <returns></returns>
        public static bool HasHangul(this string value)
        {
            var str = value.CString();
            var charArr = str.ToCharArray();

            foreach (char c in charArr)
            {
                if (char.GetUnicodeCategory(c) == System.Globalization.UnicodeCategory.OtherLetter)
                    return true;
            }

            return false;
        }

        #endregion

        #region 문자열 지역화
        /// <summary>
        /// 키값에 해당하는 단일 문자열을 지역화한다.
        /// </summary>
        /// <param name="keyValue">문자열 키값</param>
        /// <returns></returns>
        public static string Localization(this object keyValue, string defaultText = "")
        {
            var text = keyValue.CString();

            if (AppConfig.Glossary != null)
            {
                var rows = AppConfig.Glossary.Select($"[{keyField}] = '{keyValue}'");

                if (rows.Length > 0)
                {
                    text = rows[0][captionField].CString().Replace("||", Environment.NewLine);
                }
                else
                {
                    if (defaultText.Equals(string.Empty) == false)
                        text = defaultText;
                }
            }

            return text;
        }

        #endregion

        #region DataTable to XML/JSON String
        /// <summary>
        /// DataTable 데이터를 XML 문자열로 변환한다.
        /// </summary>
        /// <param name="table">Source Data</param>
        /// <param name="dateFiels">날짜형식의 필드 리스트</param>
        /// <param name="topNodeName">최상위 노드명</param>
        /// <param name="tableName">데이터 테이블 명(Xml Row)</param>
        /// <returns></returns>
        public static string ToXml(this DataTable table, string[] dateFiels = null, string topNodeName = "XMLDataSet", string tableName = "XMLDataRow")
        {
            string xml;

            try
            {
                var set = new DataSet(topNodeName); // 최상위 노드의 이름을 설정됨
                set.Tables.Add(table);
                table.TableName = tableName;

                using (System.IO.StringWriter xmlData = new System.IO.StringWriter())
                {
                    table.WriteXml(xmlData, XmlWriteMode.IgnoreSchema, false);
                    xml = xmlData.CString();

                    // 날짜 형식의 경우 -1일이 되는 문제점 발생, 그래서 강제로 형식 변경
                    if (dateFiels != null)
                    {
                        for (int i = 0; i < dateFiels.Length; i++)
                        {
                            xml = Regex.Replace(xml,
                                        @"<" + dateFiels[i] + @">(?<year>\d{4})-(?<month>\d{2})-(?<date>\d{2}).*?</" + dateFiels[i] + ">",
                                        @"<" + dateFiels[i] + @">${year}-${month}-${date}</" + dateFiels[i] + "> ",
                                        RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
                        }
                    }
                }
            }
            catch (Exception)
            {
                xml = string.Empty;
            }

            return xml;
        }

        /// <summary>
        /// DataTable 데이터를 JSON 문자열로 변환한다.
        /// </summary>
        /// <param name="table">데이터 테이블</param>
        /// <returns></returns>
        public static string ToJSON(this DataTable table)
        {
            string json;

            try
            {
                var jsSerializer = new JavaScriptSerializer();
                var parentRow = new List<Dictionary<string, object>>();
                Dictionary<string, object> childRow;

                foreach (DataRow row in table.Rows)
                {
                    childRow = new Dictionary<string, object>();

                    foreach (DataColumn col in table.Columns)
                        childRow.Add(col.ColumnName, row[col]);

                    parentRow.Add(childRow);
                }

                json = jsSerializer.Serialize(parentRow);
            }
            catch
            {
                json = string.Empty;
            }

            return json;
        }

        #endregion

        #region LayoutGroup Control
        /// <summary>
        /// LayoutControlGroup에 Custom Header Button을 추가한다.
        /// </summary>
        /// <param name="layoutGroup">LayoutControlGroup 컨트롤</param>
        /// <param name="buttons">추가할 버튼 키값(콤마로 구분)</param>
        public static void SetHeaderButtons(this LayoutControlGroup layoutGroup, string buttons)
        {
            try
            {
                var buttonItem = buttons.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                if (buttonItem == null || buttonItem.Length <= 0)
                    return;

                layoutGroup.CustomHeaderButtons.Clear();

                for (int i = 0; i < buttonItem.Length; i++)
                {
                    var btn = new DevExpress.XtraEditors.ButtonsPanelControl.GroupBoxButton
                    {
                        Tag = buttonItem[i],
                        Caption = $"    {buttonItem[i].Localization()}    ",
                    };

                    // 파일 업로드일 경우만 이미지 표시
                    if (buttonItem[i] == "FileUpload")
                        btn.ImageOptions.ImageUri = "Upload;Size16x16;Colored";

                    layoutGroup.CustomHeaderButtons.Add(btn);
                }
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.ToString());
            }
        }
        #endregion

        #region Banded GridView
        /// <summary>
        /// Banded Grid의 Band 반복한다.
        /// </summary>
        /// <param name="band">Gridband 개체</param>
        private static void IterateBandColumns(GridBand band)
        {
            if (band.Children.Count > 0) //a parent band  
            {
                foreach (GridBand childBand in band.Children)
                {
                    childBand.Caption = childBand.Caption.Localization();
                    IterateBandColumns(childBand);
                }
            }
            else //the bottommost band with columns  
            {
                foreach (BandedGridColumn column in band.Columns)
                {
                    // 용어 처리
                    var rows = AppConfig.Glossary.Select($"{keyField} = '{column.FieldName}'");
                    if (rows.Length > 0)
                    {
                        column.AppearanceCell.TextOptions.HAlignment = (HorzAlignment)rows[0][hAlignField].CShort();
                        column.AppearanceCell.TextOptions.VAlignment = (VertAlignment)rows[0][vAlignField].CShort();
                        column.DisplayFormat.FormatType = (FormatType)rows[0][typeField].CShort();
                        column.DisplayFormat.FormatString = rows[0][typeStringField].CString();
                    }
                }
            }
        }

        /// <summary>
        /// Banded GridView 초기화(지역화, BestFit)한다.
        /// </summary>
        /// <param name="band">BandedGridView 컨트롤</param>
        /// <param name="hiddenBands">숨김 처리할 밴드명(자식밴드는 숨길수 없음)</param>
        /// <param name="showFilterRow">자동 필터로 설정/해제</param>
        /// <param name="autoColumnWidth">컬럼폭을 컨트롤 폭으로 설정/해제</param>
        public static void SetColumns(this BandedGridView band, string hiddenBands = "", bool showFilterRow = true, bool autoColumnWidth = false, bool rowIndicator = true)
        {
            // 부더러운 처리를 위해 숨겨 놓은 Bands를 다시 보여줌
            band.OptionsView.ShowBands = true;
            band.OptionsView.ShowAutoFilterRow = showFilterRow;
            band.OptionsView.ColumnAutoWidth = autoColumnWidth;
            band.OptionsView.ShowIndicator = rowIndicator;

            // Band의 Caption 지역화 및 컬럼 형식 설정
            for (int i = 0; i < band.Bands.Count; i++)
            {
                // 지정된 밴드 숨김 처리(밴드 지역화 => Caption 변경전에 Caption으로 비교해서 Hidden 처리
                if (!string.IsNullOrEmpty(hiddenBands))
                    if (hiddenBands.Contains(band.Bands[i].Caption))
                        band.Bands[i].Visible = false;

                band.Bands[i].Caption = band.Bands[i].Caption.Localization();
                IterateBandColumns(band.Bands[i]);
            }

            // 사용여부에 따른 취소선 표시
            band.RowStyle += (s, e) =>
            {
                var view = s as BandedGridView;

                if (e.RowHandle >= 0 && view.Columns["UseClss"] != null && view.GetRowCellValue(e.RowHandle, "UseClss").CString() == "1")
                {
                    e.Appearance.FontStyleDelta = FontStyle.Strikeout | FontStyle.Italic;
                    e.Appearance.ForeColor = UserColors.NotUseForeColor;
                }
            };

            // 컬럼 BestFit 설정
            using (var g = (band.GridControl).CreateGraphics())
            {
                foreach (GridBand band1 in band.Bands)
                {
                    band1.MinWidth = (int)g.MeasureString(band1.Caption, band1.AppearanceHeader.Font).Width + 10;

                    if (band1.HasChildren)
                        foreach (DevExpress.XtraGrid.Views.BandedGrid.GridBand chdBand in band1.Children)
                            chdBand.MinWidth = (int)g.MeasureString(chdBand.Caption, chdBand.AppearanceHeader.Font).Width + 10;
                }
            }
        }

        #endregion

        #region GridControl and GridView / Banded GridView
        /// <summary>
        /// GridControl을 초기화 설정한다.
        /// </summary>
        /// <param name="grid">GridControl 개체</param>
        /// <param name="source">컬럼 초기화용 Data Source(빈행에 형식만 존재함)</param>
        /// <param name="showGroupPanel">GroupPanel 표시 여부(true:표시, false:숨김)</param>
        /// <param name="showFilterRow">AutoFilterRow 표시 여부(true:표시, false:숨김)</param>
        /// <param name="autoColumnWidth">컬럼폭을 컨트롤에 맞춰 표시할지 여부(true:컨트롤 폭에 맞춤, false:컨트롤 폭에 맞추지 않음)</param>
        public static void SetColumns(this GridControl grid, DataTable source, string hiddenColumns = "", bool showGroupPanel = true,
                                      bool showFilterRow = true, bool autoColumnWidth = false, bool rowIndicator = true, bool showFooter = false)
        {
            try
            {
                var view = grid.MainView as GridView;

                view.Columns.Clear();
                view.OptionsView.ShowGroupPanel = showGroupPanel;
                view.OptionsView.ShowAutoFilterRow = showFilterRow;
                view.OptionsView.ColumnAutoWidth = autoColumnWidth;
                view.OptionsView.ShowIndicator = rowIndicator;
                view.OptionsView.ShowFooter = showFooter;

                grid.DataSource = source;
                grid.ForceInitialize();

                view.Columns.ToList().ForEach(column =>
                {
                    column.OptionsColumn.AllowEdit = false;
                    column.FilterMode = ColumnFilterMode.DisplayText;
                    column.SortMode = ColumnSortMode.DisplayText;
                    column.AppearanceHeader.FontStyleDelta = FontStyle.Bold;    // 컬럼헤더만 진하게 (Row는 일반)

                    // 용어 처리
                    var rows = AppConfig.Glossary.Select($"{keyField} = '{column.FieldName}'");
                    if (rows.Length > 0)
                    {
                        column.Caption = rows[0][captionField].CString();
                        column.AppearanceCell.TextOptions.HAlignment = (HorzAlignment)rows[0][hAlignField].CShort();
                        column.AppearanceCell.TextOptions.VAlignment = (VertAlignment)rows[0][vAlignField].CShort();
                        column.DisplayFormat.FormatType = (FormatType)rows[0][typeField].CShort();
                        column.DisplayFormat.FormatString = rows[0][typeStringField].CString();
                    }
                });

                // 사용여부에 따른 취소선 표시
                if (view.Columns["UseClss"] != null)
                {
                    view.RowStyle += (s, e) =>
                    {
                      var view1 = s as GridView;

                      if (e.RowHandle >= 0 && view1.Columns["UseClss"] != null && view1.GetRowCellValue(e.RowHandle, "UseClss").CString() == "1")
                      {
                          e.Appearance.FontStyleDelta = FontStyle.Strikeout | FontStyle.Italic;
                          e.Appearance.ForeColor = UserColors.NotUseForeColor;
                      }
                    };
                }

                // 컬럼 숨김 처리
                if (!string.IsNullOrEmpty(hiddenColumns))
                {
                    hiddenColumns.Split(',').ToList().ForEach(colText =>
                    {
                        if (view.Columns[colText.Trim()] != null)
                            view.Columns[colText.Trim()].Visible = false;
                    });
                }

                view.BestFitColumns();
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// GridControl에 데이터를 표시한다.
        /// </summary>
        /// <param name="grid">GridControl 개체</param>
        /// <param name="source">데이터 소스</param>
        /// <param name="fixColumnsIndex">0~fixColumnIndex 열을 좌측에 고정(-1:고정없음)</param>
        public static void SetData(this GridControl grid, DataTable source)
        {
            try
            {
                var view = grid.MainView as GridView;
                view.BeginUpdate();

                grid.DataSource = null;
                grid.DataSource = source;

                view.BestFitColumns();
                view.SetRowIndicatorWidth();

                // 저장된 그리드 형식이 있다면 적용
                DxControl.LoadGridFormat(view, GridFormat.Current);

                view.EndUpdate();
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// SetData 메소드를 호출하지 않는 그리드의 RowIndicator Width를 설정한다.
        /// </summary>
        /// <param name="view">GridView 컨트롤</param>
        public static void SetRowIndicatorWidth(this GridView view)
        {
            // 데이터 출력후 RowIndicator에 행번호를 표시, 데이터 건수에 따라 RowIndicator의 Width를 조정 ==> Row Indicator 폭 고정으로 처리(컨트롤 초기화시)
            Graphics gr = Graphics.FromHwnd(view.GridControl.Handle);
            SizeF size = gr.MeasureString(view.RowCount.ToString(), view.PaintAppearance.Row.GetFont());
            view.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + DevExpress.XtraGrid.Views.Grid.Drawing.GridPainter.Indicator.ImageSize.Width + 15;
        }

        /// <summary>
        /// GridView 좌측에 컬럼을 고정한다.
        /// </summary>
        /// <param name="columnIndex">0~columnIndex까지 컬럼을 좌측에 고정</param>
        public static void FrozenColumns(this GridView view, int columnIndex)
        {
            if (columnIndex > -1)
            {
                view.Columns.Where(x => x.VisibleIndex <= columnIndex).ToList().ForEach(x => x.Fixed = FixedStyle.Left);
            }
        }

        /// <summary>
        /// GridView의 컬럼 폭을 고정한다.
        /// </summary>
        /// <param name="view">Gridview 개체</param>
        /// <param name="columns">Dictionary＜컬럼명, 폭＞</param>
        /// <param name="isFixMaxWidth">최대폭 고정여부(true:최대/최소 모두 고정, false:최소폭 고정)</param>
        public static void FixedColumnWidth(this GridView view, Dictionary<string, int> columns, bool isFixMaxWidth = false)
        {
            foreach (var item in columns)
            {
                view.Columns[item.Key].MinWidth = item.Value;
                if (isFixMaxWidth)
                    view.Columns[item.Key].MaxWidth = item.Value;
            }
        }

        /// <summary>
        /// GridView의 컬럼 폭을 고정한다.
        /// </summary>
        /// <param name="view">Gridview 개체</param>
        /// <param name="fieldName">컬럼명</param>
        /// <param name="width">폭</param>
        /// <param name="isFixMaxWidth">최대폭 고정여부(true:최대/최소 모두 고정, false:최소폭 고정)</param>
        public static void FixedColumnWidth(this GridView view, string fieldName, int width, bool isFixMaxWidth = false)
        {
            view.Columns[fieldName].MinWidth = width;
            if (isFixMaxWidth)
                view.Columns[fieldName].MaxWidth = width;
        }

        /// <summary>
        /// GridView상에서 EnterKey 입력시 지정된 컬럼(입력가능한 컬럼)으로 포커스를 이동한다.
        /// </summary>
        /// <param name="view">GridView Control</param>
        /// <param name="fieldNames">이동할 컬럼-이동순서대로 나열(컬럼명1,컬럼명2 ... 컬럼명n)</param>
        public static void MoveCellFocus(this GridView view)//, string fieldNames)
        {
            if (view.FocusedColumn == null)
                return;

            // Visible = true AND ColumnEdit != null AND ColumnEditType != ButtonEdit 인 컬럼 리스트 생성
            var targetFiels = string.Empty;
            foreach (GridColumn col in view.VisibleColumns)
            {

                if (col.Visible && col.ColumnEdit != null && col.ColumnEdit.EditorTypeName != "ButtonEdit")
                    targetFiels += $"{col.FieldName},";
            }

            // 문자열을 개개의 필드명으로 분리한다.
            var fields = targetFiels.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            // 데이터 저장 Dictionary 생성 => { 컬럼1, 컬럼2 }, { 컬럼2, 컬럼3 } .... { 컬럼n-1, 컬럼n } 형태로 생성
            var fieldList = new Dictionary<string, string>();
            for (int i = 0; i < fields.Length - 1; i++)
                fieldList.Add(fields[i].Trim(), fields[i + 1].Trim());

            // 현재 Focused 컬럼(Key값)으로 다음 이동 컬럼(Value값)을 찾는다.
            // 값을 찾지 못한 경우는 현재 행의 수정가능한 첫번째 컬럼(field[0])으로 이동한다. => 편집 컬럼 외 컬럼에서 Enter Key를 입력할 경우
            var nextField = (from field in fieldList
                             where field.Key == view.FocusedColumn.FieldName
                             select field.Value).DefaultIfEmpty(fields[0]).ToArray()[0].CString();

            // Focused 컬럼이 이동 대상의 마지막 컬럼이면 다음행이 존재하는지 확인 후 다음행의 첫번째 입력란으로 이동
            // Focused 컬럼이 이동 대상의 마지막 컬럼이 아니면 동일 행의 다음 이동대상 컬럼으로 이동
            if (view.FocusedColumn.FieldName == fields[fields.Length - 1])
            {
                if (view.RowCount - 1 > view.FocusedRowHandle)
                {
                    view.FocusedColumn = view.Columns[fields[0]];
                    view.FocusedRowHandle += 1;
                }
            }
            else
            {
                view.FocusedColumn = view.Columns[nextField];
                view.FocusedRowHandle = view.FocusedRowHandle;
            }
        }

        /// <summary>
        /// 그리드 컨트롤에서 2개 이상의 컬럼으로 행을 검색한다.
        /// </summary>
        /// <param name="view">GridView Control</param>
        /// <param name="columns">Dictionary＜컬럼명, 찾을값＞</param>
        /// <param name="startRow">찾기 시작행</param>
        public static void FindRowByValue(this GridView view, Dictionary<string, object> columns, int startRow = 0)
        {
            // 값을 검색할 컬럼 수 만큼 배열 선언
            var rst = new bool[columns.Count];
            var find = false;

            var viewInfo = view.GetViewInfo() as GridViewInfo;

            // 한 화면에 보여지는 행 수
            var rowCount = view.OptionsView.ShowAutoFilterRow ? viewInfo.RowsInfo.Count - 1 : viewInfo.RowsInfo.Count;

            // 행 전체를 반복
            for (int i = startRow; i < view.RowCount; i++)
            {
                // 값을 찾을 컬럼만큼 반복
                foreach (var x in columns.Select((Entry, Index) => new { Entry, Index }))
                {
                    // 찾을값과 셀값이 일치하는지 체크 (
                    rst[x.Index] = x.Entry.Value.CString().Equals(view.GetRowCellValue(i, x.Entry.Key).CString());
                    //rst[x.Index] = x.Entry.Value.CString() == view.GetRowCellValue(i, x.Entry.Key).CString();

                    // rst(결과 배열)에 false 값이 없으면 찾기 중단 및 행 위치 이동
                    if (!rst.Contains(false))
                    {
                        view.FocusedRowHandle = i;

                        // 이동 행이 한 화면의 표시 행수 보다 많으면 TopRow 변경(Scroll)
                        if (i > rowCount)
                            view.TopRowIndex = view.FocusedRowHandle;

                        find = true;
                        break;
                    }
                }

                if (find)
                    break;
            }
        }

        /// <summary>
        /// 그리드 컨트롤에서 단일 컬럼으로 행을 검색한다.
        /// </summary>
        /// <param name="view">GridView Control</param>
        /// <param name="fieldName">컬럼명</param>
        /// <param name="value">찾을 값</param>
        public static void FindRowByValue(this GridView view, string fieldName, object value)
        {
            var rowIndex = view.LocateByValue(fieldName, value);
            view.TopRowIndex = rowIndex;
            view.FocusedRowHandle = rowIndex;
        }

        /// <summary>
        /// GridView의 전체 행을 삭제
        /// </summary>
        /// <param name="view">GridView Control</param>
        public static void DeleteAllRows(this GridView view)
        {
            for (int i = 0; i < view.RowCount;)
                view.DeleteRow(i);
        }

        /// <summary>
        /// 지정된 범위(연속된 행)의 셀값을 변경합니다.
        /// </summary>
        /// <param name="view">GridView 컨트롤</param>
        /// <param name="startRow">시작 행</param>
        /// <param name="endRow">종료 행</param>
        /// <param name="fieldName">필드명(문자열 배열)</param>
        /// <param name="value">셀 입력값</param>
        public static void SetValueRange(this GridView view, int startRowHandle, int endRowHandle, string[] fieldName, object value)
        {
            // endRow의 값에 따라 for문의 조건이 RowCount(i < endRow)가 넘어 올때와 행 번호(i <= endRow)가 넘어올때 조건이 상이함
            // 조건 통일을 위해서 endRow가 RowCount와 동일하면 -1 후 for문 처리
            //endRow = (endRow == view.RowCount) ? endRow - 1 : endRow;

            // 파라메터로 RowHandler을 넘김 

            for (int i = startRowHandle; i <= endRowHandle; i++)
            {
                for (int j = 0; j < fieldName.Length; j++)
                    view.SetRowCellValue(i, fieldName[j], value);
            }
        }

        /// <summary>
        /// 지정된 범위(비 연속된 행:이웃하지 않은 다중Row 선택시)의 셀 값을 변경합니다.
        /// </summary>
        /// <param name="view">GridView 컨트롤</param>
        /// <param name="fieldName">필드명(문자열 배열)</param>
        /// <param name="value">셀 입력값</param>
        public static void SetValueRange(this GridView view, string[] fieldName, object value)
        {
            if (view.SelectedRowsCount <= 0)
                return;

            view.GetSelectedRows().ToList().ForEach(rowHandle =>
            {
                for (int j = 0; j < fieldName.Length; j++)
                    view.SetRowCellValue(rowHandle, fieldName[j], value);
            });
        }

        ///// <summary>
        ///// 그리드의 다중선택 속성을 활성화
        ///// </summary>
        ///// <param name="view">GridView Control</param>
        ///// <param name="isMultiSelect">true</param>
        //public static void ActivateMultiSelectRow(this GridView view, bool isMultiSelect)
        //{
        //    view.OptionsSelection.MultiSelectMode = GridMultiSelectMode.RowSelect;
        //    view.OptionsSelection.MultiSelect = true;
        //}

        #endregion

        #region GridView Summary Item and Group Summary Item
        /// <summary>
        /// 그리드 뷰 컨트롤에 SummaryItem을 지정합니다.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="columnName"></param>
        /// <param name="type"></param>
        /// <param name="displayFormat"></param>
        public static void SetSummaryItem(this GridView view, string fieldName, DevExpress.Data.SummaryItemType type, string displayFormat, string tag = "")
        {
            view.Columns[fieldName].SummaryItem.SummaryType = type;
            view.Columns[fieldName].SummaryItem.DisplayFormat = displayFormat;
            view.Columns[fieldName].SummaryItem.Tag = tag;
        }

        /// <summary>
        /// 그리드 뷰 컨트롤에 SummaryGroupItem을 지정합니다.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="fieldName"></param>
        /// <param name="type"></param>
        /// <param name="displayFormat"></param>
        /// <param name="tag"></param>
        public static void SetGroupSummaryItem(this GridView view, string fieldName, DevExpress.Data.SummaryItemType type, string displayFormat, string tag = "")
        {
            var item = new GridGroupSummaryItem
            {
                FieldName = fieldName,
                SummaryType = type,
                ShowInGroupColumnFooter = view.Columns[fieldName],
                DisplayFormat = displayFormat,
                Tag = tag
            };
            view.GroupSummary.Add(item);
        }

        #endregion

        #region GridControl Repository Items
        /// <summary>
        /// RepositoryItemTextEdit 개체를 생성한다.
        /// </summary>
        /// <param name="view">대상 GridView</param>
        /// <param name="columnName">대상 컬럼명</param>
        /// <returns></returns>
        public static RepositoryItemTextEdit SetRepositoryItemTextEdit(this GridView view, string columnName)
        {
            var riTextEdit = new RepositoryItemTextEdit();
            riTextEdit.Appearance.Options.UseTextOptions = true;
            riTextEdit.Appearance.ForeColor = Color.White;
            riTextEdit.Appearance.BackColor = UserColors.EditableColor;

            view.GridControl.RepositoryItems.Add(riTextEdit);
            view.Columns[columnName].ColumnEdit = riTextEdit;
            view.Columns[columnName].OptionsColumn.AllowEdit = true;

            return riTextEdit;
        }

        /// <summary>
        /// RepositoryItemDateEdit 개체를 생성한다.
        /// </summary>
        /// <param name="view">대상 GridView</param>
        /// <param name="columnName">대상 컬럼명</param>
        /// <param name="vStyle">표시 형식(일, 월, 년)</param>
        /// <returns></returns>
        public static RepositoryItemDateEdit SetRepositoryItemDateEdit(this GridView view, string columnName, ViewStyle vStyle = ViewStyle.DateView)
        {
            var riDateEdit = new RepositoryItemDateEdit();
            riDateEdit.Appearance.Options.UseTextOptions = true;
            riDateEdit.Appearance.ForeColor = Color.White;
            riDateEdit.Appearance.BackColor = UserColors.EditableColor;

            if (vStyle != ViewStyle.DateView)
            {
                switch (vStyle)
                {
                    case ViewStyle.MonthView:
                        riDateEdit.EditMask = "yyyy-MM";
                        riDateEdit.VistaCalendarViewStyle = VistaCalendarViewStyle.YearView;
                        break;

                    case ViewStyle.YearView:
                        riDateEdit.EditMask = "yyyy";
                        riDateEdit.VistaCalendarViewStyle = VistaCalendarViewStyle.YearsGroupView;
                        break;

                    default:
                        break;
                }

                riDateEdit.UseMaskAsDisplayFormat = true;
                riDateEdit.VistaDisplayMode = DefaultBoolean.True;
            }

            view.GridControl.RepositoryItems.Add(riDateEdit);
            view.Columns[columnName].ColumnEdit = riDateEdit;
            view.Columns[columnName].OptionsColumn.AllowEdit = true;

            return riDateEdit;
        }

        /// <summary>
        /// RepositoryItemButtonEdit 개체를 생성한다.
        /// </summary>
        /// <param name="view">대상 GridView</param>
        /// <param name="columnName">대상 컬럼명</param>
        /// <returns></returns>
        public static RepositoryItemButtonEdit SetRepositoryItemButtonEdit(this GridView view, string columnName)
        {
            var riButtonEdit = new RepositoryItemButtonEdit();

            riButtonEdit.TextEditStyle = TextEditStyles.Standard;
            riButtonEdit.Appearance.Options.UseTextOptions = true;
            riButtonEdit.Appearance.ForeColor = Color.White;
            riButtonEdit.Appearance.BackColor = UserColors.EditableColor;

            riButtonEdit.Buttons.Clear();
            riButtonEdit.Buttons.Add(new EditorButton(ButtonPredefines.Glyph));
            riButtonEdit.Buttons[0].ImageOptions.ImageToTextAlignment = ImageAlignToText.LeftCenter;
            riButtonEdit.Buttons[0].ImageOptions.ImageUri.Uri = "Zoom;Size16x16;Colored";
            riButtonEdit.Buttons[0].Tag = "FIND";

            riButtonEdit.Buttons.Add(new EditorButton(ButtonPredefines.Glyph));
            riButtonEdit.Buttons[1].ImageOptions.ImageToTextAlignment = ImageAlignToText.LeftCenter;
            riButtonEdit.Buttons[1].ImageOptions.ImageUri.Uri = "RemovePivotField;Size16x16;Colored";
            riButtonEdit.Buttons[1].Tag = "CLEAR";

            view.GridControl.RepositoryItems.Add(riButtonEdit);
            view.Columns[columnName].ColumnEdit = riButtonEdit;
            view.Columns[columnName].OptionsColumn.AllowEdit = true;

            riButtonEdit.ButtonClick += (s, e) =>
            {
                if (e.Button.Tag.CString() == "CLEAR")
                    view.SetFocusedRowCellValue(columnName, null);
                else if (e.Button.Tag.CString() == "FIND")
                {
                    if (ButtonEditEvent != null)
                    {
                        //ButtonEditEvent(s);

                        ButtonEdit bte = s as ButtonEdit;
                        bte.Name = $"{view.Name}_{columnName}";
                        ButtonEditEvent(bte);
                    }
                    else
                        MsgBox.ErrorMessage("Event Error!");
                }
            };

            riButtonEdit.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (ButtonEditEvent != null)
                    {
                        //ButtonEditEvent(s);

                        ButtonEdit bte = s as ButtonEdit;
                        bte.Name = $"{view.Name}_{columnName}";
                        //ButtonEditEvent(bte);

                        if (ButtonEditEvent(bte) == false)
                            e.Handled = true;
                    }
                    else
                        MsgBox.ErrorMessage("Event Error!");
                }
            };

            return riButtonEdit;
        }

        /// <summary>
        /// RepositoryItemButtonEdit(Simple 버튼) 개체를 생성한다.
        /// </summary>
        /// <param name="view">대상 GridView</param>
        /// <param name="columnName">대상 컬럼명</param>
        /// <param name="buttonType">버튼 유형(이미지 설정)</param>
        /// <param name="buttonText">버튼 텍스트</param>
        /// <returns></returns>
        public static RepositoryItemButtonEdit SetRepositoryItemSimpleButton(this GridView view, string columnName, RepositoryButtonType buttonType, string buttonText = "")
        {
            var riButtonEdit = new RepositoryItemButtonEdit();

            riButtonEdit.TextEditStyle = TextEditStyles.HideTextEditor;
            riButtonEdit.Appearance.Options.UseTextOptions = true;
            riButtonEdit.Appearance.ForeColor = Color.White;
            riButtonEdit.Appearance.BackColor = UserColors.EditableColor;

            riButtonEdit.Buttons.Clear();
            riButtonEdit.Buttons.Add(new EditorButton(ButtonPredefines.Glyph));
            riButtonEdit.Buttons[0].ImageOptions.ImageToTextAlignment = ImageAlignToText.LeftCenter;

            if (buttonType != RepositoryButtonType.None)
            {
                switch (buttonType)
                {
                    case RepositoryButtonType.Add:
                        riButtonEdit.Buttons[0].ImageOptions.ImageUri.Uri = "Add;Size16x16;Colored";
                        break;

                    case RepositoryButtonType.Delete:
                        riButtonEdit.Buttons[0].ImageOptions.ImageUri.Uri = "Remove;Size16x16;Colored";
                        break;

                    case RepositoryButtonType.Find:
                        riButtonEdit.Buttons[0].ImageOptions.ImageUri.Uri = "Zoom;Size16x16;Colored";
                        break;

                    case RepositoryButtonType.Download:
                        riButtonEdit.Buttons[0].ImageOptions.ImageUri.Uri = "Download;Size16x16;Colored";
                        break;
                }
            }

            if (!string.IsNullOrEmpty(buttonText))
                riButtonEdit.Buttons[0].Caption = buttonText;

            view.GridControl.RepositoryItems.Add(riButtonEdit);
            view.Columns[columnName].ColumnEdit = riButtonEdit;
            view.Columns[columnName].OptionsColumn.AllowEdit = true;

            riButtonEdit.LookAndFeel.UseDefaultLookAndFeel = false;
            riButtonEdit.LookAndFeel.SkinName = "Caramel";
            riButtonEdit.Buttons[0].Appearance.ForeColor = Color.Black;

            return riButtonEdit;
        }

        /// <summary>
        /// RepositoryItemGridLookUpEdit 개체를 생성한다.
        /// </summary>
        /// <param name="view">대상 GridView</param>
        /// <param name="columnName">대상 컬럼명</param>
        /// <param name="source">데이터 소스</param>
        /// <param name="valueMember">값 컬러명</param>
        /// <param name="displayMember">표시 컬럼명</param>
        /// <param name="hiddenColumns">숨김 컬럼리스트(콤마로 구분)</param>
        /// <returns></returns>
        public static RepositoryItemGridLookUpEdit SetRepositoryItemGridLookUpEdit(this GridView view, string columnName, DataTable source, string valueMember, string displayMember, string hiddenColumns = "")
        {
            var riGridLookUpEdit = new RepositoryItemGridLookUpEdit();
            riGridLookUpEdit.ShowFooter = false;
            riGridLookUpEdit.Appearance.Options.UseTextOptions = true;
            riGridLookUpEdit.Appearance.ForeColor = Color.White;
            riGridLookUpEdit.Appearance.BackColor = UserColors.EditableColor;

            riGridLookUpEdit.DataSource = source;
            riGridLookUpEdit.ValueMember = valueMember;
            riGridLookUpEdit.DisplayMember = displayMember;
            riGridLookUpEdit.NullText = string.Empty;
            //riGridLookUpEdit.View.OptionsView.ShowColumnHeaders = false;
            riGridLookUpEdit.View.OptionsView.ShowAutoFilterRow = true;
            riGridLookUpEdit.View.PopulateColumns(riGridLookUpEdit.DataSource);
            riGridLookUpEdit.View.Appearance.HeaderPanel.Options.UseTextOptions = true;
            riGridLookUpEdit.View.Appearance.HeaderPanel.TextOptions.HAlignment = HorzAlignment.Center;

            riGridLookUpEdit.Buttons.Clear();
            riGridLookUpEdit.Buttons.Add(new EditorButton(ButtonPredefines.Glyph));
            riGridLookUpEdit.Buttons[0].ImageOptions.ImageToTextAlignment = ImageAlignToText.LeftCenter;
            riGridLookUpEdit.Buttons[0].ImageOptions.ImageUri.Uri = "Zoom;Size16x16;Colored";
            riGridLookUpEdit.Buttons[0].Tag = "FIND";

            riGridLookUpEdit.Buttons.Add(new EditorButton(ButtonPredefines.Glyph));
            riGridLookUpEdit.Buttons[1].ImageOptions.ImageToTextAlignment = ImageAlignToText.LeftCenter;
            riGridLookUpEdit.Buttons[1].ImageOptions.ImageUri.Uri = "RemovePivotField;Size16x16;Colored";
            riGridLookUpEdit.Buttons[1].Tag = "CLEAR";

            riGridLookUpEdit.SetColumns(source, hiddenColumns);

            view.GridControl.RepositoryItems.Add(riGridLookUpEdit);
            view.Columns[columnName].ColumnEdit = riGridLookUpEdit;
            view.Columns[columnName].OptionsColumn.AllowEdit = true;

            riGridLookUpEdit.ButtonClick += (s, e) =>
            {
                if (e.Button.Tag.CString() == "CLEAR")
                    view.SetFocusedRowCellValue(columnName, null);
            };

            return riGridLookUpEdit;
        }

        /// <summary>
        /// RepositoryItemGridLookUpEdit 컬럼 초기화한다.
        /// </summary>
        /// <param name="riGridLookUpEdit">RepositoryItemGridLookUpEdit 개체</param>
        /// <param name="source">Data Source</param>
        /// <param name="hiddenColumns">숨김 컬럼 리스트</param>
        private static void SetColumns(this RepositoryItemGridLookUpEdit riGridLookUpEdit, DataTable source, string hiddenColumns)
        {
            try
            {
                riGridLookUpEdit.View.Columns.Clear();
                riGridLookUpEdit.DataSource = source;
                riGridLookUpEdit.PopulateViewColumns();

                // 컬럼 설정
                riGridLookUpEdit.View.Columns.ToList().ForEach(column =>
                {
                    column.OptionsColumn.AllowEdit = false;
                    column.FilterMode = ColumnFilterMode.DisplayText;
                    column.AppearanceHeader.FontStyleDelta = FontStyle.Bold;
                    column.Visible = true;

                    // 용어 처리
                    var rows = AppConfig.Glossary.Select($"{keyField} = '{column.FieldName}'");
                    if (rows.Length > 0)
                    {
                        column.Caption = rows[0][captionField].CString();
                        column.AppearanceCell.TextOptions.HAlignment = (HorzAlignment)rows[0][hAlignField].CShort();
                        column.AppearanceCell.TextOptions.VAlignment = (VertAlignment)rows[0][vAlignField].CShort();
                        column.DisplayFormat.FormatType = (FormatType)rows[0][typeField].CShort();
                        column.DisplayFormat.FormatString = rows[0][typeStringField].CString();
                    }

                    //컬럼 숨김 처리
                    if (!string.IsNullOrEmpty(hiddenColumns))
                    {
                        if (hiddenColumns.Contains(column.FieldName))
                            column.Visible = false;
                    }
                });

                // 사용여부에 따른 취소선 표시
                riGridLookUpEdit.View.RowStyle += (s, e) =>
                {
                    var view = s as GridView;

                    if (e.RowHandle >= 0 && view.Columns["UseClss"] != null && view.GetRowCellValue(e.RowHandle, "UseClss").CString() == "1")
                    {
                        e.Appearance.FontStyleDelta = FontStyle.Strikeout | FontStyle.Italic;
                        e.Appearance.ForeColor = UserColors.NotUseForeColor;
                    }
                };

                // 코드 컬럼을 숨김 처리(코드명만 표시), 명칭 컬럼은 무조건 왼쪽 정렬
                riGridLookUpEdit.View.Columns[0].Visible = false;
                riGridLookUpEdit.View.Columns[1].AppearanceCell.TextOptions.HAlignment = HorzAlignment.Near;

                // 첫번째 컬럼의 헤드 텍스트 변경
                riGridLookUpEdit.View.Columns[1].Caption = "MSG_SELECT_ITEM".Localization();

                riGridLookUpEdit.BestFitMode = BestFitMode.BestFitResizePopup;

                //컬럼 폭 조정
                riGridLookUpEdit.View.BestFitColumns();
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// RepositoryItemSearchLookUpEdit 개체를 생성한다.
        /// </summary>
        /// <param name="view">대상 GridView</param>
        /// <param name="columnName">대상 컬럼명</param>
        /// <param name="source">데이터 소스</param>
        /// <param name="valueMember">값 컬러명</param>
        /// <param name="displayMember">표시 컬럼명</param>
        /// <returns></returns>
        public static RepositoryItemSearchLookUpEdit SetRepositoryItemSearchLookUpEdit(this GridView view, string columnName, DataTable source, string valueMember, string displayMember, string hiddenColumns = "")
        {
            var riSearchLookUpEdit = new RepositoryItemSearchLookUpEdit();

            riSearchLookUpEdit.ShowFooter = false;
            riSearchLookUpEdit.BestFitMode = BestFitMode.BestFit;
            riSearchLookUpEdit.NullText = string.Empty;
            riSearchLookUpEdit.Appearance.Options.UseTextOptions = true;
            riSearchLookUpEdit.Appearance.ForeColor = Color.White;
            riSearchLookUpEdit.Appearance.BackColor = UserColors.EditableColor;

            riSearchLookUpEdit.View.Appearance.HeaderPanel.Options.UseTextOptions = true;
            riSearchLookUpEdit.View.Appearance.HeaderPanel.FontStyleDelta = FontStyle.Bold;
            riSearchLookUpEdit.View.Appearance.HeaderPanel.TextOptions.HAlignment = HorzAlignment.Center;
            riSearchLookUpEdit.View.Appearance.VertLine.Options.UseBackColor = true;
            riSearchLookUpEdit.View.Appearance.HorzLine.Options.UseBackColor = true;
            riSearchLookUpEdit.View.Appearance.VertLine.BackColor = UserColors.LineColor;
            riSearchLookUpEdit.View.Appearance.HorzLine.BackColor = UserColors.LineColor;

            riSearchLookUpEdit.Buttons.Clear();
            riSearchLookUpEdit.Buttons.Add(new EditorButton(ButtonPredefines.Glyph));
            riSearchLookUpEdit.Buttons[0].ImageOptions.ImageToTextAlignment = ImageAlignToText.LeftCenter;
            riSearchLookUpEdit.Buttons[0].ImageOptions.ImageUri.Uri = "Zoom;Size16x16;Colored";
            riSearchLookUpEdit.Buttons[0].Tag = "FIND";

            riSearchLookUpEdit.Buttons.Add(new EditorButton(ButtonPredefines.Glyph));
            riSearchLookUpEdit.Buttons[1].ImageOptions.ImageToTextAlignment = ImageAlignToText.LeftCenter;
            riSearchLookUpEdit.Buttons[1].ImageOptions.ImageUri.Uri = "RemovePivotField;Size16x16;Colored";
            riSearchLookUpEdit.Buttons[1].Tag = "CLEAR";

            riSearchLookUpEdit.DataSource = source;
            riSearchLookUpEdit.ValueMember = valueMember;
            riSearchLookUpEdit.DisplayMember = displayMember;

            SetColumns(riSearchLookUpEdit, source, hiddenColumns);

            view.GridControl.RepositoryItems.Add(riSearchLookUpEdit);
            view.Columns[columnName].ColumnEdit = riSearchLookUpEdit;
            view.Columns[columnName].OptionsColumn.AllowEdit = true;

            riSearchLookUpEdit.ButtonClick += (s, e) =>
            {
                if (e.Button.Tag.CString() == "CLEAR")
                    view.SetFocusedRowCellValue(columnName, null);
            };

            return riSearchLookUpEdit;
        }

        /// <summary>
        /// RepositoryItemSearchLookUpEdit 컬럼 설정
        /// </summary>
        /// <param name="riSearchLookUpEdit"></param>
        /// <param name="table"></param>
        /// <param name="hiddenColumns"></param>
        private static void SetColumns(this RepositoryItemSearchLookUpEdit riSearchLookUpEdit, DataTable table, string hiddenColumns)
        {
            try
            {
                riSearchLookUpEdit.View.Columns.Clear();
                riSearchLookUpEdit.DataSource = table;
                riSearchLookUpEdit.PopupWidthMode = PopupWidthMode.UseEditorWidth;
                riSearchLookUpEdit.PopulateViewColumns();

                riSearchLookUpEdit.BestFitMode = BestFitMode.BestFitResizePopup;
                riSearchLookUpEdit.View.Appearance.HeaderPanel.Options.UseTextOptions = true;
                riSearchLookUpEdit.View.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

                riSearchLookUpEdit.View.Columns.ToList().ForEach(column =>
                {
                    column.OptionsColumn.AllowEdit = false;
                    column.FilterMode = ColumnFilterMode.DisplayText;

                    // View의 컬럼 설정
                    DataRow[] resultRows = AppConfig.Glossary.Select($"{keyField} = '{column.FieldName}'");
                    if (resultRows.Length > 0)
                    {
                        column.Caption = resultRows[0][captionField].CString();
                        column.AppearanceCell.TextOptions.VAlignment = (VertAlignment)resultRows[0][hAlignField].CShort();
                        column.AppearanceCell.TextOptions.HAlignment = (HorzAlignment)resultRows[0][vAlignField].CShort();
                        column.DisplayFormat.FormatType = (FormatType)resultRows[0][typeField].CShort();
                        column.DisplayFormat.FormatString = resultRows[0][typeStringField].CString();
                    }

                    //컬럼 숨김 처리
                    if (!string.IsNullOrEmpty(hiddenColumns))
                    {
                        if (hiddenColumns.Contains(column.FieldName))
                            column.Visible = false;
                    }
                });

                // 사용여부에 따른 취소선 표시
                riSearchLookUpEdit.View.RowStyle += (s, e) =>
                {
                    GridView view = s as GridView;

                    if (e.RowHandle >= 0 && view.Columns["UseClss"] != null && view.GetRowCellValue(e.RowHandle, "UseClss").CString() == "1")
                    {
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Strikeout);// | FontStyle.Italic);
                        e.Appearance.ForeColor = UserColors.NotUseForeColor;
                    }
                };


                riSearchLookUpEdit.BestFitMode = BestFitMode.BestFitResizePopup;

                riSearchLookUpEdit.View.BestFitColumns();  //최초 헤드 설정시 헤드 텍스트에 맞게 컬럼 폭 조정

            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// RepositoryItemCheckEdit 개체를 생성한다.
        /// </summary>
        /// <param name="view">대상 GridView</param>
        /// <param name="columnName">대상 컬럼명</param>
        /// <param name="allowEdit">수정 여부</param>
        /// <returns></returns>
        public static RepositoryItemCheckEdit SetRepositoryItemCheckEdit(this GridView view, string columnName, bool allowEdit = true)
        {
            var riCheckEdit = new RepositoryItemCheckEdit();
            riCheckEdit.Appearance.Options.UseTextOptions = true;
            riCheckEdit.Appearance.ForeColor = Color.White;
            riCheckEdit.Appearance.BackColor = UserColors.EditableColor;
            riCheckEdit.ValueChecked = "0";
            riCheckEdit.ValueUnchecked = "1";
            riCheckEdit.AllowGrayed = false;
            riCheckEdit.CheckBoxOptions.Style = CheckBoxStyle.Default;

            view.GridControl.RepositoryItems.Add(riCheckEdit);
            view.Columns[columnName].ColumnEdit = riCheckEdit;
            view.Columns[columnName].OptionsColumn.AllowEdit = allowEdit;

            return riCheckEdit;
        }

        /// <summary>
        /// RepositoryItemRadioGroup  개체를 생성한다.
        /// </summary>
        /// <param name="view">대상 GridView</param>
        /// <param name="columnName">대상 컬럼명</param>
        /// <param name="radioItems">표시 항목(값1:설명1|값2:설명2|값3:설명3| ... |값n:설명n</param>
        /// <returns></returns>
        public static RepositoryItemRadioGroup SetRepositoryItemRadioGroup(this GridView view, string columnName, string radioItems)
        {
            var riRadioGroup = new RepositoryItemRadioGroup();
            riRadioGroup.Appearance.Options.UseTextOptions = true;
            //riRadioGroup.Appearance.ForeColor = Color.Black;
            riRadioGroup.Appearance.BackColor = UserColors.EditableColor;

            riRadioGroup.Items.Clear();
            if (!string.IsNullOrEmpty(radioItems))
            {
                var items = radioItems.Split('|');
                if (items.Length > 0)
                {
                    riRadioGroup.Columns = items.Length;

                    for (int i = 0; i < items.Length; i++)
                    {
                        var column = items[i].Split(':');
                        riRadioGroup.Items.Add(new RadioGroupItem(column[0].CString().Trim(), column[1].CString().Trim()));
                    }
                }
            }

            view.GridControl.RepositoryItems.Add(riRadioGroup);
            view.Columns[columnName].ColumnEdit = riRadioGroup;
            view.Columns[columnName].OptionsColumn.AllowEdit = true;

            return riRadioGroup;
        }

        /// <summary>
        /// RepositoryItemSpinEdit 개체를 생성한다.
        /// </summary>
        /// <param name="view">대상 GridView</param>
        /// <param name="columnName">대상 컬럼명</param>
        /// <param name="maskFormat">입력 형식</param>
        /// <returns></returns>
        public static RepositoryItemSpinEdit SetRepositoryItemSpinEdit(this GridView view, string columnName)//, string maskFormat)
        {
            var riSpinEdit = new RepositoryItemSpinEdit();
            riSpinEdit.Appearance.Options.UseTextOptions = true;
            //riSpinEdit.Appearance.ForeColor = Color.White;
            riSpinEdit.Appearance.BackColor = UserColors.EditableColor;
            riSpinEdit.Mask.UseMaskAsDisplayFormat = true;
            //riSpinEdit.Mask.EditMask = maskFormat;

            // 입력형식 처리
            var rows = AppConfig.Glossary.Select($"{keyField} = '{columnName}'");
            if (rows.Length > 0)
            {
                riSpinEdit.Appearance.TextOptions.HAlignment = (HorzAlignment)rows[0][hAlignField].CShort();
                riSpinEdit.Appearance.TextOptions.VAlignment = (VertAlignment)rows[0][vAlignField].CShort();
                riSpinEdit.DisplayFormat.FormatType = (FormatType)rows[0][typeField].CShort();
                riSpinEdit.Mask.EditMask = rows[0][typeStringField].CString();
            }

            view.GridControl.RepositoryItems.Add(riSpinEdit);
            view.Columns[columnName].ColumnEdit = riSpinEdit;
            view.Columns[columnName].OptionsColumn.AllowEdit = true;

            return riSpinEdit;
        }

        #endregion

        #region TreeList
        /// <summary>
        /// TreeList 컬럼을 설정한다.
        /// </summary>
        /// <param name="tree">TreeList 컨트롤</param>
        /// <param name="source">Data Source</param>
        /// <param name="hiddenColumns">숨김 컬럼(콤마로 구분)</param>
        /// <param name="showFilterRow">Filter Row 표시 여부</param>
        /// <param name="autoColumnWidth">자동 폭 조정 여부</param>
        public static void SetColumns(this TreeList tree, DataTable source, string hiddenColumns = "", bool showFilterRow = false, bool autoColumnWidth = false)
        {
            tree.Columns.Clear();
            tree.DataSource = source;

            tree.OptionsView.ShowAutoFilterRow = showFilterRow;
            tree.OptionsView.AutoWidth = autoColumnWidth;

            tree.Columns.ToList().ForEach(column =>
            {
                column.OptionsColumn.AllowEdit = false;
                column.FilterMode = ColumnFilterMode.DisplayText;
                column.SortMode = ColumnSortMode.DisplayText;
                column.AppearanceHeader.FontStyleDelta = FontStyle.Bold;    // 컬럼헤더만 진하게 (Row는 일반)

                // 용어 처리
                var rows = AppConfig.Glossary.Select($"{keyField} = '{column.FieldName}'");
                if (rows.Length > 0)
                {
                    column.Caption = rows[0][captionField].CString();
                    column.AppearanceCell.TextOptions.HAlignment = (HorzAlignment)rows[0][hAlignField].CShort();
                    column.AppearanceCell.TextOptions.VAlignment = (VertAlignment)rows[0][vAlignField].CShort();
                    column.Format.FormatType = (FormatType)rows[0][typeField].CShort();
                    column.Format.FormatString = rows[0][typeStringField].CString();
                }
            });

            // 사용여부 설정
            if (tree.Columns["UseClss"] != null)
            {
                tree.NodeCellStyle += (s, e) =>
                {
                    if (e.Node["UseClss"] != null && e.Node["UseClss"].CString() == "1")
                    {
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Strikeout | FontStyle.Italic);
                        e.Appearance.ForeColor = UserColors.NotUseForeColor;
                    }
                };
            }

            //컬럼 숨김 처리
            if (!string.IsNullOrEmpty(hiddenColumns))
            {
                hiddenColumns.Split(',').ToList().ForEach(colText =>
                {
                    if (tree.Columns[colText.Trim()] != null)
                        tree.Columns[colText.Trim()].Visible = false;
                });
            }

            tree.BestFitColumns();
        }

        /// <summary>
        /// TreeList에 데이터를 표시한다.
        /// </summary>
        /// <param name="tree">TreeList Control</param>
        /// <param name="source">DataTable 형식의 데이터 소스</param>
        /// <param name="expandLevel">Node의 확장 레벨(-1 : 전체축소, 0 ~ n : 지정된 레벨 하위 레벨을 확장) </param>
        public static void SetData(this TreeList tree, DataTable source, int expandLevel = -1)
        {
            try
            {
                tree.BeginUpdate();
                tree.DataSource = null;
                tree.DataSource = source;
                tree.BestFitColumns();

                if (expandLevel == -1)
                    tree.CollapseAll();
                else
                    tree.ExpandToLevel(expandLevel);

                tree.EndUpdate();
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// TreeList의 클릭한 Node를 반환한다.
        /// </summary>
        /// <param name="tree">TreeList Control</param>
        /// <param name="point">MousePoint : Control.MousePoint</param>
        /// <returns>TreeListNode object</returns>
        public static TreeListNode GetClickedNode(this TreeList tree, Point point)
        {
            TreeListNode node;

            try
            {
                var hitInfo = tree.CalcHitInfo(tree.PointToClient(point));
                if (hitInfo.HitInfoType != HitInfoType.Cell)
                    return null;

                node = hitInfo.Node;
            }
            catch
            {
                node = null;
            }

            return node;
        }

        /// <summary>
        /// KeyField 값으로 Node를 검색 후 해당 Node가 화면에 보여지도록 TopVisibleNodeIndex를 설정한다.
        /// </summary>
        /// <param name="tree">TreeList Control</param>
        /// <param name="value">찾을 KeyField의 값</param>
        public static void FindNodeByValue(this TreeList tree, object value)
        {
            var node = tree.FindNodeByFieldValue("KeyField", value);
            tree.FocusedNode = node;
            var index = tree.GetVisibleIndexByNode(node);

            if (index > tree.ViewInfo.RowsInfo.Rows.Count)
                tree.TopVisibleNodeIndex = index;
        }

        #endregion

        #region LookUpEdit
        /// <summary>
        /// LookUpEdit에 데이터를 표시한다.
        /// </summary>
        /// <param name="lookUpEdit">LookUpEdit Control</param>
        /// <param name="source">DataTable 형식의 데이터 소스</param>
        /// <param name="defaultValue">기본 설정값</param>
        /// <param name="hiddenColumns">숨김 컬럼 리스트</param>
        public static void SetData(this LookUpEdit lookUpEdit, DataTable source, string hiddenColumns = "", object defaultValue = null)
        {
            if (source == null)
                return;

            try
            {
                lookUpEdit.Properties.Columns.Clear();
                lookUpEdit.Properties.DataSource = source;
                lookUpEdit.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
                lookUpEdit.Properties.ValueMember = source.Columns[0].ColumnName;
                lookUpEdit.Properties.DisplayMember = source.Columns[1].ColumnName;
                lookUpEdit.Properties.ForceInitialize();
                lookUpEdit.Properties.PopulateColumns();

                // 코드 컬럼을 숨김 처리(코드명만 표시)
                lookUpEdit.Properties.Columns[0].Visible = false;

                // 첫번째 컬럼의 헤드 텍스트 변경
                lookUpEdit.Properties.Columns[1].Caption = "MSG_SELECT_ITEM".Localization();

                //컬럼 숨김 처리
                if (!string.IsNullOrEmpty(hiddenColumns))
                {
                    hiddenColumns.Split(',').ToList().ForEach(colText =>
                    {
                        if (lookUpEdit.Properties.Columns[colText.Trim()] != null)
                            lookUpEdit.Properties.Columns[colText.Trim()].Visible = false;
                    });
                }

                // 기본값 설정
                lookUpEdit.EditValue = defaultValue;

                //Form Load시 크기 변화가 없는 경우는 아래 문장으로, 크기가 변경되는 거는 아래 SizeChanged 이벤트에서
                lookUpEdit.Properties.PopupFormMinSize = new Size(lookUpEdit.Width, lookUpEdit.Properties.PopupFormSize.Height);

                //Pop-Up Window의 최소 Width를 컨트롤의 크기로 설정
                lookUpEdit.SizeChanged += (s, e) =>
                {
                    lookUpEdit.Properties.PopupFormMinSize = new Size(lookUpEdit.Width, lookUpEdit.Properties.PopupFormSize.Height);
                };

            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }
        #endregion

        #region GridLookUpEdit
        /// <summary>
        /// GridLookUpEdit에 데이터를 표시한다.
        /// </summary>
        /// <param name="gridLookUpEdit">GridLookUpEdit Control</param>
        /// <param name="source">DataTable 형식의 데이터 소스</param>
        /// <param name="defaultValue">기본 설정값</param>
        /// <param name="hiddenColumns">숨길 컬럼 리스트(컬럼명1,컬럼명2,...,컬럼명n)</param>
        public static void SetData(this GridLookUpEdit gridLookUpEdit, DataTable source, string hiddenColumns = "", object defaultValue = null)
        {
            if (source == null)
                return;

            try
            {
                gridLookUpEdit.Properties.View.Columns.Clear();
                gridLookUpEdit.Properties.DataSource = source;
                gridLookUpEdit.Properties.ValueMember = source.Columns[0].ColumnName;
                gridLookUpEdit.Properties.DisplayMember = source.Columns[1].ColumnName;
                gridLookUpEdit.Properties.View.PopulateColumns(source);

                // 컬럼 설정
                gridLookUpEdit.Properties.View.Columns.ToList().ForEach(column =>
                {
                    column.FilterMode = ColumnFilterMode.DisplayText;
                    column.SortMode = ColumnSortMode.DisplayText;

                    // 용어 처리
                    var rows = AppConfig.Glossary.Select($"{keyField} = '{column.FieldName}'");
                    if (rows.Length > 0)
                    {
                        column.Caption = rows[0][captionField].CString();
                        column.AppearanceCell.TextOptions.HAlignment = (HorzAlignment)rows[0][hAlignField].CShort();
                        column.AppearanceCell.TextOptions.VAlignment = (VertAlignment)rows[0][vAlignField].CShort();
                        column.DisplayFormat.FormatType = (FormatType)rows[0][typeField].CShort();
                        column.DisplayFormat.FormatString = rows[0][typeStringField].CString();
                    }
                });

                // 사용여부에 따른 취소선 표시
                gridLookUpEdit.Properties.View.RowStyle += (s, e) =>
                {
                    var view = s as GridView;

                    if (e.RowHandle >= 0 && view.Columns["UseClss"] != null && view.GetRowCellValue(e.RowHandle, "UseClss").CString() == "1")
                    {
                        e.Appearance.FontStyleDelta = FontStyle.Strikeout | FontStyle.Italic;
                        e.Appearance.ForeColor = Color.DarkGray;
                    }
                };

                // 코드 컬럼을 숨김 처리(코드명만 표시), 명칭 컬럼은 무조건 왼쪽 정렬
                gridLookUpEdit.Properties.View.Columns[0].Visible = false;
                //gridLookUpEdit.Properties.View.Columns[1].AppearanceCell.TextOptions.HAlignment = HorzAlignment.Near;

                // 첫번째 컬럼의 헤드 텍스트 변경
                gridLookUpEdit.Properties.View.Columns[1].Caption = "MSG_SELECT_ITEM".Localization();

                //컬럼 숨김 처리
                if (!string.IsNullOrEmpty(hiddenColumns))
                {
                    hiddenColumns.Split(',').ToList().ForEach(colText =>
                    {
                        if (gridLookUpEdit.Properties.View.Columns[colText.Trim()] != null)
                            gridLookUpEdit.Properties.View.Columns[colText.Trim()].Visible = false;
                    });
                }

                // 기본값 설정
                gridLookUpEdit.EditValue = defaultValue;

                //Form Load시 크기 변화가 없는 경우는 아래 문장으로, 크기가 변경되는 거는 아래 SizeChanged 이벤트에서
                gridLookUpEdit.Properties.PopupFormMinSize = new Size(gridLookUpEdit.Width, gridLookUpEdit.Properties.PopupFormSize.Height);

                //Pop-Up Window의 최소 Width를 컨트롤의 크기로 설정
                gridLookUpEdit.SizeChanged += (s, e) =>
                {
                    gridLookUpEdit.Properties.PopupFormMinSize = new Size(gridLookUpEdit.Width, gridLookUpEdit.Properties.PopupFormSize.Height);
                };
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }
        #endregion

        #region LookUpEditBase (Cascading, AddItem)
        /// <summary>
        /// 두개의 LookUpEditBase 컨트롤을 Cascading 합니다.
        /// </summary>
        /// <param name="childLookUp">하위 LookUpEditBase 컨트롤</param>
        /// <param name="parentLookUp">상위 LookUpEditBase 컨트롤</param>
        /// <param name="parentKey">상위 LookUpEditBase 컨트롤의 Key Member(Column Name)</param>
        /// <param name="childCascadingMember">하위 LookUpEditBase 컨트롤의 Cacading Member(Column Name)</param>
        public static void SetCascading(this LookUpEditBase childLookUp, LookUpEditBase parentLookUp, string parentKey, string childCascadingMember)
        {
            parentLookUp.Properties.KeyMember = parentKey;
            childLookUp.CascadingOwner = parentLookUp;
            childLookUp.Properties.CascadingMember = childCascadingMember;
        }

        /// <summary>
        /// 조건으로 LookUpEdit, GridLookUpEdit의 DataSource를 Filtering 한다.
        /// </summary>
        /// <param name="lookUpBase">LookUpEdit, GridLookUpEdit 컨트롤</param>
        /// <param name="originalSource">원본 데이터 소스</param>
        /// <param name="filter">필터링 조건 문자열</param>
        /// <param name="defaultValue">기본 값</param>
        /// <param name="hiddenColumns">숨김 컬럼 리스트</param>
        public static void SetCascading(this LookUpEditBase lookUpBase, DataTable originalSource, string filter, object defaultValue, string hiddenColumns = "")
        {
            if (originalSource == null)
                return;

            DataTable filterTable;

            lookUpBase.EditValue = null;
            var selectedRows = originalSource.Select(filter);
            if (selectedRows.Length > 0)
                filterTable = selectedRows.CopyToDataTable();
            else
                filterTable = originalSource.Clone();

            if (lookUpBase.GetType() == typeof(LookUpEdit))
                (lookUpBase as LookUpEdit).SetData(filterTable, hiddenColumns, defaultValue);
            else if (lookUpBase.GetType() == typeof(GridLookUpEdit))
                (lookUpBase as GridLookUpEdit).SetData(filterTable, hiddenColumns, defaultValue);
        }

        /// <summary>
        /// 문자열로 LookUpEdit, GridLookUpEdit의 DataSource를 설정한다.
        /// </summary>
        /// <param name="lookUpBase">LookUpEdit, GridLookUpEdit 컨트롤</param>
        /// <param name="items">코드1:명1|코드2:명2| ... |코드n:명n</param>
        /// <param name="defaultValue">컨트롤의 기본값</param>
        public static void AddItem(this LookUpEditBase lookUpBase, string items, object defaultValue = null)
        {
            if (string.IsNullOrEmpty(items))
                return;

            // 문자열을 분리하여 DataTable로 변환합니다.
            var table = new DataTable();

            table.Columns.Add("ValueColumn", typeof(string));
            table.Columns.Add("DisplayColumn", typeof(string));

            var rows = items.Split('|');

            for (int i = 0; i < rows.Length; i++)
            {
                var cols = rows[i].Split(':');
                table.Rows.Add(cols[0], cols[1]);
            }

            if (lookUpBase.GetType() == typeof(LookUpEdit))
                (lookUpBase as LookUpEdit).SetData(table, "", defaultValue);
            else if (lookUpBase.GetType() == typeof(GridLookUpEdit))
                (lookUpBase as GridLookUpEdit).SetData(table, "", defaultValue);
        }
        #endregion

        #region SearchLookUpEdit
        public static void SetData(this SearchLookUpEdit searchLookUpEdit, DataTable source, string hiddenColumns = "", object defaultValue = null)
        {
            if (source == null)
                return;

            try
            {
                searchLookUpEdit.Properties.DataSource = null;
                searchLookUpEdit.EditValue = null;
                searchLookUpEdit.Properties.DataSource = source;
                searchLookUpEdit.Properties.View.PopulateColumns(source);
                searchLookUpEdit.Properties.View.RefreshData();
                //searchLookUpEdit.Properties.View.OptionsView.ColumnAutoWidth = false;            
                searchLookUpEdit.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
                searchLookUpEdit.Properties.ValueMember = source.Columns[0].ColumnName;
                searchLookUpEdit.Properties.DisplayMember = source.Columns[1].ColumnName;

                // 정렬 및 포맷 설정
                searchLookUpEdit.Properties.View.Columns.ToList().ForEach(column =>
                {
                    column.FilterMode = ColumnFilterMode.DisplayText;
                    column.SortMode = ColumnSortMode.DisplayText;

                    var rows = AppConfig.Glossary.Select($"{keyField} = '{column.FieldName}'");
                    if (rows.Length > 0)
                    {
                        column.Caption = rows[0][captionField].CString();
                        column.AppearanceCell.TextOptions.HAlignment = (HorzAlignment)rows[0][hAlignField].CShort();
                        column.AppearanceCell.TextOptions.VAlignment = (VertAlignment)rows[0][vAlignField].CShort();
                        column.DisplayFormat.FormatType = (FormatType)rows[0][typeField].CShort();
                        column.DisplayFormat.FormatString = rows[0][typeStringField].CString();
                    }
                });

                // 사용여부에 따른 취소선 표시
                searchLookUpEdit.Properties.View.RowStyle += (s, e) =>
                {
                    var view = s as GridView;

                    if (e.RowHandle >= 0 && view.Columns["UseClass"] != null && view.GetRowCellValue(e.RowHandle, "UseClass").CString() == "1")
                    {
                        e.Appearance.FontStyleDelta = FontStyle.Strikeout | FontStyle.Italic;
                        e.Appearance.ForeColor = UserColors.NotUseForeColor;
                    }
                };

                //컬럼 숨김 처리
                if (!string.IsNullOrEmpty(hiddenColumns))
                {
                    hiddenColumns.Split(',').ToList().ForEach(colText =>
                    {
                        if (searchLookUpEdit.Properties.View.Columns[colText.Trim()] != null)
                            searchLookUpEdit.Properties.View.Columns[colText.Trim()].Visible = false;
                    });
                }

                searchLookUpEdit.Properties.View.BestFitColumns();
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }
        #endregion

        #region TextEdit
        /// <summary>
        /// TextEdit의 입력형식을 설정(MaskEdit 化)한다.
        /// </summary>
        /// <param name="textEdit">TextEdit or SpinEdit 컨트롤</param>
        /// <param name="maskType">입력 형식(열거형)</param>
        /// <param name="displayFormat">숫자 입력형식의 추가 형식 문자열</param>
        //public static void SetEditMask(this TextEdit textEdit, MaskType maskType, string displayFormat = "N0")
        public static void SetEditMask(this TextEdit textEdit, MaskType maskType, string displayFormat = "N0")
        {
            if (textEdit.GetType() != typeof(TextEdit) && (textEdit.GetType() != typeof(SpinEdit)))
                return;

            switch (maskType)
            {
                case MaskType.Date:
                    textEdit.Properties.Mask.EditMask = @"[0-9]{4}-[0-9]{2}-[0-9]{2}";
                    textEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Regular;
                    textEdit.Properties.NullValuePrompt = "____-__-__";
                    break;

                case MaskType.DateMinute:
                    textEdit.Properties.Mask.EditMask = @"[0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}";
                    textEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Regular;
                    textEdit.Properties.NullValuePrompt = "____-__-__ __:__";
                    break;

                case MaskType.DateTime:
                    textEdit.Properties.Mask.EditMask = @"[0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}:[0-9]{2}";
                    textEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Regular;
                    textEdit.Properties.NullValuePrompt = "____-__-__ __:__:__";
                    break;

                case MaskType.BizNumber:
                    textEdit.Properties.Mask.EditMask = @"[0-9]{3}-[0-9]{2}-[0-9]{5}";
                    textEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Regular;
                    textEdit.Properties.NullValuePrompt = "___-__-_____";
                    break;

                case MaskType.IDCardNumber:
                    textEdit.Properties.Mask.EditMask = @"[0-9]{6}-[0-9]{7}";
                    textEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Regular;
                    textEdit.Properties.NullValuePrompt = "______-_______";
                    break;

                case MaskType.Numeric:
                    textEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
                    textEdit.Properties.Mask.EditMask = displayFormat;
                    textEdit.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
                    break;

                case MaskType.PhoneFax:
                    textEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
                    textEdit.Properties.Mask.EditMask = @"[0-9]{2,4}-[0-9]{3,4}-[0-9]{4}";
                    textEdit.Properties.NullValuePrompt = "____-____-____";
                    break;

                case MaskType.CellPhone:
                    textEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
                    textEdit.Properties.Mask.EditMask = @"[0-9]{3}-[0-9]{3,4}-[0-9]{4}";
                    textEdit.Properties.NullValuePrompt = "____-____-____";
                    break;

                case MaskType.IP:
                    textEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
                    textEdit.Properties.Mask.EditMask = @"([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}";
                    break;

                case MaskType.RegExpression:
                    textEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
                    textEdit.Properties.Mask.EditMask = displayFormat;  //"[a-zA-Z0-9]{0,5}"  //"[A-Z][0-9][a-z]{0,1}
                    break;
            }

            textEdit.Properties.Mask.SaveLiteral = false;
            textEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            textEdit.Properties.NullValuePromptShowForEmptyValue = true;
        }

        #endregion

        #region ButtonEdit
        public static void SetEditButton(this ButtonEdit buttonEdit)
        {
            // 사용자 정의 버튼 추가(선택, 클리어)
            buttonEdit.Properties.Buttons.Clear();

            // 버튼 추가
            buttonEdit.Properties.Buttons.Add(new EditorButton(ButtonPredefines.Glyph));
            buttonEdit.Properties.Buttons[0].ImageOptions.ImageUri.Uri = "Zoom;Size16x16;Colored";
            buttonEdit.Properties.Buttons[0].AppearanceHovered.FontStyleDelta = FontStyle.Bold;
            buttonEdit.Properties.Buttons[0].AppearanceHovered.ForeColor = Color.Red;
            buttonEdit.Properties.Buttons[0].ImageOptions.ImageToTextAlignment = ImageAlignToText.LeftCenter;
            buttonEdit.Properties.Buttons[0].Tag = "FIND";
            buttonEdit.Properties.Buttons[0].Caption = "";

            buttonEdit.Properties.Buttons.Add(new EditorButton(ButtonPredefines.Glyph));
            buttonEdit.Properties.Buttons[1].ImageOptions.ImageUri.Uri = "Delete;Size16x16;Colored";
            buttonEdit.Properties.Buttons[1].AppearanceHovered.FontStyleDelta = FontStyle.Bold;
            buttonEdit.Properties.Buttons[1].AppearanceHovered.ForeColor = Color.Red;
            buttonEdit.Properties.Buttons[1].ImageOptions.ImageToTextAlignment = ImageAlignToText.LeftCenter;
            buttonEdit.Properties.Buttons[1].Tag = "CLEAR";
            buttonEdit.Properties.Buttons[1].Caption = "";
        }

        #endregion

        #region CheckedComboBoxEdit
        /// <summary>
        /// CheckedComboBoxEdit 컨트롤에 데이터를 표시한다.
        /// </summary>
        /// <param name="checkedComboBoxEdit">CheckedComboBoxEdit Control</param>
        /// <param name="source">DataTable 형식의 데이터 소스</param>
        public static void SetData(this CheckedComboBoxEdit checkedComboBoxEdit, DataTable source)
        {
            checkedComboBoxEdit.Properties.DataSource = null;

            checkedComboBoxEdit.Properties.ValueMember = source.Columns[0].ColumnName;
            checkedComboBoxEdit.Properties.DisplayMember = source.Columns[1].ColumnName;
            checkedComboBoxEdit.Properties.DataSource = source;
            checkedComboBoxEdit.Properties.DropDownRows = 11;   // [ㅁ Select All]을 포함한 최대 표시 행수
        }

        #endregion

        #region SpinEdit
        /// <summary>
        /// SpinEdit의 Mask를 설정한다.
        /// </summary>
        /// <param name="spinEdit">SpinEdit Control</param>
        /// <param name="columnName">대상 컬럼명</param>
        public static void SetMask(this SpinEdit spinEdit, string columnName)
        {
            var rows = AppConfig.Glossary.Select($"{keyField} = '{columnName}'");
            if (rows.Length > 0)
            {
                spinEdit.Properties.Appearance.TextOptions.HAlignment = (HorzAlignment)rows[0][hAlignField].CShort();
                spinEdit.Properties.Appearance.TextOptions.VAlignment = (VertAlignment)rows[0][vAlignField].CShort();
                spinEdit.Properties.DisplayFormat.FormatType = (FormatType)rows[0][typeField].CShort();
                spinEdit.Properties.UseMaskAsDisplayFormat = true;
                spinEdit.Properties.EditMask = rows[0][typeStringField].CString();
            }
        }
        #endregion

        #region DateEdit
        /// <summary>
        /// DateEdit의 표시형식을 정의한다.(월/년, 기본:일)
        /// </summary>
        /// <param name="dateEdit">DateEdit 개체</param>
        /// <param name="editType">MonthView or YearView</param>
        public static void SetViewStyle(this DateEdit dateEdit, ViewStyle viewStyle = ViewStyle.MonthView)
        {
            switch (viewStyle)
            {
                case ViewStyle.MonthView:
                    dateEdit.Properties.Mask.EditMask = "yyyy-MM";
                    dateEdit.Properties.VistaCalendarViewStyle = VistaCalendarViewStyle.YearView;
                    break;

                case ViewStyle.YearView:
                    dateEdit.Properties.Mask.EditMask = "yyyy";
                    dateEdit.Properties.VistaCalendarViewStyle = VistaCalendarViewStyle.YearsGroupView;
                    break;

                default:
                    break;
            }

            dateEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            dateEdit.Properties.VistaDisplayMode = DefaultBoolean.True;
        }

        /// <summary>
        /// DateEdit 컨트롤의 Tag값에 따라 날짜값을 설정한다.
        /// </summary>
        /// <param name="dateEdit"></param>
        public static void SetDefaultDate(this DateEdit dateEdit)
        {
            DateTime? dtValue;
            var tag = dateEdit.Tag.CString();

            try
            {
                if (string.IsNullOrEmpty(tag))
                {
                    dtValue = null;
                }
                else if (tag.CString().ToUpper() == "D" || tag.CString().ToUpper() == "M")
                {
                    dtValue = DateTime.Today;
                }
                else if (tag.Substring(1, 1) == "+" || tag.Substring(1, 1) == "-")
                {
                    int days = int.Parse(tag.Substring(1));
                    switch (tag.Substring(0, 1).ToUpper())
                    {
                        case "D":
                            dtValue = DateTime.Today.AddDays(days);
                            break;
                        case "M":
                            dtValue = DateTime.Today.AddMonths(days);
                            break;
                        default:
                            dtValue = DateTime.Today;
                            break;
                    }
                }
                else
                {
                    switch (tag.ToUpper())
                    {
                        case "MOW": // Monday of week
                            dtValue = DateTime.Today.MondayOfWeek();
                            break;
                        case "SOW": // Sunday of week
                            dtValue = DateTime.Today.SundayOfWeek();
                            break;
                        case "FOM": // First of month
                            dtValue = DateTime.Today.FirstOfMonth();
                            break;
                        case "LOM": // Last of month
                            dtValue = DateTime.Today.LastOfMonth();
                            break;
                        case "MAX": // Max value
                            dtValue = new DateTime(2999, 12, 31);
                            break;
                        default:
                            dtValue = DateTime.Today;
                            break;
                    }
                }
            }
            catch (Exception)
            {
                dtValue = null;
            }

            dateEdit.EditValue = dtValue;
        }

        #endregion

        #region RadioGroup

        /// <summary>
        /// RadioGroup에 항목을 추가한다.
        /// </summary>
        /// <param name="radioGroup">RadioGroup 컨트롤</param>
        /// <param name="groupItems">표시 항목(값1:설명1|값2:설명2|값3:설명3| ... |값n:설명n)</param>
        public static void SetGroupItem(this RadioGroup radioGroup, string groupItems)
        {
            radioGroup.Properties.Items.Clear();

            if (!string.IsNullOrEmpty(groupItems))
            {
                var items = groupItems.Split('|');
                if (items.Length <= 0)
                    return;

                radioGroup.Properties.Columns = items.Length;

                for (int i = 0; i < items.Length; i++)
                {
                    var column = items[i].Split(':');
                    radioGroup.Properties.Items.Add(new RadioGroupItem(column[0].CString().Trim(), column[1].CString().Trim().Localization()));
                }
            }
        }

        #endregion

        #region XtraReport
        /// <summary>
        /// Xtra Report를 미리보기 한다.
        /// </summary>
        /// <param name="report">Xtra Report 개체</param>
        /// <param name="source">데이터 소스</param>
        /// <param name="withRibbon">리본 컨트롤 표시여부</param>
        public static void Preview(this DevExpress.XtraReports.IReport report, DataTable source, bool withRibbon = true)
        {
            var rpt = report as XtraReportBase;
            //rpt.DataSource = null;
            //rpt.DataMember = null;
            rpt.DataSource = source;

            using (ReportPrintTool printTool = new ReportPrintTool(report))
            {
                if (withRibbon)
                    printTool.ShowRibbonPreviewDialog();
                else
                    printTool.ShowPreviewDialog();
            }
        }

        #endregion

        #region ChartControl
        /// <summary>
        /// 차트 컨트롤의 시리즈 설정
        /// </summary>
        /// <param name="series">시리즈</param>
        /// <param name="argumentDataMember">X축 필드명</param>
        /// <param name="valueDataMember">Y축 필드명</param>
        /// <param name="argumentScaleType">X축 유형</param>
        /// <param name="valueScaleType">Y축 범유 유형</param>
        /// <param name="seriesName">축 표시명</param>
        /// <param name="labelVisibility">축 표시 여부</param>
        public static void SetSeries(this Series series, string argumentDataMember, string valueDataMember,
                                     ScaleType argumentScaleType = ScaleType.Auto, ScaleType valueScaleType = ScaleType.Numerical, string seriesName = null, bool labelVisibility = true)
        {
            series.ArgumentDataMember = argumentDataMember;
            series.ValueDataMembers.AddRange(valueDataMember);
            series.ArgumentScaleType = argumentScaleType;
            series.ValueScaleType = valueScaleType;

            // 범례에 표시하기 위한 설정
            if (!string.IsNullOrEmpty(seriesName))
                series.Name = seriesName;

            series.LabelsVisibility = labelVisibility ? DefaultBoolean.True : DefaultBoolean.False;
        }

        /// <summary>
        /// Line 유형의 시리즈 모양 설정
        /// </summary>
        /// <param name="series">시리즈</param>
        /// <param name="kind">마커 종류</param>
        /// <param name="size">마커 크기</param>
        /// <param name="markerVisibility">마크 표시여부</param>
        public static void SetLineViewStyle(this Series series, MarkerKind kind, int size, bool markerVisibility = true)
        {
            LineSeriesView lineSeriesView = series.View as LineSeriesView;

            lineSeriesView.LineMarkerOptions.Kind = kind;
            lineSeriesView.LineMarkerOptions.Size = size;
            lineSeriesView.MarkerVisibility = markerVisibility ? DefaultBoolean.True : DefaultBoolean.False;
        }

        /// <summary>
        /// Bar 유형의 시리즈 모양 설정
        /// </summary>
        /// <param name="series">시리즈</param>
        /// <param name="fillMode">막대 채움 형태</param>
        /// <param name="color">색상</param>
        /// <param name="color2">색상2(FillMode = Gradient 일때)</param>
        public static void SetBarViewStyle(this Series series, FillMode fillMode, Color color, Color color2 = default)
        {
            BarSeriesView barSeriesView = series.View as BarSeriesView;
            barSeriesView.FillStyle.FillMode = fillMode;
            barSeriesView.Color = color;

            if (fillMode == FillMode.Gradient)
                ((GradientFillOptionsBase)barSeriesView.FillStyle.Options).Color2 = color2;
        }

        /// <summary>
        /// 차트의 Crosshiar 표시 여부
        /// </summary>
        /// <param name="chart">차트 컨트롤</param>
        /// <param name="showLine">Crosshair 라인 표시 여부</param>
        /// <param name="showLables">Crosshair 라벨 표시 여부</param>
        public static void SetCrosshair(this ChartControl chart, bool showLine = true, bool showLables = true)
        {
            chart.CrosshairEnabled = DefaultBoolean.True;

            chart.CrosshairOptions.ShowArgumentLine = showLine;// argumentLine;
            chart.CrosshairOptions.ShowCrosshairLabels = showLables;// showLables;
        }

        #endregion

        #region 컨트롤 ReadOnly 설정
        /// <summary>
        /// Edit Control(ButtonEdit,GridLookUpEdit,SearchLookUpEdit,DateEdit)의 ReadOnly 속성을 설정한다.
        /// </summary>
        /// <param name="control">대상 컨트롤</param>
        /// <param name="isReadOnly">ReadOnly 속성 설정/해제</param>
        public static void SetReadOnly(this Control control, bool isReadOnly = true)
        {
            var baseEdit = control as BaseEdit;
            baseEdit.ReadOnly = isReadOnly;
            baseEdit.TabStop = !isReadOnly;

            if (control.GetType() == typeof(ButtonEdit))
            {
                var buttonEdit = control as ButtonEdit;
                buttonEdit.Properties.Buttons.ToList().ForEach(btn => btn.Enabled = !isReadOnly);
            }
            else if (control.GetType() == typeof(GridLookUpEdit))
            {
                var gridLookUpEdit = control as GridLookUpEdit;
                gridLookUpEdit.Properties.Buttons.ToList().ForEach(btn => btn.Enabled = !isReadOnly);
            }
            else if (control.GetType() == typeof(SearchLookUpEdit))
            {
                var searchLookUpEdit = control as SearchLookUpEdit;
                searchLookUpEdit.Properties.Buttons.ToList().ForEach(btn => btn.Enabled = !isReadOnly);
            }
            else if (control.GetType() == typeof(DateEdit))
            {
                var dateEdit = control as DateEdit;
                dateEdit.Properties.Buttons.ToList().ForEach(btn => btn.Enabled = !isReadOnly);
            }
        }
        #endregion

        #region PopupMenu 설정
        /// <summary>
        /// PopUpMenu의 이미지 영역 숨김
        /// </summary>
        /// <param name="popUpMenu"></param>
        public static void SetPopupMenu(this DevExpress.XtraBars.PopupMenu popUpMenu)
        {
            popUpMenu.DrawMenuSideStrip = DefaultBoolean.False;

            // 색상 변경은 Skin으로 처리
            //if (AppConfig.App.SkinType == "D")
            //{
            //    popUpMenu.MenuAppearance.AppearanceMenu.Normal.ForeColor = Color.White;
            //    popUpMenu.MenuAppearance.AppearanceMenu.Normal.BackColor = Color.FromArgb(78, 81, 97);
            //}
            //else
            //{
            //    popUpMenu.MenuAppearance.AppearanceMenu.Normal.ForeColor = Color.Black;
            //    popUpMenu.MenuAppearance.AppearanceMenu.Normal.BackColor = Color.FromArgb(235, 236, 239);
            //}

            //popUpMenu.MenuAppearance.AppearanceMenu.Hovered.BackColor = Color.Silver;
        }

        #endregion

        #region RemoveHTML
        /// <summary>
        /// 문자열에서 HTML 태그를 제거합니다.
        /// </summary>
        /// <param name="htmlString">HTML 문자열</param>
        /// <returns></returns>
        public static string RemoveHTML(this string htmlString)
        {
            string pattern = @"<(.|\n)*?>";
            return Regex.Replace(htmlString, pattern, string.Empty);
        }

        #endregion
    }
}
