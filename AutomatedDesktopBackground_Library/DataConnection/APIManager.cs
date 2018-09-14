using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using AutomatedDesktopBackgroundLibrary.ResponseClasses;
using System.Net.Http;
using System.Windows;
using System.IO;
namespace AutomatedDesktopBackgroundLibrary
{
    public class APIManager
    {
        string authorizationKey = "client_id=2bfdc60506a259dbe29b83cf75f5d5b463b4ca97749921e4b408a07f51427430";
        //string igkey = "https://api.instagram.com/v1/tags/nofilter/media/recent?access_token=33b1c9b0ddf54010a2463e0de12220ef"
        string baseUrl = "https://api.unsplash.com/";
        string mainQuery ="";
        ImageFileManager fileManager = new ImageFileManager();
        ImageGetter imageGetter = new ImageGetter();
        private string UrlBuilder(string query, int pageAmount)
        {
            
           return  $"{baseUrl}search/photos?page={pageAmount}&query={query}&orientation=landscape&{authorizationKey}";

        }
        private void SendRequest(string url, bool isUserRequest)
        {
            RootObject response = null;
            List<string> imageResults = new List<string>();
            using (StreamReader sr = new StreamReader(GetResponseStream(url)))
            {

                string responses = sr.ReadToEnd();
                response = JsonConvert.DeserializeObject<RootObject>(responses);

                
            }
            if(response != null)
            {
                //Sets the amount of results that were received
                imageGetter.ExpectedDownloadAmount = response.results.Count;
                if(response.results.Count == 0)
                {
                    GlobalConfig.EventSystem.InvokeResultNotFoundEvent();
                    GlobalConfig.EventSystem.InvokeDownloadCompleteEvent(false);
                }
                foreach(Result r in response.results)
                {
                    imageGetter.GetImage(r.urls.full,mainQuery, isUserRequest);               
                }
                
            }
            if(response == null)
            {
                throw new Exception("There was an error with the web server");
            }
           
                       
        }
        public  LiteImageResponseModel GetLiteImageResponse(string query)
        {
            string url = UrlBuilder(query, 0);
            RootObject response = null;
            List<string> imageResults = new List<string>();
            using (StreamReader sr = new StreamReader(GetResponseStream(url)))
            {

                string responses = sr.ReadToEnd();
                response = JsonConvert.DeserializeObject<RootObject>(responses);


            }
            LiteImageResponseModel imageResponseModel = new LiteImageResponseModel();
            imageResponseModel.TotalPages = response.total_pages;
            imageResponseModel.TotalResults = response.total;
            return imageResponseModel;

        }
        private Stream GetResponseStream(string url)
        {
            using (var client = new HttpClient())
            {
                var result = client.GetAsync(url).Result;
                result.EnsureSuccessStatusCode();
                return result.Content.ReadAsStreamAsync().Result;
            }
        }
        public void GetImagesBySearch(string query,bool userRequested)
        {

            mainQuery = query;
            bool isCollectionDownloaded = fileManager.IsCollectionDownloaded(query);
            if (!isCollectionDownloaded)
            {
                int pageNumber = fileManager.GetNewPageQuerry(query);
                string url = UrlBuilder(query, pageNumber);
                SendRequest(url, userRequested);
            }
            else
            {
                LocalImageGetter.GetLocalImages(query, userRequested);
            }
        }

    }
}
