using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Models.Models.ApiResponses
{
    public class MalScrappeNotificationdAnime
    {
        public string title { get; set; }
        public string mediaType { get; set; }
    }

    public class MalScrappedNotificationAnime2
    {
        public string title { get; set; }
        public string url { get; set; }
    }

    public class MalScrappedNotification
    {
        public string id { get; set; }
        public string typeIdentifier { get; set; }
        public string categoryName { get; set; }
        public int createdAt { get; set; }
        public string createdAtForDisplay { get; set; }
        public bool isRead { get; set; }
        public string url { get; set; }
        public bool isDeleted { get; set; }
        public MalScrappeNotificationdAnime anime { get; set; }
        public string senderName { get; set; }
        public string senderProfileUrl { get; set; }
        public string pageUrl { get; set; }
        public string pageTitle { get; set; }
        public string commentUserName { get; set; }
        public string commentUserProfileUrl { get; set; }
        public string commentUserImageUrl { get; set; }
        public string text { get; set; }
        public string date { get; set; }
        public bool isApproved { get; set; }
        public string friendName { get; set; }
        public string friendProfileUrl { get; set; }
        public string friendImageUrl { get; set; }
        public string quoteUserName { get; set; }
        public string quoteUserProfileUrl { get; set; }
        public string topicUrl { get; set; }
        public string topicTitle { get; set; }
        public string message { get; set; }
        public List<MalScrappedNotificationAnime2> animes { get; set; }
    }

    public class MalScrappedRootNotification
    {
        public List<MalScrappedNotification> items { get; set; }
        public List<object> historyItems { get; set; }
    }
}
