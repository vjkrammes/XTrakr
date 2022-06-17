using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace XTrakr.Infrastructure;

// convert from int (count) to visibility where 0 = Collapsed else Visible, inverted with parm

[ValueConversion(typeof(int), typeof(Visibility), ParameterType = typeof(bool))]
public sealed class CountToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type t, object parm, CultureInfo lang)
    {
        if (parm is not bool invert)
        {
            invert = false;
        }
        if (value is int count)
        {
            if (invert)
            {
                return count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            return count == 0 ? Visibility.Collapsed : Visibility.Visible;
        }
        return Visibility.Visible;
    }
    public object ConvertBack(object value, Type t, object parm, CultureInfo lang) => DependencyProperty.UnsetValue;
}

// convert from count to bool (IsEnabled) where 0 = false else true, inverted with parm

[ValueConversion(typeof(int), typeof(bool), ParameterType = typeof(bool))]
public sealed class CountToEnabledConverter : IValueConverter
{
    public object Convert(object value, Type t, object parm, CultureInfo lang)
    {
        if (parm is not bool invert)
        {
            invert = false;
        }
        if (value is not int v)
        {
            return invert;      // default is disabled, or enabled if inverted
        }
        if (invert)
        {
            return v == 0;
        }
        return v != 0;
    }
    public object ConvertBack(object value, Type t, object parm, CultureInfo lang) => DependencyProperty.UnsetValue;
}

[ValueConversion(typeof(bool), typeof(Visibility), ParameterType = typeof(bool))]
public sealed class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type t, object parm, CultureInfo lang)
    {
        if (parm is not bool invert)
        {
            invert = false;
        }
        if (value is bool v)
        {
            if (invert)
            {
                return v ? Visibility.Collapsed : Visibility.Visible;
            }
            return v ? Visibility.Visible : Visibility.Collapsed;
        }
        return Visibility.Visible;
    }
    public object ConvertBack(object value, Type t, object parm, CultureInfo lang) => DependencyProperty.UnsetValue;
}

[ValueConversion(typeof(string), typeof(string))]
public sealed class IconToNameConverter : IValueConverter
{
    public object Convert(object value, Type t, object parm, CultureInfo lang)
    {
        if (value is not string icon)
        {
            return value;
        }
        return icon.Replace("/resources/", "").Replace("-32.png", "");
    }
    public object ConvertBack(object value, Type t, object parm, CultureInfo lang) => DependencyProperty.UnsetValue;
}

[ValueConversion(typeof(decimal), typeof(string))]
public sealed class DecimalConverter : IValueConverter
{
    public object Convert(object value, Type t, object parm, CultureInfo lang)
    {
        if (value is decimal v)
        {
            return v.ToString();
        }
        return string.Empty;
    }
    public object ConvertBack(object value, Type t, object parm, CultureInfo lang)
    {
        if (value is not string v || string.IsNullOrWhiteSpace(v))
        {
            return 0M;
        }
        var val = v;
        if (v.EndsWith("."))
        {
            val = v.TrimEnd('.');   // convert without the trailing decimal without "eating" the decimal from the textbox
        }
        if (!decimal.TryParse(val, out var d))
        {
            return 0M;
        }
        return d;
    }
}
