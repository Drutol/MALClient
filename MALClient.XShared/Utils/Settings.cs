using System;
using System.Collections.Generic;
using System.Linq;
using MALClient.Adapters;
using MALClient.Models.Enums;
using MALClient.Models.Models.Anime;
using MALClient.XShared.Comm;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.ViewModels;

namespace MALClient.XShared.Utils
{
    public static class Settings
    {
        private static readonly IApplicationDataService ApplicationDataService;

        static Settings()
        {
            ApplicationDataService = ResourceLocator.ApplicationDataService;
        }

        public static ApiType SelectedApiType
        {
            get
            {
                try
                {
                    return (ApiType) (ApplicationDataService["SelectedApiType"] ?? ApiType.Mal);
                }
                catch (Exception) //in case of testing
                {
                    return ApiType.Mal;
                }
            }
            set
            {
                if (SelectedApiType == value)
                    return;
                ApplicationDataService["SelectedApiType"] = (int) value;
                Query.CurrentApiType = value;
                ViewModelLocator.AnimeDetails.UpdateScoreFlyoutChoices();
                AnimeItemViewModel.UpdateScoreFlyoutChoices();
                ViewModelLocator.GeneralHamburger.UpdateApiDependentButtons();
                if (AnimeSortOrder == SortOptions.SortLastWatched)
                    AnimeSortOrder = SortOptions.SortTitle;
            }
        }

        public static int CachePersitence
        {
            get { return (int) (ApplicationDataService["CachePersistency"] ?? 86400); }
            set { ApplicationDataService["CachePersistency"] = value; }
        }

        public static int SelectedTheme
        {
            get
            {
                return
                    (int) (ApplicationDataService["SelectedTheme"] ?? 1);
            }
            set { ApplicationDataService["SelectedTheme"] = value; }
        }

        public static bool IsCachingEnabled
        {
            get
            {
                return (bool) (ApplicationDataService["EnableCache"] ?? true);
                ;
            }
            set { ApplicationDataService["EnableCache"] = value; }
        }

        public static SortOptions AnimeSortOrder
        {
            get { return (SortOptions) (int) (ApplicationDataService["SortOrder"] ?? 0); }
            set { ApplicationDataService["SortOrder"] = (int) value; }
        }

        public static SortOptions MangaSortOrder
        {
            get { return (SortOptions) (int) (ApplicationDataService["SortOrderM"] ?? 0); }
            set { ApplicationDataService["SortOrderM"] = (int) value; }
        }

        public static bool IsSortDescending
        {
            get { return (bool) (ApplicationDataService["SortDescending"] ?? false); }
            set { ApplicationDataService["SortDescending"] = value; }
        }

        public static bool IsMangaSortDescending
        {
            get { return (bool) (ApplicationDataService["SortDescendingM"] ?? false); }
            set { ApplicationDataService["SortDescendingM"] = value; }
        }

        public static bool MangaFocusVolumes
        {
            get { return (bool) (ApplicationDataService["MangaFocusVolumes"] ?? false); }
            set { ApplicationDataService["MangaFocusVolumes"] = value; }
        }

        public static int DefaultAnimeFilter
        {
            get
            {
                return
                    (int) (ApplicationDataService["DefaultFilter"] ?? (int) AnimeStatus.Watching);
            }
            set { ApplicationDataService["DefaultFilter"] = value; }
        }

        public static int DefaultMangaFilter
        {
            get
            {
                return
                    (int) (ApplicationDataService["DefaultFilterM"] ?? (int) AnimeStatus.Watching);
            }
            set { ApplicationDataService["DefaultFilterM"] = value; }
        }

        public static int ReviewsToPull
        {
            get { return (int) (ApplicationDataService["ReviewsToPull"] ?? 8); }
            set { ApplicationDataService["ReviewsToPull"] = value; }
        }

        public static int RecommsToPull
        {
            get { return (int) (ApplicationDataService["RecommsToPull"] ?? 10); }
            set { ApplicationDataService["RecommsToPull"] = value; }
        }

        public static int SeasonalToPull
        {
            get { return (int) (ApplicationDataService["SeasonalToPull"] ?? 45); }
            set { ApplicationDataService["SeasonalToPull"] = value; }
        }

