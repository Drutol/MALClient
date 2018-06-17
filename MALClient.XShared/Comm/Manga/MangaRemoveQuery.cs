using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MALClient.XShared.Comm.Manga
{
    public class MangaRemoveQuery : Query
    {
        private readonly string _id;

        public MangaRemoveQuery(string id)
        {
            _id = id;
            MangaUpdateQuery.UpdatedSomething = true;
            Request = WebRequest.Create(Uri.EscapeUriString($"https://myanimelist.net/api/mangalist/delete/{id}.xml"));
            Request.Credentials = Credentials.GetHttpCreditentials();
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }

        public override async Task<string> GetRequestResponse()
        {
            try
            {
                var client = await ResourceLocator.MalHttpContextProvider.GetHttpContextAsync();
                var response = await client.PostAsync($"https://myanimelist.net/ownlist/manga/{_id}/delete",
                    new StringContent(new JObject
                    {
                        ["csrf_token"] = client.Token,
                    }.ToString(Formatting.None)));
                if (response.IsSuccessStatusCode)
                    return "Updated";
            }
            catch (Exception e)
            {

            }

            return "";
        }
    }
}