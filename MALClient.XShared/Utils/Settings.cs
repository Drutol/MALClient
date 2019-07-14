using System;
using System.Collections.Generic;
using System.Linq;
using MALClient.Adapters;
using MALClient.Models.Enums;
using MALClient.Models.Models.Anime;
using MALClient.XShared.Comm;
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
                    return (ApiType)(ApplicationDataService["SelectedApiType"] ?? ApiType.Mal);
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
                ApplicationDataService["SelectedApiType"] = (int)value;
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
            get => (int)(ApplicationDataService["CachePersistency"] ?? 7200);
            set => ApplicationDataService["CachePersistency"] = value;
        }

        public static int SelectedTheme
        {
            get
            {
                if (ViewModelLocator.Mobile)
                    return (int)(ApplicationDataService["SelectedTheme"] ?? 0);

                return (int)(ApplicationDataService["SelectedTheme"] ?? 1);
            }
            set => ApplicationDataService["SelectedTheme"] = value;
        }

        public static bool DarkThemeAmoled
        {
            get => (bool)(ApplicationDataService[nameof(DarkThemeAmoled)] ?? false);
            set => ApplicationDataService[nameof(DarkThemeAmoled)] = value;
        }

        public static bool IsCachingEnabled
        {
            get => (bool)(ApplicationDataService["EnableCache"] ?? true);
            set => ApplicationDataService["EnableCache"] = value;
        }

        public static SortOptions AnimeSortOrder
        {
            get => (SortOptions)(int)(ApplicationDataService["SortOrder"] ?? 0);
            set => ApplicationDataService["SortOrder"] = (int)value;
        }

        public static SortOptions MangaSortOrder
        {
            get => (SortOptions)(int)(ApplicationDataService["SortOrderM"] ?? 0);
            set => ApplicationDataService["SortOrderM"] = (int)value;
        }

        public static bool PreferEnglishTitles
        {
            get => (bool)(ApplicationDataService[nameof(PreferEnglishTitles)] ?? false);
            set => ApplicationDataService[nameof(PreferEnglishTitles)] = value;
        }

        public static bool IsSortDescending
        {
            get => (bool)(ApplicationDataService["SortDescending"] ?? false);
            set => ApplicationDataService["SortDescending"] = value;
        }

        public static bool ShowPriorities
        {
            get => (bool)(ApplicationDataService[nameof(ShowPriorities)] ?? true);
            set => ApplicationDataService[nameof(ShowPriorities)] = value;
        }

        public static bool ShowLowPriorities
        {
            get => (bool)(ApplicationDataService[nameof(ShowLowPriorities)] ?? false);
            set => ApplicationDataService[nameof(ShowLowPriorities)] = value;
        }

        public static bool IsMangaSortDescending
        {
            get => (bool)(ApplicationDataService["SortDescendingM"] ?? false);
            set => ApplicationDataService["SortDescendingM"] = value;
        }

        public static bool MangaFocusVolumes
        {
            get => (bool)(ApplicationDataService["MangaFocusVolumes"] ?? false);
            set => ApplicationDataService["MangaFocusVolumes"] = value;
        }

        public static int DefaultAnimeFilter
        {
            get => (int)(ApplicationDataService["DefaultFilter"] ?? (int)AnimeStatus.Watching);
            set => ApplicationDataService["DefaultFilter"] = value;
        }

        public static int DefaultMangaFilter
        {
            get => (int)(ApplicationDataService["DefaultFilterM"] ?? (int)AnimeStatus.Watching);
            set => ApplicationDataService["DefaultFilterM"] = value;
        }

        public static int ReviewsToPull
        {
            get => (int)(ApplicationDataService["ReviewsToPull"] ?? 20);
            set => ApplicationDataService["ReviewsToPull"] = value;
        }

        public static int RecommsToPull
        {
            get => (int)(ApplicationDataService["RecommsToPull"] ?? 10);
            set => ApplicationDataService["RecommsToPull"] = value;
        }

        public static int SeasonalToPull
        {
            get => (int)(ApplicationDataService["SeasonalToPull"] ?? 45);
            set => ApplicationDataService["SeasonalToPull"] = value;
        }

        public static int AirDayOffset
        {
            get => (int)(ApplicationDataService["AirDayOffset"] ?? 0);
            set => ApplicationDataService["AirDayOffset"] = value;
        }

        public static int AiringNotificationOffset
        {
            get => (int)(ApplicationDataService[nameof(AiringNotificationOffset)] ?? 0);
            set => ApplicationDataService[nameof(AiringNotificationOffset)] = value;
        }

        public static string DefaultMenuTab
        {
            get => (string)(ApplicationDataService["DefaultMenuTab"] ?? "anime");
            set => ApplicationDataService["DefaultMenuTab"] = value;
        }

        public static bool DetailsAutoLoadDetails // hehe
        {
            get => (bool)(ApplicationDataService["DetailsAutoLoadDetails"] ?? false);
            set => ApplicationDataService["DetailsAutoLoadDetails"] = value;
        }

        public static bool DetailsAutoLoadReviews
        {
            get => (bool)(ApplicationDataService["DetailsAutoLoadReviews"] ?? false);
            set => ApplicationDataService["DetailsAutoLoadReviews"] = value;
        }

        public static bool DetailsAutoLoadRelated
        {
            get => (bool)(ApplicationDataService["DetailsAutoLoadRelated"] ?? false);
            set => ApplicationDataService["DetailsAutoLoadRelated"] = value;
        }

        public static bool DetailsAutoLoadRecomms
        {
            get => (bool)(ApplicationDataService["DetailsAutoLoadRecomms"] ?? false);
            set => ApplicationDataService["DetailsAutoLoadRecomms"] = value;
        }

        public static bool HideFilterSelectionFlyout
        {
            get => (bool)(ApplicationDataService["HideFilterSelectionFlyout"] ?? false);
            set => ApplicationDataService["HideFilterSelectionFlyout"] = value;
        }

        public static bool HideViewSelectionFlyout
        {
            get => (bool)(ApplicationDataService["HideViewSelectionFlyout"] ?? true);
            set => ApplicationDataService["HideViewSelectionFlyout"] = value;
        }

        public static bool HideSortingSelectionFlyout
        {
            get => (bool)(ApplicationDataService["HideSortingSelectionFlyout"] ?? false);
            set => ApplicationDataService["HideSortingSelectionFlyout"] = value;
        }

        public static bool HamburgerAnimeFiltersExpanded
        {
            get => (bool)(ApplicationDataService["HamburgerAnimeFiltersExpanded"] ?? false);
            set => ApplicationDataService["HamburgerAnimeFiltersExpanded"] = value;
        }

        public static bool HamburgerTopCategoriesExpanded
        {
            get => (bool)(ApplicationDataService["HamburgerTopCategoriesExpanded"] ?? true);
            set => ApplicationDataService["HamburgerTopCategoriesExpanded"] = value;
        }

        public static bool AnimeListEnsureSelectedItemVisibleAfterOffContentCollapse
        {
            get => (bool)
                (ApplicationDataService[
                     "AnimeListEnsureSelectedItemVisibleAfterOffContentCollapse"] ?? false);
            set =>
                ApplicationDataService["AnimeListEnsureSelectedItemVisibleAfterOffContentCollapse"] = value;
        }

        public static bool HamburgerMangaFiltersExpanded
        {
            get => (bool)(ApplicationDataService["HamburgerMangaFiltersExpanded"] ?? false);
            set => ApplicationDataService["HamburgerMangaFiltersExpanded"] = value;
        }

        public static bool HamburgerHideMangaSection
        {
            get => (bool)(ApplicationDataService["HamburgerHideMangaSection"] ?? false);
            set => ApplicationDataService["HamburgerHideMangaSection"] = value;
        }

        public static bool HamburgerMenuDefaultPaneState
        {
            get => (bool)(ApplicationDataService["HamburgerMenuDefaultPaneState"] ?? true);
            set => ApplicationDataService["HamburgerMenuDefaultPaneState"] = value;
        }

        public static DataSource PrefferedDataSource
        {
            get => (DataSource)
                (ApplicationDataService["PrefferedDataSource"] ?? DataSource.AnnHum);
            set => ApplicationDataService["PrefferedDataSource"] = (int)value;
        }

        public static bool EnableHearthAnimation
        {
            get => (bool)(ApplicationDataService["EnableHearthAnimation"] ?? true);
            set => ApplicationDataService["EnableHearthAnimation"] = value;
        }

        public static bool EnableSwipeToIncDec
        {
            get => (bool)(ApplicationDataService["EnableSwipeToIncDec"] ?? true);
            set => ApplicationDataService["EnableSwipeToIncDec"] = value;
        }

        public static bool ReverseSwipingDirection
        {
            get => (bool)(ApplicationDataService[nameof(ReverseSwipingDirection)] ?? false);
            set => ApplicationDataService[nameof(ReverseSwipingDirection)] = value;
        }

        public static bool DisplayScoreDialogAfterCompletion
        {
            get => (bool)(ApplicationDataService[nameof(DisplayScoreDialogAfterCompletion)] ?? false);
            set => ApplicationDataService[nameof(DisplayScoreDialogAfterCompletion)] = value;
        }

        public static bool EnableOfflineSync
        {
            get => false;
            set => ApplicationDataService[nameof(EnableOfflineSync)] = value;
        }

        public static bool AnimeSyncRequired
        {
            get => (bool)(ApplicationDataService[nameof(AnimeSyncRequired)] ?? false);
            set => ApplicationDataService[nameof(AnimeSyncRequired)] = value;
        }

        public static bool MangaSyncRequired
        {
            get => (bool)(ApplicationDataService[nameof(MangaSyncRequired)] ?? false);
            set => ApplicationDataService[nameof(MangaSyncRequired)] = value;
        }

        public static bool HideGlobalScoreInDetailsWhenNotRated
        {
            get => (bool)(ApplicationDataService[nameof(HideGlobalScoreInDetailsWhenNotRated)] ?? false);
            set => ApplicationDataService[nameof(HideGlobalScoreInDetailsWhenNotRated)] = value;
        }

        public static bool HideDecrementButtons
        {
            get => (bool)(ApplicationDataService[nameof(HideDecrementButtons)] ?? false);
            set => ApplicationDataService[nameof(HideDecrementButtons)] = value;
        }

        public static bool DontAskToMoveOnHoldEntries
        {
            get => (bool)(ApplicationDataService[nameof(DontAskToMoveOnHoldEntries)] ?? false);
            set => ApplicationDataService[nameof(DontAskToMoveOnHoldEntries)] = value;
        }

        public static bool SqueezeOneMoreGridItem
        {
            get => (bool)(ApplicationDataService[nameof(SqueezeOneMoreGridItem)] ?? false);
            set => ApplicationDataService[nameof(SqueezeOneMoreGridItem)] = value;
        }

        public static bool DisplayUnsetScores
        {
            get => (bool)(ApplicationDataService[nameof(DisplayUnsetScores)] ?? true);
            set => ApplicationDataService[nameof(DisplayUnsetScores)] = value;
        }

        public static bool DetailsListRecomsView
        {
            get => (bool)(ApplicationDataService["DetailsListRecomsView"] ?? true);
            set => ApplicationDataService["DetailsListRecomsView"] = value;
        }

        public static bool DetailsListReviewsView
        {
            get => (bool)(ApplicationDataService["DetailsListReviewsView"] ?? true);
            set => ApplicationDataService["DetailsListReviewsView"] = value;
        }

        public static bool EnsureRandomizerAlwaysSelectsWinner
        {
            get => (bool)(ApplicationDataService["EnsureRandomizerAlwaysSelectsWinner"] ?? false);
            set => ApplicationDataService["EnsureRandomizerAlwaysSelectsWinner"] = value;
        }

        public static bool PullHigherQualityImagesDefault = true;

        public static bool PullHigherQualityImages
        {
            get => (bool)(ApplicationDataService["PullHigherQualityImages"] ?? PullHigherQualityImagesDefault);
            set => ApplicationDataService["PullHigherQualityImages"] = value;
        }

        public static bool ForceSearchIntoOffPage
        {
            get => (bool)(ApplicationDataService[nameof(ForceSearchIntoOffPage)] ?? false);
            set => ApplicationDataService[nameof(ForceSearchIntoOffPage)] = value;
        }

        public static string AppVersion
        {
            get => ApplicationDataService["AppVersion"] as string;
            set => ApplicationDataService["AppVersion"] = value;
        }

        public static AnimeStatus DefaultStatusAfterAdding
        {
            get => (AnimeStatus)((int?)ApplicationDataService[nameof(DefaultStatusAfterAdding)] ?? 6);
            set => ApplicationDataService[nameof(DefaultStatusAfterAdding)] = (int)value;
        }

        #region Views

        public static AnimeListDisplayModes WatchingDisplayMode
        {
            get => (AnimeListDisplayModes)
                (ApplicationDataService["WatchingDisplayMode"] ??
                 AnimeListDisplayModes.IndefiniteGrid);
            set => ApplicationDataService["WatchingDisplayMode"] = (int)value;
        }

        public static AnimeListDisplayModes CompletedDisplayMode
        {
            get => (AnimeListDisplayModes)
                (ApplicationDataService["CompletedDisplayMode"] ??
                 AnimeListDisplayModes.IndefiniteGrid);
            set => ApplicationDataService["CompletedDisplayMode"] = (int)value;
        }

        public static AnimeListDisplayModes OnHoldDisplayMode
        {
            get => (AnimeListDisplayModes)
                (ApplicationDataService["OnHoldDisplayMode"] ??
                 AnimeListDisplayModes.IndefiniteGrid);
            set => ApplicationDataService["OnHoldDisplayMode"] = (int)value;
        }

        public static AnimeListDisplayModes DroppedDisplayMode
        {
            get => (AnimeListDisplayModes)
                (ApplicationDataService["DroppedDisplayMode"] ??
                 AnimeListDisplayModes.IndefiniteGrid);
            set => ApplicationDataService["DroppedDisplayMode"] = (int)value;
        }

        public static AnimeListDisplayModes PlannedDisplayMode
        {
            get => (AnimeListDisplayModes)
                (ApplicationDataService["PlannedDisplayMode"] ??
                 AnimeListDisplayModes.IndefiniteGrid);
            set => ApplicationDataService["PlannedDisplayMode"] = (int)value;
        }

        public static AnimeListDisplayModes AllDisplayMode
        {
            get => (AnimeListDisplayModes)
                (ApplicationDataService["AllDisplayMode"] ??
                 AnimeListDisplayModes.IndefiniteGrid);
            set => ApplicationDataService["AllDisplayMode"] = (int)value;
        }

        public static bool LockDisplayMode
        {
            get => (bool)(ApplicationDataService["LockDisplayMode"] ?? false);
            set => ApplicationDataService["LockDisplayMode"] = value;
        }

        public static bool AutoDescendingSorting
        {
            get => (bool)(ApplicationDataService["AutoDescendingSorting"] ?? true);
            set => ApplicationDataService["AutoDescendingSorting"] = value;
        }

        public static bool DisplaySeasonWithType
        {
            get => (bool)(ApplicationDataService["DisplaySeasonWithType"] ?? false);
            set => ApplicationDataService["DisplaySeasonWithType"] = value;
        }

        public static bool EnableImageCache
        {
            get => (bool)(ApplicationDataService["EnableImageCache"] ?? true);
            set => ApplicationDataService["EnableImageCache"] = value;
        }

        #endregion Views

        #region RatePopUp

        public static bool RatePopUpEnable
        {
            get => (bool)(ApplicationDataService["RatePopUpEnable"] ?? true);
            set => ApplicationDataService["RatePopUpEnable"] = value;
        }

        public static int RatePopUpStartupCounter
        {
            get => (int)(ApplicationDataService["RatePopUpStartupCounter"] ?? 0);
            set => ApplicationDataService["RatePopUpStartupCounter"] = value;
        }

        public static int DonatePopUpStartupCounter
        {
            get => (int)(ApplicationDataService["DonatePopUpStartupCounter"] ?? 0);
            set => ApplicationDataService["DonatePopUpStartupCounter"] = value;
        }

        public static bool Donated
        {
            get => (bool)(ApplicationDataService["Donated"] ?? false);
            set => ApplicationDataService["Donated"] = value;
        }

        #endregion RatePopUp

        #region StartEndDates

        public static bool SetStartDateOnWatching
        {
            get => (bool)(ApplicationDataService["SetStartDateOnWatching"] ?? true);
            set => ApplicationDataService["SetStartDateOnWatching"] = value;
        }

        public static bool SetStartDateOnListAdd
        {
            get => (bool)(ApplicationDataService["SetStartDateOnListAdd"] ?? false);
            set => ApplicationDataService["SetStartDateOnListAdd"] = value;
        }

        public static bool SetEndDateOnDropped
        {
            get => (bool)(ApplicationDataService["SetEndDateOnDropped"] ?? false);
            set => ApplicationDataService["SetEndDateOnDropped"] = value;
        }

        public static bool SetEndDateOnCompleted
        {
            get => (bool)(ApplicationDataService["SetEndDateOnCompleted"] ?? true);
            set => ApplicationDataService["SetEndDateOnCompleted"] = value;
        }

        public static bool OverrideValidStartEndDate
        {
            get => (bool)(ApplicationDataService["OverrideValidStartEndDate"] ?? false);
            set => ApplicationDataService["OverrideValidStartEndDate"] = value;
        }

        #endregion StartEndDates

        #region Calendar

        public static bool CalendarIncludeWatching
        {
            get => (bool)(ApplicationDataService["CalendarIncludeWatching"] ?? true);
            set => ApplicationDataService["CalendarIncludeWatching"] = value;
        }

        public static bool CalendarIncludePlanned
        {
            get => (bool)(ApplicationDataService["CalendarIncludePlanned"] ?? false);
            set => ApplicationDataService["CalendarIncludePlanned"] = value;
        }

        public static bool CalendarSwitchMonSun
        {
            get => (bool)(ApplicationDataService["CalendarSwitchMonSun"] ?? false);
            set => ApplicationDataService["CalendarSwitchMonSun"] = value;
        }

        public static bool CalendarStartOnToday
        {
            get => (bool)(ApplicationDataService["CalendarStartOnToday"] ?? false);
            set => ApplicationDataService["CalendarStartOnToday"] = value;
        }

        public static bool CalendarRemoveEmptyDays
        {
            get => (bool)(ApplicationDataService["CalendarRemoveEmptyDays"] ?? true);
            set => ApplicationDataService["CalendarRemoveEmptyDays"] = value;
        }

        public static bool CalendarPullExactAiringTime
        {
            get => (bool)(ApplicationDataService[nameof(CalendarPullExactAiringTime)] ?? true);
            set => ApplicationDataService[nameof(CalendarPullExactAiringTime)] = value;
        }

        #endregion Calendar

        #region Articles

        public static bool ArticlesLaunchExternalLinks
        {
            get => (bool)(ApplicationDataService["ArticlesLaunchExternalLinks"] ?? true);
            set => ApplicationDataService["ArticlesLaunchExternalLinks"] = value;
        }

        public static bool ArticlesDisplayScrollBar
        {
            get => (bool)(ApplicationDataService["ArticlesDisplayScrollBar"] ?? false);
            set => ApplicationDataService["ArticlesDisplayScrollBar"] = value;
        }

        #endregion Articles

        #region Favs

        public static bool SyncFavsFromTimeToTime
        {
            get => (bool)(ApplicationDataService["SyncFavsFromTimeToTime"] ?? true);
            set => ApplicationDataService["SyncFavsFromTimeToTime"] = value;
        }

        public static int LastFavTimeSync //ticks
        {
            get => (int)(ApplicationDataService["LastFavTimeSync"] ?? 0);
            set => ApplicationDataService["LastFavTimeSync"] = value;
        }

        #endregion Favs

        #region Forums

        public static string ForumsPinnedBoards
        {
            get => (string)(ApplicationDataService["ForumsPinnedBoards"] ?? "");
            set => ApplicationDataService["ForumsPinnedBoards"] = value;
        }

        public static bool PullPeekPostsOnStartup
        {
            get => (bool)(ApplicationDataService[nameof(PullPeekPostsOnStartup)] ?? true);
            set => ApplicationDataService[nameof(PullPeekPostsOnStartup)] = value;
        }

        public static bool ForumsSearchOnCopy
        {
            get => (bool)(ApplicationDataService[nameof(ForumsSearchOnCopy)] ?? true);
            set => ApplicationDataService[nameof(ForumsSearchOnCopy)] = value;
        }

        public static bool ForumsAllowSignatures
        {
            get => (bool)(ApplicationDataService[nameof(ForumsAllowSignatures)] ?? true);
            set => ApplicationDataService[nameof(ForumsAllowSignatures)] = value;
        }

        #endregion Forums

        #region Notifiations

        public static bool EnableNotifications
        {
            get => (bool)(ApplicationDataService[nameof(EnableNotifications)] ?? true);
            set => ApplicationDataService[nameof(EnableNotifications)] = value;
        }

        public static MalNotificationsTypes EnabledNotificationTypes
        {
            get => (MalNotificationsTypes)
                (ApplicationDataService[nameof(EnabledNotificationTypes)] ??
                 MalNotificationsTypes.ClubMessages | MalNotificationsTypes.ForumQuoute |
                 MalNotificationsTypes.FriendRequest | MalNotificationsTypes.Messages |
                 MalNotificationsTypes.FriendRequestAcceptDeny | MalNotificationsTypes.NewRelatedAnime |
                 MalNotificationsTypes.ProfileComment | MalNotificationsTypes.UserMentions |
                 MalNotificationsTypes.WatchedTopics | MalNotificationsTypes.NowAiring);
            set => ApplicationDataService[nameof(EnabledNotificationTypes)] = (int)value;
        }

        public static int NotificationsRefreshTime
        {
            get => (int)(ApplicationDataService[nameof(NotificationsRefreshTime)] ?? 60);
            set => ApplicationDataService[nameof(NotificationsRefreshTime)] = value;
        }

        public static bool NotificationCheckInRuntime
        {
            get => (bool)(ApplicationDataService[nameof(NotificationCheckInRuntime)] ?? true);
            set => ApplicationDataService[nameof(NotificationCheckInRuntime)] = value;
        }

        #endregion Notifiations

        #region Ads

        public static bool AdsEnable
        {
            get => (bool)(ApplicationDataService[nameof(AdsEnable)] ?? true);
            set => ApplicationDataService[nameof(AdsEnable)] = value;
        }

        public static int AdsSecondsPerDay
        {
            get => (int)(ApplicationDataService[nameof(AdsSecondsPerDay)] ?? 600);
            set => ApplicationDataService[nameof(AdsSecondsPerDay)] = value;
        }

        #endregion Ads

        #region Wallpapers

        public static List<WallpaperSources> EnabledWallpaperSources
        {
            get
            {
                var str = ApplicationDataService[nameof(EnabledWallpaperSources)] as string;
                if (string.IsNullOrEmpty(str))
                    str = "0;1;2;3;4;5;6";

                return str.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(source => (WallpaperSources)int.Parse(source)).ToList();
            }
            set
            {
                ApplicationDataService[nameof(EnabledWallpaperSources)] = string.Join(";",
                    value.Select(source => ((int)source).ToString()));
            }
        }

        public static int WallpapersBaseAmount
        {
            get => (int)(ApplicationDataService[nameof(WallpapersBaseAmount)] ?? (ViewModelLocator.Mobile ? 2 : 4));
            set => ApplicationDataService[nameof(WallpapersBaseAmount)] = value;
        }

        #endregion Wallpapers

        #region PopUpsPrompts

        public static bool WatchedEpsPromptEnable
        {
            get => (bool)(ApplicationDataService[nameof(WatchedEpsPromptEnable)] ?? true);
            set => ApplicationDataService[nameof(WatchedEpsPromptEnable)] = value;
        }

        public static bool WatchedEpsPromptProceedOnDisabled
        {
            get => (bool)(ApplicationDataService[nameof(WatchedEpsPromptProceedOnDisabled)] ?? false);
            set => ApplicationDataService[nameof(WatchedEpsPromptProceedOnDisabled)] = value;
        }

        public static bool StatusPromptEnable
        {
            get => (bool)(ApplicationDataService[nameof(StatusPromptEnable)] ?? true);
            set => ApplicationDataService[nameof(StatusPromptEnable)] = value;
        }

        public static bool StatusPromptProceedOnDisabled
        {
            get => (bool)(ApplicationDataService[nameof(StatusPromptProceedOnDisabled)] ?? false);
            set => ApplicationDataService[nameof(StatusPromptProceedOnDisabled)] = value;
        }

        public static bool EnableShareButton
        {
            get => (bool)(ApplicationDataService[nameof(StatusPromptProceedOnDisabled)] ?? true);
            set => ApplicationDataService[nameof(StatusPromptProceedOnDisabled)] = value;
        }

        #endregion PopUpsPrompts

        #region Feeds

        public static bool FeedsIncludePinnedProfiles
        {
            get => (bool)(ApplicationDataService[nameof(FeedsIncludePinnedProfiles)] ?? false);
            set => ApplicationDataService[nameof(FeedsIncludePinnedProfiles)] = value;
        }

        public static int FeedsMaxEntries
        {
            get => (int)(ApplicationDataService[nameof(FeedsMaxEntries)] ?? 5);
            set => ApplicationDataService[nameof(FeedsMaxEntries)] = value;
        }

        public static int FeedsMaxEntryAge
        {
            get => (int)(ApplicationDataService[nameof(FeedsMaxEntryAge)] ?? 7);
            set => ApplicationDataService[nameof(FeedsMaxEntryAge)] = value;
        }

        #endregion Feeds

        #region Android

        public static bool MakeGridItemsSmaller
        {
            get => (bool)(ApplicationDataService[nameof(MakeGridItemsSmaller)] ?? false);
            set => ApplicationDataService[nameof(MakeGridItemsSmaller)] = value;
        }

        public static bool AskBeforeSendingCrashReports
        {
            get => (bool)(ApplicationDataService[nameof(AskBeforeSendingCrashReports)] ?? true);
            set => ApplicationDataService[nameof(AskBeforeSendingCrashReports)] = value;
        }



        #endregion Android
    }
}