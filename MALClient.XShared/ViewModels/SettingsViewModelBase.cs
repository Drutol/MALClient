using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Android.Runtime;
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

    [Preserve(AllMembers = true)]
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
            await new ProfileQuery().GetProfileData(true, true);
            IsSyncFavsButtonEnabled = true;
        }));

        public ObservableCollection<Tuple<AnimeListDisplayModes, string>> DisplayModes { get; } = new ObservableCollection
            <Tuple<AnimeListDisplayModes, string>>
        {
            new Tuple<AnimeListDisplayModes, string>(AnimeListDisplayModes.IndefiniteList, "Detailed Grid"),
            new Tuple<AnimeListDisplayModes, string>(AnimeListDisplayModes.IndefiniteGrid, "Grid"),
            new Tuple<AnimeListDisplayModes, string>(AnimeListDisplayModes.IndefiniteCompactList, "Compact List")
        };

        public virtual List<SettingsPageEntry> SettingsPages { get; } = new List<SettingsPageEntry>
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
            new SettingsPageEntry
            {
                Header = "Friends feeds",
                Subtitle = "Customize friends feeds behaviour.",
                Symbol = SettingsSymbolsEnum.ContactInfo,
                PageType = SettingsPageIndex.Feeds
            },
            new SettingsPageEntry
            {
                Header = "Notifications",
                Subtitle = "Notifications types, polling frequency...",
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
                Subtitle = "MyAnimelist authentication.",
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
            get => DisplayModes[(int)Settings.WatchingDisplayMode];
            set => Settings.WatchingDisplayMode = value.Item1;
        }

        public Tuple<AnimeListDisplayModes, string> SelectedDefaultViewForCompleted
        {
            get => DisplayModes[(int)Settings.CompletedDisplayMode];
            set => Settings.CompletedDisplayMode = value.Item1;
        }

        public Tuple<AnimeListDisplayModes, string> SelectedDefaultViewForOnHold
        {
            get => DisplayModes[(int)Settings.OnHoldDisplayMode];
            set => Settings.OnHoldDisplayMode = value.Item1;
        }

        public Tuple<AnimeListDisplayModes, string> SelectedDefaultViewForDropped
        {
            get => DisplayModes[(int)Settings.DroppedDisplayMode];
            set => Settings.DroppedDisplayMode = value.Item1;
        }

        public Tuple<AnimeListDisplayModes, string> SelectedDefaultViewForPlanned
        {
            get => DisplayModes[(int)Settings.PlannedDisplayMode];
            set => Settings.PlannedDisplayMode = value.Item1;
        }

        public Tuple<AnimeListDisplayModes, string> SelectedDefaultViewForAll
        {
            get => DisplayModes[(int)Settings.AllDisplayMode];
            set => Settings.AllDisplayMode = value.Item1;
        }

        public bool LockDisplayMode
        {
            get => Settings.LockDisplayMode;
            set => Settings.LockDisplayMode = value;
        }

        public bool DisplaySeasonWithType
        {
            get => Settings.DisplaySeasonWithType;
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
            get => Settings.AutoDescendingSorting;
            set => Settings.AutoDescendingSorting = value;
        }

        public bool PullHigherQualityImages
        {
            get => Settings.PullHigherQualityImages;
            set => Settings.PullHigherQualityImages = value;
        }

        public bool HideFilterSelectionFlyout
        {
            get => Settings.HideFilterSelectionFlyout;
            set => Settings.HideFilterSelectionFlyout = value;
        }

        public bool HideViewSelectionFlyout
        {
            get => Settings.HideViewSelectionFlyout;
            set => Settings.HideViewSelectionFlyout = value;
        }

        public bool HideSortingSelectionFlyout
        {
            get => Settings.HideSortingSelectionFlyout;
            set => Settings.HideSortingSelectionFlyout = value;
        }

        public bool HamburgerAnimeFiltersExpanded
        {
            get => Settings.HamburgerAnimeFiltersExpanded;
            set => Settings.HamburgerAnimeFiltersExpanded = value;
        }

        public bool HamburgerMangaFiltersExpanded
        {
            get => Settings.HamburgerMangaFiltersExpanded;
            set => Settings.HamburgerMangaFiltersExpanded = value;
        }

        public bool HamburgerTopCategoriesExpanded
        {
            get => Settings.HamburgerTopCategoriesExpanded;
            set => Settings.HamburgerTopCategoriesExpanded = value;
        }

        public bool AnimeListEnsureSelectedItemVisibleAfterOffContentCollapse
        {
            get => Settings.AnimeListEnsureSelectedItemVisibleAfterOffContentCollapse;
            set => Settings.AnimeListEnsureSelectedItemVisibleAfterOffContentCollapse = value;
        }

        public bool RatePopUpEnable
        {
            get => Settings.RatePopUpEnable;
            set
            {
                Settings.RatePopUpEnable = value;
                RaisePropertyChanged(() => RatePopUpEnable);
            }
        }

        public bool EnableHearthAnimation
        {
            get => Settings.EnableHearthAnimation;
            set => Settings.EnableHearthAnimation = value;
        }

        public int RatePopUpStartupCounter => 7 - Settings.RatePopUpStartupCounter; //TODO Move this constant upper in dependency hierarchy

        public int AirDayOffset
        {
            get => Settings.AirDayOffset;
            set => Settings.AirDayOffset = value;
        }

        public int AiringNotificationOffset
        {
            get => Settings.AiringNotificationOffset;
            set => Settings.AiringNotificationOffset = value;
        }

        public bool DataSourceAnn
        {
            get => Settings.PrefferedDataSource == DataSource.Ann;
            set
            {
                if (value) Settings.PrefferedDataSource = DataSource.Ann;
            }
        }

        public bool DataSourceHum
        {
            get => Settings.PrefferedDataSource == DataSource.Hummingbird;
            set
            {
                if (value) Settings.PrefferedDataSource = DataSource.Hummingbird;
            }
        }

        public bool DataSourceAnnHum
        {
            get => Settings.PrefferedDataSource == DataSource.AnnHum;
            set
            {
                if (value) Settings.PrefferedDataSource = DataSource.AnnHum;
            }
        }

        public bool SetStartDateOnWatching
        {
            get => Settings.SetStartDateOnWatching;
            set => Settings.SetStartDateOnWatching = value;
        }

        public bool SetStartDateOnListAdd
        {
            get => Settings.SetStartDateOnListAdd;
            set => Settings.SetStartDateOnListAdd = value;
        }

        public bool SetEndDateOnDropped
        {
            get => Settings.SetEndDateOnDropped;
            set => Settings.SetEndDateOnDropped = value;
        }

        public bool SetEndDateOnCompleted
        {
            get => Settings.SetEndDateOnCompleted;
            set => Settings.SetEndDateOnCompleted = value;
        }

        public bool OverrideValidStartEndDate
        {
            get => Settings.OverrideValidStartEndDate;
            set => Settings.OverrideValidStartEndDate = value;
        }

        public bool HamburgerMenuDefaultPaneState
        {
            get => Settings.HamburgerMenuDefaultPaneState;
            set => Settings.HamburgerMenuDefaultPaneState = value;
        }

        public bool HamburgerHideMangaSection
        {
            get => Settings.HamburgerHideMangaSection;
            set
            {
                ViewModelLocator.GeneralHamburger.MangaSectionVisbility = !value;
                Settings.HamburgerHideMangaSection = value;
            }
        }

        public bool DisplayScoreDialogAfterCompletion
        {
            get => Settings.DisplayScoreDialogAfterCompletion;
            set => Settings.DisplayScoreDialogAfterCompletion = value;
        }

        public bool CalendarIncludeWatching
        {
            get => Settings.CalendarIncludeWatching;
            set => Settings.CalendarIncludeWatching = value;
        }

        public bool CalendarIncludePlanned
        {
            get => Settings.CalendarIncludePlanned;
            set => Settings.CalendarIncludePlanned = value;
        }

        public bool IsCachingEnabled
        {
            get => Settings.IsCachingEnabled;
            set => Settings.IsCachingEnabled = value;
        }

        public bool CalendarStartOnToday
        {
            get => Settings.CalendarStartOnToday;
            set => Settings.CalendarStartOnToday = value;
        }

        public bool CalendarRemoveEmptyDays
        {
            get => Settings.CalendarRemoveEmptyDays;
            set => Settings.CalendarRemoveEmptyDays = value;
        }

        public bool CalendarStartOnSummary => !Settings.CalendarStartOnToday;

        public bool CalendarSwitchMonSun
        {
            get => !Settings.CalendarSwitchMonSun;
            set => Settings.CalendarSwitchMonSun = !value;
        }

        public bool CalendarPullExactAiringTime
        {
            get => Settings.CalendarPullExactAiringTime;
            set => Settings.CalendarPullExactAiringTime = value;
        }

        public bool EnableSwipeToIncDec
        {
            get => Settings.EnableSwipeToIncDec;
            set => Settings.EnableSwipeToIncDec = value;
        }

        public bool ReverseSwipingDirection
        {
            get => Settings.ReverseSwipingDirection;
            set => Settings.ReverseSwipingDirection = value;
        }

        public bool DetailsListReviewsView
        {
            get => Settings.DetailsListReviewsView;
            set => Settings.DetailsListReviewsView = value;
        }

        public bool DetailsListRecomsView
        {
            get => Settings.DetailsListRecomsView;
            set => Settings.DetailsListRecomsView = value;
        }

        public bool ShowPriorities
        {
            get => Settings.ShowPriorities;
            set => Settings.ShowPriorities = value;
        }

        public bool ShowLowPriorities
        {
            get => Settings.ShowLowPriorities;
            set => Settings.ShowLowPriorities = value;
        }

        public bool ArticlesLaunchExternalLinks
        {
            get => Settings.ArticlesLaunchExternalLinks;
            set => Settings.ArticlesLaunchExternalLinks = value;
        }

        public bool ArticlesDisplayScrollBar
        {
            get => Settings.ArticlesDisplayScrollBar;
            set => Settings.ArticlesDisplayScrollBar = value;
        }

        public bool PreferEnglishTitles
        {
            get => Settings.PreferEnglishTitles;
            set => Settings.PreferEnglishTitles = value;
        }

        public bool MakeGridItemsSmaller
        {
            get => Settings.MakeGridItemsSmaller;
            set => Settings.MakeGridItemsSmaller = value;
        }

        public bool SyncFavsFromTimeToTime
        {
            get => Settings.SyncFavsFromTimeToTime;
            set => Settings.SyncFavsFromTimeToTime = value;
        }

        public bool EnsureRandomizerAlwaysSelectsWinner
        {
            get => Settings.EnsureRandomizerAlwaysSelectsWinner;
            set => Settings.EnsureRandomizerAlwaysSelectsWinner = value;
        }

        public bool EnableImageCache
        {
            get => Settings.EnableImageCache;
            set => Settings.EnableImageCache = value;
        }

        public bool PullPeekPostsOnStartup
        {
            get => Settings.PullPeekPostsOnStartup;
            set => Settings.PullPeekPostsOnStartup = value;
        }

        public bool ForumsSearchOnCopy
        {
            get => Settings.ForumsSearchOnCopy;
            set => Settings.ForumsSearchOnCopy = value;
        }

        public AnimeStatus DefaultStatusAfterAdding
        {
            get => Settings.DefaultStatusAfterAdding;
            set => Settings.DefaultStatusAfterAdding = value;
        }

        public bool ForceSearchIntoOffPage
        {
            get => Settings.ForceSearchIntoOffPage;
            set => Settings.ForceSearchIntoOffPage = value;
        }

        public bool WatchedEpsPromptEnable
        {
            get => Settings.WatchedEpsPromptEnable;
            set => Settings.WatchedEpsPromptEnable = value;
        }

        public bool WatchedEpsPromptProceedOnDisabled
        {
            get => Settings.WatchedEpsPromptProceedOnDisabled;
            set => Settings.WatchedEpsPromptProceedOnDisabled = value;
        }

        public bool StatusPromptEnable
        {
            get => Settings.StatusPromptEnable;
            set => Settings.StatusPromptEnable = value;
        }

        public bool StatusPromptProceedOnDisabled
        {
            get => Settings.StatusPromptProceedOnDisabled;
            set => Settings.StatusPromptProceedOnDisabled = value;
        }

        public bool EnableShareButton
        {
            get => Settings.EnableShareButton;
            set => Settings.EnableShareButton = value;
        }

        public bool DisplayUnsetScores
        {
            get => Settings.DisplayUnsetScores;
            set => Settings.DisplayUnsetScores = value;
        }

        public bool HideGlobalScoreInDetailsWhenNotRated
        {
            get => Settings.HideGlobalScoreInDetailsWhenNotRated;
            set => Settings.HideGlobalScoreInDetailsWhenNotRated = value;
        }

        public bool DontAskToMoveOnHoldEntries
        {
            get => Settings.DontAskToMoveOnHoldEntries;
            set => Settings.DontAskToMoveOnHoldEntries = value;
        }

        public bool HideDecrementButtons
        {
            get => Settings.HideDecrementButtons;
            set => Settings.HideDecrementButtons = value;
        }

        public bool SqueezeOneMoreGridItem
        {
            get => Settings.SqueezeOneMoreGridItem;
            set => Settings.SqueezeOneMoreGridItem = value;
        }

        public bool MangaFocusVolumes
        {
            get => Settings.MangaFocusVolumes;
            set
            {
                Settings.MangaFocusVolumes = value;
                foreach (var item in ResourceLocator.AnimeLibraryDataStorage.AllLoadedMangaItemAbstractions.Concat(
                    ResourceLocator.AnimeLibraryDataStorage.OthersAbstractions.Select(pair => pair.Value)
                        .SelectMany(tuple => tuple.Item2)))
                {
                    if (item.LoadedModel)
                        item.ViewModel.MangaFocusChanged(value);
                }
                if (!ViewModelLocator.Mobile)
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
                                    await new NewsQuery().GetRequestResponse()));
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
                ResourceLocator.NotificationsTaskManager.CallTask(BgTasks.Notifications);
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
                ResourceLocator.ApplicationDataService[nameof(RoamingDataTypes.ReadNotifications)] = string.Empty;
            }));

        private bool _isCallNotificationsButtonEnabled = true;

        public bool IsCallNotificationsButtonEnabled
        {
            get => _isCallNotificationsButtonEnabled;
            set
            {
                _isCallNotificationsButtonEnabled = value;
                RaisePropertyChanged(() => IsCallNotificationsButtonEnabled);
            }
        }

        public bool EnableNotifications
        {
            get => Settings.EnableNotifications;
            set
            {
                Settings.EnableNotifications = value;
                if (value)
                    ResourceLocator.NotificationsTaskManager.StartTask(BgTasks.Notifications);
                else
                    ResourceLocator.NotificationsTaskManager.StopTask(BgTasks.Notifications);
            }
        }

        public bool NotificationCheckInRuntime
        {
            get => Settings.NotificationCheckInRuntime;
            set
            {
                Settings.NotificationCheckInRuntime = value;
                if (value)
                    ResourceLocator.SchdeuledJobsManger.StartJob(ScheduledJob.FetchNotifications, 5,
                        () => ResourceLocator.NotificationsTaskManager.CallTask(BgTasks.Notifications));
                else
                    ResourceLocator.SchdeuledJobsManger.StopJob(ScheduledJob.FetchNotifications);
            }
        }

        public int NotificationsRefreshTime
        {
            get => Settings.NotificationsRefreshTime;
            set => Settings.NotificationsRefreshTime = value;
        }

        public MalNotificationsTypes EnabledNotificationTypes
        {
            get => Settings.EnabledNotificationTypes;
            set
            {
                Settings.EnabledNotificationTypes = value;
                ResourceLocator.NotificationsTaskManager.StartTask(BgTasks.Notifications);
            }
        }

        #endregion Notifications

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
            get => _emptyCachedListVisiblity;
            set
            {
                _emptyCachedListVisiblity = value;
                RaisePropertyChanged(() => EmptyCachedListVisiblity);
            }
        }

        private bool _removeAllCachedDataButtonVisibility;

        public bool RemoveAllCachedDataButtonVisibility
        {
            get => _removeAllCachedDataButtonVisibility;
            set
            {
                _removeAllCachedDataButtonVisibility = value;
                RaisePropertyChanged(() => RemoveAllCachedDataButtonVisibility);
            }
        }

        private string _totalFilesCached = "N/A";

        public string TotalFilesCached
        {
            get => _totalFilesCached;
            set
            {
                _totalFilesCached = value;
                RaisePropertyChanged(() => TotalFilesCached);
            }
        }

        private bool _isSyncFavsButtonEnabled = true;
        private bool _squeezeOneMoreGridItem;

        public bool IsSyncFavsButtonEnabled
        {
            get => _isSyncFavsButtonEnabled;
            set
            {
                _isSyncFavsButtonEnabled = value;
                RaisePropertyChanged(() => IsSyncFavsButtonEnabled);
            }
        }

        #endregion RecentlyMovedToMvvm

        #region Ads

        public event EmptyEventHander OnAdsMinutesPerDayChanged;

        public bool AdsEnable
        {
            get => Settings.AdsEnable;
            set
            {
                Settings.AdsEnable = value;
                ViewModelLocator.GeneralHamburger.UpdateBottomMargin();
                OnAdsMinutesPerDayChanged?.Invoke();
            }
        }

        public int AdsSecondsPerDay
        {
            get => Settings.AdsSecondsPerDay;
            set
            {
                Settings.AdsSecondsPerDay = value;
                OnAdsMinutesPerDayChanged?.Invoke();
            }
        }

        #endregion Ads

        #region Feeds

        public bool FeedsIncludePinnedProfiles
        {
            get => Settings.FeedsIncludePinnedProfiles;
            set => Settings.FeedsIncludePinnedProfiles = value;
        }

        public int FeedsMaxEntries
        {
            get => Settings.FeedsMaxEntries;
            set => Settings.FeedsMaxEntries = value;
        }

        public int FeedsMaxEntryAge
        {
            get => Settings.FeedsMaxEntryAge;
            set => Settings.FeedsMaxEntryAge = value;
        }

        #endregion Feeds
    }
}