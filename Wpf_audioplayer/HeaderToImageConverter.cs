using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Wpf_audioplayer
{
    /// <summary>
    /// Converts a full path to a specific image (folder, song, or playlist)
    /// </summary>
    [ValueConversion(typeof(string), typeof(BitmapImage))]
    class HeaderToImageConverter : IValueConverter
    {
        public static HeaderToImageConverter Instance = new HeaderToImageConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Get full path
            var path = (string)value;

            // if path null is null - ignore
            if (path == null)
                return null;

            //Get name of file/folder
            var name = MainWindow.GetFileOrFolderName(path);
            // by default we presume song
            var image = "Resources/Icons/song_icon.png";
            
            //Check if it's folder or song
            if (string.IsNullOrEmpty(path))
                image = "Resources/Icons/drive_icon.png";
            else if (new FileInfo(path).Attributes.HasFlag(FileAttributes.Directory))
                image = "Resources/Icons/folder_icon.png";

            // Get image from resources
            return new BitmapImage(new Uri($"pack://application:,,,/{image}"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
