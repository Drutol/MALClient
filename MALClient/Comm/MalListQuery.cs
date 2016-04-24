using System;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using MALClient.Models;

namespace MALClient.Comm
{
    public class MalListQuery : Query
    {
        public MalListQuery(MalListParameters args)
        {
            Request = WebRequest.Create(Uri.EscapeUriString(UriBuilder.GetUri(UriType.MalListQuery, args)));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
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