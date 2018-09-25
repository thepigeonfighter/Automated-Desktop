using AutomatedDesktopBackgroundLibrary.ResponseClasses;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary
{
    public class APIManager : IFileListener
    {
        private readonly ImageGetter imageGetter = new ImageGetter();
        private IFileCollection _fileCollection = new FileCollection();

        public async Task<IRootObject> GetResults(string interestName)
        {
            IAPICaller apiCaller = new API.APICaller(_fileCollection);
            IRootObject rootObject = await Task.Run(() => apiCaller.GetWebResponse(interestName)).ConfigureAwait(false);
            if (rootObject.total == 0) { HandleNoResults(); }
            DataKeeper.RegisterFileListener(this);
            return rootObject;
        }

        private void HandleNoResults()
        {
            GlobalConfig.EventSystem.InvokeResultNotFoundEvent();
            GlobalConfig.EventSystem.InvokeDownloadCompleteEvent(false);
        }

        public async Task GetImagesBySearch(string query, bool userRequested)
        {
            GlobalConfig.InCollectionRefresh = true;
            if (_fileCollection.IsEntireCollectionDownloaded(query))
            {
                LocalDownload(query, userRequested);
            }
            else
            {
                await FreshDownload(query, userRequested).ConfigureAwait(false);
            }
        }

        private async Task FreshDownload(string query, bool userRequested)
        {
            IRootObject iRootObject = await GetResults(query).ConfigureAwait(false);
            RootObject rootObject = (RootObject)iRootObject;
            imageGetter.ExpectedDownloadAmount = rootObject.results.Count;
            foreach (Result r in rootObject.results)
            {
                imageGetter.GetImage(r.urls.full, query, userRequested);
            }
        }

        private void LocalDownload(string query, bool userRequested)
        {
            LocalImageGetter localImageGetter = new LocalImageGetter(_fileCollection);
            localImageGetter.GetLocalImages(query, userRequested);
        }

        public void OnFileUpdate()
        {
            _fileCollection = DataKeeper.GetFileSnapShot();
        }
    }
}