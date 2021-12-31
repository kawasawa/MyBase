using System;

namespace MyBase.Logging
{
    /// <summary>
    /// ログイベントの情報を表します。
    /// </summary>
    public class LogEventArgs : EventArgs
    {
        /// <summary>
        /// ログの種類を取得または設定します。
        /// </summary>
        public Category Category { get; set; }

        /// <summary>
        /// ログの優先度を取得または設定します。
        /// </summary>
        public Priority Priority { get; set; }

        /// <summary>
        /// ログ情報を取得または設定します。
        /// </summary>
        public object LogInfo { get; set; }

        /// <summary>
        /// イベントをキャンセルする必要があるかどうかを示す値を取得または設定します。
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// このクラスの新しいインスタンスを生成します。
        /// </summary>
        public LogEventArgs()
        {
        }

        /// <summary>
        /// このクラスの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="category">ログの種類</param>
        /// <param name="priority">ログの優先度</param>
        /// <param name="logInfo">ログの情報</param>
        public LogEventArgs(Category category, Priority priority, object logInfo)
            : this()
        {
            this.Category = category;
            this.Priority = priority;
            this.LogInfo = logInfo;
            this.Cancel = false;
        }
    }
}