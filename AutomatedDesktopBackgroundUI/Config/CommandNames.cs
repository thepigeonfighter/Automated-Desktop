using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundUI.Config
{


        public enum CommandNames 
    {
        CheckConnection,
        LikeImage,
        HateImage,
        AddInterest,
        RemoveInterest,
        AcceptSettings,
        RevertSettings,
        SkipWallpaper,
        DownloadCollection,
        StartBackgroundRefreshing,
        StopBackgroundRefreshing,
        StartCollectionRefreshing,
        StopCollectionRefreshing,
        SettingsChanged,
        ResetApplication,
        QuitApplication
    }
    
}
