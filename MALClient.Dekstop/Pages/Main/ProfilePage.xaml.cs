using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using MalClient.Shared.Models;
using MalClient.Shared.Utils.Enums;
using MalClient.Shared.Utils.Managers;
using MalClient.Shared.ViewModels;
using MALClient.ViewModels;
using MALClient.ViewModels.Main;
using ProfilePageNavigationArgs = MalClient.Shared.NavArgs.ProfilePageNavigationArgs;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages.Main
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProfilePage : Page
    {
        private static ProfilePageNavigationArgs _lastArgs;

        public ProfilePage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private ProfilePageViewModel ViewModel => DataContext as ProfilePageViewModel;

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            ViewModel.LoadProfileData(_lastArgs);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _lastArgs = e.Parameter as ProfilePageNavigationArgs;
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ViewModelLocator.NavMgr.ResetMainBackNav();
            base.OnNavigatedFrom(e);
        }

        private void ListViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        {
            NavigateProfile((e.ClickedItem as MalUser).Name);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            NavigateProfile((string) (sender as Button).Tag);
        }

        private static void NavigateProfile(string target)
        {
            ViewModelLocator.NavMgr.RegisterBackNav(DesktopViewModelLocator.ProfilePage.PrevArgs);
            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageProfile, new ProfilePageNavigationArgs {TargetUser = target});
        }


        private void AnimesGridIndefinite_OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is AnimeItemViewModel)
                ItemFlyoutService.ShowAnimeGridItemFlyout(e.OriginalSource as FrameworkElement);
            e.Handled = true;
        }

        private void FavCharacter_OnClick(object sender, PointerRoutedEventArgs e)
        {
            var grid = sender as FrameworkElement;
            var flyout = FlyoutBase.GetAttachedFlyout(sender as FrameworkElement);
            flyout.ShowAt(grid);
        }
    }
}