using System;
using System.Collections.ObjectModel;

using XTrakr.Models;
using XTrakr.Services.Interfaces;

namespace XTrakr.ViewModels;
public partial class MainViewModel
{
    private readonly IExpenseService _expenseService;
    private readonly IPayeeService _payeeService;
    private readonly IExpenseTypeService _expenseTypeService;

    private readonly AboutViewModel _aboutViewModel;
    private readonly BackupViewModel _backupViewModel;
    private readonly DashboardViewModel _dashboardViewModel;
    private readonly ExpenseViewModel _expenseViewModel;
    private readonly ExpenseTypeViewModel _expenseTypeViewModel;
    private readonly IncomeViewModel _incomeViewModel;
    private readonly ManagePayeesViewModel _managePayeesViewModel;

    private string? _windowTitle;
    public string WindowTitle
    {
        get => _windowTitle!;
        set => SetProperty(ref _windowTitle, value);
    }

    private string? _banner;
    public string Banner
    {
        get => _banner!;
        set => SetProperty(ref _banner, value);
    }

    private bool _expenseTypesExist;
    public bool ExpenseTypesExist
    {
        get => _expenseTypesExist;
        set => SetProperty(ref _expenseTypesExist, value);
    }

    private bool _payeesExist;
    public bool PayeesExist
    {
        get => _payeesExist;
        set => SetProperty(ref _payeesExist, value);
    }

    private DateTime? _startDate;
    public DateTime? StartDate
    {
        get => _startDate;
        set => SetProperty(ref _startDate, value);
    }

    private DateTime? _endDate;
    public DateTime? EndDate
    {
        get => _endDate;
        set => SetProperty(ref _endDate, value);
    }

    private ObservableCollection<ExpenseModel>? _expenses;
    public ObservableCollection<ExpenseModel>? Expenses
    {
        get => _expenses;
        set => SetProperty(ref _expenses, value);
    }

    private ExpenseModel? _selectedExpense;
    public ExpenseModel? SelectedExpense
    {
        get => _selectedExpense;
        set => SetProperty(ref _selectedExpense, value);
    }
}
