using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary
{ 
    public class DataStorageManager//:TextConnectorProcessor
    {
        #region Singleton/CTOR
        public DataStorageManager Instance
        { get{
                if(Instance == null)
                {
                    Instance = new DataStorageManager();
                   
                }
                return Instance;
            } set { } }
        //Hook this up to initialize the session memory
        private DataStorageManager()
        {
            
        }
        #endregion
        #region SessionMemory
        private readonly List<ImageModel> _allImages;
        private readonly List<ImageModel> _hatedImages;
        private readonly List<ImageModel> _favoriteImages;
        private readonly List<InterestModel> _allInterests;
        #endregion
        #region File Directories
        private static string FileSavePath
        {
            get
            {
                string baseUrl = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string fullUrl = baseUrl + @"\DesktopBackgrounds";
                return Directory.CreateDirectory(fullUrl).FullName;

            }
            set { FileSavePath = value; }
        }
        private static string InterestFile
        {
            get
            {
                return FullFilePath("Interests.csv");
            }
            set
            {
            }
        }
        private static string ImageFile
        {
            get
            {
                return FullFilePath("Images.csv");
            }
            set
            {
            }
        }
        private static string CurrentWallpaperFile
        {
            get
            {
                return FullFilePath("CurrentWallpaper.csv");
            }
            set
            {
            }
        }
        private static string FavoritesFile
        {
            get
            {
                return FullFilePath("Favorites.csv");
            }
             set
            {
            }
        }
        private static string HatedFile
        {
            get
            {
                return FullFilePath("Hated.csv");
            }
             set
            {
            }
        }
        /// <summary>
        /// Use this method to add a file to the root directory
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static string FullFilePath(string fileName)
        {
            return $@"{FileSavePath}\{fileName}";
        }
        #endregion
        #region CRUD Interest Models
        public static void CreateInterestEntry(InterestModel interest)
        {

        }
        public static List<InterestModel> ReadAllInterestModels()
        {
            List<InterestModel> models = new List<InterestModel>();
            return models;
        }
        public static void UpdateInterestEntry(InterestModel oldInterest, InterestModel newInterest)
        {

        }
        public static void DeleteInterestEntry(InterestModel interestToDelete)
        {

        }
        #endregion
        #region CRUD Image Models
        public static void CreateImageEntry(ImageModel image)
        {

        }
        public static List<ImageModel> ReadAllImageEntries()
        {
            List<ImageModel> images = new List<ImageModel>();
            return images;
        }
        public static void UpdateImageEntry(ImageModel image)
        {

        }
        public static void DeleteImageEntry(ImageModel image)
        {

        }
        
        #endregion
        #region CRUD Favorites
        public static void CreateFavoriteEntry(ImageModel image)
        {

        }
        public static List<ImageModel> ReadAllFavoriteEntries()
        {
            List<ImageModel> models = new List<ImageModel>();
            return models;
        }
        public static void UpdateFavoritesEntry(ImageModel oldFavorite,ImageModel newFavorite)
        {

        }
        public static void DeleteFavoriteEntry(ImageModel imageToDelete)
        {

        }
        #endregion
        #region CRUD Hated Images
        public static void CreateHatedImageEntry(ImageModel image)
        {

        }
        public static List<ImageModel> ReadAllHatedImageEntries()
        {
            List<ImageModel> images = new List<ImageModel>();
            return images;
        }
        public static void UpdateHatedImageEntry(ImageModel image)
        {

        }
        public static void DeleteHatedImageEntry(ImageModel image)
        {

        }
        #endregion



    }
}
