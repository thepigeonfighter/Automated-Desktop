using AutomatedDesktopBackgroundLibrary.DataConnection;
using AutomatedDesktopBackgroundLibrary.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace AutomatedDesktopBackgroundLibrary
{
    public class JsonDataConnector : DirectoryNavigator, IDatabaseConnector
    {
        private static readonly FileRequestsManager fileRequestsManager = new FileRequestsManager();
        private static readonly ReaderWriterLockSlim _sync = new ReaderWriterLockSlim();
        private static readonly Queue<FileRequest> _deletionQueue = new Queue<FileRequest>();

        public JsonDataConnector()
        {
            fileRequestsManager.OnDeletionCompleted += OnDeletionCompleted;
        }

        private void OnDeletionCompleted(object sender, EventArgs e)
        {
            if (_deletionQueue.Count > 0)
            {
                FileRequest request = _deletionQueue.Dequeue();
                fileRequestsManager.RegisterRequest(request);
            }
        }

        public void DeleteAllFiles()
        {
            fileRequestsManager.DeleteAllFiles();
        }

        public void DeleteFile(string filePath)
        {
            using (_sync.Write())
            {
                FileRequest request = new FileRequest()
                {
                    FileOperation = FileOperation.Delete,
                    FilePath = filePath,
                };
                if (_deletionQueue.Count > 0)
                {
                    _deletionQueue.Enqueue(request);
                }
                else
                {
                    fileRequestsManager.RegisterRequest(request);
                }
            }
        }

        public void DeleteImages(List<ImageModel> images)
        {
            using (_sync.Write())
            {
                foreach (ImageModel i in images)
                {
                    FileRequest request = new FileRequest()
                    {
                        FileOperation = FileOperation.Delete,
                        FilePath = i.LocalUrl,
                    };
                    if (_deletionQueue.Count > 0)
                    {
                        _deletionQueue.Enqueue(request);
                    }
                    else
                    {
                        fileRequestsManager.RegisterRequest(request);
                    }
                }
            }
        }

        void IDatabaseConnector.CreateEntry<T>(T item, string filePath)
        {
            using (_sync.Write())
            {
                string jsonText = JsonConvert.SerializeObject(item, Formatting.Indented);

                FileRequest request = new FileRequest()
                {
                    FileOperation = FileOperation.Write,
                    FilePath = filePath,
                    Lines = new List<string>() { jsonText },
                };
                fileRequestsManager.RegisterRequest(request);
            }
        }

        void IDatabaseConnector.Delete<T>(T item, string filePath)
        {
            DeleteFile(filePath);
        }

        List<T> IDatabaseConnector.Load<T>(FileType fileType)
        {
            List<T> items = new List<T>();
            string[] filesInFolder = new string[1];
            switch (fileType)
            {
                case FileType.ImageInfo:
                    filesInFolder = GetAllImageInfoFilesInDirectory(fileType);
                    break;

                case FileType.InterestInfo:
                    filesInFolder = GetAllInterestInfoFiles(fileType);
                    break;

                case FileType.ImageFile:
                    filesInFolder = GetAllImageFilesInDirectory(fileType);
                    break;

                default:
                    break;
            }
            try
            {
                using (_sync.Write())
                {
                    foreach (string s in filesInFolder)
                    {
                        string json = File.ReadAllText(s);
                        T item = JsonConvert.DeserializeObject<T>(json);
                        items.Add(item);
                    }
                }
            }
            catch
            {
                Debug.WriteLine($"There was no items found in association with {fileType} ");
            }

            return items;
        }

        T IDatabaseConnector.LoadEntry<T>(string filePath)
        {
            T entry = new T();
            using (_sync.Write())
            {
                entry = JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));
            }
            return entry;
        }
    }
}