using System;
using System.Collections.Generic;

namespace AutomatedDesktopBackgroundLibrary
{
    public interface IInterestFileProcessor
    {
        EventHandler<List<InterestModel>> OnFileUpdate { get; set; }

        InterestModel CreateEntry(InterestModel entry);

        List<InterestModel> LoadAllEntries();

        void DeleteEntry(InterestModel entry);
    }
}