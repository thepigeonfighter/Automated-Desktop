using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary
{
    public class InterestBuilder
    {
        public async Task<InterestModel> Build(string interest, IFileCollection _fileCollection, IAPIManager manager)
        {
            List<InterestModel> existinginterests = _fileCollection.AllInterests;

            int newId = 1;
            if (existinginterests.Count > 0)
            {
                newId = existinginterests.Max(x => x.Id) + 1;
            }
            InterestModel newInterest = new InterestModel();
            IRootObject response = await manager.GetResults(interest).ConfigureAwait(false);
            newInterest.Name = interest;
            newInterest.TotalImages = response.total;
            newInterest.TotalPages = response.total_pages;
            newInterest.Id = newId;
            return newInterest;
        }
    }
}