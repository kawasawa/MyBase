namespace MyBase.Wpf.InteractionRequest
{
    /// <summary>
    /// インタラクションリクエストで授受されるコンテキストのインターフェースを表します。
    /// </summary>
    public class Confirmation : Notification, IConfirmation
    {
        /// <summary>
        /// 確認されたことを示す値を取得または設定します。
        /// </summary>
        public bool? Confirmed { get; set; }
    }
}
