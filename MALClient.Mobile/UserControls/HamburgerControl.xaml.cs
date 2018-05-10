using System;
using Windows.ApplicationModel.Store;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using MALClient.Models.Enums;
using MALClient.UWP.ViewModels;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using Microsoft.Services.Store.Engagement;

#pragma warning disable 4014

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.UWP.UserControls
{ 

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

            //FeedbackImage.Source = Settings.SelectedTheme == (int)ApplicationTheme.Dark
            //    ? new BitmapImage(new Uri("ms-appx:///Assets/GitHub-Mark-Light-120px-plus.png"))
            //    : new BitmapImage(new Uri("ms-appx:///Assets/GitHub-Mark-120px-plus.png"));
        }
    
        private void Donate(object sender, RoutedEventArgs e)
        {
            ResourceLocator.SystemControlsLauncherService.LaunchUri(new Uri("https://www.paypal.me/drutol"));
            //try
            //{
            //    var btn = sender as MenuFlyoutItem;
            //    await CurrentApp.RequestProductPurchaseAsync(btn.Tag as string);
            //    Settings.Donated = true;
            //}
            //catch (Exception)
            //{
            //    // no donation
            //}
        }

        private async void OpenRepo(object sender, RoutedEventArgs e)
        {
            ResourceLocator.TelemetryProvider.TelemetryTrackEvent(TelemetryTrackedEvents.LaunchedFeedback);
            ResourceLocator.SystemControlsLauncherService.LaunchUri(new Uri("https://github.com/Drutol/MALClient/issues"));
        }

        private void BtnProfile_OnClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel.PinnedProfiles.Count > 0)
                FlyoutBase.GetAttachedFlyout(BtnProfile).ShowAt(BtnProfile);
        }

        private void PinnedProfilesOnClick(object sender, ItemClickEventArgs e)
        {
            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageProfile,
                new ProfilePageNavigationArgs { TargetUser = e.ClickedItem as string });
        }

        private async void FeedbackButton_OnClick(object sender, RoutedEventArgs e)
        {
            await StoreServicesFeedbackLauncher.GetDefault().LaunchAsync();
        }

        private async void BtnFeedbackEmailOnClick(object sender, RoutedEventArgs e)
        {
            var emailMessage = new Windows.ApplicationModel.Email.EmailMessage();
            var emailRecipient = new Windows.ApplicationModel.Email.EmailRecipient("malclient@poczta.fm");
            emailMessage.Subject = "[MALClient] - ";
            emailMessage.To.Add(emailRecipient);
            await Windows.ApplicationModel.Email.EmailManager.ShowComposeNewEmailAsync(emailMessage);
        }
    }
}