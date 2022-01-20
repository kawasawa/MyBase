using System;

namespace MyBase.Wpf.InteractionRequest
{
    /// <summary>
    /// <see cref="IInteractionRequest.Raised"/> で使用されるイベントデータを表します。
    /// </summary>
    public class InteractionRequestedEventArgs : EventArgs
    {
        /// <summary>
        /// インタラクションのコンテキストを取得します。
        /// </summary>
        public INotification Context { get; }

        /// <summary>
        /// インタラクションが完了したときに呼び出されるコールバックメソッドを取得します。
        /// </summary>
        public Action Callback { get; }

        /// <summary>
        /// このクラスの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="context">インタラクションのコンテキスト</param>
        /// <param name="callback">インタラクションが完了したときに呼び出されるコールバックメソッド</param>
        public InteractionRequestedEventArgs(INotification context, Action callback)
        {
            this.Context = context;
            this.Callback = callback;
        }
    }
}
