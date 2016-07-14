using System;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Store;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using MALClient.Utils;
using MALClient.ViewModels;

#pragma warning disable 4014

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.UserControls
{
    public enum HamburgerButtons
    {
        AnimeList,
        AnimeSearch,
        LogIn,
        Settings,
        Profile,
        Seasonal,
        About,
        Recommendations,
        MangaList,
        MangaSearch,
        TopAnime,
        TopManga,
        Calendar,
        Articles,
        News,
        Messanging
    }

    public sealed partial class HamburgerControl : UserControl
    {
        public HamburgerControl()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            if(Settings.EnableHearthAnimation)
                SupportMeStoryboard.Begin();
        }

        private HamburgerControlViewModel ViewModel => (HamburgerControlViewModel) DataContext;


        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            ViewModel.UpdateProfileImg();
            ViewModel.SetActiveButton(Credentials.Authenticated
                ? (Settings.DefaultMenuTab == "anime" ? HamburgerButtons.AnimeList : HamburgerButtons.MangaList)
                : HamburgerButtons.LogIn);

            FeedbackImage.Source = Settings.SelectedTheme == ApplicationTheme.Dark
                ? new BitmapImage(new Uri("ms-appx:///Assets/GitHub-Mark-Light-120px-plus.png"))
                : new BitmapImage(new Uri("ms-appx:///Assets/GitHub-Mark-120px-plus.png"));
        }
    
        private async void Donate(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = sender as MenuFlyoutItem;
                await CurrentApp.RequestProductPurchaseAsync(btn.Tag as string, false);
                Settings.Donated = true;
            }
            catch (Exception)
            {
                // no donation
            }
        }

        private async void OpenRepo(object sender, RoutedEventArgs e)
        {
            Utilities.TelemetryTrackEvent(TelemetryTrackedEvents.LaunchedFeedback);
            await Launcher.LaunchUriAsync(new Uri("https://github.com/Mordonus/MALClient/issues"));
        }

    }
}