        public static int AirDayOffset
        {
            get { return (int) (ApplicationDataService["AirDayOffset"] ?? 0); }
            set { ApplicationDataService["AirDayOffset"] = value; }
        }

        public static string DefaultMenuTab
        {
            get { return (string) (ApplicationDataService["DefaultMenuTab"] ?? "anime"); }
            set { ApplicationDataService["DefaultMenuTab"] = value; }
        }

        public static bool DetailsAutoLoadDetails // hehe
        {
            get { return (bool) (ApplicationDataService["DetailsAutoLoadDetails"] ?? false); }
            set { ApplicationDataService["DetailsAutoLoadDetails"] = value; }
        }

        public static bool DetailsAutoLoadReviews
        {
            get { return (bool) (ApplicationDataService["DetailsAutoLoadReviews"] ?? false); }
            set { ApplicationDataService["DetailsAutoLoadReviews"] = value; }
        }

        public static bool DetailsAutoLoadRelated
        {
            get { return (bool) (ApplicationDataService["DetailsAutoLoadRelated"] ?? false); }
            set { ApplicationDataService["DetailsAutoLoadRelated"] = value; }
        }

        public static bool DetailsAutoLoadRecomms
        {
            get { return (bool) (ApplicationDataService["DetailsAutoLoadRecomms"] ?? false); }
            set { ApplicationDataService["DetailsAutoLoadRecomms"] = value; }
        }


        public static bool HideFilterSelectionFlyout
        {
            get { return (bool) (ApplicationDataService["HideFilterSelectionFlyout"] ?? false); }
            set { ApplicationDataService["HideFilterSelectionFlyout"] = value; }
        }

        public static bool HideViewSelectionFlyout
        {
            get { return (bool) (ApplicationDataService["HideViewSelectionFlyout"] ?? true); }
            set { ApplicationDataService["HideViewSelectionFlyout"] = value; }
        }

        public static bool HideSortingSelectionFlyout
        {
            get { return (bool) (ApplicationDataService["HideSortingSelectionFlyout"] ?? false); }
            set { ApplicationDataService["HideSortingSelectionFlyout"] = value; }
        }

        public static bool HamburgerAnimeFiltersExpanded
        {
            get
            {
                return (bool) (ApplicationDataService["HamburgerAnimeFiltersExpanded"] ?? false);
            }
            set { ApplicationDataService["HamburgerAnimeFiltersExpanded"] = value; }
        }

        public static bool HamburgerTopCategoriesExpanded
        {
            get
            {
                return (bool) (ApplicationDataService["HamburgerTopCategoriesExpanded"] ?? true);
            }
            set { ApplicationDataService["HamburgerTopCategoriesExpanded"] = value; }
        }

        public static bool AnimeListEnsureSelectedItemVisibleAfterOffContentCollapse
        {
            get
            {
                return
                    (bool)
                        (ApplicationDataService[
                            "AnimeListEnsureSelectedItemVisibleAfterOffContentCollapse"] ?? false);
            }
            set
            {
                ApplicationDataService["AnimeListEnsureSelectedItemVisibleAfterOffContentCollapse"
                    ] = value;
            }
        }

        public static bool HamburgerMangaFiltersExpanded
        {
            get
            {
                return (bool) (ApplicationDataService["HamburgerMangaFiltersExpanded"] ?? false);
            }
            set { ApplicationDataService["HamburgerMangaFiltersExpanded"] = value; }
        }

        public static bool HamburgerHideMangaSection
        {
            get
            {
                return (bool) (ApplicationDataService["HamburgerHideMangaSection"] ?? false);
            }
            set { ApplicationDataService["HamburgerHideMangaSection"] = value; }
        }

        public static bool HamburgerMenuDefaultPaneState
        {
            get
            {
                return (bool) (ApplicationDataService["HamburgerMenuDefaultPaneState"] ?? true);
            }
            set { ApplicationDataService["HamburgerMenuDefaultPaneState"] = value; }
        }

        public static DataSource PrefferedDataSource
        {
            get
            {
                return
                    (DataSource)
                        (ApplicationDataService["PrefferedDataSource"] ?? DataSource.AnnHum);
            }
            set { ApplicationDataService["PrefferedDataSource"] = (int) value; }
        }

