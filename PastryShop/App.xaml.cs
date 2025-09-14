using System.IO;
using System.Text.Json;
using System.Windows;
using PastryShop.Data.DAO;
using PastryShop.Service;
using PastryShop.Utility;
using PastryShop.View;

namespace PastryShop
{
    public partial class App : Application
    {
        public static AppSettings Settings { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                DefaultImages.CopyDefaultImages();

                LoadConfiguration();
                var userDao = new UserDao(Settings.ConnectionStrings.MySqlConnectionString);
                var stockDao = new StockDao(Settings.ConnectionStrings.MySqlConnectionString);
                var productDao = new ProductDao(Settings.ConnectionStrings.MySqlConnectionString);
                var userService = new UserService(userDao);
                var stockService = new StockService(stockDao);
                var productService = new ProductService(productDao, stockService);
                var cartService = new CartService();

                var loginPage = new LoginPage(userService, productService, stockService, cartService);
                loginPage.Show();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška pri učitavanju konfiguracije: {ex.Message}", ex);
                Shutdown();
            }
        }

        private static void LoadConfiguration()
        {
            var configFilePath = "appsettings.json";

            if (!File.Exists(configFilePath))
            {
                Logger.LogError($"Konfiguraciona datoteka '{configFilePath}' nije pronađena.", new FileNotFoundException());
                return;
            }

            var json = File.ReadAllText(configFilePath);

            var config = JsonSerializer.Deserialize<AppSettings>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (config == null)
            {
                Logger.LogError("Konfiguraciona datoteka je prazna ili neispravna.", new InvalidOperationException());
                return;
            }

            Settings = config;
        }

    }

    public class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; set; } = null!;
    }

    public class ConnectionStrings
    {
        public string MySqlConnectionString { get; set; } = null!;
    }

}