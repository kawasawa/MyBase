using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;

namespace MyBase
{
    /// <summary>
    /// 値の検証とリソースの解放メカニズムを提供するオブジェクトの基底クラスを表します。
    /// </summary>
    public abstract class ValidatableBase : BindableBase, IDisposable, INotifyDataErrorInfo
    {
        private readonly ErrorsContainer<string> _errorsContainer;

        bool INotifyDataErrorInfo.HasErrors => this._errorsContainer.HasErrors;

        /// <summary>
        /// このインスタンスの破棄に合わせて解放されるリソースコンテナを取得します。
        /// </summary>
        protected CompositeDisposable CompositeDisposable { get; } = new CompositeDisposable();

        /// <summary>
        /// リソースが解放されているかどうかを示す値を取得します。
        /// </summary>
        protected bool IsDisposed { get; private set; }

        /// <summary>
        /// リソースが解放されたときに発生します。
        /// </summary>
        public event EventHandler<EventArgs> Disposed;

        /// <summary>
        /// 検証エラーが変更されたときに発生します。
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// このクラスの新しいインスタンスを生成します。
        /// </summary>
        public ValidatableBase()
        {
            this._errorsContainer = new ErrorsContainer<string>(this.OnErrorsChanged);
        }

        /// <summary>
        /// このインスタンスが破棄されるときに呼び出されます。
        /// </summary>
        ~ValidatableBase()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// このインスタンスが保持するリソースを解放します。
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// このインスタンスが保持するリソースを解放します。
        /// </summary>
        /// <param name="disposing">マネージリソースを破棄するかどうかを示す値</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.IsDisposed)
                return;

            if (disposing)
            {
                if (this.CompositeDisposable.IsDisposed == false)
                    this.CompositeDisposable.Dispose();
            }

            this.IsDisposed = true;
            this.OnDisposed(EventArgs.Empty);
        }

        /// <summary>
        /// 指定されたプロパティに値を設定し、変更をリスナーに通知します。
        /// 値に変更があった場合は検証も行います。
        /// </summary>
        /// <typeparam name="T">プロパティの型</typeparam>
        /// <param name="storage">プロパティへの参照</param>
        /// <param name="value">プロパティに設定する値</param>
        /// <param name="propertyName">リスナーに通知されるプロパティの名称</param>
        /// <returns>プロパティに値が設定されたかどうかを示す値</returns>
        protected override bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (base.SetProperty(ref storage, value, propertyName) == false)
                return false;

            this.ValidateProperty(value, propertyName);
            return true;
        }

        /// <summary>
        /// 指定されたプロパティの値を検証し、検証エラーを更新します。
        /// </summary>
        /// <typeparam name="T">プロパティの型</typeparam>
        /// <param name="value">検証する値</param>
        /// <param name="propertyName">エラーコンテナに保持されるプロパティの名称</param>
        public virtual void ValidateProperty<T>(T value, [CallerMemberName] string propertyName = null)
        {
            var errors = new List<ValidationResult>();
            try
            {
                if (Validator.TryValidateProperty(value, new ValidationContext(this) { MemberName = propertyName }, errors))
                {
                    this._errorsContainer.ClearErrors(propertyName);
                    return;
                }
            }
            catch
            {
            }
            this._errorsContainer.SetErrors(propertyName, errors.Select(e => e.ErrorMessage));
        }

        /// <summary>
        /// すべてのパブリックプロパティの値を検証し、検証エラーを更新します。
        /// </summary>
        public virtual void ValidateProperties()
            => this.GetType().GetProperties().Where(p => p.CanRead).ForEach(p => this.ValidateProperty(p.GetValue(this), p.Name));

        /// <summary>
        /// オブジェクトを検証し、検証エラーを更新します。
        /// </summary>
        public virtual void ValidateObject()
        {
            var results = new List<ValidationResult>();
            try
            {
                if (Validator.TryValidateObject(this, new ValidationContext(this), results))
                {
                    this._errorsContainer.ClearErrors();
                    return;
                }
            }
            catch
            {
            }
            results
                .Where(r => r.MemberNames.Any())
                .GroupBy(r => r.MemberNames.First())
                .ForEach(g => this._errorsContainer.SetErrors(g.Key, g.Select(e => e.ErrorMessage)));
        }

        /// <summary>
        /// すべての検証エラーをクリアします。
        /// </summary>
        public void ClearErrors()
            => this._errorsContainer.ClearErrors();

        /// <summary>
        /// 指定された識別子に関連付けられた検証エラーをクリアします。
        /// </summary>
        /// <param name="propertyName">プロパティの名称</param>
        public void ClearErrors(string propertyName)
            => this._errorsContainer.ClearErrors(propertyName);

        /// <summary>
        /// すべての検証エラーを取得します。
        /// </summary>
        /// <returns>検証エラー</returns>
        public Dictionary<string, List<string>> GetErrors()
            => this._errorsContainer.GetErrors();

        /// <summary>
        /// 指定された識別子に関連付けられた検証エラーを取得します。
        /// </summary>
        /// <param name="propertyName">プロパティの名称</param>
        /// <returns>検証エラー</returns>
        public IEnumerable GetErrors(string propertyName)
            => this._errorsContainer.GetErrors(propertyName);

        /// <summary>
        /// 指定された識別子と検証エラーを関連付けて保持します。
        /// </summary>
        /// <param name="propertyName">プロパティの名称</param>
        /// <param name="newValidationResults">検証エラー</param>
        public void SetErrors(string propertyName, IEnumerable<string> newValidationResults)
            => this._errorsContainer.SetErrors(propertyName, newValidationResults);

        /// <summary>
        /// <see cref="Disposed"/> イベントを発生させます。
        /// </summary>
        /// <param name="e">イベントの情報</param>
        protected virtual void OnDisposed(EventArgs e)
            => this.Disposed?.Invoke(this, e);

        /// <summary>
        /// <see cref="ErrorsChanged"/> イベントを発生させます。
        /// </summary>
        /// <param name="propertyName">プロパティの名称</param>
        protected virtual void OnErrorsChanged([CallerMemberName] string propertyName = null)
            => this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }
}
