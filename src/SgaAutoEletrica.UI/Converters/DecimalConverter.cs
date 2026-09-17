using System.Globalization;
using System.Windows.Data;

namespace SgaAutoEletrica.UI.Converters;

public class DecimalConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is decimal d)
            return d.ToString("N2", new CultureInfo("pt-BR"));
        return value?.ToString() ?? string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string s)
        {
            s = s.Replace("R$", "").Replace(" ", "").Trim();

            if (string.IsNullOrWhiteSpace(s))
                return 0m;

            // Tenta pt-BR primeiro (vírgula)
            if (decimal.TryParse(s, NumberStyles.Any, new CultureInfo("pt-BR"), out var resultado))
                return resultado;

            // Tenta en-US (ponto)
            if (decimal.TryParse(s, NumberStyles.Any, new CultureInfo("en-US"), out var resultado2))
                return resultado2;
        }
        return 0m;
    }
}