using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary
{
    public interface IImageFileProcessor
    {
        EventHandler<List<ImageModel>> OnFileAltered { get; set; }
        ImageModel CreateEntry(ImageModel entry);
        List<ImageModel> LoadAllEntries();
        List<ImageModel> UpdateEntries(List<ImageModel> newEntries);
        ImageModel UpdateEntries(ImageModel entry);
        void DeleteEntry(ImageModel entry);
        void OverwriteEntries(List<ImageModel> items);
        
    }
}
