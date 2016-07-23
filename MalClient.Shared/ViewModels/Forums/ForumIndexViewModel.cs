using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FontAwesome.UWP;
using GalaSoft.MvvmLight;
using MalClient.Shared.Comm.Anime;
using MalClient.Shared.Comm.Forums;
using MalClient.Shared.Utils.Enums;

namespace MalClient.Shared.ViewModels.Forums
{
    public class ForumIndexViewModel : ViewModelBase
    {
        public class ForumBoardEntryGroup
        {
            public string Group { get; set; }
            public List<ForumBoardEntryViewModel> Items { get; set; }
        }

        public List<ForumBoardEntryGroup> Boards { get; } = new List<ForumBoardEntryGroup>
        {         
            new ForumBoardEntryGroup
            {
                Group = "MyAnimeList",
                Items = new List<ForumBoardEntryViewModel>
                {
                    new ForumBoardEntryViewModel("Updates & Announcements", "Updates, changes, and additions to MAL.",FontAwesomeIcon.Bullhorn,ForumBoards.Updates),
                    new ForumBoardEntryViewModel("MAL Guidelines & FAQ",
                        "Site rules, forum rules, database guidelines, review/recommendation guidelines, and other helpful information.",FontAwesomeIcon.Gavel,ForumBoards.Guidelines),
                    new ForumBoardEntryViewModel("Support",
                        "Have a problem using the site or think you found a bug? Post here.",FontAwesomeIcon.Support,ForumBoards.Support),
                    new ForumBoardEntryViewModel("Suggestions",
                        "Have an idea or suggestion for the site? Share it here.",FontAwesomeIcon.LightbulbOutline,ForumBoards.Suggestions),
                    new ForumBoardEntryViewModel("MAL Contests",
                        "Our season-long anime game and other user competitions can be found here.",FontAwesomeIcon.Trophy,ForumBoards.Contests),
                }
            },

            new ForumBoardEntryGroup
            {
                Group = "Anime & Manga",
                Items = new List<ForumBoardEntryViewModel>
                {
                    new ForumBoardEntryViewModel("News Discussion", "Current news in anime and manga.",FontAwesomeIcon.NewspaperOutline,ForumBoards.NewsDisc),
                    new ForumBoardEntryViewModel("Anime & Manga Recommendations",
                        "Ask the community for series recommendations or help other users looking for suggestions.",FontAwesomeIcon.Gift,ForumBoards.Recomms),
                    new ForumBoardEntryViewModel("Anime Series Discussions",
                        "Post in episode discussion threads or talk about specific anime in its series\' boards.",FontAwesomeIcon.FolderOutline,ForumBoards.AnimeSeriesDisc),
                    new ForumBoardEntryViewModel("Manga Series Discussions",
                        "Post in chapter discussion threads or talk about specific manga in its series\' boards.",FontAwesomeIcon.FolderOutline,ForumBoards.MangaSeriesDisc),
                    new ForumBoardEntryViewModel("Anime Discussion",
                        "General anime discussion that is not specific to any particular series.",FontAwesomeIcon.Television,ForumBoards.AnimeDisc),
                    new ForumBoardEntryViewModel("Manga Discussion",
                        "General manga discussion that is not specific to any particular series.",FontAwesomeIcon.Book,ForumBoards.MangaSeriesDisc),
                }
            },
            new ForumBoardEntryGroup
            {
                Group = "General",
                Items = new List<ForumBoardEntryViewModel>
                {
                    new ForumBoardEntryViewModel("Introductions", "New to MyAnimeList? Introduce yourself here.",FontAwesomeIcon.CommentOutline,ForumBoards.Intro),
                    new ForumBoardEntryViewModel("Games, Computers & Tech Support",
                        "Discuss visual novels and other video games, or ask our community a computer related question.",FontAwesomeIcon.Gamepad,ForumBoards.GamesTech),
                    new ForumBoardEntryViewModel("Music & Entertainment",
                        "Asian music and live-action series, Western media and artists, best-selling novels, etc.",FontAwesomeIcon.Music,ForumBoards.Music),
                    new ForumBoardEntryViewModel("Current Events",
                        "World headlines, the latest in science, sports competitions, and other debate topics.",FontAwesomeIcon.Glass,ForumBoards.Events),
                    new ForumBoardEntryViewModel("Casual Discussion",
                        "General interest topics that don't fall into one of the sub-categories above, such as community polls.",FontAwesomeIcon.Coffee,ForumBoards.CasualDisc),
                    new ForumBoardEntryViewModel("Creative Corner",
                        "Show your creations to get help or feedback from our community. Graphics, list designs, stories; anything goes.",FontAwesomeIcon.PictureOutline,ForumBoards.Creative),
                    new ForumBoardEntryViewModel("Forum Games", "Fun forum games are contained here.",FontAwesomeIcon.PuzzlePiece,ForumBoards.ForumsGames),
                }
            },
        };

        public async void Init()
        {
            var peekPosts = await new ForumBoardIndexPeekPostsQuery().GetPeekPosts();
            for(int i = 0;i< 5;i++)
                Boards[0].Items[i].SetPeekPosts(peekPosts[i]);
            for (int i = 0;i< 5;i++)
                Boards[1].Items[i].SetPeekPosts(peekPosts[5+i]);
            Boards[1].Items[5].SetPeekPosts(peekPosts[9]);
            for (int i = 0;i< 7;i++)
                Boards[2].Items[i].SetPeekPosts(peekPosts[10+i]);
        }
    }
}
