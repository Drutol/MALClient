using System;
using System.Net;

namespace MALClient.Comm
{
    internal class AnimeRemoveQuery : Query
    {
        public AnimeRemoveQuery(string id)
        {
            switch (CurrentApiType)
            {
                case ApiType.Mal:
                    Request = WebRequest.Create(Uri.EscapeUriString($"http://myanimelist.net/api/animelist/delete/{id}.xml"));
                    Request.Credentials = Credentials.GetHttpCreditentials();
                    Request.ContentType = "application/x-www-form-urlencoded";
                    Request.Method = "GET";
                    break;
                case ApiType.Hummingbird:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }
    }
}