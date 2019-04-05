﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MALClient.Models.Models;
using MALClient.Models.Models.MalSpecific;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;

namespace MALClient.XShared.Comm.MagicalRawQueries.Clubs
{
    public static class MalClubDetailsQuery 
    {
        public static async Task<MalClubDetails> GetClubDetails(string clubId,bool justComments = false)
        {
            try
            {
                var output = new MalClubDetails { Id = clubId };
                var client = await ResourceLocator.MalHttpContextProvider.GetHttpContextAsync();


                var response =
                    await client.GetAsync($"/clubs.php?cid={clubId}");

                if (!response.IsSuccessStatusCode)
                    return null;

                var doc = new HtmlDocument();
                doc.LoadHtml(await response.Content.ReadAsStringAsync());

                var mainFrame = doc.DocumentNode.Descendants("table").First();
                var rightBar = mainFrame.ChildNodes[1].ChildNodes[3].ChildNodes[1];
                var rightDivs =
                    rightBar.ChildNodes.Where(node => node.Name == "div" && node.Attributes.Contains("class") &&
                                                      (node.Attributes["class"].Value == "normal_header" ||
                                                       node.Attributes["class"].Value == "spaceit_pad" ||
                                                       node.Attributes["class"].Value == "borderClass"));
                var leftBar = mainFrame.ChildNodes[1].ChildNodes[1].ChildNodes[1];
                var membersTable = leftBar.ChildNodes.First(node => node.Name == "table");

                if (!justComments)
                {


                    output.Name = WebUtility.HtmlDecode(doc.DocumentNode.Descendants("h1").First().InnerText);

                    output.DescriptionHtml = doc.FirstOfDescendantsWithClassContaining("div", "club-information-header")
                        .ParentNode.ChildNodes
                        .First(node => node.Attributes.Contains("class") &&
                                       node.Attributes["class"].Value == "clearfix").InnerHtml;


                    try
                    {
                        output.ImgUrl = rightBar.Descendants("img")
                            .First(node => node.Attributes.Contains("src") &&
                                           node.Attributes["src"].Value.Contains("/clubs/")).Attributes["src"].Value;
                    }
                    catch (Exception)
                    {
                        //image magic tajm
                    }
                    int mode = 0;
                    foreach (var rightDiv in rightDivs)
                    {
                        if (rightDiv.Attributes["class"].Value == "normal_header")
                        {
                            switch (WebUtility.HtmlDecode(rightDiv.InnerText.Trim()))
                            {
                                case "Club Stats":
                                    mode = 0;
                                    break;
                                case "Club Officers":
                                    mode = 1;
                                    break;
                                case "Anime Relations":
                                    mode = 2;
                                    break;
                                case "Manga Relations":
                                    mode = 3;
                                    break;
                                case "Character Relations":
                                    mode = 4;
                                    break;
                                default:
                                    mode = -1;
                                    break;

                            }
                            continue;
                        }

                        string[] innerTextTokens;
                        HtmlNode link = null;
                        switch (mode)
                        {
                            case 0:
                                if (rightDiv.InnerText.Contains("Pictures"))
                                    continue;
                                innerTextTokens = WebUtility.HtmlDecode(rightDiv.InnerText.Trim()).Split(':');
                                output.GeneralInfo.Add((innerTextTokens[0], innerTextTokens[1]));
                                break;
                            case 1:
                                link = rightDiv.Descendants("a").First();
                                var name = (WebUtility.HtmlDecode(link.InnerText.Trim()));
                                string role = null;
                                if (rightDiv.InnerText.Contains("(") && rightDiv.InnerText.Contains(")"))
                                    role = rightDiv.InnerText.Replace(name, "").Replace("(", "").Replace(")", "")
                                        .Trim();

                                output.Officers.Add((name, string.IsNullOrEmpty(role) ? "Officer" : role));
                                break;
                            case 2:
                                link = rightDiv.Descendants("a").First();
                                output.AnimeRelations.Add(
                                    (WebUtility.HtmlDecode(link.InnerText.Trim()), link.Attributes["href"].Value
                                        .Split('=')
                                        .Last().Split('/').Last()));
                                break;
                            case 3:
                                link = rightDiv.Descendants("a").First();
                                output.MangaRelations.Add(
                                    (WebUtility.HtmlDecode(link.InnerText.Trim()), link.Attributes["href"].Value
                                        .Split('=')
                                        .Last().Split('/').Last()));
                                break;
                            case 4:
                                link = rightDiv.Descendants("a").First();
                                output.CharacterRelations.Add(
                                    (WebUtility.HtmlDecode(link.InnerText.Trim()), link.Attributes["href"].Value
                                        .Split('=')
                                        .Last()));
                                break;
                            default:
                                continue;
                        }
                    }

                    output.IsPublic = !rightBar.InnerText.Contains("This is a private club.");
                    output.Joined = rightBar.InnerText.Contains("Leave Club");



                    foreach (var member in membersTable.Descendants("td"))
                    {
                        var innerDivs = member.ChildNodes.Where(node => node.Name == "div").ToList();
                        var current = new MalUser();
                        current.Name = WebUtility.HtmlDecode(innerDivs[0].InnerText.Trim());
                        try
                        {
                            current.ImgUrl = member.Descendants("img").First().Attributes["src"].Value
                                .Replace("/thumbs", "").Replace("_thumb", "");
                        }
                        catch (Exception e)
                        {
                            //yes image again
                        }
                        output.MembersPeek.Add(current);

                    }
                }
                foreach (var htmlNode in leftBar.WhereOfDescendantsWithPartialId("div", "comment"))
                {
                    var current = new MalClubComment();
                    var tds = htmlNode.Descendants("td").Skip(1).First();
                    var firstDiv = WebUtility.HtmlDecode(tds.ChildNodes.FindFirst("div").InnerText.Trim()).Split('|')
                        .Select(s => s.Trim()).ToList();

                    current.User.Name = firstDiv[0];
                    current.Date = firstDiv[1];
                    current.Content = WebUtility.HtmlDecode(tds.InnerText).Trim();
                    current.Content = current.Content.Replace(current.Date, "");
                    current.Content = current.Content.Substring(current.Content.IndexOf('|') + 1).Trim();
                    try
                    {
                        current.User.ImgUrl = htmlNode.FirstOfDescendantsWithClass("div", "picSurround")
                            .Descendants("img").First().Attributes["src"].Value
                            .Replace("/thumbs", "").Replace("_thumb", "");
                    }
                    catch (Exception e)
                    {
                        //yes image again
                    }
                    var delComment = htmlNode.Descendants("small").LastOrDefault();
                    if (delComment != null && !delComment.InnerText.Contains("|"))
                    {
                        var txt = delComment.Descendants("a").First().Attributes.First(attribute => attribute.Value.Contains("delComment")).Value
                            .Replace("delComment", "");
                        var pos = txt.IndexOf(',');
                        current.Id = txt.Substring(0, pos - 1);

                        current.Content = current.Content.Substring(0, current.Content.Length - 6);
                    }
                    try
                    {
                        foreach (var image in htmlNode.WhereOfDescendantsWithClass("img", "userimg"))
                        {
                            current.Images.Add(image.Attributes["src"].Value);
                        }
                    }
                    catch (Exception)
                    {
                        //images strike
                    }
                    output.RecentComments.Add(current);
                }

                return output;
            }
            catch (Exception)
            {
                //here we go again... meh... yes it's html you guessed right
                return null;
            }
        }

