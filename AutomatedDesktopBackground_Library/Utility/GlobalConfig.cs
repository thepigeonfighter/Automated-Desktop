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
        private static ImageModel _currentBackground;
        public static ImageModel CurrentWallpaper {
            get
            {
                return _currentBackground;
            }
            set
            {
                _currentBackground = value;
                EventSystem.InvokeUpdateBackroundEvent();
            }
        }
        public static string FileSavePath
        {
            get
            {
                string baseUrl = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string fullUrl =  baseUrl+@"\DesktopBackgrounds";
                return Directory.CreateDirectory(fullUrl).FullName;
                
            }
            set { FileSavePath = value; }
        }
        /// <summary>
        /// The full file path to where the interests are being saved
        /// </summary>
        public static string InterestFile
        {
            get {
                return FullFilePath("Interests.csv");
            }
            private set {
            }
        }
        /// <summary>
        /// The full file path to where the images are being saved
        /// </summary>
        public static string ImageFile
        {
            get
            {
                return FullFilePath("Images.csv");
            }
            private set
            {
            }
        }
        public static string CurrentWallpaperFile
        {
            get
            {
                return FullFilePath("CurrentWallpaper.csv");
            }
            private set
            {
            }
        }
        public static string FavoritesFile
        {
            get
            {
                return FullFilePath("Favorites.csv");
            }
            private set
            {
            }
        }
        public static string HatedFile
        {
            get
            {
                return FullFilePath("Hated.csv");
            }
            private set
            {
            }
        }
        /// <summary>
        /// Use this method to add a file to the root directory
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string FullFilePath(string fileName)
        {
            return $@"{FileSavePath}\{fileName}";
        }
        public enum TimeSettings
        {
            Minutes,
            Hours,
            Days
        }
        public static JobManager JobManager = new JobManager();
        public static EventSystem EventSystem = new EventSystem();


    }
}
