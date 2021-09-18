using MultiSourceTorrentDownloader.Interfaces;
using Ninject;
using System.Reflection;
using System.Windows;

namespace MultiSourceTorrentDownloader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ILogService _logger; 
        public App()
        {

            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());
            _logger = kernel.Get<ILogService>();

            Current.DispatcherUnhandledException += HandleDispatcherUnhandledException;
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //Secret.SyncFususionLicenseKey is a const string containing the community license for the project.
            //Please replace with stringor create a file Secret.cs with a const string referring to a license.
            //If Secret.cs is created in same directory as App.xaml.cs, gitignore will not commit the secret.
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(Secret.SyncFususionLicenseKey);
        }

        private void HandleDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            _logger.Error($"Unhandled exception occured: {e.Exception.StackTrace}");
            Current.Shutdown();
        }
    }
}
