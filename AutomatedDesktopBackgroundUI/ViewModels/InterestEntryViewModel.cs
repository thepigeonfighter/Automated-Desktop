using AutomatedDesktopBackgroundLibrary.StringExtensions;
using AutomatedDesktopBackgroundUI.Config;
using AutomatedDesktopBackgroundUI.Models;
using AutomatedDesktopBackgroundUI.SessionData;

using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutomatedDesktopBackgroundUI.ViewModels
{
    public class InterestEntryViewModel:Screen
    {
        private const string defaultMessage = "Enter Search Here";
        private string _interestName = defaultMessage;
        private ISessionContext _sessionContext;
        private List<InterestInfoModel> _interests;
        public InterestEntryViewModel(ISessionContext sessionContext)
        {
            _sessionContext = sessionContext;
            _sessionContext.PropertyChanged += SessionContextPropertyChanged;
            _interests = _sessionContext.Interests;
        }

        private void SessionContextPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == PropertyNames.Interests)
            {
                _interests = _sessionContext.Interests;
            }
        }

        public string InterestName
        {
            get { return _interestName; }
            set
            {
                
                _interestName = value;               
                NotifyOfPropertyChange(() => InterestName);

            }
        }
        public void Submit(string interestName)
        {
            InterestInfoModel item = new InterestInfoModel()
            {
                Name = interestName.MakePrettyString()

            };
            _sessionContext.AddInterest(item);
            InterestName = "";
        }
        public void OnSelected()
        {
            if (InterestName == defaultMessage)
            {
                InterestName = "";
            }
        }
        public bool CanSubmit(string interestName)
        {
            if(interestName == defaultMessage)
            {
                return false;
            }
            if(String.IsNullOrEmpty(interestName))
            {
                return false;
            }
            if(_interests.Any(x => x.Name == interestName.MakePrettyString()))
            {
                return false;
            }
            return true;

        }

    }
}
