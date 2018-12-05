using AutomatedDesktopBackgroundUI.Models;
using AutomatedDesktopBackgroundUI.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundUI.SessionData
{
    public interface IDataAccess:INotifyPropertyChanged
    {   

        List<InterestInfoModel> GetAllInterests();
        void RemoveInterest(InterestInfoModel item);
        Task<InterestInfoModel> AddInterest(InterestInfoModel item);
        CurrentImageModel GetCurrentWallpaper();
        RefreshStateModel GetCurrentRefreshState();
        SettingsModel GetCurrentSettings();
        void SetCurrentRefreshState();
        void LikeCurrentImage();
        void SkipCurrentImage();
        void DownloadCollection(InterestInfoModel item);
        void HateCurrentImage();
        void UpdateSettings(SettingsModel settings);
        void ResetApplication();
        void SetRefreshState(EventContainer eventContainer);
        void UpdateContextMenu(EventContainer eventContainer);

    }
}
