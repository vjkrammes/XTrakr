using Dapper.Contrib.Extensions;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using XTrakr.Common;
using XTrakr.Common.Attributes;
using XTrakr.Repositories.Interfaces;

namespace XTrakr.Repositories.Entities;

[Table("Income")]
[BuildOrder(4)]
public class IncomeEntity : IIdEntity, ISqlEntity
{
    [Required]
    public int Id { get; set; }
    [Required, Indexed]
    public int ContractId { get; set; }
    [Required, Indexed]
    public DateTime IncomeDate { get; set; }
    [Required]
    public decimal AmountOwed { get; set; }
    [Required]
    public decimal AmountPaid { get; set; }
    [Required, MaxLength(Constants.NameLength)]
    public string Reference { get; set; }
    [Required]
    public string Description { get; set; }

    public IncomeEntity()
    {
        Id = 0;
        ContractId = 0;
        IncomeDate = DateTime.UtcNow;
        AmountOwed = 0M;
        AmountPaid = 0M;
        Reference = string.Empty;
        Description = string.Empty;
    }

    [JsonIgnore]
    [Write(false)]
    public static string Sql => "create table Income(" +
        "Id integer constraint PkIncome primary key identity (1,1) not null, " +
        "ContractId integer default((0))  not null, " +
        "IncomeDate datetime2 not null, " +
        "AmountOwed decimal(11,2) default((0)) not null, " +
        "AmountPaid decimal(11,2) default((0)) not null, " +
        "Reference nvarchar(50) not null, " +
        "Description nvarchar(max) not null " +
        ");";
}
