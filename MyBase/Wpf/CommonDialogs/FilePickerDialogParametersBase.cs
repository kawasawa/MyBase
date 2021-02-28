namespace MyBase.Wpf.CommonDialogs
{
    /// <summary>
    /// ファイルを選択するダイアログとの間で授受されるパラメータの基底クラスを表します。
    /// </summary>
    public abstract class FilePickerDialogParametersBase : FileDialogParametersBase
    {
        /// <summary>
        /// ダイアログに設定されるフィルター文字列を取得または設定します。
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        /// 既定の拡張子を取得または設定します。
        /// </summary>
        public string DefaultExtension { get; set; }

        /// <summary>
        /// 選択されたフィルター名を取得または設定します。
        /// </summary>
        public string FilterName { get; set; }
    }
}
