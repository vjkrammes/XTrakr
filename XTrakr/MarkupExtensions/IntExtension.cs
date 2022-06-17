using System;
using System.Windows.Markup;

namespace XTrakr.MarkupExtensions;
public sealed class IntExtension : MarkupExtension
{
    private int _value { get; }
    public IntExtension(int value) => _value = value;
    public override object ProvideValue(IServiceProvider _) => _value;
}
