﻿using System;

namespace AutomatedDesktopBackgroundLibrary
{
    public class EventSystem
    {
        public event EventHandler<int> DownloadPercentageEvent;

        public event EventHandler<string> DownloadedImageEvent;

        public event EventHandler<bool> DownloadCompleteEvent;

        public event EventHandler<string> ResultNotFoundEvent;

        public event EventHandler<string> StartedDownloadingEvent;

        public event EventHandler<string> UpdateBackgroundEvent;

        public event EventHandler<string> ImageHatingHasCompletedEvent;

        public event EventHandler<string> ApplicationResetEvent;

        public event EventHandler<string> OnErrorsEncounteredDuringDownloadEvent;

        public EventHandler OnRefreshStatusChange;

        public event EventHandler<string> ShowCustomMessageBoxEvent;

        public void InvokePercentChangeEvent(int percentage)
        {
            DownloadPercentageEvent?.Invoke(this, percentage);
        }

        public void InvokeDownloadImageEvent(string progess)
        {
            DownloadedImageEvent?.Invoke(this, progess);
        }

        public void InvokeDownloadCompleteEvent(bool sucess)
        {
            DownloadCompleteEvent?.Invoke(this, sucess);
        }

        public void InvokeResultNotFoundEvent()
        {
            ResultNotFoundEvent?.Invoke(this, "No Results Found");
        }

        public void InvokeStartedDownloadingEvent()
        {
            StartedDownloadingEvent?.Invoke(this, "Download Started");
        }
        public void InvokeRefreshStatusChangedEvent()
        {
            OnRefreshStatusChange?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// This should only be called in the property "Set" value. Otherwise it will result in a double call
        /// It triggers an event everytime the current background changes
        /// </summary>
        public void InvokeUpdateBackroundEvent()
        {
            UpdateBackgroundEvent?.Invoke(this, "Updating background");
        }

        public void InvokeImageHatingCompleteEvent()
        {
            ImageHatingHasCompletedEvent?.Invoke(this, "Image has been removed");
        }

        public void InvokeApplicationResetEvent()
        {
            ApplicationResetEvent?.Invoke(this, "Application was reset");
        }
        public void InvokeErrorsEncounteredEvent(string message)
        {
            OnErrorsEncounteredDuringDownloadEvent?.Invoke(this, message);
        }
        public void InvokeShowCustomMessageBoxEvent(string message)
        {
            ShowCustomMessageBoxEvent?.Invoke(this, message);
        }

    }
}