using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
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
        public static ImageModel defaultWallpaper;
        public static bool InCollectionRefresh = false;
        public static IDatabaseConnector Database = new TextFileConnector();

    }
}
