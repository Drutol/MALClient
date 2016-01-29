using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MALClient.Comm;
using MALClient.Items;
using MALClient.Pages;

namespace MALClient
{
    public static class Utils
    {
        public static string StatusToString(int status)
        {
            switch (status)
            {
                case 1:
                    return "Watching";
                case 2:
                    return "Completed";
                case 3:
                    return "On hold";
                case 4:
                    return "Dropped";
                case 6:
                    return "Plan to watch";
                case 7:
                    return "All";
                case 8:
                    return "Airing";
                default:
                    return "Not Set";
            }
        }

        public static int StatusToInt(string status)
        {
            switch (status)
            {
                case "Watching":
                    return 1;
                case "Completed":
                    return 2;
                case "On hold":
                    return 3;
                case "Dropped":
                    return 4;
                case "Plan to watch":
                    return 6;
                case "All":
                    return 7;
                case "Airing":
                    return 8;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static string DayToString(DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Friday:
                    return "Fri";
                case DayOfWeek.Monday:
                    return "Mon";
                case DayOfWeek.Saturday:
                    return "Sat";
                case DayOfWeek.Sunday:
                    return "Sun";
                case DayOfWeek.Thursday:
                    return "Thu";
                case DayOfWeek.Tuesday:
                    return "Tue";
                case DayOfWeek.Wednesday:
                    return "Wed";
                default:
                    throw new ArgumentOutOfRangeException(nameof(day), day, null);
            }
        }

        public static void RegisterTile(string id)
        {
            var tiles = (string) ApplicationData.Current.LocalSettings.Values["tiles"];
            if (String.IsNullOrWhiteSpace(tiles))
                tiles = "";
            tiles += id + ";";
            ApplicationData.Current.LocalSettings.Values["tiles"] = tiles;
        }

        public static async void CheckTiles()
        {
            string tiles = (string) ApplicationData.Current.LocalSettings.Values["tiles"];
            if (String.IsNullOrWhiteSpace(tiles))
                return;


            string newTiles = "";
            foreach (var tileId in tiles.Split(';'))
            {
                if (!SecondaryTile.Exists(tileId))
                {
                    try
                    {
                        var file = await ApplicationData.Current.LocalFolder.GetFileAsync($"{tileId}.png");
                        await file.DeleteAsync();
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
                else
                {
                    newTiles += tileId + ";";
                }
            }
            ApplicationData.Current.LocalSettings.Values["tiles"] = newTiles;
        }

        public static MainPage GetMainPageInstance()
        {
            var frame = (Frame) Window.Current.Content;
            return (MainPage) frame.Content;
        }

        public static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }

        public static int ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return (int) Math.Floor(diff.TotalSeconds);
        }

        public static int GetCachePersitence()
        {
            return (int) (ApplicationData.Current.LocalSettings.Values["CachePersistency"] ?? 3600);
        }

        public static bool IsCachingEnabled()
        {
            return (bool) (ApplicationData.Current.LocalSettings.Values["EnableCache"] ?? false);
        }

        public static AnimeListPage.SortOptions GetSortOrder()
        {
            return (AnimeListPage.SortOptions) (int) (ApplicationData.Current.LocalSettings.Values["SortOrder"] ?? 3);
        }

        public static bool IsSortDescending()
        {
            return (bool) (ApplicationData.Current.LocalSettings.Values["SortDescending"] ?? true);
        }

        internal static int GetDefaultAnimeFilter()
        {
            return (int) (ApplicationData.Current.LocalSettings.Values["DefaultFilter"] ?? (int) AnimeStatus.Watching);
        }

        public static int GetItemsPerPage()
        {
            return (int) (ApplicationData.Current.LocalSettings.Values["ItemsPerPage"] ?? 10);
        }

        static readonly string[] SizeSuffixes = {"bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"};

        /// <summary>
        /// http://stackoverflow.com/questions/14488796/does-net-provide-an-easy-way-convert-bytes-to-kb-mb-gb-etc
        /// </summary>
        public static string SizeSuffix(long value)
        {
            if (value < 0)
            {
                return "-" + SizeSuffix(-value);
            }
            if (value == 0)
            {
                return "0.0 bytes";
            }

            int mag = (int) Math.Log(value, 1024);
            decimal adjustedSize = (decimal) value/(1L << (mag*10));

            return string.Format("{0:n1} {1}", adjustedSize, SizeSuffixes[mag]);
        }

        public static async void DownloadProfileImg(int retries = 5)
        {
            try
            {
                var folder = ApplicationData.Current.LocalFolder;
                var thumb = await folder.CreateFileAsync("UserImg.png", CreationCollisionOption.ReplaceExisting);

                HttpClient http = new HttpClient();
                byte[] response = await http.GetByteArrayAsync($"http://cdn.myanimelist.net/images/userimages/{Creditentials.Id}.jpg"); //get bytes

                var fs = await thumb.OpenStreamForWriteAsync(); //get stream

                using (DataWriter writer = new DataWriter(fs.AsOutputStream()))
                {
                    writer.WriteBytes(response); //write
                    await writer.StoreAsync();
                    await writer.FlushAsync();
                }
                GetMainPageInstance().UpdateHamburger();
            }
            catch (Exception)
            {
                if (retries >= 0)
                {
                    await Task.Delay(1000);
                    DownloadProfileImg(retries - 1);
                }
            }
        }

        #region BackNavigation

        private static PageIndex _pageTo;
        private static object _args;

        public static void RegisterBackNav(PageIndex page, object args)
        {
            _pageTo = page;
            _args = args;
            var currentView = SystemNavigationManager.GetForCurrentView();
            currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            currentView.BackRequested += CurrentViewOnBackRequested;
        }

        private static void CurrentViewOnBackRequested(object sender, BackRequestedEventArgs args)
        {
            args.Handled = true;
            GetMainPageInstance().Navigate(_pageTo, _args);
        }

        public static void DeregisterBackNav()
        {
            var currentView = SystemNavigationManager.GetForCurrentView();
            currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            currentView.BackRequested -= CurrentViewOnBackRequested;
            _args = null;
        }

        #endregion
    }
}
