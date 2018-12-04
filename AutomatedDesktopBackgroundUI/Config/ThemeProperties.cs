using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AutomatedDesktopBackgroundUI.Config
{
    public static class ThemeProperties
    {

        public static readonly DependencyProperty ActiveBackground = DependencyProperty.RegisterAttached(
            "ActiveBackground", typeof(ImageBrush), typeof(ThemeProperties),
            new FrameworkPropertyMetadata(GetDefaultImageBrush(), FrameworkPropertyMetadataOptions.Inherits));
        private static ImageBrush GetDefaultImageBrush()
        {
            ImageBrush brush = new ImageBrush();
            return brush;
        }
        public static ImageBrush GetActiveBackgroundBrush(DependencyObject obj)
        {
            return (ImageBrush)obj.GetValue(ActiveBackground);
        }
        public static void SetActiveBackgroundBrush(DependencyObject obj, ImageBrush brush)
        {
            obj.SetValue(ActiveBackground, brush);
        }
        
    }
}
