using System;
using System.Linq;
using System.Threading.Tasks;

using XTrakr.Services.Interfaces;

namespace XTrakr.ViewModels;
public partial class MainViewModel
{
    private async Task LoadExpenses()
    {
        var expenses = await _expenseService.GetForDateRangeAsync(StartDate ?? default, EndDate ?? DateTime.MaxValue);
        if (expenses is not null)
        {
            Expenses = new(expenses.OrderBy(x => x.ExpenseDate));
        }
    }

    private async Task<int> GetPayeeCount()
    {
            return await _payeeService.CountAsync();
    }

    private async Task<int> GetExpenseTypeCount()
    {
            return await _expenseTypeService.CountAsync();
    }
}
