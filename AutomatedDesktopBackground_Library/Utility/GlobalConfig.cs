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
        static GlobalConfig()
        {
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
        public static object FileSavePath
        {
            get
            {
                return Directory.CreateDirectory(Path.Combine(Path.GetPathRoot(Environment.CurrentDirectory),@"data\DesktopBackgrounds")).FullName;
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
        private static bool _backgroundUpdating = false;
        public static bool BackGroundUpdating
        {
            get {
                return _backgroundUpdating;
            }
            set {
                _backgroundUpdating = value;
                EventSystem.InvokeConfigSettingChanged();
               
            }
        }
        private static bool _collectionUpdating = false;
        public static bool CollectionUpdating
        {
            get { return _collectionUpdating; }
            set
            {
                _collectionUpdating = value;
                EventSystem.InvokeConfigSettingChanged();
              
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
