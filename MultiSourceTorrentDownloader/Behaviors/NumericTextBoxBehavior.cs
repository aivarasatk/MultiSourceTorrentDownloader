using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace MultiSourceTorrentDownloader.Behaviors
{
    public class NumericTextBoxBehavior : Behavior<TextBox>
    {
        public static readonly DependencyProperty MaxValueProperty =
                DependencyProperty.Register("MaxValue", typeof(int?), typeof(NumericTextBoxBehavior));

        public static readonly DependencyProperty EmptyValueProperty =
                DependencyProperty.Register("EmptyValue", typeof(int?), typeof(NumericTextBoxBehavior));

        public int? MaxValue
        {
            get { return (int?)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        public int? EmptyValue
        {
            get { return (int?)GetValue(EmptyValueProperty); }
            set { SetValue(EmptyValueProperty, value); }
        }

        protected override void OnAttached()
        {
            AssociatedObject.PreviewTextInput += PreviewTextInput;
            AssociatedObject.LostFocus += LostFocus;
            CommandManager.AddPreviewCanExecuteHandler(AssociatedObject, PreventPastingHandler);
        }

        private void LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(AssociatedObject.Text) && EmptyValue.HasValue)
                AssociatedObject.Text = EmptyValue.Value.ToString();
        }

        private void PreventPastingHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Paste)
            {
                e.CanExecute = false;
                e.Handled = true;
            }
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewTextInput -= PreviewTextInput;
        }

        private void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex(@"^[1-9][0-9]*$");
            e.Handled = !regex.IsMatch(AssociatedObject.Text + e.Text);

            if (!e.Handled)
            {
                if (MaxValue.HasValue && int.Parse(AssociatedObject.Text + e.Text) > MaxValue.Value)
                {
                    AssociatedObject.Text = MaxValue.Value.ToString();
                    e.Handled = true;
                }
            }
        }
    }
}
