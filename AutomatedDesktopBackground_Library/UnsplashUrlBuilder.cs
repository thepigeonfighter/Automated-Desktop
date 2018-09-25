using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary
{
    public class UnsplashUrlBuilder : IURLBuilder
    {
        string authorizationKey = "client_id=2bfdc60506a259dbe29b83cf75f5d5b463b4ca97749921e4b408a07f51427430";
        string baseUrl = "https://api.unsplash.com/";
        public string BuildUrl(string query, int pageNumber)
        {
            return $"{baseUrl}search/photos?page={pageNumber}&query={query}&orientation=landscape&{authorizationKey}";
        }
    }
}