        public static bool EnableHearthAnimation
        {
            get { return (bool) (ApplicationDataService["EnableHearthAnimation"] ?? true); }
            set { ApplicationDataService["EnableHearthAnimation"] = value; }
        }

        public static bool EnableSwipeToIncDec
        {
            get { return (bool) (ApplicationDataService["EnableSwipeToIncDec"] ?? true); }
            set { ApplicationDataService["EnableSwipeToIncDec"] = value; }
        }

        public static bool DetailsListRecomsView
        {
            get { return (bool) (ApplicationDataService["DetailsListRecomsView"] ?? true); }
            set { ApplicationDataService["DetailsListRecomsView"] = value; }
        }

        public static bool DetailsListReviewsView
        {
            get { return (bool) (ApplicationDataService["DetailsListReviewsView"] ?? true); }
            set { ApplicationDataService["DetailsListReviewsView"] = value; }
        }

        public static bool EnsureRandomizerAlwaysSelectsWinner
        {
            get { return (bool) (ApplicationDataService["EnsureRandomizerAlwaysSelectsWinner"] ?? false); }
            set { ApplicationDataService["EnsureRandomizerAlwaysSelectsWinner"] = value; }
        }

        public static bool PullHigherQualityImages
        {
            get { return (bool) (ApplicationDataService["PullHigherQualityImages"] ?? true); }
            set { ApplicationDataService["PullHigherQualityImages"] = value; }
        }

        public static bool ForceSearchIntoOffPage
        {
            get { return (bool) (ApplicationDataService[nameof(ForceSearchIntoOffPage)] ?? false); }
            set { ApplicationDataService[nameof(ForceSearchIntoOffPage)] = value; }
        }

        public static string PinnedProfiles
        {
            get { return (string) (ApplicationDataService["PinnedProfiles"] ?? ""); }
            set { ApplicationDataService["PinnedProfiles"] = value; }
        }

        #region Views

        public static AnimeListDisplayModes WatchingDisplayMode
        {
            get
            {
                return
                    (AnimeListDisplayModes)
                        (ApplicationDataService["WatchingDisplayMode"] ??
                         AnimeListDisplayModes.IndefiniteGrid);
            }
            set { ApplicationDataService["WatchingDisplayMode"] = (int) value; }
        }

        public static AnimeListDisplayModes CompletedDisplayMode
        {
            get
            {
                return
                    (AnimeListDisplayModes)
                        (ApplicationDataService["CompletedDisplayMode"] ??
                         AnimeListDisplayModes.IndefiniteGrid);
            }
            set { ApplicationDataService["CompletedDisplayMode"] = (int) value; }
        }

        public static AnimeListDisplayModes OnHoldDisplayMode
        {
            get
            {
                return
                    (AnimeListDisplayModes)
                        (ApplicationDataService["OnHoldDisplayMode"] ??
                         AnimeListDisplayModes.IndefiniteGrid);
            }
            set { ApplicationDataService["OnHoldDisplayMode"] = (int) value; }
        }

        public static AnimeListDisplayModes DroppedDisplayMode
        {
            get
            {
                return
                    (AnimeListDisplayModes)
                        (ApplicationDataService["DroppedDisplayMode"] ??
                         AnimeListDisplayModes.IndefiniteGrid);
            }
            set { ApplicationDataService["DroppedDisplayMode"] = (int) value; }
        }

        public static AnimeListDisplayModes PlannedDisplayMode
        {
            get
            {
                return
                    (AnimeListDisplayModes)
                        (ApplicationDataService["PlannedDisplayMode"] ??
                         AnimeListDisplayModes.IndefiniteGrid);
            }
            set { ApplicationDataService["PlannedDisplayMode"] = (int) value; }
        }

        public static AnimeListDisplayModes AllDisplayMode
        {
            get
            {
                return
                    (AnimeListDisplayModes)
                        (ApplicationDataService["AllDisplayMode"] ??
                         AnimeListDisplayModes.IndefiniteGrid);
            }
            set { ApplicationDataService["AllDisplayMode"] = (int) value; }
        }

        public static bool LockDisplayMode
        {
            get { return (bool) (ApplicationDataService["LockDisplayMode"] ?? false); }
            set { ApplicationDataService["LockDisplayMode"] = value; }
        }

