using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using MALClient.Models;

namespace MALClient.Comm
{
    public class LibraryListQuery : Query
    {
        public LibraryListQuery(string type = "anime")
        {
            switch (CurrentApiType)
            {
                case ApiType.Mal:
                    Request = WebRequest.Create(Uri.EscapeUriString($"http://myanimelist.net/malappinfo.php?user={Credentials.UserName}?status=all?type={type}"));
                    Request.ContentType = "application/x-www-form-urlencoded";
                    Request.Method = "GET";
                    break;
                case ApiType.Hummingbird:                
                    Request = WebRequest.Create(Uri.EscapeUriString($"https://hummingbird.me/api/v1/users/{Credentials.UserName}/library"));
                    Request.ContentType = "application/x-www-form-urlencoded";
                    Request.Method = "GET";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        public async Task<List<AnimeLibraryItemData>> GetLibrary()
        {
            var output = new List<AnimeLibraryItemData>();


            return output;
        }

        public async Task<XElement> GetProfileStats(bool wantMsg = true)
        {
            var raw = await GetRequestResponse(wantMsg);
            if (string.IsNullOrEmpty(raw))
                return null;
            try
            {
                XElement doc = XElement.Parse(raw);
                return doc.Element("myinfo");
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}