using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using MALClient.ViewModels;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

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
            Loaded += (sender, args) => ViewModel.Init();
        }

        private HummingbirdProfilePageViewModel ViewModel => DataContext as HummingbirdProfilePageViewModel;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            MobileViewModelLocator.Main.CurrentStatus = $"{Credentials.UserName} - Profile";
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CurrentData.website != null)
                ResourceLocator.SystemControlsLauncherService.LaunchUri(new Uri(ViewModel.CurrentData.website as string));
        }

        private void NavDetailsFeed(object sender, TappedRoutedEventArgs e)
        {
            var id = (int) (sender as FrameworkElement).Tag;
            if (ViewModelLocator.AnimeDetails.Id == id)
                return;
            MobileViewModelLocator.Main
                .Navigate(PageIndex.PageAnimeDetails,
                    new AnimeDetailsPageNavigationArgs(id, "", null, null)
                    {Source = PageIndex.PageProfile, AnimeMode = true});
        }

        private void FavouritesNavDetails(object sender, ItemClickEventArgs e)
        {
            (e.ClickedItem as AnimeItemViewModel).NavigateDetails(PageIndex.PageProfile, new ProfilePageNavigationArgs());
        }
    }
}