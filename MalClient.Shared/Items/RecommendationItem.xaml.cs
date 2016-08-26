using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using MALClient.Models.Enums;
using MALClient.Models.Models.AnimeScrapped;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.ViewModels;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MalClient.Shared.Items
{
    public sealed partial class RecommendationItem : UserControl
    {
        private RecommendationData _data;



        private bool _dataLoaded;
        private bool _wide;

        public RecommendationItem()
        {
            InitializeComponent();
            SizeChanged += OnSizeChanged;
            Loaded += OnLoaded;
        }


        private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            if (_wide && sizeChangedEventArgs.NewSize.Width < 900)
            {
                _wide = false;
                VisualStateManager.GoToState(this, "Narrow",false);
            }
            else if(!_wide && sizeChangedEventArgs.NewSize.Width > 900)
            {
                _wide = true;
                VisualStateManager.GoToState(this, "Wide", false);
            }
        }

        public int Index { get; set; }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                _wide = ActualWidth > 900;
                VisualStateManager.GoToState(this, _wide ? "Wide" : "Narrow", false);
                var scrollViewer =
                    VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(DetailsListView, 0), 0) as ScrollViewer;
                scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            }
            catch (Exception)
            {
                //
            }
        }
     
        private void ButtonRecomDetails_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModelLocator.GeneralMain
                .Navigate(PageIndex.PageAnimeDetails,
                    new AnimeDetailsPageNavigationArgs(_data.RecommendationId, _data.RecommendationTitle,
                        _data.AnimeRecommendationData, null,
                        new RecommendationPageNavigationArgs {Index = Index}) {Source = PageIndex.PageRecomendations});
        }

        private void ButtonDependentDetails_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModelLocator.GeneralMain
                .Navigate(PageIndex.PageAnimeDetails,
                    new AnimeDetailsPageNavigationArgs(_data.DependentId, _data.DependentTitle,
                        _data.AnimeDependentData, null,
                        new RecommendationPageNavigationArgs {Index = Index}) {Source = PageIndex.PageRecomendations});
        }
    }
}