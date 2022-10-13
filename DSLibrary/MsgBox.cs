using System;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;

/// <summary>
/// 메세지박스 관련 클래스
/// </summary>
namespace DSLibrary
{
    #region 열거형 선언
    /// <summary>
    /// MessageBox 형식
    /// </summary>
    public enum MessageType
    {
        /// <summary>[OK][Information]</summary>
        Information,

        /// <summary>[OK][Warning]</summary>
        Warning,

        /// <summary>[OK][Exclamation]</summary>
        Exclamation,

        /// <summary>[OK][Error]</summary>
        Error,

        /// <summary>[YesNo][Question][Button1]</summary>
        QuestionD1,

        /// <summary>[YesNo][Question][Button2]</summary>
        QuestionD2,

        /// <summary>[YesNoCancel][Question][Button3]</summary>
        YesNoCancel,

        /// <summary>[OKCancel][Question][Button2]</summary>
        OKCancel,
    };
    #endregion

    public class MsgBox
    {
        private float textFontSize = 0;
        private string textFontFamilyName;
        private string buttonFontFamilyName;

        public bool InvokeRequired { get; private set; }

        delegate DialogResult MessageBoxShowCallback(string text, string caption, DialogResult[] buttons, MessageBoxIcon icon, float textFontSize, float buttonFontSize, int delaySecond = 0, int defaultButtonIndex = 1);      //델리게이트 선언 

        #region 특정 형식의 메세지 박스
        /// <summary>
        /// 정상 처리 메세지를 표시한다.
        /// </summary>
        /// <param name="message">메세지</param>
        /// <param name="caption">메세지창 제목(기본값:Application.ProductName)</param>
        /// <param name="delaySecond">자동 종료시간(초)</param>
        /// <returns></returns>
        public static DialogResult OKMessage(string message, string caption = "", int delaySecond = 0)
        {
            return Show(message, caption, MessageType.Information, delaySecond);
        }

        /// <summary>
        /// 경고 메세지를 표시한다.
        /// </summary>
        /// <param name="message">메세지</param>
        /// <param name="caption">메세지창 제목(기본값:Application.ProductName)</param>
        /// <param name="delaySecond">자동 종료시간(초)</param>
        public static DialogResult WarningMessage(string message, string caption = "", int delaySecond = 0)
        {
            return Show(message, caption, MessageType.Warning, delaySecond);
        }

        /// <summary>
        /// 에러 메세지를 표시한다.
        /// </summary>
        /// <param name="message">메세지</param>
        /// <param name="caption">메세지창 제목(기본값:Application.ProductName)</param>
        /// <param name="delaySecond">자동 종료시간(초)</param>
        public static DialogResult ErrorMessage(string message, string caption = "", int delaySecond = 0)
        {
            return Show(message, caption, MessageType.Exclamation, delaySecond);
        }

        /// <summary>
        /// 사용자 확인(질문) 메세지를 표시한다.
        /// </summary>
        /// <param name="message">메세지</param>
        /// <param name="caption">메세지창 제목(기본값:Application.ProductName)</param>
        /// <param name="delaySecond">자동 종료시간(초)</param>
        public static DialogResult QuestionMessage(string message, string caption = "", int delaySecond = 0)
        {
            return Show(message, caption, MessageType.QuestionD1, delaySecond);
        }

        #endregion

        #region 기본 메세지박스
        /// <summary>
        /// 메세지박스 타입에 따른 메세지박스를 호출한다.
        /// </summary>
        /// <param name="message">메세지</param>
        /// <param name="caption">메세지박스 제목(기본값:Application.ProductName)</param>
        /// <param name="type">메세지박스 유형</param>
        /// <param name="delaySecond">자동 종료 시간(초)</param>
        /// <returns></returns>
        public static DialogResult Show(string message, string caption = "", MessageType type = MessageType.Information, int delaySecond = 0)
        {
            Utils.CloseWaitForm();                      ///추가 : 메세지 박스 뜨기전 Splash화면 종료

            // MessageType.Information 기본값
            var buttons = new DialogResult[] { DialogResult.OK };
            var icon = MessageBoxIcon.Information;
            var defautButton = 0;
            var title = (caption == "") ? Application.ProductName : caption;

            switch (type)
            {
                case MessageType.Warning:
                    icon = MessageBoxIcon.Warning;
                    break;

                case MessageType.Exclamation:
                    icon = MessageBoxIcon.Exclamation;
                    break;

                case MessageType.Error:
                    icon = MessageBoxIcon.Error;
                    break;

                case MessageType.QuestionD1:
                    buttons = new DialogResult[] { DialogResult.Yes, DialogResult.No };
                    icon = MessageBoxIcon.Question;
                    break;

                case MessageType.QuestionD2:
                    buttons = new DialogResult[] { DialogResult.Yes, DialogResult.No };
                    icon = MessageBoxIcon.Question;
                    defautButton = 1;
                    break;

                case MessageType.YesNoCancel:
                    buttons = new DialogResult[] { DialogResult.Yes, DialogResult.No, DialogResult.Cancel };
                    icon = MessageBoxIcon.Question;
                    defautButton = 2;
                    break;

                case MessageType.OKCancel:
                    buttons = new DialogResult[] { DialogResult.OK, DialogResult.Cancel };
                    icon = MessageBoxIcon.Question;
                    defautButton = 1;
                    break;
            }

            return (new MsgBox()).MessageBoxShow(message, title, buttons, icon, 10.0f, 10.0f, delaySecond, defautButton);
        }
        #endregion

