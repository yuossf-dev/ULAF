namespace EntityFrameWork_Pro.Services
{
    public static class DatabaseMode
    {
        private static bool _isOnline = true; // Changed to true - use Firebase by default

        public static bool IsOnline
        {
            get => _isOnline;
            set => _isOnline = value;
        }

        public static string GetCurrentMode()
        {
            return _isOnline ? "Online (Firebase)" : "Offline (SQL Server)";
        }
    }
}
