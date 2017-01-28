using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Models.Enums;
using MALClient.Models.Models.Forums;
using MALClient.XShared.Delegates;
using MALClient.XShared.Interfaces;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;
using MALClient.XShared.Utils.Enums;

namespace MALClient.XShared.ViewModels.Forums
{
    public class ForumsMainViewModel : ViewModelBase
    {
        public event AmbiguousNavigationRequest NavigationRequested;

        public ObservableCollection<ForumBoards> PinnedBoards { get; } = new ObservableCollection<ForumBoards>();

        public ObservableCollection<ForumTopicLightEntry> PinnedTopics { get; } = new ObservableCollection<ForumTopicLightEntry>();

        public ForumTopicLightEntry SelectedForumTopicLightEntry
        {
            get { return null; }
            set
            {
                if(value != null)
                GotoPinnedTopic(value);
            }
        }

        private ICommand _removePinnedBoardCommand;

        public ICommand RemovePinnedBoardCommand
            =>
                _removePinnedBoardCommand ??
                (_removePinnedBoardCommand = new RelayCommand<ForumBoards>(RemoveFavouriteBoard));

        private ICommand _gotoPinnedBoardCommand;

        public ICommand GotoPinnedBoardCommand
            =>
                _gotoPinnedBoardCommand ??
                (_gotoPinnedBoardCommand = new RelayCommand<ForumBoards>(GotoFavouriteBoard));

        private ICommand _gotoPinnedTopicCommand;

        public ICommand GotoPinnedTopicCommand
            =>
                _gotoPinnedTopicCommand ??
                (_gotoPinnedTopicCommand = new RelayCommand<ForumTopicLightEntry>(GotoPinnedTopic));

        private ICommand _unpinTopicCommand;

        public ICommand UnpinTopicCommand
            =>
                _unpinTopicCommand ??
                (_unpinTopicCommand = new RelayCommand<ForumTopicLightEntry>(topic => PinnedTopics.Remove(topic)));

        private ICommand _navigateRecentTopicsCommand;

        public ICommand NavigateRecentTopicsCommand
            =>
                _navigateRecentTopicsCommand ??
                (_navigateRecentTopicsCommand = new RelayCommand(GotoMyRecentTopics));

        private ICommand _navigateMalClientTopicsCommand;

        public ICommand NavigateMalClientTopicsCommand
            =>
                _navigateMalClientTopicsCommand ??
                (_navigateMalClientTopicsCommand = new RelayCommand(GotoMalClientTopic));

        private ICommand _navigateWatchedTopicsCommand;

        public ICommand NavigateWatchedTopicsCommand
            =>
                _navigateWatchedTopicsCommand ??
                (_navigateWatchedTopicsCommand = new RelayCommand(GotoWatchedTopics));

        private ICommand _navigateStarredMessages;

        public ICommand NavigateStarredMessages
            =>
                _navigateStarredMessages ??
                (_navigateStarredMessages = new RelayCommand(GotoStarredMessages));

        public ISelfBackNavAware CurrentBackNavRegistrar { get; set; }

        public void Init(ForumsNavigationArgs args)
        {
            if (args == null)
            {
                ViewModelLocator.NavMgr.ResetMainBackNav();
                ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageAnimeList, null);
                args = new ForumsNavigationArgs { Page = ForumsPageIndex.PageIndex };
            }
            CurrentBackNavRegistrar = null;
            NavigationRequested?.Invoke((int)args.Page, args);
        }

        private bool _pinnedTopicsLoaded;

        public async void LoadPinnedTopics()
        {
            if(_pinnedTopicsLoaded)
                return;
            _pinnedTopicsLoaded = true;
            foreach (var item in (await ResourceLocator.DataCacheService.RetrieveDataRoaming<List<ForumTopicLightEntry>>("pinned_forum_topics.json", -1)) ?? new List<ForumTopicLightEntry>())
            {
                PinnedTopics.Add(item);
            }          
        }
         
