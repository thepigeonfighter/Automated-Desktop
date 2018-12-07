using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutomatedDesktopBackgroundLibrary;
namespace ChangeBackgroundOnce
{
    class Program
    {
        static void Main(string[] args)
        {

            IDataKeeper dataKeeper = GetDataKeeper();
            List<ImageModel> images = dataKeeper.GetFreshFileSnapShot().AllImages;
           if (images.Count > 0)
            {
               BackGroundPicker backGroundPicker = new BackGroundPicker(dataKeeper);
                backGroundPicker.PickRandomBackground(false);
                
           }
        }
        private static IDataKeeper GetDataKeeper()
        {
            IDataStorageBuilder builder = new DataStorageBuilder();
            IDataStorage jsonStorage = builder.Build(Database.JsonFile);
            IDataKeeper dataKeeper = new DataKeeper(jsonStorage);
            return dataKeeper;
        }

    }
}
