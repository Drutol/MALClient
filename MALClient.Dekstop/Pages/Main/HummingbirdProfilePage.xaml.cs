using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using MalClient.Shared.NavArgs;
using MalClient.Shared.Utils.Enums;
using MalClient.Shared.ViewModels;
using MalClient.Shared.ViewModels.Main;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages.Main
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HummingbirdProfilePage : Page
    {
        public HummingbirdProfilePage()
        {
            InitializeComponent();
            Loaded += (sender, args) => { ViewModel.Init(); };
        }

        public HummingbirdProfilePageViewModel ViewModel => DataContext as HummingbirdProfilePageViewModel;

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CurrentData.website != null)
                await Launcher.LaunchUriAsync(new Uri(ViewModel.CurrentData.website as string));
        }

        private void NavDetailsFeed(object sender, TappedRoutedEventArgs e)
        {
            var id = (int) (sender as FrameworkElement).Tag;
            if (ViewModelLocator.AnimeDetails.Id == id)
                return;
            ViewModelLocator.GeneralMain
                .Navigate(PageIndex.PageAnimeDetails,
                    new AnimeDetailsPageNavigationArgs(id, "", null, null)
                    {Source = PageIndex.PageProfile, AnimeMode = true});
        }

        private void FavouritesNavDetails(object sender, ItemClickEventArgs e)
        {
            (e.ClickedItem as AnimeItemViewModel).NavigateDetails();
        }


        private void Pivot_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as Pivot).SelectedIndex == 0)
            {
                FeedRecent.Visibility = Visibility.Visible;
                FeedPosts.Visibility = Visibility.Collapsed;
            }
            else
            {
                FeedRecent.Visibility = Visibility.Collapsed;
                FeedPosts.Visibility = Visibility.Visible;
            }
        }
    }
}