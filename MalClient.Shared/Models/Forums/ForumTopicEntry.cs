using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MalClient.Shared.Utils.Enums;

namespace MalClient.Shared.Models.Forums
{
    public class ForumBoardContent
    {
        public List<ForumTopicEntry> ForumTopicEntries { get; set; } = new List<ForumTopicEntry>();
        public int Pages { get; set; }
    }

    public class ForumTopicEntry
    {
        public string Title { get; set; }
        public string Type { get; set; } //sticky,poll etc
        public string Op { get; set; }
        public string Replies { get; set; }
        public string LastPostDate { get; set; }
        public string Created { get; set; }
        public string Id { get; set; }
        public string LastPoster { get; set; }
    }

    public class ForumTopicLightEntry
    {
        public string Title { get; set; }
        public string Op { get; set; }
        public string Created { get; set; }
        public string Id { get; set; }
        public bool Lastpost { get; set; }
        public ForumBoards SourceBoard { get; set; }

        public static ForumTopicLightEntry FromTopicEntry(ForumTopicEntry topic)
        {
            return new ForumTopicLightEntry
            {
                Title = topic.Title,
                Op = topic.Op,
                Id = topic.Id,
                Created = topic.Created
            };
        }
    }
}
