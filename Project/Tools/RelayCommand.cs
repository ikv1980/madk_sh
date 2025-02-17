﻿using System;
using System.Windows.Input;

namespace Project.Tools
{
    class RelayCommand : ICommand
    {
        public Action<object> _execute;
        public Func<object, bool> _canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this._execute = execute;
            this._canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => this._canExecute == null || this._canExecute(parameter);

        public void Execute(object parameter) => this._execute(parameter);
    }
}