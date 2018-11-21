using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundUI.Models
{
    public class InterestModel
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        private int _results;

        public int Results
        {
            get { return _results; }
            set { _results = value; }
        }


    }
}
