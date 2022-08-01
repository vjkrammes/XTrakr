
using XTrakr.Models;

namespace XTrakr.Services.Interfaces;
public interface IIncomeService : IDataService<IncomeModel>
{
    Task<IEnumerable<IncomeModel>> GetForContractAsync(string contractid);
    Task<IEnumerable<IncomeModel>> GetForDateRangeAsync(DateTime start, DateTime end);
    Task<bool> ContractHasIncomeAsync(string contractid);
}
