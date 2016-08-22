using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using MALClient.Models.Models.AnimeScrapped;
using MALClient.XShared.Comm.Anime;
using RecomendationItem = MalClient.Shared.Items.RecomendationItem;

namespace MALClient.XShared.ViewModels.Main
{
    public class RecommendationsViewModel : ViewModelBase
    {
        private bool _loading = true;

        private int _pivotItemIndex;

        public RecommendationsViewModel()
        {
            PopulateData();
        }

        public ObservableCollection<PivotItem> RecommendationItems { get; } = new ObservableCollection<PivotItem>();

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

        public async void PopulateData()
        {
            Loading = true;
            var data = new List<RecomendationData>();
            await Task.Run(async () => data = await new AnimeRecomendationsQuery().GetRecomendationsData());
            if (data == null)
            {
                Loading = false;
                return;
            }
            RecommendationItems.Clear();
            var i = 0;
            foreach (var item in data)
            {
                var pivot = new PivotItem
                {
                    Header = item.DependentTitle + "\n" + item.RecommendationTitle,
                    Content = new RecomendationItem(item, i++)
                };
                RecommendationItems.Add(pivot);
            }
            Loading = false;
            RaisePropertyChanged(() => PivotItemIndex);
        }
    }
}