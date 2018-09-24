using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomatedDesktopBackgroundLibrary.StringExtensions;
namespace AutomatedDesktopBackgroundLibrary
{
    public class HatedImageProcessor : IImageFileProcessor
    {
        public EventHandler<List<ImageModel>> OnFileAltered { get; set; }
        private const string HatedFile = "HatedImages.csv";
        private IDatabaseConnector _database;
        public HatedImageProcessor(IDatabaseConnector database)
        {
            _database = database;
        }
        //TODO handle the deleting of images here
        public ImageModel CreateEntry(ImageModel entry)
        {
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
