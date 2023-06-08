using System;
using System.Globalization;
using System.Windows.Data;

namespace GracePointeSecurity.ManagementApp;

public sealed class BoolToIndexConverter : IValueConverter
{
    public object Convert(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture) =>
        value is true
            ? "Move"
            : "Copy";

    public object ConvertBack(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture) =>
        value is "Move";
}