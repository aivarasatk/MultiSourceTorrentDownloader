using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace MultiSourceTorrentDownloader.Behaviors
{
    public class ScrolledToEndBehavior : Behavior<ScrollViewer>
    {
        public Action ScrolledToEndAction
        {
            get { return (Action)GetValue(ScrolledToEndActionProperty); }
            set { SetValue(ScrolledToEndActionProperty, value); }
        }

        public static readonly DependencyProperty ScrolledToEndActionProperty = DependencyProperty.Register("ScrolledToEndAction", typeof(Action), typeof(ScrolledToEndBehavior));

        protected override void OnAttached()
        {
            AssociatedObject.ScrollChanged += OnScrollChangedHandler;
        }

        private void OnScrollChangedHandler(object sender, ScrollChangedEventArgs e)
        {
            var scrollView = sender as ScrollViewer;
            
            if(scrollView.VerticalOffset == scrollView.ScrollableHeight)
            {
                ScrolledToEndAction?.Invoke();
                //Execute command
            }
        }


    }
}