        public static bool AutoDescendingSorting
        {
            get { return (bool) (ApplicationDataService["AutoDescendingSorting"] ?? true); }
            set { ApplicationDataService["AutoDescendingSorting"] = value; }
        }

        public static bool DisplaySeasonWithType
        {
            get { return (bool) (ApplicationDataService["DisplaySeasonWithType"] ?? false); }
            set { ApplicationDataService["DisplaySeasonWithType"] = value; }
        }

        public static bool EnableImageCache
        {
            get { return (bool) (ApplicationDataService["EnableImageCache"] ?? true); }
            set { ApplicationDataService["EnableImageCache"] = value; }
        }

        #endregion

        #region RatePopUp

        public static bool RatePopUpEnable
        {
            get { return (bool) (ApplicationDataService["RatePopUpEnable"] ?? true); }
            set { ApplicationDataService["RatePopUpEnable"] = value; }
        }

        public static int RatePopUpStartupCounter
        {
            get { return (int) (ApplicationDataService["RatePopUpStartupCounter"] ?? 0); }
            set { ApplicationDataService["RatePopUpStartupCounter"] = value; }
        }

        public static int DonatePopUpStartupCounter
        {
            get { return (int) (ApplicationDataService["DonatePopUpStartupCounter"] ?? 0); }
            set { ApplicationDataService["DonatePopUpStartupCounter"] = value; }
        }

        public static bool Donated
        {
            get { return (bool) (ApplicationDataService["Donated"] ?? false); }
            set { ApplicationDataService["Donated"] = value; }
        }

        #endregion

        #region StartEndDates

        public static bool SetStartDateOnWatching
        {
            get { return (bool) (ApplicationDataService["SetStartDateOnWatching"] ?? true); }
            set { ApplicationDataService["SetStartDateOnWatching"] = value; }
        }

        public static bool SetStartDateOnListAdd
        {
            get { return (bool) (ApplicationDataService["SetStartDateOnListAdd"] ?? false); }
            set { ApplicationDataService["SetStartDateOnListAdd"] = value; }
        }

        public static bool SetEndDateOnDropped
        {
            get { return (bool) (ApplicationDataService["SetEndDateOnDropped"] ?? false); }
            set { ApplicationDataService["SetEndDateOnDropped"] = value; }
        }

        public static bool SetEndDateOnCompleted
        {
            get { return (bool) (ApplicationDataService["SetEndDateOnCompleted"] ?? true); }
            set { ApplicationDataService["SetEndDateOnCompleted"] = value; }
        }

        public static bool OverrideValidStartEndDate
        {
            get { return (bool) (ApplicationDataService["OverrideValidStartEndDate"] ?? false); }
            set { ApplicationDataService["OverrideValidStartEndDate"] = value; }
        }

        #endregion

        #region Calendar

        public static bool CalendarIncludeWatching
        {
            get { return (bool) (ApplicationDataService["CalendarIncludeWatching"] ?? true); }
            set { ApplicationDataService["CalendarIncludeWatching"] = value; }
        }

        public static bool CalendarIncludePlanned
        {
            get { return (bool) (ApplicationDataService["CalendarIncludePlanned"] ?? false); }
            set { ApplicationDataService["CalendarIncludePlanned"] = value; }
        }

        public static bool CalendarSwitchMonSun
        {
            get { return (bool) (ApplicationDataService["CalendarSwitchMonSun"] ?? false); }
            set { ApplicationDataService["CalendarSwitchMonSun"] = value; }
        }

        public static bool CalendarStartOnToday
        {
            get { return (bool) (ApplicationDataService["CalendarStartOnToday"] ?? false); }
            set { ApplicationDataService["CalendarStartOnToday"] = value; }
        }

        public static bool CalendarRemoveEmptyDays
        {
            get { return (bool) (ApplicationDataService["CalendarRemoveEmptyDays"] ?? true); }
            set { ApplicationDataService["CalendarRemoveEmptyDays"] = value; }
        }

        public static bool CalendarPullExactAiringTime
        {
            get { return (bool) (ApplicationDataService[nameof(CalendarPullExactAiringTime)] ?? true); }
            set { ApplicationDataService[nameof(CalendarPullExactAiringTime)] = value; }
        }

        #endregion

        #region Articles

