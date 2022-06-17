using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using XTrakr.Infrastructure;
using XTrakr.Models;
using XTrakr.Services.Interfaces;

namespace XTrakr.ViewModels;

public class ExpenseViewModel : ViewModelBase
{
    #region Properties

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

    public ExpenseTypeModel? _selectedExpenseType;
    public ExpenseTypeModel? SelectedExpenseType
    {
        get => _selectedExpenseType;
        set => SetProperty(ref _selectedExpenseType, value);
    }

    private DateTime? _expenseDate;
    public DateTime? ExpenseDate
    {
        get => _expenseDate;
        set => SetProperty(ref _expenseDate, value);
    }

    private string? _amount;
    public string? Amount
    {
        get => _amount;
        set => SetProperty(ref _amount, value);
    }

    private string? _reference;
    public string? Reference
    {
        get => _reference;
        set => SetProperty(ref _reference, value);
    }

    private string? _description;
    public string? Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    #endregion

    public ExpenseViewModel() => ExpenseDate = DateTime.Now;
}
