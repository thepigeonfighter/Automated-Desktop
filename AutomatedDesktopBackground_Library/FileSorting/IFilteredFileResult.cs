using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary
{
    public interface IFilteredFileResult
    {
        List<ISaveable> GetResults();
        void SetResults(List<ISaveable> items);
        bool SucessfulQuery { get; set; }
        ISaveable SelectedResult { get; set; }

    }
}
