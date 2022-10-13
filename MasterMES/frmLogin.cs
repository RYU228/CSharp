using System;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DSLibrary;

namespace MasterMES
{
    //public partial class frmLogin : frmBase
    public partial class frmLogin : XtraForm
    {
        bool bPassChangeMode = false;   // 비밀번호 변경 모드
        int selectedIndex = -1;         // ComboboxEdit의 SelectedIndex 속성값(선택된 언어)

        #region Login 화면 용어 처리
        private readonly string[] LanguageCode = { "KOR", "ENG", "CHN", "JPN" };
        private readonly string[] LanguageName = { "한국어", "English", "中文", "にほんご" };
        private readonly string[] LocalText = { "로컬", "Local", "本地", "ローカル" };
        private readonly string[] RemoteText = { "원격", "Remote", "遠隔", "远程" };
        private readonly string[] IPText = { "IP변경", "Change IP", "IP 更改", "IPの変更" };
        private readonly string[] ApplyText = { "적용", "Apply", "适用", "適用" };
        private readonly string[] LanguageText = { "언어", "LANGUAGE", "语言", "言語" };
        private readonly string[] UserIDText = { "사용자 아이디", "USER ID", "登陆名", "ユーザー" };
        private readonly string[] PasswordText = { "비밀번호", "PASSWORD", "密码", "パスワード" };
        private readonly string[] PasswordChangeText = { "비밀번호변경", "Change PW", "更改密码", "パスワード変更" };
        private readonly string[] NewPasswordText = { "새 비밀번호", "New Password", "新建密码", "新しいパスワード" };
        private readonly string[] CheckPasswordText = { "새 비밀번호(확인)", "Confirm new PWD", "验证新密码", "新しいパスワードの確認" };
        private readonly string[] ChangeText = { "변경", "Change", "变化", "変化する" };
        private readonly string[] CancelText = { "취소", "Cancel", "取消", "取り消す" };
        private readonly string[] CloseText = { "닫기", "Close", "近", "近い" };
        private readonly string[] NotConnectMessage = { "서버에 접속할 수 없습니다.", "Unable to connect to the server.", "无法连接到服务器", "サーバーに接続できません." };
        private readonly string[] IPInputMessage = { "IP를 입력하세요.", "Please enter an IP.", "请输入IP", "IPを入力してください." };
        private readonly string[] IDInputMessage = { "아이디를 입력하세요.", "Please Enter Your ID", "请输入ID", "IDを入力してください" };
        private readonly string[] PasswordInputMessge = { "비밀번호를 입력하세요.", "Please Enter a Password", "请输入密码", "パスワードを入力してください" };
        private readonly string[] NewPasswordInputMesage = { "새 비밀번호를 입력하세요.", "Please enter a new password.", "请输入新密码.", "新しいパスワードを入力してください." };
        private readonly string[] CheckPasswordInputMessage = { "새 비밀번호(확인)를 입력하십시오.", "Please enter a new password (Confirm).", "请输入新密码（确认）", "新しいパスワード（確認）を入力してください" };
        private readonly string[] MissmatchPasswordMessage = { "비밀번호가 맞지 않습니다. 비밀번호를 다시 확인하여 주십시오.", "The password is not correct. Please check your password again.", "密码不正确. 请再次确认密码.", "パスワードが正しくありません. パスワードをもう一度確認してください." };
        private readonly string[] ChangePasswordMessage = { "비밀번호가 변경되었습니다.", "The password has been changed.", "密码已更改.", "パスワードが変更されました." };
        private readonly string[] LatestErrorMessge = { "프로그램이 최신버전이 아닙니다. 프로그램을 다시 실행합니다.", "The program is not up to date. Run the program again.", "プログラムが最新ではない. プログラムを再実行します.", "节目不最新. 重新运行程序." };
        private readonly string[] VersionErrorMessage = { "버전 정보를 찾을 수 없습니다. 프로그램을 다시 실행합니다.", "Version information not found. Run the program again.", "找不到版本信息. 重新运行程序.", "バージョン情報が見つかりません. プログラムを再実行します." };
        private readonly string[] NetworkErrorMessage = { "서버에 접속할 수 없습니다. 네트워크 상태를 확인하세요.", "Unable to connect to the server. Check the network status.", "无法连接到服务器. 检查网络状态", "サーバーに接続できません. ネットワークステータスを確認します" };
        private readonly string[] IPErrorMessage = { "서버에 접속할 수 없습니다. IP 주소를 확인하세요.", "Unable to connect to the server. Please check the IP address.", "无法连接到服务器. 请确认IP地址.", "サーバーに接続できません. IPアドレスを確認してください." };
        private readonly string[] ExpiredPasswordMessage = { "암호 만료기간이 지났습니다. 암호를 변경하십시오.", "Your password has expired. Please change your password.", "密码过期了. 请更改密码.", "パスワードの有効期限が切れました. パスワードを変更してください." };
        private readonly string[] InvalidUserIDMessgae = { "사용자 계정이 잘못되었습니다. 확인하십시오.", "The user account is invalid. Please check.", "用户帐户无效. 请确认一下.", "ユーザーアカウントが無効です. ご確認ください." };
        private readonly string[] InvalidPasswordMessage = { "암호가 잘못되었습니다. 확인하십시오.", "The password is not valid. Please check.", "密码不正确. 请确认一下.", "パスワードが正しくありません. ご確認ください." };
        private readonly string[] LockUserIDMessage = { "사용자 계정이 잠금 상태입니다. 관리자에게 문의하십시오.", "User account is locked. Contact your administrator.", "用户帐户已锁定. 请联系管理员.", "ユーザアカウントがロックされています. 管理者に連絡してください." };

