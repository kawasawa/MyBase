using System;

namespace MyBase.Wpf.InteractionRequest
{
    /// <summary>
    /// インタラクションリクエストを表します。
    /// ViewModel は変更通知プロパティを介してインタラクションリクエストオブジェクトを公開し、View はインタラクションが要求されたときにイベントを発火させることができます。
    /// </summary>
    public class InteractionRequest<T> : IInteractionRequest
        where T : INotification
    {
        /// <summary>
        /// インタラクションが要求されたときに発生します。
        /// </summary>
        public event EventHandler<InteractionRequestedEventArgs> Raised;

        /// <summary>
        /// <see cref="Raised"/> イベントを発火させます。
        /// </summary>
        /// <param name="context">インタラクションのコンテキスト</param>
        public void Raise(T context)
            => this.Raise(context, _ => { });

        /// <summary>
        /// <see cref="Raised"/> イベントを発火させます。
        /// </summary>
        /// <param name="context">インタラクションのコンテキスト</param>
        /// <param name="callback">インタラクションが完了したときに呼び出されるコールバックメソッド</param>
        public void Raise(T context, Action<T> callback)
            => this.Raised?.Invoke(this, new InteractionRequestedEventArgs(context, () => callback?.Invoke(context)));
    }
}
