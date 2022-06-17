
using XTrakr.Common;
using XTrakr.Models;
using XTrakr.Repositories.Entities;
using XTrakr.Repositories.Interfaces;
using XTrakr.Services.Interfaces;

namespace XTrakr.Services;
public sealed class ExpenseService : IExpenseService
{
    private readonly IExpenseRepository _expenseRepository;

    public ExpenseService(IExpenseRepository expenseRepository) => _expenseRepository = expenseRepository;

    public async Task<int> CountAsync() => await _expenseRepository.CountAsync();

    private static ApiError Validate(ExpenseModel model)
    {
        if (model is null)
        {
            return new(Strings.InvalidModel);
        }
        if (string.IsNullOrWhiteSpace(model.Id))
        {
            model.Id = IdEncoder.EncodeId(0);
        }
        var payeeid = IdEncoder.DecodeId(model.PayeeId);
        if (payeeid <= 0)
        {
            return new(string.Format(Strings.Invalid, "payee id"));
        }
        var expensetypeid = IdEncoder.DecodeId(model.ExpenseTypeId);
        if (expensetypeid <= 0)
        {
            return new(string.Format(Strings.Invalid, "expense type id"));
        }
        if (model.ExpenseDate == default)
        {
            model.ExpenseDate = DateTime.UtcNow;
        }
        if (model.Amount <= 0M)
        {
            return new(string.Format(Strings.Invalid, "amount"));
        }
        if (model.Reference is null)
        {
            model.Reference = string.Empty;
        }
        if (string.IsNullOrWhiteSpace(model.Description))
        {
            return new(string.Format(Strings.Required, "description"));
        }
        return ApiError.Success;
    }

    public async Task<ApiError> InsertAsync(ExpenseModel model)
    {
        var checkresult = Validate(model);
        if (!checkresult.Successful)
        {
            return checkresult;
        }
        ExpenseEntity entity = model!;
        try
        {
            var response = await _expenseRepository.InsertAsync(entity);
            if (response.Successful)
            {
                model.Id = IdEncoder.EncodeId(entity.Id);
            }
            return ApiError.FromDalResult(response);
        }
        catch (Exception ex)
        {
            return ApiError.FromException(ex);
        }
    }

    public async Task<ApiError> UpdateAsync(ExpenseModel model)
    {
        var checkresult = Validate(model);
        if (!checkresult.Successful)
        {
            return checkresult;
        }
        ExpenseEntity entity = model!;
        try
        {
            return ApiError.FromDalResult(await _expenseRepository.UpdateAsync(entity));
        }
        catch (Exception ex)
        {
            return ApiError.FromException(ex);
        }
    }

    public async Task<ApiError> DeleteAsync(ExpenseModel model)
    {
        if (model is null)
        {
            return new(Strings.InvalidModel);
        }
        var id = IdEncoder.DecodeId(model.Id);
        if (id <= 0)
        {
            return new(string.Format(Strings.NotFound, "expense", "id", model.Id));
        }
        try
        {
            return ApiError.FromDalResult(await _expenseRepository.DeleteAsync(id));
        }
        catch (Exception ex)
        {
            return ApiError.FromException(ex);
        }
    }

    private static IEnumerable<ExpenseModel> Finish(IEnumerable<ExpenseEntity> entities)
    {
        var models = entities.ToModels<ExpenseModel, ExpenseEntity>();
        models.ForEach(x => x.CanDelete = true);
        return models;
    }

    public async Task<IEnumerable<ExpenseModel>> GetExtendedAsync(int year = 0, string? payeeid = null, string? expensetypeid = null, decimal min = 0M, decimal max = 0M)
    {
        if ((min > 0M && max > 0M && max < min) || (min < 0) || max < 0)
        {
            return new List<ExpenseModel>();
        }
        var pid = string.IsNullOrWhiteSpace(payeeid) ? 0 : IdEncoder.DecodeId(payeeid);
        var etid = string.IsNullOrWhiteSpace(expensetypeid) ? 0 : IdEncoder.DecodeId(expensetypeid);
        var entities = await _expenseRepository.GetExtendedAsync(year, pid, etid, min, max);
        return Finish(entities);
    }

    public async Task<IEnumerable<ExpenseModel>> GetAsync()
    {
        var entities = await _expenseRepository.GetAsync();
        return Finish(entities);
    }

    public async Task<IEnumerable<ExpenseModel>> GetForPayeeAsync(string payeeid)
    {
        var pid = IdEncoder.DecodeId(payeeid);
        var entities = await _expenseRepository.GetForPayeeAsync(pid);
        return Finish(entities);
    }

    public async Task<IEnumerable<ExpenseModel>> GetForExpenseTypeAsync(string expensetypeid)
    {
        var pid = IdEncoder.DecodeId(expensetypeid);
        var entities = await _expenseRepository.GetForExpenseTypeAsync(pid);
        return Finish(entities);
    }

    public async Task<IEnumerable<ExpenseModel>> GetForDateAsync(DateTime date)
    {
        var entities = await _expenseRepository.GetForDateAsync(date);
        return Finish(entities);
    }

    public async Task<IEnumerable<ExpenseModel>> GetForDateRangeAsync(DateTime start, DateTime end)
    {
        var entities = await _expenseRepository.GetForDateRangeAsync(start, end);
        return Finish(entities);
    }

    public async Task<IEnumerable<ExpenseModel>> GetForYearAsync(int year)
    {
        var entities = await _expenseRepository.GetForYearAsync(year);
        return Finish(entities);
    }

    public async Task<IEnumerable<int>> GetYearsAsync() => await _expenseRepository.GetYearsAsync();

    public async Task<ExpenseModel?> ReadAsync(string id)
    {
        var pid = IdEncoder.DecodeId(id);
        var entity = await _expenseRepository.ReadAsync(pid);
        ExpenseModel model = entity!;
        if (model is not null)
        {
            model.CanDelete = true;
        }
        return model;
    }

    public async Task<bool> PayeeHasExpensesAsync(string payeeid) => await _expenseRepository.PayeeHasExpensesAsync(IdEncoder.DecodeId(payeeid));

    public async Task<bool> ExpenseTypeHasExpensesAsync(string expensetypeid) =>
        await _expenseRepository.ExpenseTypeHasExpensesAsync(IdEncoder.DecodeId(expensetypeid));

    public void Dispose()
    {
        
    }
}