        #endregion

        #region 생성자
        public frmLogin()
        {
            InitializeComponent();

            // 로그인 폼 User 스킨 적용
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.LookAndFeel.SkinName = "Duson Dark";
            this.layoutInput.LookAndFeel.UseDefaultLookAndFeel = false;
            this.layoutInput.LookAndFeel.SkinName = "Duson Dark";

            // ComboboxEdit의 Focus 사각형을 표시 안함
            cmbLanguage.Properties.AllowFocused = false;

            // 언어 설정 : 데이터베이스 미접속시도 표시하기 위해서 강제로 설정
            cmbLanguage.Properties.Items.Clear();
            foreach (string item in LanguageName)
                cmbLanguage.Properties.Items.Add(item);

            //---------------------------------------------------------------------------------------------------------------------
            // DB연결 테스트 시 서버에 접속할 수 없는 경우 딜레이 때문에 폼 Load 전에 세팅함

            // 로그인 정보 레지스트리 불러오기
            if (Utils.GetRegValue("Login", "Mode", "0") == "0")
            {
                ckbLocal.Checked = true;
                this.SetEnabledByConnectMode(false);
            }
            else
            {
                ckbRemote.Checked = true;
                this.SetEnabledByConnectMode(true);
            }

            txtIP.EditValue = Utils.GetRegValue("Login", "IP", Constants.REAL_SERVER);

            // 언어 설정값 읽기
            var selectedIndex = Array.IndexOf(LanguageCode, Utils.GetRegValue("Login", "Language", "KOR"));
            cmbLanguage.SelectedIndex = (selectedIndex == -1) ? 0 : selectedIndex;

            // 개발자 수정모드일 경우 관리자 계정으로 설정, 실행모드일 경우는 Registry에 저장된 계정 설정
            if (AppConfig.IsDebugMode)
            {
                txtUserID.EditValue = "admin";
                txtPassword.EditValue = "1";
            }
            else
            {
                txtUserID.EditValue = Utils.GetRegValue("Login", "UserID", "");
                txtPassword.EditValue = "";  //기본 초기화
            }

            ckbLocal.Click += ConnectModeChange;
            ckbRemote.Click += ConnectModeChange;

            // DB접속 테스트 및 파일 버전 확인
            if (DBConnectTest(ckbRemote.Checked))
                VersionCheck();
            //---------------------------------------------------------------------------------------------------------------------
        }

