using System;
using Windows.Storage;
using Windows.UI.Xaml;
using MalClient.Shared.Comm;
using MalClient.Shared.Models.Anime;
using MalClient.Shared.Utils.Enums;
using MalClient.Shared.ViewModels;

namespace MalClient.Shared.Utils
{
    public static class Settings
    {
        public static ApiType SelectedApiType
        {
            get
            {
                try
                {
                    return (ApiType) (ApplicationData.Current.LocalSettings.Values["SelectedApiType"] ?? ApiType.Mal);
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
                ApplicationData.Current.LocalSettings.Values["SelectedApiType"] = (int) value;
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
            get { return (int) (ApplicationData.Current.LocalSettings.Values["CachePersistency"] ?? 86400); }
            set { ApplicationData.Current.LocalSettings.Values["CachePersistency"] = value; }
        }

        public static ApplicationTheme SelectedTheme
        {
            get
            {
                return
                    (ApplicationTheme)
                        (ApplicationData.Current.LocalSettings.Values["SelectedTheme"] ?? (int) ApplicationTheme.Dark);
            }
            set { ApplicationData.Current.LocalSettings.Values["SelectedTheme"] = (int) value; }
        }

        public static bool IsCachingEnabled
        {
            get
            {
                return (bool) (ApplicationData.Current.LocalSettings.Values["EnableCache"] ?? true);
                ;
            }
            set { ApplicationData.Current.LocalSettings.Values["EnableCache"] = value; }
        }

        public static SortOptions AnimeSortOrder
        {
            get { return (SortOptions) (int) (ApplicationData.Current.LocalSettings.Values["SortOrder"] ?? 0); }
            set { ApplicationData.Current.LocalSettings.Values["SortOrder"] = (int) value; }
        }

        public static SortOptions MangaSortOrder
        {
            get { return (SortOptions) (int) (ApplicationData.Current.LocalSettings.Values["SortOrderM"] ?? 0); }
            set { ApplicationData.Current.LocalSettings.Values["SortOrderM"] = (int) value; }
        }

        public static bool IsSortDescending
        {
            get { return (bool) (ApplicationData.Current.LocalSettings.Values["SortDescending"] ?? false); }
            set { ApplicationData.Current.LocalSettings.Values["SortDescending"] = value; }
        }

        public static bool IsMangaSortDescending
        {
            get { return (bool) (ApplicationData.Current.LocalSettings.Values["SortDescendingM"] ?? false); }
            set { ApplicationData.Current.LocalSettings.Values["SortDescendingM"] = value; }
        }

        public static bool MangaFocusVolumes
        {
            get { return (bool) (ApplicationData.Current.LocalSettings.Values["MangaFocusVolumes"] ?? false); }
            set { ApplicationData.Current.LocalSettings.Values["MangaFocusVolumes"] = value; }
        }

        public static int DefaultAnimeFilter
        {
            get
            {
                return
                    (int) (ApplicationData.Current.LocalSettings.Values["DefaultFilter"] ?? (int) AnimeStatus.Watching);
            }
            set { ApplicationData.Current.LocalSettings.Values["DefaultFilter"] = value; }
        }

        public static int DefaultMangaFilter
        {
            get
            {
                return
                    (int) (ApplicationData.Current.LocalSettings.Values["DefaultFilterM"] ?? (int) AnimeStatus.Watching);
            }
            set { ApplicationData.Current.LocalSettings.Values["DefaultFilterM"] = value; }
        }

        public static int ReviewsToPull
        {
            get { return (int) (ApplicationData.Current.LocalSettings.Values["ReviewsToPull"] ?? 8); }
            set { ApplicationData.Current.LocalSettings.Values["ReviewsToPull"] = value; }
        }

        public static int RecommsToPull
        {
            get { return (int) (ApplicationData.Current.LocalSettings.Values["RecommsToPull"] ?? 10); }
            set { ApplicationData.Current.LocalSettings.Values["RecommsToPull"] = value; }
        }

        public static int SeasonalToPull
        {
            get { return (int) (ApplicationData.Current.LocalSettings.Values["SeasonalToPull"] ?? 45); }
            set { ApplicationData.Current.LocalSettings.Values["SeasonalToPull"] = value; }
        }

        public static int AirDayOffset
        {
            get { return (int) (ApplicationData.Current.LocalSettings.Values["AirDayOffset"] ?? 0); }
            set { ApplicationData.Current.LocalSettings.Values["AirDayOffset"] = value; }
        }

        public static string DefaultMenuTab
        {
            get { return (string) (ApplicationData.Current.LocalSettings.Values["DefaultMenuTab"] ?? "anime"); }
            set { ApplicationData.Current.LocalSettings.Values["DefaultMenuTab"] = value; }
        }

        public static bool DetailsAutoLoadDetails // hehe
        {
            get { return (bool) (ApplicationData.Current.LocalSettings.Values["DetailsAutoLoadDetails"] ?? false); }
            set { ApplicationData.Current.LocalSettings.Values["DetailsAutoLoadDetails"] = value; }
        }

        public static bool DetailsAutoLoadReviews
        {
            get { return (bool) (ApplicationData.Current.LocalSettings.Values["DetailsAutoLoadReviews"] ?? false); }
            set { ApplicationData.Current.LocalSettings.Values["DetailsAutoLoadReviews"] = value; }
        }

        public static bool DetailsAutoLoadRelated
        {
            get { return (bool) (ApplicationData.Current.LocalSettings.Values["DetailsAutoLoadRelated"] ?? false); }
            set { ApplicationData.Current.LocalSettings.Values["DetailsAutoLoadRelated"] = value; }
        }

        public static bool DetailsAutoLoadRecomms
        {
            get { return (bool) (ApplicationData.Current.LocalSettings.Values["DetailsAutoLoadRecomms"] ?? false); }
            set { ApplicationData.Current.LocalSettings.Values["DetailsAutoLoadRecomms"] = value; }
        }


        public static bool HideFilterSelectionFlyout
        {
            get { return (bool) (ApplicationData.Current.LocalSettings.Values["HideFilterSelectionFlyout"] ?? false); }
            set { ApplicationData.Current.LocalSettings.Values["HideFilterSelectionFlyout"] = value; }
        }

        public static bool HideViewSelectionFlyout
        {
            get { return (bool) (ApplicationData.Current.LocalSettings.Values["HideViewSelectionFlyout"] ?? true); }
            set { ApplicationData.Current.LocalSettings.Values["HideViewSelectionFlyout"] = value; }
        }

        public static bool HideSortingSelectionFlyout
        {
            get { return (bool) (ApplicationData.Current.LocalSettings.Values["HideSortingSelectionFlyout"] ?? false); }
            set { ApplicationData.Current.LocalSettings.Values["HideSortingSelectionFlyout"] = value; }
        }

        public static bool HamburgerAnimeFiltersExpanded
        {
            get
            {
                return (bool) (ApplicationData.Current.LocalSettings.Values["HamburgerAnimeFiltersExpanded"] ?? false);
            }
            set { ApplicationData.Current.LocalSettings.Values["HamburgerAnimeFiltersExpanded"] = value; }
        }

        public static bool HamburgerTopCategoriesExpanded
        {
            get
            {
                return (bool) (ApplicationData.Current.LocalSettings.Values["HamburgerTopCategoriesExpanded"] ?? true);
            }
            set { ApplicationData.Current.LocalSettings.Values["HamburgerTopCategoriesExpanded"] = value; }
        }

        public static bool AnimeListEnsureSelectedItemVisibleAfterOffContentCollapse
        {
            get
            {
                return
                    (bool)
                        (ApplicationData.Current.LocalSettings.Values[
                            "AnimeListEnsureSelectedItemVisibleAfterOffContentCollapse"] ?? false);
            }
            set
            {
                ApplicationData.Current.LocalSettings.Values["AnimeListEnsureSelectedItemVisibleAfterOffContentCollapse"
                    ] = value;
            }
        }

        public static bool HamburgerMangaFiltersExpanded
        {
            get
            {
                return (bool) (ApplicationData.Current.LocalSettings.Values["HamburgerMangaFiltersExpanded"] ?? false);
            }
            set { ApplicationData.Current.LocalSettings.Values["HamburgerMangaFiltersExpanded"] = value; }
        }

        public static bool HamburgerHideMangaSection
        {
            get
            {
                return (bool) (ApplicationData.Current.LocalSettings.Values["HamburgerHideMangaSection"] ?? false);
            }
            set { ApplicationData.Current.LocalSettings.Values["HamburgerHideMangaSection"] = value; }
        }

        public static bool HamburgerMenuDefaultPaneState
        {
            get
            {
                return (bool) (ApplicationData.Current.LocalSettings.Values["HamburgerMenuDefaultPaneState"] ?? true);
            }
            set { ApplicationData.Current.LocalSettings.Values["HamburgerMenuDefaultPaneState"] = value; }
        }

        public static DataSource PrefferedDataSource
        {
            get
            {
                return
                    (DataSource)
                        (ApplicationData.Current.LocalSettings.Values["PrefferedDataSource"] ?? DataSource.AnnHum);
            }
            set { ApplicationData.Current.LocalSettings.Values["PrefferedDataSource"] = (int) value; }
        }

        public static bool EnableHearthAnimation
        {
            get { return (bool) (ApplicationData.Current.LocalSettings.Values["EnableHearthAnimation"] ?? true); }
            set { ApplicationData.Current.LocalSettings.Values["EnableHearthAnimation"] = value; }
        }

        public static bool EnableSwipeToIncDec
        {
            get { return (bool) (ApplicationData.Current.LocalSettings.Values["EnableSwipeToIncDec"] ?? true); }
            set { ApplicationData.Current.LocalSettings.Values["EnableSwipeToIncDec"] = value; }
        }

        public static bool DetailsListRecomsView
        {
            get { return (bool) (ApplicationData.Current.LocalSettings.Values["DetailsListRecomsView"] ?? true); }
            set { ApplicationData.Current.LocalSettings.Values["DetailsListRecomsView"] = value; }
        }

        public static bool DetailsListReviewsView
        {
            get { return (bool) (ApplicationData.Current.LocalSettings.Values["DetailsListReviewsView"] ?? true); }
            set { ApplicationData.Current.LocalSettings.Values["DetailsListReviewsView"] = value; }
        }

        public static bool EnsureRandomizerAlwaysSelectsWinner
        {
            get { return (bool) (ApplicationData.Current.LocalSettings.Values["EnsureRandomizerAlwaysSelectsWinner"] ?? false); }
            set { ApplicationData.Current.LocalSettings.Values["EnsureRandomizerAlwaysSelectsWinner"] = value; }
        }

        public static string PinnedProfiles
        {
                get { return (string)(ApplicationData.Current.LocalSettings.Values["PinnedProfiles"] ?? ""); }
                set { ApplicationData.Current.LocalSettings.Values["PinnedProfiles"] = value; }
            }

        #region Views

        public static AnimeListDisplayModes WatchingDisplayMode
        {
            get
            {
                return
                    (AnimeListDisplayModes)
                        (ApplicationData.Current.LocalSettings.Values["WatchingDisplayMode"] ??
                         AnimeListDisplayModes.IndefiniteGrid);
            }
            set { ApplicationData.Current.LocalSettings.Values["WatchingDisplayMode"] = (int) value; }
        }

        public static AnimeListDisplayModes CompletedDisplayMode
        {
            get
            {
                return
                    (AnimeListDisplayModes)
                        (ApplicationData.Current.LocalSettings.Values["CompletedDisplayMode"] ??
                         AnimeListDisplayModes.IndefiniteGrid);
            }
            set { ApplicationData.Current.LocalSettings.Values["CompletedDisplayMode"] = (int) value; }
        }

        public static AnimeListDisplayModes OnHoldDisplayMode
        {
            get
            {
                return
                    (AnimeListDisplayModes)
                        (ApplicationData.Current.LocalSettings.Values["OnHoldDisplayMode"] ??
                         AnimeListDisplayModes.IndefiniteGrid);
            }
            set { ApplicationData.Current.LocalSettings.Values["OnHoldDisplayMode"] = (int) value; }
        }

        public static AnimeListDisplayModes DroppedDisplayMode
        {
            get
            {
                return
                    (AnimeListDisplayModes)
                        (ApplicationData.Current.LocalSettings.Values["DroppedDisplayMode"] ??
                         AnimeListDisplayModes.IndefiniteGrid);
            }
            set { ApplicationData.Current.LocalSettings.Values["DroppedDisplayMode"] = (int) value; }
        }

        public static AnimeListDisplayModes PlannedDisplayMode
        {
            get
            {
                return
                    (AnimeListDisplayModes)
                        (ApplicationData.Current.LocalSettings.Values["PlannedDisplayMode"] ??
                         AnimeListDisplayModes.IndefiniteGrid);
            }
            set { ApplicationData.Current.LocalSettings.Values["PlannedDisplayMode"] = (int) value; }
        }

        public static AnimeListDisplayModes AllDisplayMode
        {
            get
            {
                return
                    (AnimeListDisplayModes)
                        (ApplicationData.Current.LocalSettings.Values["AllDisplayMode"] ??
                         AnimeListDisplayModes.IndefiniteGrid);
            }
            set { ApplicationData.Current.LocalSettings.Values["AllDisplayMode"] = (int) value; }
        }

        public static bool LockDisplayMode
        {
            get { return (bool) (ApplicationData.Current.LocalSettings.Values["LockDisplayMode"] ?? false); }
            set { ApplicationData.Current.LocalSettings.Values["LockDisplayMode"] = value; }
        }

        #endregion

        #region RatePopUp

        public static bool RatePopUpEnable
        {
            get { return (bool) (ApplicationData.Current.LocalSettings.Values["RatePopUpEnable"] ?? true); }
            set { ApplicationData.Current.LocalSettings.Values["RatePopUpEnable"] = value; }
        }

        public static int RatePopUpStartupCounter
        {
            get { return (int) (ApplicationData.Current.LocalSettings.Values["RatePopUpStartupCounter"] ?? 0); }
            set { ApplicationData.Current.LocalSettings.Values["RatePopUpStartupCounter"] = value; }
        }

        public static int DonatePopUpStartupCounter
        {
            get { return (int) (ApplicationData.Current.LocalSettings.Values["DonatePopUpStartupCounter"] ?? 0); }
            set { ApplicationData.Current.LocalSettings.Values["DonatePopUpStartupCounter"] = value; }
        }

        public static bool Donated
        {
            get { return (bool) (ApplicationData.Current.LocalSettings.Values["Donated"] ?? false); }
            set { ApplicationData.Current.LocalSettings.Values["Donated"] = value; }
        }

        #endregion

        #region StartEndDates

        public static bool SetStartDateOnWatching
        {
            get { return (bool) (ApplicationData.Current.LocalSettings.Values["SetStartDateOnWatching"] ?? true); }
            set { ApplicationData.Current.LocalSettings.Values["SetStartDateOnWatching"] = value; }
        }

        public static bool SetStartDateOnListAdd
        {
            get { return (bool) (ApplicationData.Current.LocalSettings.Values["SetStartDateOnListAdd"] ?? false); }
            set { ApplicationData.Current.LocalSettings.Values["SetStartDateOnListAdd"] = value; }
        }

        public static bool SetEndDateOnDropped
        {
            get { return (bool) (ApplicationData.Current.LocalSettings.Values["SetEndDateOnDropped"] ?? false); }
            set { ApplicationData.Current.LocalSettings.Values["SetEndDateOnDropped"] = value; }
        }

        public static bool SetEndDateOnCompleted
        {
            get { return (bool) (ApplicationData.Current.LocalSettings.Values["SetEndDateOnCompleted"] ?? true); }
            set { ApplicationData.Current.LocalSettings.Values["SetEndDateOnCompleted"] = value; }
        }

        public static bool OverrideValidStartEndDate
        {
            get { return (bool) (ApplicationData.Current.LocalSettings.Values["OverrideValidStartEndDate"] ?? false); }
            set { ApplicationData.Current.LocalSettings.Values["OverrideValidStartEndDate"] = value; }
        }

        #endregion

        #region Calendar

        public static bool CalendarIncludeWatching
        {
            get { return (bool) (ApplicationData.Current.LocalSettings.Values["CalendarIncludeWatching"] ?? true); }
            set { ApplicationData.Current.LocalSettings.Values["CalendarIncludeWatching"] = value; }
        }

        public static bool CalendarIncludePlanned
        {
            get { return (bool) (ApplicationData.Current.LocalSettings.Values["CalendarIncludePlanned"] ?? false); }
            set { ApplicationData.Current.LocalSettings.Values["CalendarIncludePlanned"] = value; }
        }

        public static bool CalendarSwitchMonSun
        {
            get { return (bool) (ApplicationData.Current.LocalSettings.Values["CalendarSwitchMonSun"] ?? false); }
            set { ApplicationData.Current.LocalSettings.Values["CalendarSwitchMonSun"] = value; }
        }

        public static bool CalendarStartOnToday
        {
            get { return (bool) (ApplicationData.Current.LocalSettings.Values["CalendarStartOnToday"] ?? false); }
            set { ApplicationData.Current.LocalSettings.Values["CalendarStartOnToday"] = value; }
        }

        public static bool CalendarRemoveEmptyDays
        {
            get { return (bool) (ApplicationData.Current.LocalSettings.Values["CalendarRemoveEmptyDays"] ?? true); }
            set { ApplicationData.Current.LocalSettings.Values["CalendarRemoveEmptyDays"] = value; }
        }

        #endregion

        #region Articles

        public static bool ArticlesLaunchExternalLinks
        {
            get { return (bool) (ApplicationData.Current.LocalSettings.Values["ArticlesLaunchExternalLinks"] ?? true); }
            set { ApplicationData.Current.LocalSettings.Values["ArticlesLaunchExternalLinks"] = value; }
        }

        public static bool ArticlesDisplayScrollBar
        {
            get { return (bool) (ApplicationData.Current.LocalSettings.Values["ArticlesDisplayScrollBar"] ?? false); }
            set { ApplicationData.Current.LocalSettings.Values["ArticlesDisplayScrollBar"] = value; }
        }

        #endregion

        #region Favs

        public static bool SyncFavsFromTimeToTime
        {
            get { return (bool)(ApplicationData.Current.LocalSettings.Values["SyncFavsFromTimeToTime"] ?? true); }
            set { ApplicationData.Current.LocalSettings.Values["SyncFavsFromTimeToTime"] = value; }
        }

        public static int LastFavTimeSync //ticks
        {
            get { return (int)(ApplicationData.Current.LocalSettings.Values["LastFavTimeSync"] ?? 0); }
            set { ApplicationData.Current.LocalSettings.Values["LastFavTimeSync"] = value; }
        }

        #endregion

        #region Forums

        public static string ForumsPinnedBoards
        {
            get { return (string) (ApplicationData.Current.LocalSettings.Values["ForumsPinnedBoards"] ?? ""); }
            set { ApplicationData.Current.LocalSettings.Values["ForumsPinnedBoards"] = value; }
        }

        #endregion

    }
}