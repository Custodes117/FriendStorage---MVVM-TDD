using FriendStorage.UI.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FriendStorage.Model;
using FriendStorage.UI.DataProvider;
using Xunit;
using Moq;
using Prism.Events;

namespace FriendStorage.UITests.ViewModel
{
    public class NavigationViewModelTests
    {
        private NavigationViewModel _viewModel;

        public NavigationViewModelTests()
        {
            var eventAggregatorMock = new Mock<IEventAggregator>();
            var navigationDataProviderMock = new Mock<INavigationDataProvider>();
            navigationDataProviderMock.Setup(dp => dp.GetAllFriends()).Returns(new List<LookupItem>()
            {
                new LookupItem () {Id = 1, DisplayMember = "Julia"},
                new LookupItem() {Id = 2, DisplayMember = "Thomas"}
            });

            _viewModel = new NavigationViewModel(navigationDataProviderMock.Object, eventAggregatorMock.Object);
        }

        [Fact]
        public void ShouldLoadFriends ()
        {
            //Arrange
            
            //Act
            _viewModel.Load ();
            var friend1 = _viewModel.Friends.SingleOrDefault (f => f.Id == 1);
            var friend2 = _viewModel.Friends.SingleOrDefault (f => f.Id == 2);

            //Assert
            Assert.Equal (2, _viewModel.Friends.Count);

            Assert.NotNull (friend1);
            Assert.Equal ("Julia", friend1.DisplayMember);

            Assert.NotNull (friend2);
            Assert.Equal ("Thomas", friend2.DisplayMember);
        }

        [Fact]
        public void ShouldLoadViewModelOnlyOnce ()
        {
            //Arrange

            //Act
            _viewModel.Load ();
            _viewModel.Load ();

            //Assert
            Assert.Equal (2, _viewModel.Friends.Count);
        }
    }

    [Obsolete ]
    public class NavigationDataProviderMock : INavigationDataProvider
    {
        public IEnumerable<LookupItem> GetAllFriends ()
        {
            yield return new LookupItem () {Id = 1, DisplayMember = "Julia"};
            yield return new LookupItem() {Id = 2, DisplayMember = "Thomas"};
        }
    }
}
