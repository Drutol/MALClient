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
        SortNothing,
        SortTitle,
        SortScore,
        SortWatched,
        SortAirDay
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
            var args = e.Parameter as AnimeListPageNavigationArgs;
           
        }

        #endregion

        #region UIHelpers

        private void SwitchSortingToSeasonal()
        {
            sort3.Text = "Index";
        }

        private void SwitchFiltersToSeasonal()
        {
            (StatusSelector.Items[5] as ListViewItem).Content = "Airing"; //We are quite confiddent here
        }

        private async void UpdateStatus()
        {
            await
                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () => { UpdateNotice.Text = GetLastUpdatedStatus(); });
        }

        private void SetDefaults()
        {
            SetSortOrder(null);
            SetDesiredStatus(null);
            BtnOrderDescending.IsChecked = Utils.IsSortDescending();
            SortDescending = Utils.IsSortDescending();
        }

        internal void ScrollTo(AnimeItem animeItem)
        {
            try
            {
                var scrollViewer = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(Animes, 0), 0) as ScrollViewer;
                var offset = _animeItems.TakeWhile(t => animeItem != t).Sum(t => t.ActualHeight);
                scrollViewer.ScrollToVerticalOffset(offset);
            }
            catch (Exception)
            {
                // ehh
            }
        }









        #endregion

       



        

        #region ActionHandlers

        private void ChangeListStatus(object sender, SelectionChangedEventArgs e)
        {
            if (!_loaded) return;
            CurrentPage = 1;
            RefreshList();
        }

        private async void PinTileMal(object sender, RoutedEventArgs e)
        {
            foreach (object item in Animes.SelectedItems)
            {
                var anime = item as AnimeItem;
                if (SecondaryTile.Exists(anime.Id.ToString()))
                {
                    var msg = new MessageDialog("Tile for this anime already exists.");
                    await msg.ShowAsync();
                    continue;
                }
                anime.PinTile($"http://www.myanimelist.net/anime/{anime.Id}");
            }
        }

        private void PinTileCustom(object sender, RoutedEventArgs e)
        {
            var item = Animes.SelectedItem as AnimeItem;
            item.OpenTileUrlInput();
        }

        private async void RefreshList(object sender, RoutedEventArgs e)
        {
            if (_seasonalState)
                await FetchSeasonalData(true);
            else
                await FetchData(true);
        }

        private void SelectSortMode(object sender, RoutedEventArgs e)
        {
            var btn = sender as ToggleMenuFlyoutItem;
            switch (btn.Text)
            {
                case "Title":
                    SortOption = SortOptions.SortTitle;
                    break;
                case "Score":
                    SortOption = SortOptions.SortScore;
                    break;
                case "Watched":
                    SortOption = SortOptions.SortWatched;
                    break;
                case "Soonest airing":
                    SortOption = SortOptions.SortAirDay;
                    break;
                default:
                    SortOption = SortOptions.SortNothing;
                    break;
            }
            sort1.IsChecked = false;
            sort2.IsChecked = false;
            sort3.IsChecked = false;
            sort4.IsChecked = false;
            sort5.IsChecked = false;
            btn.IsChecked = true;
            RefreshList();
        }



        private void ChangeSortOrder(object sender, RoutedEventArgs e)
        {
            var chbox = sender as ToggleMenuFlyoutItem;
            SortDescending = chbox.IsChecked;
            RefreshList();
        }

        private async void ListSource_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if ((sender == null && e == null) || e.Key == VirtualKey.Enter)
            {
                if (_currentSoure != null &&
                    !string.Equals(_currentSoure, Creditentials.UserName, StringComparison.CurrentCultureIgnoreCase))
                    Utils.GetMainPageInstance().PurgeUserCache(_currentSoure);
                //why would we want to keep those entries?
                _currentSoure = TxtListSource.Text;
                TxtListSource.IsEnabled = false; //reset input
                TxtListSource.IsEnabled = true;
                FlyoutListSource.Hide();
                BottomCommandBar.IsOpen = false;
                await FetchData();
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

        #endregion
    }
}