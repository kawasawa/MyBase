namespace MyBase.Wpf.InteractionRequest
{
    /// <summary>
    /// インタラクションリクエストで授受されるコンテキストのインターフェースを表します。
    /// </summary>
    public interface IConfirmation : INotification
    {
        /// <summary>
        /// 確認されたことを示す値を取得または設定します。
        /// </summary>
        bool? Confirmed { get; set; }
    }
}
