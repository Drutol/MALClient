using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Adapters;
using MALClient.Models.Enums;
using MALClient.Models.Models.Anime;
using MALClient.Models.Models.Misc;
using MALClient.XShared.Comm;
using MALClient.XShared.Comm.Profile;
using MALClient.XShared.Delegates;
using MALClient.XShared.Utils;
using MALClient.XShared.Utils.Enums;
using Newtonsoft.Json;
using Settings = MALClient.XShared.Utils.Settings;

namespace MALClient.XShared.ViewModels
{
    public class SettingsPageEntry
    {
        public string Header { get; set; }
        public string Subtitle { get; set; }
        public SettingsSymbolsEnum Symbol { get; set; }
        public SettingsPageIndex PageType { get; set; }
    }

    public abstract class SettingsViewModelBase : ViewModelBase
    {
        public abstract event SettingsNavigationRequest NavigationRequest;

        protected bool _cachedItemsLoaded;
        private bool _newsLoaded;
        protected ICommand _requestNavigationCommand;
        protected ICommand _reviewCommand;
        private ICommand _syncFavsCommand;

        public SettingsViewModelBase()
        {
            CachedEntries.CollectionChanged += (sender, args) =>
            {
                if (CachedEntries.Count == 0)
                {
                    EmptyCachedListVisiblity = true;
                }
            };
        }


        public abstract ICommand ReviewCommand { get; }

        public abstract ICommand RequestNavigationCommand { get; }

        public ICommand SyncFavsCommand => _syncFavsCommand ?? (_syncFavsCommand = new RelayCommand(async () =>
        {
            IsSyncFavsButtonEnabled = false;
            await new ProfileQuery().GetProfileData(true,true);
            IsSyncFavsButtonEnabled = true;
        }));

        public ObservableCollection<Tuple<AnimeListDisplayModes, string>> DisplayModes { get; } = new ObservableCollection
            <Tuple<AnimeListDisplayModes, string>>
        {
            new Tuple<AnimeListDisplayModes, string>(AnimeListDisplayModes.IndefiniteList, "Detailed Grid"),
            new Tuple<AnimeListDisplayModes, string>(AnimeListDisplayModes.IndefiniteGrid, "Grid"),
            new Tuple<AnimeListDisplayModes, string>(AnimeListDisplayModes.IndefiniteCompactList, "Compact List")
        };

        public List<SettingsPageEntry> SettingsPages { get; } = new List<SettingsPageEntry>
        {
            new SettingsPageEntry
            {
                Header = "General",
                Subtitle = "Default filters, theme etc.",
                Symbol = SettingsSymbolsEnum.Setting,
                PageType = SettingsPageIndex.General
            },
            new SettingsPageEntry
            {
                Header = "Caching",
                Subtitle = "Cached data and caching options.",
                Symbol = SettingsSymbolsEnum.SaveLocal,
                PageType = SettingsPageIndex.Caching
            },
            new SettingsPageEntry
            {
                Header = "Calendar",
                Subtitle = "Build options, behaviours etc.",
                Symbol = SettingsSymbolsEnum.CalendarWeek,
                PageType = SettingsPageIndex.Calendar
            },
            new SettingsPageEntry
            {
                Header = "Articles&news",
                Subtitle = "Article&news view settings and live tiles.",
                Symbol = SettingsSymbolsEnum.PreviewLink,
                PageType = SettingsPageIndex.Articles
            },
            //new SettingsPageEntry
            //{
            //    Header = "News",
            //    Subtitle = "News regarding app development, bugs etc.",
            //    Symbol = SettingsSymbolsEnum.PostUpdate,
            //    PageType = SettingsPageIndex.News
            //},
            new SettingsPageEntry
            {
                Header = "Notifications",
                Subtitle = "Notifications types, pooling frequency...",
                Symbol = SettingsSymbolsEnum.Important,
                PageType = SettingsPageIndex.Notifications
            },
            new SettingsPageEntry
            {
                Header = "About",
                Subtitle = "Github repo, donations etc.",
                Symbol = SettingsSymbolsEnum.Manage,
                PageType = SettingsPageIndex.About
            },
            new SettingsPageEntry
            {
                Header = "Account",
                Subtitle = "MyAnimelist or Hummingbird authentication.",
                Symbol = SettingsSymbolsEnum.Contact,
                PageType = SettingsPageIndex.LogIn
            },
            new SettingsPageEntry
            {
                Header = "Ads",
                Subtitle = "Support me with ads on demand...",
                Symbol = SettingsSymbolsEnum.SwitchApps,
                PageType = SettingsPageIndex.Ads
            },
            new SettingsPageEntry
            {
                Header = "Miscellaneous",
                Subtitle = "Review popup settings...",
                Symbol = SettingsSymbolsEnum.Placeholder,
                PageType = SettingsPageIndex.Misc
            }
        };

