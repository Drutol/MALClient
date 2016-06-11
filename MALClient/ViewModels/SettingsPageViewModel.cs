using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Comm;
using MALClient.Models;
using MALClient.Pages;
using MALClient.Pages.SettingsPages;
using MALClient.UserControls;
using Newtonsoft.Json;

namespace MALClient.ViewModels
{
    public class SettingsPageEntry
    {
        public string Header { get; set; }
        public string Subtitle { get; set; }
        public Symbol Symbol { get; set; }
        public Type PageType { get; set; }
    }

    public class SettingsPageViewModel : ViewModelBase
    {
        private bool _newsLoaded;
        private ICommand _reviewCommand;



        public ICommand ReviewCommand => _reviewCommand ?? (_reviewCommand = new RelayCommand(async () =>
        {
            Settings.RatePopUpEnable = false;
            await
                Launcher.LaunchUriAsync(
                    new Uri($"ms-windows-store:REVIEW?PFN={Package.Current.Id.FamilyName}"));
        }));
        public event SettingsNavigationRequest NavigationRequest;
        private ICommand _requestNavigationCommand;

        public ICommand RequestNavigationCommand
            => _requestNavigationCommand ?? (_requestNavigationCommand = new RelayCommand<Type>(page =>
            {
                NavigationRequest?.Invoke(page);
                if (page != typeof(SettingsHomePage))
                    NavMgr.RegisterOneTimeOverride(new RelayCommand(() =>
                    {
                        NavigationRequest?.Invoke(typeof(SettingsHomePage));
                    }));
            }));

        public ObservableCollection<Tuple<AnimeListDisplayModes, string>> DisplayModes { get; } = new ObservableCollection
            <Tuple<AnimeListDisplayModes, string>>
        {
            new Tuple<AnimeListDisplayModes, string>(AnimeListDisplayModes.IndefiniteList, ":ist"),
            new Tuple<AnimeListDisplayModes, string>(AnimeListDisplayModes.IndefiniteGrid, "Grid"),
        };

        public List<SettingsPageEntry> SettingsPages { get; } = new List<SettingsPageEntry>
        {
            new SettingsPageEntry {Header = "General",Subtitle = "Default filters, theme etc.",Symbol = Symbol.Setting,PageType = typeof(SettingsGeneralPage)},
            new SettingsPageEntry {Header = "Caching",Subtitle = "Cached data and caching options.",Symbol = Symbol.SaveLocal,PageType = typeof(SettingsCachingPage)},
            new SettingsPageEntry {Header = "Calendar",Subtitle = "Build options, behaviours etc.",Symbol = Symbol.CalendarWeek,PageType = typeof(SettingsCalendarPage)},
            new SettingsPageEntry {Header = "News",Subtitle = "News regarding app development, bugs etc.",Symbol = Symbol.PostUpdate,PageType = typeof(SettingsNewsPage)},
            new SettingsPageEntry {Header = "About",Subtitle = "Github repo, donations etc.",Symbol = Symbol.Manage,PageType = typeof(SettingsAboutPage)},
            new SettingsPageEntry {Header = "Miscellaneous",Subtitle = "Review popup settings...",Symbol = Symbol.Placeholder,PageType = typeof(SettingsMiscPage)}
        };

        public Tuple<AnimeListDisplayModes, string> SelectedDefaultViewForWatching
        {
            get { return DisplayModes[(int)Settings.WatchingDisplayMode]; }
            set { Settings.WatchingDisplayMode = value.Item1; }
        }

        public Tuple<AnimeListDisplayModes, string> SelectedDefaultViewForCompleted
        {
            get { return DisplayModes[(int)Settings.CompletedDisplayMode]; }
            set { Settings.CompletedDisplayMode = value.Item1; }
        }

        public Tuple<AnimeListDisplayModes, string> SelectedDefaultViewForOnHold
        {
            get { return DisplayModes[(int)Settings.OnHoldDisplayMode]; }
            set { Settings.OnHoldDisplayMode = value.Item1; }
        }

        public Tuple<AnimeListDisplayModes, string> SelectedDefaultViewForDropped
        {
            get { return DisplayModes[(int)Settings.DroppedDisplayMode]; }
            set { Settings.DroppedDisplayMode = value.Item1; }
        }

