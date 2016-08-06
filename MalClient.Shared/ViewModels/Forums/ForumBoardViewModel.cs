using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using FontAwesome.UWP;
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
        private ForumsBoardNavigationArgs _prevArgs;

        private ObservableCollection<ForumTopicEntryViewModel> _topics;

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
            if (_prevArgs != null &&
                ((args.IsAnimeBoard && _prevArgs.AnimeId == args.AnimeId) || _prevArgs.TargetBoard == args.TargetBoard))
                return;
            LoadingTopics = Visibility.Visible;
            Topics?.Clear();
            Title = args.TargetBoard.GetDescription();
            Icon = Utilities.BoardToIcon(args.TargetBoard);
            _prevArgs = args;
            Topics =
                new ObservableCollection<ForumTopicEntryViewModel>(
                    (!args.IsAnimeBoard
                        ? await new ForumBoardTopicsQuery(args.TargetBoard, 0).GetTopicPosts()
                        : await new ForumBoardTopicsQuery(args.AnimeId, 0).GetTopicPosts()).Select(
                            entry => new ForumTopicEntryViewModel(entry)));
            LoadingTopics = Visibility.Collapsed;
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

        private FontAwesomeIcon _icon;

        public FontAwesomeIcon Icon
        {
            get { return _icon; }
            set
            {
                _icon = value;
                RaisePropertyChanged(() => Icon);
            }
        }

        public Visibility _loadingTopics;
        public Visibility LoadingTopics
        {
            get { return _loadingTopics; }
            set
            {
                _loadingTopics = value;
                RaisePropertyChanged(() => LoadingTopics);
            }
        }
    }
}
