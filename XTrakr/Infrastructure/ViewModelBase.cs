using System.Windows.Input;

namespace XTrakr.Infrastructure;
public abstract class ViewModelBase : NotifyBase
{
    private RelayCommand? _cancelCommand;
    public ICommand CancelCommand
    {
        get
        {
            if (_cancelCommand is null)
            {
                _cancelCommand = new(parm => Cancel(), parm => AlwaysCanExecute());
            }
            return _cancelCommand;
        }
    }

    private RelayCommand? _okCommand;
    public ICommand OkCommand
    {
        get
        {
            if (_okCommand is null)
            {
                _okCommand = new(parm => OK(), parm => OkCanExecute());
            }
            return _okCommand;
        }
    }

    public static bool AlwaysCanExecute() => true;

    public virtual void Cancel() => DialogResult = false;

    public virtual bool OkCanExecute() => true;

    public virtual void OK() => DialogResult = true;

    public bool? _dialogResult;
    public bool? DialogResult
    {
        get => _dialogResult;
        set => SetProperty(ref _dialogResult, value);
    }

    public virtual void Reset() => _dialogResult = null;
}
