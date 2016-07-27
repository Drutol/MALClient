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
using MalClient.Shared.NavArgs;
using MalClient.Shared.Utils;
using MalClient.Shared.Utils.Enums;

namespace MalClient.Shared.ViewModels.Forums
{
    public class ForumsMainViewModel : ViewModelBase
    {
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



        public SmartObservableCollection<ForumBoards> PinnedBoards { get; } = new SmartObservableCollection<ForumBoards>();

        public event AmbiguousNavigationRequest NavigationRequested;

        public void Init(ForumsNavigationArgs args)
        {
            args = args ?? new ForumsNavigationArgs {Page = ForumsPageIndex.PageIndex};
            NavigationRequested?.Invoke((int)args.Page, args);
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
    }
}
