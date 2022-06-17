
using XTrakr.Common;
using XTrakr.Repositories.Entities;

namespace XTrakr.Models;
public class PayeeModel : ModelBase, IEquatable<PayeeModel>, IComparable<PayeeModel>
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string Description { get; set; }

    public PayeeModel() : base()
    {
        Id = IdEncoder.EncodeId(0);
        Name = string.Empty;
        Address = string.Empty;
        Description = string.Empty;
    }

    public static PayeeModel? FromEntity(PayeeEntity entity) => entity is null ? null : new()
    {
        Id = IdEncoder.EncodeId(entity.Id),
        Name = entity.Name ?? string.Empty,
        Address = entity.Address ?? string.Empty,
        Description = entity.Description ?? string.Empty,
        CanDelete = true
    };

    public static PayeeEntity? FromModel(PayeeModel model) => model is null ? null : new()
    {
        Id = IdEncoder.DecodeId(model.Id),
        Name = model.Name ?? string.Empty,
        Address = model.Address ?? string.Empty,
        Description = model.Description ?? string.Empty
    };

    public PayeeModel Clone() => new()
    {
        Id = Id ?? IdEncoder.EncodeId(0),
        Name = Name ?? string.Empty,
        Address = Address ?? string.Empty,
        Description = Description ?? string.Empty,
        CanDelete = CanDelete
    };

    public override string ToString() => Name;

    public override bool Equals(object? obj) => obj is PayeeModel model && model.Id == Id;

    public bool Equals(PayeeModel? model) => model is not null && model.Id == Id;

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(PayeeModel left, PayeeModel right) => (left, right) switch
    {
        (null, null) => true,
        (null, _) or (_, null) => false,
        (_, _) => left.Id == right.Id
    };

    public static bool operator !=(PayeeModel left, PayeeModel right) => !(left == right);

    public int CompareTo(PayeeModel? other) => Name.CompareTo(other?.Name);

    public static bool operator >(PayeeModel left, PayeeModel right) => left.CompareTo(right) > 0;

    public static bool operator <(PayeeModel left, PayeeModel right) => left.CompareTo(right) < 0;

    public static bool operator >=(PayeeModel left, PayeeModel right) => left.CompareTo(right) >= 0;

    public static bool operator <=(PayeeModel left, PayeeModel right) => left.CompareTo(right) <= 0;

    public static implicit operator PayeeModel?(PayeeEntity entity) => FromEntity(entity);

    public static implicit operator PayeeEntity?(PayeeModel model) => FromModel(model);
}
