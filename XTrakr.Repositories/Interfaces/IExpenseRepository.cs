
using XTrakr.Repositories.Entities;

namespace XTrakr.Repositories.Interfaces;
public interface IExpenseRepository : IRepository<ExpenseEntity>
{
    Task<IEnumerable<ExpenseEntity>> GetExtendedAsync(int year = 0, int payeeid = 0, int expensetypeid = 0, decimal min = 0M, decimal max = 0M);
    Task<IEnumerable<ExpenseEntity>> GetForPayeeAsync(int payeeid);
    Task<IEnumerable<ExpenseEntity>> GetForExpenseTypeAsync(int expensetypeid);
    Task<IEnumerable<ExpenseEntity>> GetForDateAsync(DateTime date);
    Task<IEnumerable<ExpenseEntity>> GetForDateRangeAsync(DateTime start, DateTime end);
    Task<IEnumerable<ExpenseEntity>> GetForYearAsync(int year);
    Task<IEnumerable<int>> GetYearsAsync();
    Task<bool> PayeeHasExpensesAsync(int payeeid);
    Task<bool> ExpenseTypeHasExpensesAsync(int expensetypeid);
}
