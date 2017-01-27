using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Models.Models.Forums
{
    public class StarredForumMessage
    {
        private sealed class TopicIdEqualityComparer : IEqualityComparer<StarredForumMessage>
        {
            public bool Equals(StarredForumMessage x, StarredForumMessage y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return string.Equals(x.TopicId, y.TopicId);
            }

            public int GetHashCode(StarredForumMessage obj)
            {
                return (obj.TopicId != null ? obj.TopicId.GetHashCode() : 0);
            }
        }

        public static IEqualityComparer<StarredForumMessage> TopicIdComparer { get; } = new TopicIdEqualityComparer();

        public string MessageId { get; set; }
        public string TopicId { get; set; }
        public string TopicTitle { get; set; }
        public MalUser Poster { get; set; }
    }
}
