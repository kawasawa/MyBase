using Microsoft.Xaml.Behaviors;
using System.Windows;

namespace MyBase.Wpf.InteractionRequest
{
    /// <summary>
    /// インタラクションリクエストに対して実行されるアクションの基底クラスを表します。
    /// </summary>
    /// <typeparam name="T">依存関係オブジェクト</typeparam>
    public abstract class InteractionRequestAction<T> : TriggerAction<T>
        where T : DependencyObject
    {
        /// <summary>
        /// アタッチする関連オブジェクトを指定し <see cref="Invoke(object)"/> メソッドを呼び出します。
        /// 処理の呼び出し後に関連オブジェクトはデタッチされます。
        /// </summary>
        /// <param name="attachedObject">アタッチされる関連オブジェクト</param>
        /// <param name="args">イベントの情報</param>
        public void PerformInvoke(DependencyObject attachedObject, InteractionRequestedEventArgs args)
        {
            this.Attach(attachedObject);
            this.Invoke(args);
            this.Detach();
        }

        /// <summary>
        /// <paramref name="parameter"/> が <see cref="InteractionRequestedEventArgs"/> 型と互換性がある場合は
        /// <see cref="Invoke(InteractionRequestedEventArgs)"/> メソッドを呼び出します。
        /// </summary>
        /// <param name="parameter">パラメータ</param>
        protected override void Invoke(object parameter)
        {
            if (parameter is InteractionRequestedEventArgs args)
                this.Invoke(args);
        }

        /// <summary>
        /// このメソッドをオーバーライドしアクションを実装します。
        /// </summary>
        /// <param name="args">イベントの情報</param>
        protected abstract void Invoke(InteractionRequestedEventArgs args);
    }
}