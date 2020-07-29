using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Models.Enums;
using MALClient.Models.Models.Forums;
using MALClient.XShared.Comm.Forums;
using MALClient.XShared.Interfaces;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels.Forums.Items;

namespace MALClient.XShared.ViewModels.Forums
{
    public class ForumBoardViewModel : ViewModelBase , ISelfBackNavAware
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
#if ANDROID
        private List<Tuple<int, bool>> _availablePages;
        public List<Tuple<int, bool>> AvailablePages
        {
            get { return _availablePages; }
            set
            {
                _availablePages = value; 
                RaisePropertyChanged();
            }
        }

#else
            public ObservableCollection<Tuple<int,bool>> AvailablePages { get; } = new ObservableCollection<Tuple<int, bool>>();
#endif


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

        private bool _loadingTopics;

        public bool LoadingTopics
        {
            get { return _loadingTopics; }
            set
            {
                _loadingTopics = value;
                RaisePropertyChanged(() => LoadingTopics);
            }
        }

        private bool _emptyNoticeVisibility = false;

        public bool EmptyNoticeVisibility
        {
            get { return _emptyNoticeVisibility; }
            set
            {
                _emptyNoticeVisibility = value;
                RaisePropertyChanged(() => EmptyNoticeVisibility);
            }
        }

        private bool _newTopicButtonVisibility;

        public bool NewTopicButtonVisibility
        {
            get { return _newTopicButtonVisibility; }
            set
            {
                _newTopicButtonVisibility = value;
                RaisePropertyChanged(() => NewTopicButtonVisibility);
            }
        }

        private bool _searchButtonVisibility;

        public bool SearchButtonVisibility
        {
            get { return _searchButtonVisibility; }
            set
            {
                _searchButtonVisibility = value;
                RaisePropertyChanged(() => SearchButtonVisibility);
            }
        }

        private bool _pageNavigationControlsVisibility;

        public bool PageNavigationControlsVisibility
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
                ResourceLocator.MessageDialogProvider.ShowMessageDialog("Sorry, but MAL has introduced recaptchas which limit what I can do on the forums." +
                                                                        " Forums will be back once their API is well enough featured and stable.", "Sorry.");
                return;

