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


                var doc = await response.Content.ReadAsStringAsync();
                var output = new List<MalNotification>();
                try
                {
                    var scriptBeginPos = doc.IndexOf("\nwindow.MAL.notification",StringComparison.CurrentCultureIgnoreCase);
                    if (scriptBeginPos == -1)
                        return output;
                    var scriptBegin = doc.Substring(scriptBeginPos);
                    var startPos = scriptBegin.IndexOf('=');
                    var endPos = scriptBegin.IndexOf(';');
                    var json = scriptBegin.Substring(startPos+1, endPos-startPos-1);
                    var notifications =
                        JsonConvert.DeserializeObject<MalScrappedRootNotification>(json);

                    foreach (var notification in notifications.items)
                    {
                        output.Add(new MalNotification(notification));
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
