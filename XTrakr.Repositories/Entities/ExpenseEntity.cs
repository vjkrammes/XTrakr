using Dapper.Contrib.Extensions;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using XTrakr.Common;
using XTrakr.Common.Attributes;
using XTrakr.Repositories.Interfaces;

namespace XTrakr.Repositories.Entities;

[Table("Expenses")]
[BuildOrder(3)]
public class ExpenseEntity : IIdEntity, ISqlEntity
{
    [Required]
    public int Id { get; set; }
    [Required]
    [Indexed]
    public int PayeeId { get; set; }
    [Required]
    [Indexed]
    public int ExpenseTypeId { get; set; }
    [Required]
    [Indexed]
    public DateTime ExpenseDate { get; set; }
    [Required]
    public decimal Amount { get; set; }
    [Required, MaxLength(Constants.NameLength)]
    public string Reference { get; set; }
    [Required]
    public string Description { get; set; }

    [JsonIgnore]
    [Write(false)]
    public PayeeEntity? Payee { get; set; }

    [JsonIgnore]
    [Write(false)]
    public ExpenseTypeEntity? ExpenseType { get; set; }

    public ExpenseEntity()
    {
        Id = 0;
        PayeeId = 0;
        ExpenseTypeId = 0;
        ExpenseDate = DateTime.UtcNow;
        Amount = 0M;
        Reference = string.Empty;
        Description = string.Empty;
        Payee = null;
        ExpenseType = null;
    }

    [JsonIgnore]
    [Write(false)]
    public static string Sql => "create table Expenses (" +
        "Id integer constraint PkExpense primary key identity(1,1) not null, " +
        "PayeeId integer not null, " +
        "ExpenseTypeId integer not null, " +
        "ExpenseDate datetime2 not null, " +
        "Amount decimal(11,2) default((0)) not null, " +
        "Reference nvarchar(50) not null, " +
        "Description nvarchar(max) not null, " +
        "constraint FKExpensePayee foreign key (PayeeId) references Payees(Id), " +
        "constraint FkExpenseType foreign key (ExpenseTypeId) references ExpenseTypes(Id) " +
        ");";
}
