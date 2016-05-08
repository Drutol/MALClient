using System;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.Store;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;
using MALClient.UserControls;
using MALClient.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private bool _initialized;

        public SettingsPageViewModel ViewModel => DataContext as SettingsPageViewModel;

        public SettingsPage()
        {
            InitializeComponent();
            ListTodo.ItemsSource = new ObservableCollection<string>
            {
                "Add non image live tiles with stats and such. Overhaul tiles in general.",
                "Want something? Let me know! Scroll just a little bit and go to the issues board :)"
            };
        }

        private async void Donate(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = sender as MenuFlyoutItem;
                await CurrentApp.RequestProductPurchaseAsync(btn.Name, false);
            }
            catch (Exception)
            {
                // no donation
            }
        }

        private async void LaunchIssues(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/Mordonus/MALClient/issues"));
        }

        private async void LaunchRepo(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/Mordonus/MALClient"));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ToggleCache.IsOn = Settings.IsCachingEnabled;
            ComboCachePersistency.SelectedIndex = SecondsToIndexHelper(Settings.CachePersitence);
            SetSortOrder();
            BtnDescending.IsChecked = Settings.IsSortDescending;
            BtnMDescending.IsChecked = Settings.IsMangaSortDescending;
            SetDesiredStatus();
            SliderSetup();
            ToggleSwitchSetup();
            ComboThemes.SelectedIndex = (int) Settings.SelectedTheme;
            TxtThemeChangeNotice.Visibility = Settings.SelectedTheme != Application.Current.RequestedTheme
                ? Visibility.Visible
                : Visibility.Collapsed;
            if (Settings.DefaultMenuTab == "anime")
                RadioTabAnime.IsChecked = true;
            else
                RadioTabManga.IsChecked = true;
            Utils.GetMainPageInstance().CurrentStatus = $"Settings - {Utils.GetAppVersion()}";
            _initialized = true;

            NavMgr.RegisterBackNav(PageIndex.PageAnimeList, null);

            base.OnNavigatedTo(e);
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            NavMgr.DeregisterBackNav();
        }


        private void SetSortOrder()
        {
            switch (Settings.AnimeSortOrder)
            {
                case SortOptions.SortNothing:
                    Sort4.IsChecked = true;
                    break;
                case SortOptions.SortTitle:
                    Sort1.IsChecked = true;
                    break;
                case SortOptions.SortScore:
                    Sort2.IsChecked = true;
                    break;
                case SortOptions.SortWatched:
                    Sort3.IsChecked = true;
                    break;
                case SortOptions.SortAirDay:
                    Sort5.IsChecked = true;
                    break;
                default:
                    Sort4.IsChecked = true;
                    break;
            }
            switch (Settings.MangaSortOrder)
            {
                case SortOptions.SortNothing:
                    SortM4.IsChecked = true;
                    break;
                case SortOptions.SortTitle:
                    SortM1.IsChecked = true;
                    break;
                case SortOptions.SortScore:
                    SortM2.IsChecked = true;
                    break;
                case SortOptions.SortWatched:
                    SortM3.IsChecked = true;
                    break;
                default:
                    SortM4.IsChecked = true;
                    break;
            }
        }

        private bool _entriesPopulated;
        private async void PopulateCachedEntries()
        {
            if(_entriesPopulated)
                return;
            _entriesPopulated = true;
            var files = await ApplicationData.Current.LocalFolder.GetFilesAsync();
            foreach (var file in files)
            {
                if (file.FileType == ".json")
                {
                    ListCurrentlyCached.Items.Add(new CachedEntryItem(file, file.DisplayName.Contains("anime")));
                }
            }
            if (files.Count == 0)
                ListEmptyNotice.Visibility = Visibility.Visible;
            else
                ListEmptyNotice.Visibility = Visibility.Collapsed;
            try
            {
                var folder = await ApplicationData.Current.LocalFolder.GetFolderAsync("AnimeDetails");
                var data = await folder.GetFilesAsync();
                TxtRemoveAllDetails.Text += " (" + data.Count + " files)";
            }
            catch (Exception)
            {
                //No folder
                BtnRemoveAllDetails.Visibility = Visibility.Collapsed;
            }
        }


        /// <summary>
        ///     Converts seconds to combo box item index.
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
            Settings.CachePersitence = IndexToSecondsHelper(cmb.SelectedIndex);
        }

        private void ToggleDataCaching(object sender, RoutedEventArgs e)
        {
            if (!_initialized)
                return;
            Settings.IsCachingEnabled = ToggleCache.IsOn;
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
            SortOptions sortOptions;
            switch (btn.Text)
            {
                case "Title":
                    sortOptions = SortOptions.SortTitle;
                    break;
                case "Score":
                    sortOptions = SortOptions.SortScore;
                    break;
                case "Watched":
                    sortOptions = SortOptions.SortWatched;
                    break;
                case "Soonest airing":
                    sortOptions = SortOptions.SortAirDay;
                    break;
                default:
                    sortOptions = SortOptions.SortNothing;
                    break;
            }
            Settings.AnimeSortOrder = sortOptions;
        }

        private void ChangeSortOrder(object sender, RoutedEventArgs e)
        {
            var btn = sender as ToggleMenuFlyoutItem;
            Settings.IsSortDescending = btn.IsChecked;
        }

        private void ChangeDefaultFilter(object sender, SelectionChangedEventArgs e)
        {
            if (!_initialized)
                return;
            Settings.DefaultAnimeFilter =
                Utils.StatusToInt((string) ((sender as ComboBox).SelectedItem as ComboBoxItem).Content);
        }

        private void SetDesiredStatus()
        {
            var value = Settings.DefaultAnimeFilter;
            value = value == 6 || value == 7 ? value - 1 : value;
            value--;
            CmbDefaultFilter.SelectedIndex = value;

            value = Settings.DefaultMangaFilter;
            value = value == 6 || value == 7 ? value - 1 : value;
            value--;
            CmbDefaultMFilter.SelectedIndex = value;
        }

        private void ChangeItemsPerPage(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (!_initialized || Math.Abs(e.NewValue - e.OldValue) < 1)
                return;
            Settings.ItemsPerPage = (int) e.NewValue;
            ViewModelLocator.AnimeList.UpdatePageSetup(true);
        }

        private void ChangedReviewsToPull(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (!_initialized || Math.Abs(e.NewValue - e.OldValue) < 1)
                return;
            Settings.ReviewsToPull = (int) e.NewValue;
        }

        private void ChangedRecommsToPull(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (!_initialized || Math.Abs(e.NewValue - e.OldValue) < 1)
                return;
            Settings.RecommsToPull = (int) e.NewValue;
        }


        private void ChangedSeasonalToPull(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (!_initialized || Math.Abs(e.NewValue - e.OldValue) < 1)
                return;
            Settings.SeasonalToPull = (int) e.NewValue;
        }

        private void SliderSetup()
        {
            SliderItemsPerPage.Value = Settings.ItemsPerPage;
            SliderReccommsToPull.Value = Settings.RecommsToPull;
            SliderReviewsToPull.Value = Settings.ReviewsToPull;
            SliderSeasonalToPull.Value = Settings.SeasonalToPull;
        }

        private void ToggleSwitchSetup()
        {
            ToggleSwitchDetails.IsOn = Settings.DetailsAutoLoadDetails;
            ToggleSwitchReviews.IsOn = Settings.DetailsAutoLoadReviews;
            ToggleSwitchRecomm.IsOn = Settings.DetailsAutoLoadRecomms;
            ToggleSwitchRelated.IsOn = Settings.DetailsAutoLoadRelated;
        }

        private async void RemoveAllAnimeDetails(object sender, RoutedEventArgs e)
        {
            try
            {
                await (await ApplicationData.Current.LocalFolder.GetFolderAsync("AnimeDetails")).DeleteAsync(
                    StorageDeleteOption.PermanentDelete);

                (sender as Button).IsEnabled = false;
            }
            catch (Exception)
            {
                //
            }
        }

        private void SelectMSortOrder(object sender, RoutedEventArgs e)
        {
            var btn = sender as ToggleMenuFlyoutItem;
            SortM1.IsChecked = false;
            SortM2.IsChecked = false;
            SortM3.IsChecked = false;
            SortM4.IsChecked = false;
            btn.IsChecked = true;
            SortOptions sortOptions;
            switch (btn.Text)
            {
                case "Title":
                    sortOptions = SortOptions.SortTitle;
                    break;
                case "Score":
                    sortOptions = SortOptions.SortScore;
                    break;
                case "Read":
                    sortOptions = SortOptions.SortWatched;
                    break;
                default:
                    sortOptions = SortOptions.SortNothing;
                    break;
            }
            Settings.MangaSortOrder = sortOptions;
        }

        private void ChangeMSortOrder(object sender, RoutedEventArgs e)
        {
            var btn = sender as ToggleMenuFlyoutItem;
            Settings.IsMangaSortDescending = btn.IsChecked;
        }

        private void ChangeDefaultMFilter(object sender, SelectionChangedEventArgs e)
        {
            if (!_initialized)
                return;
            Settings.DefaultMangaFilter =
                Utils.StatusToInt((string) ((sender as ComboBox).SelectedItem as ComboBoxItem).Content);
        }

        private void ChangeDefaultTab(object sender, RoutedEventArgs e)
        {
            var radio = sender as RadioButton;
            Settings.DefaultMenuTab = radio.Tag as string;
        }

        private void ToggleSwitchDetailsAutoLoadChange(object sender, RoutedEventArgs e)
        {
            var btn = sender as ToggleSwitch;
            switch ((string) btn.Tag)
            {
                case "0":
                    Settings.DetailsAutoLoadDetails = btn.IsOn;
                    break;
                case "1":
                    Settings.DetailsAutoLoadReviews = btn.IsOn;
                    break;
                case "2":
                    Settings.DetailsAutoLoadRecomms = btn.IsOn;
                    break;
                case "3":
                    Settings.DetailsAutoLoadRelated = btn.IsOn;
                    break;
            }
        }

        private void ChangeTheme(object sender, SelectionChangedEventArgs e)
        {
            Settings.SelectedTheme = (ApplicationTheme) ComboThemes.SelectedIndex;
            TxtThemeChangeNotice.Visibility = Settings.SelectedTheme != Application.Current.RequestedTheme
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void Pivot_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if((sender as Pivot).SelectedIndex == 1)
                PopulateCachedEntries();
            else if ((sender as Pivot).SelectedIndex == 4)
                ViewModel.LoadNews();
        }
    }
}