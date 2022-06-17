using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using XTrakr.Enumerations;
using XTrakr.Infrastructure;
using XTrakr.Models;
using XTrakr.Views;

namespace XTrakr.ViewModels;
public partial class MainViewModel
{
    private async Task WindowLoaded()
    {
        var now = DateTime.UtcNow;
        EndDate = now;
        StartDate = new DateTime(now.Year, 1, 1);
        await LoadExpenses();
        PayeesExist = await GetPayeeCount() > 0;
        ExpenseTypesExist = await GetExpenseTypeCount() > 0;
    }

    private static void ExitClick() => Application.Current.Shutdown();

    private async Task FilterClick()
    {
        await LoadExpenses();
    }

    private void ExpenseTypesClick()
    {
        DialogSupport.ShowDialog<ExpenseTypeWindow>(_expenseTypeViewModel, Application.Current.MainWindow);
    }

    private void PayeesClick()
    {
        DialogSupport.ShowDialog<ManagePayeesWindow>(_managePayeesViewModel, Application.Current.MainWindow);
    }

    private bool AddCanClick() => PayeesExist && ExpenseTypesExist;

    private async Task AddClick()
    {
        _expenseViewModel.Payees = new((await _payeeService.GetAsync()).OrderBy(x => x.Name));
        _expenseViewModel.ExpenseTypes = new((await _expenseTypeService.GetAsync()).OrderBy(x => x.Name));
        if (DialogSupport.ShowDialog<ExpenseWindow>(_expenseViewModel, Application.Current.MainWindow) != true)
        {
            return;
        }
        if (_expenseViewModel.SelectedPayee is null)
        {
            PopupManager.Popup("No Payee was selected", "Missing Payee", PopupButtons.Ok, PopupImage.Stop);
            return;
        }
        if (_expenseViewModel.SelectedExpenseType is null)
        {
            PopupManager.Popup("No Expense Type was selected", "Missing Expense Type", PopupButtons.Ok, PopupImage.Stop);
            return;
        }
        if (!decimal.TryParse(_expenseViewModel.Amount, out var amount) || amount <= 0)
        {
            PopupManager.Popup("Invalid Amount. Please enter a positive decimal amount", "Invalid Amount", PopupButtons.Ok, PopupImage.Stop);
            return;
        }
        var expense = new ExpenseModel
        {
            Id = string.Empty,
            PayeeId = _expenseViewModel.SelectedPayee.Id,
            ExpenseTypeId = _expenseViewModel.SelectedExpenseType.Id,
            ExpenseDate = _expenseViewModel.ExpenseDate ?? DateTime.Now,
            Amount = amount,
            Reference = _expenseViewModel.Reference ?? string.Empty,
            Description = _expenseViewModel.Description ?? string.Empty,
            Payee = _expenseViewModel.SelectedPayee,
            ExpenseType = _expenseViewModel.SelectedExpenseType,
            CanDelete = true
        };
        var response = await _expenseService.InsertAsync(expense);
        if (response.Successful)
        {
            var ix = 0;
            while (ix < Expenses!.Count && Expenses[ix] < expense)
            {
                ix++;
            }
            Expenses!.Insert(ix, expense);
            if (StartDate > expense.ExpenseDate)
            {
                StartDate = expense.ExpenseDate;
            }
            if (EndDate < expense.ExpenseDate)
            {
                EndDate = expense.ExpenseDate;
            }
            return;
        }
        PopupManager.Popup(response.Message!, "Database Error Adding Expense", PopupButtons.Ok, PopupImage.Error);
    }

    private bool ExpenseSelected() => SelectedExpense is not null;

    private async Task EditClick()
    {
        if (SelectedExpense is not null)
        {
            _expenseViewModel.ExpenseDate = SelectedExpense.ExpenseDate;
            _expenseViewModel.Amount = SelectedExpense.Amount.ToString("n2");
            _expenseViewModel.Reference = SelectedExpense.Reference;
            _expenseViewModel.Description = SelectedExpense.Description;
            _expenseViewModel.SelectedExpenseType = SelectedExpense.ExpenseType;
            _expenseViewModel.SelectedPayee = SelectedExpense.Payee;
            var result = DialogSupport.ShowDialog<ExpenseWindow>(_expenseViewModel, Application.Current.MainWindow);
            if (result == false)
            {
                SelectedExpense = null;
                return;
            }
            if (_expenseViewModel.SelectedPayee is null)
            {
                PopupManager.Popup("No Payee was selected", "Missing Payee", PopupButtons.Ok, PopupImage.Stop);
                return;
            }
            if (_expenseViewModel.SelectedExpenseType is null)
            {
                PopupManager.Popup("No Expense Type was selected", "Missing Expense Type", PopupButtons.Ok, PopupImage.Stop);
                return;
            }
            if (!decimal.TryParse(_expenseViewModel.Amount, out var amount) || amount <= 0)
            {
                PopupManager.Popup("Invalid Amount. Please enter a positive decimal amount", "Invalid Amount", PopupButtons.Ok, PopupImage.Stop);
                return;
            }
            var expense = new ExpenseModel
            {
                Id = SelectedExpense.Id,
                ExpenseTypeId = _expenseViewModel.SelectedExpenseType.Id,
                PayeeId = _expenseViewModel.SelectedPayee.Id,
                ExpenseDate = _expenseViewModel.ExpenseDate ?? DateTime.Now,
                Amount = amount,
                Reference = _expenseViewModel.Reference ?? string.Empty,
                Description = _expenseViewModel.Description ?? string.Empty,
                ExpenseType = _expenseViewModel.SelectedExpenseType,
                Payee = _expenseViewModel.SelectedPayee,
                CanDelete = SelectedExpense.CanDelete
            };
            var response = await _expenseService.UpdateAsync(expense);
            if (response.Successful)
            {
                Expenses!.Remove(SelectedExpense);
                var ix = 0;
                while (ix < Expenses!.Count && Expenses[ix] < expense)
                {
                    ix++;
                }
                Expenses!.Insert(ix, expense);
                if (StartDate > expense.ExpenseDate)
                {
                    StartDate = expense.ExpenseDate;
                }
                if (EndDate < expense.ExpenseDate)
                {
                    EndDate = expense.ExpenseDate;
                }
                return;
            }
            PopupManager.Popup(response.Message!, "Database Error Updating Expense", PopupButtons.Ok, PopupImage.Error);
        }
    }

    private void DeselectClick() => SelectedExpense = null;

    private async Task DeleteClick()
    {
        if (SelectedExpense is not null)
        {
            var response = await _expenseService.DeleteAsync(SelectedExpense);
            if (response.Successful)
            {
                Expenses!.Remove(SelectedExpense);
                SelectedExpense = null;
                return;
            }
            PopupManager.Popup(response.Message!, "Database Error Deleting Expense", PopupButtons.Ok, PopupImage.Error);
        }
    }

    private void DashboardClick()
    {
        DialogSupport.ShowDialog<DashboardWindow>(_dashboardViewModel, Application.Current.MainWindow);
    }

    private void AboutClick()
    {
        DialogSupport.ShowDialog<AboutWindow>(_aboutViewModel, Application.Current.MainWindow);
    }
}
