using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using MALClient.ViewModels;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.Items
{
    public sealed partial class AnimeGridItem : UserControl, IAnimeItemInteractions
    {
        public AnimeGridItem(AnimeItemViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
            vm.ViewGrid = this;
        }

        public AnimeItemViewModel ViewModel => DataContext as AnimeItemViewModel;

        public Flyout WatchedFlyout => null;//WatchedEpsFlyout;
        public object MoreFlyout => null;//FlyoutMore;

        private void SubmitWatchedEps(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                ViewModel.ChangeWatchedEps();
                e.Handled = true;
            }
        }
        public void MoreFlyoutHide()
        {
           // FlyoutMore.Hide();
        }

        private void ShowWatchedFlyour(object sender, RoutedEventArgs e)
        {
            //WatchedEpsFlyout.ShowAt(WatchedFlyoutButton);
        }

    }
}