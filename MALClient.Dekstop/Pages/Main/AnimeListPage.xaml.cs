using System;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MalClient.Shared.NavArgs;
using MalClient.Shared.Utils;
using MalClient.Shared.Utils.Enums;
using MalClient.Shared.Utils.Managers;
using MalClient.Shared.ViewModels;
using MalClient.Shared.ViewModels.Main;
using MALClient.Utils.Managers;
using MALClient.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages.Main
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AnimeListPage : Page , IAnimeListViewInteractions
    {
        private ScrollViewer _indefiniteScrollViewer;
        public AnimeListViewModel ViewModel => DataContext as AnimeListViewModel;

        public ScrollViewer IndefiniteScrollViewer
        {
            private get
            {
                return _indefiniteScrollViewer ??
                       (_indefiniteScrollViewer =
                           VisualTreeHelper.GetChild(
                               VisualTreeHelper.GetChild((DependencyObject) GetScrollingContainer(), 0), 0) as
                               ScrollViewer);
            }
            set { _indefiniteScrollViewer = value; }
        }

        public Flyout FlyoutViews => ViewsFlyout;
        public Flyout FlyoutFilters => FiltersFlyout;
        public MenuFlyout FlyoutSorting => SortingFlyout;

        private double _prevWidth;
        private double _prevHeight;
        private bool _loaded;
        private AnimeListPageNavigationArgs _navArgs;

        public AnimeListPage()
        {
            InitializeComponent();
            Loaded += (sender, args) =>
            {
                ViewModel.View = this;
                _loaded = true;
                ViewModel.CanAddScrollHandler = true;
                ViewModel.ScrollIntoViewRequested += ViewModelOnScrollRequest;
                ViewModel.SortingSettingChanged += OnSortingSettingChanged;
                ViewModel.SelectionResetRequested += ResetSelectionForMode;
                ViewModel.Init(_navArgs);
                SizeChanged += (s, a) => { ViewModel.UpdateGridItemWidth(a); };
            };

            
            ViewModelLocator.GeneralMain.OffContentPaneStateChanged += MainOnOffContentPaneStateChanged;
        }

        private void MainOnOffContentPaneStateChanged()
        {
            if (Settings.AnimeListEnsureSelectedItemVisibleAfterOffContentCollapse &&
                AnimesGridIndefinite.SelectedItem != null)
                AnimesGridIndefinite.ScrollIntoView(AnimesGridIndefinite.SelectedItem);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _navArgs = e.Parameter as AnimeListPageNavigationArgs;
        }

        public async Task<ScrollViewer> GetIndefiniteScrollViewer()
        {
            if (!_loaded)
            {
                var retries = 15;
                while (retries-- > 0 && !_loaded)
                {
                    await Task.Delay(50);
                }
            }
            try
            {
                if (_loaded)
                    return IndefiniteScrollViewer;
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void FlyoutSeasonSelectionHide()
        {
            FlyoutSeasonSelection.Hide();
        }

        private object GetScrollingContainer()
        {
            switch (ViewModel.DisplayMode)
            {
                case AnimeListDisplayModes.IndefiniteList:
                    return AnimesItemsIndefinite;
                case AnimeListDisplayModes.IndefiniteGrid:
                    return AnimesGridIndefinite;
                case AnimeListDisplayModes.IndefiniteCompactList:
                    return AnimeCompactItemsIndefinite;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void AnimesItemsIndefinite_OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is AnimeItemViewModel)
                ItemFlyoutService.ShowAnimeListItemFlyout(e.OriginalSource as FrameworkElement);
            e.Handled = true;
        }

        private void AnimesGridIndefinite_OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is AnimeItemViewModel)
                ItemFlyoutService.ShowAnimeGridItemFlyout(e.OriginalSource as FrameworkElement);
            e.Handled = true;
        }

        private void AnimeCompactItemsIndefinite_OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).DataContext is AnimeItemViewModel)
                ItemFlyoutService.ShowAnimeListItemFlyout(e.OriginalSource as FrameworkElement);
            e.Handled = true;
        }

        private void ResetSelectionForMode(AnimeListDisplayModes currMode)
        {
            try
            {
                AnimesItemsIndefinite.SelectedItem = null;
                AnimesGridIndefinite.SelectedItem = null;
                AnimeCompactItemsIndefinite.SelectedItem = null;
            }
            catch (Exception)
            {
                //
            }
        }

        private void ViewModelOnScrollRequest(AnimeItemViewModel item)
        {
            try
            {
                switch (ViewModel.DisplayMode)
                {
                    case AnimeListDisplayModes.IndefiniteCompactList:
                        AnimeCompactItemsIndefinite.ScrollIntoView(item);
                        break;
                    case AnimeListDisplayModes.IndefiniteList:
                        AnimesItemsIndefinite.ScrollIntoView(item);
                        break;
                    case AnimeListDisplayModes.IndefiniteGrid:
                        AnimesGridIndefinite.ScrollIntoView(item);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception)
            {
                //
            }
        }

        private void AnimeCompactItemsIndefinite_OnItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.TemporarilySelectedAnimeItem = e.ClickedItem as AnimeItemViewModel;
        }

        private async void AnimesItemsIndefinite_OnItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.TemporarilySelectedAnimeItem = e.ClickedItem as AnimeItemViewModel;
            await Task.Delay(50);
            AnimesItemsIndefinite.ScrollIntoView(e.ClickedItem);
        }

        private async void AnimesGridIndefinite_OnItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.TemporarilySelectedAnimeItem = e.ClickedItem as AnimeItemViewModel;
            await Task.Delay(50);
            AnimesGridIndefinite.ScrollIntoView(e.ClickedItem);
        }


        private void ChangeSortOrder(object sender, RoutedEventArgs e)
        {
            var chbox = sender as ToggleMenuFlyoutItem;
            ViewModel.SortDescending = chbox.IsChecked;
            ViewModel.RefreshList();
        }

        private async void ListSource_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if ((sender == null && e == null) || e.Key == VirtualKey.Enter)
            {
                e.Handled = true;
                TxtListSource.IsEnabled = false; //reset input
                TxtListSource.IsEnabled = true;
                FlyoutListSource.Hide();
                BottomCommandBar.IsOpen = false;
                await ViewModel.FetchData();
            }
        }

        private void ShowListSourceFlyout(object sender, RoutedEventArgs e)
        {
            FlyoutListSource.ShowAt(sender as FrameworkElement);
        }

        private void FlyoutListSource_OnOpened(object sender, object e)
        {
            TxtListSource.SelectAll();
        }

        private void OnSortingSettingChanged(SortOptions option, bool descending)
        {
            SortTitle.IsChecked =
                SortScore.IsChecked =
                    Sort3.IsChecked =
                        SortAiring.IsChecked =
                            SortNone.IsChecked =
                                SortLastWatched.IsChecked =
                                    SortEndDate.IsChecked =
                                        SortStartDate.IsChecked = false;
            switch (option)
            {
                case SortOptions.SortTitle:
                    SortTitle.IsChecked = true;
                    break;
                case SortOptions.SortScore:
                    SortScore.IsChecked = true;
                    break;
                case SortOptions.SortWatched:
                    Sort3.IsChecked = true;
                    break;
                case SortOptions.SortAirDay:
                    SortAiring.IsChecked = true;
                    break;
                case SortOptions.SortNothing:
                    SortNone.IsChecked = true;
                    break;
                case SortOptions.SortLastWatched:
                    SortLastWatched.IsChecked = true;
                    break;
                case SortOptions.SortStartDate:
                    SortStartDate.IsChecked = true;
                    break;
                case SortOptions.SortEndDate:
                    SortEndDate.IsChecked = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(option), option, null);
            }
            BtnOrderDescending.IsChecked = descending;
        }

        private void UpperNavBarPivotOnSelectionChanged(object sender,
            SelectionChangedEventArgs selectionChangedEventArgs)
        {
            ViewModel.StatusSelectorSelectedIndex = UpperNavBarPivot.SelectedIndex;
        }



    }
}