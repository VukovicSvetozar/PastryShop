using System.IO;
using System.Text;

namespace PastryShop.Utility
{
    public static class Logger
    {
        private static readonly object _lock = new();
        private static readonly string _logDir;

        static Logger()
        {
            var appName = "PastryShopApp";
            _logDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), appName, "logs");
            Directory.CreateDirectory(_logDir);
        }

        public static void LogError(string message, Exception ex)
        {
            var sb = new StringBuilder();
            sb.AppendLine("--------------------------------------------------");
            sb.AppendLine(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff") + " UTC");
            sb.AppendLine("Message: " + message);
            sb.AppendLine("Exception Type: " + ex.GetType().FullName);
            sb.AppendLine("Exception Message: " + ex.Message);
            sb.AppendLine("StackTrace:");
            sb.AppendLine(ex.StackTrace ?? "");
            if (ex.InnerException != null)
            {
                sb.AppendLine("InnerException: " + ex.InnerException.Message);
                sb.AppendLine(ex.InnerException.StackTrace ?? "");
            }
            sb.AppendLine();

            WriteToFile(sb.ToString());
        }

        private static void WriteToFile(string text)
        {
            var file = Path.Combine(_logDir, $"log-{DateTime.UtcNow:yyyy-MM-dd}.txt");
            lock (_lock)
            {
                File.AppendAllText(file, text);
            }
        }

    }
}