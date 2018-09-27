using AutomatedDesktopBackgroundLibrary.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace AutomatedDesktopBackgroundLibrary.DataConnection
{
    public class ObjectToTextProcessor
    {
        protected static FileRequestsManager _requestsManager = new FileRequestsManager();
        protected static ReaderWriterLockSlim _sync = new ReaderWriterLockSlim();

        private static List<string> LoadFileAsStrings(string file)
        {
            using (_sync.Write())
            {
                if (!File.Exists(file))
                {
                    return new List<string>();
                }

                FileRequest request = new FileRequest
                {
                    FilePath = file,
                    FileOperation = FileOperation.Read
                };
                _requestsManager.RequestFileRead(request);

                return request.Lines;
            }
        }

        /// <summary>
        /// Formats that data into a text file and saves it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">
        /// The group of items that you want to save
        /// </param>
        /// <param name="filePath">
        /// The path to which you want to save this collection of data
        /// </param>
        protected static void SaveToTextFile<T>(List<T> items, string filePath) where T : class, new()
        {
            List<string> lines = new List<string>();
            StringBuilder line = new StringBuilder();
            //If you try to save an empty list it will throw an error
            if (items == null || string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException("Invalid Operation, cannot save file because" +
                    " it is missing either items or a valid save path");
            }

            //This stores the names of each of the properties in the associated class

            var cols = items[0].GetType().GetProperties();
            //This builds the header line in the text file
            foreach (var col in cols)
            {
                line.Append(col.Name);
                line.Append(",");
            }
            //Adds the header line while removing the leftover comma at the end of the line
            lines.Add(line.ToString().Substring(0, line.Length - 1));

            foreach (var item in items)
            {
                //Each item needs it's own line to store data in
                line = new StringBuilder();
                foreach (var col in cols)
                {
                    // This loops through each of the properties of the  items and adds it's value to a string
                    line.Append(col.GetValue(item));
                    // The format looks like this ' value,value,value'
                    line.Append(",");
                }
                //Adds the line that contains all the properties of the item to the collection
                // of lines that are to be written to the text file
                lines.Add(line.ToString().Substring(0, line.Length - 1));
            }
            using (_sync.Write())
            {
                FileRequest request = new FileRequest
                {
                    FilePath = filePath,
                    Lines = lines,
                    FileOperation = FileOperation.Write
                };
                _requestsManager.RegisterRequest(request);
            }
            // WriteFile(lines, filePath);
        }

        /// <summary>
        /// Adds an item to an existing file giving it a unique Id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entry">
        /// Item to be added
        /// </param>
        /// <param name="filePath">
        /// Path to save collection to.
        /// </param>
        /// <returns></returns>
        protected static T CreateEntry<T>(T entry, string filePath) where T : class, new()
        {
            List<T> entries = LoadFromTextFile<T>(filePath);
            T tempObj = new T();
            var cols = tempObj.GetType().GetProperties();
            int currentId = 1;
            if (entries.Count > 0)
            {
                foreach (var item in entries)
                {
                    try
                    {
                        Type t = item.GetType();
                        PropertyInfo prop = t.GetProperty("Id");
                        if ((int)prop.GetValue(item) > currentId)
                        {
                            currentId = (int)prop.GetValue(item);
                        }
                    }
                    catch
                    {
                        throw new ArgumentNullException("There was a problem finding the Id of the object");
                    }
                }
                //Increments the id to have a unique value
                currentId++;
            }

            //Sets the id to of the entry to the newly created id value
            try
            {
                Type type = entry.GetType();
                PropertyInfo idProp = type.GetProperty("Id");
                idProp.SetValue(entry, currentId);
            }
            catch
            {
                //In case the property named "Id" doesn't exist
                throw new ArgumentNullException("There was a problem setting the new id of the entry");
            }
            //Adds the newly created entry to the old list of entries
            entries.Add(entry);
            SaveToTextFile<T>(entries, filePath);
            //Returns to new entry with it's new id
            return entry;
        }

        protected static List<T> LoadFromTextFile<T>(string filePath) where T : class, new()
        {
            List<T> output = new List<T>();
            if (File.Exists(filePath))
            {
                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Length > 0)
                {
                    T entry = new T();
                    List<string> lines = new List<string>();
                    var cols = entry.GetType().GetProperties();
                    if (string.IsNullOrEmpty(filePath))
                    {
                        throw new ArgumentNullException("Invalid file path given, no file can be loaded. Stop fooling around and straighten up!");
                    }
                    lines = LoadFileAsStrings(filePath);

                    if (lines.Count < 2)
                    {
                        return output;
                        //  throw new IndexOutOfRangeException("The loaded file is either empty or corrupted");
                    }
                    var headers = lines[0].Split(',');
                    lines.RemoveAt(0);
                    foreach (var row in lines)
                    {
                        entry = new T();
                        var vals = row.Split(',');
                        for (var i = 0; i < headers.Length; i++)
                        {
                            foreach (var col in cols)
                            {
                                if (col.Name == headers[i])
                                {
                                    col.SetValue(entry, Convert.ChangeType(vals[i], col.PropertyType));
                                }
                            }
                        }
                        output.Add(entry);
                    }
                }
            }
            return output;
        }

        protected static List<T> GetAllItemsWithThisId<T>(string propertyName, List<T> itemsTolookThrough, int requiredId) where T : class, new()
        {
            List<T> output = new List<T>();
            foreach (T item in itemsTolookThrough)
            {
                Type type = item.GetType();
                PropertyInfo propertyInfo = type.GetProperty(propertyName);
                if ((int)propertyInfo.GetValue(item) == requiredId)
                {
                    output.Add(item);
                }
            }
            return output;
        }

        protected static void DeleteEntry<T>(T objectToDelete, string filePath) where T : class, new()
        {
            List<T> items = LoadFromTextFile<T>(filePath);

            Type t = objectToDelete.GetType();
            PropertyInfo prop = t.GetProperty("Id");
            int Id = (int)prop.GetValue(objectToDelete);

            foreach (T i in items)
            {
                Type temp = i.GetType();
                PropertyInfo tempProp = t.GetProperty("Id");
                int tempId = (int)prop.GetValue(i);
                if (tempId == Id)
                {
                    objectToDelete = i;
                    break;
                }
            }

            items.Remove(objectToDelete);
            if (items.Count > 0)
            {
                SaveToTextFile(items, filePath);
            }
            else
            {
                using (_sync.Write())
                {
                    FileRequest request = new FileRequest
                    {
                        FilePath = filePath,
                        FileOperation = FileOperation.Delete
                    };
                    _requestsManager.RegisterRequest(request);
                }
            }
        }

        protected static List<T> UpdateEntry<T>(T newEntry, string filePath) where T : class, new()
        {
            List<T> items = LoadFromTextFile<T>(filePath);
            Type t = newEntry.GetType();
            PropertyInfo prop = t.GetProperty("Id");
            int Id = (int)prop.GetValue(newEntry);
            T oldObject = null;
            foreach (T i in items)
            {
                Type temp = i.GetType();
                PropertyInfo tempProp = t.GetProperty("Id");
                int tempId = (int)prop.GetValue(newEntry);
                if (tempId == Id)
                {
                    oldObject = i;

                    break;
                }
            }
            //Swap out the old with the new

            items.Remove(oldObject);
            items.Add(newEntry);
            items = items.OrderBy(x => prop).ToList();
            SaveToTextFile(items, filePath);
            return items;
        }

        protected static List<T> UpdateCollection<T>(List<T> newEntries, string filePath) where T : class, new()
        {
            List<T> items = LoadFromTextFile<T>(filePath);
            for (int i = 0; i < newEntries.Count; i++)
            {
                Type t = newEntries[i].GetType();
                PropertyInfo prop = t.GetProperty("Id");
                int Id = (int)prop.GetValue(newEntries[i]);
                T oldObject = null;
                foreach (T j in items)
                {
                    Type temp = j.GetType();
                    PropertyInfo tempProp = t.GetProperty("Id");
                    int tempId = (int)prop.GetValue(j);
                    if (tempId == Id)
                    {
                        oldObject = j;

                        break;
                    }
                }
                //Swap out the old with the new

                items.Remove(oldObject);
                items.Add(newEntries[i]);
            }
            SaveToTextFile(items, filePath);
            return items;
        }
    }
}