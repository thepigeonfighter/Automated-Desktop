using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary
{
    public class FilteredFileResult : IFilteredFileResult
    {
        private List<ISaveable> _results;
        public bool SucessfulQuery { get; set; }
        public  void SetResults(List<ISaveable> items)
        {
            _results = items;
        }
        public List<ISaveable> GetResults()
        {
            return _results;
        }
        public ISaveable SelectedResult { get; set; }
    }
}
