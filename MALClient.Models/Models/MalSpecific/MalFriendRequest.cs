using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Models.Models.MalSpecific
{
    public class MalFriendRequest
    {
        public MalUser User { get; } = new MalUser();
        public string Message { get; set; }
        public string Id { get; set; }
    }
}
