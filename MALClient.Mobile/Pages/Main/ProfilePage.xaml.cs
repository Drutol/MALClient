using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using MalClient.Shared.Items;
using MALClient.Models.Models;
using MALClient.ViewModels;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

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

        public ProfilePageViewModel ViewModel => DataContext as ProfilePageViewModel;

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            (DataContext as ProfilePageViewModel).LoadProfileData(_lastArgs);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _lastArgs = e.Parameter as ProfilePageNavigationArgs;
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        private void NavigateProfile(object sender, ItemClickEventArgs e)
        {
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageProfile,new ProfilePageNavigationArgs {TargetUser = ViewModel.CurrentData.User.Name},PageIndex.PageProfile);
            MobileViewModelLocator.Main.Navigate(PageIndex.PageProfile, new ProfilePageNavigationArgs { TargetUser = (e.ClickedItem as MalUser).Name });
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageProfile, new ProfilePageNavigationArgs { TargetUser = ViewModel.CurrentData.User.Name },PageIndex.PageProfile);
            MobileViewModelLocator.Main.Navigate(PageIndex.PageProfile, new ProfilePageNavigationArgs { TargetUser = (string)(sender as Button).Tag });
        }

        private void FavCharacter_OnClick(object sender, TappedRoutedEventArgs e)
        {
            ViewModelLocator.ProfilePage.NavigateCharacterDetailsCommand.Execute(
                ((sender as FrameworkElement).DataContext as FavouriteViewModel).Data);
        }

        private void FavPerson_OnClick(object sender, TappedRoutedEventArgs e)
        {
            ViewModelLocator.ProfilePage.NavigateStaffDetailsCommand.Execute(
                ((sender as FrameworkElement).DataContext as FavouriteViewModel).Data);
        }


        private void FavCharacter_OnRightClick(object sender, RightTappedRoutedEventArgs e)
        {
            var grid = sender as IItemWithFlyout;
            grid?.ShowFlyout();
        }

        private void ButtonGotoProfileOnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(GotoUserName.Text))
                return;
            GotoFlyout.Hide();
            ViewModelLocator.NavMgr.RegisterBackNav(MobileViewModelLocator.ProfilePage.PrevArgs);
            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageProfile, new ProfilePageNavigationArgs { TargetUser = GotoUserName.Text });
            GotoUserName.Text = "";
        }

        private void GotoUserName_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                if (string.IsNullOrEmpty(GotoUserName.Text))
                    return;
                GotoFlyout.Hide();
                ViewModelLocator.NavMgr.RegisterBackNav(MobileViewModelLocator.ProfilePage.PrevArgs);
                ViewModelLocator.GeneralMain.Navigate(PageIndex.PageProfile, new ProfilePageNavigationArgs { TargetUser = GotoUserName.Text });
                GotoUserName.Text = "";
                e.Handled = true;
            }
        }
    }
}