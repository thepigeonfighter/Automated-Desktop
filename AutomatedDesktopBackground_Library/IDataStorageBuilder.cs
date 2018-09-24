using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary
{
    public interface IDataStorageBuilder
    {
        IDataStorage Build(Database currentDatabase);
    }
}
