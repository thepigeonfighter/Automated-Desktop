using Quartz;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary
{
    public class ChangeBackgroundJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            BackGroundPicker picker = new BackGroundPicker();
            await Task.Run(() => picker.PickRandomBackground()).ConfigureAwait(false);
        }
    }
}