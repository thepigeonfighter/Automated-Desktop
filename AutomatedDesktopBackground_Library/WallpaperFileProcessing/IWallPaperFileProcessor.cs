using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary
{
    public interface IWallPaperFileProcessor
    {
        EventHandler<ImageModel> OnWallPaperUpdate { get; set; }
        ImageModel Load();
        ImageModel Update(ImageModel entry);

    }
}
