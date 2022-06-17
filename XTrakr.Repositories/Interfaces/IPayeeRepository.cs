
using XTrakr.Repositories.Entities;

namespace XTrakr.Repositories.Interfaces;
public interface IPayeeRepository : IRepository<PayeeEntity>
{
    Task<PayeeEntity?> ReadAsync(string name);
}
