using AutomatedDesktopBackgroundLibrary.Jobs;
using AutomatedDesktopBackgroundLibrary.Utility;
using Quartz;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary
{
    public class ChangeBackgroundJob : IJob
    {
        //TODO keep thinking about way to be able to use DI to get rid of the dependencies
        public Task Execute(IJobExecutionContext context)
        {
            IDataKeeper dataKeeper = BuildDataKeeper();
            BackGroundPicker picker = new BackGroundPicker(dataKeeper);
            picker.PickRandomBackground(true);
            return new Task(() => new JobResult() { Subject = "Background Changed", Sucess = true });
        }
        private IDataKeeper BuildDataKeeper()
        {
            IDataStorageBuilder builder = new DataStorageBuilder();
            IDataStorage dataStorage = builder.Build(Database.JsonFile);
            IDataKeeper dataKeeper = new DataKeeper(dataStorage);
            return dataKeeper;
        }



    }
}