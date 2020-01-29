using CefSharp;
using CefSharp.Wpf;
using System.Windows;
using System.Windows.Interactivity;

namespace MultiSourceTorrentDownloader.Behaviors
{
    public class BindHtmlBehavior : Behavior<ChromiumWebBrowser>
    {
        private const string _dynamicImageStyle = "<head><style>img { padding: 0; display: block; margin: 0 auto; max-height: 100%; max-width: 100%; }</style></head>";

        public static readonly DependencyProperty HtmlProperty =
                DependencyProperty.Register("Html", typeof(string), typeof(BindHtmlBehavior));

        public string Html
        {
            get { return (string)GetValue(HtmlProperty); }
            set { SetValue(HtmlProperty, value); }
        }

        protected override void OnAttached()
        {
            AssociatedObject.IsBrowserInitializedChanged += IsBrowserInitializedChanged;
        }

        private void IsBrowserInitializedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (AssociatedObject.IsBrowserInitialized)
            {
                var fitToScreenHtml = $"{_dynamicImageStyle}{Html ?? string.Empty}";
                AssociatedObject.LoadHtml(fitToScreenHtml);
            }
        }

        protected override void OnDetaching()
        {
            AssociatedObject.IsBrowserInitializedChanged -= IsBrowserInitializedChanged;
        }

    }
}
