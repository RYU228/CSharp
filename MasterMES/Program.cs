using System;
using System.Data;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DSLibrary;

namespace MasterMES
{
    static class Constants
    {
        // DB 접속 관련
        public const string LOCAL_SERVER_DEBUG = "DSSERVER2\\DS2017";   // 로컬 접속 - 디버그용
        public const string LOCAL_SERVER = "SERVER";                    // 로컬 접속
        public const string REAL_SERVER = "192.0.0.1";                  // 원격 접속 (실제 서버 REAL IP)
        public const string DB_PORT = "9874";                           // SQL 포트
        public const string SQL_ID = "Duson";                           // SQL 접속 계정
        public const string SQL_PASSWORD = "Duson6988#";                // SQL 접속 암호

        // INI 관련 (Main)
        public static readonly string INI_FILENAME = Environment.GetFolderPath(Environment.SpecialFolder.Windows) + "\\Master.ini";
        public const string INI_SECTION = "SQLSERVER";
        public const string INI_SERVER_KEY = "SERVER";
        public const string INI_DB_KEY = "DATABASE";
        public const string INI_LANGUAGE_KEY = "LANGUAGE";

        // INI 관련 (VersionCheck)
        public static readonly string INI_V_FILENAME = Application.StartupPath + "\\VersionCheck.ini";
        public const string INI_V_SECTION = "SERVER";
        public const string INI_V_SERVER_KEY = "SERVER";
        public const string INI_V_DB_KEY = "DB";
        public const string INI_V_PGGUBUN_KEY = "PGGUBUN";
        public const string INI_V_VERID_KEY = "VERID";
    }

    static class Program
    {
        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length > 0)
                AppConfig.IsDebugMode = (args[0] == "DEBUG_MODE");

            if (Utils.PreInstance())
            {
                MsgBox.WarningMessage("프로그램이 이미 실행중입니다.");
                Application.Exit();
                return;
            }

            Assembly asm = typeof(DevExpress.UserSkins.DusonSkinProject).Assembly;
            DevExpress.Skins.SkinManager.Default.RegisterAssembly(asm);
            //UserLookAndFeel.Default.SkinName = "DusonSkin";
            //UserLookAndFeel.Default.SkinName = "Duson Dark";

            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ko");
            DevExpress.XtraEditors.Controls.Localizer.Active = DevExpress.XtraEditors.Controls.Localizer.CreateDefaultLocalizer();

            Application.Run(new frmMain());
        }


        /// <summary>
        /// 용어집 정보를 읽는다.
        /// </summary>
        public static void GetGlossaryInfo()
        {
            try
            {
                // Registry에 설정된 언어 코드를 읽는다.
                var lnaguage = Utils.GetRegValue("Login", "Language", "KOR");

                // 언어 코드에 해당하는 용어집 정보를 조회한다.
                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", "81", SqlDbType.Char);
                inParams.Add("@i_LanguageCode", lnaguage, SqlDbType.VarChar);

                // 용어 검색에 사용될 DataSource
                AppConfig.Glossary = SqlHelper.GetDataTable("xp_Glossary", inParams, out string retCode, out string retMsg);

                if (retCode != "00")
                    throw new Exception(retMsg);

                // DataTable Primary Key 설정(효과 ?)
                DataColumn[] pk = new DataColumn[1];
                pk[0] = AppConfig.Glossary.Columns[0];
                AppConfig.Glossary.PrimaryKey = pk;
                AppConfig.Glossary.AcceptChanges();
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }
    }
}
