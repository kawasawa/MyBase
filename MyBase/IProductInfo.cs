using System;
using System.Collections.Generic;
using System.Reflection;

namespace MyBase
{
    /// <summary>
    /// プロダクト情報を取得するためのインターフェースを表します。
    /// </summary>
    public interface IProductInfo
    {
        /// <summary>
        /// アセンブリ情報を取得します。
        /// </summary>
        Assembly Assembly { get; }

        /// <summary>
        /// バージョン情報を取得します。
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// タイトルを取得します。
        /// </summary>
        string Title { get; }

        /// <summary>
        /// 説明文を取得します。
        /// </summary>
        string Description { get; }

        /// <summary>
        /// 社名を取得します。
        /// </summary>
        string Company { get; }

        /// <summary>
        /// 製品名を取得します。
        /// </summary>
        string Product { get; }

        /// <summary>
        /// 著作権表示を取得します。
        /// </summary>
        string Copyright { get; }

        /// <summary>
        /// 商標表示を取得します。
        /// </summary>
        string Trademark { get; }

        /// <summary>
        /// 完全パスを取得します。
        /// </summary>
        string Location { get; }

        /// <summary>
        /// 実行フォルダのパスを取得します。
        /// </summary>
        string Working { get; }

        /// <summary>
        /// ユーザ設定フォルダのパスを取得します。
        /// </summary>
        string Roaming { get; }

        /// <summary>
        /// ローカルフォルダのパスを取得します。
        /// </summary>
        string Local { get; }

        /// <summary>
        /// 一時フォルダのパスを取得します。
        /// </summary>
        string Temporary { get; }

        /// <summary>
        /// デバッグビルドによって生成されたことを示す値を取得します。
        /// </summary>
        bool IsDebugBuild { get; }

        /// <summary>
        /// 参照中のアセンブリ情報を取得します。
        /// </summary>
        IEnumerable<AssemblyName> ReferencedAssemblies { get; }
    }
}
