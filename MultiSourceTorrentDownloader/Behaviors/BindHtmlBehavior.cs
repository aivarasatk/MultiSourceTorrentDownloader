using CefSharp;
using CefSharp.Wpf;
using MultiSourceTorrentDownloader.Models;
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
                AssociatedObject.LoadHtml(Html ?? string.Empty);
            }
        }

        protected override void OnDetaching()
        {
            AssociatedObject.IsBrowserInitializedChanged -= IsBrowserInitializedChanged;
        }

    }
}
