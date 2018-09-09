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
        IScheduler scheduler;
        private const string BackgroundJob = "bgjob";
        private const string CollectionsJob = "cj";
  
        public  async Task UpdateBackGroundAsync()
        {
            
            if (!GlobalConfig.BackGroundUpdating)
            {
                try
                {
                    NameValueCollection prop = new NameValueCollection
                {
                    {"quartz.serializer.type", "binary" }
                };
                    StdSchedulerFactory factory = new StdSchedulerFactory(prop);
                    scheduler = await Task.Run(()=>factory.GetScheduler());
                    await scheduler.Start();
                    IJobDetail jobDetail = JobBuilder.Create<ChangeBackgroundJob>().WithIdentity(BackgroundJob).Build();
                    int refreshRate =(int) Math.Round( Scheduler.ScheduleManager.BackgroundRefreshSetting().TotalSeconds);
                    ITrigger trigger = TriggerBuilder.Create().WithIdentity(BackgroundJob).StartNow().WithSimpleSchedule(x =>
                     x.WithIntervalInSeconds(refreshRate).RepeatForever()).Build();
                    await Task.Run(()=>scheduler.ScheduleJob(jobDetail, trigger));
                   
                    GlobalConfig.BackGroundUpdating = true;
                }
                catch (SchedulerException se)
                {
                    throw new Exception(se.ToString());
                }

            }
        }

          //  To Run a line of parrallel processes  have a list of tasks that are running and then at the end of the method 
         //   use a  await Task.WhenAll(List<Task>) to stop the method at the until the process is complete
         /*
        public  JobManager()
        {
            BuildScheduler();
        }
        private async Task BuildScheduler()
        {
            NameValueCollection prop = new NameValueCollection
                {
                    {"quartz.serializer.type", "binary" }
                };
            StdSchedulerFactory factory = new StdSchedulerFactory(prop);
            scheduler = await Task.Run(()=>factory.GetScheduler());
            
        }
        
        public void UpdateBackGround()
        {

            if (!GlobalConfig.BackGroundUpdating)
            {
                try
                {
                    scheduler.Start();
                    IJobDetail jobDetail = JobBuilder.Create<ChangeBackgroundJob>().WithIdentity(BackgroundJob).Build();
                    int refreshRate = (int)Math.Round(Scheduler.ScheduleManager.BackgroundRefreshSetting().TotalSeconds);
                    ITrigger trigger = TriggerBuilder.Create().WithIdentity(BackgroundJob).StartNow().WithSimpleSchedule(x =>
                     x.WithIntervalInSeconds(refreshRate).RepeatForever()).Build();
                     scheduler.ScheduleJob(jobDetail, trigger);

                    GlobalConfig.BackGroundUpdating = true;
                }
                catch (SchedulerException se)
                {
                    throw new Exception(se.ToString());
                }

            }
        }
        */
        public async Task StopScheduler()
        {
            if (scheduler != null)
            {
                GlobalConfig.BackGroundUpdating = false;
                GlobalConfig.CollectionUpdating = false;
                await Task.Run(()=>scheduler.Shutdown());
            }
        }
        public async Task StopBackgroundChange()
        {

            JobKey key = JobKey.Create(BackgroundJob);
            if (await Task.Run(() =>scheduler.CheckExists(key)))
            {
                await Task.Run(()=>scheduler.DeleteJob(key));
                GlobalConfig.BackGroundUpdating = false;
                
            }
            else throw new Exception("Job not found!");
        }
        public async Task StopCollectionChange()
        {
            JobKey key = JobKey.Create(CollectionsJob);
            if (await Task.Run(()=>scheduler.CheckExists(key)))
            {
                GlobalConfig.CollectionUpdating = false;
                await Task.Run(()=>scheduler.DeleteJob(key));
            }
            else throw new Exception("Job not found!");
        }
        public async Task StartCollectionChange()
        {
            try
            {
                GlobalConfig.CollectionUpdating = true;
            }
            catch 
            {

                throw;
            }
        }
    }
}
