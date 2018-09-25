using AutomatedDesktopBackgroundLibrary.ResponseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary
{
    public interface IAPICaller
    {
        IRootObject GetWebResponse(string url);
    }
}
