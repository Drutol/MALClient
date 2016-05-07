using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MALClient.Comm;
using MALClient.Items;
using MALClient.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HummingbirdProfilePage : Page
    {
        public HummingbirdProfilePageViewModel ViewModel => DataContext as HummingbirdProfilePageViewModel;

        public HummingbirdProfilePage()
        {
            this.InitializeComponent();
            Loaded += (sender, args) =>
            {
                ViewModel.Init();
            };
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if(ViewModel.CurrentData.website != null)
                await Launcher.LaunchUriAsync(new Uri(ViewModel.CurrentData.website as string));
        }

        private async void NavDetailsFeed(object sender, TappedRoutedEventArgs e)
        {
            int id = (int) (sender as FrameworkElement).Tag;
            if (ViewModelLocator.AnimeDetails.Id == id)
                return;
            await ViewModelLocator.Main
                .Navigate(PageIndex.PageAnimeDetails,
                    new AnimeDetailsPageNavigationArgs(id, "", null, null)
                    {Source = PageIndex.PageProfile, AnimeMode = true});
        }

        private void FavouritesNavDetails(object sender, ItemClickEventArgs e)
        {
            (e.ClickedItem as AnimeItemViewModel).NavigateDetails();
        }

        private void IgnorePivotScroll(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = false;
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
