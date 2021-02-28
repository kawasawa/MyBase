using System.Windows.Documents;

namespace MyBase.Wpf.CommonDialogs
{
    /// <summary>
    /// 印刷ダイアログとの間で授受されるパラメータを表します。
    /// </summary>
    public class PrintDialogParameters : ICommonDialogParameters
    {
        /// <summary>
        /// ダイアログのタイトルを取得または設定します。
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// フロードキュメントを取得または設定します。
        /// </summary>
        public FlowDocument FlowDocument { get; set; }

        /// <summary>
        /// ページ範囲を指定できるかどうかを示す値を取得または設定します。
        /// </summary>
        public bool UserPageRangeEnabled { get; set; }

        /// <summary>
        /// 選択されたページを印刷できるかどうかを示す値を取得または設定します。
        /// </summary>
        public bool SelectedPagesEnabled { get; set; }

        /// <summary>
        /// 現在のページを印刷できるかどうかを示す値を取得または設定します。
        /// </summary>
        public bool CurrentPageEnabled { get; set; }

        /// <summary>
        /// 最小のページ番号を取得または設定します。
        /// </summary>
        public uint MinPage { get; set; } = 1U;

        /// <summary>
        /// 最大のページ番号を取得または設定します。
        /// </summary>
        public uint MaxPage { get; set; } = 9999U;
    }
}
