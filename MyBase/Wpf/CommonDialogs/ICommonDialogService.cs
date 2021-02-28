using System;

namespace MyBase.Wpf.CommonDialogs
{
    /// <summary>
    /// コモンダイアログを表示するためのインターフェースを表します。
    /// </summary>
    public interface ICommonDialogService
    {
        /// <summary>
        /// ダイアログを表示します。
        /// </summary>
        /// <param name="parameters">パラメータ</param>
        /// <returns>処理が実行されたかどうかを表す値</returns>
        bool ShowDialog(ICommonDialogParameters parameters);

        /// <summary>
        /// ダイアログを表示します。
        /// </summary>
        /// <param name="parameters">パラメータ</param>
        /// <param name="previewShowDialogCallback">ダイアログが表示される前に実行されるコールバック関数</param>
        /// <param name="dialogClosedCallback">ダイアログが閉じられた後に実行されるコールバック関数</param>
        /// <returns>処理が実行されたかどうかを表す値</returns>
        bool ShowDialog(ICommonDialogParameters parameters, Action<object, ICommonDialogParameters> previewShowDialogCallback, Action<object, ICommonDialogParameters, bool> dialogClosedCallback);
    }
}
