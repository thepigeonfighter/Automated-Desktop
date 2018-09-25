using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomatedDesktopBackgroundLibrary.StringExtensions;
namespace AutomatedDesktopBackgroundLibrary
{
    public class InterestFileProcessor : IInterestFileProcessor
    {
        private IDatabaseConnector _database;
        private const string InterestFile ="Interests.csv";

        public EventHandler<List<InterestModel>> OnFileUpdate { get ; set; }

        public InterestFileProcessor(IDatabaseConnector database)
        {
            _database = database;
            OnFileUpdate?.Invoke(this, LoadAllEntries());
        }
        public InterestModel CreateEntry(InterestModel entry)
        {
            _database.CreateEntry(entry, InterestFile.FullFilePath());
            OnFileUpdate?.Invoke(this, LoadAllEntries());
            return entry;
        }

        public void DeleteEntry(InterestModel entry)
        {
            _database.Delete(entry, InterestFile.FullFilePath());
            OnFileUpdate?.Invoke(this, LoadAllEntries());
        }

        public List<InterestModel> LoadAllEntries()
        {
           return  _database.Load<InterestModel>(InterestFile.FullFilePath());
        }

        public void OverwriteEntries(List<InterestModel> items)
        {
            _database.SaveToFile(items, InterestFile.FullFilePath());
            OnFileUpdate?.Invoke(this, LoadAllEntries());
        }

        public List<InterestModel> UpdateEntries(List<InterestModel> newEntries)
        {
           List<InterestModel> output =  _database.Update(newEntries, InterestFile.FullFilePath());
            OnFileUpdate?.Invoke(this, LoadAllEntries());
            return output;
        }

        public InterestModel UpdateInterest(InterestModel entry)
        {
             _database.Update(entry, InterestFile.FullFilePath());
            OnFileUpdate?.Invoke(this, LoadAllEntries());
            return entry;
        }
    }
}
