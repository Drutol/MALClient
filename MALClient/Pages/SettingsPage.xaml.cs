using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;
using MALClient.UserControls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private bool _initialized = false;
        public SettingsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ToggleCache.IsOn = Utils.IsCachingEnabled();
            ComboCachePersistency.SelectedIndex = SecondsToIndexHelper(Utils.GetCachePersitence());
            SetSortOrder();
            BtnDescending.IsChecked = Utils.IsSortDescending();
            PopulateCachedEntries();
            SetDesiredStatus();
            SetItemsPerPage();
            Utils.GetMainPageInstance()?.SetStatus("Settings");
            _initialized = true;

            Utils.RegisterBackNav(PageIndex.PageAnimeList,null);

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            Utils.DeregisterBackNav();
        }


        private void SetSortOrder()
        {
            switch (Utils.GetSortOrder())
            {
                case AnimeListPage.SortOptions.SortNothing:
                    Sort4.IsChecked = true;
                    break;
                case AnimeListPage.SortOptions.SortTitle:
                    Sort1.IsChecked = true;
                    break;
                case AnimeListPage.SortOptions.SortScore:
                    Sort2.IsChecked = true;
                    break;
                case AnimeListPage.SortOptions.SortWatched:
                    Sort3.IsChecked = true;
                    break;
                case AnimeListPage.SortOptions.SortAirDay:
                    Sort5.IsChecked = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        private async void PopulateCachedEntries()
        {
            var files = await ApplicationData.Current.LocalFolder.GetFilesAsync();
            foreach (var file in files)
            {
                if (file.DisplayName.Contains("anime_data"))
                {
                    ListCurrentlyCached.Items.Add(new CachedEntryItem(file));
                }
            }
            if (files.Count == 0)
                ListEmptyNotice.Visibility = Visibility.Visible;
            else
            {
                ListEmptyNotice.Visibility = Visibility.Collapsed;
            }
        }


        /// <summary>
        /// Converts seconds to combo box item index.
        /// </summary>
        /// <returns></returns>
        private int SecondsToIndexHelper(int secs)
        {
            switch (secs)
            {
                case 600: //10m
                    return 0;
                case 3600: //1h
                    return 1;
                case 7200: //2h
                    return 2;
                case 10800: //3h
                    return 3;
                case 18000: //5h
                    return 4;
                case 36000: //10h
                    return 5;
                case 86400: //1d
                    return 6;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private int IndexToSecondsHelper(int index)
        {
            switch (index)
            {
                case 0: //10m
                    return 600;
                case 1: //1h
                    return 3600;
                case 2: //2h
                    return 7200;
                case 3: //3h
                    return 10800;
                case 4: //5h
                    return 18000;
                case 5: //10h
                    return 36000;
                case 6: //1d
                    return 86400;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ChangeCachePersistency(object sender, SelectionChangedEventArgs e)
        {
            if (!_initialized)
                return;
            ;
            var cmb = sender as ComboBox;
            ApplicationData.Current.LocalSettings.Values["CachePersistency"] = IndexToSecondsHelper(cmb.SelectedIndex);
        }

        private void ToggleDataCaching(object sender, RoutedEventArgs e)
        {
            if (!_initialized)
                return;
            ApplicationData.Current.LocalSettings.Values["EnableCache"] = ToggleCache.IsOn;
        }

        private void SelectSortOrder(object sender, RoutedEventArgs e)
        {
            var btn = sender as ToggleMenuFlyoutItem;
            Sort1.IsChecked = false;
            Sort2.IsChecked = false;
            Sort3.IsChecked = false;
            Sort4.IsChecked = false;
            Sort5.IsChecked = false;
            btn.IsChecked = true;
            AnimeListPage.SortOptions sortOptions;
            switch (btn.Text)
            {
                case "Title":
                    sortOptions = AnimeListPage.SortOptions.SortTitle;
                    break;
                case "MyScore":
                    sortOptions = AnimeListPage.SortOptions.SortScore;
                    break;
                case "Watched":
                    sortOptions = AnimeListPage.SortOptions.SortWatched;
                    break;
                case "Soonest airing":
                    sortOptions = AnimeListPage.SortOptions.SortAirDay;
                    break;
                default:
                    sortOptions = AnimeListPage.SortOptions.SortNothing;
                    break;
            }
            ApplicationData.Current.LocalSettings.Values["SortOrder"] = (int) sortOptions;
        }

        private void ChangeSortOrder(object sender, RoutedEventArgs e)
        {
            var btn = sender as ToggleMenuFlyoutItem;
            ApplicationData.Current.LocalSettings.Values["SortDescending"] = btn.IsChecked;
        }

        private void ChangeDefaultFilter(object sender, SelectionChangedEventArgs e)
        {
            if (!_initialized)
                return;
            ApplicationData.Current.LocalSettings.Values["DefaultFilter"] = Utils.StatusToInt((string)((sender as ComboBox).SelectedItem as ComboBoxItem).Content);
        }

        private void SetDesiredStatus()
        {
            int value = Utils.GetDefaultAnimeFilter();
            value = (value == 6 || value == 7) ? value - 1 : value;
            value--;
            CmbDefaultFilter.SelectedIndex = value;
        }

        private void ChangeItemsPerPage(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (!_initialized || Math.Abs(e.NewValue - e.OldValue) < 1 )
                return;
            ApplicationData.Current.LocalSettings.Values["ItemsPerPage"] = (int)e.NewValue;
        }

        private void SetItemsPerPage()
        {
            SliderItemsPerPage.Value = Utils.GetItemsPerPage();
        }
    }
}
