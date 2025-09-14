using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace PastryShop.Converter
{
    public class ImagePathToImageConverter : IValueConverter
    {
        private static BitmapImage GetDefaultImage(string folder)
        {
            string defaultPackUri;

            if (folder == "Products")
            {
                defaultPackUri = "pack://application:,,,/Resource/Images/Application/Base_No_Photo_Product.png";
            }
            else if (folder == "Users")
            {
                defaultPackUri = "pack://application:,,,/Resource/Images/Application/Base_No_Photo_User.png";
            }
            else
            {
                defaultPackUri = "pack://application:,,,/Resource/Images/Application/Base_No_Photo.png";
            }

            var defaultImage = new BitmapImage();
            defaultImage.BeginInit();
            defaultImage.UriSource = new Uri(defaultPackUri, UriKind.Absolute);
            defaultImage.CacheOption = BitmapCacheOption.OnLoad;
            defaultImage.EndInit();
            return defaultImage;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string folder = (parameter as string) ?? "Users";

            string baseFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "PastryShop",
                "Resource",
                "Images",
                folder);

            if (value is string path && !string.IsNullOrEmpty(path))
            {
                string filePath = Path.Combine(baseFolder, path);
                if (!File.Exists(filePath))
                {
                    return GetDefaultImage(folder);
                }

                var image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(filePath, UriKind.Absolute);
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                return image;
            }

            return GetDefaultImage(folder);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

    }
}