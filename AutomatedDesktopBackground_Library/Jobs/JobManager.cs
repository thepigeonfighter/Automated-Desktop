using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;


namespace AutomatedDesktopBackgroundLibrary
{
    public  class JobManager
    {
        private IScheduler scheduler;
        private const string BackgroundJob = "bgjob";
        private const string CollectionsJob = "cj";
  
        public  async Task StartBackgroundUpdatingAsync()
        {


            try
            {
                NameValueCollection prop = new NameValueCollection
                {
                    {"quartz.serializer.type", "binary" }
                };
                StdSchedulerFactory factory = new StdSchedulerFactory(prop);
                scheduler = await Task.Run(() => factory.GetScheduler());
                await scheduler.Start();
                IJobDetail jobDetail = JobBuilder.Create<ChangeBackgroundJob>().WithIdentity(BackgroundJob).Build();
                int refreshRate = (int)Math.Round(Scheduler.ScheduleManager.BackgroundRefreshSetting().TotalSeconds);
                ITrigger trigger = TriggerBuilder.Create().WithIdentity(BackgroundJob).StartAt(DateTime.Now.AddSeconds(refreshRate)).WithSimpleSchedule(x =>
                 x.WithIntervalInSeconds(refreshRate).RepeatForever()).Build();
                await Task.Run(() => scheduler.ScheduleJob(jobDetail, trigger));


            }
            catch (SchedulerException se)
            {
                throw new Exception(se.ToString());
            }

            }
        
        public async Task StartCollectionUpdatingAsync()
        {

    
                try
                {
            NameValueCollection prop = new NameValueCollection
                {
                    {"quartz.serializer.type", "binary" }
                };
                    StdSchedulerFactory factory = new StdSchedulerFactory(prop);
                    scheduler = await Task.Run(() => factory.GetScheduler());
                    await scheduler.Start();
                    IJobDetail jobDetail = JobBuilder.Create<CollectionRefreshJob>().WithIdentity(CollectionsJob).Build();
                    int refreshRate = (int)Math.Round(Scheduler.ScheduleManager.CollectionRefreshSetting().TotalSeconds);
                    ITrigger trigger = TriggerBuilder.Create().WithIdentity(CollectionsJob).StartAt(DateTime.Now.AddSeconds(refreshRate)).WithSimpleSchedule(x =>
                     x.WithIntervalInSeconds(refreshRate).RepeatForever()).Build();
                    await Task.Run(() => scheduler.ScheduleJob(jobDetail, trigger));

                }
                catch (SchedulerException se)
                {
                    throw new Exception(se.ToString());
                }

            
        }

        public async Task StopSchedulerAsync()
        {
            if (scheduler != null)
            {
                await Task.Run(()=>scheduler.Shutdown());
            }
        }
        public async Task StopBackgroundUpdatingAsync()
        {

            JobKey key = JobKey.Create(BackgroundJob);
            if (await Task.Run(() =>scheduler.CheckExists(key)))
            {
                await Task.Run(()=>scheduler.DeleteJob(key));
                
            }
            else throw new Exception("Job not found!");
        }
        
        public async Task StopCollectionUpdatingAsync()
        {
            JobKey key = JobKey.Create(CollectionsJob);
            if (await Task.Run(()=>scheduler.CheckExists(key)))
            {
                await Task.Run(()=>scheduler.DeleteJob(key));
            }
           else throw new Exception("Job not found!");
        }
        
        public async Task<bool> JobRunning(JobType jobType)
        {
            switch (jobType)
            {
                case JobType.BackgroundRefresh:
                    return await Task.Run(()=>scheduler.CheckExists(JobKey.Create(BackgroundJob)));

                case JobType.CollectionRefresh:
                    return await Task.Run(() => scheduler.CheckExists(JobKey.Create(CollectionsJob)));
                default:
                    return false;
                    
            }
        }

    }
}
