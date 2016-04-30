using Windows.Foundation;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using MALClient.ViewModels;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.Items
{
    public sealed partial class AnimeItem : UserControl
    {
        public AnimeItemViewModel ViewModel => DataContext as AnimeItemViewModel;

        public AnimeItem(AnimeItemViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
      
        public void ClearImage()
        {
            Image.Source = null;
        }

        public void BindImage()
        {
            if (Image.Source != null)
                return;
            var bnd = new Binding { Source = ViewModel.Image };
            Image.SetBinding(Image.SourceProperty, bnd);
        }

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

        private void CloseTileUrlInput(object sender, RoutedEventArgs e)
        {        
            ViewModel.TileUrlInputVisibility = Visibility.Collapsed;
        }

        #endregion

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
    }
}