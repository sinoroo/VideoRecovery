using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VideoRecovery.App.ViewModels;

/// <summary>
/// null이면 Collapsed, 값이 있으면 Visible로 변환한다. <see cref="InstanceInverted"/>는 반대.
/// 상세 패널의 빈 상태 처리에 사용한다.
/// </summary>
public sealed class NullToVisibilityConverter : IValueConverter
{
    /// <summary>값이 있으면 Visible.</summary>
    public static readonly NullToVisibilityConverter Instance = new(invert: false);

    /// <summary>값이 없으면 Visible.</summary>
    public static readonly NullToVisibilityConverter InstanceInverted = new(invert: true);

    private readonly bool _invert;

    private NullToVisibilityConverter(bool invert) => _invert = invert;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var hasValue = value is not null;
        if (_invert)
        {
            hasValue = !hasValue;
        }

        return hasValue ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
