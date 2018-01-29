using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Models.Models.Notifications
{
    public class AiringShowNotificationEntry
    {
        public string Title { get; set; }
        public string  Id { get; set; }
        public int TriggeredNotifications { get; set; }
        public int EpisodeCount { get; set; }
        public string ImageUrl { get; set; }
    }
}