                ForumsNewTopicNavigationArgs arg;
                switch (PrevArgs.WorkMode)
                {
                    case ForumBoardPageWorkModes.AnimeBoard:
                        arg = new ForumsNewTopicNavigationArgs(TopicType.Anime, PrevArgs.AnimeTitle, PrevArgs.AnimeId);
                        break;
                    case ForumBoardPageWorkModes.MangaBoard:
                        arg = new ForumsNewTopicNavigationArgs(TopicType.Manga, PrevArgs.AnimeTitle, PrevArgs.AnimeId);
                        break;
                    case ForumBoardPageWorkModes.Club:
                        arg = new ForumsNewTopicNavigationArgs(int.Parse(PrevArgs.ClubId),PrevArgs.ClubName);
                        break;
                    default:
                        arg = new ForumsNewTopicNavigationArgs(PrevArgs.TargetBoard);
                        break;
                }


                ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageForumIndex, PrevArgs);
               ViewModelLocator.GeneralMain.Navigate(PageIndex.PageForumIndex,arg); 
            }));

        private ICommand _searchCommand;

        public ICommand SearchCommand => _searchCommand ?? (_searchCommand = new RelayCommand(
            () =>
            {
                if (SearchQuery?.Length <= 2)
                    return;
                ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageForumIndex, PrevArgs);
                ViewModelLocator.GeneralMain.Navigate(PageIndex.PageForumIndex,
                    new ForumsBoardNavigationArgs(SearchQuery, PrevArgs.TargetBoard));
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
                LoadPage(_allPages);
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
            if (!int.TryParse(GotoPageTextBind, out int val))
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

#if ANDROID
                var list = new List<Tuple<int, bool>>();
                var start = value <= 2 ? 1 : value - 2;
                for (int i = start; i <= start + 4 && i <= _allPages + 1; i++)
                    list.Add(new Tuple<int, bool>(i, i == value + 1));
                AvailablePages = list;
#else
                AvailablePages.Clear();
                var start = value <= 2 ? 1 : value - 2;
                for (int i = start; i <= start + 4 && i <= _allPages + 1; i++)
                    AvailablePages.Add(new Tuple<int, bool>(i, i == value + 1));
#endif

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
        private bool _forceReloadOnNextInit;



        public void Init(ForumsBoardNavigationArgs args,bool force = false)
        {
            ViewModelLocator.ForumsMain.CurrentBackNavRegistrar = this;

            if (_forceReloadOnNextInit)//navigating from crete new topic
            {
                force = true; 
                _forceReloadOnNextInit = false;
            }


            if (!force)
            {
                if (_initizalizing)
                    return;
                if (args.Equals(PrevArgs))
                    return;
            }
            _initizalizing = true;
            PrevArgs = args;

            LoadPage(args.PageNumber,false,force);

            switch (args.WorkMode)
            {
                case ForumBoardPageWorkModes.Standard:
                        PageNavigationControlsVisibility = SearchButtonVisibility = true;
                        Title = args.TargetBoard.GetDescription();
                        Icon = Utilities.BoardToIcon(args.TargetBoard);
                    break;
                case ForumBoardPageWorkModes.AnimeBoard:
                    SearchButtonVisibility = false;
                    PageNavigationControlsVisibility = true;
                    Title = args.AnimeTitle;
                    Icon = FontAwesomeIcon.Tv;
                    break;
                case ForumBoardPageWorkModes.MangaBoard:
                    SearchButtonVisibility = false;
                    PageNavigationControlsVisibility = true;
                    Title = args.AnimeTitle;
                    Icon = FontAwesomeIcon.Book;
                    break;
                case ForumBoardPageWorkModes.Search:
                    PageNavigationControlsVisibility = SearchButtonVisibility = false;
                    Title = $"Search - {args.Query}";
                    Icon = args.Scope == null ? FontAwesomeIcon.Search : Utils.Utilities.BoardToIcon(args.Scope.Value);
                    break;
                case ForumBoardPageWorkModes.UserSearch:
                    Title = args.Query == Credentials.UserName ? "My Recent Posts" : $"{args.Query}'s Recent Posts";
                    PageNavigationControlsVisibility = true;
                    SearchButtonVisibility = false;
                    Icon = FontAwesomeIcon.ClockOutline;
                    break;
                case ForumBoardPageWorkModes.WatchedTopics:
                    Icon = FontAwesomeIcon.Eye;
                    Title = "Watched Topics";
                    PageNavigationControlsVisibility = true;
                    SearchButtonVisibility = false;
                    break;
                case ForumBoardPageWorkModes.Club:
                    PageNavigationControlsVisibility = SearchButtonVisibility = true;
                    Title = $"{args.ClubName}'s Forum";
                    Icon = FontAwesomeIcon.Group;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (args.WorkMode == ForumBoardPageWorkModes.Search || args.WorkMode == ForumBoardPageWorkModes.UserSearch ||
                args.WorkMode == ForumBoardPageWorkModes.WatchedTopics || args.TargetBoard == ForumBoards.NewsDisc ||
                args.TargetBoard == ForumBoards.AnimeSeriesDisc || args.TargetBoard == ForumBoards.MangaSeriesDisc ||
                args.TargetBoard == ForumBoards.Updates || args.TargetBoard == ForumBoards.Guidelines)
                NewTopicButtonVisibility = false;
            else
                NewTopicButtonVisibility = true;
        }

        public async void LoadPage(int page, bool decrement = false,bool force = false)
        {
            LoadingTopics = true;
            if (decrement)
                page--;
            try
            {
                Topics = new List<ForumTopicEntryViewModel>();
                ForumBoardContent topics;
                int? arg;
                if (page != 0)
                    arg = _allPages;
                else
                    arg = null;
                switch (PrevArgs.WorkMode)
                {
                    case ForumBoardPageWorkModes.Standard:
                        topics = await new ForumBoardTopicsQuery(PrevArgs.TargetBoard, page).GetTopicPosts(arg,force);
                        break;
                    case ForumBoardPageWorkModes.AnimeBoard:
                    case ForumBoardPageWorkModes.MangaBoard:
                        topics = await
                            new ForumBoardTopicsQuery(PrevArgs.AnimeId, page, PrevArgs.IsAnimeBoard).GetTopicPosts(arg,force);
                        break;
                    case ForumBoardPageWorkModes.Search:
                        topics = await
                            ForumSearchQuery.GetSearchResults(PrevArgs.Query, PrevArgs.Scope);
                        break;
                    case ForumBoardPageWorkModes.UserSearch:
                        topics = await
                            ForumSearchQuery.GetRecentTopics(PrevArgs.Query);
                        break;
                    case ForumBoardPageWorkModes.WatchedTopics:
                        topics = await
                             ForumSearchQuery.GetWatchedTopics();
                        break;
                    case ForumBoardPageWorkModes.Club:
                        topics = await new ForumBoardTopicsQuery(PrevArgs.ClubId, page).GetTopicPosts(arg, force);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                if (topics.ForumTopicEntries.Count != 0)
                {
                    Topics = topics.ForumTopicEntries.Select(entry => new ForumTopicEntryViewModel(entry)).ToList();
                    _allPages = topics.Pages;
                    CurrentPage = page;
                }
                EmptyNoticeVisibility = !Topics.Any();
            }
            catch (Exception)
            {
                //no shuch page
            }
            _initizalizing = false;
            LoadingTopics = false;
        }

        public void LoadTopic(ForumTopicEntryViewModel topic, bool lastpost = false)
        {
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageForumIndex, PrevArgs);
            if (PrevArgs.WorkMode == ForumBoardPageWorkModes.UserSearch ||
                PrevArgs.WorkMode == ForumBoardPageWorkModes.WatchedTopics)
                ViewModelLocator.GeneralMain.Navigate(PageIndex.PageForumIndex,
                    new ForumsTopicNavigationArgs( topic.Data.Id, lastpost ? (int?)-1 : null));
            else
                ViewModelLocator.GeneralMain.Navigate(PageIndex.PageForumIndex,
                    new ForumsTopicNavigationArgs(topic.Data.Id, lastpost ? (int?)-1 : null));
        }

        public void Reload()
        {
            Init(PrevArgs,true);
        }

        public void ReloadOnNextLoad()
        {
            _forceReloadOnNextInit = true;
        }

        public void RegisterSelfBackNav()
        {
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageForumIndex,PrevArgs);
        }
    }
}
