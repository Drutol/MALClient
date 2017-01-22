using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Models.Enums;
using MALClient.Models.Models;
using MALClient.Models.Models.Forums;
using MALClient.XShared.Comm.MagicalRawQueries.Forums;
using MALClient.XShared.Delegates;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.ViewModels.Forums.Items;

namespace MALClient.XShared.ViewModels.Forums
{
    public class ForumTopicViewModel : ViewModelBase
    {
        public interface IScrollInfoProvider
        {
            int GetFirstVisibleItemIndex();
        }

        public event EventHandler<int> RequestScroll; 

        private bool _loadingTopic;
        

        private ForumsTopicNavigationArgs _prevArgs;
        private int _currentPage;
        private ObservableCollection<ForumTopicMessageEntryViewModel> _messages;
        private ICommand _loadPageCommand;
        private ICommand _loadGotoPageCommand;
        private ICommand _gotoLastPageCommand;
        private ICommand _gotoFirstPageCommand;
        private string _gotoPageTextBind;
        private string _title;
        private ForumTopicData _currentTopicData;
        private ICommand _toggleWatchingCommand;
        private string _toggleWatchingButtonText;
        private ICommand _createReplyCommand;
        private string _replyMessage;
        private ICommand _navigateMessagingCommand;

        public IScrollInfoProvider ScrollInfoProvider { get; set; }

        public async void Init(ForumsTopicNavigationArgs args)
        {
            if (LoadingTopic)
                return;

            LoadingTopic = true;
            _prevArgs = args;
            Messages?.Clear();
            ToggleWatchingButtonText = "Toggle watching";
            CurrentTopicData = await ForumTopicQueries.GetTopicData(_prevArgs.TopicId, _prevArgs.TopicPage, _prevArgs.LastPost);
            CurrentPage = _prevArgs.LastPost ? CurrentTopicData.AllPages : CurrentTopicData.CurrentPage;

            Messages = new ObservableCollection<ForumTopicMessageEntryViewModel>(
                CurrentTopicData.Messages.Select(
                    entry => new ForumTopicMessageEntryViewModel(entry)));

            if (_prevArgs.FirstVisibleItemIndex != null)
            {
                RequestScroll?.Invoke(this,_prevArgs.FirstVisibleItemIndex.Value);
            }
            else if (_prevArgs.LastPost)
            {
                RequestScroll?.Invoke(this,Messages.Count-1);
            }

            LoadingTopic = false;
        }

        public ObservableCollection<ForumTopicMessageEntryViewModel> Messages
        {
            get { return _messages; }
            set
            {
                _messages = value;
                RaisePropertyChanged(() => Messages);
            }
        }


        public ForumTopicData CurrentTopicData
        {
            get { return _currentTopicData; }
            set
            {
                _currentTopicData = value;
                RaisePropertyChanged(() => CurrentTopicData);
            }
        }

        public string Header => !LoadingTopic ? CurrentTopicData?.Title : null;
        public bool IsLockedVisibility => !LoadingTopic && (CurrentTopicData?.IsLocked ?? false);
        public List<ForumBreadcrumb> Breadcrumbs => !LoadingTopic ? CurrentTopicData?.Breadcrumbs  : null;

