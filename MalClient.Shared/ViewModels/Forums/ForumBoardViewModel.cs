using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using FontAwesome.UWP;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MalClient.Shared.Comm.Forums;
using MalClient.Shared.Models.Forums;
using MalClient.Shared.NavArgs;
using MalClient.Shared.Utils;
using MalClient.Shared.Utils.Enums;

namespace MalClient.Shared.ViewModels.Forums
{
    public class ForumBoardViewModel : ViewModelBase
    {
        public ForumsBoardNavigationArgs PrevArgs;

        private List<ForumTopicEntryViewModel> _topics;

        public List<ForumTopicEntryViewModel> Topics
        {
            get { return _topics; }
            set
            {
                _topics = value;
                RaisePropertyChanged(() => Topics);
            }
        }

        //page and IsActive
        public ObservableCollection<Tuple<int,bool>> AvailablePages { get; } = new ObservableCollection<Tuple<int, bool>>();

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

        private string _searchQuery;

        public string SearchQuery
        {
            get { return _searchQuery; }
            set
            {
                _searchQuery = value;
                RaisePropertyChanged(() => SearchQuery);
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

        private Visibility _loadingTopics;

        public Visibility LoadingTopics
        {
            get { return _loadingTopics; }
            set
            {
                _loadingTopics = value;
                RaisePropertyChanged(() => LoadingTopics);
            }
        }

        private Visibility _emptyNoticeVisibility = Visibility.Collapsed;

        public Visibility EmptyNoticeVisibility
        {
            get { return _emptyNoticeVisibility; }
            set
            {
                _emptyNoticeVisibility = value;
                RaisePropertyChanged(() => EmptyNoticeVisibility);
            }
        }

        private Visibility _newTopicButtonVisibility;

        public Visibility NewTopicButtonVisibility
        {
            get { return _newTopicButtonVisibility; }
            set
            {
                _newTopicButtonVisibility = value;
                RaisePropertyChanged(() => NewTopicButtonVisibility);
            }
        }

        private Visibility _searchButtonVisibility;

        public Visibility SearchButtonVisibility
        {
            get { return _searchButtonVisibility; }
            set
            {
                _searchButtonVisibility = value;
                RaisePropertyChanged(() => SearchButtonVisibility);
            }
        }

        private Visibility _pageNavigationControlsVisibility;

        public Visibility PageNavigationControlsVisibility
        {
            get { return _pageNavigationControlsVisibility; }
            set
            {
                _pageNavigationControlsVisibility = value;
                RaisePropertyChanged(() => PageNavigationControlsVisibility);
            }
        }

        private ICommand _loadPageCommand;

        public ICommand LoadPageCommand => _loadPageCommand ?? (_loadPageCommand = new RelayCommand<int>(i => LoadPage(i,true)));

        private ICommand _createNewTopicCommand;

        public ICommand CreateNewTopicCommand => _createNewTopicCommand ?? (_createNewTopicCommand = new RelayCommand(
            () =>
            {
               var arg = ForumsTopicNavigationArgs.NewTopic;
               arg.SourceBoard = PrevArgs.TargetBoard;
               ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageForumIndex, PrevArgs);
               ViewModelLocator.GeneralMain.Navigate(PageIndex.PageForumIndex,arg); 
            }));

        private ICommand _searchCommand;

        public ICommand SearchCommand => _searchCommand ?? (_searchCommand = new RelayCommand(
            () =>
            {
                if (SearchQuery.Length <= 2)
                    return;
                ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageForumIndex, PrevArgs);
                ViewModelLocator.GeneralMain.Navigate(PageIndex.PageForumIndex,
                    new ForumsBoardNavigationArgs(SearchQuery, PrevArgs.Scope));
                SearchQuery = "";
            }));

        private ICommand _gotoLastPostCommand;

        public ICommand GotoLastPostCommand => _gotoLastPostCommand ?? (_gotoLastPostCommand = new RelayCommand<ForumTopicEntryViewModel>(
            topic =>
            {
                LoadTopic(topic,true);
            }));

        private ICommand _gotoLastPageCommand;

        public ICommand GotoLastPageCommand => _gotoLastPageCommand ?? (_gotoLastPageCommand = new RelayCommand(
            () =>
            {
                LoadPage(_allPages,false,true);
            }));

        private ICommand _gotoFirstPageCommand;

        public ICommand GotoFirstPageCommand => _gotoFirstPageCommand ?? (_gotoFirstPageCommand = new RelayCommand(
            () =>
            {
                LoadPage(0);
            }));

        private ICommand _loadGotoPageCommand;

        public ICommand LoadGotoPageCommand => _loadGotoPageCommand ?? (_loadGotoPageCommand = new RelayCommand(() =>
        {
            int val;
            if (!int.TryParse(GotoPageTextBind, out val))            
                return;
            LoadPage(val);
            GotoPageTextBind = "";
        }));

