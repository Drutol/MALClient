using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using MALClient.Models.Enums;
using MALClient.XShared.Interfaces;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels.Items;

namespace MALClient.XShared.ViewModels.Main
{
    public enum ComparisonFilter
    {
        OnBoth,
        OnMine,
        OnOther,
        All,
    }

    public enum ComparisonSorting
    {
        ScoreDifference,
        MyScore,
        OtherScore,

        WatchedDifference,
        MyWatched,
        OtherWatched
    }

    public enum ComparisonStatusFilterTarget
    {
        My,
        Other,
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
        private AnimeStatus _statusFilter;
        private ComparisonStatusFilterTarget _statusFilterTarget;
        private bool _sortAscending;


        public SmartObservableCollection<ComparisonItemViewModel> CurrentItems { get; }
            = new SmartObservableCollection<ComparisonItemViewModel>();

        public ComparisonFilter ComparisonFilter
        {
            get { return _comparisonFilter; }
            set
            {
                _comparisonFilter = value;
                RaisePropertyChanged();
            }
        }

        public ComparisonSorting ComparisonSorting
        {
            get { return _comparisonSorting; }
            set
            {
                _comparisonSorting = value;
                RaisePropertyChanged();
            }
        }

        public AnimeStatus StatusFilter
        {
            get { return _statusFilter; }
            set
            {
                _statusFilter = value;
                RaisePropertyChanged();
            }
        }

        public ComparisonStatusFilterTarget StatusFilterTarget
        {
            get { return _statusFilterTarget; }
            set
            {
                _statusFilterTarget = value;
                RaisePropertyChanged();
            }
        }

        public bool SortAscending
        {
            get { return _sortAscending; }
            set
            {
                _sortAscending = value;
                RaisePropertyChanged();
            }
        }

        public ListComparisonViewModel(IAnimeLibraryDataStorage animeLibraryDataStorage, INavMgr navMgr)
        {
            _animeLibraryDataStorage = animeLibraryDataStorage;
            _navMgr = navMgr;


        }

        public void NavigatedTo(ListComparisonPageNavigationArgs args)
        {
            _navArgs = args;

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
        }

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


        }

    }
}
