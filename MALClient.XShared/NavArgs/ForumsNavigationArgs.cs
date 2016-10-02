using System;
using MALClient.XShared.Utils.Enums;

namespace MALClient.XShared.NavArgs
{
    public enum ForumBoardPageWorkModes
    {
        Standard,
        AnimeBoard,
        MangaBoard,
        Search,
        UserSearch,
        WatchedTopics
    }

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
        public bool IsAnimeBoard { get; }
        public ForumBoardPageWorkModes WorkMode { get; set; }
        public string Query { get; set; }
        public ForumBoards? Scope { get; set; }


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
                        return WorkMode == arg.WorkMode;
                    case ForumBoardPageWorkModes.WatchedTopics:
                        return WorkMode == arg.WorkMode;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return false;
        }
    }

    public class ForumsTopicNavigationArgs : ForumsNavigationArgs
    {
        public bool CreateNewTopic { get; set; }
        public string TopicId { get; set; }
        public ForumBoards? SourceBoard { get; set; }
        public bool Lastpost { get; set; }

        public ForumsTopicNavigationArgs(string topicId, ForumBoards sourceBoard, bool lastpost = false)
        {
            TopicId = topicId;
            Page = ForumsPageIndex.PageTopic;
            SourceBoard = sourceBoard;
            Lastpost = lastpost;
        }

        public ForumsTopicNavigationArgs(string topicId, bool lastpost)
        {
            TopicId = topicId;
            Page = ForumsPageIndex.PageTopic;
            Lastpost = lastpost;
        }

        private ForumsTopicNavigationArgs()
        {
        }

        public static ForumsTopicNavigationArgs NewTopic => new ForumsTopicNavigationArgs {CreateNewTopic = true, Page = ForumsPageIndex.PageTopic};
    }
}
