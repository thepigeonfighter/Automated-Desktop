using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundUI.Models
{
    public class RefreshStateModel
    {
        private bool _isBackgroundRefreshing;

        public bool IsBackgroundRefreshing
        {
            get { return _isBackgroundRefreshing; }
            set { _isBackgroundRefreshing = value; }
        }
        private bool _isCollectionRefreshing;

        public bool IsCollectionRefreshing
        {
            get { return _isCollectionRefreshing; }
            set { _isCollectionRefreshing = value; }
        }

    }
}
