using AutomatedDesktopBackgroundLibrary.ImageFileProcessing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace AutomatedDesktopBackgroundLibrary
{
    public class ImageFileProcessor : IImageFileProcessor
    {
        public EventHandler<List<ImageModel>> OnFileAltered { get; set; }
        public EventHandler<ImageModel> OnWallPaperUpdate { get; set; }
        private readonly IDatabaseConnector _database;
        private List<ImageModel> _images = new List<ImageModel>();

        public ImageFileProcessor(IDatabaseConnector database)
        {
            _database = database;
        }

        public ImageModel CreateEntry(ImageModel entry)
        {
            _database.CreateEntry(entry, entry.InfoFileDir);
            OnFileAltered?.Invoke(this, LoadAllEntries());

            return entry;
        }

        public void DeleteEntry(ImageModel entry)
        {
            _database.Delete(entry, entry.InfoFileDir);
            OnFileAltered?.Invoke(this, LoadAllEntries());
        }

        public List<ImageModel> LoadAllEntries()
        {
            _images = _database.Load<ImageModel>(FileType.ImageInfo);
            return _images;
        }

        public void RemoveAllImagesByInterest(InterestModel interest)
        {
            try
            {
                
                List<ImageModel> images = DataKeeper.GetFileSnapShot().AllImages.AllImagesByInterest(interest);
                List<ImageModel> updatedImages = RemoveFavoritePhotos(images);
                bool favoritesExist = images.Count > updatedImages.Count;
                if (favoritesExist)
                {
                    DeleteNonFavoritePhotos(updatedImages);
                }
                else
                {
                    DeleteEntireFolder(interest.Name);
                }
                
            }
            catch (Exception e)
            {
                Debug.WriteLine("Couldn't delete directory  " + e.InnerException.ToString());
            }
        }
        private void DeleteNonFavoritePhotos(List<ImageModel> images)
        {
            foreach (ImageModel image in images)
            {
                DeleteEntry(image);
                _database.DeleteFile(image.LocalUrl);
            }
        }
        private void DeleteEntireFolder(string interestName)
        {
            string dir = $@"{InternalFileDirectorySystem.ImagesFolder}\{interestName}";
            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
            }
        }
        private List<ImageModel> RemoveFavoritePhotos(List<ImageModel> entries)
        {
            List<ImageModel> updatedEntires = new List<ImageModel>();
            foreach (ImageModel image in entries)
            {
                if(!image.IsFavorite)
                {
                    updatedEntires.Add(image);
                }
            }
            return updatedEntires;
        }

        public void UpdateWallPaper(ImageModel entry, ImageModel oldWallpaper = null)
        {
            if (oldWallpaper != null)
            {
                oldWallpaper.IsWallpaper = false;
                CreateEntry(oldWallpaper);
            }
            entry.IsWallpaper = true;
            CreateEntry(entry);
            OnWallPaperUpdate?.Invoke(this, entry);
        }
    }
}