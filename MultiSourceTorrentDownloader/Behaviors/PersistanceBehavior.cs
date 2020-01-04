using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;

namespace MultiSourceTorrentDownloader.Behaviors
{
    public class PersistanceBehavior : Behavior<Window>
    {
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
            if (WindowSizeSettingsExist())
            {
                AssociatedObject.SizeToContent = SizeToContent.Manual;
                AssociatedObject.Width = Properties.Settings.Default.WindowWidth;
                AssociatedObject.Height = Properties.Settings.Default.WindowHeight;
            }
            else
                AssociatedObject.SizeToContent = SizeToContent.WidthAndHeight;

            if (WindowPositionSettingsExist())
            {
                AssociatedObject.WindowStartupLocation = Properties.Settings.Default.WindowStartupLocation;
                AssociatedObject.Left = Properties.Settings.Default.WindowPositionLeft;
                AssociatedObject.Top = Properties.Settings.Default.WindowPositionTop;
            }
        }

        private bool WindowSizeSettingsExist()
        {
            return Properties.Settings.Default.WindowWidth != 0 
                && Properties.Settings.Default.WindowHeight != 0;
        }

        private bool WindowPositionSettingsExist()
        {
            return Properties.Settings.Default.WindowPositionTop != 0 
                && Properties.Settings.Default.WindowPositionLeft != 0;
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.WindowWidth = AssociatedObject.ActualWidth;
            Properties.Settings.Default.WindowHeight = AssociatedObject.ActualHeight;

            Properties.Settings.Default.WindowStartupLocation = WindowStartupLocation.Manual;
            Properties.Settings.Default.WindowPositionLeft = AssociatedObject.Left;
            Properties.Settings.Default.WindowPositionTop = AssociatedObject.Top;

            Properties.Settings.Default.Save();
        }
    }
}
