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

        private ICommand _navigateWatchedTopicsCommand;

        public ICommand NavigateWatchedTopicsCommand
            =>
                _navigateWatchedTopicsCommand ??
                (_navigateWatchedTopicsCommand = new RelayCommand(GotoWatchedTopics));

        public void Init(ForumsNavigationArgs args)
        {
            if (args == null)
            {
                ViewModelLocator.NavMgr.ResetMainBackNav();
                ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageAnimeList, null);
                args = new ForumsNavigationArgs { Page = ForumsPageIndex.PageIndex };
            }   
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
            ViewModelLocator.NavMgr.ResetMainBackNav();
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageForumIndex, new ForumsNavigationArgs());
            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageForumIndex,new ForumsBoardNavigationArgs(board));
        }

        private void GotoPinnedTopic(ForumTopicLightEntry topic)
        {
            ViewModelLocator.NavMgr.ResetMainBackNav();
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageForumIndex, new ForumsNavigationArgs());
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageForumIndex, new ForumsBoardNavigationArgs(topic.SourceBoard));
            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageForumIndex, new ForumsTopicNavigationArgs(topic.SourceBoard,topic.Id,topic.Lastpost ? (int?)-1 : null,1));
        }

        private void GotoMyRecentTopics()
        {
            ViewModelLocator.NavMgr.ResetMainBackNav();
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageForumIndex, new ForumsNavigationArgs());
            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageForumIndex, new ForumsBoardNavigationArgs(ForumBoardPageWorkModes.UserSearch));
        }

        private void GotoWatchedTopics()
        {
            ViewModelLocator.NavMgr.ResetMainBackNav();
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageForumIndex, new ForumsNavigationArgs());
            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageForumIndex, new ForumsBoardNavigationArgs(ForumBoardPageWorkModes.WatchedTopics));
        }
    }
}
