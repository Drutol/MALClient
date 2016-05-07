using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Input;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MALClient.Items;
using MALClient.UserControls;
using MALClient.ViewModels;
using WinRTXamlToolkit.AwaitableUI;
using WinRTXamlToolkit.Controls.Extensions;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages
{
    public enum AnimeListWorkModes
    {
        Anime,
        SeasonalAnime,
        Manga,
        TopAnime,
        TopManga
    }

    public class AnimeListPageNavigationArgs
    {
        public readonly int CurrPage;
        public readonly bool Descending;
        public readonly string ListSource;
        public readonly bool NavArgs;
        public readonly int Status;
        public readonly int? StatusIndex;
        public AnimeSeason CurrSeason;
        public AnimeListDisplayModes DisplayMode;
        public SortOptions SortOption;
        public AnimeListWorkModes WorkMode = AnimeListWorkModes.Anime;

        public AnimeListPageNavigationArgs(SortOptions sort, int status, bool desc, int page,
            AnimeListWorkModes seasonal, string source, AnimeSeason season, AnimeListDisplayModes dispMode)
        {
            SortOption = sort;
            Status = status;
            Descending = desc;
            CurrPage = page;
            WorkMode = seasonal;
            ListSource = source;
            NavArgs = true;
            CurrSeason = season;
            DisplayMode = dispMode;
        }

        private AnimeListPageNavigationArgs()
        {
        }

        public AnimeListPageNavigationArgs(int index, AnimeListWorkModes workMode)
        {
            WorkMode = workMode;
            StatusIndex = index;
        }

        public static AnimeListPageNavigationArgs Seasonal
            => new AnimeListPageNavigationArgs {WorkMode = AnimeListWorkModes.SeasonalAnime};

        public static AnimeListPageNavigationArgs Manga
            => new AnimeListPageNavigationArgs {WorkMode = AnimeListWorkModes.Manga};

        public static AnimeListPageNavigationArgs TopAnime
            => new AnimeListPageNavigationArgs {WorkMode = AnimeListWorkModes.TopAnime};

        public static AnimeListPageNavigationArgs TopManga
            => new AnimeListPageNavigationArgs {WorkMode = AnimeListWorkModes.TopManga};
    }

    public enum SortOptions
    {
        SortTitle,
        SortScore,
        SortWatched,
        SortAirDay,
        SortNothing
    }

    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AnimeListPage : Page
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

        public async Task<ScrollViewer> GetIndefiniteScrollViewer()
        {
            if (!_loaded)
            {
                int retries = 5;
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

        public Flyout FlyoutViews => ViewsFlyout;
        public Flyout FlyoutFilters => FiltersFlyout;
        public MenuFlyout FlyoutSorting => SortingFlyout;

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

        #region Init

        private double _prevWidth;
        private double _prevHeight;
        private bool _loaded;
        public AnimeListPage()
        {
            InitializeComponent();
            ViewModel.View = this;
            Loaded += (sender, args) =>
            {
                ViewModel.CanAddScrollHandler = true;              
                _loaded = true;
            };

            SizeChanged += (sender, args) =>
            {
                if (Math.Abs(args.NewSize.Height - _prevHeight) > 100 || Math.Abs(args.NewSize.Width - _prevWidth) > 200)
                {
                    //if(ViewModelLocator.Main.OffContentVisibility == Visibility.Visible)
                        //ViewModelLocator.Main.View.InitSplitter();
                    _prevHeight = args.NewSize.Height;
                    _prevWidth = args.NewSize.Width;
                    if ((DataContext as AnimeListViewModel).AreThereItemsWaitingForLoad)
                    {
                        ViewModelLocator.AnimeList.RefreshList();
                    }
                }
                ViewModel.UpdateGridItemWidth();
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.Init(e.Parameter as AnimeListPageNavigationArgs);
        }

        #endregion

        #region ActionHandlersPin

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

        private void SetListSource(object sender, RoutedEventArgs e)
        {
            ListSource_OnKeyDown(null, null);
        }

        private void FlyoutListSource_OnOpened(object sender, object e)
        {
            TxtListSource.SelectAll();
        }

        internal void InitSortOptions(SortOptions option, bool descending)
        {
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
                default:
                    throw new ArgumentOutOfRangeException(nameof(option), option, null);
            }
            BtnOrderDescending.IsChecked = descending;
        }

        #endregion

        private void AnimesItemsIndefinite_OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ItemFlyoutService.ShowAnimeListItemFlyout(e.OriginalSource as FrameworkElement);
            e.Handled = true;
        }

        private void AnimesGridIndefinite_OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ItemFlyoutService.ShowAnimeGridItemFlyout(e.OriginalSource as FrameworkElement);
            e.Handled = true;
        }

        private void AnimeCompactItemsIndefinite_OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ItemFlyoutService.ShowAnimeListItemFlyout(e.OriginalSource as FrameworkElement);
            e.Handled = true;
        }

        public void ResetSelectionForMode(AnimeListDisplayModes currMode)
        {
            try
            {
                switch (currMode)
                {
                    case AnimeListDisplayModes.IndefiniteList:
                        AnimesItemsIndefinite.SelectedItem = null;
                        break;
                    case AnimeListDisplayModes.IndefiniteGrid:
                        AnimesGridIndefinite.SelectedItem = null;
                        break;
                    case AnimeListDisplayModes.IndefiniteCompactList:
                        AnimeCompactItemsIndefinite.SelectedItem = null;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(currMode), currMode, null);
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

        private void AnimesItemsIndefinite_OnItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.TemporarilySelectedAnimeItem = e.ClickedItem as AnimeItemViewModel;
        }

        private void AnimesGridIndefinite_OnItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.TemporarilySelectedAnimeItem = e.ClickedItem as AnimeItemViewModel;
        }
    }
}