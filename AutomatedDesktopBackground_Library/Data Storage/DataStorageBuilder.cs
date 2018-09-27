using System;

namespace AutomatedDesktopBackgroundLibrary
{
    public class DataStorageBuilder : IDataStorageBuilder
    {
        public IDataStorage Build(Database currentDatabase)
        {
            switch (currentDatabase)
            {
                case Database.Textfile:
                    throw new Exception("This save method has not been implemented");
                case Database.JsonFile:
                    return BuildJson();

                default:
                    return BuildJson();
            }
        }

        private DataStorage BuildJson()
        {
            IDatabaseConnector _database = new JsonDataConnector();
            DataStorage dataStorage = new DataStorage()
            {
                Database = _database,
                FileCollection = new FileCollection(),
                ImageFileProcessor = new ImageFileProcessor(_database),
                InterestFileProcessor = new InterestFileProcessor(_database),
            };
            dataStorage.WireUpEvents();
            return dataStorage;
        }
    }
}