        public static bool ArticlesLaunchExternalLinks
        {
            get { return (bool) (ApplicationDataService["ArticlesLaunchExternalLinks"] ?? true); }
            set { ApplicationDataService["ArticlesLaunchExternalLinks"] = value; }
        }

        public static bool ArticlesDisplayScrollBar
        {
            get { return (bool) (ApplicationDataService["ArticlesDisplayScrollBar"] ?? false); }
            set { ApplicationDataService["ArticlesDisplayScrollBar"] = value; }
        }

        #endregion

        #region Favs

        public static bool SyncFavsFromTimeToTime
        {
            get { return (bool)(ApplicationDataService["SyncFavsFromTimeToTime"] ?? true); }
            set { ApplicationDataService["SyncFavsFromTimeToTime"] = value; }
        }

        public static int LastFavTimeSync //ticks
        {
            get { return (int)(ApplicationDataService["LastFavTimeSync"] ?? 0); }
            set { ApplicationDataService["LastFavTimeSync"] = value; }
        }

        #endregion

        #region Forums
        public static string ForumsPinnedBoards
        {
            get { return (string) (ApplicationDataService["ForumsPinnedBoards"] ?? ""); }
            set { ApplicationDataService["ForumsPinnedBoards"] = value; }
        }

        public static bool PullPeekPostsOnStartup
        {
            get { return (bool) (ApplicationDataService[nameof(PullPeekPostsOnStartup)] ?? true); }
            set { ApplicationDataService[nameof(PullPeekPostsOnStartup)] = value; }
        }



        #endregion

        #region Notifiations

        public static bool EnableNotifications
        {
            get { return (bool)(ApplicationDataService[nameof(EnableNotifications)] ?? true); }
            set { ApplicationDataService[nameof(EnableNotifications)] = value; }
        }

        public static MalNotificationsTypes EnabledNotificationTypes
        {
            get
            {
                return
                    (MalNotificationsTypes)
                    (ApplicationDataService[nameof(EnabledNotificationTypes)] ??
                     MalNotificationsTypes.ClubMessages | MalNotificationsTypes.ForumQuoute |
                     MalNotificationsTypes.FriendRequest | MalNotificationsTypes.Messages |
                     MalNotificationsTypes.FriendRequestAcceptDeny | MalNotificationsTypes.NewRelatedAnime |
                     MalNotificationsTypes.ProfileComment | MalNotificationsTypes.UserMentions |
                     MalNotificationsTypes.WatchedTopics | MalNotificationsTypes.NowAiring);
            } 
            set { ApplicationDataService[nameof(EnabledNotificationTypes)] = (int)value; }
        }

        public static int NotificationsRefreshTime
        {
            get { return (int)(ApplicationDataService[nameof(NotificationsRefreshTime)] ?? 15); }
            set { ApplicationDataService[nameof(NotificationsRefreshTime)] = value; }
        }
        #endregion

        #region Ads

        public static bool AdsEnable
        {
            get { return (bool)(ApplicationDataService[nameof(AdsEnable)] ?? false); }
            set { ApplicationDataService[nameof(AdsEnable)] = value; }
        }

        public static int AdsSecondsPerDay
        {
            get { return (int)(ApplicationDataService[nameof(AdsSecondsPerDay)] ?? 600); }
            set { ApplicationDataService[nameof(AdsSecondsPerDay)] = value; }
        }


        #endregion

        #region Wallpapers

        public static List<WallpaperSources> EnabledWallpaperSources
        {
            get
            {
                var str = (string) (ApplicationDataService[nameof(EnabledWallpaperSources)] ?? "0;1;2;3;4;5;6");

                return str.Split(new [] { ";" } ,StringSplitOptions.RemoveEmptyEntries).Select(source => (WallpaperSources) int.Parse(source)).ToList();
            }
            set
            {
                ApplicationDataService[nameof(EnabledWallpaperSources)] = string.Join(";",
                    value.Select(source => ((int) source).ToString()));
            }
        }

        public static int WallpapersBaseAmount
        {
            get { return (int)(ApplicationDataService[nameof(WallpapersBaseAmount)] ?? (ViewModelLocator.Mobile ? 2 : 4)); }
            set { ApplicationDataService[nameof(WallpapersBaseAmount)] = value; }
        }
        #endregion
    }
}