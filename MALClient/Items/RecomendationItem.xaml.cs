using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using MALClient.Pages;
using MALClient.ViewModels;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.Items
{
    public sealed partial class RecomendationItem : UserControl
    {
        private readonly RecomendationData _data;
        private readonly ObservableCollection<Tuple<string,string,string,string,string>> _detailItems = new ObservableCollection<Tuple<string,string, string, string, string>>();
        private bool _dataLoaded;

        public RecomendationItem(RecomendationData data, int index)
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Index = index;
            _data = data;
        }

        public int Index { get; }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                var scrollViewer =
                    VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(DetailsListView, 0), 0) as ScrollViewer;
                scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            }
            catch (Exception)
            {
                //
            }
        }


        public async Task PopulateData()
        {
            if (_dataLoaded)
                return;
            SpinnerLoading.Visibility = Visibility.Visible;
            await _data.FetchData();
            DepImg.Source = new BitmapImage(new Uri(_data.DependentImgUrl));
            RecImg.Source = new BitmapImage(new Uri(_data.RecommendationImgUrl));
            TxtDepTitle.Text = _data.DependentTitle;
            TxtRecTitle.Text = _data.RecommendationTitle;
            TxtRecommendation.Text = _data.Description;

            var myDepItem = await ViewModelLocator.AnimeList.TryRetrieveAuthenticatedAnimeItem(_data.DependentId);
            var myRecItem = await ViewModelLocator.AnimeList.TryRetrieveAuthenticatedAnimeItem(_data.RecommendationId);

            _detailItems.Add(new Tuple<string, string, string, string,string>("Episodes", _data.DependentEpisodes,myDepItem?.MyEpisodes.ToString() ?? "", _data.RecommendationEpisodes, myRecItem?.MyEpisodes.ToString() ?? ""));
            _detailItems.Add(new Tuple<string, string, string, string, string>("Score", _data.DependentGlobalScore.ToString(), myDepItem?.MyScore.ToString() ?? "", _data.RecommendationGlobalScore.ToString(), myRecItem?.MyScore.ToString() ?? ""));
            _detailItems.Add(new Tuple<string, string, string, string, string>("Type", _data.DependentType,"", _data.RecommendationType,""));
            _detailItems.Add(new Tuple<string, string, string, string, string>("Status", _data.DependentStatus,"", _data.RecommendationStatus,""));
            _detailItems.Add(new Tuple<string, string, string, string, string>("Start:", _data.DependentStartDate, myDepItem?.StartDate ?? "", _data.RecommendationStartDate, myRecItem?.StartDate ?? ""));
            _detailItems.Add(new Tuple<string, string, string, string, string>("End:", _data.DependentEndDate, myDepItem?.EndDate ?? "", _data.RecommendationStartDate, myRecItem?.EndDate?? ""));
            DetailsListView.ItemsSource = _detailItems;
            _dataLoaded = true;
            SpinnerLoading.Visibility = Visibility.Collapsed;
        }

        private async void ButtonRecomDetails_OnClick(object sender, RoutedEventArgs e)
        {
            await Utils.GetMainPageInstance()
                .Navigate(PageIndex.PageAnimeDetails,
                    new AnimeDetailsPageNavigationArgs(_data.RecommendationId, _data.RecommendationTitle,
                        _data.RecommendationData, null,
                        new RecommendationPageNavigationArgs {Index = Index}) {Source = PageIndex.PageRecomendations});
        }

        private async void ButtonDependentDetails_OnClick(object sender, RoutedEventArgs e)
        {
            await Utils.GetMainPageInstance()
                .Navigate(PageIndex.PageAnimeDetails,
                    new AnimeDetailsPageNavigationArgs(_data.DependentId, _data.DependentTitle,
                        _data.DependentData, null,
                        new RecommendationPageNavigationArgs {Index = Index}) {Source = PageIndex.PageRecomendations});
        }
    }
}