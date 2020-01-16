using CefSharp.Wpf;
using MultiSourceTorrentDownloader.Models;
using System.Windows;
using System.Windows.Controls;

namespace MultiSourceTorrentDownloader.Views
{
    /// <summary>
    /// Interaction logic for TorrentInfoDialogView.xaml
    /// </summary>
    public partial class TorrentInfoDialogView : UserControl
    {
        private static UserControl _userControl;
        public TorrentInfoDialogView()
        {
            _userControl = this;
            Loaded += TorrentInfoDialogView_Loaded;
            InitializeComponent();
        }

        private void TorrentInfoDialogView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is TorrentInfoDialogModel context)
            {
                context.IsLoading = true;// showing loading icon while browser loads up
            }
        }

        private void ChromiumWebBrowser_IsBrowserInitializedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is ChromiumWebBrowser browser && browser.IsBrowserInitialized)
            {
                if (DataContext is TorrentInfoDialogModel context)
                {
                    context.IsLoading = false;
                }
                else
                {
                    MessageBox.Show("Model for torrent info is incorrect! Loss in information may occur");
                }
            }
        }

        public static void ParentWindowSizeChangedHandler(object sender, SizeChangedEventArgs e)
        {
            if(_userControl != null)
            {
                _userControl.Width = e.NewSize.Width;
                _userControl.Height = e.NewSize.Height;
            }
        }
    }
}
