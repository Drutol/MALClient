using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using MalClient.Shared.NavArgs;
using MALClient.Utils.Enums;
using MALClient.ViewModels;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.Items
{
    public sealed partial class RecomendationItem : UserControl
    {
        private readonly RecomendationData _data;

        private readonly ObservableCollection<Tuple<string, string, string, string, string>> _detailItems =
            new ObservableCollection<Tuple<string, string, string, string, string>>();

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
            DepImg.Source = new BitmapImage(new Uri(_data.DependentData.ImgUrl));
            RecImg.Source = new BitmapImage(new Uri(_data.RecommendationData.ImgUrl));
            TxtDepTitle.Text = _data.DependentTitle;
            TxtRecTitle.Text = _data.RecommendationTitle;
            TxtRecommendation.Text = _data.Description;

            var myDepItem = await ViewModelLocator.GeneralAnimeList.TryRetrieveAuthenticatedAnimeItem(_data.DependentId);
            var myRecItem = await ViewModelLocator.GeneralAnimeList.TryRetrieveAuthenticatedAnimeItem(_data.RecommendationId);

            _detailItems.Add(new Tuple<string, string, string, string, string>("Episodes:",
                _data.DependentData.AllEpisodes.ToString(),
                myDepItem?.MyEpisodes == null ? "" : myDepItem.MyEpisodes + $"/{_data.DependentData.AllEpisodes}",
                _data.RecommendationData.AllEpisodes.ToString(),
                myRecItem?.MyEpisodes == null ? "" : myRecItem.MyEpisodes + $"/{_data.RecommendationData.AllEpisodes}"));
            _detailItems.Add(new Tuple<string, string, string, string, string>("Score:",
                _data.DependentData.GlobalScore.ToString(),
                myDepItem?.MyScore == null ? "" : $"{myDepItem.MyScore}/10",
                _data.RecommendationData.GlobalScore.ToString(),
                myRecItem?.MyScore == null ? "" : $"{myRecItem.MyScore}/10"));
            _detailItems.Add(new Tuple<string, string, string, string, string>("Type:", _data.DependentData.Type, "",
                _data.RecommendationData.Type, ""));
            _detailItems.Add(new Tuple<string, string, string, string, string>("Status:", _data.DependentData.Status, "",
                _data.RecommendationData.Status, ""));

            _detailItems.Add(new Tuple<string, string, string, string, string>("Start:",
                _data.DependentData.StartDate == AnimeItemViewModel.InvalidStartEndDate
                    ? "?"
                    : _data.DependentData.StartDate,
                myDepItem != null
                    ? (myDepItem.StartDate != AnimeItemViewModel.InvalidStartEndDate ? myDepItem.StartDate : "Not set")
                    : "",
                _data.RecommendationData.StartDate == AnimeItemViewModel.InvalidStartEndDate
                    ? "?"
                    : _data.RecommendationData.StartDate,
                myRecItem != null
                    ? (myRecItem.StartDate != AnimeItemViewModel.InvalidStartEndDate ? myRecItem.StartDate : "Not set")
                    : ""));

            _detailItems.Add(new Tuple<string, string, string, string, string>("End:",
                _data.DependentData.EndDate == AnimeItemViewModel.InvalidStartEndDate
                    ? "?"
                    : _data.DependentData.EndDate,
                myDepItem != null
                    ? (myDepItem.EndDate != AnimeItemViewModel.InvalidStartEndDate ? myDepItem.EndDate : "Not set")
                    : "",
                _data.RecommendationData.EndDate == AnimeItemViewModel.InvalidStartEndDate
                    ? "?"
                    : _data.RecommendationData.EndDate,
                myRecItem != null
                    ? (myRecItem.EndDate != AnimeItemViewModel.InvalidStartEndDate ? myRecItem.EndDate : "Not set")
                    : ""));
            DetailsListView.ItemsSource = _detailItems;
            _dataLoaded = true;
            SpinnerLoading.Visibility = Visibility.Collapsed;
        }

        private  void ButtonRecomDetails_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModelLocator.GeneralMain
               .Navigate(PageIndex.PageAnimeDetails,
                    new AnimeDetailsPageNavigationArgs(_data.RecommendationId, _data.RecommendationTitle,
                        _data.RecommendationData, null,
                        new RecommendationPageNavigationArgs {Index = Index})
                    {Source = PageIndex.PageRecomendations});
        }

        private  void ButtonDependentDetails_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModelLocator.GeneralMain
               .Navigate(PageIndex.PageAnimeDetails,
                    new AnimeDetailsPageNavigationArgs(_data.DependentId, _data.DependentTitle,
                        _data.DependentData, null,
                        new RecommendationPageNavigationArgs {Index = Index})
                    {Source = PageIndex.PageRecomendations});
        }
    }
}