        public Tuple<AnimeListDisplayModes, string> SelectedDefaultViewForWatching
        {
            get { return DisplayModes[(int) Settings.WatchingDisplayMode]; }
            set { Settings.WatchingDisplayMode = value.Item1; }
        }

        public Tuple<AnimeListDisplayModes, string> SelectedDefaultViewForCompleted
        {
            get { return DisplayModes[(int) Settings.CompletedDisplayMode]; }
            set { Settings.CompletedDisplayMode = value.Item1; }
        }

        public Tuple<AnimeListDisplayModes, string> SelectedDefaultViewForOnHold
        {
            get { return DisplayModes[(int) Settings.OnHoldDisplayMode]; }
            set { Settings.OnHoldDisplayMode = value.Item1; }
        }

        public Tuple<AnimeListDisplayModes, string> SelectedDefaultViewForDropped
        {
            get { return DisplayModes[(int) Settings.DroppedDisplayMode]; }
            set { Settings.DroppedDisplayMode = value.Item1; }
        }

        public Tuple<AnimeListDisplayModes, string> SelectedDefaultViewForPlanned
        {
            get { return DisplayModes[(int) Settings.PlannedDisplayMode]; }
            set { Settings.PlannedDisplayMode = value.Item1; }
        }

        public Tuple<AnimeListDisplayModes, string> SelectedDefaultViewForAll
        {
            get { return DisplayModes[(int) Settings.AllDisplayMode]; }
            set { Settings.AllDisplayMode = value.Item1; }
        }

        public bool LockDisplayMode
        {
            get { return Settings.LockDisplayMode; }
            set { Settings.LockDisplayMode = value; }
        }

        public bool DisplaySeasonWithType
        {
            get { return Settings.DisplaySeasonWithType; }
            set
            {
                Settings.DisplaySeasonWithType = value;
                foreach (var animeListAnimeItem in ViewModelLocator.AnimeList.AnimeItems)
                {
                    animeListAnimeItem.RaisePropertyChanged(() => animeListAnimeItem.Type);
                }
            }
        }

        public bool AutoDescendingSorting
        {
            get { return Settings.AutoDescendingSorting; }
            set { Settings.AutoDescendingSorting = value; }
        }

