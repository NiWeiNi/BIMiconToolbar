using BIMiconToolbar.Helpers.Browser.Data;
using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using UIFramework;

namespace BIMiconToolbar.Helpers.Browser
{
    [ValueConversion(typeof(BrowserItemType), typeof(BitmapImage))]
    public class HeaderToImageConverter : IValueConverter
    {
        public static HeaderToImageConverter Instance = new HeaderToImageConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // File icon by default
            var image = "file.png";

            switch ((BrowserItemType)value)
            {
                case BrowserItemType.Drive:
                    image = "harddrive.png";
                    break;
                case BrowserItemType.Folder:
                    image = "folder.png";
                    break;
            }

            return new BitmapImage(new Uri($"pack://application:,,,/BIMiconToolbar;component/Helpers/Browser/Images/{image}"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
