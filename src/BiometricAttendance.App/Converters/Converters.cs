using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BiometricAttendance.App.Converters;

/// <summary>bool → Visibility (true=Visible, false=Collapsed)</summary>
public sealed class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is true ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => value is Visibility.Visible;
}

/// <summary>bool → inverted bool</summary>
public sealed class InverseBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is false;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => value is false;
}

/// <summary>bool → loading/normal text. ConverterParameter = "Loading|Normal"</summary>
public sealed class BoolToTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var parts = (parameter?.ToString() ?? "|").Split('|');
        return value is true ? parts[0] : (parts.Length > 1 ? parts[1] : string.Empty);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => Binding.DoNothing;
}

/// <summary>bool → eye icon text</summary>
public sealed class BoolToEyeIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is true ? "🙈" : "👁";

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => Binding.DoNothing;
}

/// <summary>string → initials (first letter of each word, max 2)</summary>
public sealed class InitialsConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string name || string.IsNullOrWhiteSpace(name)) return "?";
        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length >= 2
            ? $"{parts[0][0]}{parts[^1][0]}".ToUpperInvariant()
            : name[0].ToString().ToUpperInvariant();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => Binding.DoNothing;
}

/// <summary>AttendanceStatus → background brush key (used in attendance grids)</summary>
public sealed class AttendanceStatusToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.ToString() switch
        {
            "Present"  => "SuccessBrush",
            "Late"     => "WarningBrush",
            "Absent"   => "DangerBrush",
            "OnLeave"  => "InfoBrush",
            "Holiday"  => "AccentDarkBrush",
            _          => "MutedBrush"
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => Binding.DoNothing;
}
