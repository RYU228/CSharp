using System;
using System.IO;

/// <summary>
/// 로그 기록(App, 폼, Action) 클래스
/// </summary>
namespace DSLibrary
{
    #region 열거형 선언
    /// <summary>
    /// 파일로그 기록시 폴더 및 파일 생성 형식
    /// </summary>
    public enum FileLogType
    {
        /// <summary>日 기준</summary>
        Daily,
        /// <summary>月 기준</summary>
        Monthly
    }

    #endregion
    public class LogWriter : Utils
    {
        private string path;

        #region 생성자

        /// <summary>
        /// 생성자 : path, logType, prefix, suffix
        /// </summary>
        /// <param name="path">로그 파일 저장 경로</param>
        /// <param name="logType">Daily or Monthly</param>
        /// <param name="prefix">파일명 접두사</param>
        /// <param name="suffix">파일명 접미사</param>
        public LogWriter(string path, FileLogType logType, string prefix, string suffix)
        {
            this.path = path;
            SetLogPath(logType, prefix, suffix);
        }

        /// <summary>
        /// 생성자 : [AppPath\Log], [Daily], prefix, suffix
        /// </summary>
        /// <param name="prefix">파일명 접두사</param>
        /// <param name="suffix">파일명 접미사</param>
        public LogWriter(string prefix, string suffix)
            : this(Path.Combine(AppPath, "Log"), FileLogType.Daily, prefix, suffix)
        {
        }

        /// <summary>
        /// 생성자 : [AppPath\Log], [Daily], [null], [null]
        /// </summary>
        public LogWriter()
            : this(Path.Combine(AppPath, "Log"), FileLogType.Daily, null, null)
        {
        }
        #endregion

        #region 메서드
        /// <summary>
        /// 로그를 기록할 경로 및 파일을 생성한다.
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="prefix"></param>
        /// <param name="suffix"></param>
        private void SetLogPath(FileLogType logType, string prefix, string suffix)
        {
            var path = string.Empty;
            var name = string.Empty;

            switch (logType)
            {
                case FileLogType.Daily:
                    path = string.Format(@"{0}", $"{DateTime.Now.Year}{DateTime.Now:MM}");
                    name = DateTime.Now.ToString("yyyyMMdd");
                    break;

                case FileLogType.Monthly:
                    path = string.Format(@"{0}\", DateTime.Now.Year);
                    name = DateTime.Now.ToString("yyyyMM");
                    break;
            }

            this.path = Path.Combine(this.path, path);
            if (!Directory.Exists(this.path))
                Directory.CreateDirectory(this.path);

            if (!string.IsNullOrEmpty(prefix))
                name = prefix + name;

            if (!string.IsNullOrEmpty(suffix))
                name += suffix;

            name += ".txt";

            this.path = Path.Combine(this.path, name);
        }

        /// <summary>
        /// 로그파일에 데이터를 기록한다.
        /// </summary>
        /// <param name="data">로그 파일에 기록할 내용</param>
        public void Write(string data)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(this.path, true))
                {
                    writer.Write(data);
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 로그파일에 데이터를 기록한다.
        /// </summary>
        /// <param name="data">로그 파일에 기록할 내용</param>
        public void WriteLine(string data)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(this.path, true))
                {
                    writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss\t") + data);
                }
            }
            catch (Exception) { }
        }
        #endregion
    }
}
