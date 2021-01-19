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
                var client = await ResourceLocator.MalHttpContextProvider.GetApiHttpContextAsync();

                var response =
                    await client.DeleteAsync($"https://api.myanimelist.net/v2/manga/{_id}/my_list_status");

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