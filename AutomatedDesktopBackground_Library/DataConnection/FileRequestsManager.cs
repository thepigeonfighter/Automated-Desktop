using AutomatedDesktopBackgroundLibrary.Utility;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AutomatedDesktopBackgroundLibrary.DataConnection
{
    public class FileRequestsManager
    {
        public EventHandler OnDeletionCompleted;

        public void RegisterRequest(FileRequest request)
        {
            HandleRequest(request);
        }

        public FileRequest RequestFileRead(FileRequest request)
        {
            request.Lines = File.ReadAllLines(request.FilePath).ToList();
            request.SucessfulOperation = true;
            return request;
        }

        private void HandleRequest(FileRequest request)
        {
            switch (request.FileOperation)
            {
                case FileOperation.Delete:
                    Task.Run(() => HandleDeletions(request));
                    break;

                case FileOperation.Write:
                    HandleWrites(request);
                    break;

                case FileOperation.Copy:
                    HandleCopy(request);
                    break;
            }
        }

        private void HandleCopy(FileRequest request)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(request.CopyPath));
                File.Copy(request.FilePath, request.CopyPath, true);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task HandleDeletions(FileRequest request)
        {
            FileInfo file = new FileInfo(request.FilePath);
            try
            {
                bool fileLocked = true;
                int tries = 0;
                const int timeout = 100;
                while (fileLocked && tries < timeout)
                {
                    if (!File.Exists(request.FilePath))
                    {
                        GlobalConfig.EventSystem.InvokeImageHatingCompleteEvent();
                        OnDeletionCompleted?.Invoke(this, EventArgs.Empty);
                        break;
                    }
                    tries++;

                    fileLocked = IsFileLocked(file);
                    if (!fileLocked)
                    {
                        File.Delete(request.FilePath);
                        GlobalConfig.EventSystem.InvokeImageHatingCompleteEvent();
                        OnDeletionCompleted?.Invoke(this, EventArgs.Empty);
                        break;
                    }
                    await Task.Delay(2000).ConfigureAwait(false);
                }
                if (tries == timeout)
                {
                    throw new Exception("Deletion timed out");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                stream?.Close();
            }

            //file is not locked
            return false;
        }

        private void HandleWrites(FileRequest request)
        {
            File.WriteAllLines(request.FilePath, request.Lines);
        }

        public void DeleteAllFiles()
        {
            string path = InternalFileDirectorySystem.ApplicationDirectory;
            try
            {
                Directory.Delete(path, true);
            }
            catch (IOException)
            {
                
            }
            GlobalConfig.EventSystem.InvokeApplicationResetEvent();
        }
    }
}