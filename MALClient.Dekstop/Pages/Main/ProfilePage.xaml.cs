using System.ComponentModel;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using MALClient.Models.Enums;
using MALClient.Models.Models;
using MALClient.UWP.Shared.Items;
using MALClient.UWP.Shared.Managers;
using MALClient.UWP.ViewModels;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;
using ProfilePageNavigationArgs = MALClient.XShared.NavArgs.ProfilePageNavigationArgs;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.UWP.Pages.Main
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
            ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
        }

        private ProfilePageViewModel ViewModel => DataContext as ProfilePageViewModel;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _lastArgs = e.Parameter as ProfilePageNavigationArgs;
            ViewModel.LoadProfileData(_lastArgs);

            base.OnNavigatedTo(e);
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == nameof(ViewModel.AboutMeWebViewVisibility))
            {
                ToggleAboutMeButton.IsChecked = ViewModel.AboutMeWebViewVisibility;
            }
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

        private void FavCharacter_OnClick(object sender, TappedRoutedEventArgs e)
        {
            ViewModelLocator.ProfilePage.NavigateCharacterDetailsCommand.Execute(
                ((sender as FrameworkElement).DataContext as FavouriteViewModel).Data);
        }


        private void FavCharacterOnClick(object sender, ItemClickEventArgs e)
        {
            ViewModelLocator.ProfilePage.NavigateCharacterDetailsCommand.Execute(
                         (e.ClickedItem as FavouriteViewModel).Data);
        }

        private void FavPersonOnClick(object sender, ItemClickEventArgs e)
        {
            ViewModelLocator.ProfilePage.NavigateStaffDetailsCommand.Execute(
                         (e.ClickedItem as FavouriteViewModel).Data);
        }

        private void FavCharacter_OnRightClick(object sender, RightTappedRoutedEventArgs e)
        {
            var grid = sender as IItemWithFlyout;
            grid?.ShowFlyout();
        }



        private void OnAnimeItemClick(object sender, ItemClickEventArgs e)
        {
            DesktopViewModelLocator.ProfilePage.TemporarilySelectedAnimeItem = e.ClickedItem as AnimeItemViewModel;
        }


        private void AboutMeWebView_OnNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            if(args.Uri == null)
                return;
            args.Cancel = true;
            var arg = MalLinkParser.GetNavigationParametersForUrl(args.Uri.ToString());
            if (arg == null)
                ResourceLocator.SystemControlsLauncherService.LaunchUri(args.Uri);
            else
            {
                if(ViewModelLocator.Mobile || !arg.Item1.GetAttribute<EnumUtilities.PageIndexEnumMember>().OffPage)
                    ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageProfile,_lastArgs);
                ViewModelLocator.GeneralMain.Navigate(arg.Item1,arg.Item2);
            }
        }

        private void SearchBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (string.IsNullOrEmpty(GoToInputBox.Text))
                return;
            GotoFlyout.Hide();
            ViewModelLocator.NavMgr.RegisterBackNav(DesktopViewModelLocator.ProfilePage.PrevArgs);
            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageProfile, new ProfilePageNavigationArgs { TargetUser = GoToInputBox.Text });
            GoToInputBox.Text = "";
        }

        private void GoToInputBox_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                if (string.IsNullOrEmpty(GoToInputBox.Text))
                    return;
                GotoFlyout.Hide();
                ViewModelLocator.NavMgr.RegisterBackNav(DesktopViewModelLocator.ProfilePage.PrevArgs);
                ViewModelLocator.GeneralMain.Navigate(PageIndex.PageProfile, new ProfilePageNavigationArgs { TargetUser = GoToInputBox.Text });
                GoToInputBox.Text = "";
                e.Handled = true;
            }
        }

        private void AboutMeWebView_OnContentLoading(WebView sender, WebViewContentLoadingEventArgs args)
        {
            AboutMeLoader.Visibility = Visibility.Visible;
        }


        private void AboutMeWebView_OnNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            AboutMeLoader.Visibility = Visibility.Collapsed;
        }
    }
}