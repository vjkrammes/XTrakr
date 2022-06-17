
using XTrakr.Common;
using XTrakr.Repositories.Entities;

namespace XTrakr.Models;
public class ExpenseTypeModel : ModelBase, IEquatable<ExpenseTypeModel>, IComparable<ExpenseTypeModel>
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Background { get; set; }
    public string Icon { get; set; }
    public long ARGB { get; set; }

    public ExpenseTypeModel() : base()
    {
        Id = IdEncoder.EncodeId(0);
        Name = string.Empty;
        Background = string.Empty;
        Icon = string.Empty;
        ARGB = 0xFFFFFFFF;
    }

    public static ExpenseTypeModel? FromEntity(ExpenseTypeEntity entity) => entity is null ? null : new()
    {
        Id = IdEncoder.EncodeId(entity.Id),
        Name = entity.Name ?? string.Empty,
        Background = entity.Background ?? string.Empty,
        Icon = entity.Icon ?? string.Empty,
        ARGB = entity.ARGB,
        CanDelete = true
    };

    public static ExpenseTypeEntity? FromModel(ExpenseTypeModel model) => model is null ? null : new()
    {
        Id = IdEncoder.DecodeId(model.Id),
        Name = model.Name ?? string.Empty,
        Background = model.Background ?? string.Empty,
        Icon = model.Icon ?? string.Empty,
        ARGB = model.ARGB
    };

    public ExpenseTypeModel Clone() => new()
    {
        Id = Id ?? IdEncoder.EncodeId(0),
        Name = Name ?? string.Empty,
        Background = Background ?? string.Empty,
        Icon = Icon ?? string.Empty,
        ARGB = ARGB,
        CanDelete = CanDelete
    };

    public override string ToString() => Name;

    public override bool Equals(object? obj) => obj is ExpenseTypeModel model && model.Id == Id;

    public bool Equals(ExpenseTypeModel? model) => model is not null && model.Id == Id;

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(ExpenseTypeModel left, ExpenseTypeModel right) => (left, right) switch
    {
        (null, null) => true,
        (null, _) or (_, null) => false,
        (_, _) => left.Id == right.Id
    };

    public static bool operator !=(ExpenseTypeModel left, ExpenseTypeModel right) => !(left == right);

    public int CompareTo(ExpenseTypeModel? other) => Name.CompareTo(other?.Name);

    public static bool operator >(ExpenseTypeModel left, ExpenseTypeModel right) => left.CompareTo(right) > 0;

    public static bool operator <(ExpenseTypeModel left, ExpenseTypeModel right) => left.CompareTo(right) < 0;

    public static bool operator >=(ExpenseTypeModel left, ExpenseTypeModel right) => left.CompareTo(right) >= 0;

    public static bool operator <=(ExpenseTypeModel left, ExpenseTypeModel right) => left.CompareTo(right) <= 0;

    public static implicit operator ExpenseTypeModel?(ExpenseTypeEntity entity) => FromEntity(entity);

    public static implicit operator ExpenseTypeEntity?(ExpenseTypeModel model) => FromModel(model);
}
