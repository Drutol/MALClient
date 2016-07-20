using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using MalClient.Shared.Utils.Managers;
using MalClient.Shared.ViewModels;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MalClient.Shared.Items
{
    public sealed partial class AnimeItem : UserControl
    {
        public AnimeItem()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        public static readonly DependencyProperty DisplayContextProperty =
            DependencyProperty.Register("DisplayContext", typeof(AnimeItemDisplayContext), typeof(AnimeItem),
                new PropertyMetadata(AnimeItemDisplayContext.AirDay));

        public AnimeItemDisplayContext DisplayContext
        {
            get { return (AnimeItemDisplayContext)GetValue(DisplayContextProperty); }
            set { SetValue(DisplayContextProperty, value); }
        }

        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (DataContext == null)
                return;
            ViewModel.AnimeItemDisplayContext = DisplayContext;
            Bindings.Update();
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

    }
}