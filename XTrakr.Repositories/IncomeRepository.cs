using Dapper;

using System.Data;
using System.Data.SqlClient;

using XTrakr.Repositories.Entities;
using XTrakr.Repositories.Interfaces;
using XTrakr.Repositories.Models;

namespace XTrakr.Repositories;

public class IncomeRepository : RepositoryBase<IncomeEntity>, IIncomeRepository
{
    public IncomeRepository(IDatabase database) : base(database) { }

    public async Task<IEnumerable<IncomeEntity>> GetForContractAsync(int contractid) =>
        await GetAsync($"select * from Income where ContractId={contractid};");


    public async Task<IEnumerable<IncomeEntity>> GetForDateRangeAsync(DateTime start, DateTime end)
    {
        var sql = "Select * from Income where CAST(IncomeDate as DATE) >= CAST(@start as DATE) and CAST(IncomeDate as DATE) <= CAST(@end as DATE);";
        return await GetAsync(sql, new QueryParameter("start", start, DbType.DateTime2),
            new QueryParameter("end", end, DbType.DateTime2));
    }

    public async Task<bool> ContractHasIncomeAsync(int contractid)
    {
        var sql = $"select count(*) from Income where ContractId={contractid};";
        using var conn = new SqlConnection(ConnectionString);
        var count = 0;
        try
        {
            await conn.OpenAsync();
            count = await conn.ExecuteScalarAsync<int>(sql);
            return count > 0;
        }
        finally
        {
            await conn.CloseAsync();
        }
    }
}
