using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using MALClient.Pages;

namespace MALClient
{
    public static class Settings
    {
        public static int GetCachePersitence()
        {
            return (int)(ApplicationData.Current.LocalSettings.Values["CachePersistency"] ?? 3600);
        }

        public static bool IsCachingEnabled()
        {
            return (bool)(ApplicationData.Current.LocalSettings.Values["EnableCache"] ?? false);
        }

        public static SortOptions GetSortOrder()
        {
            return (SortOptions)(int)(ApplicationData.Current.LocalSettings.Values["SortOrder"] ?? 3);
        }

        public static SortOptions GetSortOrderM()
        {
            return (SortOptions)(int)(ApplicationData.Current.LocalSettings.Values["SortOrderM"] ?? 3);
        }

        public static bool IsSortDescending()
        {
            return (bool)(ApplicationData.Current.LocalSettings.Values["SortDescending"] ?? true);
        }

        public static bool IsSortDescendingM()
        {
            return (bool)(ApplicationData.Current.LocalSettings.Values["SortDescendingM"] ?? true);
        }

        public static int GetDefaultAnimeFilter()
        {
            return (int)(ApplicationData.Current.LocalSettings.Values["DefaultFilter"] ?? (int)AnimeStatus.Watching);
        }

        public static int GetDefaultMangaFilter()
        {
            return (int)(ApplicationData.Current.LocalSettings.Values["DefaultFilterM"] ?? (int)AnimeStatus.Watching);
        }

        public static int GetItemsPerPage()
        {
            return (int)(ApplicationData.Current.LocalSettings.Values["ItemsPerPage"] ?? 5);
        }

        public static int GetReviewsToPull()
        {
            return (int)(ApplicationData.Current.LocalSettings.Values["ReviewsToPull"] ?? 4);
        }

        public static int GetRecommsToPull()
        {
            return (int)(ApplicationData.Current.LocalSettings.Values["RecommsToPull"] ?? 8);
        }

        public static string GetDefaultMenuTab()
        {
            return (string)(ApplicationData.Current.LocalSettings.Values["DefaultMenuTab"] ?? "anime");
        }
    }
}
