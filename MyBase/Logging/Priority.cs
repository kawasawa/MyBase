namespace MyBase.Logging
{
    /// <summary>
    /// <see cref="ILoggerFacade"/> で使用されるログの優先度を定義します。
    /// </summary>
    public enum Priority
    {
        /// <summary>
        /// 優先度を指定しません。
        /// </summary>
        None,

        /// <summary>
        /// 低い優先度を表します。
        /// </summary>
        Low,

        /// <summary>
        /// 通常の優先度を表します。
        /// </summary>
        Medium,

        /// <summary>
        /// 高い優先度を表します。
        /// </summary>
        High,
    }
}
