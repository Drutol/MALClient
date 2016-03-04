using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using MALClient.Comm;
using MALClient.Items;

namespace MALClient.ViewModels
{
    public class RecommendationsViewModel : ViewModelBase
    {
        private readonly ObservableCollection<PivotItem> _recomendationItems = new ObservableCollection<PivotItem>();
        public ObservableCollection<PivotItem> RecommendationItems
        {
            get { return _recomendationItems; }
        }

        private bool _loading = true;
        public bool Loading
        {
            get { return _loading; }
            set
            {
                _loading = value;
                RaisePropertyChanged(() => Loading);
            }
        }

        private int _pivotItemIndex;
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

        public RecommendationsViewModel()
        {
            PopulateData();
        }

        public async void PopulateData()
        {
            Loading = true;
            List<RecomendationData> data = new List<RecomendationData>();
            await Task.Run(async () => data = await new AnimeRecomendationsQuery().GetRecomendationsData());
            _recomendationItems.Clear();
            int i = 0;
            foreach (var item in data)
            {
                var pivot = new PivotItem
                {
                    Header = item.DependentTitle + "\n" + item.RecommendationTitle,
                    Content = new RecomendationItem(item, i++)
                };
                _recomendationItems.Add(pivot);
            }
            Loading = false;
            RaisePropertyChanged(() => PivotItemIndex);
        }
    }
}
