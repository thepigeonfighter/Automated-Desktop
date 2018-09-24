using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary
{
    public interface IInterestFileProcessor
    {
        EventHandler<List<InterestModel>> OnFileUpdate { get; set; }
        InterestModel CreateEntry(InterestModel entry);
        List<InterestModel> LoadAllEntries();
        List<InterestModel> UpdateEntries(List<InterestModel> newEntries);
        InterestModel UpdateInterest(InterestModel entry);
        void DeleteEntry(InterestModel entry);
        void OverwriteEntries(List<InterestModel> items);
    }
}
