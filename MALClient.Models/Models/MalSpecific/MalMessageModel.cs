using System.Collections.Generic;

namespace MALClient.Models.Models.MalSpecific
{
    public class MalMessageModel
    {
        private string _target;
        public string Sender { get; set; }

        public string Target
        {
            get { return IsMine ? _target : Sender; }
            set { _target = value; }
        }

        public string Content { get; set; }
        public string Date { get; set; }
        public string Id { get; set; }
        public string Subject { get; set; }
        public string ThreadId { get; set; }
        public string ReplyId { get; set; }
        public bool IsRead { get; set; }
        public bool IsMine { get; set; }
        public List<string> Images { get; set; } = new List<string>();
    }
}