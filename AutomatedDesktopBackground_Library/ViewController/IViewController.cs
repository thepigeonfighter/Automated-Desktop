using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary
{
    public interface IViewController
    {
        void DownloadComplete(bool sucess);
        void DownloadPercentage(int percentComplete);
    }
}
