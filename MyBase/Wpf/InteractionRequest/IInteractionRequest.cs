using System;

namespace MyBase.Wpf.InteractionRequest
{
    /// <summary>
    /// インタラクションリクエストのインターフェースを表します。
    /// </summary>
    public interface IInteractionRequest
    {
        /// <summary>
        /// インタラクションリクエストが要求されたときに発生します。
        /// </summary>
        event EventHandler<InteractionRequestedEventArgs> Raised;
    }
}
