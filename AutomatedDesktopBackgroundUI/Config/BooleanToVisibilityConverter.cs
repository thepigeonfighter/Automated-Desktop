using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace AutomatedDesktopBackgroundUI.Config
{
    [System.Windows.Localizability(System.Windows.LocalizationCategory.NeverLocalize)]
    public class BooleanToVisibilityConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value.GetType() != typeof(bool))
            {
                throw new InvalidOperationException("Object's value must be of type Boolean");
            }
            bool visibility = (bool)value;
            if(visibility)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Hidden;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value.GetType() != typeof(Visibility))
            {
                throw new InvalidOperationException("Object's value must be of type Visibility");
            }
            Visibility visibility = (Visibility)value;
            switch (visibility)
            {
                case Visibility.Visible:
                    return true;
                case Visibility.Hidden:
                    return false;
                case Visibility.Collapsed:
                    return false;
                default:
                    return false;
            }
        }
    }
}
