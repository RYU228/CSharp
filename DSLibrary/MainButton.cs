using System.Linq;
using DevExpress.XtraBars.Ribbon;

/// <summary>
/// 메인 화면 공용버튼 제어 클래스
/// </summary>
namespace DSLibrary
{
    public class MainButton
    {
        public static RibbonControl MainRibbon { get; set; }

        #region 리본 버튼 초기 설정
        /// <summary>
        /// 리본 버튼 전체를 비활성화 후 지정된 버튼만 활성화한다.
        /// </summary>
        /// <param name="buttons">활성화 할 버튼 키값(INIT,NEW,SAVE,COPY,DELETE,VIEW,PRINT) - 순서 무관, 생락:전체 비활성</param>
        public static void ActiveButtons(string buttons = "")
        {
            MainRibbon.Items.ToList().ForEach(buttonItem =>
            {
                if (buttonItem.GetType() == typeof(DevExpress.XtraBars.BarButtonItem))
                    buttonItem.Enabled = new string[] { "bbiClose" }.Contains(buttonItem.Name);

                if (!string.IsNullOrEmpty(buttons))
                {
                    buttons.Split(new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(button =>
                    {
                        var btnName = $"bbi{button.First().ToString().ToUpper()}{button.Substring(1).ToLower()}";
                        if (MainRibbon.Items[btnName] != null)
                            MainRibbon.Items[btnName].Enabled = true;
                    });
                }
            });
        }

        #endregion

        #region 개별 리본 버튼 활성화/비활성화 설정
        /// <summary>
        /// 초기화 버튼
        /// </summary>
        public static bool InitButton
        {
            get { return MainRibbon.Items["bbiInit"].Enabled; }
            set { MainRibbon.Items["bbiInit"].Enabled = value; }
        }

        /// <summary>
        /// 조회 버튼
        /// </summary>
        public static bool ViewButton
        {
            get { return MainRibbon.Items["bbiView"].Enabled; }
            set { MainRibbon.Items["bbiView"].Enabled = value; }
        }

        /// <summary>
        /// 신규 버튼
        /// </summary>
        public static bool NewButton
        {
            get { return MainRibbon.Items["bbiNew"].Enabled; }
            set { MainRibbon.Items["bbiNew"].Enabled = value; }
        }

        /// <summary>
        /// 복사 버튼
        /// </summary>
        public static bool CopyButton
        {
            get { return MainRibbon.Items["bbiCopy"].Enabled; }
            set { MainRibbon.Items["bbiCopy"].Enabled = value; }
        }

        /// <summary>
        /// 저장 버튼
        /// </summary>
        public static bool SaveButton
        {
            get { return MainRibbon.Items["bbiSave"].Enabled; }
            set { MainRibbon.Items["bbiSave"].Enabled = value; }
        }

        /// <summary>
        /// 삭제 버튼
        /// </summary>
        public static bool DeleteButton
        {
            get { return MainRibbon.Items["bbiDelete"].Enabled; }
            set { MainRibbon.Items["bbiDelete"].Enabled = value; }
        }

        /// <summary>
        /// 출력 버튼
        /// </summary>
        public static bool PrintButton
        {
            get { return MainRibbon.Items["bbiPrint"].Enabled; }
            set { MainRibbon.Items["bbiPrint"].Enabled = value; }
        }

        #endregion

        #region SimpleButton으로 처리할 경우 소스
        //private static Dictionary<string, SimpleButton> mainButtons;

        //#region 메서드
        /////// <summary>
        /////// 메인화면에서 사용할 SimpleButton(공용 버튼)을 설정한다.
        /////// </summary>
        /////// <param name="buttons">Dictionary＜키값,버튼 개체＞</param>
        ////public static void SetButtonControls(Dictionary<string, SimpleButton> buttons)
        ////{
        ////    if (buttons == null)
        ////        return;

        ////    mainButtons = new Dictionary<string, SimpleButton>();

        ////    foreach (KeyValuePair<string, SimpleButton> item in buttons)
        ////    {
        ////        mainButtons.Add(item.Key.ToUpper(), item.Value);
        ////        mainButtons[item.Key.ToUpper()].Text = mainButtons[item.Key.ToUpper()].Text.Localization();
        ////        //_mainButtons[item.Key.ToUpper()].ImageOptions.Image = null;
        ////        //_mainButtons[item.Key.ToUpper()].Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
        ////        //_mainButtons[item.Key.ToUpper()].Appearance.ForeColor = System.Drawing.Color.Maroon;
        ////    }
        ////}

        ///// <summary>
        ///// 메인화면에서 사용할 SimpleButton(공용 버튼)을 설정한다.
        ///// </summary>
        ///// <param name="buttons"></param>
        //public static void SetButtonControls(params SimpleButton[] buttons)
        //{
        //    if (buttons == null)
        //        return;

        //    mainButtons = new Dictionary<string, SimpleButton>();

        //    // 지역화는 DXControl에서
        //    for (int i = 0; i < buttons.Count(); i++)
        //        mainButtons.Add(buttons[i].Name.Replace("btn", "").ToUpper(), buttons[i]);
        //}

        ///// <summary>
        ///// 지정된 버튼을 활성화한다.
        ///// </summary>
        ///// <param name="buttons">활성화 할 버튼 키값(INIT,NEW,SAVE,COPY,DELETE,VIEW,PRINT) - 순서 무관, 생락:전체 비활성</param>
        //public static void ActiveButton(string buttons = "")
        //{
        //    // 모든 버튼을 비활성화 합니다.
        //    foreach (KeyValuePair<string, SimpleButton> item in mainButtons)
        //    {
        //        if (item.Key != "CLOSE")
        //        {
        //            mainButtons[item.Key].Enabled = false;
        //            //mainButtons[item.Key].Appearance.ForeColor = UserColors.ButtonDisabled;
        //        }
        //    }

        //    if (string.IsNullOrEmpty(buttons))
        //        return;

        //    buttons.CString().Split(',').ToList().ForEach(key =>
        //    {
        //        if (mainButtons.ContainsKey(key.ToUpper()))
        //        {
        //            mainButtons[key.ToUpper()].Enabled = true;
        //            // _mainButtons[key.ToUpper()].Appearance.ForeColor = UserColors.ButtonEnabled;
        //        }
        //    });
        //}
        //#endregion

        //#region 속성
        ///// <summary>
        ///// 초기화 버튼 활성화 설정/반환
        ///// </summary>
        //public static bool InitButton
        //{
        //    get { return mainButtons["INIT"].Enabled; }
        //    set { mainButtons["INIT"].Enabled = value; }
        //}

        ///// <summary>
        ///// 조회 버튼 활성화 설정/반환
        ///// </summary>
        //public static bool ViewButton
        //{
        //    get { return mainButtons["VIEW"].Enabled; }
        //    set { mainButtons["VIEW"].Enabled = value; }
        //}

        ///// <summary>
        ///// 신규 버튼 활성화 설정/반환
        ///// </summary>
        //public static bool NewButton
        //{
        //    get { return mainButtons["NEW"].Enabled; }
        //    set { mainButtons["NEW"].Enabled = value; }
        //}

        ///// <summary>
        ///// 복사 버튼 활성화 설정/반환
        ///// </summary>
        //public static bool CopyButton
        //{
        //    get { return mainButtons["COPY"].Enabled; }
        //    set { mainButtons["COPY"].Enabled = value; }
        //}

        ///// <summary>
        ///// 저장 버튼 활성화 설정/반환
        ///// </summary>
        //public static bool SaveButton
        //{
        //    get { return mainButtons["SAVE"].Enabled; }
        //    set { mainButtons["SAVE"].Enabled = value; }
        //}

        ///// <summary>
        ///// 삭제 버튼 활성화 설정/반환
        ///// </summary>
        //public static bool DeleteButton
        //{
        //    get { return mainButtons["DELETE"].Enabled; }
        //    set { mainButtons["DELETE"].Enabled = value; }
        //}

        ///// <summary>
        ///// 출력 버튼 활성화 설정/반환
        ///// </summary>
        //public static bool PrintButton
        //{
        //    get { return mainButtons["PRINT"].Enabled; }
        //    set { mainButtons["PRINT"].Enabled = value; }
        //}

        ///// <summary>
        ///// 닫기 버튼 활성화 설정/반환
        ///// </summary>
        //public static bool CloseButton
        //{
        //    get { return mainButtons["CLOSE"].Enabled; }
        //    set { mainButtons["CLOSE"].Enabled = value; }
        //}
        //#endregion

        #endregion
    }
}
