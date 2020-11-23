using SharpPad.FileExplorer;
using System;
using System.Globalization;
using System.Windows.Data;

namespace SharpPad.Converters
{
    public class FileExistsToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((string)value).IsFile();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }
}
