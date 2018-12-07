using AutomatedDesktopBackgroundUI.Config;
using AutomatedDesktopBackgroundUI.Models;
using AutomatedDesktopBackgroundUI.SessionData;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundUI.ViewModels
{
    public class InterestListViewModel:Conductor<Screen>
    {
        private InterestInfoViewModel interestInfoViewModel;
        private bool _interestSelected;
        private ISessionContext _sessionContext;
        private IEventAggregator _eventAggregator;
        public InterestListViewModel(ISessionContext sessionContext, IEventAggregator eventAggregator)
        {
            _sessionContext = sessionContext;
            Interests = new BindableCollection<InterestInfoModel>( _sessionContext.Interests);
            _sessionContext.PropertyChanged += SessionConTextPropertyChanged;
            _eventAggregator = eventAggregator;
            
        }

        private void SessionConTextPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == PropertyNames.Interests)
            {
                Interests = new BindableCollection<InterestInfoModel>(_sessionContext.Interests);
            }
        }

        public bool InterestSelected
        {
            get { return _interestSelected; }
            set
            {
                _interestSelected = value;
                NotifyOfPropertyChange(() => InterestSelected);
            }
        }

        private BindableCollection<InterestInfoModel> _interests;

        public BindableCollection<InterestInfoModel> Interests
        {
            get { return _interests; }
            set
            {
                _interests = value;
                NotifyOfPropertyChange(() => Interests);

            }
        }
        private InterestInfoModel _selectedInterest;

        public InterestInfoModel SelectedInterest
        {
            get { return _selectedInterest; }
            set
            {

                _selectedInterest = value;
                _sessionContext.SelectedInterest = value;
                ActivateInterestViewModel();
                NotifyOfPropertyChange(() => SelectedInterest);

            }
        }
        private void DeactivateInterestInfo()
        {
            if(interestInfoViewModel!=null)
            {
                DeactivateItem(interestInfoViewModel, true);
            }
        }
        private void ActivateInterestViewModel()
        {
            if (SelectedInterest != null)
            {
                interestInfoViewModel = new InterestInfoViewModel(SelectedInterest,_eventAggregator,_sessionContext);
                ActivateItem(interestInfoViewModel);
            }
            else
            {
                DeactivateInterestInfo();
            }
        }

    }
}
