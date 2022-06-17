
using XTrakr.Repositories.Entities;
using XTrakr.Repositories.Interfaces;
using XTrakr.Repositories.Models;

namespace XTrakr.Repositories;
public class ExpenseTypeRepository : RepositoryBase<ExpenseTypeEntity>, IExpenseTypeRepository
{
    public ExpenseTypeRepository(IDatabase database) : base(database) { }

    public async Task<ExpenseTypeEntity?> ReadAsync(string name)
    {
        var sql = "select * from ExpenseTypes where Name=@name;";
        return await ReadAsync(sql, new QueryParameter("name", name));
    }
}
