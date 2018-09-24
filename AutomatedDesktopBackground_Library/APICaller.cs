using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomatedDesktopBackgroundLibrary.ResponseClasses;
using AutomatedDesktopBackgroundLibrary.StringExtensions;
using Newtonsoft.Json;

namespace AutomatedDesktopBackgroundLibrary.API
{
    public class APICaller : IAPICaller
    {
        private IFileCollection _fileCollection;

        public APICaller(IFileCollection fileCollection)
        {
            _fileCollection = fileCollection;
        }

        private RootObject GetUseableResults(string result)
        {
            RootObject response = new RootObject();
            response =  JsonConvert.DeserializeObject<RootObject>(result);
            return response;
        }

        public IRootObject GetWebResponse(string query)
        {
            string url = URLBuilder(query, CurrentAPIClient.Unsplash);
            IAPIResult result = new APIResult();
            string webResponse = result.GetResponse(url);
            RootObject rootObject = GetUseableResults(webResponse);
            return rootObject;
            
        }

        private string URLBuilder(string query, CurrentAPIClient client)
        {
            //If there are more clients you could test the second parameter
            //And create different urls based on whichever client you were currently using
            IURLBuilder uRLBuilder = new UnsplashUrlBuilder();
            int pageNumber = _fileCollection.GetNextPageInCollection(query);
            string url = uRLBuilder.BuildUrl(query, pageNumber);
            return url;
            
        }

    }
}
