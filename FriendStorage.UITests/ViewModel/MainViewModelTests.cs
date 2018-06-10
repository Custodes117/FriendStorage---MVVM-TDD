using FriendStorage.Model;
using FriendStorage.UI.Events;
using FriendStorage.UI.ViewModel;
using Moq;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FriendStorage.UITests.Extensions;

namespace FriendStorage.UITests.ViewModel
{
    public class MainViewModelTests
    {
        private Mock<INavigationViewModel> _navigationViewModelMock;
        private OpenFriendEditViewEvent _openFriendEditViewEvent;
        private Mock<IEventAggregator> _eventAggregatorMock;
        private MainViewModel _viewModelUnderTest;
        private List<Mock<IFriendEditViewModel>> _friendEditViewModelMocks;

        public MainViewModelTests()
        {
            _navigationViewModelMock = new Mock<INavigationViewModel>();
            _friendEditViewModelMocks = new List<Mock<IFriendEditViewModel>>();
            _openFriendEditViewEvent = new OpenFriendEditViewEvent();
            _eventAggregatorMock = new Mock<IEventAggregator>();
            _eventAggregatorMock.Setup(ea => ea.GetEvent<OpenFriendEditViewEvent>()).Returns(_openFriendEditViewEvent);

            _viewModelUnderTest = new MainViewModel(_navigationViewModelMock.Object,
                CreateFriendEditViewModel, _eventAggregatorMock.Object);
        }


        [Fact]
        public void ShouldCallTheLoadMethodOfNavigationViewModel()
        {
            //Arrange

            //Act
            _viewModelUnderTest.Load();

            //Assert
            //Assert.True(navigationViewModelMock.LoadHasBeenCalled);
            _navigationViewModelMock.Verify(vm => vm.Load(), Times.Once);
        }

        [Fact]
        public void ShouldAddFriendEditViewModelAndLoadAndSelectIt()
        {
            const int friendId = 7;

            _openFriendEditViewEvent.Publish(friendId);

            Assert.Single(_viewModelUnderTest.FriendEditViewModels);
            var friendEditVm = _viewModelUnderTest.FriendEditViewModels.First();
            Assert.Equal(friendEditVm, _viewModelUnderTest.SelectedFriendEditViewModel);
            _friendEditViewModelMocks.First().Verify(vm => vm.Load(friendId), Times.Once);
        }

        [Fact]
        public void ShouldAddFriendEditViewModelsOnlyOnce()
        {
            _openFriendEditViewEvent.Publish(5);
            _openFriendEditViewEvent.Publish(5);
            _openFriendEditViewEvent.Publish(6);
            _openFriendEditViewEvent.Publish(7);
            _openFriendEditViewEvent.Publish(7);

            Assert.Equal(3, _viewModelUnderTest.FriendEditViewModels.Count);
        }

        [Fact]
        public void ShouldRaisePropertyChangedEventForSelectedFriendEditViewModel()
        {
            var friendEditVmMock = new Mock<IFriendEditViewModel>();
            var fired = _viewModelUnderTest.IsPropertyChangedFired(() =>
            {
                _viewModelUnderTest.SelectedFriendEditViewModel = friendEditVmMock.Object;
            }, nameof(_viewModelUnderTest.SelectedFriendEditViewModel));

            Assert.True(fired);
        }

        private IFriendEditViewModel CreateFriendEditViewModel()
        {
            var friendEditViewModelMock = new Mock<IFriendEditViewModel>();
            friendEditViewModelMock.Setup(vm => vm.Load(It.IsAny<int>())).Callback<int>(friendId =>
                {
                    friendEditViewModelMock.Setup(vm => vm.Friend).Returns(new Friend { Id = friendId });
                });

            _friendEditViewModelMocks.Add(friendEditViewModelMock);
            return friendEditViewModelMock.Object;
        }
    }

    [Obsolete]
    public class NavigationViewModelMock : INavigationViewModel
    {
        public bool LoadHasBeenCalled { get; set; }

        public void Load()
        {
            LoadHasBeenCalled = true;
        }
    }
}
