using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace MultiSourceTorrentDownloader.Common
{
    public class Command : ICommand
    {
        private Predicate<object> _canExecute;
        private Action<object> _execute;

        public Command(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public Command(Action<object> execute)
        {
            _execute = execute;
        }

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, new EventArgs());

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _canExecute != null ? _canExecute(parameter) : true;
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}
