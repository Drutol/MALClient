using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MALClient.Models.Enums;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MALClient.XShared.Comm.Anime
{
    public class AnimeAddQuery : Query
    {
        private readonly string _id;
        public const string NewAnimeParamChain = "&status=plan-to-watch&rating=0&episodes_watched=0";

        public AnimeAddQuery(string id)
        {
            _id = id;
            AnimeUpdateQuery.UpdatedSomething = true;
            switch (CurrentApiType)
            {
                case ApiType.Mal:
                    AddAnimeMal(id);
                    break;
                case ApiType.Hummingbird:
                    AddAnimeHummingbird(id);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void AddAnimeMal(string id)
        {
            var xml = new StringBuilder();
            xml.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xml.AppendLine("<entry>");
            xml.AppendLine("<episode>0</episode>");
            xml.AppendLine($"<status>{(int)Settings.DefaultStatusAfterAdding}</status>");
            xml.AppendLine("<score>0</score>");
            if (Settings.SetStartDateOnListAdd)
                xml.AppendLine($"<date_start>{DateTimeOffset.Now:MMddyyyy}</date_start>");
            //xml.AppendLine("<download_episodes></download_episodes>");
            //xml.AppendLine("<storage_type></storage_type>");
            //xml.AppendLine("<storage_value></storage_value>");
            //xml.AppendLine("<times_rewatched></times_rewatched>");
            //xml.AppendLine("<rewatch_value></rewatch_value>");
            //xml.AppendLine("<date_finish></date_finish>");
            //xml.AppendLine("<priority></priority>");
            //xml.AppendLine("<enable_discussion></enable_discussion>");
            //xml.AppendLine("<enable_rewatching></enable_rewatching>");
            //xml.AppendLine("<comments></comments>");
            //xml.AppendLine("<fansub_group></fansub_group>");
            //xml.AppendLine("<tags></tags>");
            xml.AppendLine("</entry>");


            Request =
                WebRequest.Create(Uri.EscapeUriString($"https://myanimelist.net/api/animelist/add/{id}.xml?data={xml}"));
            Request.Credentials = Credentials.GetHttpCreditentials();
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }

        public override async Task<string> GetRequestResponse()
        {
            try
            {

                var client = await ResourceLocator.MalHttpContextProvider.GetApiHttpContextAsync();
                var data = new List<KeyValuePair<string, string>>
                {
                    new("status", ToApiParam(AnimeStatus.PlanToWatch)),
                };

                if (Settings.SetStartDateOnListAdd)
                    data.Add(new("start_date", DateTime.Now.ToString("yyyy-MM-dd")));

                using var content = new FormUrlEncodedContent(data);
                var response = await client.SendAsync(new HttpRequestMessage(new HttpMethod("PUT"),
                        $"https://api.myanimelist.net/v2/anime/{_id}/my_list_status")
                    { Content = content });

                if (response.IsSuccessStatusCode)
                    return "Created";
            }
            catch (Exception e)
            {

            }

            return "";
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

        private void AddAnimeHummingbird(string id)
        {
            Request =
                WebRequest.Create(
                    Uri.EscapeUriString(
                        $"http://hummingbird.me/api/v1/libraries/{id}?auth_token={Credentials.HummingbirdToken}{NewAnimeParamChain}"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "POST";
        }
    }
}