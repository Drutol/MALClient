using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Models.Models.Forums
{
    public class MalForumUser
    {
        public MalUser MalUser { get; set; } = new MalUser();
        public string Title { get; set; }
        public string Status { get; set; }
        public string Joined { get; set; }
        public string Posts { get; set; }
        public string SignatureHtml { get; set; }

        protected bool Equals(MalForumUser other)
        {
            return Equals(MalUser, other.MalUser) && string.Equals(Status, other.Status) && string.Equals(Joined, other.Joined) && string.Equals(SignatureHtml, other.SignatureHtml);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MalForumUser) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (MalUser != null ? MalUser.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Status != null ? Status.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Joined != null ? Joined.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (SignatureHtml != null ? SignatureHtml.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
