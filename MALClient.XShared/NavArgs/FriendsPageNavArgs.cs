using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MALClient.Models.Models;

namespace MALClient.XShared.NavArgs
{
    public class FriendsPageNavArgs
    {
        public MalUser TargetUser { get; set; }

        protected bool Equals(FriendsPageNavArgs other)
        {
            return Equals(TargetUser, other.TargetUser);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FriendsPageNavArgs) obj);
        }

        public override int GetHashCode()
        {
            return (TargetUser != null ? TargetUser.GetHashCode() : 0);
        }
    }
}
