using Windows.Foundation;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using MALClient.ViewModels;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.Items
{
    public sealed partial class AnimeItem : UserControl
    {
        private bool _expandState;

        public AnimeItem(AnimeItemViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

        public AnimeItem()
        {
            InitializeComponent();
        }

        public AnimeItemViewModel ViewModel => DataContext as AnimeItemViewModel;

        private void BtnWatchedEpsOnClick(object sender, RoutedEventArgs e)
        {
            ItemFlyoutService.ShowWatchedEpisodesFlyout(sender as FrameworkElement);
        }

        private void BtnMoreOnClick(object sender, RoutedEventArgs e)
        {
            ItemFlyoutService.ShowAnimeListItemFlyout(sender as FrameworkElement);
        }

        private void BtnScoreOnClick(object sender, RoutedEventArgs e)
        {
            ItemFlyoutService.ShowAnimeListItemScoreFlyout(sender as FrameworkElement);
        }

        private void BtnStatusOnClick(object sender, RoutedEventArgs e)
        {
            ItemFlyoutService.ShowAnimeListItemStatusFlyout(sender as FrameworkElement);
        }

        #region Swipe

        private Point _initialPoint;
        private bool _manipulating;

        private void ManipStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            _initialPoint = e.Position;
            _manipulating = true;
        }

        private void ManipDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (e.IsInertial && _manipulating)
            {
                var currentpoint = e.Position;
                if (currentpoint.X - _initialPoint.X >= 70) // swipe right
                {
                    e.Complete();
                    e.Handled = true;
                    _manipulating = false;
                    ViewModel.NavigateDetails();
                }
            }
        }

        #endregion

        #region CustomTilePin

        private void TxtTileUrl_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key != VirtualKey.Enter) return;
            var txt = sender as TextBox;
            txt.IsEnabled = false; //reset input
            txt.IsEnabled = true;
            ViewModel.PinTile();
            CloseTileUrlInput(null, null);
        }

        public void OpenTileUrlInput()
        {
            TxtTileUrl.Text = "";
            //Utils.GetMainPageInstance().AnimeListScrollTo(this);
            ViewModel.TileUrlInputVisibility = Visibility.Visible;
            TxtTileUrl.Focus(FocusState.Keyboard);
        }

        private void CloseTileUrlInput(object sender, RoutedEventArgs e)
        {
            ViewModel.TileUrlInputVisibility = Visibility.Collapsed;
        }

        #endregion
    }
}