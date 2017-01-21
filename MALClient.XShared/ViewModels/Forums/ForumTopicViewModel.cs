using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Models.Enums;
using MALClient.Models.Models.Forums;
using MALClient.XShared.Comm.MagicalRawQueries.Forums;
using MALClient.XShared.Delegates;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.ViewModels.Forums.Items;

namespace MALClient.XShared.ViewModels.Forums
{
    public class ForumTopicViewModel : ViewModelBase
    {

        private bool _loadingTopic;
        

        private ForumsTopicNavigationArgs _prevArgs;
        private int _currentPage;
        private List<Tuple<int, bool>> _availablePages;
        private int _allPages = 5;
        private ObservableCollection<ForumTopicMessageEntryViewModel> _messages;
        private ICommand _loadPageCommand;
        private ICommand _loadGotoPageCommand;
        private ICommand _gotoLastPageCommand;
        private ICommand _gotoFirstPageCommand;
        private ForumTopicData _currentTopicData;
        private string _gotoPageTextBind;
        private string _title;


        public async void Init(ForumsTopicNavigationArgs args)
        {
            LoadingTopic = true;
            _prevArgs = args;


            var data = await ForumTopicQueries.GetTopicMessages(_prevArgs.TopicId, _prevArgs.TopicPage, _prevArgs.LastPost);
            if (_prevArgs.LastPost)
            {
                CurrentPage = data.AllPages;
            }
            else if (_prevArgs.MessageId != null)
            {
                CurrentPage = data.CurrentPage;
            }
            Messages = new ObservableCollection<ForumTopicMessageEntryViewModel>(
                data.Messages.Select(
                    entry => new ForumTopicMessageEntryViewModel(entry)));

            

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

        public int CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                AvailablePages.Clear();
                var start = value <= 2 ? 1 : value - 2;
                for (int i = start; i <= start + 4 && i <= _allPages + 1; i++)
                    AvailablePages.Add(new Tuple<int, bool>(i, i == value + 1));
            }
        }

        public List<Tuple<int, bool>> AvailablePages
        {
            get { return _availablePages; }
            set
            {
                _availablePages = value;
                RaisePropertyChanged(() => AvailablePages);
            }
        }

        public bool LoadingTopic
        {
            get { return _loadingTopic; }
            set
            {
                _loadingTopic = value;
                RaisePropertyChanged(() => LoadingTopic);
            }
        }

        public bool IsMangaBoard
            =>
                _prevArgs.TargetBoard == null && _prevArgs.TopicType == TopicType.Manga;

        public string GotoPageTextBind
        {
            get { return _gotoPageTextBind; }
            set
            {
                _gotoPageTextBind = value;
                RaisePropertyChanged(() => GotoPageTextBind);
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged(() => Title);
            }
        }

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
                CurrentPage = _currentTopicData.AllPages;
                LoadCurrentTopicPage();
            }));



        public ICommand GotoFirstPageCommand => _gotoFirstPageCommand ?? (_gotoFirstPageCommand = new RelayCommand(
            () =>
            {
                CurrentPage = 1;
                LoadCurrentTopicPage();
            }));


        private async void LoadCurrentTopicPage()
        {
            LoadingTopic = true;

            _currentTopicData = await ForumTopicQueries.GetTopicMessages(_prevArgs.TopicId, CurrentPage);
            Messages = new ObservableCollection<ForumTopicMessageEntryViewModel>(
                _currentTopicData.Messages.Select(
                    entry => new ForumTopicMessageEntryViewModel(entry)));

            LoadingTopic = false;
        }
    }
}
