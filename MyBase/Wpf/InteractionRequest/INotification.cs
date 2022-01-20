namespace MyBase.Wpf.InteractionRequest
{
    /// <summary>
    /// インタラクションリクエストで授受されるコンテキストのインターフェースを表します。
    /// </summary>
    public interface INotification
    {
        /// <summary>
        /// 通知に使用されるタイトルを取得または設定します。
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// 通知の内容を取得または設定します。
        /// </summary>
        object Content { get; set; }
    }
}
