using System;
using System.Windows.Markup;

namespace XTrakr.MarkupExtensions;
public sealed class BoolExtension : MarkupExtension
{
    private bool _value { get; }
    public BoolExtension(bool value) => _value = value;
    public override object ProvideValue(IServiceProvider _) => _value;
}
