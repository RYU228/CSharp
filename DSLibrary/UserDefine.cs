using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

/// <summary>
/// 사용자 정의 변수/상수 클래스
/// </summary>
namespace DSLibrary
{
    #region 프로그램 환경변수 클래스
    public class AppConfig
    {
        /// <summary>메인 MDI 폼</summary>
        public static Form MainForm;

        /// <summary>용어집 용어 리스트</summary>
        public static DataTable Glossary;

        public static Form ActivateForm;

        /// <summary>프로그램 실행 모드</summary>
        public static bool IsDebugMode;

        /// <summary>로그인</summary>
        public static LoginInformation Login = new LoginInformation();

        /// <summary>로컬 컴퓨터</summary>
        public static MachineInformation Machine = new MachineInformation();

        /// <summary>프로그램 설정</summary>
        public static ApplicationInformation App = new ApplicationInformation();
    }

    /// <summary>
    /// 사용자 로그인정보 저장 클래스
    /// </summary>
    public class LoginInformation
    {
        /// <summary>로그인 사용자 ID</summary>
        public string UserID { get; set; }

        /// <summary>로그인 사용자의 사원 ID </summary>
        public string PersonID { get; set; }

        /// <summary>로그인 사용자 이름</summary>
        public string PersonName { get; set; }

        /// <summary>언어</summary>
        public string Language { get; set; }

        /// <summary>사용자 ID 저장여부(기본 : Yes)</summary>
        public string SaveUserID { get; set; }
    }

    /// <summary>
    /// 로컬 컴퓨터정보 저장 클래스
    /// </summary>
    public class MachineInformation
    {
        /// <summary>로컬 컴퓨터명</summary>
        public string ComputerName { get; set; }

        /// <summary>로컬 컴퓨터 IP</summary>
        public string LocalIP { get; set; }
    }

    /// <summary>
    /// 프로그램 환경설정정보 저장 클래스
    /// </summary>
    public class ApplicationInformation
    {
        /// <summary>스킨 유형</summary>
        public string SkinType { get; set; }

        /// <summary>메인메뉴 표시 유형</summary>
        public string MenuType { get; set; }

        /// <summary>AccordionControl 표시 방법</summary>
        public string AccordionMode { get; set; }

        /// <summary>프로그램 글꼴 이름</summary>
        public string FontName { get; set; }

        /// <summary>프로그램 글꼴 크기</summary>
        public float FontSize { get; set; }
    }

    #endregion

    #region  폼 로그 기록 Tag 클래스
    public class FormLog
    {
        #region 속성
        /// <summary>
        /// MenuID 반환
        /// </summary>
        public string MenuID { get; set; }

        /// <summary>
        /// 시작 시각 반환
        /// </summary>
        public DateTime StartTime { get; set; }
        #endregion

        #region 생성자
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="menuID">메뉴 ID</param>
        /// <param name="startTime">시작 시각</param>
        public FormLog(string menuID, DateTime startTime)
        {
            this.MenuID = menuID;
            this.StartTime = startTime;
        }
        #endregion
    }

    #endregion

    #region 메세지 텍스트 클래스
    public class MessageText
    {
        /// <summary> 정상 입력 </summary>
        public static readonly string OKInsert = "MSG_OK_INSERT".Localization();

        /// <summary> 정상 복사 </summary>
        public static readonly string OKCopy = "MSG_OK_COPY".Localization();

        /// <summary> 정상 수정 </summary>
        public static readonly string OKUpdate = "MSG_OK_UPDATE".Localization();

        /// <summary> 정상 삭제 </summary>
        public static readonly string OKDelete = "MSG_OK_DELETE".Localization();

        /// <summary> 초기화 에러 </summary>
        public static readonly string ErrorInit = "MSG_ERROR_INIT".Localization();

        /// <summary> 조회 에러 </summary>
        public static readonly string ErrorView = "MSG_ERROR_VIEW".Localization();

