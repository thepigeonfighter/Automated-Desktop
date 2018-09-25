using System.Collections.Generic;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary
{
    public class CollectionRefresher
    {
        private List<InterestModel> allInterests = new List<InterestModel>();
        private readonly APIManager apiManager = new APIManager();

        public async Task RefreshAllCollections()
        {
            allInterests = DataKeeper.GetFileSnapShot().AllInterests;
            if (allInterests.Count > 0)
            {
                foreach (InterestModel i in allInterests)
                {
                    await Task.Run(() => apiManager.GetImagesBySearch(i.Name, false)).ConfigureAwait(false);
                }
            }
        }
    }
}