        public async Task SavePinnedTopics()
        {
            await ResourceLocator.DataCacheService.SaveDataRoaming(PinnedTopics.ToList(), "pinned_forum_topics.json");     
        }

        public ForumsMainViewModel()
        {
            if (!string.IsNullOrEmpty(Settings.ForumsPinnedBoards))
            {
                foreach (var item in Settings.ForumsPinnedBoards.Split(',').Select(int.Parse).Cast<ForumBoards>())
                {
                    PinnedBoards.Add(item);
                }
            }
        }


        public void AddFavouriteBoard(ForumBoards board)
        {
            if (!PinnedBoards.Contains(board))
            {
                PinnedBoards.Add(board);
                Settings.ForumsPinnedBoards = string.Join(",", PinnedBoards.Cast<int>());              
            }
        }

        private void RemoveFavouriteBoard(ForumBoards board)
        {
            PinnedBoards.Remove(board);
            Settings.ForumsPinnedBoards = string.Join(",", PinnedBoards.Cast<int>());              
        }

        private void GotoFavouriteBoard(ForumBoards board)
        {
            if (CurrentBackNavRegistrar == null)
            {
                ViewModelLocator.NavMgr.ResetMainBackNav();
                ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageForumIndex, new ForumsNavigationArgs());
            }
            else
            {
                CurrentBackNavRegistrar.RegisterSelfBackNav();
            }

            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageForumIndex,new ForumsBoardNavigationArgs(board));
        }

        private void GotoPinnedTopic(ForumTopicLightEntry topic)
        {
            if (CurrentBackNavRegistrar == null)
            {
                ViewModelLocator.NavMgr.ResetMainBackNav();
                ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageForumIndex, new ForumsNavigationArgs());
                if(topic.SourceBoard != null)
                    ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageForumIndex, new ForumsBoardNavigationArgs(topic.SourceBoard.Value));
            }
            else
            {
                CurrentBackNavRegistrar.RegisterSelfBackNav();
            }

            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageForumIndex, new ForumsTopicNavigationArgs(topic.Id,topic.Lastpost ? (int?)-1 : null,1));
        }

        private void GotoMyRecentTopics()
        {
            if (CurrentBackNavRegistrar == null)
            {
                ViewModelLocator.NavMgr.ResetMainBackNav();
                ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageForumIndex, new ForumsNavigationArgs());
            }
            else
            {
                CurrentBackNavRegistrar.RegisterSelfBackNav();
            }

            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageForumIndex, new ForumsBoardNavigationArgs(Credentials.UserName));
        }

        private void GotoWatchedTopics()
        {
            if (CurrentBackNavRegistrar == null)
            {
                ViewModelLocator.NavMgr.ResetMainBackNav();
                ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageForumIndex, new ForumsNavigationArgs());
            }
            else
            {
                CurrentBackNavRegistrar.RegisterSelfBackNav();
            }
            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageForumIndex, new ForumsBoardNavigationArgs(ForumBoardPageWorkModes.WatchedTopics));
        }

        private void GotoMalClientTopic()
        {
            if (CurrentBackNavRegistrar == null)
            {
                ViewModelLocator.NavMgr.ResetMainBackNav();
                ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageForumIndex, new ForumsNavigationArgs());
                ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageForumIndex, new ForumsBoardNavigationArgs(ForumBoardPageWorkModes.WatchedTopics));
            }
            else
            {
                CurrentBackNavRegistrar.RegisterSelfBackNav();
            }          
            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageForumIndex, new ForumsTopicNavigationArgs("1499207",null));
        }

        private void GotoStarredMessages()
        {
            if (CurrentBackNavRegistrar == null)
            {
                ViewModelLocator.NavMgr.ResetMainBackNav();
                ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageForumIndex, new ForumsNavigationArgs());
            }
            else
            {
                CurrentBackNavRegistrar.RegisterSelfBackNav();
            }
            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageForumIndex, new ForumStarredMessagesNavigationArgs());
        }
    }
}
