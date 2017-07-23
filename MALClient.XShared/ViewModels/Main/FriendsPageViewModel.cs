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
using MALClient.XShared.Comm.Profile;
using MALClient.XShared.NavArgs;

namespace MALClient.XShared.ViewModels.Main
{
    public class FriendsPageViewModel : ViewModelBase
    {
        private ObservableCollection<MalFriend> _friends;
        private bool _loading;
        private FriendsPageNavArgs _lastArgs;

        private Dictionary<string, List<MalFriend>> _friendsCache = new Dictionary<string, List<MalFriend>>();
        private ICommand _navigateUserCommand;

        public async void NavigatedTo(FriendsPageNavArgs args)
        {
            if (args.Equals(_lastArgs))
             return;
            if(_friendsCache.ContainsKey(args.TargetUser.Name.ToLower()))
                Friends = new ObservableCollection<MalFriend>(_friendsCache[args.TargetUser.Name.ToLower()]);
            else
            {
                Loading = true;
                var result = await new FriendsQuery(args.TargetUser.Name).GetFriends();
                _friendsCache[args.TargetUser.Name.ToLower()] = result;

                Friends = new ObservableCollection<MalFriend>(result);
                Loading = false;
            }

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

        public bool Loading
        {
            get { return _loading; }
            set
            {
                _loading = value;
                RaisePropertyChanged();
            }
        }

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
    }
}
