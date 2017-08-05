using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Models.Models.MalSpecific
{


    public class MalClubEntry
    {
        public enum JoinAction
        {
            Join,
            Request,
            AcceptDeny,
            None
        }

        public string Name { get; set; }
        public string Id { get; set; }
        public string ImgUrl { get; set; }
        public string Members { get; set; }
        public string LastCommentDate { get; set; }
        public string LastCommentAuthor { get; set; }
        public string LastPost { get; set; }
        public string Description { get; set; }
        public JoinAction JoinType { get; set; }

        public string JoinData { get; set; }

        public string JoinButtonText
        {
            get
            {
                switch (JoinType)
                {
                    case JoinAction.Join:
                        return "Join";
                    case JoinAction.Request:
                        return "Request";
                    case JoinAction.AcceptDeny:
                        return "Accept";
                    case JoinAction.None:
                        return "";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
