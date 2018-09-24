using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomatedDesktopBackgroundLibrary.StringExtensions;

namespace AutomatedDesktopBackgroundLibrary
{
    public class FavoriteImageProcessor : IImageFileProcessor
    {
        private const string FavoriteFile = "FavoriteImages.csv";
        private DirectoryInfo favoriteDir;
        private IDatabaseConnector _database;
        public EventHandler<List<ImageModel>> OnFileAltered { get ; set; }

        public FavoriteImageProcessor (IDatabaseConnector database)
        {
            string favoriteFolder = "Favorites";
            favoriteDir = Directory.CreateDirectory(favoriteFolder.FullFilePath());
            _database = database;
        }
        //TODO Add the Copy functionality here
        /*
         * The favorites file is not getting updated reliably. For some reason it is 
         * entering the wrong entry every once in a while also not removing the image entries reliably
         * also it is making double entries into the favorite file 
         * 
         */
        public ImageModel CreateEntry(ImageModel entry)
        {
            
            _database.CopyImage(entry, favoriteDir.FullName);   
            _database.CreateEntry(entry, FavoriteFile.FullFilePath());
            DataKeeper.DeleteImage(entry);
             OnFileAltered?.Invoke(this, LoadAllEntries());
            
            return entry;
        }

        public void DeleteEntry(ImageModel entry)
        {
            _database.Delete(entry, FavoriteFile.FullFilePath());
            OnFileAltered?.Invoke(this, LoadAllEntries());
        }

        public void OverwriteEntries(List<ImageModel> items)
        {
            _database.SaveToFile(items, FavoriteFile.FullFilePath());
            OnFileAltered?.Invoke(this, LoadAllEntries());
        }

        public List<ImageModel> LoadAllEntries()
        {
            return _database.Load<ImageModel>(FavoriteFile.FullFilePath());
        }

        public List<ImageModel> UpdateEntries(List<ImageModel> newEntries)
        {
            List<ImageModel> output =  _database.Update(newEntries, FavoriteFile.FullFilePath());
            OnFileAltered?.Invoke(this, LoadAllEntries());
            return output;
        }

        public ImageModel UpdateEntries(ImageModel entry)
        {
            _database.Update(entry, FavoriteFile.FullFilePath());
            return entry;
        }
    }
}
