using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Models.Enums;
using MALClient.Models.Models;
using MALClient.XShared.Interfaces;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels.Items;

namespace MALClient.XShared.ViewModels.Main
{
    public enum ComparisonFilter
    {
        [EnumUtilities.Description("Show shared items")]
        OnBoth,
        [EnumUtilities.Description("Show my items")]
        OnMine,
        [EnumUtilities.Description("Show other user's items")]
        OnOther,
        [EnumUtilities.Description("Show all items")]
        All,
    }

    public enum ComparisonSorting
    {
        [EnumUtilities.Description("Score Difference")]
        ScoreDifference,
        [EnumUtilities.Description("My Score")]
        MyScore,
        [EnumUtilities.Description("Other Score")]
        OtherScore,

        [EnumUtilities.Description("Watched Difference")]
        WatchedDifference,
        [EnumUtilities.Description("My Watched")]
        MyWatched,
        [EnumUtilities.Description("Other Watched")]
        OtherWatched
    }

    public enum ComparisonStatusFilterTarget
    {
        [EnumUtilities.Description("My items")]
        My,
        [EnumUtilities.Description("Other items")]
        Other,
        [EnumUtilities.Description("Both items")]
        Both
    }

    public class ListComparisonViewModel : ViewModelBase
    {
        private readonly IAnimeLibraryDataStorage _animeLibraryDataStorage;
        private readonly INavMgr _navMgr;

        private ListComparisonPageNavigationArgs _navArgs;
        private List<ComparisonItemViewModel> _allSharedItems = new List<ComparisonItemViewModel>();
        private List<ComparisonItemViewModel> _allMyItems = new List<ComparisonItemViewModel>();
        private List<ComparisonItemViewModel> _allOtherItems = new List<ComparisonItemViewModel>();
        private ComparisonFilter _comparisonFilter;
        private ComparisonSorting _comparisonSorting;
        private AnimeStatus _statusFilter = AnimeStatus.AllOrAiring;
        private ComparisonStatusFilterTarget _statusFilterTarget;
        private bool _sortAscending;
        private bool _loading;
        private bool _emptyNoticeVisibility;

        public ProfileData MyData { get; set; }
        public ProfileData OtherData { get; set; }


        public SmartObservableCollection<ComparisonItemViewModel> CurrentItems { get; set; }
            = new SmartObservableCollection<ComparisonItemViewModel>();

        public ComparisonFilter ComparisonFilter
        {
            get { return _comparisonFilter; }
            set
            {
                _comparisonFilter = value;
                RaisePropertyChanged();
                RefreshList();
            }
        }

        public ComparisonSorting ComparisonSorting
        {
            get { return _comparisonSorting; }
            set
            {
                _comparisonSorting = value;
                RaisePropertyChanged();
                RefreshList();
            }
        }

        public AnimeStatus StatusFilter
        {
            get { return _statusFilter; }
            set
            {
                _statusFilter = value;
                RaisePropertyChanged();
                RefreshList();
            }
        }

        public ComparisonStatusFilterTarget StatusFilterTarget
        {
            get { return _statusFilterTarget; }
            set
            {
                _statusFilterTarget = value;
                RaisePropertyChanged();
                RefreshList();
            }
        }