        #endregion

        #region 오버라이드 이벤트
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            // ID가 입력되어 있으면 Password 입력에, 그렇지 않으면 ID 입력에 포커스를 이동
            if (string.IsNullOrEmpty(txtUserID.EditValue.CString()))
                txtUserID.Focus();
            else
                txtPassword.Focus();

            // Debug Mode 이면 자동으로 로그인 한다.(OK 버튼 클릭)
            if (AppConfig.IsDebugMode)
                btnOk.PerformClick();
        }

        #endregion

        #region CheckButton 변경(로컬/원격)
        /// <summary>
        /// 접속 모드에 따른 컨트롤 활성화/비활성화
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectModeChange(object sender, EventArgs e)
        {
            bool isRemote;
            string server;

            isRemote = !((sender as CheckButton).Name == "ckbLocal");

            // 모드에 따른 IP 입력창 표시 변경
            this.SetEnabledByConnectMode(isRemote);

            // 접속모드 변경시 서버 자동 세팅
            if (isRemote == false)
            {
                if (AppConfig.IsDebugMode)
                    server = Constants.LOCAL_SERVER_DEBUG;
                else
                    server = Constants.LOCAL_SERVER;
            }
            else
            {
                server = Utils.GetRegValue("Login", "IP", "");

                if (server.ToString().Length == 0)
                    server = Constants.REAL_SERVER;

                server += "," + Constants.DB_PORT;
            }

            // Master.Ini SERVER 접속 변경
            Utils.WriteIniValue(Constants.INI_SECTION, Constants.INI_SERVER_KEY, server, Constants.INI_FILENAME);

            // VersionCheck.ini SERVER 접속 변경
            Utils.WriteIniValue(Constants.INI_V_SECTION, Constants.INI_V_SERVER_KEY, server, Constants.INI_V_FILENAME);

            // 레지스트리 저장
            Utils.SetRegValue("Login", "Mode", (isRemote ? "1" : "0"));

            if (string.IsNullOrEmpty(txtUserID.EditValue.CString()))
                txtUserID.Focus();
            else
                txtPassword.Focus();

            // DB접속 테스트 및 파일 버전 확인
            if (DBConnectTest(isRemote, server))
                VersionCheck();
        }

        /// <summary>
        /// 접속 모드에 따른 컨트롤 활성화/비활성화
        /// </summary>
        /// <param name="isServer"></param>
        private void SetEnabledByConnectMode(bool isServer)
        {
            lciIP.ContentVisible = isServer;
            lciIPChange.ContentVisible = isServer;
            lciUnderLineB1.ContentVisible = isServer;
            lciUnderLineR1.ContentVisible = isServer;
        }

        #endregion

