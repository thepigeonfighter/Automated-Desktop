using AutomatedDesktopBackgroundLibrary;
using AutomatedDesktopBackgroundUI.Models;
using AutomatedDesktopBackgroundUI.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundUI.Utility
{
    public class RefreshCycleServices : IRefreshCycleServices
    {

        public void StartBackgroundJob(RefreshStateModel model)
        {
            if(!model.IsBackgroundRefreshing)
            {
                Task.Run(() => GlobalConfig.JobManager.StartBackgroundUpdatingAsync(Settings.Default.BGCycle)).ConfigureAwait(false);
                GlobalConfig.BackgroundRefreshing = true;
            }
        }

        public void StartCollectionJob(RefreshStateModel model)
        {
            if(!model.IsCollectionRefreshing)
            {
                Task.Run(() => GlobalConfig.JobManager.StartCollectionUpdatingAsync(Settings.Default.CollectionCycle)).ConfigureAwait(false);
                GlobalConfig.CollectionsRefreshing = true;
            }
        }

        public void StopBackgroundJob(RefreshStateModel model)
        {
            if(model.IsBackgroundRefreshing)
            {
                Task.Run(() => GlobalConfig.JobManager.StopBackgroundUpdatingAsync()).ConfigureAwait(false);
                GlobalConfig.BackgroundRefreshing = false;
            }
        }

        public void StopCollectionJob(RefreshStateModel model)
        {
            if(model.IsCollectionRefreshing)
            {
                Task.Run(() => GlobalConfig.JobManager.StopCollectionUpdatingAsync()).ConfigureAwait(false);
                GlobalConfig.CollectionsRefreshing = false;
            }
        }

        public async Task UpateBackgroundJobTimeAsync(RefreshStateModel model)
        {
            if(model.IsBackgroundRefreshing)
            {
                await GlobalConfig.JobManager.StopBackgroundUpdatingAsync();
                StartBackgroundJob(model);
            }
            else
            {
                StartBackgroundJob(model);
            }
        }

        public async Task UpdateCollectionJobTimeAsync(RefreshStateModel model)
        {
           if(model.IsCollectionRefreshing)
            {
                await GlobalConfig.JobManager.StopCollectionUpdatingAsync();
                StartCollectionJob(model);
            }
            else
            {
                StartCollectionJob(model);
            }
        }
    }
}
