using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Models.Enums;
using MALClient.Models.Models;
using MALClient.Models.Models.MalSpecific;
using MALClient.XShared.Comm.MalSpecific;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;
using MALClient.XShared.Utils.Enums;

namespace MALClient.XShared.ViewModels.Main
{
    public class FriendsFeedsViewModel : ViewModelBase
    {
        private List<UserFeedEntryModel> _feeds;
        private ICommand _navigateDeitalsCommand;
        private ICommand _navigateProfileCommand;
        private bool _loading;

        public double ItemWidth => AnimeItemViewModel.MaxWidth;

        public List<UserFeedEntryModel> Feeds
        {
            get { return _feeds; }
            set
            {
                _feeds = value;
                RaisePropertyChanged(() => Feeds);
            }
        }

        public async void Init(bool force = false)
        {
            if (ViewModelLocator.ProfilePage.MyFriends == null)
            {
                Loading = true;
                ViewModelLocator.GeneralMain.LockCurrentStatus = true;
                await ViewModelLocator.ProfilePage.LoadProfileData(new ProfilePageNavigationArgs{TargetUser = Credentials.UserName});
                ViewModelLocator.GeneralMain.LockCurrentStatus = false;
            }
            if(Feeds != null && !force)
                return;
            Loading = true;
            Feeds = new List<UserFeedEntryModel>();
            var source =
                ViewModelLocator.ProfilePage.MyFriends.Concat(ResourceLocator.HandyDataStorage.PinnedUsers)
                    .Distinct(MalUser.NameComparer)
                    .ToList();
            Feeds =
                (await new MalFriendsFeedsQuery(source).GetFeeds()).OrderByDescending(
                    model => model.Date).ToList();
            Loading = false;
        }

        public bool Loading
        {
            get { return _loading; }
            set
            {
                _loading = value;
                RaisePropertyChanged(() => Loading);
            }
        }

        public ICommand NavigateDeitalsCommand
            => _navigateDeitalsCommand ?? (_navigateDeitalsCommand = new RelayCommand<UserFeedEntryModel>(
                   item =>
                   {
                       if
                           (ViewModelLocator.AnimeDetails.Id != item.Id)
                           ViewModelLocator.GeneralMain.Navigate
                           (PageIndex.PageAnimeDetails,
                               new AnimeDetailsPageNavigationArgs(item.Id, item.Title, null, null)
                               {
                                   Source = PageIndex.PageFeeds
                               });
                   }));

        public ICommand NavigateProfileCommand
            => _navigateProfileCommand ?? (_navigateProfileCommand = new RelayCommand<MalUser>(
                   user =>
                   {
                       ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageFeeds,null);
                       ViewModelLocator.GeneralMain.Navigate(PageIndex.PageProfile,new ProfilePageNavigationArgs{TargetUser = user.Name});
                   }));

    }
}