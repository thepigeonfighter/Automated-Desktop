using AutomatedDesktopBackgroundLibrary.Utility;
using System;
using System.Collections.Generic;

namespace AutomatedDesktopBackgroundLibrary
{
    public class InterestFileProcessor : IInterestFileProcessor
    {
        private readonly IDatabaseConnector _database;
        private readonly string _interestFile = InternalFileDirectorySystem.InterestInfoFolder;

        public EventHandler<List<InterestModel>> OnFileUpdate { get; set; }

        public InterestFileProcessor(IDatabaseConnector database)
        {
            _database = database;
        }

        public InterestModel CreateEntry(InterestModel entry)
        {
            entry.InfoFileDir = _interestFile + $@"\{entry.Name}.intInfo";
            _database.CreateEntry(entry, entry.InfoFileDir);
            OnFileUpdate?.Invoke(this, LoadAllEntries());
            return entry;
        }

        public void DeleteEntry(InterestModel entry)
        {
            if (entry != null)
            {
                _database.Delete(entry, entry.InfoFileDir,OnDeletionCompleted);                
            }
        }
        public void OnDeletionCompleted()
        {
            OnFileUpdate?.Invoke(this, LoadAllEntries());
        }

        public List<InterestModel> LoadAllEntries()
        {
            return _database.Load<InterestModel>(FileType.InterestInfo);
        }
    }
}