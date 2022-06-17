
using XTrakr.Common;
using XTrakr.Models;
using XTrakr.Repositories.Entities;
using XTrakr.Repositories.Interfaces;
using XTrakr.Services.Interfaces;

namespace XTrakr.Services;
public sealed class PayeeService : IPayeeService
{
    private readonly IPayeeRepository _payeeRepository;
    private readonly IExpenseRepository _expenseRepository;

    public PayeeService(IPayeeRepository payeeRepository, IExpenseRepository expenseRepository)
    {
        _payeeRepository = payeeRepository;
        _expenseRepository = expenseRepository;
    }

    public async Task<int> CountAsync() => await _payeeRepository.CountAsync();

    private async Task<ApiError> ValidateAsync(PayeeModel model, bool checkid = false, bool update = false)
    {
        if (model is null || string.IsNullOrWhiteSpace(model.Name))
        {
            return new(Strings.InvalidModel);
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
        var existing = await _payeeRepository.ReadAsync(model.Name);
        if (update)
        {
            if (existing is not null && existing.Id != IdEncoder.DecodeId(model.Id))
            {
                return new(string.Format(Strings.Duplicate, "payee", "name", model.Name));
            }
        }
        else if (existing is not null)
        {
            return new(string.Format(Strings.Duplicate, "payee", "name", model.Name));
        }
        return ApiError.Success;
    }

    public async Task<ApiError> InsertAsync(PayeeModel model)
    {
        var checkresult = await ValidateAsync(model);
        if (!checkresult.Successful)
        {
            return checkresult;
        }
        PayeeEntity entity = model!;
        try
        {
            var result = await _payeeRepository.InsertAsync(entity);
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

    public async Task<ApiError> UpdateAsync(PayeeModel model)
    {
        var checkresult = await ValidateAsync(model, true, true);
        if (!checkresult.Successful)
        {
            return checkresult;
        }
        PayeeEntity entity = model!;
        try
        {
            return ApiError.FromDalResult(await _payeeRepository.UpdateAsync(entity));
        }
        catch (Exception ex)
        {
            return ApiError.FromException(ex);
        }
    }

    public async Task<ApiError> DeleteAsync(PayeeModel model)
    {
        if (model is null)
        {
            return new(Strings.InvalidModel);
        }
        if (!model.CanDelete)
        {
            return new(string.Format(Strings.CantDelete, "payee", "expenses"));
        }
        var decodedid = IdEncoder.DecodeId(model.Id);
        if (decodedid <= 0)
        {
            return new(string.Format(Strings.NotFound, "payee", "id", model.Id));
        }
        try
        {
            return ApiError.FromDalResult(await _payeeRepository.DeleteAsync(decodedid));
        }
        catch (Exception ex)
        {
            return ApiError.FromException(ex);
        }
    }

    private async Task<IEnumerable<PayeeModel>> Finish(IEnumerable<PayeeEntity> entities)
    {
        var models = entities.ToModels<PayeeModel, PayeeEntity>();
        foreach (var model in models)
        {
            model.CanDelete = !await _expenseRepository.PayeeHasExpensesAsync(IdEncoder.DecodeId(model.Id));
        }
        return models;
    }

    public async Task<IEnumerable<PayeeModel>> GetAsync()
    {
        var entities = await _payeeRepository.GetAsync();
        return await Finish(entities);
    }

    private async Task<PayeeModel?> Finish(PayeeEntity? entity)
    {
        PayeeModel model = entity!;
        if (model is not null)
        {
            model.CanDelete = !await _expenseRepository.PayeeHasExpensesAsync(IdEncoder.DecodeId(model.Id));
        }
        return model;
    }

    public async Task<PayeeModel?> ReadAsync(string id)
    {
        var entity = await _payeeRepository.ReadAsync(IdEncoder.DecodeId(id));
        return await Finish(entity);
    }

    public async Task<PayeeModel?> ReadForNameAsync(string name)
    {
        var entity = await _payeeRepository.ReadAsync(name);
        return await Finish(entity);
    }

    public void Dispose()
    {
        
    }
}
