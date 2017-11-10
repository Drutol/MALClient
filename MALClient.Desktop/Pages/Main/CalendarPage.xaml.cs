﻿using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using MALClient.UWP.Shared.Managers;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.UWP.Pages.Main
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CalendarPage : Page
    {
        public CalendarPage()
        {
            InitializeComponent();
            Loaded += (a1, a2) =>
            {
                (DataContext as CalendarPageViewModel).Init();
                ViewModelLocator.CalendarPage.PivotSelectedIndexChange += CalendarPageOnPivotSelectedIndexChange;
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModelLocator.NavMgr.ResetMainBackNav();
            base.OnNavigatedTo(e);
        }

        private void CalendarPageOnPivotSelectedIndexChange()
        {
            RootPivot.SelectedIndex = ViewModelLocator.CalendarPage.CalendarPivotIndex;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ViewModelLocator.CalendarPage.PivotSelectedIndexChange -= CalendarPageOnPivotSelectedIndexChange;
        }

        private void ItemsViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        {
            (e.ClickedItem as AnimeItemViewModel).NavigateDetails();
        }

        private void AnimesGridIndefinite_OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is AnimeItemViewModel)
                ItemFlyoutService.ShowAnimeGridItemFlyout((FrameworkElement) e.OriginalSource);
            e.Handled = true;
        }
    }
}