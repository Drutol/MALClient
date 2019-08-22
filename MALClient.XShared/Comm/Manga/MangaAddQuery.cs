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