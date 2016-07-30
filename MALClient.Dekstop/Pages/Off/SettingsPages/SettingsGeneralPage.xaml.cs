using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MalClient.Shared.Utils;
using MalClient.Shared.Utils.Enums;
using MalClient.Shared.ViewModels;
using Settings = MalClient.Shared.Utils.Settings;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages.Off.SettingsPages
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsGeneralPage : Page
    {
        private bool _initialized;

        public SettingsGeneralPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            SetSortOrder();
            BtnDescending.IsChecked = Settings.IsSortDescending;
            BtnMDescending.IsChecked = Settings.IsMangaSortDescending;
            SetDesiredStatus();
            ToggleSwitchSetup();
            ScrollViewer.Focus(FocusState.Pointer);
            ComboThemes.SelectedIndex = (int) Settings.SelectedTheme;
            TxtThemeChangeNotice.Visibility = Settings.SelectedTheme != Application.Current.RequestedTheme
                ? Visibility.Visible
                : Visibility.Collapsed;
            if (Settings.DefaultMenuTab == "anime")
                RadioTabAnime.IsChecked = true;
            else
                RadioTabManga.IsChecked = true;
            ViewModelLocator.GeneralMain.CurrentOffStatus = $"Settings - {Utilities.GetAppVersion()}";
            _initialized = true;
        }


        private void ToggleSwitchSetup()
        {
            ToggleSwitchDetails.IsOn = Settings.DetailsAutoLoadDetails;
            ToggleSwitchReviews.IsOn = Settings.DetailsAutoLoadReviews;
            ToggleSwitchRecomm.IsOn = Settings.DetailsAutoLoadRecomms;
            ToggleSwitchRelated.IsOn = Settings.DetailsAutoLoadRelated;
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
                Utilities.StatusToInt((string) ((sender as ComboBox).SelectedItem as ComboBoxItem).Content);
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

        private void SelectSortOrder(object sender, RoutedEventArgs e)
        {
            var btn = sender as ToggleMenuFlyoutItem;
            Sort1.IsChecked = false;
            Sort2.IsChecked = false;
            Sort3.IsChecked = false;
            Sort4.IsChecked = false;
            Sort5.IsChecked = false;
            Sort6.IsChecked = false;
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
                case "Last watched":
                    sortOptions = SortOptions.SortLastWatched;
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
                Utilities.StatusToInt((string) ((sender as ComboBox).SelectedItem as ComboBoxItem).Content);
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
    }
}