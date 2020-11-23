using System;
using System.Globalization;
using System.Windows.Data;

namespace SharpPad.Converters
{
    public class TopPaneOpenedToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool paneOpened)
            {
                if ((bool)value)
                    return new Uri(@"/SharpPad;component/Resources/topTabsClose.png", UriKind.Relative);
                else
                    return new Uri(@"/SharpPad;component/Resources/topTabsOpen.png", UriKind.Relative);
            }
            else
                return new Uri(@"/SharpPad;component/Resources/topTabsOpen.png", UriKind.Relative);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }
    }
}
