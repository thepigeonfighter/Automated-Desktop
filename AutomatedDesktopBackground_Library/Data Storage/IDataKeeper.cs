using System.Collections.Generic;

namespace AutomatedDesktopBackgroundLibrary
{
    public interface IDataKeeper
    {
        ImageModel AddImage(ImageModel entry);
        void AddInterest(InterestModel interest);
        void DeleteGroupOfFiles(List<ImageModel> images);
        void DeleteImage(ImageModel image, bool KeepRecord);
        void DeleteImageAndImageInfoEntry(ImageModel entry);
        void DeleteInterest(InterestModel interest);
        IFileCollection GetFileSnapShot();
        IFileCollection GetFreshFileSnapShot();
        void RegisterFileListener(IFileListener fileListener);
        void ResetApplication();
        void UpdateAllLists();
        void UpdateWallpaper(string url);
    }
}