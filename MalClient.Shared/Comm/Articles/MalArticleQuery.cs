using System;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MalClient.Shared.Models.MalSpecific;
using MalClient.Shared.Utils;

namespace MalClient.Shared.Comm.Articles
{
    public class MalArticleQuery : Query
    {
        private readonly string _title;
        private readonly MalNewsType _type;
        public MalArticleQuery(string url,string title,MalNewsType type)
        {
            _type = type;
            _title = title;
            Request =
                WebRequest.Create(Uri.EscapeUriString(url));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }

        public async Task<string> GetArticleHtml()
        {
            var possibleData = await DataCache.RetrieveArticleContentData(_title,_type);
            if (possibleData != null)
                return possibleData;
            var raw = await GetRequestResponse();
            if (string.IsNullOrEmpty(raw))
                return null;
            var doc = new HtmlDocument();
            doc.LoadHtml(raw);
            var htmlData = doc.FirstOfDescendantsWithClass("div", "news-container").OuterHtml;
            DataCache.SaveArticleContentData(_title,htmlData,_type);
            return htmlData;
        }
    }
}
