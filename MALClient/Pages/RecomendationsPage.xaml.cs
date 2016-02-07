using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MALClient.Comm;
using MALClient.Items;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages
{
    public class RecommendationPageNavigationArgs
    {
        public int Index;
    }


    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RecomendationsPage : Page
    {
        private readonly ObservableCollection<PivotItem> _recomendationItems = new ObservableCollection<PivotItem>();
       

        private bool _loaded;
        private int _startItem = 0;

        public RecomendationsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SpinnerLoading.Visibility = Visibility.Visible;
            if (e.Parameter is RecommendationPageNavigationArgs)
                _startItem = (e.Parameter as RecommendationPageNavigationArgs).Index;

            FetchData();
        }

        private async void FetchData()
        {          
            PopulateData(await new AnimeRecomendationsQuery().GetRecomendationsData());
        }

        private void PopulateData(List<RecomendationData> data)
        {
            int i = 0;
            foreach (var item in data)
            {
                var pivot = new PivotItem
                {
                    Header = new StackPanel
                    {
                        Children = { new TextBlock {Text = item.DependentTitle , FontSize = 18} , new TextBlock  { Text = item.RecommendationTitle , FontSize = 18} }
                    },                   
                    Content = new RecomendationItem(item,i++)
                };
                _recomendationItems.Add(pivot);
            }
            Pivot.ItemsSource = _recomendationItems;
            Pivot.SelectedIndex = _startItem;       
            SpinnerLoading.Visibility = Visibility.Collapsed;
            _loaded = true;
        }

        private void Pivot_OnPivotItemLoading(Pivot sender, PivotItemEventArgs args)
        {
            (args.Item.Content as RecomendationItem).PopulateData();
        }
    }
}
