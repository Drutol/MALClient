using System;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using MalClient.Shared.Items;
using MalClient.Shared.Models;
using MalClient.Shared.NavArgs;
using MalClient.Shared.Utils;
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
            ViewModel.OnWebViewNavigationRequest += ViewModelOnOnWebViewNavigationRequest;
        }

        private void ViewModelOnOnWebViewNavigationRequest(string content, bool b)
        {
            var uiSettings = new UISettings();
            var color = uiSettings.GetColorValue(UIColorType.Accent);
            var color1 = uiSettings.GetColorValue(UIColorType.AccentDark2);
            var color2 = uiSettings.GetColorValue(UIColorType.AccentLight2);
            var css = MalArticlesPage.Css.Replace("AccentColourBase", "#" + color.ToString().Substring(3)).
                Replace("AccentColourLight", "#" + color2.ToString().Substring(3)).
                Replace("AccentColourDark", "#" + color1.ToString().Substring(3))
                .Replace("BodyBackgroundThemeColor",
                    Settings.SelectedTheme == ApplicationTheme.Dark ? "#2d2d2d" : "#e6e6e6")
                .Replace("BodyForegroundThemeColor",
                    Settings.SelectedTheme == ApplicationTheme.Dark ? "white" : "black").Replace(
                        "HorizontalSeparatorColor",
                        Settings.SelectedTheme == ApplicationTheme.Dark ? "#0d0d0d" : "#b3b3b3");
            css += "</style>";
            AboutMeWebView.NavigateToString(css + content);
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
            var grid = sender as IItemWithFlyout;
            grid?.ShowFlyout();
        }

        private void ButtonGotoProfileOnClick(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(GotoUserName.Text))
                return;
            GotoFlyout.Hide();
            ViewModelLocator.NavMgr.RegisterBackNav(DesktopViewModelLocator.ProfilePage.PrevArgs);
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
                ViewModelLocator.NavMgr.RegisterBackNav(DesktopViewModelLocator.ProfilePage.PrevArgs);
                ViewModelLocator.GeneralMain.Navigate(PageIndex.PageProfile, new ProfilePageNavigationArgs { TargetUser = GotoUserName.Text });
                GotoUserName.Text = "";
                e.Handled = true;
            }
        }

        private void OnAnimeItemClick(object sender, ItemClickEventArgs e)
        {
            DesktopViewModelLocator.ProfilePage.TemporarilySelectedAnimeItem = e.ClickedItem as AnimeItemViewModel;
        }

        private void AboutMeWebView_OnDOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            ViewModel.AboutMeWebViewVisibility = true;
            ViewModel.LoadingAboutMeVisibility = false;
        }

        private void AboutMeWebView_OnFrameNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            args.Cancel = true;
        }

        private async void AboutMeWebView_OnNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            if(args.Uri == null)
                return;
            args.Cancel = true;
            await Launcher.LaunchUriAsync(args.Uri);
        }
    }
}