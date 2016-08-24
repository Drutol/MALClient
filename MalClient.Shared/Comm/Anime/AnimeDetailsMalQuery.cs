using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MalClient.Shared.Models.AnimeScrapped;
using MalClient.Shared.Models.ScrappedDetails;
using MalClient.Shared.Utils;

namespace MalClient.Shared.Comm.Anime
{
    public class AnimeDetailsMalQuery : Query
    {
        private readonly int _id;
        private readonly bool _anime;

        public AnimeDetailsMalQuery(int id,bool anime)
        {
            _id = id;
            _anime = anime;
            Request =
                WebRequest.Create(
                    Uri.EscapeUriString($"https://myanimelist.net/{(anime ? "anime" : "manga")}/{id}"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }

        public async Task<AnimeScrappedDetails> GetDetails(bool force)
        {
            var possibleData = force ? null :
                await DataCache.RetrieveData<AnimeScrappedDetails>(_id.ToString(), "anime_details_scrapped", 14);
            if (possibleData != null)
                return possibleData;

            var output = new AnimeScrappedDetails();
            var raw = await GetRequestResponse();
            if (string.IsNullOrEmpty(raw))
                return null;
            var doc = new HtmlDocument();
            doc.LoadHtml(raw);

            output.Id = _id;

            bool recording = false;
            string currentString = "";
            int currentStage = 0;
            try
            {
                foreach (var child in doc.FirstOfDescendantsWithClass("div", "js-scrollfix-bottom").ChildNodes)
                {

                    if (!recording)
                    {
                        if (child.Attributes.Contains("class") &&
                            child.Attributes["class"].Value.Contains("js-sns-icon-container"))
                            recording = true;
                        continue;
                    }
                    if (child.Name == "div" && child.Attributes.Contains("class") && child.Attributes["class"].Value.Contains("mauto"))
                        break;

                    if (child.Name == "h2")
                    {
                        currentStage++;
                        continue;
                    }
                    if (child.Name == "div")
                    {
                        currentString = Regex.Replace(WebUtility.HtmlDecode(child.InnerText.Replace('\n', ' ').Trim()), @"[ ]{2,}", " ");
                        switch (currentStage)
                        {
                            case 1:
                                output.AlternativeTitles.Add(currentString);
                                break;
                            case 2:
                                output.Information.Add(currentString);
                                break;
                            case 3:
                                output.Statistics.Add(currentString);
                                break;
                        }
                    }


                }

                if (_anime)
                {
                    foreach (var row in doc.FirstOfDescendantsWithClass("div", "theme-songs js-theme-songs ending").Descendants("span"))
                    {
                        output.Endings.Add(WebUtility.HtmlDecode(row.InnerText));
                    }
                    foreach (var row in doc.FirstOfDescendantsWithClass("div", "theme-songs js-theme-songs opnening").Descendants("span"))
                    {
                        output.Openings.Add(WebUtility.HtmlDecode(row.InnerText));
                    }
                }
            }
            catch (Exception)
            {
                //hatemeł
            }
            
            DataCache.SaveData(output,_id.ToString(), "anime_details_scrapped");
            return output;
        }
    }
}
