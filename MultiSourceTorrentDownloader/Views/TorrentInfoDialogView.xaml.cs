using CefSharp.Wpf;
using MultiSourceTorrentDownloader.Models;
using System;
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

        private void IsBrowserInitializedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is ChromiumWebBrowser browser && browser.IsBrowserInitialized)
            {
                TrySetLoadingIndicator(false);
            }
        }

        private void LoadingStateChanged(object sender, CefSharp.LoadingStateChangedEventArgs e)
        {
            if(e.IsLoading)
                TrySetLoadingIndicator(true);
            else
                TrySetLoadingIndicator(false);
        }

        private void TrySetLoadingIndicator(bool value)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                if (DataContext is TorrentInfoDialogModel context)
                {
                    context.IsLoading = value;
                }
                else
                {
                    MessageBox.Show("Model for torrent info is incorrect! Loss in information may occur");
                }
            });
            
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
