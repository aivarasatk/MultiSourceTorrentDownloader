using MultiSourceTorrentDownloader.Enums;
using MultiSourceTorrentDownloader.Models;
using MultiSourceTorrentDownloader.ViewModels;
using Ninject;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;

namespace MultiSourceTorrentDownloader.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());
            var viewModel = kernel.Get<MainViewModel>();

            DataContext = viewModel.Model;

            Loaded += WindowLoaded;
            Closing += new CancelEventHandler((sender, e) => CanSavePersistanceHandler(sender, e, viewModel.Model));
            
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            if(Properties.Settings.Default.WindowWidth != 0)
                Width = Properties.Settings.Default.WindowWidth;

            if(Properties.Settings.Default.WindowHeight != 0)
                Height = Properties.Settings.Default.WindowHeight;
        }

        private void CanSavePersistanceHandler(object sender, CancelEventArgs e, MainModel model)
        {
            Properties.Settings.Default.WindowWidth = ActualWidth;
            Properties.Settings.Default.WindowHeight = ActualHeight;

            Properties.Settings.Default.Save();
        }
    }
}
