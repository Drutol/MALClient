using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Models.Models.Forums
{
    public class ForumTopicData
    {
        protected bool Equals(ForumTopicData other)
        {
            return AllPages == other.AllPages && CurrentPage == other.CurrentPage &&
                   string.Equals(Title, other.Title) && IsLocked == other.IsLocked &&
                   (string.Equals(TargetMessageId, other.TargetMessageId) || string.IsNullOrEmpty(TargetMessageId) && string.IsNullOrEmpty(other.TargetMessageId)) && string.Equals(Id, other.Id) &&
                   other.Messages.All(entry => Messages.Any(messageEntry => messageEntry.Id == entry.Id));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ForumTopicData) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = AllPages;
                hashCode = (hashCode * 397) ^ CurrentPage;
                hashCode = (hashCode * 397) ^ (Title != null ? Title.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ IsLocked.GetHashCode();
                hashCode = (hashCode * 397) ^ (TargetMessageId != null ? TargetMessageId.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Id != null ? Id.GetHashCode() : 0);
                return hashCode;
            }
        }

        public List<ForumMessageEntry> Messages { get; set; } = new List<ForumMessageEntry>();
        public int AllPages { get; set; }
        public int CurrentPage { get; set; }
        public string Title { get; set; }
        public List<ForumBreadcrumb> Breadcrumbs { get; set; } = new List<ForumBreadcrumb>();
        public bool IsLocked { get; set; }
        public string TargetMessageId { get; set; }
        public string Id { get; set; }
    }
}
