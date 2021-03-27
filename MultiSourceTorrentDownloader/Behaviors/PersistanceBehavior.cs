using MultiSourceTorrentDownloader.Data;
using MultiSourceTorrentDownloader.Interfaces;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;

namespace MultiSourceTorrentDownloader.Behaviors
{
    public class PersistanceBehavior : Behavior<System.Windows.Window>
    {
        private IUserConfiguration _config;
        protected override void OnAttached()
        {
            AssociatedObject.Loaded += WindowLoaded;
            AssociatedObject.Closing += WindowClosing;
        }


        protected override void OnDetaching()
        {
            AssociatedObject.Loaded -= WindowLoaded;
            AssociatedObject.Closing -= WindowClosing;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            var settings = GetUserSettings();

            if(settings.Window == null)
            {
                AssociatedObject.SizeToContent = SizeToContent.WidthAndHeight;
                return;
            }

            AssociatedObject.SizeToContent = SizeToContent.Manual;
            AssociatedObject.WindowStartupLocation = WindowStartupLocation.Manual;

            AssociatedObject.Width = settings.Window.Width;
            AssociatedObject.Height = settings.Window.Height;
            AssociatedObject.Left = settings.Window.PositionLeft;
            AssociatedObject.Top = settings.Window.PositionTop;
        }

        private Settings GetUserSettings()
        {
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());
            _config = kernel.Get<IUserConfiguration>();
            return _config.GetConfiguration();
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var window = new Data.Window
            {
                Width = AssociatedObject.ActualWidth,
                Height = AssociatedObject.ActualHeight,
                PositionLeft = AssociatedObject.Left,
                PositionTop = AssociatedObject.Top
            };

            _config.SaveSettings(window);
        }
    }
}
