using Dapper.Contrib.Extensions;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using XTrakr.Common;
using XTrakr.Common.Attributes;
using XTrakr.Repositories.Interfaces;

namespace XTrakr.Repositories.Entities;

[Table("Payees")]
[BuildOrder(1)]
public class PayeeEntity : IIdEntity, ISqlEntity
{
    [Required]
    public int Id { get; set; }
    [Required, MaxLength(Constants.NameLength)]
    [Indexed]
    public string Name { get; set; }
    [Required]
    public string Address { get; set; }
    [Required]
    public string Description { get; set; }

    public PayeeEntity()
    {
        Id = 0;
        Name = string.Empty;
        Address = string.Empty;
        Description = string.Empty;
    }

    [JsonIgnore]
    [Write(false)]
    public static string Sql => "create table Payees(" +
        "Id integer constraint PkPayee primary key identity(1,1) not null, " +
        "Name nvarchar(50) not null, " +
        "Address nvarchar(max) not null, " +
        "Description nvarchar(max) not null, " +
        "Constraint UniqueName unique nonclustered (Name asc) " +
        ");";
}
