using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MALClient.Utils.Enums;
using MALClient.Utils.Managers;
using MALClient.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CalendarPage : Page
    {
        public CalendarPage()
        {
            this.InitializeComponent();
            Loaded += (sender, args) => NavMgr.RegisterBackNav(PageIndex.PageAnimeList, null);
            Loaded += (a1, a2) => (DataContext as CalendarPageViewModel).Init();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            NavMgr.DeregisterBackNav();
            base.OnNavigatingFrom(e);
        }

        private void ItemsViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        {
            (e.ClickedItem as AnimeItemViewModel).NavigateDetails();
        }

        private void UIElement_OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            (sender as FrameworkElement).Opacity = 1;
        }

        private void UIElement_OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            (sender as FrameworkElement).Opacity = .5;
        }

        private void AnimesGridIndefinite_OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if((e.OriginalSource as FrameworkElement).DataContext is AnimeItemViewModel)
                ItemFlyoutService.ShowAnimeGridItemFlyout((FrameworkElement)e.OriginalSource);
                e.Handled = true;
        }
    }
}
