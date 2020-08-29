using System;
using System.Globalization;
using System.Windows.Data;

namespace Notepad2.Converters
{
    public class LeftPaneOpenedToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool paneOpened)
            {
                if ((bool)value)
                    return new Uri(@"/SharpPad;component/Resources/closePane.png", UriKind.Relative);
                else
                    return new Uri(@"/SharpPad;component/Resources/openPane.png", UriKind.Relative);
            }
            else
                return new Uri(@"/SharpPad;component/Resources/openPane.png", UriKind.Relative);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }
    }
}
