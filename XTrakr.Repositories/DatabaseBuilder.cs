using Dapper.Contrib.Extensions;

using System.Reflection;

using XTrakr.Common;
using XTrakr.Common.Attributes;
using XTrakr.Repositories.Interfaces;
using XTrakr.Repositories.Models;

namespace XTrakr.Repositories;
public class DatabaseBuilder : IDatabaseBuilder
{
    private readonly Dictionary<int, string> _tables = new();
    private readonly Dictionary<int, string> _tableNames = new();
    private readonly Dictionary<int, List<IndexDefinition>> _indices = new();
    private readonly IDatabase _database;

    public DatabaseBuilder(IDatabase database)
    {
        _database = database;
        LoadTables();
    }

    public void BuildDatabase(bool dropIfExists)
    {
        if (dropIfExists && _database.DatabaseExists())
        {
            _database.DropDatabase();
        }
        if (!_database.DatabaseExists())
        {
            _database.CreateDatabase();
        }
        if (_database.DatabaseExists())
        {
            _tables.OrderBy(x => x.Key).ForEach(x => _database.CreateTable(_tableNames[x.Key], x.Value));
            _indices
                .OrderBy(x => x.Key)
                .ForEach(x => _database.CreateIndices(_tableNames[x.Key], _indices[x.Key]));
        }
    }

    public (int order, string sql)[] Tables()
    {
        List<(int, string)> ret = new();
        _tables.ForEach(x => ret.Add(new(x.Key, x.Value)));
        return ret.ToArray();
    }

    public (int order, string name)[] TableNames()
    {
        List<(int, string)> ret = new();
        _tableNames.ForEach(x => ret.Add(new(x.Key, x.Value)));
        return ret.ToArray();
    }

    public (int order, List<IndexDefinition> indices)[] Indices()
    {
        List<(int order, List<IndexDefinition>)> ret = new();
        _indices.ForEach(x => ret.Add(new(x.Key, x.Value)));
        return ret.ToArray();
    }

    private void LoadTables()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var type = typeof(ISqlEntity);
        var types = assembly
            .GetTypes()
            .Where(x => type.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
            .ToList();
        if (types is not null && types.Any())
        {
            types.ForEach(x =>
            {
                var obj = Activator.CreateInstance(x);
                var sqlprop = x.GetProperty("Sql", BindingFlags.Public | BindingFlags.Static);
                if (sqlprop is not null)
                {
                    var buildOrder = (x.GetCustomAttribute(typeof(BuildOrderAttribute), false) as BuildOrderAttribute)?.BuildOrder ?? 0;
                    if (buildOrder > 0)
                    {
                        _indices[buildOrder] = new();
                        if (_tableNames.ContainsKey(buildOrder))
                        {
                            throw new InvalidOperationException($"Multiple tables with build order '{buildOrder}'");
                        }
                        var sql = sqlprop.GetValue(obj) as string;
                        if (sql is not null)
                        {
                            _tables.Add(buildOrder, sql);
                        }
                        var tableName = (x.GetCustomAttribute(typeof(TableAttribute), false) as TableAttribute)?.Name ?? string.Empty;
                        if (string.IsNullOrWhiteSpace(tableName))
                        {
                            throw new InvalidOperationException($"No Table attribute found on class '{x.Name}'");
                        }
                        _tableNames.Add(buildOrder, tableName);
                        var properties = x.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                        foreach (var property in properties)
                        {
                            var attr = property.GetCustomAttribute(typeof(IndexedAttribute), false) as IndexedAttribute;
                            if (attr is not null)
                            {
                                var index = new IndexDefinition
                                {
                                    ColumnName = property.Name,
                                    IndexName = string.IsNullOrWhiteSpace(attr.IndexName) ? $"Ix{property.Name}" : attr.IndexName
                                };
                                _indices[buildOrder].Add(index);
                            }
                        }
                    }
                }
            });
        }
    }
}
