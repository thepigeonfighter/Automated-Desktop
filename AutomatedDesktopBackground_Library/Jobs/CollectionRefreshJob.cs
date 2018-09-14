using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary
{
    public class CollectionRefreshJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            CollectionRefresher collectionRefresher = new CollectionRefresher();
            await Task.Run(() => collectionRefresher.RefreshAllCollections());
        }
    }
}
