using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using MalClient.Shared.Comm.Forums;
using MalClient.Shared.Models.Forums;
using MalClient.Shared.NavArgs;
using MalClient.Shared.Utils;
using MalClient.Shared.Utils.Enums;

namespace MalClient.Shared.ViewModels.Forums
{
    public class ForumBoardViewModel : ViewModelBase
    {
        private ForumBoards? _currentBoard;

        public ObservableCollection<ForumTopicEntryViewModel> _topics;

        public ObservableCollection<ForumTopicEntryViewModel> Topics
        {
            get { return _topics; }
            set
            {
                _topics = value;
                RaisePropertyChanged(() => Topics);
            }
        }

        public async void Init(ForumsBoardNavigationArgs args)
        {
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageForumIndex, new ForumsNavigationArgs());
            if (_currentBoard != null && _currentBoard == args.TargetBoard)
                return;
            Topics =
                new ObservableCollection<ForumTopicEntryViewModel>(
                    (await new ForumBoardTopicsQuery(args.TargetBoard,0).GetTopicPosts()).Select(
                        entry => new ForumTopicEntryViewModel(entry)));
            Title = args.TargetBoard.GetDescription();
            _currentBoard = args.TargetBoard;
        }

        private string _title;

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged(() => Title);
            }
        }
    }
}
