using BIMicon.BIMiconToolbar.Helpers.Browser.Data;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace BIMicon.BIMiconToolbar.Helpers.Browser
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
                case BrowserItemType.Dwg:
                    image = "dwg.png";
                    break;
                case BrowserItemType.Pdf:
                    image = "pdf.png";
                    break;
                case BrowserItemType.Image:
                    image = "jpg.png";
                    break;
                case BrowserItemType.Txt:
                    image = "txt.png";
                    break;
                case BrowserItemType.Xls:
                    image = "xls.png";
                    break;
                case BrowserItemType.Revit:
                    image = "revit.png";
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
