using AutomatedDesktopBackgroundLibrary.StringExtensions;
using System;
using System.Collections.Generic;

namespace AutomatedDesktopBackgroundLibrary
{
    public class WallpaperFileProcessor : IWallPaperFileProcessor
    {
        private readonly IDatabaseConnector _database;
        private const string WallpaperFile = "Wallpaper.csv";

        public EventHandler<ImageModel> OnWallPaperUpdate { get; set; }

        public WallpaperFileProcessor(IDatabaseConnector database)
        {
            _database = database;
        }

        public ImageModel Load()
        {
            List<ImageModel> images = _database.Load<ImageModel>(WallpaperFile.FullFilePath());
            if (images.Count > 0)
            {
                return images[0];
            }
            return null;
        }

        public ImageModel Update(ImageModel entry)
        {
            _database.Update(entry, WallpaperFile.FullFilePath());
            OnWallPaperUpdate?.Invoke(this, entry);
            return entry;
        }
    }
}