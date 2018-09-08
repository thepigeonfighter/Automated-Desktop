﻿using AutomatedDesktopBackgroundLibrary;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary
{
    public static class TextConnectorProcessor
    {

        public static string FullFilePath(this string fileName)
        {
            //To change the file path change the value in the app config file
            return $"{GlobalConfig.FileSavePath}\\{fileName}";
        }
        /// <summary>
        /// Returns the file as a list of strings
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
         public static List<string> LoadFileAsStrings(this string file)
        {
            if(!File.Exists(file) )
            {
                return new List<string>();
            }
            return File.ReadAllLines(file).ToList();
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
        public static  void SaveToTextFile<T>(this List<T> items, string filePath) where T: class, new()
        {
            List<string> lines = new List<string>();
            StringBuilder line = new StringBuilder();
            //If you try to save an empty list it will throw an error
            if(items == null || string.IsNullOrEmpty(filePath))
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
                File.WriteAllLines(filePath, lines);
            
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
        public static T CreateEntry<T>(T entry, string filePath) where T: class, new()
        {
            List<T> entries = LoadFromTextFile<T>(filePath);
            T tempObj= new T();
            var cols = tempObj.GetType().GetProperties();
            int currentId = 1;
            if (entries.Count > 0)
            {

                foreach(var item in entries)
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
                        throw  new ArgumentNullException("There was a problem finding the Id of the object");
                    }
                }
                //Increments the id to have a unique value 
                currentId += 1;
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
        public static List<T> LoadFromTextFile<T>( this string filePath) where T: class, new()
        {
            List<T> output = new List<T>();
            T entry = new T();
            List<string> lines = new List<string>();
            var cols = entry.GetType().GetProperties();
            if(string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException("Invalid filepath given, no file can be loaded. Stop fooling around and straighten up!");
            }
            lines = LoadFileAsStrings(filePath);
            

            if(lines.Count <2)
            {
                return output;
              //  throw new IndexOutOfRangeException("The loaded file is either empty or corrupted");
            }
            var headers = lines[0].Split(',');
            lines.RemoveAt(0);
            foreach(var row in lines)
            {
                entry = new T();
                var vals = row.Split(',');
                for( var i = 0; i < headers.Length; i++)
                {
                    foreach(var col in cols)
                    {
                        if( col.Name == headers[i])
                        {
                            col.SetValue(entry, Convert.ChangeType(vals[i], col.PropertyType));

                        }
                    }
                }
                output.Add(entry);
            }
            return output;
            
        }
        public static List<T> GetAllItemsWithThisId<T>(string propertyName, List<T> itemsTolookThrough, int requiredId) where T : class, new()
        {
            List<T> output = new List<T>();
            foreach(T item in itemsTolookThrough)
            {
                Type type = item.GetType();
                PropertyInfo propertyInfo = type.GetProperty(propertyName);
                if((int)propertyInfo.GetValue(item) == requiredId)
                {
                    output.Add(item);
                }
                
            }
            return output;
        }
        private const FileOptions DefaultOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;


    }
}
