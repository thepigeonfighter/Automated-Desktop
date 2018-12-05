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
        private static bool _isCollectionRefreshing;
        public static bool CollectionsRefreshing
        {
            get
            {
                return _isCollectionRefreshing;
            }
            set
            {
                if (value != CollectionsRefreshing)
                {
                    _isCollectionRefreshing = value;
                    EventSystem.InvokeRefreshStatusChangedEvent();
                }

            }
        }
        private static bool _backgroundRefreshing;
        public static bool BackgroundRefreshing
        {
            get
            {
                return _backgroundRefreshing;
            }
            set
            {
                if (value != BackgroundRefreshing)
                {
                    _backgroundRefreshing = value;
                    EventSystem.InvokeRefreshStatusChangedEvent();
                }

            }
        }

        
    }
}