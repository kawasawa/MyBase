namespace MyBase.Wpf.CommonDialogs
{
    /// <summary>
    /// ファイルダイアログとの間で授受されるパラメータの基底クラスを表します。
    /// </summary>
    public abstract class FileDialogParametersBase : ICommonDialogParameters
    {
        /// <summary>
        /// ダイアログのタイトルを取得または設定します。
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// ダイアログの起動時に表示されるフォルダのパスを取得または設定します。
        /// </summary>
        public string InitialDirectory { get; set; }

        /// <summary>
        /// 既定のファイルの完全パスを取得または設定します。
        /// </summary>
        public string DefaultFileName { get; set; }

        /// <summary>
        /// 選択されたファイルの完全パスを取得または設定します。
        /// </summary>
        public string FileName { get; set; }
    }
}
