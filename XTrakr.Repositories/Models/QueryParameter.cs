using System.Data;

namespace XTrakr.Repositories.Models;
public class QueryParameter
{
    public string Name { get; set; }
    public DbType Type { get; set; }
    public object? Value { get; set; }

    public QueryParameter()
    {
        Name = string.Empty;
        Type = DbType.String;
        Value = null;
    }


    public QueryParameter(string name, object? value, DbType type = DbType.String)
    {
        Name = name;
        Value = value;
        Type = type;
    }
}
