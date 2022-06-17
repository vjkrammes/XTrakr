
using XTrakr.Repositories.Entities;
using XTrakr.Repositories.Interfaces;
using XTrakr.Repositories.Models;

namespace XTrakr.Repositories;
public class PayeeRepository : RepositoryBase<PayeeEntity>, IPayeeRepository
{
    public PayeeRepository(IDatabase database) : base(database) { }

    public async Task<PayeeEntity?> ReadAsync(string name)
    {
        var sql = "select * from Payees where Name=@name;";
        return await ReadAsync(sql, new QueryParameter("name", name));
    }
}
