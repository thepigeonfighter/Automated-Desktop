using AutomatedDesktopBackgroundLibrary.DataConnection;
using AutomatedDesktopBackgroundLibrary.Utility;
using log4net;
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
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
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
                try
                {
                    FileRequest request = _deletionQueue.Dequeue();
                    fileRequestsManager.RegisterRequest(request);
                    log.Debug("Deletion has completed. Pulling the next object to delete");
                }
                catch(Exception ex)
                {
                    log.Error("Have failed to pull a filerequest out of deletion queue");
                    log.Info(ex.InnerException.Message);
                }
            }
        }

        public void DeleteAllFiles()
        {
            try
            {
                fileRequestsManager.DeleteAllFiles();
                log.Debug("Deleted all files");
            }
            catch(Exception ex)
            {
                log.Error("Failed to delete all files. Probabaly because this log is being acessed");
                log.Info(ex.InnerException.Message);
            }
        }

        public void DeleteFile(string filePath)
        {
            using (_sync.Write())
            {
                try
                {
                    FileRequest request = new FileRequest()
                    {
                        FileOperation = FileOperation.Delete,
                        FilePath = filePath,
                    };
                    if (_deletionQueue.Count > 0)
                    {
                        _deletionQueue.Enqueue(request);
                        log.Debug($"Enqueued a file  to be deleted at this file path {filePath}");
                    }
                    else
                    {
                        fileRequestsManager.RegisterRequest(request);
                        log.Debug($"Deleted a file at this file path {filePath}");
                    }
                    
                }
                catch(Exception ex)
                {
                    log.Error($"Failed to delete file at this file path {filePath}");
                    log.Info(ex.InnerException.Message);
                }
            }
        }

        public void DeleteImages(List<ImageModel> images)
        {
            using (_sync.Write())
            {
                foreach (ImageModel i in images)
                {
                    try
                    {
                        FileRequest request = new FileRequest()
                        {
                            FileOperation = FileOperation.Delete,
                            FilePath = i.LocalUrl,
                        };
                        if (_deletionQueue.Count > 0)
                        {
                            _deletionQueue.Enqueue(request);
                            log.Debug($"Enqueued a file  to be deleted at this file path {i.LocalUrl}");
                        }
                        else
                        {
                            fileRequestsManager.RegisterRequest(request);
                            log.Debug($"Deleted  a file at this file path {i.LocalUrl}");
                        }
                    }
                    catch(Exception ex)
                    {
                        log.Error($"Failed to delete file at this file path {i.LocalUrl}");
                        log.Info(ex.InnerException.Message);
                    }
                }
            }
        }

        void IDatabaseConnector.CreateEntry<T>(T item, string filePath)
        {
            using (_sync.Write())
            {
                try
                {
                    string jsonText = JsonConvert.SerializeObject(item, Formatting.Indented);

                    FileRequest request = new FileRequest()
                    {
                        FileOperation = FileOperation.Write,
                        FilePath = filePath,
                        Lines = new List<string>() { jsonText },
                    };
                    fileRequestsManager.RegisterRequest(request);
                    log.Debug($"Created a file at {filePath}");
                }
                catch(Exception ex)
                {
                    log.Error($"Failed to create a file at this file path {filePath}");
                    log.Info(ex.InnerException.Message);
                }
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
            try
            {
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

            }
            catch(Exception ex)
            {
                log.Error($"Failed to load items of type {fileType}");
                log.Info(ex.InnerException.Message);
            }
            return items;
        }

        T IDatabaseConnector.LoadEntry<T>(string filePath)
        {
            try
            {

                T entry = new T();
                using (_sync.Write())
                {
                    entry = JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));
                }
                log.Debug($"Successfully loaded a file from {filePath}");
                return entry;
            }
            catch(Exception ex)
            {
                log.Error($"Failed to load file from {filePath}");
                log.Info(ex.InnerException.Message);
            }
            return null;
        }
    }
}