        #region 폼 컨트롤 이벤트
        /// <summary>
        /// 폼 키 입력 : Enter(Login), ESC(닫기) 버튼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmLogin_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch ((Keys)e.KeyChar)
            {
                case Keys.Escape:
                    btnCancel.PerformClick();
                    break;

                case Keys.Enter:
                    if (!bPassChangeMode && !string.IsNullOrEmpty(txtUserID.EditValue.CString()) && !string.IsNullOrEmpty(txtPassword.EditValue.CString()))
                        btnOk.PerformClick();
                    break;
            }
        }
        /// <summary>
        ///  언어 선택에 따른 화면 표시 언어
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 선택된 언어의 index => 용어 배열의 index와 일치함
            selectedIndex = cmbLanguage.SelectedIndex;

            ckbLocal.Text = LocalText[selectedIndex];                   // 로컬
            ckbRemote.Text = RemoteText[selectedIndex];                 // 원격
            lblErrMsg.Text = NotConnectMessage[selectedIndex];          // 서버 접속 메세지
            lciIP.Text = IPText[selectedIndex];                         // IP
            btnIPChange.Text = ApplyText[selectedIndex];                // 적용
            lciLanguage.Text = LanguageText[selectedIndex];             // 언어

            //// 비밀번호 변경 모드 또는 로그인 모드에 따라 설정값이 달라짐 (아이디/비밀번호 또는 새 비밀번호/ 새 비밀번호(확인))
            lciUserID.Text = bPassChangeMode ? NewPasswordText[selectedIndex] : UserIDText[selectedIndex];
            lciPassword.Text = bPassChangeMode ? CheckPasswordText[selectedIndex] : PasswordText[selectedIndex];

            btnPassChange.Text = PasswordChangeText[selectedIndex];     // 비밀번호변경
            btnCancel.Text = CloseText[selectedIndex];                  // 닫기
        }

        /// <summary>
        /// 확인 버튼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                // 서버에 접속할 수 없으면 로그인 차단
                if (lblErrMsg.Visible == true)
                {
                    ConnectErrorMessage(ckbRemote.Checked);
                    return;
                }

                // 아이디 입력 여부
                if (string.IsNullOrEmpty(txtUserID.EditValue.CString()))
                {
                    MsgBox.WarningMessage(IDInputMessage[selectedIndex]);
                    txtUserID.Focus();
                    return;
                }

                // 비밀번호 입력 여부
                if (string.IsNullOrEmpty(txtPassword.EditValue.CString()))
                {
                    MsgBox.WarningMessage(PasswordInputMessge[selectedIndex]);
                    txtPassword.Focus();
                    return;
                }

                // 사용자 로그인 체크
                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Exist, SqlDbType.Char);
                inParams.Add("@i_UserID", txtUserID.EditValue.CString(), SqlDbType.VarChar);
                inParams.Add("@i_Password", txtPassword.EditValue.CString(), SqlDbType.VarChar);

                var row = SqlHelper.GetDataRow("xp_Login", inParams, out string retCode, out string retMsg);             

                if (retCode == "00" || retCode == "88")
                {
                    // 사용자 및 로컬 컴퓨터 정보를 전역변수에 저장
                    AppConfig.Login.UserID = txtUserID.EditValue.CString();     // 사용자 계정
                    AppConfig.Login.PersonID = row["PersonID"].CString();       // 계정에 해당하는 사원 ID
                    AppConfig.Login.PersonName = row["PersonName"].CString();   // 사원명
                    AppConfig.Machine.ComputerName = Utils.GetComputerName();   // 컴퓨터 명
                    AppConfig.Machine.LocalIP = Utils.GetLocalIP();             // 컴퓨터 IP
                    AppConfig.Login.Language = LanguageCode[selectedIndex];     // 선택된 언어의 코드[KOR, ENG, CHN, JPN]

                    // 로그인 정보 레지스트리 저장
                    Utils.SetRegValue("Login", "UserID", txtUserID.EditValue.CString());    // 사용자 계정
                    Utils.SetRegValue("Login", "Language", AppConfig.Login.Language);       // 언어

                    // 비밀번호 변경 유효기간 메세지
                    if (retCode == "88")
                        MsgBox.WarningMessage(LoginEerrorMessage(retMsg));

                    // 사용자의 환경설정 정보 읽기
                    inParams = new DbParamList();
                    inParams.Add("@i_ProcessID", ProcessType.GetRow, SqlDbType.Char);
                    inParams.Add("@i_PersonID", AppConfig.Login.PersonID, SqlDbType.VarChar);

                    row = SqlHelper.GetDataRow("xp_Config", inParams, out retCode, out retMsg);

                    if (retCode == "00" && row != null)
                    {
                        // 환경설정 정보 전역변수에 저장
                        AppConfig.App.SkinType = row["SkinType"].CString();             // 스킨 종류(Dark/Light)
                        AppConfig.App.MenuType = row["MenuType"].CString();             // 메뉴 유형(가로/세로)
                        AppConfig.App.AccordionMode = row["AccordionMode"].CString();   // 세로 메뉴 유형(덮어쓰기, 자리차지)
                        AppConfig.App.FontName = row["FontName"].CString();             // 글꼴 명
                        AppConfig.App.FontSize = row["FontSize"].CFloat();              // 글꼴 크기
                    }
                    else
                    {
                        // 최초 설정값이 없으면 기본 값으로 설정
                        AppConfig.App.SkinType = "D";                   // Dark Skin
                        AppConfig.App.MenuType = "H";                   // 가로형
                        AppConfig.App.AccordionMode = "O";              // 덮어쓰기
                        AppConfig.App.FontName = "IBM Plex Sans KR";    // 기본 글꼴
                        AppConfig.App.FontSize = 10.0f;                 // 기본 글꼴 크기
                    }

                    // 시스템 전체 글꼴 변경
                    DevExpress.Utils.AppearanceObject.DefaultFont = new System.Drawing.Font(AppConfig.App.FontName, AppConfig.App.FontSize);

                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    MsgBox.ErrorMessage(LoginEerrorMessage(retMsg));
                }
            }
            catch (Exception ex)
            {
                MsgBox.ErrorMessage(ex.CString());
            }
        }

        /// <summary>
        /// 취소 버튼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (bPassChangeMode == false)
                this.DialogResult = DialogResult.Cancel;    // 로그인 모드
            else
                PassChangeMode(false);                      // 비밀번호 변경 모드
        }

        /// <summary>
        /// 비밀번호 변경 버튼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPassChange_Click(object sender, EventArgs e)
        {
            // 서버에 접속할 수 없으면 비밀번호변경 차단
            if (lblErrMsg.Visible == true)
            {
                ConnectErrorMessage(ckbRemote.Checked);
                return;
            }

            if (bPassChangeMode == false)
            {
                // 로그인 모드 상태에서 "비밀번호 변경" 버튼을 클릭할때 => 로그인 체크 후 정상 로그인이면 비밀번호 변경모드로 변경, 아니면 에러메세지 출력
                if (string.IsNullOrEmpty(txtUserID.EditValue.CString()))
                {
                    MsgBox.WarningMessage(IDInputMessage[selectedIndex]);
                    txtUserID.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(txtPassword.EditValue.CString()))
                {
                    MsgBox.WarningMessage(PasswordInputMessge[selectedIndex]);
                    txtPassword.Focus();
                    return;
                }

                var inParams = new DbParamList();
                inParams.Add("@i_ProcessID", ProcessType.Exist, SqlDbType.Char);
                inParams.Add("@i_UserID", txtUserID.EditValue.CString(), SqlDbType.VarChar);
                inParams.Add("@i_Password", txtPassword.EditValue.CString(), SqlDbType.VarChar);

                var row = SqlHelper.GetDataRow("xp_Login", inParams, out string retCode, out string retMsg);

                if (retCode == "00" || retCode == "88")
                {
                    AppConfig.Login.UserID = txtUserID.EditValue.CString();
                    AppConfig.Login.PersonID = row["PersonID"].CString();

                    PassChangeMode(true);
                }
                else
                {
                    MsgBox.ErrorMessage(LoginEerrorMessage(retMsg));
                }
            }
            else
            {
                // 비밀번호 변경 모드에서 "비밀번호 변경" 버튼을 클릭할 때 => 비밀번호 일치 여부 확인 및 비밀번호 변경 처리
                if (txtUserID.EditValue.CString() == "")
                {
                    MsgBox.WarningMessage(NewPasswordInputMesage[selectedIndex]);
                    txtUserID.Focus();
                    return;
                }

                if (txtPassword.EditValue.CString() == "")
                {
                    MsgBox.WarningMessage(CheckPasswordInputMessage[selectedIndex]);
                    txtPassword.Focus();
                    return;
                }

                if (txtUserID.EditValue.CString() != txtPassword.EditValue.CString())
                {
                    MsgBox.WarningMessage(MissmatchPasswordMessage[selectedIndex]);
                    txtUserID.Focus();
                    return;
                }

                try
                {
                    var inParams = new DbParamList();
                    inParams.Add("@i_ProcessID", ProcessType.Update, SqlDbType.Char);
                    inParams.Add("@i_UserID", AppConfig.Login.UserID, SqlDbType.VarChar);
                    inParams.Add("@i_NewPassword", txtPassword.EditValue.CString(), SqlDbType.VarChar);

                    SqlHelper.Execute("xp_Login", inParams, out string retCode, out string retMsg);

                    if (retCode != "00")
                        throw new Exception(retMsg);

                    MsgBox.OKMessage(ChangePasswordMessage[selectedIndex]);

                    PassChangeMode(false);
                }
                catch (Exception ex)
                {
                    MsgBox.ErrorMessage(ex.CString());
                }
            }
        }

        /// <summary>
        /// 비밀번호 변경 적용
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnIPChange_Click(object sender, EventArgs e)
        {
            string server;

            if (txtIP.EditValue.CString() == "")
            {
                MsgBox.WarningMessage(IPInputMessage[selectedIndex]);
                txtIP.Focus();
                return;
            }

            server = txtIP.EditValue.CString();

            // IP 정보 레지스트리 저장
            Utils.SetRegValue("Login", "IP", server);

            server += "," + Constants.DB_PORT;

            // Master.Ini SERVER 접속 변경
            Utils.WriteIniValue(Constants.INI_SECTION, Constants.INI_SERVER_KEY, server, Constants.INI_FILENAME);

            // VersionCheck.ini SERVER 접속 변경
            Utils.WriteIniValue(Constants.INI_V_SECTION, Constants.INI_V_SERVER_KEY, server, Constants.INI_V_FILENAME);

            // DB접속 테스트 및 파일 버전 확인
            if (DBConnectTest(true, server))
                VersionCheck();
        }

        /// <summary>
        /// 로그인 창 마우스 이동
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmLogin_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Utils.ObjectMouseMove(Handle);
        }

        #endregion

        #region 사용자 정의 함수
        /// <summary>
        /// 비밀번호 변경 모드 전환
        /// </summary>
        /// <param name="bPassChange"></param>
        private void PassChangeMode(bool bPassChange)
        {
            bPassChangeMode = bPassChange;

            if (bPassChange == true)
            {
                // 비밀번호 변경모드
                txtUserID.EditValue = "";
                txtPassword.EditValue = "";
                txtUserID.Properties.PasswordChar = '*';

                lciUserID.Text = NewPasswordText[selectedIndex];
                lciPassword.Text = CheckPasswordText[selectedIndex];
                lciLogIn.ContentVisible = false;
                btnPassChange.Text = ChangeText[selectedIndex];
                btnCancel.Text = CancelText[selectedIndex];

                txtUserID.Focus();
            }
            else
            {
                // 로그인 모드
                txtUserID.EditValue = AppConfig.Login.UserID;
                txtPassword.EditValue = "";
                txtUserID.Properties.PasswordChar = default(char);

                lciUserID.Text = UserIDText[selectedIndex];
                lciPassword.Text = PasswordText[selectedIndex];
                lciLogIn.ContentVisible = true;
                btnPassChange.Text = PasswordChangeText[selectedIndex];
                btnCancel.Text = CloseText[selectedIndex];

                txtPassword.Focus();
            }
        }

        /// <summary>
        /// DB접속 테스트
        /// </summary>
        /// <param name="server"></param>
        private bool DBConnectTest(bool isRemote, string server = "")
        {
            string database;
            bool isconnect = true;

            Cursor.Current = Cursors.WaitCursor;    // Wait 커서 변경

            if (server == "")
                server = Utils.ReadIniValue(Constants.INI_SECTION, Constants.INI_SERVER_KEY, "SERVER", Constants.INI_FILENAME);

            database = Utils.ReadIniValue(Constants.INI_SECTION, Constants.INI_DB_KEY, "MASTERMES", Constants.INI_FILENAME);

            //server = "DSSERVER2\\DS2017";
            //server = "DS-KHS";
            server = "DESKTOP-J8AIK7N\\SQLEXPRESS";
            database = "STI";

            // DB접속 테스트
            SqlHelper.SetConnetionString(server, database, Constants.SQL_ID, Constants.SQL_PASSWORD);

            if (SqlHelper.DBConnectCheck() == false)
            {
                ConnectErrorMessage(isRemote);

                isconnect = false;
                lblErrMsg.Visible = true;
            }
            else
            {
                lblErrMsg.Visible = false;
            }

            Cursor.Current = Cursors.Default;   // Default 커서 변경

            return isconnect;
        }

        /// <summary>
        /// 프로그램 버전 확인
        /// </summary>
        private void VersionCheck()
        {
            if (AppConfig.IsDebugMode)
                return;

            string pggubun;
            int version;

            pggubun = Utils.ReadIniValue(Constants.INI_V_SECTION, Constants.INI_V_PGGUBUN_KEY, "01", Constants.INI_V_FILENAME);
            version = Utils.ReadIniValue(Constants.INI_V_SECTION, Constants.INI_V_VERID_KEY, "0", Constants.INI_V_FILENAME).CInt();

            var inParams = new DbParamList();
            inParams.Add("@i_ProcessID", ProcessType.GetRow, SqlDbType.Char);
            inParams.Add("@i_PGGubun", pggubun, SqlDbType.Char);

            var row = SqlHelper.GetDataRow("xp_VersionCheck", inParams, out string retCode, out _);

            if (retCode == "00" && row != null)
            {
                if (version < row["VerID"].CInt())
                {
                    MsgBox.WarningMessage(LatestErrorMessge[selectedIndex]);

                    Process.Start(Application.StartupPath + "\\VersionCheck.exe");
                    this.DialogResult = DialogResult.Cancel;
                }
            }
            else
            {
                MsgBox.WarningMessage(VersionErrorMessage[selectedIndex]);

                Process.Start(Application.StartupPath + "\\VersionCheck.exe");
                this.DialogResult = DialogResult.Cancel;
            }
        }

        /// <summary>
        /// 서버 접속 오류 메세지 출력
        /// </summary>
        /// <param name="isRemote"></param>
        private void ConnectErrorMessage(bool isRemote)
        {
            if (isRemote == false)
                MsgBox.WarningMessage(NetworkErrorMessage[selectedIndex]);
            else
                MsgBox.WarningMessage(IPErrorMessage[selectedIndex]);
        }

        #endregion

        #region 로그인 실패시 언어별 메세지(case의 상수값은 Stored Procedure에서 반환)
        /// <summary>
        /// 언어에 따른 로그인 실패 메세지 반환
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private string LoginEerrorMessage(string message)
        {
            var returnMessage = string.Empty;

            switch (message.Trim())
            {
                case "MSG_EXPIRED_PWD":
                    returnMessage = ExpiredPasswordMessage[selectedIndex];
                    break;

                case "MSG_INVALID_ID":
                    returnMessage = InvalidUserIDMessgae[selectedIndex];
                    break;

                case "MSG_INVALID_PWD":
                    returnMessage = InvalidPasswordMessage[selectedIndex];
                    break;

                case "MSG_LOCKED_USER":
                    returnMessage = LockUserIDMessage[selectedIndex];
                    break;

                default:
                    returnMessage = message;
                    break;

            }

            return returnMessage;
        }
        #endregion
    }
}