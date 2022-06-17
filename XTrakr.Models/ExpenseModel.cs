
using XTrakr.Common;
using XTrakr.Repositories.Entities;

namespace XTrakr.Models;
public class ExpenseModel : ModelBase, IEquatable<ExpenseModel>, IComparable<ExpenseModel>
{
    public string Id { get; set; }
    public string PayeeId { get; set; }
    public string ExpenseTypeId { get; set; }
    public DateTime ExpenseDate { get; set; }
    public decimal Amount { get; set; }
    public string Reference { get; set; }
    public string Description { get; set; }

    public PayeeModel? Payee { get; set; }

    public ExpenseTypeModel? ExpenseType { get; set; }

    public ExpenseModel() : base()
    {
        Id = IdEncoder.EncodeId(0);
        PayeeId = IdEncoder.EncodeId(0);
        ExpenseTypeId = IdEncoder.EncodeId(0);
        ExpenseDate = DateTime.UtcNow;
        Amount = 0M;
        Reference = string.Empty;
        Description = string.Empty;
        CanDelete = true;
    }

    public static ExpenseModel? FromEntity(ExpenseEntity entity) => entity is null ? null : new()
    {
        Id = IdEncoder.EncodeId(entity.Id),
        PayeeId = IdEncoder.EncodeId(entity.PayeeId),
        ExpenseTypeId = IdEncoder.EncodeId(entity.ExpenseTypeId),
        ExpenseDate = entity.ExpenseDate,
        Amount = entity.Amount,
        Reference = entity.Reference ?? string.Empty,
        Description = entity.Description ?? string.Empty,
        Payee = entity.Payee!,
        ExpenseType = entity.ExpenseType!,
        CanDelete = true
    };

    public static ExpenseEntity? FromModel(ExpenseModel model) => model is null ? null : new()
    {
        Id = IdEncoder.DecodeId(model.Id),
        PayeeId = IdEncoder.DecodeId(model.PayeeId),
        ExpenseTypeId = IdEncoder.DecodeId(model.ExpenseTypeId),
        ExpenseDate = model.ExpenseDate,
        Amount = model.Amount,
        Reference = model.Reference ?? string.Empty,
        Description = model.Description ?? string.Empty,
        Payee = model.Payee!,
        ExpenseType = model.ExpenseType!
    };

    public ExpenseModel Clone() => new()
    {
        Id = Id ?? IdEncoder.EncodeId(0),
        PayeeId = PayeeId ?? IdEncoder.EncodeId(0),
        ExpenseTypeId = ExpenseTypeId ?? IdEncoder.EncodeId(0),
        ExpenseDate = ExpenseDate,
        Amount = Amount,
        Reference = Reference ?? string.Empty,
        Description = Description ?? string.Empty,
        Payee = Payee?.Clone(),
        ExpenseType = ExpenseType?.Clone(),
        CanDelete = CanDelete
    };

    public override string ToString() => $"{ExpenseDate.ToShortDateString()} {Payee?.Name ?? "Unknown"} - {Amount:c2}";

    public override bool Equals(object? obj) => obj is ExpenseModel model && model.Id == Id;

    public bool Equals(ExpenseModel? model) => model is not null && model.Id == Id;

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(ExpenseModel left, ExpenseModel right) => (left, right) switch
    {
        (null, null) => true,
        (null, _) or (_, null) => false,
        (_, _) => left.Id == right.Id
    };

    public static bool operator !=(ExpenseModel left, ExpenseModel right) => !(left == right);

    public int CompareTo(ExpenseModel? other) => ExpenseDate.CompareTo(other?.ExpenseDate);

    public static bool operator >(ExpenseModel left, ExpenseModel right) => left.CompareTo(right) > 0;

    public static bool operator <(ExpenseModel left, ExpenseModel right) => left.CompareTo(right) < 0;

    public static bool operator >=(ExpenseModel left, ExpenseModel right) => left.CompareTo(right) >= 0;

    public static bool operator <=(ExpenseModel left, ExpenseModel right) => left.CompareTo(right) <= 0;

    public static implicit operator ExpenseModel?(ExpenseEntity entity) => FromEntity(entity);

    public static implicit operator ExpenseEntity?(ExpenseModel model) => FromModel(model);
}
