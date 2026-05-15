using System;

namespace RetailingApp.Patterns
{
    public class AppLogger
    {
        private static AppLogger? _instance;
        private static readonly object _lock = new object();

        private AppLogger() { }

        public static AppLogger Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new AppLogger();
                    }
                    return _instance;
                }
            }
        }

        public void LogInfo(string message)
        {
            Console.WriteLine($"[INFO] {DateTime.Now}: {message}");
        }

        public void LogError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] {DateTime.Now}: {message}");
            Console.ResetColor();
        }
    }
}
