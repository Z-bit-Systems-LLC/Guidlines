using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ZBitSystems.Wpf.UI.Localization;

/// <summary>
/// Formats bound values with a composite format string supplied as the first bound value.
/// Used by <see cref="LocalizeFormatExtension"/> so the format string can come from a
/// binding that updates when the culture changes.
/// </summary>
public class LocalizedFormatConverter : IMultiValueConverter
{
    /// <inheritdoc />
    public object Convert(object[] values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Length == 0 || values[0] is not string format)
            return string.Empty;

        if (values.Length == 1)
            return format;

        var arguments = new object?[values.Length - 1];
        for (var index = 1; index < values.Length; index++)
        {
            arguments[index - 1] = values[index] == DependencyProperty.UnsetValue ? null : values[index];
        }

        try
        {
            return string.Format(culture, format, arguments);
        }
        catch (FormatException)
        {
            // A malformed resource string should not take down the visual tree
            return format;
        }
    }

    /// <inheritdoc />
    public object[] ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
