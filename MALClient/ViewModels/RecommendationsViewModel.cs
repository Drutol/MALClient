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
        private ObservableCollection<PivotItem> _recomendationItems = new ObservableCollection<PivotItem>();
        public ObservableCollection<PivotItem> RecommendationItems
        {
            get { return _recomendationItems; }
            set
            {
                _recomendationItems = value;                
            }
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

        private async void PopulateData()
        {
            var data = await new AnimeRecomendationsQuery().GetRecomendationsData();
            int i = 0;
            foreach (var item in data)
            {
                var pivot = new PivotItem
                {
                    Header = new StackPanel
                    {
                        Children = { new TextBlock { Text = item.DependentTitle, FontSize = 18 }, new TextBlock { Text = item.RecommendationTitle, FontSize = 18 } }
                    },
                    Content = new RecomendationItem(item, i++)
                };
                _recomendationItems.Add(pivot);
            }
            Loading = false;
            RaisePropertyChanged(() => PivotItemIndex);
        }
    }
}
