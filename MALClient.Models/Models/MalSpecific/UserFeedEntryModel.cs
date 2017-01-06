using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Models.Models.MalSpecific
{
    public class UserFeedEntryModel
    {
        public MalUser User { get; set; }
        public string Title { get; set; }
        public string Header { get; set; }
        public string Link { get; set; }
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
    }
}
