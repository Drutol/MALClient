using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Android.Runtime;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Adapters;
using MALClient.Models.Enums;
using MALClient.Models.Models.MalSpecific;
using MALClient.XShared.Comm.MagicalRawQueries.Clubs;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;

namespace MALClient.XShared.ViewModels.Clubs
{
    public class ClubIndexViewModel : ViewModelBase
    {
        private readonly IMessageDialogProvider _dialogProvider;
        private bool _loading;
        private SmartObservableCollection<MalClubEntry> _clubs;
        private string _searchQuery;
        private ObservableCollection<MalClubEntry> _myClubs;
        private List<MalClubEntry> _allClubs;
        private List<MalClubEntry> _lastQueryClubs;
        private bool _emptyNoticeVisibility;
        private int _currentPage = 1;
        private bool _moreButtonVisibility = true;
        private MalClubQueries.QueryType _queryType = MalClubQueries.QueryType.My;
        private ICommand _joinClubCommand;
        private bool _myClubsEmptyNoticeVisibility;

        public ClubIndexViewModel(IMessageDialogProvider dialogProvider)
        {
            _dialogProvider = dialogProvider;
        }

        public bool Loading
        {
            get { return _loading; }
            set
            {
                _loading = value;
                RaisePropertyChanged();
            }
        }

        public SmartObservableCollection<MalClubEntry> Clubs
        {
            get { return _clubs; }
            set
            {
                _clubs = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<MalClubEntry> MyClubs
        {
            get { return _myClubs; }
            set
            {
                _myClubs = value;
                RaisePropertyChanged();
            }
        }

        
        public string SearchQuery
        {
            get { return _searchQuery; }
            [Preserve]
            set
            {
                _searchQuery = value;
                RaisePropertyChanged();
            }
        }

        public bool EmptyNoticeVisibility
        {
            get { return _emptyNoticeVisibility; }
            set
            {
                _emptyNoticeVisibility = value;
                RaisePropertyChanged();
            }
        }

        public bool MoreButtonVisibility
        {
            get { return _moreButtonVisibility; }
            set
            {
                _moreButtonVisibility = value;
                RaisePropertyChanged();
            }
        }

        public bool MyClubsEmptyNoticeVisibility
        {
            get { return _myClubsEmptyNoticeVisibility; }
            set
            {
                _myClubsEmptyNoticeVisibility = value;
                RaisePropertyChanged();
            }
        }

        public MalClubQueries.QueryType QueryType
        {
            get { return _queryType; }
            set
            {
                _queryType = value;
                if (QueryType == MalClubQueries.QueryType.All)
                    LoadAllClubs();
                RaisePropertyChanged();
            }
        }

        
        [Preserve]
        public MalClubQueries.SearchCategory SearchCategory { get; set; }

        public async void NavigatedTo(bool force = false)
        {
            if (QueryType == MalClubQueries.QueryType.My)
            {
                if (_myClubs == null)
                {
                    Loading = true;
                    MyClubs = new ObservableCollection<MalClubEntry>(await MalClubQueries.GetClubs(QueryType, 0) ?? Enumerable.Empty<MalClubEntry>());
                    MyClubsEmptyNoticeVisibility = !MyClubs.Any();
                    Loading = false;

                }
            }
        }

        private async void LoadAllClubs()
        {
            if (_allClubs == null)
            {
                Loading = true;
                _allClubs = await MalClubQueries.GetClubs(MalClubQueries.QueryType.All, 0);
                Loading = false;
                if (_allClubs != null && _allClubs.Any())
                {
                    Clubs = new SmartObservableCollection<MalClubEntry>(_allClubs);
                    EmptyNoticeVisibility = false;
                }
                else
                {
                    Clubs = new SmartObservableCollection<MalClubEntry>();
                    EmptyNoticeVisibility = true;
                }
            }
        }

        public ICommand SearchCommand => new RelayCommand( async () =>
        {
            _currentPage = 1;
#if ANDROID
            Clubs.Clear();
            EmptyNoticeVisibility = false;
#endif
            Loading = true;
            _lastQueryClubs = await MalClubQueries.GetClubs(QueryType, 0, SearchCategory, SearchQuery);
            Loading = false;
            if (_lastQueryClubs != null)
            {
                Clubs = new SmartObservableCollection<MalClubEntry>(_lastQueryClubs);
                MoreButtonVisibility = true;
                EmptyNoticeVisibility = false;
            }
            else
            {
                Clubs = new SmartObservableCollection<MalClubEntry>();
                MoreButtonVisibility = false;
                EmptyNoticeVisibility = true;
            }
        });

        public ICommand MoreCommand => new RelayCommand( async () =>
        {
            _currentPage++;
            Loading = true;
            var clubs = await MalClubQueries.GetClubs(QueryType, _currentPage, SearchCategory, SearchQuery);
            Loading = false;
            if (clubs != null)
            {
                if(_lastQueryClubs != null)
                    _lastQueryClubs.AddRange(clubs);
                else
                    _lastQueryClubs = new List<MalClubEntry>(clubs);

                if(Clubs != null)
                    Clubs.AddRange(clubs);
                else
                    Clubs = new SmartObservableCollection<MalClubEntry>(clubs);
            }
            else
                MoreButtonVisibility = false;
        });

        public ICommand NavigateDetailsCommand => new RelayCommand<MalClubEntry>(entry =>
        {
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageClubIndex,null);
            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageClubDetails,new ClubDetailsPageNavArgs{Id = entry.Id});
        });

        public ICommand JoinClubCommand => _joinClubCommand ?? (_joinClubCommand = new RelayCommand<MalClubEntry>( async entry =>
        {
            switch (entry.JoinType)
            {
                case MalClubEntry.JoinAction.Join:
                    await MalClubQueries.JoinClub(entry.Id);
                    MyClubs.Add(entry);
                    break;
                case MalClubEntry.JoinAction.Request:
                    await MalClubQueries.RequestJoinClub(entry.Id);
                    break;
                case MalClubEntry.JoinAction.AcceptDeny:
                    await MalClubQueries.AcceptDenyInvitation(entry.Id, true);
                    MyClubs.Add(entry);
                    break;
            }

            entry.JoinType = MalClubEntry.JoinAction.None;
        }));

        public async void ReloadMyClubs()
        {
            Loading = true;
            MyClubs = new ObservableCollection<MalClubEntry>(await MalClubQueries.GetClubs(MalClubQueries.QueryType.My, 0));
            MyClubsEmptyNoticeVisibility = !MyClubs.Any();
            Loading = false;
        }
    }
}
