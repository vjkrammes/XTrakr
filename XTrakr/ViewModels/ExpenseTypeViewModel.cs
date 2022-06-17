using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;

using XTrakr.Common;
using XTrakr.Enumerations;
using XTrakr.Infrastructure;
using XTrakr.Interfaces;
using XTrakr.Models;
using XTrakr.Services.Interfaces;

namespace XTrakr.ViewModels;

public class ExpenseTypeViewModel : ViewModelBase
{

    #region Properties 

    private readonly IExpenseTypeService _expenseTypeService;

    private bool _isEditing = false;

    private string? _name;
    public string? Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    private ObservableCollection<string>? _icons;
    public ObservableCollection<string>? Icons
    {
        get => _icons;
        set => SetProperty(ref _icons, value);
    }

    private string? _selectedIcon;
    public string? SelectedIcon
    {
        get => _selectedIcon;
        set => SetProperty(ref _selectedIcon, value);
    }

    private ObservableCollection<string>? _colors;
    public ObservableCollection<string>? Colors
    {
        get => _colors;
        set => SetProperty(ref _colors, value);
    }

    private string? _selectedColor;
    public string? SelectedColor
    {
        get => _selectedColor;
        set => SetProperty(ref _selectedColor, value);
    }

    private ObservableCollection<ExpenseTypeModel>? _expenseTypes;
    public ObservableCollection<ExpenseTypeModel>? ExpenseTypes
    {
        get => _expenseTypes;
        set => SetProperty(ref _expenseTypes, value);
    }

    private ExpenseTypeModel? _selectedExpenseType;
    public ExpenseTypeModel? SelectedExpenseType
    {
        get => _selectedExpenseType;
        set
        {
            SetProperty(ref _selectedExpenseType, value);
            if (SelectedExpenseType is not null)
            {
                LoadScreen();
                _isEditing = true;
            }
        }
    }

    #endregion

    #region Commands

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

    private AsyncCommand? _saveCommand;
    public IAsyncCommand SaveCommand
    {
        get
        {
            if (_saveCommand is null)
            {
                _saveCommand = new(SaveClick, SaveCanClick);
            }
            return _saveCommand;
        }
    }

    private RelayCommand? _clearCommand;
    public ICommand ClearCommand
    {
        get
        {
            if (_clearCommand is null)
            {
                _clearCommand = new(parm => ClearClick(), parm => AlwaysCanExecute());
            }
            return _clearCommand;
        }
    }

    private AsyncCommand? _deleteCommand;

    public ICommand DeleteCommand
    {
        get
        {
            if (_deleteCommand is null)
            {
                _deleteCommand = new(DeleteClick, DeleteCanClick);
            }
            return _deleteCommand;
        }
    }

    #endregion

    #region Command Methods

    private bool SaveCanClick()
    {
        if (_isEditing)
        {
            return SelectedExpenseType is not null && RequiredData();
        }
        else
        {
            return RequiredData();
        }
    }

    private async Task SaveClick()
    {
        if (_isEditing)
        {
            var expenseType = new ExpenseTypeModel
            {
                Id = SelectedExpenseType!.Id,
                Name = Name!.Capitalize(),
                Icon = SelectedIcon!,
                Background = SelectedColor!,
                ARGB = Palette.Value(Palette.Get(SelectedColor!))
            };
            var response = await _expenseTypeService.UpdateAsync(expenseType);
            if (response.Successful)
            {
                ExpenseTypes!.Remove(SelectedExpenseType);
                var ix = 0;
                while (ix < ExpenseTypes!.Count && expenseType > ExpenseTypes[ix])
                {
                    ix++;
                }
                ExpenseTypes.Insert(ix, expenseType);
                ClearClick();
            }
            else
            {
                PopupManager.Popup(response.Message!, "Database Error Updating Expense Type", PopupButtons.Ok, PopupImage.Error);
            }
        }
        else
        {
            var expenseType = new ExpenseTypeModel
            {
                Id = string.Empty,
                Name = Name!.Capitalize(),
                Icon = SelectedIcon!,
                Background = SelectedColor!,
                ARGB = Palette.Value(Palette.Get(SelectedColor!))
            };
            var response = await _expenseTypeService.InsertAsync(expenseType);
            if (response.Successful)
            {
                var ix = 0;
                while (ix < ExpenseTypes!.Count && expenseType > ExpenseTypes[ix])
                {
                    ix++;
                }
                ExpenseTypes.Insert(ix, expenseType);
                ClearClick();
            }
            else
            {
                PopupManager.Popup(response.Message!, "Database Error Adding Expense Type", PopupButtons.Ok, PopupImage.Error);
            }
        }
    }

    private void ClearClick()
    {
        SelectedColor = null;
        SelectedIcon = null;
        SelectedExpenseType = null;
        Name = string.Empty;
    }

    private bool ExpenseTypeSelected() => SelectedExpenseType is not null;

    private bool DeleteCanClick() => ExpenseTypeSelected() && (SelectedExpenseType?.CanDelete ?? false);

    private async Task DeleteClick()
    {
        if (SelectedExpenseType is null)
        {
            PopupManager.Popup("Please select an expense type", "No Expense Type Selected", PopupButtons.Ok, PopupImage.Stop);
        }
        else if (!SelectedExpenseType.CanDelete)
        {
            PopupManager.Popup("That expense type cannot be deleted because it has associated expenses", "Can't Delete Expense Type",
                PopupButtons.Ok, PopupImage.Stop);
        }
        else
        {
            var response = await _expenseTypeService.DeleteAsync(SelectedExpenseType);
            if (response.Successful)
            {
                ExpenseTypes!.Remove(SelectedExpenseType);
                ClearClick();
            }
            else
            {
                PopupManager.Popup(response.Message!, "Database Error Deleting Expense Type", PopupButtons.Ok, PopupImage.Error);
            }
        }
    }

    private async Task WindowLoaded()
    {
        ExpenseTypes = new((await _expenseTypeService.GetAsync()).OrderBy(x => x.Name));
        Icons = new(Infrastructure.Tools.GetImages(Assembly.GetExecutingAssembly()));
        Colors = new(Palette.Names());
    }

    #endregion

    #region Utility Methods

    private bool RequiredData() => !string.IsNullOrWhiteSpace(Name) && SelectedColor is not null && SelectedIcon is not null;

    private void LoadScreen()
    {
        if (SelectedExpenseType is null)
        {
            ClearClick();
        }
        else
        {
            Name = SelectedExpenseType.Name;
            SelectedIcon = SelectedExpenseType.Icon;
            SelectedColor = SelectedExpenseType.Background;
        }
    }

    #endregion

    public ExpenseTypeViewModel(IExpenseTypeService expenseTypeService) => _expenseTypeService = expenseTypeService;
}
