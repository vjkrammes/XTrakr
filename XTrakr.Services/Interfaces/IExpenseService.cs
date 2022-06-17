
using XTrakr.Models;

namespace XTrakr.Services.Interfaces;
public interface IExpenseService : IDataService<ExpenseModel>
{
    Task<IEnumerable<ExpenseModel>> GetExtendedAsync(int year = 0, string? payeeid = null, string? expensetypeid = null, decimal min = 0M, decimal max = 0M);
    Task<IEnumerable<ExpenseModel>> GetForPayeeAsync(string payeeid);
    Task<IEnumerable<ExpenseModel>> GetForExpenseTypeAsync(string expensetypeid);
    Task<IEnumerable<ExpenseModel>> GetForDateAsync(DateTime date);
    Task<IEnumerable<ExpenseModel>> GetForDateRangeAsync(DateTime start, DateTime end);
    Task<IEnumerable<ExpenseModel>> GetForYearAsync(int year);
    Task<IEnumerable<int>> GetYearsAsync();
    Task<bool> PayeeHasExpensesAsync(string payeeid);
    Task<bool> ExpenseTypeHasExpensesAsync(string expensetypeid);
}
