using AutomatedDesktopBackgroundLibrary.ImageFileProcessing;
using AutomatedDesktopBackgroundLibrary.ResponseClasses;
using Newtonsoft.Json;
using System.Linq;

namespace AutomatedDesktopBackgroundLibrary.API
{
    public class APICaller : IAPICaller
    {
        private readonly IFileCollection _fileCollection;

        public APICaller(IFileCollection fileCollection)
        {
            _fileCollection = fileCollection;
        }

        private RootObject GetUseableResults(string result)
        {
            RootObject response = new RootObject();
            return JsonConvert.DeserializeObject<RootObject>(result);
        }

        public IRootObject GetWebResponse(string url)
        {
            string customizedUrl = URLBuilder(url);
            IAPIResult result = new APIResult();
            string webResponse = result.GetResponse(customizedUrl);
            RootObject rootObject = GetUseableResults(webResponse);
            return rootObject;
        }

        private string URLBuilder(string query)
        {
            IURLBuilder uRLBuilder = new UnsplashUrlBuilder();
            int pageNumber = GetNextPage(query);
            string url = uRLBuilder.BuildUrl(query, pageNumber);
            return url;
        }

        private int GetNextPage(string query)
        {
            InterestModel interest = _fileCollection.AllInterests.FirstOrDefault(x => x.Name == query);
            int newPageNumber = 1;
            //The interest may actually be null because when it is first created the first search will be done to
            // pull data about how many available images there are with it
            if (interest != null)
            {
                int maxImages = interest.TotalImages;
                int currentlyDownloadedImages = _fileCollection.AllImages.AllImagesByInterest(interest).Count;
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
    }
}