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
        public async Task RefreshAllCollections()
        {
            allInterests = TextConnectorProcessor.LoadFromTextFile<InterestModel>(GlobalConfig.InterestFile);
            if(allInterests.Count>0)
            {
                
                foreach(InterestModel i in allInterests)
                {
                    List<ImageModel> imagesAssociated = imageFileManager.GetAllImagesByInterestId(i.Id);
                    imagesAssociated.ForEach(x => x.IsDownloaded =false);
                    TextConnectorProcessor.SaveToTextFile(imagesAssociated, GlobalConfig.ImageFile);

                    foreach(ImageModel img in imagesAssociated)
                    {
                        bool fileReady = false;
                        int tries = 0;
                        int timeout = 100;
                        while(!fileReady && tries< timeout)
                        {
                            tries++;
                            fileReady = FileReady(img.FileDir);
                            if(fileReady)
                            {
                                File.Delete(img.FileDir);
                                continue;
                            }
                            await Task.Delay(1000);
                        }
                        if(tries>= timeout) { throw new Exception("Image Deletion timed out in the collection refresher job."); }
                    }
                    apiManager.GetImagesBySearch(i.Name, false);
                    
                }
            }
        }
        private bool FileReady(string fileDir)
        {
            try
            {
                using (FileStream inputStream = File.Open(fileDir, FileMode.Open, FileAccess.Read, FileShare.None))
                    return inputStream.Length > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
