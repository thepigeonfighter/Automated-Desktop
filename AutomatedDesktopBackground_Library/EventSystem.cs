using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary
{
    public class EventSystem
    {

        public event EventHandler<int> DownloadPercentageEvent;
        public event EventHandler<bool> DownloadedImageEvent;
        public event EventHandler<bool> DownloadCompleteEvent;
        public event EventHandler<string> ResultNotFoundEvent;
        public event EventHandler<string> StartedDownloadingEvent;
        public event EventHandler<string> ConfigSettingChangedEvent;
        public event EventHandler<string> UpdateBackgroundEvent;
        public void InvokePercentChangeEvent(int percentage)
        {
            DownloadPercentageEvent?.Invoke(this, percentage );
        }
        public void InvokeDownloadImageEvent(bool success)
        {
            DownloadedImageEvent?.Invoke(this, success);
        }
        public void InvokeDownloadCompleteEvent(bool sucess)
        {
            DownloadCompleteEvent?.Invoke(this, sucess);
        }
        public void InvokeResultNotFoundEvent()
        {
            ResultNotFoundEvent?.Invoke(this,"No Results Found");
        }
        public void InvokeStartedDownloadingEvent()
        {
            StartedDownloadingEvent?.Invoke(this,"Download Started");
        }
        /// <summary>
        ///This should only be called in the property "Set" value. Otherwise it will result in a double call
        ///It triggers an event when important configuration settings are changed
        /// </summary>
        public void InvokeConfigSettingChanged()
        {
            ConfigSettingChangedEvent?.Invoke(this, "A Global Config Setting was changed");
        }
        /// <summary>
        /// This should only be called in the property "Set" value. Otherwise it will result in a double call
        /// It triggers an event everytime the current background changes
        /// </summary>
        public void InvokeUpdateBackroundEvent()
        {
            UpdateBackgroundEvent?.Invoke(this, "Updating background");
        }
    }
}
