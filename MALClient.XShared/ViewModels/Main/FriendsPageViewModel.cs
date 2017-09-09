using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Models.Enums;
using MALClient.Models.Models.MalSpecific;
using MALClient.XShared.Comm.MagicalRawQueries.Profile;
using MALClient.XShared.Comm.Profile;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;

namespace MALClient.XShared.ViewModels.Main
{
    public class FriendsPageViewModel : ViewModelBase
    {
        private ObservableCollection<MalFriend> _friends;
        private bool _loading;
        private FriendsPageNavArgs _lastArgs;

        private readonly Dictionary<string, List<MalFriend>> _friendsCache = new Dictionary<string, List<MalFriend>>();
        private ICommand _navigateUserCommand;
        private ObservableCollection<MalFriendRequest> _requests;
        private bool _showPending;
        private bool _loadingPending;
        private bool _requestsEmptyNoticeVisibility;
        private bool _friendsEmptyNoticeVisibility;
        private ICommand _acceptFriendRequest;
        private ICommand _denyFriendRequest;

        public async void NavigatedTo(FriendsPageNavArgs args)
        {
            if (args.Equals(_lastArgs))
             return;
            if(_friendsCache.ContainsKey(args.TargetUser.Name.ToLower()))
                Friends = new ObservableCollection<MalFriend>(_friendsCache[args.TargetUser.Name.ToLower()]);
            else
            {
                
                Loading = true;
                Friends = new ObservableCollection<MalFriend>();

                var result = await new FriendsQuery(args.TargetUser.Name).GetFriends();
                _friendsCache[args.TargetUser.Name.ToLower()] = result;

                Friends = new ObservableCollection<MalFriend>(result);
                Loading = false;
            }

            if (args.TargetUser.Name.Equals(Credentials.UserName, StringComparison.CurrentCultureIgnoreCase))
            {
                ShowPending = true;
                RefreshPendingCommand.Execute(null);
            }
            else
                ShowPending = false;

            FriendsEmptyNoticeVisibility = !Friends.Any();

            _lastArgs = args;
        }


        public ObservableCollection<MalFriend> Friends
        {
            get { return _friends; }
            set
            {
                _friends = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<MalFriendRequest> Requests
        {
            get { return _requests; }
            set
            {
                _requests = value;
                RaisePropertyChanged();
            }
        }

        public bool ShowPending
        {
            get { return _showPending; }
            set
            {
                _showPending = value;
                RaisePropertyChanged();
            }
        }

        public bool Loading
        {
            get { return _loading; }
            set
            {
                _loading = value;
                RaisePropertyChanged();
            }
        }

        public bool LoadingPending
        {
            get { return _loadingPending; }
            set
            {
                _loadingPending = value;
                RaisePropertyChanged();
            }
        }

        public bool RequestsEmptyNoticeVisibility
        {
            get { return _requestsEmptyNoticeVisibility; }
            set
            {
                _requestsEmptyNoticeVisibility = value;
                RaisePropertyChanged();
            }
        }

        public bool FriendsEmptyNoticeVisibility
        {
            get { return _friendsEmptyNoticeVisibility; }
            set
            {
                _friendsEmptyNoticeVisibility = value;
                RaisePropertyChanged();
            }
        }

        public ICommand RefreshPendingCommand => new RelayCommand(async () =>
        {
            LoadingPending = true;
            Requests = new ObservableCollection<MalFriendRequest>(await MalFriendsQueries.GetFriendRequests());
            LoadingPending = false;

            RequestsEmptyNoticeVisibility = !Requests.Any();
        });

        public ICommand NavigateUserCommand => _navigateUserCommand ?? (_navigateUserCommand =
                                                   new RelayCommand<MalFriend>(
                                                       friend =>
                                                       {
                                                           ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageFriends,_lastArgs);
                                                           ViewModelLocator.GeneralMain.Navigate(PageIndex.PageProfile,
                                                               new ProfilePageNavigationArgs
                                                               {
                                                                   AllowBackNavReset = false,
                                                                   TargetUser = friend.User.Name
                                                               });
                                                       }));

        public ICommand AcceptFriendRequestCommand => _acceptFriendRequest ?? (_acceptFriendRequest =
                                                   new RelayCommand<MalFriendRequest>(
                                                       async request =>
                                                       {
                                                           if (await MalFriendsQueries.RespondToFriendRequest(request.Id,true))
                                                           {
                                                               Requests.Remove(request);
                                                               Friends.Add(new MalFriend
                                                               {
                                                                   User = request.User,
                                                                   FriendsSince = "Today",
                                                                   LastOnline = "N/A",
                                                               });
                                                           }
                                                       }));

        public ICommand DenyFriendRequestCommand => _denyFriendRequest ?? (_denyFriendRequest =
                                                   new RelayCommand<MalFriendRequest>(
                                                       async request =>
                                                       {
                                                           if (await MalFriendsQueries.RespondToFriendRequest(request.Id, false))
                                                           {
                                                               Requests.Remove(request);
                                                           }
                                                       }));

    }
}
