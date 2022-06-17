
using XTrakr.Repositories.Entities;

namespace XTrakr.Repositories.Interfaces;
public interface IExpenseTypeRepository : IRepository<ExpenseTypeEntity>
{
    Task<ExpenseTypeEntity?> ReadAsync(string name);
}
