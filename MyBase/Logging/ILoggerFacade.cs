namespace MyBase.Logging
{
    /// <summary>
    /// ロガーファサードを表します。
    /// </summary>
    public interface ILoggerFacade
    {
        /// <summary>
        /// ログを出力します。
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="category">ログの種類</param>
        /// <param name="priority">ログの優先度</param>
        void Log(string message, Category category, Priority priority);
    }
}