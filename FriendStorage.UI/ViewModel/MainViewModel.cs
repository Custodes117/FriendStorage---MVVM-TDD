using System;
using System.Collections.ObjectModel;
using FriendStorage.DataAccess;
using FriendStorage.UI.DataProvider;
using FriendStorage.UI.Events;
using Prism.Events;
using System.Linq;

namespace FriendStorage.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private IFriendEditViewModel _selectedFriendEditViewModel;
        public INavigationViewModel NavigationViewModel { get; private set; }
        private Func<IFriendEditViewModel> _friendEditVmCreator;

        public ObservableCollection<IFriendEditViewModel> FriendEditViewModels { get; private set; }

        public MainViewModel(INavigationViewModel navigationViewModel,
            Func<IFriendEditViewModel> friendEditVmCreator,
            IEventAggregator eventAggregator)
        {
            //new NavigationViewModel(new NavigationDataProvider (() => new FileDataService ()));
            NavigationViewModel = navigationViewModel;
            FriendEditViewModels = new ObservableCollection<IFriendEditViewModel>();
            _friendEditVmCreator = friendEditVmCreator;
            eventAggregator.GetEvent<OpenFriendEditViewEvent>().Subscribe(OnOpenFriendEditView);
        }

        private void OnOpenFriendEditView(int friendId)
        {
            var friendEditVm = FriendEditViewModels.SingleOrDefault(friendVm => friendVm.Friend?.Id == friendId);

            if(friendEditVm == null)
            {
                friendEditVm = _friendEditVmCreator();
                FriendEditViewModels.Add(friendEditVm);
                friendEditVm.Load(friendId);
            }            
            SelectedFriendEditViewModel = friendEditVm;
        }

        public IFriendEditViewModel SelectedFriendEditViewModel
        {
            get { return _selectedFriendEditViewModel; }
            set
            {
                _selectedFriendEditViewModel = value;
                OnPropertyChanged();
            }
        }

        public void Load()
        {
            NavigationViewModel.Load();
        }


    }
}
