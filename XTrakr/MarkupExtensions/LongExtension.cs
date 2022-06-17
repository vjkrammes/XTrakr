using System;
using System.Windows.Markup;

namespace XTrakr.MarkupExtensions;
public sealed class LongExtension : MarkupExtension
{
    private long _value { get; }
    public LongExtension(long value) => _value = value;
    public override object ProvideValue(IServiceProvider _) => _value;
}
