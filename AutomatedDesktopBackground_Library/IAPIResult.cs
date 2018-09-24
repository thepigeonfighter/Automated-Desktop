using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary
{
    public interface IAPIResult
    {
        bool IsSucessfulQuery();
        string GetResponse(string url);
    }
}
