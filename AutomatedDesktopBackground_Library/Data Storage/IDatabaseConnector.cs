using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary
{
    public interface IDatabaseConnector
    {
        List<T> Load<T>(FileType fileType) where T : class, ISaveable, new();
        T LoadEntry<T>(string filePath) where T : class, ISaveable, new();
        void CreateEntry<T>(T item, string filePath) where T: class, ISaveable, new();
        void Delete<T>(T item, string filePath, Action callBack) where T : class, ISaveable, new();
        void DeleteFile(string filePath, Action callBack = null);
        void DeleteImages(List<ImageModel> images);
        void DeleteAllFiles();
    }
}
