﻿using System;
using System.Windows.Input;

namespace MusicPlayer.Model;

public class Command : ICommand
{
    private readonly Action<object> _execute;
    private readonly Func<object, bool> _canExecute;
    
    public bool CanExecute(object parameter)
    {
        return this._canExecute == null || this._canExecute(parameter);
    }

    public void Execute(object parameter)
    {
        this._execute(parameter);
    }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public Command(Action<object> execute, Func<object, bool> canExecute = null)
    {
        this._execute = execute;
        this._canExecute = null;
    }
}