using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Models.Models.AnimeScrapped;
using MALClient.XShared.Comm.Anime;

namespace MALClient.XShared.ViewModels.Main
{
    public class RecommendationsViewModel : ViewModelBase
    {
        public class XPivotItem
        {
            public object Header { get; set; }
            public object Content { get; set; }
        }

        private bool _loading;

        private int _pivotItemIndex;
        private bool _animeMode = true;

        public ObservableCollection<XPivotItem> RecommendationItems { get; } = new ObservableCollection<XPivotItem>();

        public bool Loading
        {
            get { return _loading; }
            set
            {
                _loading = value;
                RaisePropertyChanged(() => Loading);
            }
        }

        public int PivotItemIndex
        {
            get { return _pivotItemIndex; }
            set
            {
                _pivotItemIndex = value;
                if (!Loading)
                    RaisePropertyChanged(() => PivotItemIndex);
            }
        }

        public bool AnimeMode
        {
            get { return _animeMode; }
            set
            {
                if(_animeMode == value)
                    return;

                _animeMode = value;
                PopulateData();             
            }
        }

        public ICommand SwitchToAnimeCommand => new RelayCommand(() => AnimeMode = true);

        public ICommand SwitchToMangaCommand => new RelayCommand(() => AnimeMode = false);

        private bool? _prevLoaded;

        public async void PopulateData()
        {
            if(Loading || _prevLoaded == AnimeMode)
                return;
            Loading = true;
            var data = new List<RecommendationData>();
            ViewModelLocator.GeneralMain.CurrentStatus = AnimeMode ? "Anime Recommendations" : "Manga Recommendations";
            _prevLoaded = AnimeMode;
            await Task.Run(async () => data = await new AnimeRecomendationsQuery(AnimeMode).GetRecomendationsData());

            if (data == null)
            {
                Loading = false;
                return;
            }

            RecommendationItems.Clear();
            var i = 0;
            foreach (var item in data)
            {
                var pivot = new XPivotItem
                {
                    Header = item.DependentTitle + "\n" + item.RecommendationTitle,
                    Content = new RecommendationItemViewModel(item, i++)
                };
                RecommendationItems.Add(pivot);
            }
            Loading = false;
            RaisePropertyChanged(() => PivotItemIndex);
        }
    }
}