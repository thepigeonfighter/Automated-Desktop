using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary.ResponseClasses
{

    public class RootObject :IRootObject
    {
        public int total { get; set; }
        public int total_pages { get; set; }
        public List<Result> results { get; set; }
    }
}
