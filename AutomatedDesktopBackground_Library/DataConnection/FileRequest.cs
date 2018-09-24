﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary.DataConnection
{
    public class FileRequest
    {
        public List<string> Lines { get; set; }
        public string FilePath { get; set; }
        public FileOperation FileOperation { get; set; }
        public bool SucessfulOperation { get; set; }
        public string CopyPath { get; set; }
        public bool DeleteOrigin { get; set; }
        public ImageModel Image { get; set; }
    }
}
