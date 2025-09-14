using System.IO;

namespace PastryShop.Utility
{
    public class DefaultImages
    {
        public static void CopyDefaultImages()
        {

            string productAppDataFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "PastryShop",
                "Resource",
                "Images",
                "Products");

            if (!Directory.Exists(productAppDataFolder))
            {
                Directory.CreateDirectory(productAppDataFolder);
            }

            string defaultProductFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DefaultProductImages");
            if (Directory.Exists(defaultProductFolder))
            {

                foreach (var file in Directory.GetFiles(defaultProductFolder))
                {
                    string fileName = Path.GetFileName(file);
                    string destinationFile = Path.Combine(productAppDataFolder, fileName);
                    if (!File.Exists(destinationFile))
                    {
                        File.Copy(file, destinationFile);
                    }
                }
            }

            string userAppDataFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "PastryShop",
                "Resource",
                "Images",
                "Users");

            if (!Directory.Exists(userAppDataFolder))
            {
                Directory.CreateDirectory(userAppDataFolder);
            }

            string defaultUserFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DefaultUserImages");
            if (Directory.Exists(defaultUserFolder))
            {
                foreach (var file in Directory.GetFiles(defaultUserFolder))
                {
                    string fileName = Path.GetFileName(file);
                    string destinationFile = Path.Combine(userAppDataFolder, fileName);
                    if (!File.Exists(destinationFile))
                    {
                        File.Copy(file, destinationFile);
                    }
                }
            }
        }

    }
}