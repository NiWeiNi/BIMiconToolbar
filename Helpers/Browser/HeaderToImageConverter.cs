using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using UIFramework;

namespace BIMiconToolbar.Helpers.Browser
{
    [ValueConversion(typeof(string), typeof(BitmapImage))]
    public class HeaderToImageConverter : IValueConverter
    {
        public static HeaderToImageConverter Instance = new HeaderToImageConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var path = (string)value;

            if (path == null)
            {
                return null;
            }

            var name = BrowserWindow.GetFileFolderName(path);

            // File icon by default
            var image = "file.png";

            // Blank name, assumed to be a drive
            if (string.IsNullOrEmpty(name))
            {
                image = "harddrive.png";
            }
            else if (new FileInfo(path).Attributes.HasFlag(FileAttributes.Directory))
            {
                image = "folder.png";
            }
            else if (name.Contains(".dwg"))
            {
                image = "dwg.png";
            }
            else if (name.Contains(".jpg"))
            {
                image = "jpg.png";
            }
            else if (name.Contains(".pdf"))
            {
                image = "pdf.png";
            }
            else if (name.Contains(".rfa"))
            {
                image = "revit.png";
            }
            else if (name.Contains(".rte"))
            {
                image = "revit.png";
            }
            else if (name.Contains(".rvt"))
            {
                image = "revit.png";
            }
            else if (name.Contains(".txt"))
            {
                image = "txt.png";
            }
            else if (name.Contains(".xls") || name.Contains(".xlsx"))
            {
                image = "xls.png";
            }

            return new BitmapImage(new Uri($"pack://application:,,,/BIMiconToolbar;component/Helpers/Browser/Images/{image}"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
