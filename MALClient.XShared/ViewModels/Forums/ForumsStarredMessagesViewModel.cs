using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Models.Enums;
using MALClient.Models.Interfaces;
using MALClient.Models.Models;
using MALClient.Models.Models.Forums;
using MALClient.XShared.Interfaces;
using MALClient.XShared.NavArgs;
using IHandyDataStorage = MALClient.XShared.ViewModels.Interfaces.IHandyDataStorage;

namespace MALClient.XShared.ViewModels.Forums
{
    public class ForumsStarredMessagesViewModel : ViewModelBase , ISelfBackNavAware
    {
        private readonly IHandyDataStorage _dataStorage;
        //count - unique thread
        private Dictionary<MalUser,List<StarredForumMessage>> _leaderboard;
        private ICommand _gotoTopicCommand;
        private ICommand _goToProfileCommand;
        private ICommand _unstarMessageCommand;

        public ForumsStarredMessagesViewModel(IHandyDataStorage dataStorage)
        {
            _dataStorage = dataStorage;
        }

        public void Init(ForumStarredMessagesNavigationArgs args)
        {
            ViewModelLocator.ForumsMain.CurrentBackNavRegistrar = this;
            if (Leaderboard != null && Leaderboard.Count == _dataStorage.StarredMessages.Count)
                return;

            LoadLeaderboard();
        }

        private void LoadLeaderboard()
        {
            var data = _dataStorage.StarredMessages;
            _leaderboard = new Dictionary<MalUser, List<StarredForumMessage>>();
            foreach (var user in data.Keys.OrderByDescending(s => data[s].Count))
            {
                Leaderboard.Add(data[user][0].Poster, data[user]);
            }
            RaisePropertyChanged(() => Leaderboard);
            RaisePropertyChanged(() => EmptyNoticeVisibility);
        }


        public Dictionary<MalUser,List<StarredForumMessage>> Leaderboard
        {
            get { return _leaderboard; }
            set
            {
                _leaderboard = value;
                RaisePropertyChanged(() => Leaderboard);
                RaisePropertyChanged(() => EmptyNoticeVisibility);
            }
        }


        public ICommand GotoTopicCommand
            => _gotoTopicCommand ?? (_gotoTopicCommand = new RelayCommand<StarredForumMessage>(
                   message =>
                   {
                       ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageForumIndex,
                           new ForumStarredMessagesNavigationArgs(message.MessageId));
                       ViewModelLocator.GeneralMain.Navigate(PageIndex.PageForumIndex,
                           new ForumsTopicNavigationArgs(null, long.Parse(message.MessageId)));
                   }));

        public ICommand UnstarMessageCommand
            => _unstarMessageCommand ?? (_unstarMessageCommand = new RelayCommand<StarredForumMessage>(
                   message =>
                   {
                       _dataStorage.UnstarForumMessage(message.MessageId,message.Poster);
                       LoadLeaderboard();
                   }));

        public ICommand GoToProfileCommand
            => _goToProfileCommand ?? (_goToProfileCommand = new RelayCommand<StarredForumMessage>(
                   message =>
                   {
                       ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageForumIndex,
                           new ForumStarredMessagesNavigationArgs(message.MessageId));
                       ViewModelLocator.GeneralMain.Navigate(PageIndex.PageProfile,
                           new ProfilePageNavigationArgs
                           {
                               TargetUser = message.Poster.Name
                           });
                   }));

        public bool EmptyNoticeVisibility => !Leaderboard?.Any() ?? true;

        public void RegisterSelfBackNav()
        {
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageForumIndex,new ForumStarredMessagesNavigationArgs());
        }
    }
}
