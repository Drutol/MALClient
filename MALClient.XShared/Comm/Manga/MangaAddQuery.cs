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

namespace MALClient.XShared.Comm.Manga
{
    public class MangaAddQuery : Query
    {
        private readonly string _id;

        public MangaAddQuery(string id)
        {
            _id = id;
            MangaUpdateQuery.UpdatedSomething = true;

            var xml = new StringBuilder();
            xml.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xml.AppendLine("<entry>");
            xml.AppendLine("<chapter>0</chapter>");
            xml.AppendLine("<status>6</status>");
            xml.AppendLine("<score>0</score>");
            xml.AppendLine("<volume>0</volume>");
            if (Settings.SetStartDateOnListAdd)
                xml.AppendLine($"<date_start>{DateTimeOffset.Now.ToString("MMddyyyy")}</date_start>");
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
                WebRequest.Create(Uri.EscapeUriString($"https://myanimelist.net/api/mangalist/add/{id}.xml?data={xml}"));
            Request.Credentials = Credentials.GetHttpCreditentials();
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }


        public override async Task<string> GetRequestResponse()
        {
            try
            {
                var client = await ResourceLocator.MalHttpContextProvider.GetHttpContextAsync();
                var response = await client.PostAsync("https://myanimelist.net/ownlist/manga/add.json",
                    new StringContent(new JObject
                    {
                        ["manga_id"] = int.Parse(_id),
                        ["status"] = (int)AnimeStatus.PlanToWatch,
                        ["score"] = 0,
                        ["num_read_chapters"] = 0,
                        ["num_read_volumes"] = 0,
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
    }
}