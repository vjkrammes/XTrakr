using CsvHelper;
using CsvHelper.Configuration;

using Microsoft.Win32;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using XTrakr.Common;
using XTrakr.Enumerations;
using XTrakr.Infrastructure;
using XTrakr.Models;
using XTrakr.Services.Interfaces;

namespace XTrakr.ViewModels;

public class DashboardViewModel : ViewModelBase
{
    #region Properties

    private readonly IExpenseService _expenseService;
    private readonly IPayeeService _payeeService;
    private readonly IExpenseTypeService _expenseTypeService;

    private ObservableCollection<ChartItem> _expensesByCategory;
    public ObservableCollection<ChartItem> ExpensesByCategory
    {
        get => _expensesByCategory;
        set => SetProperty(ref _expensesByCategory, value);
    }

    private ObservableCollection<ChartItem> _expensesByPayee;
    public ObservableCollection<ChartItem> ExpensesByPayee
    {
        get => _expensesByPayee;
        set => SetProperty(ref _expensesByPayee, value);
    }

    private bool _showGraphs;
    public bool ShowGraphs
    {
        get => _showGraphs;
        set => SetProperty(ref _showGraphs, value);
    }

    private ObservableCollection<PayeeModel>? _payees;
    public ObservableCollection<PayeeModel>? Payees
    {
        get => _payees;
        set => SetProperty(ref _payees, value);
    }

    private PayeeModel? _selectedPayee;
    public PayeeModel? SelectedPayee
    {
        get => _selectedPayee;
        set => SetProperty(ref _selectedPayee, value);
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
        set => SetProperty(ref _selectedExpenseType, value);
    }

    private ObservableCollection<ExpenseModel>? _expenses;
    public ObservableCollection<ExpenseModel>? Expenses
    {
        get => _expenses;
        set
        {
            SetProperty(ref _expenses, value);
            if (Expenses is null)
            {
                ExpenseCount = 0;
                ExpenseTotal = 0M;
                ShowGraphs = false;
            }
            else
            {
                ExpenseCount = Expenses.Count;
                ExpenseTotal = Expenses.Sum(x => x.Amount);
                ComputeGraphs();
                ShowGraphs = true;
            }
        }
    }

    private int _expenseCount;
    public int ExpenseCount
    {
        get => _expenseCount;
        set => SetProperty(ref _expenseCount, value);
    }

    private decimal _expenseTotal;
    public decimal ExpenseTotal
    {
        get => _expenseTotal;
        set => SetProperty(ref _expenseTotal, value);
    }

    private bool _allYears;
    public bool AllYears
    {
        get => _allYears;
        set => SetProperty(ref _allYears, value);
    }

    private bool _selectYear;
    public bool SelectYear
    {
        get => _selectYear;
        set => SetProperty(ref _selectYear, value);
    }

    private ObservableCollection<int>? _years;
    public ObservableCollection<int>? Years
    {
        get => _years;
        set => SetProperty(ref _years, value);
    }

    private int? _selectedYear;
    public int? SelectedYear
    {
        get => _selectedYear;
        set => SetProperty(ref _selectedYear, value);
    }

    private decimal _min;
    public decimal Min
    {
        get => _min;
        set => SetProperty(ref _min, value);
    }

