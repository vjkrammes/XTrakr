using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XTrakr.Common;
public static class ExtensionMethods
{
    public static string FirstWord(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }
        return value.Split(new char[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries)?.First() ?? string.Empty;
    }

    public static string Host(this string apiuri)
    {
        if (string.IsNullOrWhiteSpace(apiuri))
        {
            return string.Empty;
        }
        try
        {
            var uri = new Uri(apiuri);
            return uri.Host;
        }
        catch
        {
            return string.Empty;
        }
    }

    public static int Sign<T>(this T value) where T : IComparable<T>
    {
        if (value.CompareTo(default) < 0)
        {
            return -1;
        }
        if (value.CompareTo(default) > 0)
        {
            return 1;
        }
        return 0;
    }

    public static string StripPadding(this string value) => value.TrimEnd('=');

    public static string AddPadding(this string value) => (value.Length % 4) switch
    {
        0 => value,
        1 => value + "===",
        2 => value + "==",
        _ => value + "="
    };

    public static string Hexify(this ulong value)
    {
        StringBuilder sb = new("0x");
        var (o0, o1, o2, o3) = value.Octets();
        sb.Append(o0.ToString("x2"));
        sb.Append(o1.ToString("x2"));
        sb.Append(o2.ToString("x2"));
        sb.Append(o3.ToString("x2"));
        return sb.ToString();
    }

    public static string Hexify(this int value) => Hexify((ulong)value);

    public static string Hexify(this long value) => Hexify((ulong)value);

    public static string Hexify(this uint value) => Hexify((ulong)value);

    public static string Hexify(this byte[] array)
    {
        if (array is null)
        {
            throw new ArgumentNullException(nameof(array));
        }
        var sb = new StringBuilder("0x");
        foreach (var b in array)
        {
            sb.Append(b.ToString("x2"));
        }
        return sb.ToString();
    }

    public static (byte o0, byte o1, byte o2, byte o3) Octets(this ulong value)
    {
        var o0 = (byte)((value >> 24) & 0xff);
        var o1 = (byte)((value >> 16) & 0xff);
        var o2 = (byte)((value >> 8) & 0xff);
        var o3 = (byte)(value & 0xff);
        return (o0, o1, o2, o3);
    }

    public static (byte o0, byte o1, byte o2, byte o3) Octets(this long value) => ((ulong)value).Octets();

    public static (byte o0, byte o1, byte o2, byte o3) Octets(this uint value) => ((ulong)value).Octets();

    public static (byte o0, byte o1, byte o2, byte o3) Octets(this int value) => ((ulong)value).Octets();

    public static IEnumerable<TModel> ToModels<TModel, TEntity>(this IEnumerable<TEntity> entities, string methodName = "FromEntity")
        where TModel : class where TEntity : class
    {
        var ret = new List<TModel>();
        var method = typeof(TModel).GetMethod(methodName, BindingFlags.Public | BindingFlags.Static, null,
            new[] { typeof(TEntity) }, Array.Empty<ParameterModifier>());
        if (method is not null)
        {
            if (entities is not null && entities.Any())
            {
                entities.ForEach(x => ret.Add((method.Invoke(null, new[] { x }) as TModel)!));
            }
        }
        return ret;
    }

    public static string TrimEnd(this string value, string suffix, StringComparison comparer = StringComparison.OrdinalIgnoreCase)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }
        if (!string.IsNullOrWhiteSpace(suffix) && value.EndsWith(suffix, comparer))
        {
            return value[..^suffix.Length];
        }
        return value;
    }

    public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
    {
        if (list is null)
        {
            throw new ArgumentNullException(nameof(list));
        }
        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }
        foreach (var item in list)
        {
            action(item);
        }
    }

    public static string Beginning(this string value, int length, char ellipsis = '.')
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }
        return value.Length <= length ? value : string.Concat(value.AsSpan(0, length), new string(ellipsis, 3));
    }

    public static bool IsDescending(this char value) => value is 'd' or 'D';

    public static T[] Add<T>(this T[] source, T[] items)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        if (items is null || !items.Any())
        {
            return source;
        }
        var ret = new T[source.Length + items.Length];
        Array.Copy(source, 0, ret, 0, source.Length);
        Array.Copy(items, 0, ret, source.Length, items.Length);
        return ret;
    }

    public static IEnumerable<T> Add<T>(this IEnumerable<T> source, IEnumerable<T> items) => source.ToArray().Add(items.ToArray());

    public static T[] ArrayCopy<T>(this T[] source)
    {
        if (source is null || !source.Any())
        {
            return Array.Empty<T>();
        }
        var length = source.Length;
        var ret = new T[length];
        Array.Copy(source, ret, length);
        return ret;
    }

    public static bool ArrayEquals<T>(this T[] left, T[] right, bool wholelength = false) where T : IEquatable<T>
    {
        if (left is null)
        {
            if (right is null)
            {
                return true;
            }
            return false;
        }
        if (right is null)
        {
            return false;
        }
        if (ReferenceEquals(left, right))
        {
            return true;
        }
        if (left.Length != right.Length)
        {
            return false;
        }
        var comparer = EqualityComparer<T>.Default;
        var ret = true;
        for (var i = 0; i < left.Length; i++)
        {
            if (!comparer.Equals(left[i], right[i]))
            {
                ret = false;
                if (!wholelength)
                {
                    break;
                }
            }
        }
        return ret;
    }

    public static string Capitalize(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }
        return value.First().ToString().ToUpper(CultureInfo.CurrentCulture) + string.Join(string.Empty, value.Skip(1));
    }

    public static string GetDescriptionFromEnumValue<T>(this T value) where T : Enum =>
        typeof(T)
            .GetField(value.ToString())!
            .GetCustomAttributes(typeof(DescriptionAttribute), false)
            .SingleOrDefault() is not DescriptionAttribute attribute ? value.ToString() : attribute.Description;

    public static string Innermost(this Exception exception)
    {
        if (exception is null)
        {
            throw new ArgumentNullException(nameof(exception));
        }
        if (exception.InnerException is null)
        {
            return exception.Message;
        }
        return exception.InnerException.Innermost();
    }

    public static bool IsBetween(this DateTime date, DateTime start, DateTime end) => date >= start && date <= end;
}
