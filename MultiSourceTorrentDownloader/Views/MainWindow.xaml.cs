using MultiSourceTorrentDownloader.ViewModels;
using Ninject;
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
        }
    }
}
