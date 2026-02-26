using System.Globalization;

namespace kafka_sufficiently_advanced_technology.Converters;

public class BoolToColorConverter : IValueConverter
{
    public Color TrueColor  { get; set; } = Color.FromArgb("#CCE8FF");
    public Color FalseColor { get; set; } = Colors.Transparent;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is true ? TrueColor : FalseColor;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
