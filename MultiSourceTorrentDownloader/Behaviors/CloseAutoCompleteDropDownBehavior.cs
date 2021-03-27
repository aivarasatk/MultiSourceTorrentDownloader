using Syncfusion.Windows.Controls.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interactivity;

namespace MultiSourceTorrentDownloader.Behaviors
{
    public class CloseAutoCompleteDropDownBehavior : Behavior<SfTextBoxExt>
    {
        protected override void OnAttached()
        {
            AssociatedObject.LostKeyboardFocus += OnLostFocus;

            base.OnAttached();
        }

        private void OnLostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            AssociatedObject.IsSuggestionOpen = false;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.LostKeyboardFocus -= OnLostFocus;

            base.OnDetaching();
        }
    }
}
