using Dapper;

using System.Data;
using System.Data.SqlClient;
using System.Text;

using XTrakr.Repositories.Entities;
using XTrakr.Repositories.Interfaces;
using XTrakr.Repositories.Models;

namespace XTrakr.Repositories;
public class ExpenseRepository : RepositoryBase<ExpenseEntity>, IExpenseRepository
{
    private readonly IPayeeRepository _payeeRepository;
    private readonly IExpenseTypeRepository _expenseTypeRepository;

    public ExpenseRepository(IDatabase database, IPayeeRepository payeeRepository, IExpenseTypeRepository expenseTypeRepository) : base(database)
    {
        _payeeRepository = payeeRepository;
        _expenseTypeRepository = expenseTypeRepository;
    }

    public override async Task<IEnumerable<ExpenseEntity>> GetAsync(string sql, params QueryParameter[] parameters)
    {
        using var conn = new SqlConnection(ConnectionString);
        try
        {
            await conn.OpenAsync();
            var entities = await base.GetAsync(sql, parameters);
            if (entities is not null && entities.Any())
            {
                foreach (var entity in entities)
                {
                    entity.Payee = await _payeeRepository.ReadAsync(entity.PayeeId);
                    entity.ExpenseType = await _expenseTypeRepository.ReadAsync(entity.ExpenseTypeId);
                }
            }
            return entities ?? new List<ExpenseEntity>();
        }
        finally
        {
            await conn.CloseAsync();
        }
    }

    public async Task<IEnumerable<ExpenseEntity>> GetExtendedAsync(int year = 0, int payeeid = 0, int expensetypeid = 0, decimal min = 0M, decimal max = 0M)
    {
        const string and = " and";
        if (year == 0 && payeeid == 0 && expensetypeid == 0 && min == 0M && max == 0M)
        {
            return await GetAsync();
        }
        var sb = new StringBuilder("select * from Expenses where");
        bool andNeeded = false;
        if (year != 0)
        {
            sb.Append($" YEAR(ExpenseDate) = {year}");
            andNeeded = true;
        }
        if (payeeid != 0)
        {
            if (andNeeded)
            {
                sb.Append(and);
            }
            sb.Append($" PayeeId = {payeeid}");
            andNeeded = true;
        }
        if (expensetypeid != 0)
        {
            if (andNeeded)
            {
                sb.Append(and);
            }
            sb.Append($" ExpenseTypeId = {expensetypeid}");
            andNeeded = true;
        }
        if (min > 0)
        {
            if (andNeeded)
            {
                sb.Append(and);
            }
            sb.Append($" Amount >= {min}");
            andNeeded = true;
        }
        if (max > 0)
        {
            if (andNeeded)
            {
                sb.Append(and);
            }
            sb.Append($" Amount <= {max}");
        }
        sb.Append(';');
        var sql = sb.ToString();
        return await GetAsync(sql);
    }

    public async Task<IEnumerable<ExpenseEntity>> GetForPayeeAsync(int payeeid)
    {
        var sql = $"select * from Expenses where PayeeId={payeeid};";
        return await GetAsync(sql);
    }

    public async Task<IEnumerable<ExpenseEntity>> GetForExpenseTypeAsync(int expensetypeid)
    {
        var sql = $"select * from Expenses where ExpenseTypeId={expensetypeid};";
        return await GetAsync(sql);
    }

    public async Task<IEnumerable<ExpenseEntity>> GetForDateAsync(DateTime date)
    {
        var sql = "select * from Expenses where CAST(ExpenseDate as DATE) = CAST(@date as DATE);";
        return await GetAsync(sql, new QueryParameter("date", date.Date, DbType.Date));
    }

    public async Task<IEnumerable<ExpenseEntity>> GetForDateRangeAsync(DateTime start, DateTime end)
    {
        var sql = "select * from Expenses where CAST(ExpenseDate as DATE) >= CAST(@start as DATE) and CAST(ExpenseDate as DATE) <= CAST(@end as DATE);";
        return await GetAsync(sql, new QueryParameter("start", start.Date, DbType.Date), 
            new QueryParameter("end", end.Date, DbType.Date));
    }

    public async Task<IEnumerable<ExpenseEntity>> GetForYearAsync(int year)
    {
        var sql = $"select * from Expenses where YEAR(ExpenseDate) = {year};";
        return await GetAsync(sql);
    }

    public async Task<IEnumerable<int>> GetYearsAsync()
    {
        var sql = $"select distinct YEAR(ExpenseDate) from Expenses;";
        using var conn = new SqlConnection(ConnectionString);
        try
        {
            await conn.OpenAsync();
            var ret = await conn.QueryAsync<int>(sql);
            return ret;
        }
        finally
        {
            await conn.CloseAsync();
        }
    }

    private async Task<int> GetCountAsync(string sql)
    {
        using var conn = new SqlConnection(ConnectionString);
        try
        {
            await conn.OpenAsync();
            var ret = await conn.ExecuteScalarAsync<int>(sql);
            return ret;
        }
        catch
        {
            return 0;
        }
        finally
        {
            await conn.CloseAsync();
        }
    }

    public async Task<bool> PayeeHasExpensesAsync(int payeeid)
    {
        var sql = $"select count(*) from Expenses where PayeeId={payeeid};";
        return await GetCountAsync(sql) > 0;
    }

    public async Task<bool> ExpenseTypeHasExpensesAsync(int expensetypeid)
    {
        var sql = $"select count(*) from Expenses where ExpenseTypeId={expensetypeid};";
        return await GetCountAsync(sql) > 0;
    }
}
