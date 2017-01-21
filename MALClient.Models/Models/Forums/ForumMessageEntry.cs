using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Models.Models.Forums
{
    public class ForumMessageEntry
    {
        public string Id { get; set; }
        public string TopicId { get; set; }
        public string MessageNumber { get; set; }
        public string CreateDate { get; set; }
        public string HtmlContent { get; set; }
        public MalForumUser Poster { get; set; }
        public string EditDate { get; set; }
        public bool CanDelete { get; set; }
        public bool CanEdit { get; set; }
    }
}
