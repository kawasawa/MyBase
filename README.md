# MyBase

[NuGet](https://www.nuget.org/packages/MyBase)  
[GitHib](https://github.com/kawasawa/MyBase)  

## 概要

MyBase は .NET Framework, .NET Core による WPF アプリケーションの開発をサポートするためのライブラリです。  
Prism の標準機能を拡張するための少数のクラスから構成されています。  

| 対象のフレームワーク | バージョンの下限値 |
|----------------------|--------------------|
| .NET Framework       | 4.8                |
| .NET Core            | 3.1                |
| .NET                 | 5.0                |

## 使用例

### ProductInfo - プロダクト情報

ProductInfo クラスを利用することでエントリーポイントとなる実行ファイルのプロダクト情報を取得することができます。このクラスは IProductInfo インターフェースを実装しており、他のサービスと同様にシングルトンとして DI コンテナに登録しておくと良いでしょう。  

```cs
using MyBase;
using Prism.Ioc;
using Prism.Unity;

namespace TestApp
{
    public partial class App : PrismApplication
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // シングルトンの登録
            containerRegistry.RegisterSingleton<IProductInfo, ProductInfo>();
        }
    }
}
```

ViewModel 層では DI フレームワークからインスタンスを注入し、View 層でこれをバインドします。  

```cs
using MyBase;
using Unity;

namespace TestApp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        // インスタンスが注入されるプロパティ
        [Dependency]
        public IProductInfo ProductInfo { get; set; }
    }
}
```

```xml
<Window x:Class="TestApp.Views.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Grid>
        <!-- プロパティがバインドされるコントロール -->
        <TextBlock Text="{Binding ProductInfo.Product, Mode=OneWay}"/>
    </Grid>
</Window>
```

ProductInfo クラスは内部で遅延初期化される Assembly を保持しており、これをもとにプロダクト情報を取得しています。この Assembly の既定値は Assembly.GetEntryAssembly() と等価ですが、インスタンスの生成時にこの値を変更することもできます。  

```cs
    new ProductInfo(Assembly.GetExecutingAssembly());
```

### ValidatableBase - 変更通知オブジェクト

Prism が提供する変更通知処理が搭載された BindableBase クラスを継承し、IDisposable, INotifyDataErrorInfo を実装した ValidatableBase があります。このクラスから派生し Model 層、ViewModel 層を実装することができます。  

```cs
using MyBase;

namespace TestApp.ViewModels
{
    public abstract class ViewModelBase : ValidatableBase
    {
    }

    public class MainWindowViewModel : ViewModelBase
    {
        private string _text;

        // 変更通知プロパティ
        public string Text
        {
            get => this._text;
            set => this.SetProperty(ref this._text, value);
        }
    }
}
```

ValidatableBase は、同じく Prism の ErrorsContainer を内包しており、妥当性検証が必要なオブジェクトの基底クラスとして利用できます。例えば以下のように、コンストラクタで ValidateProperties メソッドを呼び出すことで、インスタンスの生成と同時にプロパティの検証を行い、エラー情報を付与することができます。

```cs
using System.ComponentModel.DataAnnotations;

namespace TestApp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string _text;

        // 検証属性が付与されたプロパティ
        [Required]
        public string Text
        {
            get => this._text;
            set => this.SetProperty(ref this._text, value);
        }

        public MainWindowViewModel()
        {
            this.ValidateProperties();
        }
    }
}
```

また、リソースを一括で開放できる System.Reactive の CompositeDisposable がプロパティとして定義されています。ReactiveProperty 等を利用する際、定義されたプロパティの開放に使います。  

```cs
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace TestApp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        // ReactiveProperty の宣言
        public ReactiveProperty<bool> IsWorking { get; }

        public MainWindowViewModel()
        {
            // ReactiveProperty の初期化と CompositeDisposable の紐づけ
            this.IsWorking = new ReactiveProperty<bool>().AddTo(this.CompositeDisposable);
        }
    }
}
```

### ILoggerFacade - ロガーファサード

Prism 8.0 で廃止された ILoggerFacade インターフェース、Category, Priority を模した列挙値を提供します。  
いくつかのロガーも移植されており、これらによって Prism のかつてのロギングと同等の実装が可能になります。  

```cs
    this.Logger = new DebugLogger();
    this.Logger.Log("メッセージ", Category.Debug, Priority.None);
```

### CompositeLogger - 複合ロガー

CompositeLogger は他のロガーを複数内包できるコンテナです。このクラス自身も ILoggerFacade を実装しており、実行される Log メソッドは内包されたすべてのロガーに伝播します。また同インターフェースにより DI コンテナへの登録も可能です。  

```cs
    this.Logger = new CompositeLogger(new DebugLogger(), new TraceLogger());
    this.Logger.Log("メッセージ", Category.Debug, Priority.None);
```

```cs
using MyBase;
using Prism.Ioc;
using Prism.Unity;

namespace TestApp
{
    public partial class App : PrismApplication
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // インスタンスの登録
            containerRegistry.RegisterInstance(new CompositeLogger(new DebugLogger(), new TraceLogger()));
        }
    }
}
```

### CommonDialogService - コモンダイアログの表示

以前の Prism では InteractionRequest とそれに対応する Trigger による相互作用でダイアログを表示する方法が一般的でした。これが Prism 7.2 以降のバージョンでは新たに IDialogService インターフェースが追加され、こちらを使用する方法が推奨されるようになりました。  
この方式を踏襲し、本ライブラリではコモンダイアログの表示に特化した ICommonDialogService を提供しています。処理で授受される ICommonDialogParameters インターフェースを実装したいくつかのクラスが定義されています。現状では以下の機能が実装されています。  

| パラメータ                    | 主な機能                       |
|-------------------------------|--------------------------------|
| SaveFileDialogParameters      | 保存先となるファイルパスの指定 |
| OpenFileDialogParameters      | 読込元となるファイルパスの指定 |
| FolderBrowserDialogParameters | フォルダーパスの指定           |
| PrintDialogParameters         | フロードキュメントの印刷       |

これらを使い ViewModel を起点に OpenFileDialog を表示する場合は以下のようになります。  

```cs
using MyBase.Wpf;

public class MainWindowViewModel : ViewModelBase
{
    /// <summary>
    /// ファイルを選択するダイアログを表示します。
    /// </summary>
    public void ShowOpenFileDialog()
    {
        var parameters = new OpenFileDialogParameters()
            {
                InitialDirectory = root,
                Filter = "すべてのファイル|*.*|テキストファイル|*.txt",
                DefaultExtension = ".txt",
                Multiselect = true,
            };
        var result = this.CommonDialogService.ShowDialog(parameters);
        if (!result)
            return;
        var fileName = parameters.FileName;
    }
}
```

単純な表示だけなく、ダイアログを表示する前、閉じられた後に呼び出されるコールバックメソッドを引数で指定することもできます。以下は、OpenFileDialog に文字コードの選択欄を追加し、ユーザによって選択された文字コードを取得する例です。ダイアログの装飾には Windows API CodePack を使用しています。  

```cs
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.WindowsAPICodePack.Dialogs.Controls;
using MyBase.Wpf;
using System.Linq;
using System.Text;

public class MainWindowViewModel : ViewModelBase
{
    /// <summary>
    /// ファイルを選択するダイアログを表示します。
    /// </summary>
    public void ShowOpenFileDialog()
    {
        IEnumerable<string> fileNames = null;
        string filter = null;
        Encoding encoding = null;
        this.CommonDialogService.ShowDialog(
            new OpenFileDialogParameters()
            {
                InitialDirectory = root,
                Filter = "すべてのファイル|*.*|テキストファイル|*.txt",
                DefaultExtension = ".txt",
                Multiselect = true,
            },

            // ダイアログを表示する前に呼び出されるコールバックメソッド
            (dialog, parameters) =>
            {
                // 文字コードの選択欄を追加する
                var d = (CommonFileDialog)dialog;
                var encodingComboBox = this.CreateEncodingComboBox(Encoding.UTF8);
                var encodingGroupBox = new CommonFileDialogGroupBox($"文字コード(&E):");
                encodingGroupBox.Items.Add(encodingComboBox);
                d.Controls.Add(encodingGroupBox);
            },

            // ダイアログが閉じられた後に呼び出されるコールバックメソッド
            (dialog, parameters, result) =>
            {
                if (result == false)
                    return;

                var d = (CommonFileDialog)dialog;
                var p = (OpenFileDialogParameters)parameters;
                fileNames = p.FileNames;
                filter = p.FilterName;

                // 選択された文字コードを取得する
                var encodingGroupBox = (CommonFileDialogGroupBox)d.Controls.First();
                var encodingComboBox = (CommonFileDialogComboBox)encodingGroupBox.Items.First();
                encoding = ((EncodingComboBoxItem)encodingComboBox.Items[encodingComboBox.SelectedIndex]).Encoding;
            });

        var result = this.CommonDialogService.ShowDialog(parameters);
        if (!result)
            return;
    }

    /// <summary>
    /// 文字コード選択用のコンボボックスを構築します。
    /// </summary>
    /// <param name="defaultEncoding">既定の文字コード</param>
    /// <returns>文字コード選択用のコンボボックス</returns>
    public static CommonFileDialogComboBox CreateEncodingComboBox(Encoding defaultEncoding)
    {
        var comboBox = new CommonFileDialogComboBox();
        var encodings = Constants.ENCODINGS;
        for (var i = 0; i < encodings.Count(); i++)
        {
            comboBox.Items.Add(new EncodingComboBoxItem(encodings.ElementAt(i)));
            if (encodings.ElementAt(i) == defaultEncoding)
                comboBox.SelectedIndex = i;
        }
        return comboBox;
    }

    /// <summary>
    /// 文字コードを格納するコンボボックスアイテムを表します。
    /// </summary>
    private class EncodingComboBoxItem : CommonFileDialogComboBoxItem
    {
        public Encoding Encoding { get; }

        public EncodingComboBoxItem(Encoding encoding)
            : this(encoding, encoding?.EncodingName)
        {
        }

        public EncodingComboBoxItem(Encoding encoding, string text)
            : base(text)
        {
            this.Encoding = encoding;
        }
    }
}
```
