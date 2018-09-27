using System;
using System.Collections.Generic;

namespace AutomatedDesktopBackgroundLibrary
{
    public interface IImageFileProcessor
    {
        EventHandler<ImageModel> OnWallPaperUpdate { get; set; }

        EventHandler<List<ImageModel>> OnFileAltered { get; set; }

        ImageModel CreateEntry(ImageModel entry);

        List<ImageModel> LoadAllEntries();

        void DeleteEntry(ImageModel entry);

        void RemoveAllImagesByInterest(InterestModel interest);

        void UpdateWallPaper(ImageModel entry, ImageModel oldWallpaper = null);
    }
}