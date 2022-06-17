
using XTrakr.Models;

namespace XTrakr.Services.Interfaces;
public interface IPayeeService : IDataService<PayeeModel>
{
    Task<PayeeModel?> ReadForNameAsync(string name);
}
