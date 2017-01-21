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
    }
}
