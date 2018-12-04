using AutomatedDesktopBackgroundLibrary.ImageFileProcessing;
using AutomatedDesktopBackgroundLibrary.ResponseClasses;
using System.Linq;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary
{
    public class APIManager : IAPIManager
    {
        private readonly ImageGetter _imageGetter;
        private IDataKeeper _dataKeeper;

        public APIManager(ImageGetter imageGetter, IDataKeeper dataKeeper)
        {
            _imageGetter = imageGetter;
            this._dataKeeper = dataKeeper;
        }

        public async Task<IRootObject> GetResults(string interestName)
        {
            IAPICaller apiCaller = new API.APICaller(_dataKeeper.GetFileSnapShot());
            IRootObject rootObject = await Task.Run(() => apiCaller.GetWebResponse(interestName)).ConfigureAwait(false);
            if (rootObject.total == 0) { HandleNoResults(); }
            return rootObject;
        }

        public async Task GetImagesBySearch(string query, bool userRequested)
        {
            GlobalConfig.InCollectionRefresh = true;


            try
            {
                InterestModel interestModel = _dataKeeper.GetFileSnapShot().AllInterests.FirstOrDefault(x => x.Name == query);
                if (interestModel.EntireCollectionDownloaded)
                {
                    LocalDownload(query, userRequested);
                }
                else
                {
                    if (GetNextPage(interestModel) > interestModel.TotalPages)
                    {
                        LocalDownload(query, userRequested);
                    }
                    await FreshDownload(query, userRequested).ConfigureAwait(false);
                }
            }
            catch
            {
                throw new System.Exception("No matching interest found file system out of sync...");
            }
        }
        private void HandleNoResults()
        {
            GlobalConfig.EventSystem.InvokeResultNotFoundEvent();
            GlobalConfig.EventSystem.InvokeDownloadCompleteEvent(false);
        }

        private int GetNextPage(InterestModel interest)
        {
            int newPageNumber = 1;
            //The interest may actually be null because when it is first created the first search will be done to
            // pull data about how many available images there are with it
            if (interest != null)
            {
                int maxImages = interest.TotalImages;
                int currentlyDownloadedImages = _dataKeeper.GetFileSnapShot().AllImages.AllImagesByInterest(interest).Count;
                newPageNumber = (currentlyDownloadedImages / 10) + 1;
                if (currentlyDownloadedImages % 10 != 0)
                {
                    newPageNumber++;
                }
                if (maxImages == currentlyDownloadedImages)
                {
                    newPageNumber++;
                }
            }

            return newPageNumber;
        }

        private async Task FreshDownload(string query, bool userRequested)
        {
            IRootObject iRootObject = await GetResults(query).ConfigureAwait(false);
            RootObject rootObject = (RootObject)iRootObject;
            _imageGetter.ExpectedDownloadAmount = rootObject.results.Count;
            foreach (Result r in rootObject.results)
            {
                string name = query;
                 if(r.description !=null)
                {
                    name = r.description;
                }
                _imageGetter.GetImage(r.urls.full, query,name, userRequested);
            }
        }

        private void LocalDownload(string query, bool userRequested)
        {
            LocalImageGetter localImageGetter = new LocalImageGetter(_dataKeeper,_imageGetter);
            localImageGetter.GetLocalImages(query, userRequested);
        }

    }
}