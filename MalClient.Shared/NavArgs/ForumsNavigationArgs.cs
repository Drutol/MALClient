using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MalClient.Shared.Utils.Enums;

namespace MalClient.Shared.NavArgs
{
    public class ForumsNavigationArgs
    {
        public ForumsPageIndex Page { get; set; }
    }

    public class ForumsBoardNavigationArgs : ForumsNavigationArgs
    {
        public ForumBoards TargetBoard { get; set; }

        public ForumsBoardNavigationArgs(ForumBoards board)
        {
            TargetBoard = board;
            Page = ForumsPageIndex.PageBoard;
        }
    }

    public class ForumsTopicNavigationArgs : ForumsNavigationArgs
    {
        public string TopicId { get; set; }

        public ForumsTopicNavigationArgs(string topicId)
        {
            TopicId = topicId;
            Page = ForumsPageIndex.PageTopic;
        }
    }
}
