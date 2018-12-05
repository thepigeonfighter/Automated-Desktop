using AutomatedDesktopBackgroundUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundUI.Utility
{
    public interface IRefreshCycleServices
    {
        Task UpdateCollectionJobTimeAsync(RefreshStateModel model);
        Task UpateBackgroundJobTimeAsync(RefreshStateModel model);
        void StartBackgroundJob(RefreshStateModel model);
        void StopBackgroundJob(RefreshStateModel model);
        void StartCollectionJob(RefreshStateModel model);
        void StopCollectionJob(RefreshStateModel model);
    }
}
