namespace XTrakr.Common.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class ExplorerIconAttribute : Attribute
{
    public string Icon { get; }
    public ExplorerIconAttribute(string icon) => Icon = icon;
    public string ExplorerIcon => Icon;
}