        public bool SortAscending
        {
            get { return _sortAscending; }
            set
            {
                _sortAscending = value;
                RaisePropertyChanged();
                RefreshList();
            }
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

        public bool EmptyNoticeVisibility
        {
            get { return _emptyNoticeVisibility; }
            set
            {
                _emptyNoticeVisibility = value;
                RaisePropertyChanged();
            }
        }

        public ListComparisonViewModel(IAnimeLibraryDataStorage animeLibraryDataStorage, INavMgr navMgr)
        {
            _animeLibraryDataStorage = animeLibraryDataStorage;
            _navMgr = navMgr;
        }

        public async void NavigatedTo(ListComparisonPageNavigationArgs args)
        {
            CurrentItems = new SmartObservableCollection<ComparisonItemViewModel>();
            RaisePropertyChanged(() => CurrentItems);

            _navArgs = args;
            Loading = true;
            MyData = await DataCache.RetrieveProfileData(Credentials.UserName);
            OtherData = await DataCache.RetrieveProfileData(args.CompareWith.Name);
            RaisePropertyChanged(() => MyData);
            RaisePropertyChanged(() => OtherData);

            var otherItems = _animeLibraryDataStorage.OthersAbstractions[_navArgs.CompareWith.Name].Item1;

            foreach (var myItem in _animeLibraryDataStorage.AllLoadedAuthAnimeItems)
            {
                var sharedItem = otherItems.FirstOrDefault(abstraction => abstraction.Id == myItem.Id);

                if(sharedItem != null)
                    _allSharedItems.Add(new ComparisonItemViewModel(myItem.ViewModel,sharedItem.ViewModel));
                else
                    _allMyItems.Add(new ComparisonItemViewModel(myItem.ViewModel,null));
            }
            var usedIds = _allSharedItems.Concat(_allMyItems).Select(model => model.MyEntry.Id);
            _allOtherItems = otherItems.Where(other => !usedIds.Any(i => i == other.Id))
                .Select(abstraction => new ComparisonItemViewModel(null, abstraction.ViewModel)).ToList();

            Loading = false;
            RefreshList();

        }

        public ICommand NavigateDetailsCommand => new RelayCommand<ComparisonItemViewModel>(viewModel =>
        {
            if (viewModel.MyEntry != null)
            {
                viewModel.MyEntry.NavigateDetails(PageIndex.PageListComparison,_navArgs);
            }
            else
            {
                if (ViewModelLocator.Mobile)
                {
                    _navMgr.RegisterBackNav(PageIndex.PageListComparison,_navArgs);                   
                }
                ViewModelLocator.GeneralMain.Navigate(PageIndex.PageAnimeDetails, new AnimeDetailsPageNavigationArgs(viewModel.Id, viewModel.Title, null, null));
            }
        });

        public void RefreshList()
        {
            List<ComparisonItemViewModel> source;
            switch (ComparisonFilter)
            {
                case ComparisonFilter.OnBoth:
                    source = _allSharedItems;
                    break;
                case ComparisonFilter.OnMine:
                    source = _allMyItems;
                    break;
                case ComparisonFilter.OnOther:
                    source = _allOtherItems;
                    break;
                case ComparisonFilter.All:
                    source = _allSharedItems.Concat(_allMyItems).Concat(_allOtherItems).ToList();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (StatusFilter != AnimeStatus.AllOrAiring)
            {
                switch (StatusFilterTarget)
                {
                    case ComparisonStatusFilterTarget.My:
                        source = source.Where(model => model.MyEntry.MyStatus == StatusFilter).ToList();
                        break;
                    case ComparisonStatusFilterTarget.Other:
                        source = source.Where(model => model.OtherEntry.MyStatus == StatusFilter).ToList();
                        break;
                    case ComparisonStatusFilterTarget.Both:
                        source = source.Where(model => model.MyEntry.MyStatus == StatusFilter && model.OtherEntry.MyStatus == StatusFilter).ToList();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            switch (ComparisonSorting)
            {
                case ComparisonSorting.ScoreDifference:
                    source = source.OrderByDescending(model => model.ScoreDifference).ToList();
                    break;
                case ComparisonSorting.MyScore:
                    source = source.OrderByDescending(model => model.MyEntry?.MyScore ?? 0).ToList();
                    break;
                case ComparisonSorting.OtherScore:
                    source = source.OrderByDescending(model => model.OtherEntry?.MyScore ?? 0).ToList();
                    break;
                case ComparisonSorting.WatchedDifference:
                    source = source.OrderByDescending(model => model.WatchedDifference).ToList();
                    break;
                case ComparisonSorting.MyWatched:
                    source = source.OrderByDescending(model => model.MyEntry?.MyEpisodes ?? 0).ToList();
                    break;
                case ComparisonSorting.OtherWatched:
                    source = source.OrderByDescending(model => model.OtherEntry?.MyEpisodes ?? 0).ToList();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (SortAscending)
                source.Reverse();

            CurrentItems.Clear();
            CurrentItems.AddRange(source);
            EmptyNoticeVisibility = !CurrentItems.Any();

        }

    }
}
