using System.Windows;

namespace MyBase.Wpf.InteractionRequest
{
    /// <summary>
    /// インタラクションリクエストに対してメッセージを通知するアクションを表します。
    /// </summary>
    public class MessageAction : InteractionRequestAction<DependencyObject>
    {
        /// <summary>
        /// メッセージボックスのタイトルを示す依存関係プロパティを表します。
        /// </summary>
        public static readonly DependencyProperty TitleProperty
            = DependencyProperty.Register(nameof(Title), typeof(string), typeof(MessageAction), new PropertyMetadata());

        /// <summary>
        /// メッセージボックスのテキストを示す依存関係プロパティを表します。
        /// </summary>
        public static readonly DependencyProperty MessageProperty
            = DependencyProperty.Register(nameof(Message), typeof(string), typeof(MessageAction), new PropertyMetadata());

        /// <summary>
        /// メッセージボックスに表示するボタンを示す依存関係プロパティを表します。
        /// </summary>
        public static readonly DependencyProperty ButtonsProperty
            = DependencyProperty.Register(nameof(Buttons), typeof(MessageBoxButton), typeof(MessageAction), new PropertyMetadata(MessageBoxButton.OK));

        /// <summary>
        /// メッセージボックスに表示するイメージを示す依存関係プロパティを表します。
        /// </summary>
        public static readonly DependencyProperty ImageProperty
            = DependencyProperty.Register(nameof(Image), typeof(MessageBoxImage), typeof(MessageAction), new PropertyMetadata(MessageBoxImage.None));

        /// <summary>
        /// メッセージボックスの既定の結果を示す依存関係プロパティを表します。
        /// </summary>
        public static readonly DependencyProperty DefaultResultProperty
            = DependencyProperty.Register(nameof(DefaultResult), typeof(MessageBoxResult), typeof(MessageAction), new PropertyMetadata(MessageBoxResult.None));

        /// <summary>
        /// メッセージボックスのオプションを示す依存関係プロパティを表します。
        /// </summary>
        public static readonly DependencyProperty OptionsProperty
            = DependencyProperty.Register(nameof(Options), typeof(MessageBoxOptions), typeof(MessageAction), new PropertyMetadata(MessageBoxOptions.None));

        /// <summary>
        /// <see cref="TitleProperty"/> の CLR ラッパープロパティを表します。
        /// </summary>
        public string Title
        {
            get => (string)this.GetValue(TitleProperty);
            set => this.SetValue(TitleProperty, value);
        }

        /// <summary>
        /// <see cref="MessageProperty"/> の CLR ラッパープロパティを表します。
        /// </summary>
        public string Message
        {
            get => (string)this.GetValue(MessageProperty);
            set => this.SetValue(MessageProperty, value);
        }


        /// <summary>
        /// <see cref="ButtonsProperty"/> の CLR ラッパープロパティを表します。
        /// </summary>
        public MessageBoxButton Buttons
        {
            get => (MessageBoxButton)this.GetValue(ButtonsProperty);
            set => this.SetValue(ButtonsProperty, value);
        }

        /// <summary>
        /// <see cref="ImageProperty"/> の CLR ラッパープロパティを表します。
        /// </summary>
        public MessageBoxImage Image
        {
            get => (MessageBoxImage)this.GetValue(ImageProperty);
            set => this.SetValue(ImageProperty, value);
        }

        /// <summary>
        /// <see cref="DefaultResultProperty"/> の CLR ラッパープロパティを表します。
        /// </summary>
        public MessageBoxResult DefaultResult
        {
            get => (MessageBoxResult)this.GetValue(DefaultResultProperty);
            set => this.SetValue(DefaultResultProperty, value);
        }

        /// <summary>
        /// <see cref="OptionsProperty"/> の CLR ラッパープロパティを表します。
        /// </summary>
        public MessageBoxOptions Options
        {
            get => (MessageBoxOptions)this.GetValue(OptionsProperty);
            set => this.SetValue(OptionsProperty, value);
        }

        /// <summary>
        /// アクションを実行します。
        /// </summary>
        /// <param name="args">イベントの情報</param>
        protected override void Invoke(InteractionRequestedEventArgs args)
        {
            if (args.Context is not IConfirmation context)
                return;

            var owner = this.AssociatedObject is Window window ? window : Window.GetWindow(this.AssociatedObject);
            var title = string.IsNullOrEmpty(this.Title) == false ? this.Title : context.Title;
            var message = string.IsNullOrEmpty(this.Message) == false ? this.Message : context.Content?.ToString() ?? string.Empty;

            this.Invoke(context, owner, title, message, this.Buttons, this.Image, this.DefaultResult, this.Options);
            args.Callback?.Invoke();
        }

        /// <summary>
        /// アクションを実行します。
        /// </summary>
        /// <param name="context">インタラクションのコンテキスト</param>
        /// <param name="owner">オーナーウィンドウ</param>
        /// <param name="title">メッセージボックスのタイトル</param>
        /// <param name="message">メッセージボックスに表示するテキスト</param>
        /// <param name="buttons">メッセージボックスに表示するボタン</param>
        /// <param name="image">メッセージボックスに表示するイメージ</param>
        /// <param name="defaultResult">メッセージボックスの既定の結果</param>
        /// <param name="options">メッセージボックスのオプション</param>
        protected virtual void Invoke(IConfirmation context, Window owner, string title, string message, MessageBoxButton buttons, MessageBoxImage image, MessageBoxResult defaultResult, MessageBoxOptions options)
        {
            owner?.Activate();
            var result = MessageBox.Show(owner, message, title, buttons, image, defaultResult, options);
            context.Confirmed = result switch
            {
                MessageBoxResult.OK or MessageBoxResult.Yes => true,
                MessageBoxResult.No => false,
                _ => null,
            };
        }
    }
}