        public static async Task<List<MalClubComment>> GetClubComments(string clubId, int page)
        {
            var output = new List<MalClubComment>();
            try
            {
                var client = await ResourceLocator.MalHttpContextProvider.GetHttpContextAsync();


                var response =
                    await client.GetAsync($"/clubs.php?id={clubId}&action=view&t=comments&show={page*20}");

                if (!response.IsSuccessStatusCode)
                    return null;

                var doc = new HtmlDocument();
                doc.LoadHtml(await response.Content.ReadAsStringAsync());

                foreach (var htmlNode in doc.DocumentNode.WhereOfDescendantsWithPartialId("div", "comment"))
                {
                    var current = new MalClubComment();
                    var tds = htmlNode.Descendants("td").Skip(1).First();
                    var firstDiv = WebUtility.HtmlDecode(tds.ChildNodes.FindFirst("div").InnerText.Trim()).Split('|')
                        .Select(s => s.Trim()).ToList();

                    current.User.Name = firstDiv[0];
                    current.Date = firstDiv[1];
                    current.Content = WebUtility.HtmlDecode(tds.InnerText).Trim();
                    current.Content = current.Content.Replace(current.Date, "");
                    current.Content = current.Content.Substring(current.Content.IndexOf('|') + 1).Trim();
                    try
                    {
                        current.User.ImgUrl = htmlNode.FirstOfDescendantsWithClass("div", "picSurround")
                            .Descendants("img").First().Attributes["src"].Value
                            .Replace("/thumbs", "").Replace("_thumb", "");
                    }
                    catch (Exception e)
                    {
                        //yes image again
                    }
                    var delComment = htmlNode.Descendants("small").LastOrDefault();
                    if (delComment != null && !delComment.InnerText.Contains("|")) 
                    {
                        var txt = delComment.Descendants("a").First().Attributes.First(attribute => attribute.Value.Contains("delComment")).Value
                            .Replace("delComment", "");
                        var pos = txt.IndexOf(',');
                        current.Id = txt.Substring(0, pos - 1);
                        current.Content = current.Content.Substring(0, current.Content.Length - 6);
                    }
                    try
                    {
                        foreach (var image in htmlNode.WhereOfDescendantsWithClass("img", "userimg"))
                        {
                            current.Images.Add(image.Attributes["src"].Value);
                        }
                    }
                    catch (Exception)
                    {
                        //images strike
                    }


                    output.Add(current);
                }

                return output;

            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<List<MalUser>> GetMoreUsers(string clubId)
        {
            var output = new List<MalUser>();
            try
            {
                var client = await ResourceLocator.MalHttpContextProvider.GetHttpContextAsync();


                var response =
                    await client.GetAsync($"/clubs.php?id={clubId}&action=view&t=members");

                if (!response.IsSuccessStatusCode)
                    return null;

                var doc = new HtmlDocument();
                doc.LoadHtml(await response.Content.ReadAsStringAsync());

                foreach (var htmlNode in doc.DocumentNode.WhereOfDescendantsWithClass("td", "borderClass"))
                {
                    var current = new MalUser();

                    current.Name = WebUtility.HtmlDecode(htmlNode.InnerText.Trim());

                    try
                    {
                        current.ImgUrl = htmlNode.Descendants("img").First().Attributes["src"].Value
                            .Replace("/thumbs", "").Replace("_thumb", "");
                    }
                    catch (Exception)
                    {
                        //picture
                    }

                    output.Add(current);
                }

                return output;

            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
