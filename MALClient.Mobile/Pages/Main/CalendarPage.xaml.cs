using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using MalClient.Shared.Utils.Enums;
using MalClient.Shared.Utils.Managers;
using MalClient.Shared.ViewModels;
using MalClient.Shared.ViewModels.Main;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages.Main
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CalendarPage : Page
    {
        public CalendarPage()
        {
            this.InitializeComponent();
            Loaded += (sender, args) => ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageAnimeList, null);
            Loaded += (a1, a2) => (DataContext as CalendarPageViewModel).Init();
        }


        private void ItemsViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        {
            (e.ClickedItem as AnimeItemViewModel).NavigateDetails();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModelLocator.NavMgr.ResetMainBackNav();
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageAnimeList,null);
            base.OnNavigatedTo(e);
        }

        private void AnimesGridIndefinite_OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if((e.OriginalSource as FrameworkElement).DataContext is AnimeItemViewModel)
                ItemFlyoutService.ShowAnimeGridItemFlyout((FrameworkElement)e.OriginalSource);
                e.Handled = true;
        }
    }
}
