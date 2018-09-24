using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary
{
    public interface IDataStorage
    {
        IImageFileProcessor ImageFileProcessor { get; set; }
        IImageFileProcessor HatedImageFileProcessor { get; set; }
        IImageFileProcessor FavoritedImageFileProcessor { get; set; }

        IInterestFileProcessor InterestFileProcessor { get; set; }

        IWallPaperFileProcessor WallPaperFileProcessor { get; set; }

        IDatabaseConnector Database { get; set; }
        
        IFileCollection FileCollection { get; set; }

        void UpdateAllLists();
        void RegisterFileListeners(IFileListener fileListener);
        void ResetApplication();


    }
}
