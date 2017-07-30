using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Adapters;
using MALClient.Models.Models.MalSpecific;
using MALClient.XShared.Comm.MagicalRawQueries.Clubs;
using MALClient.XShared.Utils;

namespace MALClient.XShared.ViewModels.Clubs
{
    public class ClubIndexViewModel : ViewModelBase
    {
        private readonly IMessageDialogProvider _dialogProvider;
        private bool _loading;
        private SmartObservableCollection<MalClubEntry> _clubs;
        private string _searchQuery;
        private List<MalClubEntry> _myClubs;
        private List<MalClubEntry> _allClubs;
        private List<MalClubEntry> _lastQueryClubs;
        private bool _emptyNoticeVisibility;
        private int _currentPage = 1;
        private bool _moreButtonVisibility;
        private MalClubQueries.QueryType _queryType = MalClubQueries.QueryType.My;

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

        public List<MalClubEntry> MyClubs
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

        public MalClubQueries.QueryType QueryType
        {
            get { return _queryType; }
            set
            {
                _queryType = value;
                if (QueryType == MalClubQueries.QueryType.All)
                    LoadAllClubs();
            }
        }

        public MalClubQueries.SearchCategory SearchCategory { get; set; }

        public async void NavigatedTo(bool force = false)
        {
            if (QueryType == MalClubQueries.QueryType.My)
            {
                if (_myClubs == null)
                {
                    Loading = true;
                    MyClubs = await MalClubQueries.GetClubs(QueryType, 0);
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

                Clubs = new SmartObservableCollection<MalClubEntry>(_allClubs);
            }
        }

        public ICommand SearchCommand => new RelayCommand( async () =>
        {
            if (SearchQuery.Length <= 2)
            {
                _dialogProvider.ShowMessageDialog("Invalid query.","Serach phrase needs to be at least 3 characters long.");
                return;
            }
            _currentPage = 1;

            _lastQueryClubs = await MalClubQueries.GetClubs(QueryType, 0, SearchCategory, SearchQuery);
            if (_lastQueryClubs != null)
            {
                Clubs.AddRange(_lastQueryClubs);
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
            var clubs = await MalClubQueries.GetClubs(QueryType, _currentPage, SearchCategory, SearchQuery);
            if (clubs != null)
            {
                _lastQueryClubs.AddRange(clubs);
                Clubs.AddRange(clubs);
            }
            else
                MoreButtonVisibility = false;
        });
    }
}
