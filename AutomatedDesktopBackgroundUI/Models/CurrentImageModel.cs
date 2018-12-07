using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundUI.Models
{
    public class CurrentImageModel
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        private bool _isHated;

        public bool IsHated
        {
            get { return _isHated; }
            set { _isHated = value; }
        }
        private bool _isLiked;

        public bool IsLiked
        {
            get { return _isLiked; }
            set { _isLiked = value; }
        }
        private string _localUrl;

        public string LocalUrl
        {
            get { return _localUrl; }
            set { _localUrl = value; }
        }




    }
}
