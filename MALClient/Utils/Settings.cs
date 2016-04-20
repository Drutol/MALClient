using Windows.Storage;
using Windows.UI.Xaml;
using MALClient.Models;
using MALClient.Pages;
using MALClient.ViewModels;

namespace MALClient
{
    public static class Settings
    {
        public static int CachePersitence
        {
            get { return (int) (ApplicationData.Current.LocalSettings.Values["CachePersistency"] ?? 3600); }
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
                return (bool) (ApplicationData.Current.LocalSettings.Values["EnableCache"] ?? false);
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

        public static int ItemsPerPage
        {
            get { return (int) (ApplicationData.Current.LocalSettings.Values["ItemsPerPage"] ?? 5); }
            set { ApplicationData.Current.LocalSettings.Values["ItemsPerPage"] = value; }
        }

        public static int ReviewsToPull
        {
            get { return (int) (ApplicationData.Current.LocalSettings.Values["ReviewsToPull"] ?? 4); }
            set { ApplicationData.Current.LocalSettings.Values["ReviewsToPull"] = value; }
        }

        public static int RecommsToPull
        {
            get { return (int) (ApplicationData.Current.LocalSettings.Values["RecommsToPull"] ?? 8); }
            set { ApplicationData.Current.LocalSettings.Values["RecommsToPull"] = value; }
        }

        public static int SeasonalToPull
        {
            get { return (int) (ApplicationData.Current.LocalSettings.Values["SeasonalToPull"] ?? 30); }
            set { ApplicationData.Current.LocalSettings.Values["SeasonalToPull"] = value; }
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
            get { return (bool) (ApplicationData.Current.LocalSettings.Values["HamburgerAnimeFiltersExpanded"] ?? true); }
            set { ApplicationData.Current.LocalSettings.Values["HamburgerAnimeFiltersExpanded"] = value; }
        }

        public static bool HamburgerMangaFiltersExpanded
        {
            get { return (bool) (ApplicationData.Current.LocalSettings.Values["HamburgerMangaFiltersExpanded"] ?? false); }
            set { ApplicationData.Current.LocalSettings.Values["HamburgerMangaFiltersExpanded"] = value; }
        }

        public static bool HamburgerMenuDefaultPaneState
        {
            get { return (bool) (ApplicationData.Current.LocalSettings.Values["HamburgerMenuDefaultPaneState"] ?? true); }
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

        #region Views

        public static AnimeListDisplayModes WatchingDisplayMode
        {
            get
            {
                return
                    (AnimeListDisplayModes)
                        (ApplicationData.Current.LocalSettings.Values["WatchingDisplayMode"] ??
                         AnimeListDisplayModes.IndefiniteList);
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
                         AnimeListDisplayModes.IndefiniteList);
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
                         AnimeListDisplayModes.IndefiniteList);
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
                         AnimeListDisplayModes.IndefiniteList);
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

        #endregion

        #region StartEndDates
        public static bool SetStartDateOnWatching
        {
            get { return (bool)(ApplicationData.Current.LocalSettings.Values["SetStartDateOnWatching"] ?? false); }
            set { ApplicationData.Current.LocalSettings.Values["SetStartDateOnWatching"] = value; }
        }

        public static bool SetStartDateOnListAdd
        {
            get { return (bool)(ApplicationData.Current.LocalSettings.Values["SetStartDateOnListAdd"] ?? false); }
            set { ApplicationData.Current.LocalSettings.Values["SetStartDateOnListAdd"] = value; }
        }

        public static bool SetEndDateOnDropped
        {
            get { return (bool)(ApplicationData.Current.LocalSettings.Values["SetEndDateOnDropped"] ?? false); }
            set { ApplicationData.Current.LocalSettings.Values["SetEndDateOnDropped"] = value; }
        }

        public static bool SetEndDateOnCompleted
        {
            get { return (bool)(ApplicationData.Current.LocalSettings.Values["SetEndDateOnCompleted"] ?? false); }
            set { ApplicationData.Current.LocalSettings.Values["SetEndDateOnCompleted"] = value; }
        }

        public static bool OverrideValidStartEndDate
        {
            get { return (bool)(ApplicationData.Current.LocalSettings.Values["OverrideValidStartEndDate"] ?? false); }
            set { ApplicationData.Current.LocalSettings.Values["OverrideValidStartEndDate"] = value; }
        }

        #endregion
    }
}