        #region 메세지 박스 생성을 위한 소스
        /// <summary>
        /// 메세지박스 생성
        /// </summary>
        /// <param name="text">내용</param>
        /// <param name="caption">타이틀</param>
        /// <param name="buttons">출력버튼</param>
        /// <param name="icon">아이콘 종류</param>
        /// <param name="textFontSize">글자크기</param>
        /// <param name="buttonFontSize">버튼 글자크기</param>
        /// <param name="delaySecond">자동닫기 초</param>
        /// <param name="defaultButtonIndex">선택 버튼 인덱스 설정</param>
        /// <returns></returns>
        private DialogResult MessageBoxShow(string text, string caption, DialogResult[] buttons, MessageBoxIcon icon, float textFontSize, float buttonFontSize, int delaySecond, int defaultButtonIndex)
        {
            if (this.InvokeRequired) //Invoke 메서드
            {
                MessageBoxShowCallback d = new MessageBoxShowCallback(MessageBoxShow);
                DialogResult dr = (DialogResult)this.Invoke(d, new object[] { text, caption, buttons, icon, textFontSize, buttonFontSize, delaySecond, defaultButtonIndex });
                return dr;
            }
            else
            {
                return Show(null, text, caption, buttons, GetSystemIconForMessageBox(icon), textFontSize, null, 0, delaySecond, defaultButtonIndex);
            }
        }

        private DialogResult Invoke(MessageBoxShowCallback d, object[] vs)
        {
            throw new NotImplementedException();
        }

        private DialogResult Show(IWin32Window owner, string text, string caption, DialogResult[] buttons, Icon icon,
                                  float textFontSize, string textFontFamilyName,
                                  int defaultButton, int delaySecond, int defaultButtonIndex)
        {
            this.textFontFamilyName = string.IsNullOrEmpty(textFontFamilyName) ? "IBM Plex Sans KR" : textFontFamilyName;
            this.buttonFontFamilyName = string.IsNullOrEmpty(this.buttonFontFamilyName) ? "IBM Plex Sans KR" : this.buttonFontFamilyName;

            this.textFontSize = textFontSize > 0 ? textFontSize : 11;
            XtraMessageBoxArgs messageBoxArgs = new XtraMessageBoxArgs(owner, text, caption, buttons, icon, defaultButton);

            if (delaySecond > 0)
            {
                messageBoxArgs.Text += string.Format($"\r\n\r\n({"MSG_AUTO_CLOSE".Localization()})", delaySecond); // MSG_AUTO_CLOSE : {0}초 후에 자동으로 닫힙니다.
                messageBoxArgs.AutoCloseOptions.Delay = delaySecond * 1000;
                messageBoxArgs.AutoCloseOptions.ShowTimerOnDefaultButton = true;
            }

            messageBoxArgs.DefaultButtonIndex = defaultButtonIndex;
            messageBoxArgs.Showing += MessageBoxArgsOnShowing;

            using (XtraMessageBoxForm form = new XtraMessageBoxForm())
            {
                form.Appearance.Font = new Font(new FontFamily(this.textFontFamilyName), this.textFontSize);
                form.AutoSize = true;
                form.MaximumSize = new Size(480, 450);
                form.ControlBox = false;

                return form.ShowMessageBoxDialog(messageBoxArgs);
            }
        }

        private void MessageBoxArgsOnShowing(object sender, XtraMessageShowingArgs e)
        {
            e.Form.Appearance.Font = new Font("IBM Plex Sans KR", 10f);

            foreach (var control in e.Form.Controls)
            {
                if (control is SimpleButton button)
                {
                    button.Font = new Font("IBM Plex Sans KR", 10f);
                    button.Size = new Size(110, 36);

                    button.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
                    //button.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;

                    button.Appearance.Options.UseForeColor = true;
                    //button.Appearance.ForeColor = Color.White;
                    button.Appearance.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.ControlText;

                    button.AppearanceDisabled.Options.UseForeColor = true;
                    button.AppearanceDisabled.ForeColor = Color.Silver;

                    button.AppearanceHovered.Options.UseForeColor = true;
                    button.AppearanceHovered.ForeColor = Color.FromArgb(237, 125, 49);
                    button.AppearanceHovered.FontStyleDelta = FontStyle.Bold;
                    button.AppearanceHovered.BorderColor = Color.FromArgb(78, 81, 97);

                    button.AppearancePressed.Options.UseForeColor = true;
                    button.AppearancePressed.ForeColor = Color.FromArgb(187, 100, 4);
                    button.AppearancePressed.BorderColor = Color.FromArgb(78, 81, 97);
                    button.AppearancePressed.BackColor = Color.FromArgb(78, 81, 97);

                    button.Text = button.DialogResult.ToString().Localization();
                }
            }

            e.Form.AutoScaleMode = AutoScaleMode.Font;
            e.Form.PerformAutoScale();
        }

        public Icon GetSystemIconForMessageBox(MessageBoxIcon messageBoxIcon)
        {
            switch (messageBoxIcon)
            {
                case MessageBoxIcon.None:
                    return null;

                case MessageBoxIcon.Hand:
                    return SystemIcons.Hand;

                case MessageBoxIcon.Question:
                    return SystemIcons.Question;

                case MessageBoxIcon.Exclamation:
                    return SystemIcons.Exclamation;

                case MessageBoxIcon.Information:
                    return SystemIcons.Information;

                default:
                    return null;
            }
        }
        #endregion
    }
}
