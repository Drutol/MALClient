using System;
using MALClient.Models.Enums;
using MALClient.Models.Models.Forums;

namespace MALClient.XShared.NavArgs
{
    public enum ForumBoardPageWorkModes
    {
        Standard,
        AnimeBoard,
        MangaBoard,
        Search,
        UserSearch,
        WatchedTopics,
        Club
    }

    public class ForumsNavigationArgs
    {
        public ForumsPageIndex Page { get; set; }

        public bool IgnoreBackNavHandling { get; set; }
    }

    public class ForumsBoardNavigationArgs : ForumsNavigationArgs
    {
        public ForumBoards TargetBoard { get; }
        public int PageNumber { get; }
        public int AnimeId { get; }
        public string AnimeTitle { get; set; }
        public bool IsAnimeBoard { get; }
        public ForumBoardPageWorkModes WorkMode { get; set; }
        public string Query { get; set; }
        public ForumBoards? Scope { get; set; }
        public string ClubId { get; set; }
        public string ClubName { get; set; }


        public ForumsBoardNavigationArgs(ForumBoards board,int page = 0)
        {
            TargetBoard = board;
            Page = ForumsPageIndex.PageBoard;
            WorkMode = ForumBoardPageWorkModes.Standard;
            PageNumber = page;
        }

        public ForumsBoardNavigationArgs(int animeId,string title,bool anime,int page = 0)
        {
            AnimeId = animeId;
            IsAnimeBoard = anime;
            AnimeTitle = title;
            PageNumber = page;
            WorkMode = anime ? ForumBoardPageWorkModes.AnimeBoard : ForumBoardPageWorkModes.MangaBoard;
            Page = ForumsPageIndex.PageBoard;
        }

        public ForumsBoardNavigationArgs(string query,ForumBoards? scope)
        {
            Query = query;
            Scope = scope;
            WorkMode = ForumBoardPageWorkModes.Search;
            Page = ForumsPageIndex.PageBoard;
        }

        public ForumsBoardNavigationArgs(ForumBoardPageWorkModes workMode)
        {
            WorkMode = workMode;
            Page = ForumsPageIndex.PageBoard;
        }

        public ForumsBoardNavigationArgs(string userName)
        {
            Page = ForumsPageIndex.PageBoard;
            WorkMode = ForumBoardPageWorkModes.UserSearch;
            Query = userName;
        }

        public ForumsBoardNavigationArgs(string id,string clubName)
        {
            Page = ForumsPageIndex.PageBoard;
            WorkMode = ForumBoardPageWorkModes.Club;
            ClubId = id;
            ClubName = clubName;
        }

        public override bool Equals(object obj)
        {
            var arg = obj as ForumsBoardNavigationArgs;

            if (arg?.WorkMode == WorkMode)
            {
                switch (WorkMode)
                {
                    case ForumBoardPageWorkModes.Standard:
                        return TargetBoard == arg.TargetBoard;
                    case ForumBoardPageWorkModes.AnimeBoard:
                    case ForumBoardPageWorkModes.MangaBoard:
                        return AnimeId == arg.AnimeId;
                    case ForumBoardPageWorkModes.Search:
                        return Query == arg.Query && Scope == arg.Scope;
                    case ForumBoardPageWorkModes.UserSearch:
                        return arg.Query == Query;
                    case ForumBoardPageWorkModes.WatchedTopics:
                        return WorkMode == arg.WorkMode;
                    case ForumBoardPageWorkModes.Club:
                        return ClubId == arg.ClubId;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return false;
        }
    }

    public class ForumsTopicNavigationArgs : ForumsNavigationArgs
    {
        public ForumTopicEntry Entry { get; }
        public TopicType TopicType { get; }
        public string TopicId { get; set; }
        public long? MessageId { get; set; }
        public int TopicPage { get; set; }
        public bool LastPost => MessageId == -1;

        public int? FirstVisibleItemIndex { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="topicType"></param>
        /// <param name="id"></param>
        /// <param name="mediaTitle">Anime or manga title</param>
        /// <param name="messageId">Null for start, -1 for last post, else scroll to message</param>
        /// <param name="page"></param>
        public ForumsTopicNavigationArgs(TopicType topicType,string id,int? messageId,int page)
        {
            Page = ForumsPageIndex.PageTopic;
            TopicType = topicType;
            TopicId = id;
            MessageId = messageId;
            TopicPage = page;
        }

        public ForumsTopicNavigationArgs(string id, long? messageId,int page = 1)
        {
            Page = ForumsPageIndex.PageTopic;
            TopicId = id;
            MessageId = messageId;
            TopicPage = page;
        }

        private ForumsTopicNavigationArgs()
        {
        }

        
    }

    public class ForumsNewTopicNavigationArgs : ForumsNavigationArgs
    {

        public ForumType ForumType { get; }
        public TopicType? TopicType { get; }
        public ForumBoards? BoardType { get; }
        public string Title { get; }
        public int Id { get; }
        public string ClubName { get; set; }


        public ForumsNewTopicNavigationArgs(TopicType topicType,string title,int mediaId)
        {
            Page = ForumsPageIndex.PageNewTopic;
            ForumType = ForumType.Normal;
            TopicType = topicType;
            Title = title;
            Id = mediaId;
        }

        public ForumsNewTopicNavigationArgs(ForumBoards boardType)
        {
            Page = ForumsPageIndex.PageNewTopic;
            ForumType = ForumType.Normal;
            BoardType = boardType;
            Id = (int) boardType;
        }

        public ForumsNewTopicNavigationArgs(int id,string clubName)
        {
            Page = ForumsPageIndex.PageNewTopic;
            ForumType = ForumType.Clubs;
            Id = id;
            ClubName = clubName;
        }
    }

    public class ForumStarredMessagesNavigationArgs : ForumsNavigationArgs
    {
        public string MessageId { get; }

        public ForumStarredMessagesNavigationArgs()
        {
            Page = ForumsPageIndex.PageStarred;
        }

        public ForumStarredMessagesNavigationArgs(string messageId) : this()
        {
            MessageId = messageId;
        }
    }
}
