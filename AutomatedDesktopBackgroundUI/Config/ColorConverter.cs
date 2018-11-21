using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace AutomatedDesktopBackgroundUI.Config
{
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string text = (string)value;
            switch (text)
            {
                case "fontColor":
                       return System.Drawing.ColorTranslator.FromHtml("#a6d785");
                case "bgColor":
                    return System.Drawing.ColorTranslator.FromHtml("#a6d785");
                default:
                    return Colors.Pink;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
