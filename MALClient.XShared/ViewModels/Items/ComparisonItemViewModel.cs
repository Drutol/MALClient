using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Models.Models.Library;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.Utils;

namespace MALClient.XShared.ViewModels.Items
{
    public class ComparisonItemViewModel : ViewModelBase
    {
        private ICommand _addToMyListCommand;
        private bool _isComparisonValid;
        private bool _isValidScoreDifference;
        private int _watchedDifference;
        private int _scoreDifference;
        private bool _watchedComparisonBarVisibility;

        public AnimeItemViewModel MyEntry { get; private set; }
        public AnimeItemViewModel OtherEntry { get; }


        public ComparisonItemViewModel(AnimeItemViewModel myEntry, AnimeItemViewModel otherEntry)
        {
            MyEntry = myEntry;
            OtherEntry = otherEntry;

            var setBasicData = false;

            if (MyEntry != null)
            {
                MyEntry.ChangedScore += EntryOnChangedScore;
                MyEntry.ChangedAuth += MyEntryOnChangedAuth;
                MyEntry.ChangedWatched += EntryOnChangedWatched;

                Title = MyEntry.Title;
                ImgUrl = MyEntry.ImgUrl;
                Id = MyEntry.Id;

                setBasicData = true;
            }
            if (OtherEntry != null)
            {
                OtherEntry.ChangedScore += EntryOnChangedScore;
                OtherEntry.ChangedWatched += EntryOnChangedWatched;

                if (!setBasicData)
                {
                    Title = OtherEntry.Title;
                    ImgUrl = OtherEntry.ImgUrl;
                    Id = OtherEntry.Id;
                }

            }

            IsComparisonValid = myEntry != null && otherEntry != null;

            UpdateScoreDiff();
            UpdateWatchedDiff();
        }



        public string ScoreDifferenceBind
        {
            get
            {
                if (ScoreDifference == 0)
                    return "-";
                if (ScoreDifference > 0)
                    return $"+{ScoreDifference}";
                return ScoreDifference.ToString();
            }
        }

        public int ScoreDifference
        {
            get { return _scoreDifference; }
            set
            {
                _scoreDifference = value;
                RaisePropertyChanged();
                RaisePropertyChanged(() => ScoreDifferenceBind);
            }
        }

        public string WatchedDifferenceBind
        {
            get
            {
                if (WatchedDifference == 0)
                    return "-";
                if (WatchedDifference > 0)
                    return $"+{WatchedDifference}";
                return WatchedDifference.ToString();
            }
        }

        public int WatchedDifference
        {
            get { return _watchedDifference; }
            set
            {
                _watchedDifference = value;
                RaisePropertyChanged();
                RaisePropertyChanged(() => WatchedDifferenceBind);
                WatchedComparisonBarVisibility = value != 0;
            }
        }

        public bool IsValidScoreDifference
        {
            get { return _isValidScoreDifference; }
            set
            {
                _isValidScoreDifference = value;
                RaisePropertyChanged();
            }
        }

        public bool IsComparisonValid
        {
            get { return _isComparisonValid; }
            set
            {
                _isComparisonValid = value;
                if (value)
                {
                    UpdateScoreDiff();
                    UpdateWatchedDiff();
                }
                RaisePropertyChanged();
                RaisePropertyChanged(() => IsOnMyList);
                RaisePropertyChanged(() => IsOnlyOnOtherList);
                RaisePropertyChanged(() => IsOnlyOnMyList);
            }
        }

        public bool WatchedComparisonBarVisibility
        {
            get { return _watchedComparisonBarVisibility; }
            set
            {
                _watchedComparisonBarVisibility = value;
                RaisePropertyChanged();
            }
        }

        public string Title { get; set; }
        public string ImgUrl { get; set; }
        public int Id { get; set; }


        private void UpdateScoreDiff()
        {
            if (IsComparisonValid)
            {
                if (MyEntry.MyScore > 0 && OtherEntry.MyScore > 0)
                    ScoreDifference = (int)(OtherEntry.MyScore - MyEntry.MyScore);
                else
                    IsValidScoreDifference = false;
            }
        }

        private void UpdateWatchedDiff()
        {
            if(IsComparisonValid)
                WatchedDifference = OtherEntry.MyEpisodes - MyEntry.MyEpisodes;
        }

        public bool IsOnMyList => MyEntry != null;
        public bool IsOnlyOnOtherList => OtherEntry != null && MyEntry == null;
        public bool IsOnlyOnMyList => MyEntry != null && OtherEntry == null;



        public ICommand AddToMyListCommand => _addToMyListCommand ?? (_addToMyListCommand = new RelayCommand(async () =>
        {
            await AddToMyListAsync();
        }));

        public async Task AddToMyListAsync()
        {
            var response = await new AnimeAddQuery(OtherEntry.Id.ToString()).GetRequestResponse();
            if (response != "Created")
                return;

            var vm = new AnimeItemAbstraction(true, new AnimeLibraryItemData(OtherEntry.ParentAbstraction.EntryData));

            var startDate = "0000-00-00";
            if (Settings.SetStartDateOnListAdd)
            {
                startDate = DateTimeOffset.Now.ToString("yyyy-MM-dd");
            }
            vm.MyStartDate = startDate;
            vm.MyStatus = Settings.DefaultStatusAfterAdding;

            MyEntry = vm.ViewModel;
            ResourceLocator.AnimeLibraryDataStorage.AddAnimeEntry(vm);

            IsComparisonValid = true;
            RaisePropertyChanged(() => MyEntry);
        }


        #region UpdateHandlers

        private void EntryOnChangedWatched(object sender, int i)
        {
            UpdateWatchedDiff();
        }

        private void MyEntryOnChangedAuth(object sender, bool b)
        {
            IsComparisonValid = b;
        }

        private void EntryOnChangedScore(object sender, int i)
        {
            UpdateScoreDiff();
        }

        #endregion

        ~ComparisonItemViewModel()
        {
            if (MyEntry != null)
            {
                MyEntry.ChangedScore -= EntryOnChangedScore;
                MyEntry.ChangedAuth -= MyEntryOnChangedAuth;
                MyEntry.ChangedWatched -= EntryOnChangedWatched;
            }

            if (OtherEntry != null)
            {
                OtherEntry.ChangedScore -= EntryOnChangedScore;
                OtherEntry.ChangedWatched -= EntryOnChangedWatched;
            }

        }
    }
}
