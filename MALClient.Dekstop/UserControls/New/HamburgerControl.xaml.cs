using System;
using Windows.ApplicationModel.Store;
using Windows.System;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using MALClient.Models.Enums;
using MALClient.Utils.Managers;
using MALClient.ViewModels;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.ViewModels;
using Microsoft.Services.Store.Engagement;
using Settings = MALClient.XShared.Utils.Settings;

#pragma warning disable 4014

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.UserControls.New
{
    public sealed partial class HamburgerControl : UserControl
    {
        private static readonly Brush _b2 =
            new SolidColorBrush(Application.Current.RequestedTheme == ApplicationTheme.Dark
                ? Color.FromArgb(220, 50, 50, 50)
                : Color.FromArgb(220, 190, 190, 190));

        private bool _animeFiltersExpanded;
        private bool _mangaFiltersExpanded;
        private bool _topCategoriesExpanded;

        public HamburgerControl()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            if (Settings.EnableHearthAnimation)
                SupportMeStoryboard.Begin();
        }

        private HamburgerControlViewModel ViewModel => (HamburgerControlViewModel) DataContext;

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            ViewModel.UpdateProfileImg();
            if (Settings.HamburgerMenuDefaultPaneState && ApplicationView.GetForCurrentView().VisibleBounds.Width > 500)
            {
                if (Settings.HamburgerAnimeFiltersExpanded)
                    ButtonExpandAnimeFiltersOnClick(null, null);
                if (Settings.HamburgerMangaFiltersExpanded)
                    ButtonExpandMangaFiltersOnClick(null, null);
                if (Settings.HamburgerTopCategoriesExpanded)
                    ButtonExpandTopCategoriesOnClick(null, null);
            }

            FeedbackImage.Source = Settings.SelectedTheme == (int)ApplicationTheme.Dark
                ? new BitmapImage(new Uri("ms-appx:///Assets/GitHub-Mark-Light-120px-plus.png"))
                : new BitmapImage(new Uri("ms-appx:///Assets/GitHub-Mark-120px-plus.png"));
        }

        private void Button_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            (sender as Button).Background = _b2;
        }

        private void Button_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            (sender as Button).Background = new SolidColorBrush(Colors.Transparent);
        }

        private void ButtonExpandAnimeFiltersOnRightClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel.HamburgerExpanded)
                ButtonExpandAnimeFiltersOnClick(null, null);
            else
                HamburgerFlyoutService.ShowAnimeFiltersFlyout(sender as FrameworkElement);
        }

        private void ButtonExpandAnimeFiltersOnClick(object sender, RoutedEventArgs e)
        {
            if (!_animeFiltersExpanded)
            {
                ExpandAnimeListFiltersStoryboard.Begin();
                RotateAnimeListFiltersStoryboard.Begin();
            }
            else
            {
                CollapseAnimeListFiltersStoryboard.Begin();
                RotateBackAnimeListFiltersStoryboard.Begin();
            }

            _animeFiltersExpanded = !_animeFiltersExpanded;
        }

        private void ButtonExpandMangaFiltersOnRightClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel.HamburgerExpanded)
                ButtonExpandMangaFiltersOnClick(null, null);
            else
                HamburgerFlyoutService.ShowAnimeMangaFiltersFlyout(sender as FrameworkElement);
        }

        private void ButtonExpandMangaFiltersOnClick(object sender, RoutedEventArgs e)
        {
            if (!_mangaFiltersExpanded)
            {
                ExpandMangaListFiltersStoryboard.Begin();
                RotateMangaListFiltersStoryboard.Begin();
            }
            else
            {
                CollapseMangaListFiltersStoryboard.Begin();
                RotateBackMangaListFiltersStoryboard.Begin();
            }

            _mangaFiltersExpanded = !_mangaFiltersExpanded;
        }


        private void ButtonExpandTopCategoriesOnRightClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel.HamburgerExpanded)
                ButtonExpandTopCategoriesOnClick(null, null);
            else
                HamburgerFlyoutService.ShowTopCategoriesFlyout(sender as FrameworkElement);
        }

        private void ButtonExpandTopCategoriesOnClick(object sender, RoutedEventArgs e)
        {
            if (!_topCategoriesExpanded)
            {
                ExpandTopAnimeCategoriesStoryboard.Begin();
                RotateTopAnimeCategoriesStoryboard.Begin();
            }
            else
            {
                CollapseTopAnimeCategoriesStoryboard.Begin();
                RotateBackTopAnimeCategoriesStoryboard.Begin();
            }

            _topCategoriesExpanded = !_topCategoriesExpanded;
        }

        private void HamburgerControl_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width == 250.0) //opened
            {
                ViewModel.HamburgerWidthChanged(true);
                MidSeparator.Width = BottomSeparator.Width = 250;
                Mid2Separator.Width = BottomSeparator.Width = 250;

                //double column
                ButtonImages.SetValue(Grid.RowProperty, 0);
                ButtonImages.SetValue(Grid.ColumnProperty, 1);
                ButtonImages.SetValue(Grid.ColumnSpanProperty, 1);
                ButtonForums.SetValue(Grid.ColumnSpanProperty, 1);

                ButtonNews.SetValue(Grid.RowProperty, 0);
                ButtonNews.SetValue(Grid.ColumnProperty, 1);
                ButtonNews.SetValue(Grid.ColumnSpanProperty, 1);
                ButtonArticles.SetValue(Grid.ColumnSpanProperty, 1);
            }
            else //closed
            {
                ViewModel.HamburgerWidthChanged(false);
                MidSeparator.Width = BottomSeparator.Width = 60;
                Mid2Separator.Width = BottomSeparator.Width = 60;
                if (_topCategoriesExpanded)
                {
                    CollapseTopAnimeCategoriesStoryboard.Begin();
                    RotateBackTopAnimeCategoriesStoryboard.Begin();
                    _topCategoriesExpanded = false;
                }
                if (_animeFiltersExpanded)
                {
                    CollapseAnimeListFiltersStoryboard.Begin();
                    RotateBackAnimeListFiltersStoryboard.Begin();
                    _animeFiltersExpanded = false;
                }
                if (_mangaFiltersExpanded)
                {
                    CollapseMangaListFiltersStoryboard.Begin();
                    RotateBackMangaListFiltersStoryboard.Begin();
                    _mangaFiltersExpanded = false;
                }

                //double column
                ButtonImages.SetValue(Grid.RowProperty, 1);
                ButtonImages.SetValue(Grid.ColumnProperty, 0);
                ButtonImages.SetValue(Grid.ColumnSpanProperty, 2);
                ButtonForums.SetValue(Grid.ColumnSpanProperty,2);

                ButtonNews.SetValue(Grid.RowProperty, 1);
                ButtonNews.SetValue(Grid.ColumnProperty, 0);
                ButtonNews.SetValue(Grid.ColumnSpanProperty, 2);
                ButtonArticles.SetValue(Grid.ColumnSpanProperty, 2);
            }
        }


        private async void Donate(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = sender as MenuFlyoutItem;
                await CurrentApp.RequestProductPurchaseAsync(btn.Tag as string);
                Settings.Donated = true;
            }
            catch (Exception)
            {
                // no donation
            }
        }

        private async void OpenRepo(object sender, RoutedEventArgs e)
        {
            ResourceLocator.TelemetryProvider.TelemetryTrackEvent(TelemetryTrackedEvents.LaunchedFeedback);
            await Launcher.LaunchUriAsync(new Uri("https://github.com/Drutol/MALClient/issues"));
        }

        private void BtnProfile_OnClick(object sender, RoutedEventArgs e)
        {
            if(ViewModel.PinnedProfiles.Count > 0)
                FlyoutBase.GetAttachedFlyout(BtnProfile).ShowAt(BtnProfile);
        }

        private void PinnedProfilesOnClick(object sender, ItemClickEventArgs e)
        {
            if(ViewModelLocator.GeneralMain.CurrentMainPage != PageIndex.PageProfile)
                ViewModelLocator.NavMgr.ResetMainBackNav();
            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageProfile,
                new ProfilePageNavigationArgs {TargetUser = e.ClickedItem as string});
        }

        private async void FeedbackButton_OnClick(object sender, RoutedEventArgs e)
        {
            await Feedback.LaunchFeedbackAsync();
        }
    }
}