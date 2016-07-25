using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using MalClient.Shared.Delegates;
using MalClient.Shared.NavArgs;
using MalClient.Shared.Utils;
using MalClient.Shared.Utils.Enums;

namespace MalClient.Shared.ViewModels.Forums
{
    public class ForumsMainViewModel : ViewModelBase
    {
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
    }
}
