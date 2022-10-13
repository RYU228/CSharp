using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraSplashScreen;
using Microsoft.Win32;

/// <summary>
/// 다용도(폼, INI, Registy 등) 클래스
/// </summary>
namespace DSLibrary
{
    public class Utils
    {
        #region 객체 마우스 이동 - DLL Import
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        /// <summary>
        /// 넘어온 핸들 값으로 객체를 이동한다.
        /// </summary>
        /// <returns></returns>
        public static void ObjectMouseMove(IntPtr hwnd)
        {
            ReleaseCapture();
            SendMessage(hwnd, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }
        #endregion

        #region INI File 제어 - 경로 및 DLL Import, 읽기/쓰기
        //private static readonly string filePath = Application.StartupPath + "\\AppSettings.ini";

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        /// <summary>
        /// INI File의 지정된 키에 값을 설정한다.
        /// </summary>
        /// <param name="section">Section</param>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public static void WriteIniValue(string section, string key, object value, string filePath)
        {
            WritePrivateProfileString(section, key, value.ToString(), filePath);
        }

        /// <summary>
        /// INI File에서 지정된 키의 값을 반환한다.
        /// </summary>
        /// <param name="section">Section name</param>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Key 값이 미존재시 반환할 기본값</param>
        /// <returns></returns>
        public static string ReadIniValue(string section, string key, string defaultValue, string filePath)
        {
            StringBuilder retValue = new StringBuilder(255);
            GetPrivateProfileString(section, key, defaultValue, retValue, 255, filePath);

            return retValue.ToString();
        }

        #endregion

        #region 폼 열기/닫기
        /// <summary>
        /// MDI 자식 폼을 호출한다.
        /// </summary>
        /// <param name="parentForm">MDI 폼</param>
        /// <param name="childForm">MDI 자식 폼</param>
        public static void ShowMidChildForm(Form parentForm, Form childForm)
        {
            try
            {
                foreach (Form form in Application.OpenForms)
                {
                    if (form.Name == childForm.Name)
                    {
                        form.Activate();
                        return;
                    }
                }

                childForm.MdiParent = parentForm;
                //childForm.Text = $"{childForm.Text.Localization()}"//[{childForm.Name}]";
                var frmName = AppConfig.IsDebugMode ? $"[{childForm.Name}]" : "";
                childForm.Text = $"{childForm.Text.Localization()}{frmName}";
                childForm.Show();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Mdi 자식 폼을 닫는다.
        /// </summary>
        /// <param name="childForm"></param>
        public static void CloseMdiChildForm(Form childForm)
        {
            try
            {
                foreach (Form form in Application.OpenForms)
                {
                    if (form.Name == childForm.Name)
                    {
                        form.Close();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Pop-Up 폼을 호출한다.
        /// </summary>
        /// <param name="popupForm">PopUp 폼</param>
        public static DialogResult ShowPopupForm(Form popupForm)
        {
            popupForm.FormBorderStyle = FormBorderStyle.FixedSingle;
            popupForm.MaximizeBox = false;
            popupForm.MinimizeBox = false;
            popupForm.StartPosition = FormStartPosition.CenterScreen;
            popupForm.Text = $"{popupForm.Text.Localization()}[{popupForm.Name}]";

            return popupForm.ShowDialog();
        }

        #endregion

        #region 어플리케이션 
        /// <summary>
        /// 프로그램 실행경로를 반환한다.
        /// </summary>
        public static string AppPath
        {
            get { return AppDomain.CurrentDomain.BaseDirectory; }
        }

        /// <summary>
        /// 프로그램 실행 여부를 반환한다.
        /// </summary>
        /// <param name="isOnlyProcessName">true:파일명으로 체크, false:경로+파일명으로 체크</param>
        /// <returns></returns>
        public static bool PreInstance(bool isOnlyProcessName = false)
        {
            string processName;
            Process[] processes;

            if (isOnlyProcessName)
            {
                processName = Process.GetCurrentProcess().ProcessName;
                processes = Process.GetProcessesByName(processName);

                return processes.Length > 1;
            }
            else
            {
                processName = Process.GetCurrentProcess().ProcessName;
                processes = Process.GetProcessesByName(processName);

                // 경로 및 파일명이 현재 프로세스와 동일한 수를 카운트한다.
                var count = processes.Count(x => x.MainModule.FileName == Process.GetCurrentProcess().MainModule.FileName);

                return count > 1;
            }
        }

        #endregion

        #region 문자열 암호화/복호화
        /// <summary>
        /// 문자열을 암호화 한다.
        /// </summary>
        /// <param name="value">암호화 대상 문자열</param>
        /// <param name="key">암호화 키</param>
        /// <returns></returns>
        public static string Encrypt(string value, string key = "DSCnATec")
        {
            try
            {
                var encryption = new Crypt(key);
                return encryption.Encrypt(value);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 문자열을 복호화 한다.
        /// </summary>
        /// <param name="value">복호화 대상 문자열</param>
        /// <param name="key">복호화 키(암호화 키와 동일)</param>
        /// <returns></returns>
        public static string Decrypt(string value, string key = "DSCnATec")
        {
            try
            {
                var decryption = new Crypt(key);
                return decryption.Decrypt(value).Replace("\0", "");
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region 레지스트리
        /// <summary>
        /// 레지스트리 키 존재 여부를 반환한다.
        /// </summary>
        /// <param name="subKey">CurrentUser의 하위 레지스트리 키값</param>
        /// <returns></returns>
        public static bool ExistRegKey(string subKey)
        {
            subKey = $"{System.Reflection.Assembly.GetEntryAssembly().GetName().Name}\\{subKey}";
            using (RegistryKey regKey = Registry.CurrentUser.OpenSubKey(subKey))
            {
                return regKey != null;
            }
        }

        /// <summary>
        /// 레지스트리 키에 해당하는 값을 반환한다.
        /// </summary>
        /// <param name="subKey">CurrentUser의 하위 레지스트리 키값</param>
        /// <param name="name">검색할 값의 이름</param>
        /// <param name="defaultValue">name이 존재하지 않을 경우 반환할 기본값</param>
        /// <returns></returns>
        public static string GetRegValue(string subKey, string name, string defaultValue)
        {
            subKey = $"{System.Reflection.Assembly.GetEntryAssembly().GetName().Name}\\{subKey}";
            using (RegistryKey regKey = Registry.CurrentUser.OpenSubKey(subKey))
            {
                return (regKey != null) ?
                        regKey.GetValue(name, defaultValue).ToString().Replace("\0", "") :
                        defaultValue;
            }
        }

        /// <summary>
        /// 레지스트리 키에 해당하는 값을 설정한다.
        /// </summary>
        /// <param name="subKey">CurrentUser의 하위 레지스트리 키값</param>
        /// <param name="name">저장할 값의 이름</param>
        /// <param name="value">저장할 값</param>
        public static void SetRegValue(string subKey, string name, string value)
        {
            subKey = $"{System.Reflection.Assembly.GetEntryAssembly().GetName().Name}\\{subKey}";
            using (RegistryKey regKey = Registry.CurrentUser.CreateSubKey(subKey))
            {
                if (regKey != null)
                    regKey.SetValue(name, value);
            }
        }
        #endregion

        #region Waiting 폼
        /// <summary>
        /// Wainting 폼을 호출한다.
        /// </summary>
        /// <param name="paretnForm">Wait 폼을 표시할 상위 폼 (현재 폼)</param>
        /// <param name="caption">Caption 영역에 표시할 문자열(기본값:데이터 처리중입니다.)</param>
        /// <param name="description">Description영역에 표시할 문자열(기본값:잠시만 기다려 주십시오.</param>
        public static void ShowWaitForm(Form paretnForm, string caption = "MSG_DATA_PROCESSING", string description = "MSG_PLEASE_WAIT")
        {
            try
            {
                SplashScreenManager.CloseForm(false);
                SplashScreenManager.ShowForm(paretnForm, typeof(frmWaitForm), false, false, true, 0);
                SplashScreenManager.Default.SetWaitFormCaption(caption.Localization());
                SplashScreenManager.Default.SetWaitFormDescription(description.Localization());
            }
            catch
            {
                if (!SplashScreenManager.ActivateParentOnSplashFormClosing)
                    SplashScreenManager.CloseForm();
            }
        }

        /// <summary>
        /// Waiting 폼을 닫는다.
        /// </summary>
        public static void CloseWaitForm()
        {
            if (SplashScreenManager.ActivateParentOnSplashFormClosing)
            {
                try
                {
                    SplashScreenManager.CloseForm();
                }
                catch //(System.InvalidOperationException)
                {
                }
            }
        }
        #endregion

        #region 필수 입력 항목 체크
        /// <summary>
        /// 필수 입력 컨트롤에 대해 입력 여부를 체크한다.
        /// </summary>
        /// <param name="fields">Dictionary＜필드명[,길이], 조건문＞</param>
        /// <param name="autoCloseSecond">메세지박스 자동 종료 시간(초)(0:수동 종료)</param>
        /// <returns></returns>
        public static bool CheckField(Dictionary<string, bool> fields, int autoCloseSecond = 0)
        {
            var message = string.Empty;

            try
            {
                foreach (var field in fields)
                {
                    if (field.Value)
                    {
                        if (field.Key.Contains(","))
                        {
                            var key = field.Key.Split(',');
                            message += "\n  ☞  " + $"{key[0].Localization()} [{key[1]}byte(s)]";
                        }
                        else
                        {
                            message += "\n  ☞  " + field.Key.Localization();
                        }
                    }
                }

                if (message != string.Empty)
                {
                    MsgBox.WarningMessage("MSG_CHECK_NEED_FIELD".Localization() + message, "", autoCloseSecond);
                    //MessageBox.Show("MSG_CHECK_NEED_FIELD".Localization() + message, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region 기타 - IP/윈도우계정 등
        /// <summary>
        /// 컴퓨터의 IP를 반환한다.(2개 이상 네트워크 카드 ?)
        /// </summary>
        /// <returns></returns>
        //public static string GetLocalIP()
        //{
        //    try
        //    {
        //        var strHostName = "";
        //        strHostName = Dns.GetHostName();
        //        IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
        //        IPAddress[] addr = ipEntry.AddressList;
        //        return addr[addr.Length - 1].CString();
        //    }
        //    catch (Exception)
        //    {
        //        return "127.0.0.1";
        //    }
        //}

        /// <summary>
        /// 컴퓨터의 IP를 반환한다.
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIP()
        {

            string ipAddress = string.Empty;

            System.Net.IPHostEntry mIPList = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());

            for (int i = 0; i < mIPList.AddressList.Length; i++)
            {
                if (mIPList.AddressList[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    ipAddress = mIPList.AddressList[i].ToString();
                    break;
                }
            }

            return ipAddress;
        }

        /// <summary>
        /// 현재 컴퓨터의 윈도우 도메인\\계정을 반환한다.
        /// </summary>
        /// <returns></returns>
        public static string GetWinAccount()
        {
            return System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        }

        /// <summary>
        /// 현재 컴퓨터의 윈도우 계정명을 반환한다.
        /// </summary>
        /// <returns></returns>
        public static string GetWinAccountName()
        {
            return (System.Security.Principal.WindowsIdentity.GetCurrent().Name).Split('\\')[1];
        }

        /// <summary>
        /// 현재 컴퓨터의 컴퓨터명을 반환한다.
        /// </summary>
        /// <returns></returns>
        public static string GetComputerName()
        {
            return SystemInformation.ComputerName;
        }
        #endregion

        public static string GetMenuID(Form form)
        {
            return ((FormLog)form.Tag).MenuID;
        }
    }
}
