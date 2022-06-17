namespace XTrakr.Common.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class IndexedAttribute : Attribute
{
    public string IndexName { get; }
    public IndexedAttribute(string? name = null) => IndexName = string.IsNullOrWhiteSpace(name) ? string.Empty : name;
}