    private decimal _max;
    public decimal Max
    {
        get => _max;
        set => SetProperty(ref _max, value);
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

    private RelayCommand? _resetCommand;

    public ICommand ResetCommand
    {
        get
        {
            if (_resetCommand is null)
            {
                _resetCommand = new(parm => ResetClick(), parm => AlwaysCanExecute());
            }
            return _resetCommand;
        }
    }

    private RelayCommand? _exportCommand;
    public ICommand ExportCommand
    {
        get
        {
            if (_exportCommand is null)
            {
                _exportCommand = new(parm => ExportClick(), parm => AlwaysCanExecute());
            }
            return _exportCommand;
        }
    }

    #endregion

    #region Command Methods

    private async Task FilterClick()
    {
        var year = AllYears ? 0 : SelectedYear ?? 0;
        var pid = SelectedPayee is null ? null : SelectedPayee.Id == "0" ? null : SelectedPayee.Id;
        var etid = SelectedExpenseType is null ? null : SelectedExpenseType.Id == "0" ? null : SelectedExpenseType.Id;
        var expenses = await _expenseService.GetExtendedAsync(year, pid, etid, Min, Max);
        Expenses = new(expenses);

    }

    private void ResetClick()
    {
        AllYears = true;
        SelectedYear = null;
        SelectedPayee = Payees!.SingleOrDefault(x => x.Id == "0");
        SelectedExpenseType = ExpenseTypes!.SingleOrDefault(x => x.Id == "0");
        Min = 0M;
        Max = 0M;
    }

    private void ExportClick()
    {
        if (Expenses is null || !Expenses.Any())
        {
            return;
        }
        var sfd = new SaveFileDialog
        {
            FileName = "XTrakr",
            DefaultExt = ".csv",
            Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        };
        if (sfd.ShowDialog() != true)
        {
            return;
        }
        var filename = sfd.FileName;
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            NewLine = Environment.NewLine,
            
        };
        using var writer = new StreamWriter(filename);
        using var csv = new CsvWriter(writer, config);
        csv.Context.RegisterClassMap<ExpenseMap>();
        try
        {
            csv.WriteRecords(Expenses);
        }
        catch (Exception ex)
        {
            PopupManager.Popup(ex.Innermost(), "Error Writing File", PopupButtons.Ok, PopupImage.Error);
            return;
        }
        writer.Flush();
        PopupManager.Popup("File created successfully", "Create Complete", PopupButtons.Ok, PopupImage.Information);
    }

    private async Task WindowLoaded()
    {
        Years = new(await _expenseService.GetYearsAsync());
        Payees = new((await _payeeService.GetAsync()).OrderBy(x => x.Name));
        Payees.Insert(0, new() { Id = "0", Name = "All Payees" });
        SelectedPayee = Payees.SingleOrDefault(x => x.Id == "0");
        ExpenseTypes = new((await _expenseTypeService.GetAsync()).OrderBy(x => x.Name));
        ExpenseTypes.Insert(0, new() { Id = "0", Name = "All Expense Types" });
        SelectedExpenseType = ExpenseTypes.SingleOrDefault(x => x.Id == "0");
        _expenses = new();
    }

    #endregion

    #region Utility Methods

    private void ComputeGraphs()
    {
        ExpensesByCategory = new();
        ExpensesByPayee = new();
        ShowGraphs = false;
        if (Expenses is null || !Expenses.Any())
        {
            return;
        }
        foreach (var expense in Expenses)
        {
            var payee = Payees!.SingleOrDefault(x => x.Id == expense.PayeeId);
            if (payee is not null)
            {
                var existing = ExpensesByPayee.SingleOrDefault(x => x.Name == payee.Name);
                if (existing is null)
                {
                    ExpensesByPayee.Add(new(payee.Name, expense.Amount));
                }
                else
                {
                    existing.Amount += expense.Amount;
                }
            }
            var expensetype = ExpenseTypes!.SingleOrDefault(x => x.Id == expense.ExpenseTypeId);
            if (expensetype is not null)
            {
                var existing = ExpensesByCategory.SingleOrDefault(x => x.Name == expensetype.Name);
                if (existing is null)
                {
                    ExpensesByCategory.Add(new(expensetype.Name, expense.Amount));
                }
                else
                {
                    existing.Amount += expense.Amount;
                }
            }
        }
        ShowGraphs = true;
    }

    #endregion

    public override void Reset()
    {
        base.Reset();
        Expenses = new();
        ExpensesByCategory = new();
        ExpensesByPayee = new();
        AllYears = true;
        SelectYear = false;
    }

    public DashboardViewModel(IExpenseService expenseService, IPayeeService payeeService, IExpenseTypeService expenseTypeService)
    {
        AllYears = true;
        SelectYear = false;
        _expensesByCategory = new();
        _expensesByPayee = new();
        _expenseService = expenseService;
        _payeeService = payeeService;
        _expenseTypeService = expenseTypeService;
    }
}
