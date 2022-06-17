
using XTrakr.Common;
using XTrakr.Models;

namespace XTrakr.Services.Interfaces;
public interface IDataService<TModel> : IDisposable where TModel : ModelBase
{
    Task<int> CountAsync();
    Task<ApiError> InsertAsync(TModel model);
    Task<ApiError> UpdateAsync(TModel model);
    Task<ApiError> DeleteAsync(TModel model);
    Task<IEnumerable<TModel>> GetAsync();
    Task<TModel?> ReadAsync(string id);
}
