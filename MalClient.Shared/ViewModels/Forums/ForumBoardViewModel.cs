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

        private ICommand _gotoLastPostCommand;

        public ICommand GotoLastPostCommand => _gotoLastPostCommand ?? (_gotoLastPostCommand = new RelayCommand<ForumTopicEntryViewModel>(
            topic =>
            {
                LoadTopic(topic,true);
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

        public int CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                RaisePropertyChanged(() => CurrentPage);
                
                AvailablePages.Clear();
                var start = value <= 2 ? 1 : value-2;
                for (int i = start; i <= start+4; i++)
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


        public  void Init(ForumsBoardNavigationArgs args)
        {
            if (PrevArgs != null && Topics.Count != 0 &&
                ((args.IsAnimeBoard != null && PrevArgs.AnimeId == args.AnimeId) || (args.IsAnimeBoard == null && PrevArgs.TargetBoard == args.TargetBoard)))
                return;
            PrevArgs = args;
            LoadPage(args.PageNumber);
            Topics?.Clear();
            Title = args.IsAnimeBoard != null ? args.AnimeTitle : args.TargetBoard.GetDescription();
            if (args.IsAnimeBoard != null)
                Icon = args.IsAnimeBoard.Value ? FontAwesomeIcon.Tv : FontAwesomeIcon.Book;
            else
                Icon = Utilities.BoardToIcon(args.TargetBoard);          

            if( args.TargetBoard == ForumBoards.NewsDisc||
                args.TargetBoard == ForumBoards.AnimeDisc||
                args.TargetBoard == ForumBoards.MangaDisc||
                args.TargetBoard == ForumBoards.Updates||
                args.TargetBoard == ForumBoards.Guidelines)
                NewTopicButtonVisibility = Visibility.Collapsed;
            else
                NewTopicButtonVisibility = Visibility.Visible;
        }

        public async void LoadPage(int page,bool decrement = false)
        {
            LoadingTopics = Visibility.Visible;
            if (decrement)
                page--;        
            try
            {
                Topics =
                    new ObservableCollection<ForumTopicEntryViewModel>(
                        (PrevArgs.IsAnimeBoard == null
                            ? await new ForumBoardTopicsQuery(PrevArgs.TargetBoard, page).GetTopicPosts()
                            : await new ForumBoardTopicsQuery(PrevArgs.AnimeId, page, PrevArgs.IsAnimeBoard.Value).GetTopicPosts()).Select(
                                entry => new ForumTopicEntryViewModel(entry)));
                CurrentPage = page;
            }
            catch (Exception)
            {
                //no shuch page
            }
            LoadingTopics = Visibility.Collapsed;
        }

        public void LoadTopic(ForumTopicEntryViewModel topic,bool lastpost = false)
        {
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageForumIndex, PrevArgs);
            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageForumIndex,
                new ForumsTopicNavigationArgs(topic.Data.Id, PrevArgs.TargetBoard,lastpost));
        }
    }
}
