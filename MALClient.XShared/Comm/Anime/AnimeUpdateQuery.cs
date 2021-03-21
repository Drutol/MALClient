using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MALClient.Adapters;
using MALClient.Models.Enums;
using MALClient.Models.Models.Library;
using MALClient.XShared.Utils;
using MALClient.XShared.Utils.Managers;
using MALClient.XShared.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MALClient.XShared.Comm.Anime
{
    public class AnimeUpdateQuery : Query
    {
        private readonly int? _rewatched;
        private readonly IAnimeData _item;
        public static bool SuppressOfflineSync { get; set; }
        public static bool UpdatedSomething { get; set; } //used for data saving on suspending in app.xaml.cs
        private static SemaphoreSlim _updateSemaphore = new SemaphoreSlim(1);

        /// <summary>
        /// Just send rewatched value witch cannot be retrieved back
        /// </summary>
        /// <param name="item"></param>
        /// <param name="rewatched"></param>
        public AnimeUpdateQuery(IAnimeData item, int? rewatched)
        {
            _item = item;
            _rewatched = rewatched;
        }



        public AnimeUpdateQuery(IAnimeData item)
            : this(item.Id, item.MyEpisodes, (int)item.MyStatus, item.MyScore, item.StartDate, item.EndDate, item.Notes,item.IsRewatching)
        {
            _item = item;
            try
            {
                ResourceLocator.LiveTilesManager.UpdateTile(item);
            }
            catch (Exception)
            {
                //not windows
            }
        }


        private AnimeUpdateQuery(int id, int watchedEps, int myStatus, float myScore, string startDate, string endDate, string notes,bool rewatching)
        {
            UpdatedSomething = true;
        }

        public override async Task<string> GetRequestResponse()
        {

            try
            {
                await _updateSemaphore.WaitAsync();
                var result = "";
                try
                {
                    var client = await ResourceLocator.MalHttpContextProvider.GetApiHttpContextAsync();

                    var dateStart = FormatDate(_item.StartDate);
                    var dateEnd = FormatDate(_item.EndDate);

                    var data = new List<KeyValuePair<string, string>>
                    {
                        new("status", ToApiParam(_item.MyStatus)),
                        new("is_rewatching", _item.IsRewatching.ToString().ToLower()),
                        new("score", _item.MyScore.ToString()),
                        new("num_watched_episodes", _item.MyEpisodes.ToString()),
                        //new("priority", ((int) _item.Priority).ToString()),
                        new("tags", _item.Notes),
                    };

                    if (_rewatched != null)
                    {
                        data.Add(new KeyValuePair<string, string>("num_times_rewatched", _rewatched.Value.ToString()));
                    }

                    if (dateStart != null)
                    {
                        data.Add(new KeyValuePair<string, string>("start_date", dateStart));
                    }      
                    
                    if (dateEnd != null)
                    {
                        data.Add(new KeyValuePair<string, string>("finish_date", dateEnd));
                    }

                    using var content = new FormUrlEncodedContent(data);
                    var response = await client.SendAsync(new HttpRequestMessage(new HttpMethod("PUT"),
                        $"https://api.myanimelist.net/v2/anime/{_item.Id}/my_list_status") {Content = content});

                    response.EnsureSuccessStatusCode();

                    if (response.IsSuccessStatusCode)
                        result = "Updated";

                    //var updateHtml = await
                    //    client.GetStreamAsync($"https://myanimelist.net/ownlist/anime/{_item.Id}/edit?hideLayout=");
                    //var doc = new HtmlDocument();
                    //doc.Load(updateHtml);

                    ////var priority = doc.DocumentNode
                    ////                   .FirstOfDescendantsWithId("select", "add_anime_priority")
                    ////                   .Descendants("option")
                    ////                   .First(node => node.Attributes.Contains("selected"))?
                    ////                   .Attributes["value"].Value ?? "0";

                    //var storage = doc.DocumentNode
                    //    .FirstOfDescendantsWithId("select", "add_anime_storage_type")
                    //    .Descendants("option")
                    //    .FirstOrDefault(node => node.Attributes.Contains("selected"))?
                    //    .Attributes["value"].Value;

                    //var storageValue = doc.DocumentNode
                    //    .FirstOfDescendantsWithId("input", "add_anime_storage_value")
                    //    .Attributes["value"].Value;

                    //var rewatches = doc.DocumentNode
                    //    .FirstOfDescendantsWithId("input", "add_anime_num_watched_times")
                    //    .Attributes["value"].Value;

                    //var rewatchValue = doc.DocumentNode
                    //    .FirstOfDescendantsWithId("select", "add_anime_rewatch_value")
                    //    .Descendants("option")
                    //    .FirstOrDefault(node => node.Attributes.Contains("selected"))?
                    //    .Attributes["value"].Value;

                    //var comments = doc.DocumentNode
                    //    .FirstOfDescendantsWithId("textarea", "add_anime_comments").InnerText;

                    //var askDiscuss = doc.DocumentNode
                    //                     .FirstOfDescendantsWithId("select", "add_anime_is_asked_to_discuss")
                    //                     .Descendants("option")
                    //                     .FirstOrDefault(node => node.Attributes.Contains("selected"))?
                    //                     .Attributes["value"].Value ?? "0";

                    //var postSns = doc.DocumentNode
                    //                  .FirstOfDescendantsWithId("select", "add_anime_sns_post_type")
                    //                  .Descendants("option")
                    //                  .FirstOrDefault(node => node.Attributes.Contains("selected"))?
                    //                  .Attributes["value"].Value ?? "0";


                    //var content = new Dictionary<string, string>
                    //{
                    //    ["anime_id"] = _item.Id.ToString(),
                    //    ["add_anime[status]"] = ((int) _item.MyStatus).ToString(),
                    //    ["add_anime[score]"] = _item.MyScore == 0 ? null : _item.MyScore.ToString(),
                    //    ["add_anime[num_watched_episodes]"] = _item.MyEpisodes.ToString(),
                    //    ["add_anime[tags]"] = string.IsNullOrEmpty(_item.Notes) ? "" : _item.Notes,
                    //    ["add_anime[priority]"] = ((int)_item.Priority).ToString(),

                    //    ["csrf_token"] = client.Token,

                    //    ["add_anime[storage_type]"] = storage,
                    //    ["add_anime[storage_value]"] = storageValue,
                    //    ["add_anime[num_watched_times]"] = rewatches,
                    //    ["add_anime[rewatch_value]"] = rewatchValue,
                    //    ["add_anime[comments]"] = comments,
                    //    ["add_anime[is_asked_to_discuss]"] = askDiscuss,
                    //    ["add_anime[sns_post_type]"] = postSns,

                    //};



                    //if (_item.IsRewatching)
                    //    content["add_anime[is_rewatching]"] = "1";

                    //if (_item.StartDate != null)
                    //{
                    //    content["add_anime[start_date][month]"] = _item.StartDate.Substring(5, 2).TrimStart('0');
                    //    content["add_anime[start_date][day]"] = _item.StartDate.Substring(8, 2).TrimStart('0');
                    //    content["add_anime[start_date][year]"] = _item.StartDate.Substring(0, 4).Replace("0000","");
                    //}

                    //if (_item.EndDate != null)
                    //{
                    //    content["add_anime[finish_date][month]"] = _item.EndDate.Substring(5, 2).TrimStart('0');
                    //    content["add_anime[finish_date][day]"] = _item.EndDate.Substring(8, 2).TrimStart('0');
                    //    content["add_anime[finish_date][year]"] = _item.EndDate.Substring(0, 4).Replace("0000","");
                    //}

                    //var response = await client.PostAsync(
                    //    $"https://myanimelist.net/ownlist/anime/{_item.Id}/edit?hideLayout",
                    //    new FormUrlEncodedContent(content));
                    //if (!(await response.Content.ReadAsStringAsync()).Contains("badresult"))
                    //    result = "Updated";
                }
                catch (Exception e)
                {
                    ResourceLocator.SnackbarProvider.ShowText("Failed to send update to MAL. Please try signing in again if problem persists.");
                }

                if (string.IsNullOrEmpty(result) && !SuppressOfflineSync && Settings.EnableOfflineSync)
                {
                    result = "Updated";
                    Settings.AnimeSyncRequired = true;
                }

                ResourceLocator.ApplicationDataService[RoamingDataTypes.LastLibraryUpdate] = DateTime.Now.ToBinary();
                return result;
            }
            finally
            {
                _updateSemaphore.Release();
            }
        }

        private string FormatDate(string date)
        {
            if (date == null)
                return null;

            var month = date.Substring(5, 2);
            var day = date.Substring(8, 2);
            var year = date.Substring(0, 4);

            return $"{year}-{month}-{day}";
        }

        private string ToApiParam(AnimeStatus itemMyStatus)
        {
            return itemMyStatus switch
            {
                AnimeStatus.Watching => "watching",
                AnimeStatus.Completed => "completed",
                AnimeStatus.OnHold => "on_hold",
                AnimeStatus.Dropped => "dropped",
                AnimeStatus.PlanToWatch => "plan_to_watch",
                _ => throw new ArgumentOutOfRangeException(nameof(itemMyStatus), itemMyStatus, null)
            };
        }

        public override string SnackbarMessageOnFail => "Your changes will be synced with MAL on next app launch when online.";

        private void UpdateAnimeHummingbird(int id, int watchedEps, int myStatus, float myScore, string startDate,
            string endDate)
        {
            Request =
                WebRequest.Create(
                    Uri.EscapeUriString(
                        $"http://hummingbird.me/api/v1/libraries/{id}?auth_token={Credentials.HummingbirdToken}&episodes_watched={watchedEps}&rating={myScore}&status={AnimeStatusToHum((AnimeStatus) myStatus)}"));
            Request.ContentType = "application/x-www-form-urlencoded";
            {
                Request.Method = "POST";
            }
        }

        private static string AnimeStatusToHum(AnimeStatus status)
        {
            switch (status)
            {
                case AnimeStatus.Watching:
                    return "currently-watching";
                case AnimeStatus.PlanToWatch:
                    return "plan-to-watch";
                case AnimeStatus.Completed:
                    return "completed";
                case AnimeStatus.OnHold:
                    return "on-hold";
                case AnimeStatus.Dropped:
                    return "dropped";
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }
    }
}