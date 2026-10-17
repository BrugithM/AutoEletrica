using System.Globalization;
using System.Windows.Data;

namespace SgaAutoEletrica.UI.Converters;

public class UtcParaLocalConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime dt)
            return dt.ToLocalTime();

        if (value is DateTimeOffset dto)
            return dto.ToLocalTime();

        return value;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException("UtcParaLocalConverter é apenas de exibição.");
}