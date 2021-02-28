#define DEBUG
using System;
using System.Diagnostics;
using System.Globalization;

namespace MyBase.Logging
{
    /// <summary>
    /// デバッグリスナーにメッセージを出力するためのロガーを表します。
    /// </summary>
    public class DebugLogger : ILoggerFacade
    {
        /// <summary>
        /// ログに出力されるメッセージのフォーマットを取得または設定します。
        /// </summary>
        public string Format { get; set; } = "{0}: {1}. Priority: {2}. Timestamp:{3:u}.";

        /// <summary>
        /// ログが出力されるときに発生します。
        /// </summary>
        public event EventHandler<LogEventArgs> LogWriting;

        /// <summary>
        /// このクラスの新しいインスタンスを生成します。
        /// </summary>
        public DebugLogger()
        {
        }

        /// <summary>
        /// ログを出力します。
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="category">ログの種類</param>
        /// <param name="priority">ログの優先度</param>
        public void Log(string message, Category category, Priority priority)
        {
            var messageToLog = string.Format(
                CultureInfo.InvariantCulture,
                this.Format,
                category.ToString().ToUpper(CultureInfo.InvariantCulture),
                message,
                priority,
                DateTime.Now);

            var e = new LogEventArgs(category, priority, messageToLog);
            this.LogWriting?.Invoke(this, e);
            if (e.Cancel)
                return;

            Debug.WriteLine(messageToLog);
        }
    }
}