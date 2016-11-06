﻿using System;
using System.Windows.Input;

namespace Calculator.ViewModels
{
    class ActionCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Action<object> _executeParam;
        private readonly Func<bool> _canExecute;


        public ActionCommand(Action executeAction, Func<bool> canExecuteFunc)
        {
            _execute = executeAction;
            _canExecute = canExecuteFunc;
        }

        public ActionCommand(Action<object> executeAction, Func<bool> canExecuteFunc)
        {
            _executeParam = executeAction;
            _canExecute = canExecuteFunc;
        }

        
        void ICommand.Execute(object parameter)
        {
            if (_execute != null) {
                _execute();
            }
            else if (_executeParam != null) {
                _executeParam(parameter);
            }
        }


        bool ICommand.CanExecute(object parameter)
        {
            if (_canExecute != null) {
                return _canExecute();
            }

            return true;
        }

        
        event EventHandler ICommand.CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
