using System;
using System.Threading.Tasks;
using System.Windows.Media;

using XTrakr.Common;

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
}
