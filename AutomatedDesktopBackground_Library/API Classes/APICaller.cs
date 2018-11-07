using AutomatedDesktopBackgroundLibrary.ImageFileProcessing;
using AutomatedDesktopBackgroundLibrary.ResponseClasses;
using log4net;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace AutomatedDesktopBackgroundLibrary.API
{
    public class APICaller : IAPICaller
    {
        private readonly IFileCollection _fileCollection;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public APICaller(IFileCollection fileCollection)
        {
            _fileCollection = fileCollection;
        }

        private RootObject GetUseableResults(string result)
        {
            
            RootObject response = JsonConvert.DeserializeObject<RootObject>(result);
            log.Debug($"Found {response.results.Count} useable results");
            return response;
        }

        public IRootObject GetWebResponse(string url)
        {
            try
            {
                log.Debug($"Trying to find useable results for {url} ");
                string customizedUrl = URLBuilder(url);
                IAPIResult result = new APIResult();
                string webResponse = result.GetResponse(customizedUrl);
                RootObject rootObject = GetUseableResults(webResponse);
                return rootObject;
            }
            catch(Exception ex)
            {
                log.Error("Failed to get a usable web response");
                log.Info(ex.InnerException.Message);
                return null;
            }
        }

        private string URLBuilder(string query)
        {
            log.Debug($"building a url from the query {query}");
            IURLBuilder uRLBuilder = new UnsplashUrlBuilder();
            int pageNumber = GetNextPage(query);
            string url = uRLBuilder.BuildUrl(query, pageNumber);
            log.Debug($"{query} has been converted to {url}");
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