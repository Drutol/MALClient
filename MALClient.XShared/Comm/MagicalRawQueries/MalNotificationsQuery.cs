using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MALClient.Models.Enums;
using MALClient.Models.Models.ApiResponses;
using MALClient.Models.Models.MalSpecific;
using MALClient.Models.Models.Notifications;
using MALClient.XShared.Utils;
using Newtonsoft.Json;

namespace MALClient.XShared.Comm.MagicalRawQueries
{
    public static class MalNotificationsQuery
    {
        public static async Task<List<MalNotification>> GetNotifications()
        {
            try
            {
                var client = await MalHttpContextProvider.GetHttpContextAsync();

                var response =
                    await client.GetAsync("https://myanimelist.net/notification");

                var doc = new HtmlDocument();
                doc.LoadHtml(await response.Content.ReadAsStringAsync());
                var output = new List<MalNotification>();
                var c = await response.Content.ReadAsStringAsync();
                try
                {
                    //foreach (var notificationRow in doc.FirstOfDescendantsWithClass("ol","notification-item-list").Descendants("li"))
                    //{
                    //    var current = new MalNotification();
                    //    current.Content =
                    //        WebUtility.HtmlDecode(
                    //            notificationRow.FirstOfDescendantsWithClass("div", "notification-item-content")
                    //                .InnerText.Trim());
                    //    current.Date =
                    //        WebUtility.HtmlDecode(
                    //            notificationRow.FirstOfDescendantsWithClass("span", "time").InnerText.Trim());
                    //}
                    HtmlNode notificationsScriptNode = null;
                    foreach (var descendant in doc.DocumentNode.Descendants("script"))
                    {
                        if (descendant.InnerHtml.StartsWith("\nwindow.MAL.headerNotification"))
                        {
                            notificationsScriptNode = descendant;
                            break;
                        }
                    }
                    if(notificationsScriptNode == null)
                        throw new Exception();
                    var startPos = notificationsScriptNode.InnerHtml.IndexOf("=");
                    var endPos = notificationsScriptNode.InnerHtml.IndexOf(";");
                    var json = notificationsScriptNode.InnerHtml.Substring(startPos+1, endPos-startPos-1);
                    var notifications =
                        JsonConvert.DeserializeObject<MalScrappedRootNotification>(json);

                    foreach (var notification in notifications.items)
                    {
                        output.Add(MalNotification.CreateFromRawData(notification));
                    }
                }
                catch (Exception)
                {
                    //hatml
                }

                return output;
            }
            catch (WebException)
            {
                //inner background task exception ¯\_(ツ)_/¯
                return new List<MalNotification>();
            }
            
        }
    }
}
