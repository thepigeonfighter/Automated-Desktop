using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundUI.Models
{
    public class TimeModel
    {
        private TimeSpan _time;

        public TimeSpan Time
        {
            get { return _time; }
            set { _time = value; }
        }
        public string Name { get { return this.ToString(); } private set { } }
        public override string ToString()
        {
            if(Time == null)
            {
                return "Not Set";
            }
            if (Time.Days > 0)
            {
                if(Time.Days == 1)
                {
                    return $"{Time.Days} day";
                }
                return $"{Time.Days} days";
                //return days
            }
            else if (Time.Hours > 0)
            {
                if (Time.Hours == 1)
                {
                    return $"{Time.Hours} hour";
                }
                return $"{Time.Hours} hours";
                // return hour format
            }
            else
            {
                if (Time.Minutes == 1)
                {
                    return $"{Time.Minutes} Minute";
                }
                return $"{Time.Minutes} minutes";
                //return minute format
            }
        }

    }
}
