using System;
using System.Globalization;
using System.IO;

namespace MyBase.Logging
{
    /// <summary>
    /// テキストライターにメッセージを出力するためのロガーを表します。
    /// </summary>
    public class TextLogger : ILoggerFacade, IDisposable
    {
        /// <summary>
        /// テキストライターを取得または設定します。
        /// </summary>
        public TextWriter Writer { get; set; } = Console.Out;

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
        public TextLogger()
        {
        }

        /// <summary>
        /// このインスタンスが破棄されるときに呼び出されます。
        /// </summary>
        ~TextLogger()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// このインスタンスが保持するリソースを解放します。
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// このインスタンスが保持するリソースを解放します。
        /// </summary>
        /// <param name="disposing">マネージリソースを破棄するかどうかを示す値</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                this.Writer?.Dispose();
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

            this.Writer.WriteLine(messageToLog);
        }
    }
}
