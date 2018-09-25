using AutomatedDesktopBackgroundLibrary.StringExtensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace AutomatedDesktopBackgroundLibrary
{
    public class FavoriteImageProcessor : IImageFileProcessor
    {
        private const string FavoriteFile = "FavoriteImages.csv";
        private readonly DirectoryInfo favoriteDir;
        private readonly IDatabaseConnector _database;
        public EventHandler<List<ImageModel>> OnFileAltered { get; set; }

        public FavoriteImageProcessor(IDatabaseConnector database)
        {
            const string favoriteFolder = "Favorites";
            favoriteDir = Directory.CreateDirectory(favoriteFolder.FullFilePath());
            _database = database;
        }

        public ImageModel CreateEntry(ImageModel entry)
        {
            _database.CopyImage(entry, favoriteDir.FullName);
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
            List<ImageModel> output = _database.Update(newEntries, FavoriteFile.FullFilePath());
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