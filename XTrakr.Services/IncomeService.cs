
using XTrakr.Common;
using XTrakr.Models;
using XTrakr.Repositories.Entities;
using XTrakr.Repositories.Interfaces;
using XTrakr.Services.Interfaces;

namespace XTrakr.Services;
public sealed class IncomeService : IIncomeService
{
    private readonly IIncomeRepository _incomeRepository;

    public IncomeService(IIncomeRepository incomeRepository) => _incomeRepository = incomeRepository;

    public async Task<int> CountAsync() => await _incomeRepository.CountAsync();

    private static ApiError Validate(IncomeModel model, bool checkid = false)
    {
        if (model is null)
        {
            return new(Strings.InvalidModel);
        }
        if (model.AmountOwed < 0M)
        {
            return new(string.Format(Strings.Invalid, "amount owed"));
        }
        if (model.AmountPaid < 0M)
        {
            return new(string.Format(Strings.Invalid, "amount paid"));
        }
        if (string.IsNullOrWhiteSpace(model.Reference) && string.IsNullOrWhiteSpace(model.Description))
        {
            return new(string.Format(Strings.RequiredAlt, "Either a Reference or a Description", "is"));
        }
        if (model.IncomeDate == default)
        {
            model.IncomeDate = DateTime.UtcNow;
        }
        if (string.IsNullOrWhiteSpace(model.Id))
        {
            model.Id = IdEncoder.EncodeId(0);
        }
        if (checkid)
        {
            if (string.IsNullOrWhiteSpace(model.Id))
            {
                return new(string.Format(Strings.Invalid, "id"));
            }
            var pid = IdEncoder.DecodeId(model.Id);
            if (pid <= 0)
            {
                return new(string.Format(Strings.Invalid, "id"));
            }
        }
        return ApiError.Success;
    }

    public async Task<ApiError> InsertAsync(IncomeModel model)
    {
        var checkresult = Validate(model);
        if (!checkresult.Successful)
        {
            return checkresult;
        }
        IncomeEntity entity = model!;
        try
        {
            var result = await _incomeRepository.InsertAsync(entity);
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

    public async Task<ApiError> UpdateAsync(IncomeModel model)
    {
        var checkresult = Validate(model, true);
        if (!checkresult.Successful)
        {
            return checkresult;
        }
        IncomeEntity entity = model!;
        try
        {
            return ApiError.FromDalResult(await _incomeRepository.UpdateAsync(entity));
        }
        catch (Exception ex)
        {
            return ApiError.FromException(ex);
        }
    }

    public async Task<ApiError> DeleteAsync(IncomeModel model)
    {
        if (model is null)
        {
            return new(Strings.InvalidModel);
        }
        var decodedid = IdEncoder.DecodeId(model.Id);
        if (decodedid <= 0)
        {
            return new(string.Format(Strings.Invalid, "id"));
        }
        try
        {
            return ApiError.FromDalResult(await _incomeRepository.DeleteAsync(decodedid));
        }
        catch (Exception ex)
        {
            return ApiError.FromException(ex);
        }
    }

    private static IEnumerable<IncomeModel> Finish(IEnumerable<IncomeEntity> entities)
    {
        var models = entities.ToModels<IncomeModel, IncomeEntity>();
        models.ForEach(x => x.CanDelete = true);
        return models;
    }

    public async Task<IEnumerable<IncomeModel>> GetAsync()
    {
        var entities = await _incomeRepository.GetAsync();
        return Finish(entities);
    }

    public async Task<IEnumerable<IncomeModel>> GetForContractAsync(string contractid)
    {
        var cid = IdEncoder.DecodeId(contractid);
        var entities = await _incomeRepository.GetForContractAsync(cid);
        return Finish(entities);
    }

    public async Task<IEnumerable<IncomeModel>> GetForDateRangeAsync(DateTime start, DateTime end)
    {
        var entities = await _incomeRepository.GetForDateRangeAsync(start, end);
        return Finish(entities);
    }

    private static IncomeModel? Finish(IncomeEntity? entity)
    {
        IncomeModel model = entity!;
        model.CanDelete = true;
        return model;
    }

    public async Task<IncomeModel?> ReadAsync(string id)
    {
        var pid = IdEncoder.DecodeId(id);
        var entity = await _incomeRepository.ReadAsync(pid);
        return Finish(entity);
    }

    public async Task<bool> ContractHasIncomeAsync(string contractid)
    {
        var cid = IdEncoder.DecodeId(contractid);
        return await _incomeRepository.ContractHasIncomeAsync(cid);
    }

    public void Dispose()
    {

    }
}
