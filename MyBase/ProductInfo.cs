using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MyBase
{
    /// <summary>
    /// プロダクト情報を取得するためのサービスを表します。
    /// </summary>
    public class ProductInfo : IProductInfo
    {
        #region 遅延初期化変数

        private readonly Lazy<Assembly> _lazyAssembly;
        private readonly Lazy<string> _lazyTitle;
        private readonly Lazy<string> _lazyDescription;
        private readonly Lazy<string> _lazyCompany;
        private readonly Lazy<string> _lazyProduct;
        private readonly Lazy<string> _lazyCopyritht;
        private readonly Lazy<string> _lazyTrademark;
        private readonly Lazy<bool> _lazyIsDebugBuild;

        #endregion

        #region プロパティ

        private readonly Assembly _specifiedAssembly;

        /// <summary>
        /// アセンブリ情報を取得します。
        /// </summary>
        public Assembly Assembly => this._specifiedAssembly ?? this._lazyAssembly.Value;

        /// <summary>
        /// バージョン情報を取得します。
        /// </summary>
        public Version Version => this._lazyAssembly.Value?.GetName()?.Version;

        /// <summary>
        /// タイトルを取得します。
        /// </summary>
        public string Title => this._lazyTitle.Value;

        /// <summary>
        /// 説明文を取得します。
        /// </summary>
        public string Description => this._lazyDescription.Value;

        /// <summary>
        /// 社名を取得します。
        /// </summary>
        public string Company => this._lazyCompany.Value;

        /// <summary>
        /// 製品名を取得します。
        /// </summary>
        public string Product => this._lazyProduct.Value;

        /// <summary>
        /// 著作権表示を取得します。
        /// </summary>
        public string Copyright => this._lazyCopyritht.Value;

        /// <summary>
        /// 商標表示を取得します。
        /// </summary>
        public string Trademark => this._lazyTrademark.Value;

        /// <summary>
        /// 完全パスを取得します。
        /// </summary>
        public string Location => this._lazyAssembly.Value?.Location;

        /// <summary>
        /// 実行フォルダのパスを取得します。
        /// </summary>
        public string Working => Path.GetDirectoryName(this.Location);

        /// <summary>
        /// ユーザ設定フォルダのパスを取得します。
        /// </summary>
        public string Roaming => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), this.Product);

        /// <summary>
        /// ローカルフォルダのパスを取得します。
        /// </summary>
        public string Local => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), this.Product);

        /// <summary>
        /// 一時フォルダのパスを取得します。
        /// </summary>
        public string Temporary => Path.Combine(Path.GetTempPath(), this.Product);

        /// <summary>
        /// デバッグビルドによって生成されたことを示す値を取得します。
        /// </summary>
        public bool IsDebugBuild => this._lazyIsDebugBuild.Value;

        /// <summary>
        /// 参照中のアセンブリ情報を取得します。
        /// </summary>
        public IEnumerable<AssemblyName> ReferencedAssemblies => this.Assembly?.GetReferencedAssemblies() ?? Enumerable.Empty<AssemblyName>();

        #endregion

        /// <summary>
        /// このクラスの新しいインスタンスを生成します。
        /// </summary>
        public ProductInfo()
        {
            this._lazyAssembly = new Lazy<Assembly>(() => Assembly.GetEntryAssembly());
            this._lazyTitle = new Lazy<string>(() => ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(this.Assembly, typeof(AssemblyTitleAttribute)))?.Title);
            this._lazyDescription = new Lazy<string>(() => ((AssemblyDescriptionAttribute)Attribute.GetCustomAttribute(this.Assembly, typeof(AssemblyDescriptionAttribute)))?.Description);
            this._lazyCompany = new Lazy<string>(() => ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(this.Assembly, typeof(AssemblyCompanyAttribute)))?.Company);
            this._lazyProduct = new Lazy<string>(() => ((AssemblyProductAttribute)Attribute.GetCustomAttribute(this.Assembly, typeof(AssemblyProductAttribute)))?.Product);
            this._lazyCopyritht = new Lazy<string>(() => ((AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(this.Assembly, typeof(AssemblyCopyrightAttribute)))?.Copyright);
            this._lazyTrademark = new Lazy<string>(() => ((AssemblyTrademarkAttribute)Attribute.GetCustomAttribute(this.Assembly, typeof(AssemblyTrademarkAttribute)))?.Trademark);
            this._lazyIsDebugBuild = new Lazy<bool>(() => (Attribute.GetCustomAttribute(this.Assembly, typeof(DebuggableAttribute)) as DebuggableAttribute)?.IsJITTrackingEnabled ?? false);
        }

        /// <summary>
        /// アセンブリ情報を指定してインスタンスを生成します。
        /// </summary>
        /// <param name="specifiedAssembly">アセンブリ情報</param>
        public ProductInfo(Assembly specifiedAssembly)
            : this()
        {
            this._specifiedAssembly = specifiedAssembly;
        }

        /// <summary>
        /// 現在のオブジェクトを表す文字列を返します。
        /// </summary>
        /// <returns>現在のオブジェクトを表す文字列</returns>
        public override string ToString()
        {
            return string.Join(", ", this
                .GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.CanRead)
                .Select(p =>
                {
                    var value = p.GetValue(this, null);
                    return value switch
                    {
                        IEnumerable<object> array => $"{p.Name}: [{string.Join(", ", array.Select((o, i) => $"[{i}]: {o}"))}]",
                        _ => $"{p.Name}: {value}",
                    };
                }));
        }
    }
}
