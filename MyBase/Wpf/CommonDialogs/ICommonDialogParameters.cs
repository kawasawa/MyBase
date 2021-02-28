namespace MyBase.Wpf.CommonDialogs
{
    /// <summary>
    /// コモンダイアログとの間で授受されるパラメータのインターフェースを表します。
    /// </summary>
    public interface ICommonDialogParameters
    {
        /// <summary>
        /// ダイアログのタイトルを取得または設定します。
        /// </summary>
        string Title { get; set; }
    }
}