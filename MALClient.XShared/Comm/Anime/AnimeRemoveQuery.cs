using System;
using System.Net;
using MALClient.XShared.Utils;

namespace MALClient.XShared.Comm.Anime
{
    public class AnimeRemoveQuery : Query
    {
        public AnimeRemoveQuery(string id)
        {
            AnimeUpdateQuery.UpdatedSomething = true;
            switch (CurrentApiType)
            {
                case ApiType.Mal:
                    Request =
                        WebRequest.Create(Uri.EscapeUriString($"http://myanimelist.net/api/animelist/delete/{id}.xml"));
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
    }
}