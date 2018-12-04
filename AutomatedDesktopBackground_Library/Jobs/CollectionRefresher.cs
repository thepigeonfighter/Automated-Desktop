using System.Collections.Generic;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary
{
    public class CollectionRefresher
    {
        private List<InterestModel> _allInterests;
        private readonly IAPIManager _apiManager;
        private IDataKeeper _dataKeeper;

        public CollectionRefresher( IAPIManager apiManager, IDataKeeper dataKeeper)
        {
            
            _apiManager = apiManager;
            _dataKeeper = dataKeeper;
            _allInterests = dataKeeper.GetFreshFileSnapShot().AllInterests;
        }

        public async Task RefreshAllCollections()
        {
            if (_allInterests.Count > 0)
            {
                foreach (InterestModel i in _allInterests)
                {
                    await Task.Run(() => _apiManager.GetImagesBySearch(i.Name, false)).ConfigureAwait(false);
                }
            }
        }
    }
}