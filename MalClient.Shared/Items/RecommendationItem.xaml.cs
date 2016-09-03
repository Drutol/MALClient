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

namespace MALClient.Shared.Items
{
    public sealed partial class RecommendationItem : UserControl
    {
        private RecommendationData _data;

        RecommendationItemViewModel ViewModel => DataContext as RecommendationItemViewModel;

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
            if (_wide && sizeChangedEventArgs.NewSize.Width < 1000)
            {
                _wide = false;
                VisualStateManager.GoToState(this, "Narrow",false);
            }
            else if(!_wide && sizeChangedEventArgs.NewSize.Width > 1000)
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
                _wide = ActualWidth > 1000;
                VisualStateManager.GoToState(this, _wide ? "Wide" : "Narrow", false);
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
                    new AnimeDetailsPageNavigationArgs(ViewModel.Data.RecommendationId, ViewModel.Data.RecommendationTitle,
                       ViewModel.Data.AnimeRecommendationData, null,
                        new RecommendationPageNavigationArgs {Index = Index}) {Source = PageIndex.PageRecomendations});
        }

        private void ButtonDependentDetails_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModelLocator.GeneralMain
                .Navigate(PageIndex.PageAnimeDetails,
                    new AnimeDetailsPageNavigationArgs(ViewModel.Data.DependentId, ViewModel.Data.DependentTitle,
                        ViewModel.Data.AnimeDependentData, null,
                        new RecommendationPageNavigationArgs {Index = Index}) {Source = PageIndex.PageRecomendations});
        }
    }
}