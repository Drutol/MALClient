using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MalClient.Shared.Delegates;
using MalClient.Shared.Models.Forums;
using MalClient.Shared.NavArgs;
using MalClient.Shared.Utils;
using MalClient.Shared.Utils.Enums;

namespace MalClient.Shared.ViewModels.Forums
{
    public class ForumsMainViewModel : ViewModelBase
    {
        public event AmbiguousNavigationRequest NavigationRequested;

        public SmartObservableCollection<ForumBoards> PinnedBoards { get; } = new SmartObservableCollection<ForumBoards>();

        public SmartObservableCollection<ForumTopicLightEntry> PinnedTopics { get; } = new SmartObservableCollection<ForumTopicLightEntry>();

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




        public void Init(ForumsNavigationArgs args)
        {
            args = args ?? new ForumsNavigationArgs {Page = ForumsPageIndex.PageIndex};
            NavigationRequested?.Invoke((int)args.Page, args);
        }

        public async void LoadPinnedTopics()
        {
            PinnedTopics.AddRange((await DataCache.RetrieveData<List<ForumTopicLightEntry>>("pinned_forum_topics.json","",-1)) ?? new List<ForumTopicLightEntry>());
        }
         
        public async Task SavePinnedTopics()
        {
            await DataCache.SaveData(PinnedTopics.ToList(), "pinned_forum_topics.json", "");     
        }

        public ForumsMainViewModel()
        {
            if(!string.IsNullOrEmpty(Settings.ForumsPinnedBoards))
                PinnedBoards.AddRange(Settings.ForumsPinnedBoards.Split(',').Select(int.Parse).Cast<ForumBoards>());
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
            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageForumIndex,new ForumsBoardNavigationArgs(board));
        }

        private void GotoPinnedTopic(ForumTopicLightEntry topic)
        {
            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageForumIndex, new ForumsTopicNavigationArgs(topic.Id,topic.SourceBoard,topic.Lastpost));
        }
    }
}
