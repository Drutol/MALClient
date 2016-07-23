using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MalClient.Shared.Models.Forums
{
    public class ForumTopicEntry
    {
        public string Title { get; set; }
        public string Type { get; set; } //sticky,poll etc
        public string Op { get; set; }
        public string Replies { get; set; }
        public string LastPostDate { get; set; }
        public string Created { get; set; }
        public string Url { get; set; }
        public string Id { get; set; }
        public string LastPoster { get; set; }
    }
}
