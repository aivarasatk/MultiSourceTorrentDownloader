using CefSharp;
using CefSharp.Wpf;
using System.Windows;
using System.Windows.Interactivity;

namespace MultiSourceTorrentDownloader.Behaviors
{
    public class BindHtmlBehavior : Behavior<ChromiumWebBrowser>
    {
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
                //hack around 1337x images. downloaded img src is pointing to empty .svg need to redirect to data-original
                AssociatedObject.LoadHtml(Html?.Replace("src", "nothing")?.Replace("data-original", "src") ?? string.Empty);
            }
        }

        protected override void OnDetaching()
        {
            AssociatedObject.IsBrowserInitializedChanged -= IsBrowserInitializedChanged;
        }

    }
}