        private int _currentPage;
        private string _gotoPageTextBind;
        private int _allPages;

        public int CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                RaisePropertyChanged(() => CurrentPage);
                
                AvailablePages.Clear();
                var start = value <= 2 ? 1 : value-2;
                for (int i = start; i <= start+4 && i <= _allPages + 1; i++)
                    AvailablePages.Add(new Tuple<int, bool>(i,i == value+1));
            }
        }

        public string GotoPageTextBind
        {
            get { return _gotoPageTextBind; }
            set
            {
                _gotoPageTextBind = value; 
                RaisePropertyChanged(() => GotoPageTextBind);
            }
        }

        private bool _initizalizing;


        public void Init(ForumsBoardNavigationArgs args)
        {
            if (_initizalizing)
                return;
            if (args.Equals(PrevArgs))
                return;
            _initizalizing = true;
            PrevArgs = args;

            LoadPage(args.PageNumber);

            switch (args.WorkMode)
            {
                case ForumBoardPageWorkModes.Standard:
                    PageNavigationControlsVisibility = SearchButtonVisibility = Visibility.Visible;
                    Title = args.TargetBoard.GetDescription();
                    Icon = Utilities.BoardToIcon(args.TargetBoard);
                    break;
                case ForumBoardPageWorkModes.AnimeBoard:
                    SearchButtonVisibility = Visibility.Collapsed;
                    PageNavigationControlsVisibility = Visibility.Visible;
                    Title = args.AnimeTitle;
                    Icon = FontAwesomeIcon.Tv;
                    break;
                case ForumBoardPageWorkModes.MangaBoard:
                    SearchButtonVisibility = Visibility.Collapsed;
                    PageNavigationControlsVisibility = Visibility.Visible;
                    Title = args.AnimeTitle;
                    Icon = FontAwesomeIcon.Book;
                    break;
                case ForumBoardPageWorkModes.Search:
                    PageNavigationControlsVisibility = SearchButtonVisibility = Visibility.Collapsed;
                    Title = "Search - " + args.Query;
                    Icon = args.Scope == null ? FontAwesomeIcon.Search : Utilities.BoardToIcon(args.Scope.Value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (args.WorkMode == ForumBoardPageWorkModes.Search || args.TargetBoard == ForumBoards.NewsDisc ||
                args.TargetBoard == ForumBoards.AnimeSeriesDisc || args.TargetBoard == ForumBoards.MangaSeriesDisc ||
                args.TargetBoard == ForumBoards.Updates || args.TargetBoard == ForumBoards.Guidelines)
                NewTopicButtonVisibility = Visibility.Collapsed;
            else
                NewTopicButtonVisibility = Visibility.Visible;
        }

        public async void LoadPage(int page, bool decrement = false,bool lastPage = false)
        {
            LoadingTopics = Visibility.Visible;
            if (decrement)
                page--;
            try
            {
                var prevTopics = Topics;
                Topics = new List<ForumTopicEntryViewModel>();
                ForumBoardContent topics;
                int? arg;
                if (lastPage)
                    arg = _allPages;
                else
                    arg = null;
                switch (PrevArgs.WorkMode)
                {
                    case ForumBoardPageWorkModes.Standard:
                        topics = await new ForumBoardTopicsQuery(PrevArgs.TargetBoard, page).GetTopicPosts(arg);
                        break;
                    case ForumBoardPageWorkModes.AnimeBoard:
                    case ForumBoardPageWorkModes.MangaBoard:
                        topics = await
                            new ForumBoardTopicsQuery(PrevArgs.AnimeId, page, PrevArgs.IsAnimeBoard).GetTopicPosts(arg);
                        break;
                    case ForumBoardPageWorkModes.Search:
                        topics = await
                            ForumSearchQuery.GetSearchResults(PrevArgs.Query, PrevArgs.Scope);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                if (topics.ForumTopicEntries.Count == 0)
                    Topics = prevTopics;
                else
                {
                    Topics = topics.ForumTopicEntries.Select(entry => new ForumTopicEntryViewModel(entry)).ToList();
                    _allPages = topics.Pages;
                    CurrentPage = page;
                }
                EmptyNoticeVisibility = Topics.Any() ? Visibility.Collapsed : Visibility.Visible;
            }
            catch (Exception)
            {
                //no shuch page
            }
            _initizalizing = false;
            LoadingTopics = Visibility.Collapsed;
        }

        public void LoadTopic(ForumTopicEntryViewModel topic, bool lastpost = false)
        {
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageForumIndex, PrevArgs);
            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageForumIndex, new ForumsTopicNavigationArgs(topic.Data.Id, PrevArgs.TargetBoard, lastpost));
        }
    }
}
