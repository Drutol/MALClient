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
        }

        public override async Task<string> GetRequestResponse()
        {
            try
            {
                var client = await ResourceLocator.MalHttpContextProvider.GetHttpContextAsync();
                var response = await client.PostAsync("https://myanimelist.net/ownlist/anime/add.json",
                    new StringContent(new JObject
                    {
                        ["anime_id"] = int.Parse(_id),
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
    }
}