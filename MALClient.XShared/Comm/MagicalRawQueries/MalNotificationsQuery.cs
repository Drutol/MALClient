using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MALClient.Models.Enums;
using MALClient.Models.Models.ApiResponses;
using MALClient.Models.Models.MalSpecific;
using MALClient.Models.Models.Notifications;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using Newtonsoft.Json;

namespace MALClient.XShared.Comm.MagicalRawQueries
{
    public static class MalNotificationsQuery
    {
        public static async Task<List<MalNotification>> GetNotifications()
        {
            try
            {
                var client = await ResourceLocator.MalHttpContextProvider.GetHttpContextAsync();

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

        public static Task<bool> MarkNotifiactionsAsRead(MalNotification notification)
        {
            return MarkNotifiactionsAsRead(new[] {notification});
        }

        public static async Task<bool> MarkNotifiactionsAsRead(IEnumerable<MalNotification> notification)
        {
            try
            {
                var client = await ResourceLocator.MalHttpContextProvider.GetHttpContextAsync();

                var dto = new ReadNotificationDTO(notification,client.Token);
                var content = new StringContent(JsonConvert.SerializeObject(dto));
                var response = await client.PostAsync("/notification/api/check-items-as-read.json", content);

                return (await response.Content.ReadAsStringAsync()).Contains("true");

            }
            catch (Exception e)
            {
                ResourceLocator.MalHttpContextProvider.ErrorMessage("Notifications");
                return false;
            }
        }

        class ReadNotificationDTO
        {
            public ReadNotificationDTO(IEnumerable<MalNotification> notifications,string token)
            {
                csrf_token = token;
                notification_ids = notifications.Select(notification => notification.Id).ToList();
            }

            public string csrf_token { get; private set; }
            public List<string> notification_ids { get; private set; }
        }
    }
}
