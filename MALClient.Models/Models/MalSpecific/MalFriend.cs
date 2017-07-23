using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Models.Models.MalSpecific
{
    public class MalFriend
    {
        public MalUser User { get; set; } = new MalUser();
        public string LastOnline { get; set; }
        public string FriendsSince { get; set; }
    }
}
