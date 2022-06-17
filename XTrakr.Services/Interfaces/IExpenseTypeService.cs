
using XTrakr.Models;

namespace XTrakr.Services.Interfaces;
public interface IExpenseTypeService : IDataService<ExpenseTypeModel>
{
    Task<ExpenseTypeModel?> ReadForNameAsync(string name);
}
