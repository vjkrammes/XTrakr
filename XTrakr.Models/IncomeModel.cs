
using XTrakr.Common;
using XTrakr.Repositories.Entities;

namespace XTrakr.Models;
public class IncomeModel : ModelBase, IEquatable<IncomeModel>
{
    public string Id { get; set; }
    public string ContractId { get; set; }
    public DateTime IncomeDate { get; set; }
    public decimal AmountOwed { get; set; }
    public decimal AmountPaid { get; set; }
    public string Reference { get; set; }
    public string Description { get; set; }

    public IncomeModel() : base()
    {
        Id = IdEncoder.EncodeId(0);
        ContractId = IdEncoder.EncodeId(0);
        IncomeDate = DateTime.UtcNow;
        AmountOwed = 0M;
        AmountPaid = 0M;
        Reference = string.Empty;
        Description = string.Empty;
    }

    public static IncomeModel? FromEntity(IncomeEntity entity) => entity is null ? null : new()
    {
        Id = IdEncoder.EncodeId(entity.Id),
        ContractId = IdEncoder.EncodeId(entity.ContractId),
        IncomeDate = entity.IncomeDate,
        AmountOwed = entity.AmountOwed,
        AmountPaid = entity.AmountPaid,
        Reference = entity.Reference ?? string.Empty,
        Description = entity.Description ?? string.Empty,
        CanDelete = true
    };

    public static IncomeEntity? FromModel(IncomeModel model) => model is null ? null : new()
    {
        Id = IdEncoder.DecodeId(model.Id),
        ContractId = IdEncoder.DecodeId(model.Id),
        IncomeDate = model.IncomeDate,
        AmountOwed = model.AmountOwed,
        AmountPaid = model.AmountPaid,
        Reference = model.Reference ?? string.Empty,
        Description = model.Description ?? string.Empty
    };

    public IncomeModel Clone() => new()
    {
        Id = Id,
        ContractId = ContractId,
        IncomeDate = IncomeDate,
        AmountOwed = AmountOwed,
        AmountPaid = AmountPaid,
        Reference = Reference ?? string.Empty,
        Description = Description ?? string.Empty,
        CanDelete = CanDelete
    };

    public override string ToString() => $"{IncomeDate.ToShortDateString()}: {AmountPaid:c2} ({AmountOwed:c2})";

    public override bool Equals(object? obj) => obj is IncomeModel model && model.Id == Id;

    public bool Equals(IncomeModel? model) => model is not null && model.Id == Id;

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(IncomeModel left, IncomeModel right) => (left, right) switch
    {
        (null, null) => true,
        (null, _) or (_, null) => false,
        (_, _) => left.Id == right.Id
    };

    public static bool operator !=(IncomeModel left, IncomeModel right) => !(left == right);

    public static implicit operator IncomeEntity?(IncomeModel model) => FromModel(model);

    public static implicit operator IncomeModel?(IncomeEntity entity) => FromEntity(entity);
}
