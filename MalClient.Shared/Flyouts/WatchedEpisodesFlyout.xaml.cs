using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using MALClient.XShared.ViewModels;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.Shared.Flyouts
{
    public sealed partial class WatchedEpisodesFlyout : FlyoutPresenter
    {
        AnimeItemViewModel ViewModel => DataContext as AnimeItemViewModel;

        public WatchedEpisodesFlyout()
        {
            InitializeComponent();
        }

        public async void ShowAt(FrameworkElement target)
        {
            DataContext = target.DataContext;
            WatchedEpsFlyout.ShowAt(target);

            var numbers = new List<int>();
            int i = ViewModel.MyEpisodesFocused, j = ViewModel.MyEpisodesFocused - 1, k = 0;
            for (; k < 10; i++ , j--, k++)
            {
                if (ViewModel.AllEpisodes == 0 || i <= ViewModel.AllEpisodesFocused)
                    numbers.Add(i);
                if (j >= 0)
                    numbers.Add(j);
            }
            QuickSelectionGrid.ItemsSource = numbers.OrderBy(i1 => i1).Select(i1 => i1.ToString());
            await Task.Delay(100);
            QuickSelectionGrid.SelectedItem = ViewModel.MyEpisodesFocused.ToString();
            QuickSelectionGrid.ScrollIntoView(QuickSelectionGrid.SelectedItem);


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
            QuickSelectionGrid.ItemsSource = null;
        }

        private void BtnSubmitOnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.OnFlyoutEpsKeyDown.Execute(null);
            WatchedEpsFlyout.Hide();
        }

        private void QuickSelectionGrid_OnItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.WatchedEpsInput = e.ClickedItem as string;
            ViewModel.OnFlyoutEpsKeyDown.Execute(null);
            WatchedEpsFlyout.Hide();
        }
    }
}