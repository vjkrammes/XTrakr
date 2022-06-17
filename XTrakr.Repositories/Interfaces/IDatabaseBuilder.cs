
using XTrakr.Repositories.Models;

namespace XTrakr.Repositories.Interfaces;
public interface IDatabaseBuilder
{
    void BuildDatabase(bool dropIfExists);
    (int order, string sql)[] Tables();
    (int order, string name)[] TableNames();
    (int order, List<IndexDefinition> indices)[] Indices();
}
