using Microsoft.Xaml.Behaviors;

namespace MyBase.Wpf.InteractionRequest
{
    /// <summary>
    /// インタラクションリクエストに対応するイベントトリガーを表します。
    /// </summary>
    public class InteractionRequestTrigger : EventTrigger
    {
        /// <summary>
        /// イベントトリガーがリッスンするイベントの名称を取得します。
        /// </summary>
        /// <returns><see cref = "IInteractionRequest" /> との接続を前提としているため、常に <see cref="IInteractionRequest.Raised"/> を返します。</returns>
        protected override string GetEventName()
            => nameof(IInteractionRequest.Raised);
    }
}
