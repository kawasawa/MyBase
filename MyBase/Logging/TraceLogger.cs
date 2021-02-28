using System;
using System.Diagnostics;

namespace MyBase.Logging
{
    /// <summary>
    /// トレースリスナーにメッセージを出力するためのロガーを表します。
    /// </summary>
    public class TraceLogger : ILoggerFacade
    {
        /// <summary>
        /// ログが出力されるときに発生します。
        /// </summary>
        public event EventHandler<LogEventArgs> LogWriting;

        /// <summary>
        /// このクラスの新しいインスタンスを生成します。
        /// </summary>
        public TraceLogger()
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
            var e = new LogEventArgs(category, priority, message);
            this.LogWriting?.Invoke(this, e);
            if (e.Cancel)
                return;

            switch (category)
            {
                case Category.Error:
                    Trace.TraceError(message);
                    break;
                case Category.Warn:
                    Trace.TraceWarning(message);
                    break;
                default:
                    Trace.TraceInformation(message);
                    break;
            }
        }
    }
}