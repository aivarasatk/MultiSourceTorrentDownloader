using System.Windows;

namespace MultiSourceTorrentDownloader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //Secret.SyncFususionLicenseKey is a const string containing the community license for the project.
            //Please replace with stringor create a file Secret.cs with a const string referring to a license.
            //If Secret.cs is created in same directory as App.xaml.cs, gitignore will not commit the secret.
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(Secret.SyncFususionLicenseKey);
        }
    }
}
