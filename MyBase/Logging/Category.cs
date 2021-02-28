namespace MyBase.Logging
{
    /// <summary>
    /// <see cref="ILoggerFacade"/> で使用されるログの種類を定義します。
    /// </summary>
    public enum Category
    {
        /// <summary>
        /// トレースを表します。
        /// </summary>
        Trace,

        /// <summary>
        /// デバッグを表します。
        /// </summary>
        Debug,

        /// <summary>
        /// 情報を表します。
        /// </summary>
        Info,

        /// <summary>
        /// 警告を表します。
        /// </summary>
        Warn,

        /// <summary>
        /// エラーを表します。
        /// </summary>
        Error,

        /// <summary>
        /// 深刻なエラーを表します。
        /// </summary>
        Fatal,
    }
}
