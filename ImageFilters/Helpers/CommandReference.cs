﻿using System;
using System.Windows;
using System.Windows.Input;

namespace ImageFilters.Helpers
{
    public class CommandReference : Freezable, ICommand
    {
        public CommandReference()
        {
        }
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(CommandReference), new PropertyMetadata(new PropertyChangedCallback(OnCommandChanged)));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            if (Command != null)
                return Command.CanExecute(parameter);
            return false;
        }

        public void Execute(object parameter)
        {
            Command.Execute(parameter);
        }

        public event EventHandler CanExecuteChanged;

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CommandReference commandReference = d as CommandReference;
            if (commandReference != null)
            {
                ICommand oldCommand = e.OldValue as ICommand;
                if (oldCommand != null)
                    oldCommand.CanExecuteChanged -= commandReference.CanExecuteChanged;

                ICommand newCommand = e.NewValue as ICommand;
                if (newCommand != null)
                    newCommand.CanExecuteChanged += commandReference.CanExecuteChanged;
            }
        }

        #endregion

        #region Freezable

        protected override Freezable CreateInstanceCore()
        {
            return new CommandReference();
        }

        #endregion
    }
}