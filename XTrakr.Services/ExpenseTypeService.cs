
using XTrakr.Common;
using XTrakr.Models;
using XTrakr.Repositories.Entities;
using XTrakr.Repositories.Interfaces;
using XTrakr.Services.Interfaces;

namespace XTrakr.Services;
public sealed class ExpenseTypeService : IExpenseTypeService
{
    private readonly IExpenseTypeRepository _expenseTypeRepository;
    private readonly IExpenseRepository _expenseRepository;

    public ExpenseTypeService(IExpenseTypeRepository expenseTypeRepository, IExpenseRepository expenseRepository)
    {
        _expenseTypeRepository = expenseTypeRepository;
        _expenseRepository = expenseRepository;
    }

    public async Task<int> CountAsync() => await _expenseTypeRepository.CountAsync();

    private async Task<ApiError> ValidateAsync(ExpenseTypeModel model, bool checkid = false, bool update = false)
    {
        if (model is null || string.IsNullOrWhiteSpace(model.Name))
        {
            return new(Strings.InvalidModel);
        }
        if (string.IsNullOrWhiteSpace(model.Background))
        {
            model.Background = "White";
            model.ARGB = 0xFFFFFFFF;
        }
        if (model.Icon is null)
        {
            model.Icon = string.Empty;
        }
        if (string.IsNullOrWhiteSpace(model.Id))
        {
            model.Id = IdEncoder.EncodeId(0);
        }
        if (checkid)
        {
            var decodedid = IdEncoder.DecodeId(model.Id);
            if (decodedid <= 0)
            {
                return new(string.Format(Strings.Invalid, "id"));
            }
        }
        var existing = await _expenseTypeRepository.ReadAsync(model.Name);
        if (update)
        {
            if (existing is not null && existing.Id != IdEncoder.DecodeId(model.Id))
            {
                return new(string.Format(Strings.Duplicate, "expense type", "name", model.Name));
            }
        }
        else if (existing is not null)
        {
            return new(string.Format(Strings.Duplicate, "expense type", "name", model.Name));
        }
        return ApiError.Success;
    }

    public async Task<ApiError> InsertAsync(ExpenseTypeModel model)
    {
        var checkresult = await ValidateAsync(model);
        if (!checkresult.Successful)
        {
            return checkresult;
        }
        ExpenseTypeEntity entity = model!;
        try
        {
            var result = await _expenseTypeRepository.InsertAsync(entity);
            if (result.Successful)
            {
                model.Id = IdEncoder.EncodeId(entity.Id);
            }
            return ApiError.FromDalResult(result);
        }
        catch (Exception ex)
        {
            return ApiError.FromException(ex);
        }
    }

    public async Task<ApiError> UpdateAsync(ExpenseTypeModel model)
    {
        var checkresult = await ValidateAsync(model, true, true);
        if (!checkresult.Successful)
        {
            return checkresult;
        }
        ExpenseTypeEntity entity = model!;
        try
        {
            return ApiError.FromDalResult(await _expenseTypeRepository.UpdateAsync(entity));
        }
        catch (Exception ex)
        {
            return ApiError.FromException(ex);
        }
    }

    public async Task<ApiError> DeleteAsync(ExpenseTypeModel model)
    {
        if (model is null)
        {
            return new(Strings.InvalidModel);
        }
        if (!model.CanDelete)
        {
            return new(string.Format(Strings.CantDelete, "expense type", "expenses"));
        }
        var decodedid = IdEncoder.DecodeId(model.Id);
        if (decodedid <= 0)
        {
            return new(string.Format(Strings.NotFound, "expense type", "id", model.Id));
        }
        try
        {
            return ApiError.FromDalResult(await _expenseTypeRepository.DeleteAsync(decodedid));
        }
        catch (Exception ex)
        {
            return ApiError.FromException(ex);
        }
    }

    private async Task<IEnumerable<ExpenseTypeModel>> FinishAsync(IEnumerable<ExpenseTypeEntity> entities)
    {
        var models = entities.ToModels<ExpenseTypeModel, ExpenseTypeEntity>();
        foreach (var model in models)
        {
            model.CanDelete = !await _expenseRepository.ExpenseTypeHasExpensesAsync(IdEncoder.DecodeId(model.Id));
        }
        return models;
    }

    public async Task<IEnumerable<ExpenseTypeModel>> GetAsync()
    {
        var entities = await _expenseTypeRepository.GetAsync();
        return await FinishAsync(entities);
    }

    private async Task<ExpenseTypeModel?> FinishAsync(ExpenseTypeEntity? entity)
    {
        ExpenseTypeModel model = entity!;
        if (model is not null)
        {
            model.CanDelete = !await _expenseRepository.ExpenseTypeHasExpensesAsync(IdEncoder.DecodeId(model.Id));
        }
        return model;
    }

    public async Task<ExpenseTypeModel?> ReadAsync(string id)
    {
        var entity = await _expenseTypeRepository.ReadAsync(IdEncoder.DecodeId(id));
        return await FinishAsync(entity);
    }

    public async Task<ExpenseTypeModel?> ReadForNameAsync(string name)
    {
        var entity = await _expenseTypeRepository.ReadAsync(name);
        return await FinishAsync(entity);
    }

    public void Dispose()
    {
        
    }
}
