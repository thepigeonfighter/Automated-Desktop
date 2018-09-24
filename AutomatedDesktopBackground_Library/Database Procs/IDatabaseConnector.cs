using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary
{
    public interface IDatabaseConnector
    {
        List<T> Load<T>(string filePath) where T : class, ISaveable, new();
        void CreateEntry<T>(T item, string filePath) where T: class, ISaveable, new();
        List<T> Update<T>(T items, string filePath) where T : class, ISaveable, new();
        List<T> Update<T>(List<T> items, string filePath) where T : class, ISaveable, new();
        void Delete<T>(T item, string filePath) where T : class, ISaveable, new();
        void SaveToFile<T>(List<T> items, string filePath) where T : class, ISaveable, new();
        void DeleteFile(string filePath);
        void CopyImage(ImageModel image, string copyPath);
        void DeleteAllFiles();
    }
}
