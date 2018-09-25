using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutomatedDesktopBackgroundLibrary.DataConnection;
using AutomatedDesktopBackgroundLibrary.Utility;
namespace AutomatedDesktopBackgroundLibrary
{
    public class TextFileConnector : ObjectToTextProcessor, IDatabaseConnector
    {
        private Queue<FileRequest> DeleteRequestQueue = new Queue<FileRequest>();
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

        List<T> IDatabaseConnector.Update<T>(T item, string filePath)
        {
            return UpdateEntry(item, filePath);
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
            
                FileRequest request = new FileRequest();
                request.FileOperation = FileOperation.Delete;
                request.FilePath = filePath;
                if(DeleteRequestQueue.Count>0)
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
                FileRequest request = new FileRequest();
                request.FileOperation = FileOperation.Copy;
                request.FilePath = image.LocalUrl;
                request.CopyPath = $"{copyPath}/{image.Name}";
                request.DeleteOrigin = true;
                request.Image = image;
                requestsManager.RegisterRequest(request);
            }
            try
            {
                FileRequest deletion = new FileRequest();
                deletion.FilePath = oldFilePath;
                deletion.FileOperation = FileOperation.Delete;
                requestsManager.RegisterRequest(deletion);
                image.LocalUrl = $"{copyPath}/{image.Name}";

            }
            catch(Exception e)
            {
                throw e;
            }
            DataKeeper.UpdateFavoriteImage(image);

        }

        public void DeleteImages(List<ImageModel> images)
        {
            foreach(ImageModel i in images)
            {
                FileRequest request = new FileRequest();
                request.FileOperation = FileOperation.Delete;
                request.FilePath = i.LocalUrl;
                
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
            if(DeleteRequestQueue.Count>0)
            {
                FileRequest request = DeleteRequestQueue.Dequeue();
                requestsManager.RegisterRequest(request);
            }
        }
    }
}
