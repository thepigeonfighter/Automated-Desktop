using Quartz;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary
{
    public class CollectionRefreshJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            CollectionRefresher collectionRefresher = new CollectionRefresher();
            await Task.Run(() => collectionRefresher.RefreshAllCollections()).ConfigureAwait(false);
        }
    }
}