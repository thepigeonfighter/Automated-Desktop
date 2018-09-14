using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary
{
    public  class InterestHelper
    {

        public static async Task<List< InterestModel>> CreateInterest(string interest )
        {
            List<InterestModel> interests = TextConnectorProcessor.LoadFromTextFile<InterestModel>(GlobalConfig.InterestFile);
            int id = 1;
            if (interests.Count > 0)
            {
                id = interests.Max(x => x.Id) + 1;
            }
            InterestModel newInterest = new InterestModel() { Name = interest, Id = id };
            APIManager aPIManager = new APIManager();
            LiteImageResponseModel response = new LiteImageResponseModel();
            await Task.Run(()=> response = aPIManager.GetLiteImageResponse(interest));
            newInterest.TotalImages = response.TotalResults;
            newInterest.TotalPages = response.TotalPages;
            interests.Add(newInterest);

            TextConnectorProcessor.SaveToTextFile(interests.ToList(), GlobalConfig.InterestFile);
            return interests;
        }
    }
}
