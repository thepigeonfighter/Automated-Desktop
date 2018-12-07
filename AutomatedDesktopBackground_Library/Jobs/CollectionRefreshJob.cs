using Quartz;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary
{
    public class CollectionRefreshJob : IJob
    {
        
        public async Task Execute(IJobExecutionContext context)
        {
            //TODO Clean up these Dependencies 
            IDataKeeper dataKeeper = BuildDataKeeper();
            IAPIManager manager = BuildAPIManager(dataKeeper);
            CollectionRefresher collectionRefresher = new CollectionRefresher(manager,dataKeeper);
            await Task.Run(() => collectionRefresher.RefreshAllCollections()).ConfigureAwait(false);
        }
        private IDataKeeper BuildDataKeeper()
        {
            IDataStorageBuilder builder = new DataStorageBuilder();
            IDataStorage dataStorage = builder.Build(Database.JsonFile);
            IDataKeeper dataKeeper = new DataKeeper(dataStorage);
            return dataKeeper;
        }
        private IAPIManager BuildAPIManager(IDataKeeper dataKeeper)
        {
            ImageModelBuilder builder = new ImageModelBuilder();
            ImageGetter imageGetter = new ImageGetter(dataKeeper,builder);
            IAPIManager manager = new APIManager(imageGetter, dataKeeper);
            return manager;
        }
    }
}