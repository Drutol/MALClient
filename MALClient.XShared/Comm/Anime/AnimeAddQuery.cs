using System;
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
                var client = await ResourceLocator.MalHttpContextProvider.GetHttpContextAsync();
                var response = await client.PostAsync("https://myanimelist.net/ownlist/anime/edit.json",
                    new StringContent(new JObject
                    {
                        ["anime_id"] = _id,
                        ["status"] = (int)AnimeStatus.PlanToWatch,
                        ["score"] = 0,
                        ["num_watched_episodes"] = 0,
                        ["csrf_token"] = client.Token,
                    }.ToString(Formatting.None)));
                if (response.IsSuccessStatusCode)
                    return "Created";
            }
            catch (Exception e)
            {

            }

            return "";
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