using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary
{
    public class CollectionRefresher
    {
        List<InterestModel> allInterests = new List<InterestModel>();
        APIManager apiManager = new APIManager();
        public async Task RefreshAllCollections()
        {
            allInterests = DataKeeper.GetFileSnapShot().AllInterests;
            if(allInterests.Count>0)
            {
                
                foreach(InterestModel i in allInterests)
                {

                   await Task.Run(()=> apiManager.GetImagesBySearch(i.Name, false));
                    
                }
            }
        }
    }
}
