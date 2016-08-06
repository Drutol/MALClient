using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FontAwesome.UWP;
using MalClient.Shared.Utils.Enums;

namespace MalClient.Shared.NavArgs
{
    public class ForumsNavigationArgs
    {
        public ForumsPageIndex Page { get; set; }
    }

    public class ForumsBoardNavigationArgs : ForumsNavigationArgs
    {
        public ForumBoards TargetBoard { get; }
        public int PageNumber { get; }
        public int AnimeId { get; }
        public string AnimeTitle { get; set; }
        public bool? IsAnimeBoard { get; }


        public ForumsBoardNavigationArgs(ForumBoards board,int page = 0)
        {
            TargetBoard = board;
            Page = ForumsPageIndex.PageBoard;
            PageNumber = page;
        }

        public ForumsBoardNavigationArgs(int animeId,string title,bool anime,int page = 0)
        {
            AnimeId = animeId;
            IsAnimeBoard = anime;
            AnimeTitle = title;
            PageNumber = page;
            Page = ForumsPageIndex.PageBoard;
        }
    }

    public class ForumsTopicNavigationArgs : ForumsNavigationArgs
    {
        public string TopicId { get; set; }
        public ForumBoards SourceBoard { get; set; }
        public bool Lastpost { get; set; }

        public ForumsTopicNavigationArgs(string topicId, ForumBoards sourceBoard,bool lastpost = false)
        {
            TopicId = topicId;
            Page = ForumsPageIndex.PageTopic;
            SourceBoard = sourceBoard;
            Lastpost = lastpost;
        }
    }
}
