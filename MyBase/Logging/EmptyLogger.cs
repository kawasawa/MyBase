namespace MyBase.Logging
{
    /// <summary>
    /// 何も処理を行わないロガーを表します。
    /// </summary>
    public class EmptyLogger : ILoggerFacade
    {
        /// <summary>
        /// このクラスの新しいインスタンスを生成します。
        /// </summary>
        public EmptyLogger()
        {
        }

        /// <summary>
        /// (このメソッドは何も処理を行いません。)
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="category">ログの種類</param>
        /// <param name="priority">ログの優先度</param>
        public void Log(string message, Category category, Priority priority)
        {
        }
    }
}
