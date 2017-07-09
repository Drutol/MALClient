using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MALClient.Models.Models;

namespace MALClient.XShared.NavArgs
{
    public class ListComparisonPageNavigationArgs
    {
        public MalUser CompareWith { get; set; }

        protected bool Equals(ListComparisonPageNavigationArgs other)
        {
            return Equals(CompareWith, other.CompareWith);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ListComparisonPageNavigationArgs) obj);
        }

        public override int GetHashCode()
        {
            return (CompareWith != null ? CompareWith.GetHashCode() : 0);
        }
    }
}
