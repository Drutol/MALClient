using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MalClient.Shared.Models.MalSpecific;
using MalClient.Shared.Utils;

namespace MalClient.Shared.Comm
{
    public class ProfileHistoryQuery : Query
    {
        public ProfileHistoryQuery()
        {
            Request =
                WebRequest.Create(
                    Uri.EscapeUriString($"http://myanimelist.net/history/{Credentials.UserName}"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }

        public async Task<Dictionary<string, List<MalProfileHistoryEntry>>> GetProfileHistory()
        {
            var output = new Dictionary<string, List<MalProfileHistoryEntry>>();

            var raw = await GetRequestResponse();
            if (string.IsNullOrEmpty(raw))
                return null;
            var doc = new HtmlDocument();
            doc.LoadHtml(raw);
            foreach (var historyRow in doc.DocumentNode.Descendants("tr"))
            {
                try
                {
                    //so this is one big table if it contains only on chld it means that it's day/week header so
                    if (historyRow.ChildNodes.Count == 3)
                    {
                        if(historyRow.InnerText.Trim() == "&nbsp;")
                            continue;
                        output.Add(historyRow.InnerText.Trim(), new List<MalProfileHistoryEntry>());
                    }
                    else
                    {
                        var current = new MalProfileHistoryEntry();

                        var link = historyRow.Descendants("a").First();
                        current.Id = int.Parse(link.Attributes["href"].Value.Split('=').Last());
                        current.IsAnime = link.Attributes["href"].Value.Contains("/anime");
                        current.WatchedEpisode = int.Parse(historyRow.Descendants("strong").First().InnerText);
                        current.Date = historyRow.Descendants("td").Last().InnerText; //skip "Edit" button
                        output.Last().Value.Add(current);
                    }
                }
                catch (Exception e)
                {
                    //html
                }

            }

            return output;
        }
    }
}
