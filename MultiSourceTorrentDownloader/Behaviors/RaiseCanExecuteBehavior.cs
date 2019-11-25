using MultiSourceTorrentDownloader.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace MultiSourceTorrentDownloader.Behaviors
{
    public class RaiseCanExecuteBehavior : Behavior<CheckBox>
    {
        public static readonly DependencyProperty Command1Property =
         DependencyProperty.Register("Command1", typeof(Command), typeof(RaiseCanExecuteBehavior));

        public Command Command1
        {
            get { return (Command)GetValue(Command1Property); }
            set { SetValue(Command1Property, value); }
        }

        public static readonly DependencyProperty Command2Property =
         DependencyProperty.Register("Command2", typeof(Command), typeof(RaiseCanExecuteBehavior));

        public Command Command2
        {
            get { return (Command)GetValue(Command2Property); }
            set { SetValue(Command2Property, value); }
        }

        protected override void OnAttached()
        {
            AssociatedObject.Click += CheckedHandler;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Click -= CheckedHandler;
        }

        private void CheckedHandler(object sender, RoutedEventArgs e)
        {
            Command1?.RaiseCanExecuteChanged();
            Command2?.RaiseCanExecuteChanged();
        }
    }
}
