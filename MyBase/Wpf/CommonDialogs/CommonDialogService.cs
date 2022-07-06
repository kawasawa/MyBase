using Microsoft.WindowsAPICodePack.Dialogs;
using Prism.Ioc;
using Prism.Services.Dialogs;
using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MyBase.Wpf.CommonDialogs
{
    /// <summary>
    /// コモンダイアログを表示するためのサービスを表します。
    /// </summary>
    public class CommonDialogService : DialogService, ICommonDialogService
    {
        private static Window GetActiveWindow()
            => Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive);

        /// <summary>
        /// このクラスの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="containerExtension">DI コンテナ</param>
        public CommonDialogService(IContainerExtension containerExtension) : base(containerExtension) { }

        /// <summary>
        /// ダイアログを表示します。
        /// </summary>
        /// <param name="parameters">パラメータ</param>
        /// <returns>処理が実行されたかどうかを表す値</returns>
        public virtual bool ShowDialog(ICommonDialogParameters parameters)
            => this.ShowDialog(parameters, null, null);

        /// <summary>
        /// ダイアログを表示します。
        /// </summary>
        /// <param name="parameters">パラメータ</param>
        /// <param name="previewShowDialogCallback">ダイアログが表示される前に実行されるコールバック関数</param>
        /// <param name="dialogClosedCallback">ダイアログが閉じられた後に実行されるコールバック関数</param>
        /// <returns>処理が実行されたかどうかを表す値</returns>
        public virtual bool ShowDialog(ICommonDialogParameters parameters, Action<object, ICommonDialogParameters> previewShowDialogCallback, Action<object, ICommonDialogParameters, bool> dialogClosedCallback)
        {
            switch (parameters)
            {
                case FileDialogParametersBase f:
                    {
                        static CommonFileDialog createDialog(FileDialogParametersBase parameters)
                        {
                            static void createDialogFilter<T>(T dialog, FilePickerDialogParametersBase parameters)
                                where T : CommonFileDialog
                            {
                                const char FILTER_SEPARATOR = '|';
                                const char EXTENSION_PREFIX = '.';

                                var filters = parameters.Filter?.Split(FILTER_SEPARATOR) ?? Array.Empty<string>();
                                if (filters.Length % 2 != 0)
                                    throw new ArgumentException($"拡張子フィルターはファイルの種類と拡張子を \"{FILTER_SEPARATOR}\" で区切って指定します。");
                                foreach (var filter in filters.Buffer(2).Select(f => new CommonFileDialogFilter(f[0], f[1])))
                                    dialog.Filters.Add(filter);

                                dialog.DefaultExtension =
                                    parameters.DefaultExtension?.StartsWith(EXTENSION_PREFIX.ToString()) == true ?
                                    parameters.DefaultExtension.Substring(1) :
                                    parameters.DefaultExtension;
                            }

                            switch (parameters)
                            {
                                case FolderBrowserDialogParameters f:
                                    {
                                        var dialog = new CommonOpenFileDialog
                                        {
                                            Title = f.Title,
                                            InitialDirectory = f.InitialDirectory,
                                            DefaultFileName = f.DefaultFileName,
                                            IsFolderPicker = true
                                        };
                                        return dialog;
                                    }
                                case OpenFileDialogParameters o:
                                    {
                                        var dialog = new CommonOpenFileDialog
                                        {
                                            Title = o.Title,
                                            InitialDirectory = o.InitialDirectory,
                                            DefaultFileName = o.DefaultFileName,
                                            Multiselect = o.Multiselect
                                        };
                                        createDialogFilter(dialog, o);
                                        return dialog;
                                    }
                                case SaveFileDialogParameters s:
                                    {
                                        var dialog = new CommonSaveFileDialog
                                        {
                                            Title = s.Title,
                                            InitialDirectory = s.InitialDirectory,
                                            DefaultFileName = s.DefaultFileName
                                        };
                                        createDialogFilter(dialog, s);
                                        return dialog;
                                    }
                                default:
                                    throw new ArgumentException($"{nameof(parameters)} が不正です。");
                            }
                        }

                        static void populateResultValues<T>(FileDialogParametersBase parameters, T dialog)
                            where T : CommonFileDialog
                        {
                            static string getSelectedFilterName(T dialog)
                            {
                                const string NAME_RAWDISPLAYNAME = "rawDisplayName";

                                if (dialog.Filters.Any())
                                    // HACK: CommonFileDialogFilter に設定されたフィルタ名を取得
                                    // CommonFileDialogFilter.DisplayName ではフィルタ名に拡張子が結合された文字列で取得される。
                                    // フィルタ名だけを取り出したい場合は private メンバの rawDisplayName から取り出す。
                                    return (string)typeof(CommonFileDialogFilter)
                                        .GetField(NAME_RAWDISPLAYNAME, BindingFlags.Instance | BindingFlags.NonPublic)
                                        .GetValue(dialog.Filters[dialog.SelectedFileTypeIndex - 1]);
                                else
                                    return null;
                            }

                            switch (parameters)
                            {
                                case FolderBrowserDialogParameters f:
                                    {
                                        f.FileName = dialog.FileAsShellObject.ParsingName;
                                        break;
                                    }
                                case OpenFileDialogParameters o:
                                    {
                                        if (dialog is CommonOpenFileDialog { Multiselect: true } openFileDialog)
                                            o.FileNames = openFileDialog.FilesAsShellObject.Select(obj => obj.ParsingName);
                                        else
                                            o.FileName = dialog.FileAsShellObject.ParsingName;
                                        o.FilterName = getSelectedFilterName(dialog);
                                        break;
                                    }
                                case SaveFileDialogParameters s:
                                    {
                                        s.FileName = dialog.FileAsShellObject.ParsingName;
                                        s.FilterName = getSelectedFilterName(dialog);
                                        break;
                                    }
                            }
                        }

                        var dialog = createDialog(f);
                        previewShowDialogCallback?.Invoke(dialog, parameters);

                        var dialogResult = dialog.ShowDialog(GetActiveWindow());
                        var result = dialogResult switch { CommonFileDialogResult.Ok => true, _ => false, };
                        if (result)
                            populateResultValues(f, dialog);

                        dialogClosedCallback?.Invoke(dialog, parameters, result);
                        return result;
                    }
                case PrintDialogParameters p:
                    {
                        var dialog = new PrintDialog
                        {
                            UserPageRangeEnabled = p.UserPageRangeEnabled,
                            SelectedPagesEnabled = p.SelectedPagesEnabled,
                            CurrentPageEnabled = p.CurrentPageEnabled,
                            MinPage = p.MinPage,
                            MaxPage = p.MaxPage
                        };
                        previewShowDialogCallback?.Invoke(dialog, parameters);

                        var result = false;
                        var buffer = Application.Current.MainWindow;
                        try
                        {
                            Application.Current.MainWindow = GetActiveWindow();
                            var dialogResult = dialog.ShowDialog();
                            result = dialogResult switch { true => true, _ => false, };
                        }
                        finally
                        {
                            Application.Current.MainWindow = buffer;
                        }
                        if (result)
                        {
                            p.FlowDocument.PageHeight = dialog.PrintableAreaHeight;
                            p.FlowDocument.PageWidth = dialog.PrintableAreaWidth;
                            p.FlowDocument.ColumnWidth = dialog.PrintableAreaWidth;
                            dialog.PrintDocument(((IDocumentPaginatorSource)p.FlowDocument).DocumentPaginator, p.Title);
                        }

                        dialogClosedCallback?.Invoke(dialog, parameters, result);
                        return result;
                    }
            }

            return false;
        }
    }
}
