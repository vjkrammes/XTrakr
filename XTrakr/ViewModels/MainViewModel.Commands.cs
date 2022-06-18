using System.Windows.Input;

using XTrakr.Infrastructure;

namespace XTrakr.ViewModels;
public partial class MainViewModel
{
    private AsyncCommand? _windowLoadedCommand;
    public ICommand WindowLoadedCommand
    {
        get
        {
            if (_windowLoadedCommand is null)
            {
                _windowLoadedCommand = new(WindowLoaded, AlwaysCanExecute);
            }
            return _windowLoadedCommand;
        }
    }

    private RelayCommand? _exitCommand;
    public ICommand ExitCommand
    {
        get
        {
            if (_exitCommand is null)
            {
                _exitCommand = new(parm => ExitClick(), parm => AlwaysCanExecute());
            }
            return _exitCommand;
        }
    }

    private AsyncCommand? _filterCommand;
    public ICommand FilterCommand
    {
        get
        {
            if (_filterCommand is null)
            {
                _filterCommand = new(FilterClick, AlwaysCanExecute);
            }
            return _filterCommand;
        }
    }

    private RelayCommand? _expenseTypesCommand;
    public ICommand ExpenseTypesCommand
    {
        get
        {
            if (_expenseTypesCommand is null)
            {
                _expenseTypesCommand = new(parm => ExpenseTypesClick(), parm => AlwaysCanExecute());
            }
            return _expenseTypesCommand;
        }
    }

    private RelayCommand? _payeesCommand;
    public ICommand PayeesCommand
    {
        get
        {
            if (_payeesCommand is null)
            {
                _payeesCommand = new(parm => PayeesClick(), parm => AlwaysCanExecute());
            }
            return _payeesCommand;
        }
    }

    private AsyncCommand? _addCommand;
    public ICommand AddCommand
    {
        get
        {
            if (_addCommand is null)
            {
                _addCommand = new(AddClick, AddCanClick);
            }
            return _addCommand;
        }
    }

    private AsyncCommand? _editCommand;
    public ICommand EditCommand
    {
        get
        {
            if (_editCommand is null)
            {
                _editCommand = new(EditClick, ExpenseSelected);
            }
            return _editCommand;
        }
    }

    private RelayCommand? _deselectCommand;
    public ICommand DeselectCommand
    {
        get
        {
            if (_deselectCommand is null)
            {
                _deselectCommand = new(parm => DeselectClick(), parm => ExpenseSelected());
            }
            return _deselectCommand;
        }
    }

    private AsyncCommand? _deleteCommand;
    public ICommand DeleteCommand
    {
        get
        {
            if (_deleteCommand is null)
            {
                _deleteCommand = new(DeleteClick, ExpenseSelected);
            }
            return _deleteCommand;
        }
    }

    private RelayCommand? _dashboardCommand;
    public ICommand DashboardCommand
    {
        get
        {
            if (_dashboardCommand is null)
            {
                _dashboardCommand = new(parm => DashboardClick(), parm => AlwaysCanExecute());
            }
            return _dashboardCommand;
        }
    }

    private RelayCommand? _backupCommand;
    public ICommand BackupCommand
    {
        get
        {
            if (_backupCommand is null)
            {
                _backupCommand = new(parm => BackupClick(), parm => AlwaysCanExecute());
            }
            return _backupCommand;
        }
    }

    private RelayCommand? _aboutCommand;
    public ICommand AboutCommand
    {
        get
        {
            if (_aboutCommand is null)
            {
                _aboutCommand = new(parm => AboutClick(), parm => AlwaysCanExecute());
            }
            return _aboutCommand;
        }
    }
}
