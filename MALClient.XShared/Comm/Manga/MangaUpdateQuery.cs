using System;
using System.Collections.Generic;
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
using MALClient.XShared.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MALClient.XShared.Comm.Manga
{
    public class MangaUpdateQuery : Query
    {
        private readonly IAnimeData _item;
        private SemaphoreSlim _updateSemaphore = new SemaphoreSlim(1);
        public static bool SuppressOfflineSync { get; set; }
        public static bool UpdatedSomething { get; set; } //used for data saving on suspending in app.xaml.cs

        /// <summary>
        /// Just send rewatched value witch cannot be retrieved back
        /// </summary>
        /// <param name="item"></param>
        /// <param name="rewatched"></param>
        public MangaUpdateQuery(IAnimeData item, int rewatched)
        {
            _item = item;
            var xml = new StringBuilder();
            xml.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xml.AppendLine("<entry>");
            xml.AppendLine($"<times_reread>{rewatched}</times_reread>");
            xml.AppendLine("</entry>");

            Request =
                WebRequest.Create(Uri.EscapeUriString($"https://myanimelist.net/api/mangalist/update/{item.Id}.xml?data={xml}"));
            Request.Credentials = Credentials.GetHttpCreditentials();
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }

        public MangaUpdateQuery(IAnimeData item)
            : this(
                item.Id, item.MyEpisodes, (int)item.MyStatus, (int) item.MyScore, item.MyVolumes, item.StartDate,
                item.EndDate,item.Notes,item.IsRewatching)
        {
            _item = item;
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
                    var data = new List<KeyValuePair<string, string>>
                    {
                        new("status", ToApiParam(_item.MyStatus)),
                        new("is_rereading", _item.IsRewatching.ToString().ToLower()),
                        new("score", _item.MyScore.ToString()),
                        new("num_chapters_read", _item.MyEpisodes.ToString()),
                        new("num_volumes_read", _item.MyVolumes.ToString()),
                        //new("priority", ((int) _item.Priority).ToString()),
                        new("tags", _item.Notes),
                    };
                    using var content = new FormUrlEncodedContent(data);
                    var response = await client.SendAsync(new HttpRequestMessage(new HttpMethod("PUT"),
                            $"https://api.myanimelist.net/v2/manga/{_item.Id}/my_list_status")
                        { Content = content });

                    if (response.IsSuccessStatusCode)
                        result = "Updated";

                    //var updateHtml = await
                    //    client.GetStreamAsync($"https://myanimelist.net/ownlist/manga/{_item.Id}/edit?hideLayout=");
                    //var doc = new HtmlDocument();
                    //doc.Load(updateHtml);

                    ////var priority = doc.DocumentNode
                    ////                   .FirstOfDescendantsWithId("select", "add_manga_priority")
                    ////                   .Descendants("option")
                    ////                   .First(node => node.Attributes.Contains("selected"))?
                    ////                   .Attributes["value"].Value ?? "0";

                    //var storage = doc.DocumentNode
                    //    .FirstOfDescendantsWithId("select", "add_manga_storage_type")
                    //    .Descendants("option")
                    //    .FirstOrDefault(node => node.Attributes.Contains("selected"))?
                    //    .Attributes["value"].Value;

                    //var storageValue = doc.DocumentNode
                    //    .FirstOfDescendantsWithId("input", "add_manga_num_retail_volumes")
                    //    .Attributes["value"].Value;

                    //var rewatches = doc.DocumentNode
                    //    .FirstOfDescendantsWithId("input", "add_manga_num_read_times")
                    //    .Attributes["value"].Value;

                    //var rewatchValue = doc.DocumentNode
                    //    .FirstOfDescendantsWithId("select", "add_manga_reread_value")
                    //    .Descendants("option")
                    //    .FirstOrDefault(node => node.Attributes.Contains("selected"))?
                    //    .Attributes["value"].Value;

                    //var comments = doc.DocumentNode
                    //    .FirstOfDescendantsWithId("textarea", "add_manga_comments").InnerText;

                    //var askDiscuss = doc.DocumentNode
                    //                     .FirstOfDescendantsWithId("select", "add_manga_is_asked_to_discuss")
                    //                     .Descendants("option")
                    //                     .FirstOrDefault(node => node.Attributes.Contains("selected"))?
                    //                     .Attributes["value"].Value ?? "0";

                    //var postSns = doc.DocumentNode
                    //                  .FirstOfDescendantsWithId("select", "add_manga_sns_post_type")
                    //                  .Descendants("option")
                    //                  .FirstOrDefault(node => node.Attributes.Contains("selected"))?
                    //                  .Attributes["value"].Value ?? "0";


                    //var content = new Dictionary<string, string>
                    //{
                    //    ["entry_id"] = "0",
                    //    ["manga_id"] = _item.Id.ToString(),
                    //    ["add_manga[status]"] = ((int)_item.MyStatus).ToString(),
                    //    ["add_manga[score]"] = _item.MyScore == 0 ? null : _item.MyScore.ToString(),
                    //    ["add_manga[num_read_chapters]"] = _item.MyEpisodes.ToString(),
                    //    ["add_manga[num_read_volumes]"] = _item.MyVolumes.ToString(),
                    //    ["add_manga[tags]"] = string.IsNullOrEmpty(_item.Notes) ? "" : _item.Notes,
                    //    ["add_manga[priority]"] = ((int)_item.Priority).ToString(),

                    //    ["csrf_token"] = client.Token,

                    //    //["add_manga[priority]"] = priority,
                    //    ["add_manga[storage_type]"] = storage,
                    //    ["add_manga[num_retail_volumes]"] = storageValue,
                    //    ["add_manga[num_read_times]"] = rewatches,
                    //    ["add_manga[reread_value]"] = rewatchValue,
                    //    ["add_manga[comments]"] = comments,
                    //    ["add_manga[is_asked_to_discuss]"] = askDiscuss,
                    //    ["add_manga[sns_post_type]"] = postSns,

                    //    ["submitIt"] = "0",
                    //    ["last_completed_vol"] = "",

                    //};

                    //if (_item.IsRewatching)
                    //    content["add_manga[is_rereading]"] = "1";

                    //if (_item.StartDate != null)
                    //{
                    //    content["add_manga[start_date][month]"] = _item.StartDate.Substring(5, 2).TrimStart('0');
                    //    content["add_manga[start_date][day]"] = _item.StartDate.Substring(8, 2).TrimStart('0');
                    //    content["add_manga[start_date][year]"] = _item.StartDate.Substring(0, 4).Replace("0000", "");
                    //}

                    //if (_item.EndDate != null)
                    //{
                    //    content["add_manga[finish_date][month]"] = _item.EndDate.Substring(5, 2).TrimStart('0');
                    //    content["add_manga[finish_date][day]"] = _item.EndDate.Substring(8, 2).TrimStart('0');
                    //    content["add_manga[finish_date][year]"] = _item.EndDate.Substring(0, 4).Replace("0000", "");
                    //}

                    //var response = await client.PostAsync(
                    //    $"https://myanimelist.net/ownlist/manga/{_item.Id}/edit?hideLayout",
                    //    new FormUrlEncodedContent(content));
                    //if (!(await response.Content.ReadAsStringAsync()).Contains("badresult"))
                    //    result = "Updated";
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



            //try
            //{
            //    var result = "";
            //    try
            //    {
            //        var client = await ResourceLocator.MalHttpContextProvider.GetHttpContextAsync();
            //        client.DefaultRequestHeaders.Add("X-Requested-With", new[] {"XMLHttpRequest"});

            //        var response = await client.PostAsync("https://myanimelist.net/ownlist/manga/edit.json",
            //            new StringContent(new JObject
            //            {
            //                ["manga_id"] = _item.Id,
            //                ["status"] = (int) _item.MyStatus,
            //                ["score"] = (int) _item.MyScore,
            //                ["num_read_volumes"] = _item.MyVolumes,
            //                ["num_read_chapters"] = _item.MyEpisodes,
            //                ["csrf_token"] = client.Token,
            //            }.ToString(Formatting.None)));
            //        if (response.IsSuccessStatusCode)
            //            result = "Updated";
            //    }
            //    catch (Exception e)
            //    {

            //    }

            //    //var result = await base.GetRequestResponse();

            //    if (string.IsNullOrEmpty(result) && !SuppressOfflineSync && Settings.EnableOfflineSync)
            //    {
            //        result = "Updated";
            //        Settings.MangaSyncRequired = true;
            //    }

            //    ResourceLocator.ApplicationDataService[RoamingDataTypes.LastLibraryUpdate] = DateTime.Now.ToBinary();
            //    return result;
            //}
            //catch (Exception)
            //{
            //    return string.Empty;
            //}
        }

        private string ToApiParam(AnimeStatus itemMyStatus)
        {
            return itemMyStatus switch
            {
                AnimeStatus.Watching => "reading",
                AnimeStatus.Completed => "completed",
                AnimeStatus.OnHold => "on_hold",
                AnimeStatus.Dropped => "dropped",
                AnimeStatus.PlanToWatch => "plan_to_read",
                _ => throw new ArgumentOutOfRangeException(nameof(itemMyStatus), itemMyStatus, null)
            };
        }


        public override string SnackbarMessageOnFail => "Your changes will be synced with MAL on next app launch when online.";



        private MangaUpdateQuery(int id, int watchedEps, int myStatus, int myScore, int myVol, string startDate,
            string endDate,string notes,bool rereading)
        {
            UpdatedSomething = true;
            if (startDate != null)
            {
                var splitDate = startDate.Split('-');
                startDate = $"{splitDate[1]}{splitDate[2]}{splitDate[0]}";
            }
            if (endDate != null)
            {
                var splitDate = endDate.Split('-');
                endDate = $"{splitDate[1]}{splitDate[2]}{splitDate[0]}";
            }
            var xml = new StringBuilder();
            xml.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xml.AppendLine("<entry>");
            xml.AppendLine($"<chapter>{watchedEps}</chapter>");
            xml.AppendLine($"<status>{myStatus}</status>");
            xml.AppendLine($"<score>{myScore}</score>");
            xml.AppendLine($"<volume>{myVol}</volume>");
            //xml.AppendLine("<storage_type></storage_type>");
            //xml.AppendLine("<storage_value></storage_value>");
            //xml.AppendLine("<times_rewatched></times_rewatched>");
            //xml.AppendLine("<rewatch_value></rewatch_value>");
            if(startDate != null) xml.AppendLine($"<date_start>{startDate}</date_start>");
            if(endDate != null) xml.AppendLine($"<date_finish>{endDate}</date_finish>");
            //xml.AppendLine("<priority></priority>");
            //xml.AppendLine("<enable_discussion></enable_discussion>");
            xml.AppendLine($"<enable_rereading>{(rereading ? "1" : "0")}</enable_rereading>");
            //xml.AppendLine("<comments></comments>");
            //xml.AppendLine("<fansub_group></fansub_group>");
            xml.AppendLine($"<tags>{notes}</tags>");
            xml.AppendLine("</entry>");


            Request =
                WebRequest.Create(Uri.EscapeUriString($"https://myanimelist.net/api/mangalist/update/{id}.xml?data={xml}"));
            Request.Credentials = Credentials.GetHttpCreditentials();
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }
    }
}