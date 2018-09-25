using AutomatedDesktopBackgroundLibrary.StringExtensions;
using System;
using System.Collections.Generic;

namespace AutomatedDesktopBackgroundLibrary
{
    public class HatedImageProcessor : IImageFileProcessor
    {
        public EventHandler<List<ImageModel>> OnFileAltered { get; set; }
        private const string HatedFile = "HatedImages.csv";
        private readonly IDatabaseConnector _database;

        public HatedImageProcessor(IDatabaseConnector database)
        {
            _database = database;
        }

        //TODO handle the deleting of images here
        public ImageModel CreateEntry(ImageModel entry)
        {
            entry.IsDownloaded = false;
            _database.CreateEntry(entry, HatedFile.FullFilePath());

            OnFileAltered?.Invoke(this, LoadAllEntries());
            return entry;
        }

        public void DeleteEntry(ImageModel entry)
        {
            _database.Delete(entry, HatedFile.FullFilePath());
            OnFileAltered?.Invoke(this, LoadAllEntries());
        }

        public void OverwriteEntries(List<ImageModel> items)
        {
            _database.SaveToFile(items, HatedFile.FullFilePath());
            OnFileAltered?.Invoke(this, LoadAllEntries());
        }

        public List<ImageModel> LoadAllEntries()
        {
            return _database.Load<ImageModel>(HatedFile.FullFilePath());
        }

        public List<ImageModel> UpdateEntries(List<ImageModel> newEntries)
        {
            List<ImageModel> output = _database.Update(newEntries, HatedFile.FullFilePath());
            OnFileAltered?.Invoke(this, LoadAllEntries());
            return output;
        }

        public ImageModel UpdateEntries(ImageModel entry)
        {
            _database.Update(entry, HatedFile.FullFilePath());
            return entry;
        }
    }
}