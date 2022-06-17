using Dapper.Contrib.Extensions;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using XTrakr.Common;
using XTrakr.Common.Attributes;
using XTrakr.Repositories.Interfaces;

namespace XTrakr.Repositories.Entities;

[Table("ExpenseTypes")]
[BuildOrder(2)]
public class ExpenseTypeEntity : IIdEntity, ISqlEntity
{
    [Required]
    public int Id { get; set; }
    [Required, MaxLength(Constants.NameLength)]
    [Indexed]
    public string Name { get; set; }
    [Required, MaxLength(Constants.NameLength)]
    public string Background { get; set; }
    [Required, MaxLength(Constants.UriLength)]
    public string Icon { get; set; }
    [Required]
    public long ARGB { get; set; }

    public ExpenseTypeEntity()
    {
        Id = 0;
        Name = string.Empty;
        Background = "White";
        Icon = string.Empty;
        ARGB = 0xFFFFFFFF;
    }

    public override string ToString() => Name;

    [JsonIgnore]
    [Write(false)]
    public static string Sql => "create table ExpenseTypes (" +
        "Id integer constraint PkExpenseType primary key identity(1,1) not null, " +
        "Name nvarchar(50) not null, " +
        "Background nvarchar(50) not null, " +
        "Icon nvarchar(256) not null, " +
        "ARGB bigint default ((0)) not null, " +
        "Constraint UniqueExpenseType unique nonclustered (Name asc) " +
        ");";
}
