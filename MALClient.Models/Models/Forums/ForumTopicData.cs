using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Models.Models.Forums
{
    public class ForumTopicData
    {
        public List<ForumMessageEntry> Messages { get; set; } = new List<ForumMessageEntry>();
        public int AllPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
