using System.Globalization;
using System.Windows.Data;

namespace SgaAutoEletrica.UI.Converters;

public class BoolParaStatusConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool b)
            return b ? "Ativo" : "Inativo";
        return "Inativo";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.ToString() == "Ativo";
    }
}