using AutomatedDesktopBackgroundUI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundUI.SessionData
{
    public interface ISessionContext:INotifyPropertyChanged
    {

        List<InterestInfoModel> Interests { get; set; }
        void AddInterest(InterestInfoModel item);
        InterestInfoModel SelectedInterest { get; set; }
        CurrentImageModel CurrentWallpaper { get; set; }
        RefreshStateModel CurrentRefreshState { get; set; }
        bool IsDownloading { get; set; }
        SettingsModel CurrentSettings { get; set; }
    }
}
