namespace AutomatedDesktopBackgroundLibrary
{
    public static class GlobalConfig
    {
        public static bool IsConnected()
        {
            return InterenetConnectionChecker.CheckConnection();
        }

        public static JobManager JobManager = new JobManager();
        public static EventSystem EventSystem = new EventSystem();
        public static bool InCollectionRefresh = false;
        public static bool CollectionsRefreshing;
        public static bool BackgroundRefreshing;
        //TODO remove this smelly bool 
        public static bool SettingsWindowOpen;
    }
}