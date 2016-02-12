using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel.Core;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MALClient.Comm;
using MALClient.Items;
using MALClient.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages
{
    public class AnimeListPageNavigationArgs
    {
        public readonly int CurrPage;
        public readonly bool Descending;
        public readonly string ListSource;
        public readonly bool LoadSeasonal;
        public readonly bool NavArgs;
        public readonly int Status;
        public SortOptions SortOption;

        public AnimeListPageNavigationArgs(SortOptions sort, int status, bool desc, int page,
            bool seasonal, string source)
        {
            SortOption = sort;
            Status = status;
            Descending = desc;
            CurrPage = page;
            LoadSeasonal = seasonal;
            ListSource = source;
            NavArgs = true;
        }

        public AnimeListPageNavigationArgs()
        {
            LoadSeasonal = true;
        }
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
        private bool _loaded;
        private AnimeListViewModel ViewModel => (DataContext as AnimeListViewModel);
        #region Init

        public AnimeListPage()
        {
            InitializeComponent();
            ViewModel.View = this;
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _loaded = true;
            try
            {
                var scrollViewer = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(Animes, 0), 0) as ScrollViewer;
                scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                ViewModel.UpdateUpperStatus();
            }
            catch (Exception)
            {
                //ignored
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.Init(e.Parameter as AnimeListPageNavigationArgs);           
        }

        #endregion

        #region UIHelpers

        internal void ScrollTo(AnimeItem animeItem)
        {
            try
            {
                var scrollViewer = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(Animes, 0), 0) as ScrollViewer;
                var offset = ViewModel._animeItems.TakeWhile(t => animeItem != t).Sum(t => t.ActualHeight);
                scrollViewer.ScrollToVerticalOffset(offset);
            }
            catch (Exception)
            {
                // ehh
            }
        }

        #endregion

       



        

        #region ActionHandlers

        private async void PinTileMal(object sender, RoutedEventArgs e)
        {
            foreach (object item in Animes.SelectedItems)
            {
                var anime = item as AnimeItem;
                if (SecondaryTile.Exists(anime.ViewModel.Id.ToString()))
                {
                    var msg = new MessageDialog("Tile for this anime already exists.");
                    await msg.ShowAsync();
                    continue;
                }
                anime.ViewModel.PinTile($"http://www.myanimelist.net/anime/{anime.ViewModel.Id}");
            }
        }

        private void PinTileCustom(object sender, RoutedEventArgs e)
        {
            var item = Animes.SelectedItem as AnimeItem;
            item.OpenTileUrlInput();
        }

        private void SelectSortMode(object sender, RoutedEventArgs e)
        {
            var btn = sender as ToggleMenuFlyoutItem;
            switch (btn.Text)
            {
                case "Title":
                    ViewModel.SortOption = SortOptions.SortTitle;
                    break;
                case "Score":
                    ViewModel.SortOption = SortOptions.SortScore;
                    break;
                case "Watched":
                    ViewModel.SortOption = SortOptions.SortWatched;
                    break;
                case "Soonest airing":
                    ViewModel.SortOption = SortOptions.SortAirDay;
                    break;
                default:
                    ViewModel.SortOption = SortOptions.SortNothing;
                    break;
            }
            foreach (var child in SortToggles.Children)
            {
                (child as ToggleMenuFlyoutItem).IsChecked = false;
            }
            btn.IsChecked = true;
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

        private void Animes_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AppbarBtnPinTile.IsEnabled = true;
        }

        internal void InitSortOptions(SortOptions option,bool descending)
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

        public void DisablePinButton()
        {
            AppbarBtnPinTile.IsEnabled = false;
        }
    }
}