        public int CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                AvailablePages.Clear();
                var start = value <= 2 ? 1 : value - 2;
                for (int i = start; i <= start + 4 && i <= CurrentTopicData.AllPages; i++)
                    AvailablePages.Add(new Tuple<int, bool>(i, i == value));

            }
        }

        public ObservableCollection<Tuple<int, bool>> AvailablePages { get; } = new ObservableCollection<Tuple<int, bool>>();


        public bool LoadingTopic
        {
            get { return _loadingTopic; }
            set
            {
                _loadingTopic = value;
                RaisePropertyChanged(() => LoadingTopic);
                RaisePropertyChanged(() => Header);
                RaisePropertyChanged(() => Breadcrumbs);
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

        public string ToggleWatchingButtonText
        {
            get { return _toggleWatchingButtonText; }
            set
            {
                _toggleWatchingButtonText = value;
                RaisePropertyChanged(() => ToggleWatchingButtonText);
            }
        }

        public string ReplyMessage
        {
            get { return _replyMessage; }
            set
            {
                _replyMessage = value;
                RaisePropertyChanged(() => ReplyMessage);
            }
        }

        #region Commands

        public ICommand NavigateBreadcrumbsCommand => new RelayCommand<ForumBreadcrumb>(breadcrumb =>
        {
            var args = MalLinkParser.GetNavigationParametersForUrl(breadcrumb.Link);
            RegisterSelfBackNav();
            ViewModelLocator.GeneralMain.Navigate(args.Item1,args.Item2);
        });

        public ICommand NavigateProfileCommand => new RelayCommand<MalUser>(user =>
        {
            RegisterSelfBackNav();
            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageProfile,
                new ProfilePageNavigationArgs {TargetUser = user.Name});
        });

        public ICommand LoadPageCommand => _loadPageCommand ?? (_loadPageCommand = new RelayCommand<int>(page =>
        {
            CurrentPage = page;
            LoadCurrentTopicPage();
        }));

        public ICommand LoadGotoPageCommand => _loadGotoPageCommand ?? (_loadGotoPageCommand = new RelayCommand(() =>
        {
            int val;
            if (!int.TryParse(GotoPageTextBind, out val))
                return;
            CurrentPage = val;
            LoadCurrentTopicPage();
            GotoPageTextBind = "";
        }));


        public ICommand GotoLastPageCommand => _gotoLastPageCommand ?? (_gotoLastPageCommand = new RelayCommand(
            () =>
            {
                CurrentPage = CurrentTopicData.AllPages;
                LoadCurrentTopicPage();
            }));



        public ICommand GotoFirstPageCommand => _gotoFirstPageCommand ?? (_gotoFirstPageCommand = new RelayCommand(
            () =>
            {
                CurrentPage = 1;
                LoadCurrentTopicPage();
            }));

        public ICommand ToggleWatchingCommand => _toggleWatchingCommand ?? (_toggleWatchingCommand = new RelayCommand(
                                                     async () =>
                                                     {
                                                         var res =
                                                             await ForumTopicQueries.ToggleTopicWatching(
                                                                 _prevArgs.TopicId);
                                                         if(res == null)
                                                             ResourceLocator.MessageDialogProvider.ShowMessageDialog("Unable to toggle watching status.","Something went wrong");

                                                         ToggleWatchingButtonText = res == true ? "Watching" : "Stopped watching";
                                                     }));

        public ICommand CreateReplyCommand => _createReplyCommand ?? (_createReplyCommand = new RelayCommand(
                                                  async () =>
                                                  {
                                                      if(await ForumTopicQueries.CreateMessage(_prevArgs.TopicId,ReplyMessage))
                                                      {
                                                          ReplyMessage = string.Empty;
                                                          LoadCurrentTopicPage(true,true);
                                                      }
                                                      else
                                                      {
                                                          ResourceLocator.MessageDialogProvider.ShowMessageDialog("Unable to send your reply","Something went wrong");
                                                      }

                                                  }));

        public ICommand NavigateMessagingCommand
            => _navigateMessagingCommand ?? (_navigateMessagingCommand = new RelayCommand<MalUser>(
                   user =>
                   {
                       ViewModelLocator.GeneralMain.Navigate(PageIndex.PageMessageDetails,new MalMessageDetailsNavArgs{WorkMode = MessageDetailsWorkMode.Message,NewMessageTarget = user.Name});
                   }));



        public bool IsMangaBoard => _prevArgs.TopicType == TopicType.Manga;

        #endregion



        private async void LoadCurrentTopicPage(bool force = false,bool lastpost = false)
        {
            LoadingTopic = true;

            CurrentTopicData = await ForumTopicQueries.GetTopicData(_prevArgs.TopicId, CurrentPage,lastpost,null,force);
            Messages = new ObservableCollection<ForumTopicMessageEntryViewModel>(
                CurrentTopicData.Messages.Select(
                    entry => new ForumTopicMessageEntryViewModel(entry)));
            if (lastpost)
            {
                CurrentPage = CurrentTopicData.AllPages;
            }

            LoadingTopic = false;

        }

        public void RemoveMessage(ForumTopicMessageEntryViewModel forumTopicMessageEntryViewModel)
        {
            Messages.Remove(forumTopicMessageEntryViewModel);
        }

        public async void QuouteMessage(string dataId,string poster)
        {
            ReplyMessage += $"[quote={poster} message={dataId}]" + await ForumTopicQueries.GetQuote(dataId) + "[/quoute]";
        }

        public void RegisterSelfBackNav()
        {
            _prevArgs.FirstVisibleItemIndex = ScrollInfoProvider.GetFirstVisibleItemIndex();
            _prevArgs.TopicPage = CurrentPage;
            _prevArgs.MessageId = null;
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageForumIndex, _prevArgs);
        }
    }
}
