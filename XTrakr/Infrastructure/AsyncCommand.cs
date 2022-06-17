using System;
using System.Threading.Tasks;
using System.Windows.Input;

using XTrakr.Interfaces;

namespace XTrakr.Infrastructure;

//
// code based on this article: https://johnthiriet.com/mvvm-going-async-with-async-command/
//

public class AsyncCommand : IAsyncCommand
{
    private bool _isExecuting;
    private readonly Func<Task> _execute;
    private readonly Func<bool>? _canExecute;
    private readonly Action<Exception>? _errorHandler;

    public AsyncCommand(Func<Task> execute, Func<bool>? canExecute, Action<Exception>? handler = null)
    {
        _isExecuting = false;
        _execute = execute;
        _canExecute = canExecute;
        _errorHandler = handler;
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute() => !_isExecuting && (_canExecute?.Invoke() ?? true);

    public async Task ExecuteAsync()
    {
        if (CanExecute())
        {
            try
            {
                _isExecuting = true;
                await _execute();
            }
            finally
            {
                _isExecuting = false;
            }
        }
    }

    bool ICommand.CanExecute(object? parameter) => CanExecute();

    void ICommand.Execute(object? parameter)
    {
        ExecuteAsync().FireAndForgetSafeAsync(_errorHandler);
    }
}

public class AsyncCommand<T> : IAsyncCommand<T>
{
    private bool _isExecuting;
    private readonly Func<T, Task> _execute;
    private readonly Func<T, bool>? _canExecute;
    private readonly Action<Exception>? _errorHandler;

    public AsyncCommand(Func<T, Task> execute, Func<T, bool>? canExecute, Action<Exception>? handler = null)
    {
        _isExecuting = false;
        _execute = execute;
        _canExecute = canExecute;
        _errorHandler = handler;
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(T parameter)
    {
        return !_isExecuting && (_canExecute?.Invoke(parameter) ?? true);
    }

    public async Task ExecuteAsync(T parameter)
    {
        if (CanExecute(parameter))
        {
            try
            {
                _isExecuting = true;
                await _execute(parameter);
            }
            finally
            {
                _isExecuting = false;
            }
        }
    }

    bool ICommand.CanExecute(object? parameter) => CanExecute((T)parameter!);

    void ICommand.Execute(object? parameter)
    {
        ExecuteAsync((T)parameter!).FireAndForgetSafeAsync(_errorHandler);
    }
}