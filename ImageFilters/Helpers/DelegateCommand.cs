using System;
using System.Windows.Input;

namespace ImageFilters.Helpers
{
    public class DelegateCommand : ICommand
    {
        private readonly Action<object> _command;
        private readonly Action _command2;
        private readonly Func<bool> _canExecute;
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public DelegateCommand(Action command2, Func<bool> canExecute = null)
        {
            if (command2 == null)
                throw new ArgumentNullException();
            _canExecute = canExecute;
            _command2 = command2;
        }

        public DelegateCommand(Action<object> command, Func<bool> canExecute = null)
        {
            if (command == null)
                throw new ArgumentNullException();
            _canExecute = canExecute;
            _command = command;
        }

        public void Execute(object parameter)
        {
            if (parameter == null)
                _command2();
            else
            _command(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

    }
}