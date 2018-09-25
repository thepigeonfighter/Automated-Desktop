using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary
{
    public class JobManager
    {
        private IScheduler scheduler;
        private const string BackgroundJob = "bgjob";
        private const string CollectionsJob = "cj";

        public async Task StartBackgroundUpdatingAsync()
        {
            try
            {
                NameValueCollection prop = new NameValueCollection
                {
                    {"quartz.serializer.type", "binary" }
                };
                StdSchedulerFactory factory = new StdSchedulerFactory(prop);
                scheduler = await Task.Run(() => factory.GetScheduler()).ConfigureAwait(false);
                await scheduler.Start().ConfigureAwait(false);
                IJobDetail jobDetail = JobBuilder.Create<ChangeBackgroundJob>().WithIdentity(BackgroundJob).Build();
                int refreshRate = (int)Math.Round(Scheduler.ScheduleManager.BackgroundRefreshSetting().TotalSeconds);
                ITrigger trigger = TriggerBuilder.Create().WithIdentity(BackgroundJob).StartAt(DateTime.Now.AddSeconds(refreshRate)).WithSimpleSchedule(x =>
                 x.WithIntervalInSeconds(refreshRate).RepeatForever()).Build();
                await Task.Run(() => scheduler.ScheduleJob(jobDetail, trigger)).ConfigureAwait(false);
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
                scheduler = await Task.Run(() => factory.GetScheduler()).ConfigureAwait(false);
                await scheduler.Start().ConfigureAwait(false);
                IJobDetail jobDetail = JobBuilder.Create<CollectionRefreshJob>().WithIdentity(CollectionsJob).Build();
                int refreshRate = (int)Math.Round(Scheduler.ScheduleManager.CollectionRefreshSetting().TotalSeconds);
                ITrigger trigger = TriggerBuilder.Create().WithIdentity(CollectionsJob).StartAt(DateTime.Now.AddSeconds(refreshRate)).WithSimpleSchedule(x =>
                 x.WithIntervalInSeconds(refreshRate).RepeatForever()).Build();
                await Task.Run(() => scheduler.ScheduleJob(jobDetail, trigger)).ConfigureAwait(false);
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
                await Task.Run(() => scheduler.Shutdown()).ConfigureAwait(false);
            }
        }

        public async Task StopBackgroundUpdatingAsync()
        {
            JobKey key = JobKey.Create(BackgroundJob);
            if (await Task.Run(() => scheduler.CheckExists(key)).ConfigureAwait(false))
            {
                await Task.Run(() => scheduler.DeleteJob(key)).ConfigureAwait(false);
            }
            else
            {
                throw new Exception("Job not found!");
            }
        }

        public async Task StopCollectionUpdatingAsync()
        {
            JobKey key = JobKey.Create(CollectionsJob);
            if (await Task.Run(() => scheduler.CheckExists(key)).ConfigureAwait(false))
            {
                await Task.Run(() => scheduler.DeleteJob(key)).ConfigureAwait(false);
            }
            else
            {
                throw new Exception("Job not found!");
            }
        }

        public async Task<bool> JobRunning(JobType jobType)
        {
            if (scheduler != null)
            {
                switch (jobType)
                {
                    case JobType.BackgroundRefresh:
                        return await Task.Run(() => scheduler.CheckExists(JobKey.Create(BackgroundJob))).ConfigureAwait(false);

                    case JobType.CollectionRefresh:
                        return await Task.Run(() => scheduler.CheckExists(JobKey.Create(CollectionsJob))).ConfigureAwait(false);

                    default:
                        return false;
                }
            }
            return false;
        }
    }
}