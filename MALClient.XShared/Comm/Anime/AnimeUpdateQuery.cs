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
        private readonly IAnimeData _item;
        public static bool SuppressOfflineSync { get; set; }
        public static bool UpdatedSomething { get; set; } //used for data saving on suspending in app.xaml.cs
        private static SemaphoreSlim _updateSemaphore = new SemaphoreSlim(1);


        public AnimeUpdateQuery(IAnimeData item, int rewatchCount = 0)
        {
            _item = item;
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
                    var client = await ResourceLocator.MalHttpContextProvider.GetHttpContextAsync();

                    var updateHtml = await
                        client.GetStreamAsync($"https://myanimelist.net/ownlist/anime/{_item.Id}/edit?hideLayout=");
                    var doc = new HtmlDocument();
                    doc.Load(updateHtml);

                    //var priority = doc.DocumentNode
                    //                   .FirstOfDescendantsWithId("select", "add_anime_priority")
                    //                   .Descendants("option")
                    //                   .First(node => node.Attributes.Contains("selected"))?
                    //                   .Attributes["value"].Value ?? "0";

                    var storage = doc.DocumentNode
                        .FirstOfDescendantsWithId("select", "add_anime_storage_type")
                        .Descendants("option")
                        .FirstOrDefault(node => node.Attributes.Contains("selected"))?
                        .Attributes["value"].Value;

                    var storageValue = doc.DocumentNode
                        .FirstOfDescendantsWithId("input", "add_anime_storage_value")
                        .Attributes["value"].Value;

                    var rewatches = doc.DocumentNode
                        .FirstOfDescendantsWithId("input", "add_anime_num_watched_times")
                        .Attributes["value"].Value;

                    var rewatchValue = doc.DocumentNode
                        .FirstOfDescendantsWithId("select", "add_anime_rewatch_value")
                        .Descendants("option")
                        .FirstOrDefault(node => node.Attributes.Contains("selected"))?
                        .Attributes["value"].Value;

                    var comments = doc.DocumentNode
                        .FirstOfDescendantsWithId("textarea", "add_anime_comments").InnerText;

                    var askDiscuss = doc.DocumentNode
                                         .FirstOfDescendantsWithId("select", "add_anime_is_asked_to_discuss")
                                         .Descendants("option")
                                         .FirstOrDefault(node => node.Attributes.Contains("selected"))?
                                         .Attributes["value"].Value ?? "0";

                    var postSns = doc.DocumentNode
                                      .FirstOfDescendantsWithId("select", "add_anime_sns_post_type")
                                      .Descendants("option")
                                      .FirstOrDefault(node => node.Attributes.Contains("selected"))?
                                      .Attributes["value"].Value ?? "0";


                    var content = new Dictionary<string, string>
                    {
                        ["anime_id"] = _item.Id.ToString(),
                        ["add_anime[status]"] = ((int) _item.MyStatus).ToString(),
                        ["add_anime[score]"] = _item.MyScore == 0 ? null : _item.MyScore.ToString(),
                        ["add_anime[num_watched_episodes]"] = _item.MyEpisodes.ToString(),
                        ["add_anime[tags]"] = string.IsNullOrEmpty(_item.Notes) ? "" : _item.Notes,
                        ["add_anime[priority]"] = ((int)_item.Priority).ToString(),

                        ["csrf_token"] = client.Token,

                        ["add_anime[storage_type]"] = storage,
                        ["add_anime[storage_value]"] = storageValue,
                        ["add_anime[num_watched_times]"] = rewatches,
                        ["add_anime[rewatch_value]"] = rewatchValue,
                        ["add_anime[comments]"] = comments,
                        ["add_anime[is_asked_to_discuss]"] = askDiscuss,
                        ["add_anime[sns_post_type]"] = postSns,

                    };



                    if (_item.IsRewatching)
                        content["add_anime[is_rewatching]"] = "1";

                    if (_item.StartDate != null)
                    {
                        content["add_anime[start_date][month]"] = _item.StartDate.Substring(5, 2).TrimStart('0');
                        content["add_anime[start_date][day]"] = _item.StartDate.Substring(8, 2).TrimStart('0');
                        content["add_anime[start_date][year]"] = _item.StartDate.Substring(0, 4).Replace("0000","");
                    }

                    if (_item.EndDate != null)
                    {
                        content["add_anime[finish_date][month]"] = _item.EndDate.Substring(5, 2).TrimStart('0');
                        content["add_anime[finish_date][day]"] = _item.EndDate.Substring(8, 2).TrimStart('0');
                        content["add_anime[finish_date][year]"] = _item.EndDate.Substring(0, 4).Replace("0000","");
                    }

                    var response = await client.PostAsync(
                        $"https://myanimelist.net/ownlist/anime/{_item.Id}/edit?hideLayout",
                        new FormUrlEncodedContent(content));
                    if (!(await response.Content.ReadAsStringAsync()).Contains("badresult"))
                        result = "Updated";
                }
                catch (Exception e)
                {
#if ANDROID
                ResourceLocator.SnackbarProvider.ShowText("Failed to send update to MAL.");
#endif
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

        public override string SnackbarMessageOnFail => "Your changes will be synced with MAL on next app launch when online.";
    }
}