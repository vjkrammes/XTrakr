
using XTrakr.Repositories.Entities;

namespace XTrakr.Repositories.Interfaces;
public interface IIncomeRepository : IRepository<IncomeEntity>
{
    Task<IEnumerable<IncomeEntity>> GetForContractAsync(int contractid);
    Task<IEnumerable<IncomeEntity>> GetForDateRangeAsync(DateTime start, DateTime end);
    Task<bool> ContractHasIncomeAsync(int contractid);
}
