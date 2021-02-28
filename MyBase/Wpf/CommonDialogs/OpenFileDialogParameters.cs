using System.Collections.Generic;

namespace MyBase.Wpf.CommonDialogs
{
    /// <summary>
    /// ファイルを開くダイアログとの間で授受されるパラメータを表します。
    /// </summary>
    public class OpenFileDialogParameters : FilePickerDialogParametersBase
    {
        /// <summary>
        /// 選択された複数のファイルの完全パスを取得または設定します。
        /// </summary>
        public IEnumerable<string> FileNames { get; set; }

        /// <summary>
        /// 複数のファイルを選択できるかどうかを示す値を取得または設定します。
        /// </summary>
        public bool Multiselect { get; set; }
    }
}
