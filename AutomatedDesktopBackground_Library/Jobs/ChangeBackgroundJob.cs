using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary
{
    public class ChangeBackgroundJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            
            BackGroundPicker picker = new BackGroundPicker();
            await Task.Run(()=>picker.PickRandomBackground()).ConfigureAwait(false);

        }

    }
}
