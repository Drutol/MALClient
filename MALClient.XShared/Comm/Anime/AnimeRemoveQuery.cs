using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MALClient.Models.Enums;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MALClient.XShared.Comm.Anime
{
    public class AnimeRemoveQuery : Query
    {
        private readonly string _id;

        public AnimeRemoveQuery(string id)
        {
            _id = id;
            AnimeUpdateQuery.UpdatedSomething = true;
        }

        public override async Task<string> GetRequestResponse()
        {         
            try
            {
                var client = await ResourceLocator.MalHttpContextProvider.GetHttpContextAsync();
                var response = await client.PostAsync($"https://myanimelist.net/ownlist/anime/{_id}/delete",
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