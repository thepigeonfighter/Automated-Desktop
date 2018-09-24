using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomatedDesktopBackgroundLibrary.DataConnection;
using AutomatedDesktopBackgroundLibrary.StringExtensions;
namespace AutomatedDesktopBackgroundLibrary
{
    public class ImageFileProcessor: IImageFileProcessor
    {
        public EventHandler<List<ImageModel>> OnFileAltered { get; set; }
        private const string ImageFile = "Images.csv";
        private IDatabaseConnector _database;
        public ImageFileProcessor(IDatabaseConnector database)
        {
            _database = database;
        }

        public ImageModel CreateEntry(ImageModel entry)
        {
            _database.CreateEntry(entry,ImageFile.FullFilePath());
            OnFileAltered?.Invoke(this, LoadAllEntries());
            return entry;
        }

        public void DeleteEntry(ImageModel entry)
        {
            _database.Delete(entry, ImageFile.FullFilePath());
            OnFileAltered?.Invoke(this, LoadAllEntries());
        }

        public void OverwriteEntries(List<ImageModel> items)
        {
            _database.SaveToFile(items, ImageFile.FullFilePath());
            OnFileAltered?.Invoke(this, LoadAllEntries());
        }

        public List<ImageModel> LoadAllEntries()
        {
            return _database.Load<ImageModel>(ImageFile.FullFilePath());
        }

        public List<ImageModel> UpdateEntries(List<ImageModel> newEntries)
        {
           List<ImageModel> output = _database.Update(newEntries, ImageFile.FullFilePath());
            OnFileAltered?.Invoke(this, LoadAllEntries());
            return output;
        }

        public ImageModel UpdateEntries(ImageModel entry)
        {
            _database.Update(entry, ImageFile.FullFilePath());
            return entry;
        }
    }
}
