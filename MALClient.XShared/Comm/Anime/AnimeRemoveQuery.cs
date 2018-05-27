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
            switch (CurrentApiType)
            {
                case ApiType.Mal:
                    Request =
                        WebRequest.Create(Uri.EscapeUriString($"https://myanimelist.net/api/animelist/delete/{id}.xml"));
                    Request.Credentials = Credentials.GetHttpCreditentials();
                    Request.ContentType = "application/x-www-form-urlencoded";
                    Request.Method = "GET";
                    break;
                case ApiType.Hummingbird:
                    Request =
                        WebRequest.Create(
                            Uri.EscapeUriString(
                                $"http://hummingbird.me/api/v1/libraries/{id}/remove?auth_token={Credentials.HummingbirdToken}{AnimeAddQuery.NewAnimeParamChain}"));
                    Request.ContentType = "application/x-www-form-urlencoded";
                    Request.Method = "POST";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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