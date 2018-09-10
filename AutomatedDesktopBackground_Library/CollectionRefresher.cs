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
        ImageFileManager imageFileManager = new ImageFileManager();
        public void RefreshAllCollections()
        {
            allInterests = TextConnectorProcessor.LoadFromTextFile<InterestModel>(GlobalConfig.InterestFile);
            if(allInterests.Count>0)
            {
                
                foreach(InterestModel i in allInterests)
                {
                    List<ImageModel> imagesAssociated = imageFileManager.GetAllImagesByInterestId(i.Id);
                    imagesAssociated.ForEach(x => x.IsDownloaded =false);
                    TextConnectorProcessor.SaveToTextFile(imagesAssociated, GlobalConfig.ImageFile);
                    imagesAssociated.ForEach(x => File.Delete(x.FileDir));
                    apiManager.GetImagesBySearch(i.Name, false);
                    
                }
            }
        }
    }
}
