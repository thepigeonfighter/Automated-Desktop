using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary
{

    public class DataStorageBuilder:IDataStorageBuilder
    {
        public IDataStorage Build(Database currentDatabase)
        {
           
            switch(currentDatabase)
            {
                case Database.Textfile:
                    return BuildDefault();
               
                default:
                    return BuildDefault();
                    

            }
            
        }
        private  DataStorage BuildDefault()
        {
            IDatabaseConnector _database = new TextFileConnector();
            DataStorage dataStorage = new DataStorage()
            {
                Database = _database,
                FavoritedImageFileProcessor = new FavoriteImageProcessor(_database),
                FileCollection = new FileCollection(),
                HatedImageFileProcessor = new HatedImageProcessor(_database),
                ImageFileProcessor = new ImageFileProcessor(_database),
                InterestFileProcessor = new InterestFileProcessor(_database),
                WallPaperFileProcessor = new WallpaperFileProcessor(_database)

                
            };
            dataStorage.WireUpEvents();
            return dataStorage;

        }
    }
}
