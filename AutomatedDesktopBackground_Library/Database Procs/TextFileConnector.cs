using AutomatedDesktopBackgroundLibrary.DataConnection;
using AutomatedDesktopBackgroundLibrary.Utility;
using System;
using System.Collections.Generic;

namespace AutomatedDesktopBackgroundLibrary
{
    public class TextFileConnector : ObjectToTextProcessor, IDatabaseConnector
    {
        private readonly Queue<FileRequest> DeleteRequestQueue = new Queue<FileRequest>();

        public TextFileConnector()
        {
            requestsManager.OnDeletionCompleted += OnDeletionCompleted;
        }

        void IDatabaseConnector.Delete<T>(T item, string filePath)
        {
            DeleteEntry(item, filePath);
        }

        List<T> IDatabaseConnector.Load<T>(string filePath)
        {
            return LoadFromTextFile<T>(filePath);
        }

        void IDatabaseConnector.CreateEntry<T>(T item, string filePath)
        {
            CreateEntry<T>(item, filePath);
        }

        List<T> IDatabaseConnector.Update<T>(T items, string filePath)
        {
            return UpdateEntry(items, filePath);
        }

        List<T> IDatabaseConnector.Update<T>(List<T> items, string filePath)
        {
            return UpdateCollection(items, filePath);
        }

        void IDatabaseConnector.SaveToFile<T>(List<T> items, string filePath)
        {
            SaveToTextFile(items, filePath);
        }

        void IDatabaseConnector.DeleteFile(string filePath)
        {
            FileRequest request = new FileRequest
            {
                FileOperation = FileOperation.Delete,
                FilePath = filePath
            };
            if (DeleteRequestQueue.Count > 0)
            {
                DeleteRequestQueue.Enqueue(request);
            }
            else
            {
                requestsManager.RegisterRequest(request);
            }
        }

        public void DeleteAllFiles()
        {
            requestsManager.DeleteAllFiles();
        }

        public void CopyImage(ImageModel image, string copyPath)
        {
            string oldFilePath = image.LocalUrl;
            using (_sync.Write())
            {
                FileRequest request = new FileRequest
                {
                    FileOperation = FileOperation.Copy,
                    FilePath = image.LocalUrl,
                    CopyPath = $"{copyPath}/{image.Name}",
                    DeleteOrigin = true,
                    Image = image
                };
                requestsManager.RegisterRequest(request);
            }
            try
            {
                FileRequest deletion = new FileRequest
                {
                    FilePath = oldFilePath,
                    FileOperation = FileOperation.Delete
                };
                requestsManager.RegisterRequest(deletion);
                image.LocalUrl = $"{copyPath}/{image.Name}";
            }
            catch (Exception)
            {
                throw;
            }
            DataKeeper.UpdateFavoriteImage(image);
        }

        public void DeleteImages(List<ImageModel> images)
        {
            foreach (ImageModel i in images)
            {
                FileRequest request = new FileRequest
                {
                    FileOperation = FileOperation.Delete,
                    FilePath = i.LocalUrl
                };

                if (DeleteRequestQueue.Count > 0)
                {
                    DeleteRequestQueue.Enqueue(request);
                }
                else
                {
                    requestsManager.RegisterRequest(request);
                }
            }
        }

        private void OnDeletionCompleted(object sender, EventArgs e)
        {
            if (DeleteRequestQueue.Count > 0)
            {
                FileRequest request = DeleteRequestQueue.Dequeue();
                requestsManager.RegisterRequest(request);
            }
        }
    }
}