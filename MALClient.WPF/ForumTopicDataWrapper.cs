using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MALClient.Models.Models.Forums;

namespace MALClient.WPF
{
    public class ForumTopicDataWrapper : ISerializable
    {
        public ForumTopicData TopicData { get; set; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(TopicData.Id),TopicData.Id);
            info.AddValue(nameof(TopicData.AllPages),TopicData.AllPages);
            info.AddValue(nameof(TopicData.CurrentPage),TopicData.CurrentPage);
            info.AddValue(nameof(TopicData.Title),TopicData.Title);
            info.AddValue(nameof(TopicData.Id),TopicData.Id);
        }
    }
}