        /// <summary> 복사 에러 </summary>
        public static readonly string ErrorCopy = "MSG_ERROR_COPY".Localization();

        /// <summary> 입력 에러 </summary>
        public static readonly string ErrorInert = "MSG_ERROR_INSERT".Localization();

        /// <summary> 삭제 에러 </summary>
        public static readonly string ErrorDelete = "MSG_ERROR_DELETE".Localization();

        /// <summary> 출력 에러 </summary>
        public static readonly string ErrorPrint = "MSG_ERROR_PRINT".Localization();

        /// <summary> 입력 확인 </summary>
        public static readonly string QuestionInsert = "MSG_QUESTION_INSERT".Localization();

        /// <summary> 복사 확인 </summary>
        public static readonly string QuestionCopy = "MSG_QUESTION_COPY".Localization();

        /// <summary> 수정 확인 </summary>
        public static readonly string QuestionUpdate = "MSG_QUESTION_UPDATE".Localization();

        /// <summary> 삭제 확인 </summary>
        public static readonly string QuestionDelete = "MSG_QUESTION_DELETE".Localization();

        /// <summary> 그리드 행 삭제 확인 </summary>
        public static readonly string QuestionDeleteRow = "MSG_QUESTION_DELETE_ROW".Localization();

        /// <summary> 파일 다운로드 확인 </summary>
        public static readonly string QuestionDownload = "MSG_QUESTION_DOWNLOAD".Localization();

        /// <summary> 정상 다운로드 </summary>
        public static readonly string OKDownload = "MSG_OK_DOWNLOAD".Localization();

        /// <summary> 삭제 에러 </summary>
        public static readonly string ErrorDownload = "MSG_ERROR_DOWNLOAD".Localization();
    }
    #endregion

    #region 프로시저 처리 유형 클래스
    public class ProcessType
    {
        /// <summary>초기화</summary>
        public static readonly string Initial = "10";

        /// <summary>레코드 존재여부 확인(by PrimaryKey)</summary>
        public static readonly string Exist = "20";

        /// <summary>레코드 입력</summary>
        public static readonly string Insert = "30";

        /// <summary>레코드 복사</summary>
        public static readonly string Copy = "40";

        /// <summary>레코드 수정</summary>
        public static readonly string Update = "50";

        /// <summary>레코드 삽입/수정</summary>
        public static readonly string Merge = "60";

        /// <summary>레코드 삭제</summary>
        public static readonly string Delete = "70";

        /// <summary>GridControl 데이터 조회</summary>
        public static readonly string GetList = "80";

        /// <summary>레코드 행 조회(by Primary key field)</summary>
        public static readonly string GetRow = "90";

        /// <summary>리포트 출력</summary>
        public static readonly string Print = "A0";
    }
    #endregion

    #region 사용자 정의 색상 클래스
    public class UserColors
    {
        /// <summary> 컨트롤 수정시 BackColor </summary>
        public static readonly Color EditableColor = Color.FromArgb(118, 113, 113);

        /// <summary> 컨트롤 ReadOnly BackColor </summary>
        public static readonly Color ReadOnlyBackColor = Color.FromArgb(224, 224, 224);

        /// <summary> 컨트롤 ReadOnly ForeColor </summary>
        public static readonly Color ReadOnlyForeColor = Color.Black;

        /// <summary> 그리드,트리 형식의 컨트롤 라인 색상 </summary>
        public static readonly Color LineColor = Color.FromArgb(100, 100, 100);

        /// <summary> Simple Button ForeColor (when Disabled) </summary>
        //public static readonly Color ButtonDisabled = Color.LightGray;

        /// <summary> Simple Button ForeColor (when Enabled) </summary>
        //public static readonly Color ButtonEnabled = Color.Maroon;

        /// <summary> 미사용 데이터의 ForeColor </summary>
        public static readonly Color NotUseForeColor = Color.Silver;
    }
    #endregion
}
