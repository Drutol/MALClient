using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using MALClient.XShared.ViewModels;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MalClient.Shared.Flyouts
{
    public sealed partial class WatchedEpisodesFlyout : FlyoutPresenter
    {
        public WatchedEpisodesFlyout()
        {
            InitializeComponent();
        }

        public void ShowAt(FrameworkElement target)
        {
            DataContext = target.DataContext;
            WatchedEpsFlyout.ShowAt(target);
        }

        private void TxtBoxWatchedEps_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key != VirtualKey.Enter) return;

            (DataContext as AnimeItemViewModel).OnFlyoutEpsKeyDown.Execute(e);
            WatchedEpsFlyout.Hide();
        }

        private void WatchedEpsFlyout_OnClosed(object sender, object e)
        {
            DataContext = null;
        }

        private void BtnSubmitOnClick(object sender, RoutedEventArgs e)
        {
            WatchedEpsFlyout.Hide();
        }
    }
}