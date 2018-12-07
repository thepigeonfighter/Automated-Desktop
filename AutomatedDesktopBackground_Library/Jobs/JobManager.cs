using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary
{
    public class JobManager
    {
        private static IScheduler scheduler;
        private const string BackgroundJob = "bgjob";
        private const string CollectionsJob = "cj";

        public JobManager()
        {
            Task.Run(()=>BuildScheduler()).ConfigureAwait(false);
        }
        private async Task BuildScheduler()
        {
            NameValueCollection prop = new NameValueCollection
                {
                    {"quartz.serializer.type", "binary" }
                };
            StdSchedulerFactory factory = new StdSchedulerFactory(prop);
            scheduler = await  factory.GetScheduler();
        }
        public async Task StartBackgroundUpdatingAsync(TimeSpan time)
        {
            try
            {
                await scheduler.Start().ConfigureAwait(false);
                IJobDetail jobDetail = JobBuilder.Create<ChangeBackgroundJob>().WithIdentity(BackgroundJob).Build();
                int refreshRate = (int)Math.Round(time.TotalSeconds);
                ITrigger trigger = TriggerBuilder.Create().WithIdentity(BackgroundJob).StartAt(DateTime.Now.AddSeconds(refreshRate)).WithSimpleSchedule(x =>
                 x.WithIntervalInSeconds(refreshRate).RepeatForever()).Build();
                await scheduler.ScheduleJob(jobDetail, trigger);
            }
            catch (SchedulerException se)
            {
                throw new Exception(se.ToString());
            }
        }

        public async Task StartCollectionUpdatingAsync(TimeSpan time)
        {
            try
            {
                await scheduler.Start().ConfigureAwait(false);
                IJobDetail jobDetail = JobBuilder.Create<CollectionRefreshJob>().WithIdentity(CollectionsJob).Build();
                int refreshRate = (int)Math.Round(time.TotalSeconds);
                 ITrigger trigger = TriggerBuilder.Create().WithIdentity(CollectionsJob).StartNow().WithSimpleSchedule(x =>
                 x.WithIntervalInSeconds(refreshRate).RepeatForever()).Build();
                await scheduler.ScheduleJob(jobDetail, trigger);
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
                await scheduler.Clear();
               
            }
        }

        public async Task StopBackgroundUpdatingAsync()
        {
            JobKey key = JobKey.Create(BackgroundJob);
            if (scheduler != null)
            {
                if (await scheduler.CheckExists(key))
                {
                    await  scheduler.DeleteJob(key);
                }
            }
        }

        public async Task StopCollectionUpdatingAsync()
        {
            JobKey key = JobKey.Create(CollectionsJob);
            if (scheduler != null)
            {
                if (await scheduler.CheckExists(key))
                {
                    await scheduler.DeleteJob(key);
                }
            }

        }

        public async Task<bool> JobRunning(JobType jobType)
        {
            if (scheduler != null)
            {
                switch (jobType)
                {
                    case JobType.BackgroundRefresh:
                        return await  scheduler.CheckExists(JobKey.Create(BackgroundJob));

                    case JobType.CollectionRefresh:
                        return await scheduler.CheckExists(JobKey.Create(CollectionsJob));

                    default:
                        return false;
                }
            }
            return false;
        }
    }
}