        public Tuple<AnimeListDisplayModes, string> SelectedDefaultViewForPlanned
        {
            get { return DisplayModes[(int)Settings.PlannedDisplayMode]; }
            set { Settings.PlannedDisplayMode = value.Item1; }
        }

        public Tuple<AnimeListDisplayModes, string> SelectedDefaultViewForAll
        {
            get { return DisplayModes[(int)Settings.AllDisplayMode]; }
            set { Settings.AllDisplayMode = value.Item1; }
        }

        public bool LockDisplayMode
        {
            get { return Settings.LockDisplayMode; }
            set { Settings.LockDisplayMode = value; }
        }

        public bool HideFilterSelectionFlyout
        {
            get { return Settings.HideFilterSelectionFlyout; }
            set { Settings.HideFilterSelectionFlyout = value; }
        }

        public bool HideViewSelectionFlyout
        {
            get { return Settings.HideViewSelectionFlyout; }
            set { Settings.HideViewSelectionFlyout = value; }
        }

        public bool HideSortingSelectionFlyout
        {
            get { return Settings.HideSortingSelectionFlyout; }
            set { Settings.HideSortingSelectionFlyout = value; }
        }

        public bool RatePopUpEnable
        {
            get { return Settings.RatePopUpEnable; }
            set
            {
                Settings.RatePopUpEnable = value;
                RaisePropertyChanged(() => RatePopUpEnable);
            }
        }

        public bool EnableHearthAnimation
        {
            get { return Settings.EnableHearthAnimation; }
            set { Settings.EnableHearthAnimation = value; }
        }

        public int RatePopUpStartupCounter
        {
            get { return RateReminderPopUp.LaunchThresholdValue - Settings.RatePopUpStartupCounter; }
        }

        public int AirDayOffset
        {
            get { return Settings.AirDayOffset; }
            set { Settings.AirDayOffset = value; }
        }

        public bool DataSourceAnn
        {
            get { return Settings.PrefferedDataSource == DataSource.Ann; }
            set
            {
                if (value) Settings.PrefferedDataSource = DataSource.Ann;
            }
        }

        public bool DataSourceHum
        {
            get { return Settings.PrefferedDataSource == DataSource.Hummingbird; }
            set
            {
                if (value) Settings.PrefferedDataSource = DataSource.Hummingbird;
            }
        }

        public bool DataSourceAnnHum
        {
            get { return Settings.PrefferedDataSource == DataSource.AnnHum; }
            set
            {
                if (value) Settings.PrefferedDataSource = DataSource.AnnHum;
            }
        }

        public static bool SetStartDateOnWatching
        {
            get { return Settings.SetStartDateOnWatching; }
            set { Settings.SetStartDateOnWatching = value; }
        }

        public static bool SetStartDateOnListAdd
        {
            get { return Settings.SetStartDateOnListAdd; }
            set { Settings.SetStartDateOnListAdd = value; }
        }

        public static bool SetEndDateOnDropped
        {
            get { return Settings.SetEndDateOnDropped; }
            set { Settings.SetEndDateOnDropped = value; }
        }

        public static bool SetEndDateOnCompleted
        {
            get { return Settings.SetEndDateOnCompleted; }
            set { Settings.SetEndDateOnCompleted = value; }
        }

        public static bool OverrideValidStartEndDate
        {
            get { return Settings.OverrideValidStartEndDate; }
            set { Settings.OverrideValidStartEndDate = value; }
        }

        public static bool CalendarIncludeWatching
        {
            get { return Settings.CalendarIncludeWatching; }
            set { Settings.CalendarIncludeWatching = value; }
        }

        public static bool CalendarIncludePlanned
        {
            get { return Settings.CalendarIncludePlanned; }
            set { Settings.CalendarIncludePlanned = value; }
        }

        public static bool IsCachingEnabled
        {
            get { return Settings.IsCachingEnabled; }
            set { Settings.IsCachingEnabled = value; }
        }

        public static bool CalendarStartOnToday
        {
            get { return Settings.CalendarStartOnToday; }
            set { Settings.CalendarStartOnToday = value; }
        }

        public static bool CalendarRemoveEmptyDays
        {
            get { return Settings.CalendarRemoveEmptyDays; }
            set { Settings.CalendarRemoveEmptyDays = value; }
        }

