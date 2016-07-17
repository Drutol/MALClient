using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using MalClient.Shared.Utils.Managers;
using MalClient.Shared.ViewModels;
using MalClient.Shared.ViewModels.Main;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages.Main
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CalendarPage : Page
    {
        public CalendarPage()
        {
            InitializeComponent();
            Loaded += (a1, a2) => (DataContext as CalendarPageViewModel).Init();
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
            if ((e.OriginalSource as FrameworkElement).DataContext is AnimeItemViewModel)
                ItemFlyoutService.ShowAnimeGridItemFlyout((FrameworkElement) e.OriginalSource);
            e.Handled = true;
        }
    }
}