using AutomatedDesktopBackgroundLibrary.StringExtensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace AutomatedDesktopBackgroundLibrary
{
    public class ImageFileProcessor : IImageFileProcessor
    {
        public EventHandler<List<ImageModel>> OnFileAltered { get; set; }
        private const string ImageFile = "Images.csv";
        private readonly IDatabaseConnector _database;
        private List<ImageModel> _images = new List<ImageModel>();
        public ImageFileProcessor(IDatabaseConnector database)
        {
            _database = database;
        }

        public ImageModel CreateEntry(ImageModel entry)
        {
            if (ValidEntry(entry))
            {
                _database.CreateEntry(entry, ImageFile.FullFilePath());
                OnFileAltered?.Invoke(this, LoadAllEntries());
            }
            return entry;
        }
        private bool ValidEntry(ImageModel image)
        {
            bool valid = true;
            foreach (var i in _images)
            {
                if(i.Name == image.Name)
                {
                    valid = false;
                }
            }
            string folderName = Directory.GetParent(image.LocalUrl).Name;
            if(folderName == "Favorites")
            {
                return false;
            }
            return valid;
        }

        public void DeleteEntry(ImageModel entry)
        {
            _database.Delete(entry, ImageFile.FullFilePath());
            OnFileAltered?.Invoke(this, LoadAllEntries());
        }

        public void OverwriteEntries(List<ImageModel> items)
        {
            List<ImageModel> validEntries = new List<ImageModel>();
            foreach(ImageModel i in items)
            {
                if(!ValidEntry(i))
                {
                    validEntries.Add(i);
                }
            }
            _database.SaveToFile(validEntries, ImageFile.FullFilePath());
            OnFileAltered?.Invoke(this, LoadAllEntries());
        }

        public List<ImageModel> LoadAllEntries()
        {
            _images = _database.Load<ImageModel>(ImageFile.FullFilePath());
            return _images;
        }

        public List<ImageModel> UpdateEntries(List<ImageModel> newEntries)
        {
            foreach (ImageModel i in newEntries)
            {
                if (!ValidEntry(i))
                {
                    newEntries.Remove(i);
                }
            }
            List<ImageModel> output = _database.Update(newEntries, ImageFile.FullFilePath());
            OnFileAltered?.Invoke(this, LoadAllEntries());
            return output;
        }

        public ImageModel UpdateEntries(ImageModel entry)
        {
            if (ValidEntry(entry))
            {
                _database.Update(entry, ImageFile.FullFilePath());
                OnFileAltered?.Invoke(this, LoadAllEntries());
            }
            return entry;
        }
    }
}