        public static bool CalendarStartOnSummary => !Settings.CalendarStartOnToday;

        public static bool CalendarSwitchMonSun
        {
            get { return !Settings.CalendarSwitchMonSun; }
            set { Settings.CalendarSwitchMonSun = !value; }
        }

        public static bool ArticlesLaunchExternalLinks
        {
            get { return Settings.ArticlesLaunchExternalLinks; }
            set { Settings.ArticlesLaunchExternalLinks = value; }
        }

        private List<NewsData> _currentNews { get; set; } = new List<NewsData>();
        public List<NewsData> CurrentNews
        {
            get
            {
                LoadNews();
                return _currentNews;
            }
            set
            {
                _currentNews = value;
                RaisePropertyChanged(() => CurrentNews);
            }
        }


        public Visibility MalApiDependatedntSectionsVisibility
            => Settings.SelectedApiType == ApiType.Mal ? Visibility.Visible : Visibility.Collapsed;

        public bool HumApiDependatedntSectionsEnabled
             => Settings.SelectedApiType != ApiType.Mal;



        public async void LoadNews()
        {
            if (_newsLoaded)
                return;
            _newsLoaded = true;

            var data = new List<NewsData>();
            try
            {
                await
                    Task.Run(
                        async () =>
                            data =
                                JsonConvert.DeserializeObject<List<NewsData>>(
                                    await new NewsQuery().GetRequestResponse(false)));
            }
            catch (Exception)
            {
                return;
            }

            CurrentNews = data;
        }

        private bool _cachedItemsLoaded;
        public async void LoadCachedEntries()
        {
            if (_cachedItemsLoaded)
                return;
            _cachedItemsLoaded = true;
            var files = await ApplicationData.Current.LocalFolder.GetFilesAsync();
            foreach (var file in files)
            {
                if (file.FileType == ".json")
                {
                    var data = await file.GetBasicPropertiesAsync();
                    CachedEntries.Add(new CachedEntryModel
                    {
                        Date = data.DateModified.LocalDateTime.ToString("dd/MM/yyyy HH:mm"),
                        FileName = file.Name,
                        Size = Utils.SizeSuffix((long)data.Size),
                    });
                }
            }
            EmptyCachedListVisiblity = files.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            try
            {
                var folder = await ApplicationData.Current.LocalFolder.GetFolderAsync("AnimeDetails");
                var data = await folder.GetFilesAsync();
                TotalFilesCached = $"Remove all anime details data({data.Count}files)";
                RemoveAllCachedDataButtonVisibility = Visibility.Visible;
            }
            catch (Exception)
            {
                //No folder yet
                RemoveAllCachedDataButtonVisibility = Visibility.Collapsed;
            }
        }

        #region RecentlyMovedToMvvm

        private ObservableCollection<CachedEntryModel> _cachedEntries = new ObservableCollection<CachedEntryModel>();

        public ObservableCollection<CachedEntryModel> CachedEntries
        {
            get
            {
                LoadCachedEntries();
                return _cachedEntries;
            }
        }

        private Visibility _emptyCachedListVisiblity = Visibility.Collapsed;

        public Visibility EmptyCachedListVisiblity
        {
            get { return _emptyCachedListVisiblity; }
            set
            {
                _emptyCachedListVisiblity = value;
                RaisePropertyChanged(() => EmptyCachedListVisiblity);
            }
        }

        private Visibility _removeAllCachedDataButtonVisibility = Visibility.Collapsed;

        public Visibility RemoveAllCachedDataButtonVisibility
        {
            get { return _removeAllCachedDataButtonVisibility; }
            set
            {
                _removeAllCachedDataButtonVisibility = value;
                RaisePropertyChanged(() => RemoveAllCachedDataButtonVisibility);
            }
        }

        private string _totalFilesCached = "N/A";

        public string TotalFilesCached
        {
            get { return _totalFilesCached; }
            set
            {
                _totalFilesCached = value;
                RaisePropertyChanged(() => TotalFilesCached);
            }
        }





        #endregion

        public SettingsPageViewModel()
        {
            CachedEntries.CollectionChanged += (sender, args) =>
            {
                if ((sender as ObservableCollection<CachedEntryModel>).Count == 0)
                {
                    EmptyCachedListVisiblity = Visibility.Visible;
                }
            };
        }
    }
}