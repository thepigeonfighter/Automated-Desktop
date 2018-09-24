using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutomatedDesktopBackgroundLibrary.Utility;

namespace AutomatedDesktopBackgroundLibrary.DataConnection
{
    public class FileRequestsManager
    {


        public void RegisterRequest(FileRequest request)
        {

            HandleRequest(request);
           

        }
        public FileRequest RequestFileRead(FileRequest request)
        {

               request.Lines = File.ReadAllLines(request.FilePath).ToList();
               request.SucessfulOperation = true;
              //  OnQueueEnableEvent(this, new EventArgs());
                return request;
        }


        private void HandleRequest(FileRequest request)
        {
            switch (request.FileOperation)
            {
                case FileOperation.Delete:
                    HandleDeletions(request);
                    break;
                case FileOperation.Write:
                    HandleWrites(request);
                    break;
                case FileOperation.Copy:
                    HandleCopy(request);
                    break;
            }
        }

        private  void HandleCopy(FileRequest request)
        {


            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(request.CopyPath));
                File.Copy(request.FilePath, request.CopyPath, true);
            }
            catch(Exception e)
            {
                throw e;
            } 


            
        }

        private async Task HandleDeletions(FileRequest request)
        {

            FileInfo file = new FileInfo(request.FilePath);
                try
                {
                bool fileLocked = true;
                int tries = 0;
                int timeout = 100;
                while(fileLocked && tries< timeout)
                {
                    tries++;

                    fileLocked = IsFileLocked(file);
                    if(!fileLocked)
                    {
                        File.Delete(request.FilePath);
                        break;
                    }
                    await Task.Delay(1000);
                }
                if(tries == timeout)
                {
                    throw new Exception("Deletion timed out");
                }


                    
                }
                catch(Exception e)
                {
                    throw e;
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
                if (stream != null)
                    stream.Close();
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

                string path =StringExtensions.StringExtensions.GetApplicationDirectory();
                Directory.Delete(path,true);
                GlobalConfig.EventSystem.InvokeApplicationResetEvent();

            

        }




    }
}
