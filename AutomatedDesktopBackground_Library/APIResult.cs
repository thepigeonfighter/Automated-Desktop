using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary
{
    public class APIResult : IAPIResult
    {
        private bool _responseSuccess = false;
        private string _response;
        public string GetResponse(string url)
        {
            SendRequest(url);
            return _response;
        }

        public bool IsSucessfulQuery()
        {
            return _responseSuccess;
        }
        private void SendRequest(string url)
        {
            using (StreamReader sr = new StreamReader(GetResponseStream(url)))
            {

                _response= sr.ReadToEnd();
                _responseSuccess = true;

            }


        }

        private Stream GetResponseStream(string url)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var result = client.GetAsync(url).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return result.Content.ReadAsStreamAsync().Result;
                    }
                    else
                    {
                        ConnectionError();
                        return null;
                    }
                }
                catch
                {
                    ConnectionError();
                    return null;
                }
            }
        }
        private void ConnectionError()
        {
            _responseSuccess = false;
            throw new Exception("There was an error with the web server, sent from the APIResult Class");
        }
    }
}