        public bool PullHigherQualityImages
        {
            get { return Settings.PullHigherQualityImages; }
            set { Settings.PullHigherQualityImages = value; }
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

        public bool HamburgerAnimeFiltersExpanded
        {
            get { return Settings.HamburgerAnimeFiltersExpanded; }
            set { Settings.HamburgerAnimeFiltersExpanded = value; }
        }

        public bool HamburgerMangaFiltersExpanded
        {
            get { return Settings.HamburgerMangaFiltersExpanded; }
            set { Settings.HamburgerMangaFiltersExpanded = value; }
        }

        public bool HamburgerTopCategoriesExpanded
        {
            get { return Settings.HamburgerTopCategoriesExpanded; }
            set { Settings.HamburgerTopCategoriesExpanded = value; }
        }

        public bool AnimeListEnsureSelectedItemVisibleAfterOffContentCollapse
        {
            get { return Settings.AnimeListEnsureSelectedItemVisibleAfterOffContentCollapse; }
            set { Settings.AnimeListEnsureSelectedItemVisibleAfterOffContentCollapse = value; }
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

        public int RatePopUpStartupCounter => 7 - Settings.RatePopUpStartupCounter; //TODO Move this constant upper in dependency hierarchy

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

        public  bool SetStartDateOnWatching
        {
            get { return Settings.SetStartDateOnWatching; }
            set { Settings.SetStartDateOnWatching = value; }
        }

        public  bool SetStartDateOnListAdd
        {
            get { return Settings.SetStartDateOnListAdd; }
            set { Settings.SetStartDateOnListAdd = value; }
        }

        public  bool SetEndDateOnDropped
        {
            get { return Settings.SetEndDateOnDropped; }
            set { Settings.SetEndDateOnDropped = value; }
        }

        public  bool SetEndDateOnCompleted
        {
            get { return Settings.SetEndDateOnCompleted; }
            set { Settings.SetEndDateOnCompleted = value; }
        }

        public  bool OverrideValidStartEndDate
        {
            get { return Settings.OverrideValidStartEndDate; }
            set { Settings.OverrideValidStartEndDate = value; }
        }

        public  bool HamburgerMenuDefaultPaneState
        {
            get { return Settings.HamburgerMenuDefaultPaneState; }
            set { Settings.HamburgerMenuDefaultPaneState = value; }
        }

        public  bool HamburgerHideMangaSection
        {
            get { return Settings.HamburgerHideMangaSection; }
            set
            {
                ViewModelLocator.GeneralHamburger.MangaSectionVisbility = !value;
                Settings.HamburgerHideMangaSection = value;
            }
        }

        public  bool CalendarIncludeWatching
        {
            get { return Settings.CalendarIncludeWatching; }
            set { Settings.CalendarIncludeWatching = value; }
        }

        public  bool CalendarIncludePlanned
        {
            get { return Settings.CalendarIncludePlanned; }
            set { Settings.CalendarIncludePlanned = value; }
        }

        public  bool IsCachingEnabled
        {
            get { return Settings.IsCachingEnabled; }
            set { Settings.IsCachingEnabled = value; }
        }

        public  bool CalendarStartOnToday
        {
            get { return Settings.CalendarStartOnToday; }
            set { Settings.CalendarStartOnToday = value; }
        }

        public  bool CalendarRemoveEmptyDays
        {
            get { return Settings.CalendarRemoveEmptyDays; }
            set { Settings.CalendarRemoveEmptyDays = value; }
        }

        public  bool CalendarStartOnSummary => !Settings.CalendarStartOnToday;

        public  bool CalendarSwitchMonSun
        {
            get { return !Settings.CalendarSwitchMonSun; }
            set { Settings.CalendarSwitchMonSun = !value; }
        }

        public  bool CalendarPullExactAiringTime
        {
            get { return Settings.CalendarPullExactAiringTime; }
            set { Settings.CalendarPullExactAiringTime = value; }
        }

        public  bool EnableSwipeToIncDec
        {
            get { return Settings.EnableSwipeToIncDec; }
            set { Settings.EnableSwipeToIncDec = value; }
        }

        public  bool DetailsListReviewsView
        {
            get { return Settings.DetailsListReviewsView; }
            set { Settings.DetailsListReviewsView = value; }
        }

        public  bool DetailsListRecomsView
        {
            get { return Settings.DetailsListRecomsView; }
            set { Settings.DetailsListRecomsView = value; }
        }

        public  bool ArticlesLaunchExternalLinks
        {
            get { return Settings.ArticlesLaunchExternalLinks; }
            set { Settings.ArticlesLaunchExternalLinks = value; }
        }

        public  bool ArticlesDisplayScrollBar
        {
            get { return Settings.ArticlesDisplayScrollBar; }
            set { Settings.ArticlesDisplayScrollBar = value; }
        }

        public  bool SyncFavsFromTimeToTime
        {
            get { return Settings.SyncFavsFromTimeToTime; }
            set { Settings.SyncFavsFromTimeToTime = value; }
        }

        public  bool EnsureRandomizerAlwaysSelectsWinner
        {
            get { return Settings.EnsureRandomizerAlwaysSelectsWinner; }
            set { Settings.EnsureRandomizerAlwaysSelectsWinner = value; }
        }

        public  bool EnableImageCache
        {
            get { return Settings.EnableImageCache; }
            set { Settings.EnableImageCache = value; }
        }

        public  bool PullPeekPostsOnStartup
        {
            get { return Settings.PullPeekPostsOnStartup; }
            set { Settings.PullPeekPostsOnStartup = value; }
        }

        public  bool WatchedEpsPromptEnable
        {
            get { return Settings.WatchedEpsPromptEnable; }
            set { Settings.WatchedEpsPromptEnable = value; }
        }

        public  bool WatchedEpsPromptProceedOnDisabled
        {
            get { return Settings.WatchedEpsPromptProceedOnDisabled; }
            set { Settings.WatchedEpsPromptProceedOnDisabled = value; }
        }

        public  bool StatusPromptEnable
        {
            get { return Settings.StatusPromptEnable; }
            set { Settings.StatusPromptEnable = value; }
        }

        public  bool StatusPromptProceedOnDisabled
        {
            get { return Settings.StatusPromptProceedOnDisabled; }
            set { Settings.StatusPromptProceedOnDisabled = value; }
        }

        public  bool MangaFocusVolumes
        {
            get { return Settings.MangaFocusVolumes; }
            set
            {
                Settings.MangaFocusVolumes = value;
                foreach (var item in ViewModelLocator.AnimeList.AllLoadedMangaItemAbstractions)
                {
                    if (item.LoadedModel)
                        item.ViewModel.MangaFocusChanged(value);
                }
                if (ViewModelLocator.GeneralMain.CurrentMainPage == PageIndex.PageProfile)
                {
                    foreach (var item in ViewModelLocator.ProfilePage.FavManga.Concat(ViewModelLocator.ProfilePage.RecentManga))
                    {
                        item.MangaFocusChanged(value);
                    }               
                }
            }
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

        public bool MalApiDependatedntSectionsVisibility
            => Settings.SelectedApiType == ApiType.Mal;

        public bool HumApiDependatedntSectionsEnabled
            => Settings.SelectedApiType != ApiType.Mal;

        private async void LoadNews()
        {
            if (_newsLoaded)
                return;
            _newsLoaded = true;
            ResourceLocator.TelemetryProvider.TelemetryTrackEvent(TelemetryTrackedEvents.FetchedNews);
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

        public abstract void LoadCachedEntries();

        #region Notifications
        private ICommand _SetNotificationsRefreshTimeCommand;

        public ICommand SetNotificationsRefreshTimeCommand
            =>
            _SetNotificationsRefreshTimeCommand ??
            (_SetNotificationsRefreshTimeCommand = new RelayCommand<int>(i => NotificationsRefreshTime = i));

        private ICommand _callBackgroundTaskCommand;

        public ICommand CallBackgroundTaskCommand
            =>
            _callBackgroundTaskCommand ??
            (_callBackgroundTaskCommand = new RelayCommand(async () =>
            {
                ResourceLocator.NotificationsTaskManager.CallTask();
                IsCallNotificationsButtonEnabled = false;
                await Task.Delay(60000);
                IsCallNotificationsButtonEnabled = true;
            }));

        private ICommand _resetSeenNotificationsCommand;

        public ICommand ResetSeenNotificationsCommand
            =>
            _resetSeenNotificationsCommand ??
            (_resetSeenNotificationsCommand = new RelayCommand(() =>
            {
                ResourceLocator.ApplicationDataService[RoamingDataTypes.ReadNotifications] = string.Empty;
            }));

        private bool _isCallNotificationsButtonEnabled = true;

        public bool IsCallNotificationsButtonEnabled
        {
            get { return _isCallNotificationsButtonEnabled; }
            set
            {
                _isCallNotificationsButtonEnabled = value;
                RaisePropertyChanged(() => IsCallNotificationsButtonEnabled);
            }
        }

        public bool EnableNotifications
        {
            get { return Settings.EnableNotifications; }
            set
            {
                Settings.EnableNotifications = value;
                if(value)
                    ResourceLocator.NotificationsTaskManager.StartTask();
                else
                    ResourceLocator.NotificationsTaskManager.StopTask();                 
            }
        }

        public int NotificationsRefreshTime
        {
            get { return Settings.NotificationsRefreshTime; }
            set { Settings.NotificationsRefreshTime = value; }
        }


        public MalNotificationsTypes EnabledNotificationTypes
        {
            get { return Settings.EnabledNotificationTypes; }
            set
            {
                Settings.EnabledNotificationTypes = value;
                ResourceLocator.NotificationsTaskManager.StartTask();
            }
        }


        #endregion

        #region RecentlyMovedToMvvm

        private readonly ObservableCollection<CachedEntryModel> _cachedEntries =
            new ObservableCollection<CachedEntryModel>();

        public ObservableCollection<CachedEntryModel> CachedEntries
        {
            get
            {
                LoadCachedEntries();
                return _cachedEntries;
            }
        }

        private bool _emptyCachedListVisiblity;

        public bool EmptyCachedListVisiblity
        {
            get { return _emptyCachedListVisiblity; }
            set
            {
                _emptyCachedListVisiblity = value;
                RaisePropertyChanged(() => EmptyCachedListVisiblity);
            }
        }

        private bool _removeAllCachedDataButtonVisibility;

        public bool RemoveAllCachedDataButtonVisibility
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

        private bool _isSyncFavsButtonEnabled = true;

        public bool IsSyncFavsButtonEnabled
        {
            get { return _isSyncFavsButtonEnabled; }
            set
            {
                _isSyncFavsButtonEnabled = value;
                RaisePropertyChanged(() => IsSyncFavsButtonEnabled);
            }
        }

        #endregion

        #region Ads

        public event EmptyEventHander OnAdsMinutesPerDayChanged;

        public bool AdsEnable
        {
            get { return Settings.AdsEnable; }
            set
            {
                Settings.AdsEnable = value;
                ViewModelLocator.GeneralHamburger.UpdateBottomMargin();
                OnAdsMinutesPerDayChanged?.Invoke();
            }
        }

        public int AdsMinutesPerDay
        {
            get { return Settings.AdsSecondsPerDay; }
            set
            {
                Settings.AdsSecondsPerDay = value;
                OnAdsMinutesPerDayChanged?.Invoke();
            }
        }

        #endregion
    }
}