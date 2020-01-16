using MultiSourceTorrentDownloader.ViewModels;
using Ninject;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

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
            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;//yields smoother window movement

            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());
            var viewModel = kernel.Get<MainViewModel>();
            DataContext = viewModel.Model;

            SizeChanged += TorrentInfoDialogView.ParentWindowSizeChangedHandler;
            Closing += viewModel.Closing;
        }
    }
}
