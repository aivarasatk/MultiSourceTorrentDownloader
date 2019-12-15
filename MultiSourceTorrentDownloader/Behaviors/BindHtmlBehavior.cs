using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace MultiSourceTorrentDownloader.Behaviors
{
    public class BindHtmlBehavior : Behavior<WebBrowser>
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
            AssociatedObject.Loaded += Loaded;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Loaded -= Loaded;
        }

        private void Loaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.NavigateToString(Html ?? string.Empty);
        }

    }
}
