using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

using XTrakr.Common;
using XTrakr.Common.Attributes;

namespace XTrakr.Infrastructure;
public static class ExtensionMethods
{
    public static SolidColorBrush GetBrush(this long argb)
    {
        (var a, var r, var g, var b) = argb.Octets();
        return new(Color.FromArgb(a, r, g, b));
    }

    public static async void FireAndForgetSafeAsync(this Task task, Action<Exception>? errorHandler = null)
    {
        try
        {
            await task.ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            errorHandler?.Invoke(ex);
        }
    }

    public static Uri? GetIconFromEnumValue<T>(this T value) where T : Enum =>
        typeof(T)
            .GetField(value.ToString())!
            .GetCustomAttributes(typeof(ExplorerIconAttribute), false)
            .SingleOrDefault() is not ExplorerIconAttribute attr ? null : new Uri(attr.ExplorerIcon, UriKind.Relative);
}
