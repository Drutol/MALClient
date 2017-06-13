using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MALClient.Models.Enums;
using MALClient.Models.Models;
using MALClient.Models.Models.MalSpecific;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels.Main;
using Newtonsoft.Json;

namespace MALClient.XShared.Utils
{
    public static class MalLinkParser
    {
        public static Tuple<PageIndex, object> GetNavigationParametersForUrl(string uri)
        {
            uri = uri.Replace("http://", "https://");
            if (Regex.IsMatch(uri, @"https:\/\/myanimelist.net\/forum\/\?subboard=\d+"))
            {
                var id = uri.Split('=').Last();  
                if (id == "1")
                    return new Tuple<PageIndex, object>(PageIndex.PageForumIndex, new ForumsBoardNavigationArgs(ForumBoards.AnimeSeriesDisc));
                else if (id == "4")
                    return new Tuple<PageIndex, object>(PageIndex.PageForumIndex, new ForumsBoardNavigationArgs(ForumBoards.MangaSeriesDisc));
            }
            else if (Regex.IsMatch(uri, @"https:\/\/myanimelist.net\/forum\/\?board=\d+"))
            {
                ForumBoards board;
                if (ForumBoards.TryParse(uri.Split('=').Last(), out board))
                {
                    return new Tuple<PageIndex, object>(PageIndex.PageForumIndex, new ForumsBoardNavigationArgs(board));
                }
            }
            //else if (Regex.IsMatch(uri, @"https:\/\/myanimelist.net\/forum\/\?clubid=\d+"))
            //{
            //    var id = uri.Split('=').Last();

            //    return new Tuple<PageIndex, object>(PageIndex.PageForumIndex, new ForumsBoardNavigationArgs());
            //}
            else if(Regex.IsMatch(uri, @"https:\/\/myanimelist.net\/forum\/\?animeid=\d+"))
            {
                int id;
                if (int.TryParse(uri.Split('=').Last(), out id))
                {
                    return new Tuple<PageIndex, object>(PageIndex.PageForumIndex, new ForumsBoardNavigationArgs(id, "Anime Series Board", true));
                }
            }
            else if(Regex.IsMatch(uri, @"https:\/\/myanimelist.net\/forum\/\?mangaid=\d+"))
            {
                int id;
                if (int.TryParse(uri.Split('=').Last(), out id))
                {
                    return new Tuple<PageIndex, object>(PageIndex.PageForumIndex, new ForumsBoardNavigationArgs(id, "Manga Series Board", false));
                }
            }
            else if(Regex.IsMatch(uri, @"https:\/\/myanimelist\.net\/forum\/message\/\d+\.*"))
            {
                //https://myanimelist.net/forum/message/49205130?goto=topic
                var id = int.Parse(uri.Replace("?goto=topic","").Substring(10).Split('/')[3]);
                return new Tuple<PageIndex, object>(PageIndex.PageForumIndex, new ForumsTopicNavigationArgs(null,id));
            }
            else if(Regex.IsMatch(uri, @"https:\/\/myanimelist.net\/forum\/\?topicid=\d+.*"))
            {
                bool lastpost = false;
                if (uri.Contains("&goto=lastpost"))
                {
                    lastpost = true;
                    uri = uri.Replace("&goto=lastpost", "");
                }
                var pos = uri.IndexOf('&');
                var id = (pos == -1 ? uri : uri.Substring(0,pos)).Split('=').Last();
                long targetMessage = -1;
                if (uri.Contains("#"))
                    long.TryParse(uri.Split('#').Last().Substring(3),out targetMessage);

                return new Tuple<PageIndex, object>(PageIndex.PageForumIndex,
                    new ForumsTopicNavigationArgs(id,
                        targetMessage == 0 ? (lastpost ? (long?) -1 : null) : targetMessage));
            }
            else if (uri == "https://myanimelist.net/forum/")
            {
                return new Tuple<PageIndex, object>(PageIndex.PageForumIndex, new ForumsNavigationArgs());
            }
            else if (Regex.IsMatch(uri,@"https:\/\/myanimelist.net\/comtocom.php\?id1=\d+&id2=\d+\|.*"))
            {
                var split = uri.Split('|');
                uri = split[0];
                var data = string.Join("", split.Skip(1));
                return new Tuple<PageIndex, object>(PageIndex.PageMessageDetails, new MalMessageDetailsNavArgs
                {
                    WorkMode = MessageDetailsWorkMode.ProfileComments,
                    Arg = new MalComment { ComToCom = uri.Split('?').Last() , User = new MalUser { Name = data } }
                });
            }
            else if (Regex.IsMatch(uri, @"https:\/\/myanimelist.net\/mymessages.php\?go=read&id=\d+\|.*"))
            {
                var split = uri.Split('|');
                var data = string.Join("", split.Skip(1));
                var msg = JsonConvert.DeserializeObject<MalMessageModel>(data);
                return new Tuple<PageIndex, object>(PageIndex.PageMessageDetails, new MalMessageDetailsNavArgs
                {
                    WorkMode = MessageDetailsWorkMode.Message,
                    Arg = msg
                });
            }
            else if(Regex.IsMatch(uri , @"https:\/\/myanimelist.net\/people/\d+\/.*"))
            {
                var tokens = uri.Split('/');
                return new Tuple<PageIndex, object>(PageIndex.PageStaffDetails, new StaffDetailsNaviagtionArgs {Id = int.Parse(tokens[tokens.Length - 2]), ResetNav = true});
            }
            else if (Regex.IsMatch(uri, @"https:\/\/myanimelist.net\/character/\d+\/.*"))
            {
                var tokens = uri.Split('/');
                return new Tuple<PageIndex, object>(PageIndex.PageCharacterDetails,
                    new CharacterDetailsNavigationArgs {Id = int.Parse(tokens[tokens.Length - 2]),ResetNav = true});
            }
            else if (uri == "https://myanimelist.net/news")
            {
                return new Tuple<PageIndex, object>(PageIndex.PageNews, MalArticlesPageNavigationArgs.News);
            }
            else if (uri == "https://myanimelist.net/featured")
            {
                return new Tuple<PageIndex, object>(PageIndex.PageArticles, MalArticlesPageNavigationArgs.Articles);
            }
            else if (uri == "https://myanimelist.net/mymessages.php")
            {
                return new Tuple<PageIndex, object>(PageIndex.PageMessanging, null);
            }
            else if (uri == "https://myanimelist.net/forum")
            {
                return new Tuple<PageIndex, object>(PageIndex.PageForumIndex, null);
            }
            else if (uri == "https://myanimelist.net/anime.php")
            {
                return new Tuple<PageIndex, object>(PageIndex.PageSearch, new SearchPageNavigationArgs());
            }


            /////////////////////////////////////////////////////Removed ? from the end

            var paramIndex = uri.IndexOf('?');
            if (paramIndex != -1)
                uri = uri.Substring(0, paramIndex);
            if (Regex.IsMatch(uri, "anime\\/\\d") || Regex.IsMatch(uri, "manga\\/\\d"))
            {
                var link = uri.Substring(8).Split('/');
                if (link.Length < 3) //we probably don't have name 
                    return null;
                var id = int.Parse(link[2]);
                //if (Settings.SelectedApiType == ApiType.Hummingbird) //id switch            
                //    id = await new AnimeDetailsHummingbirdQuery(id).GetHummingbirdId();
                return new Tuple<PageIndex, object>(PageIndex.PageAnimeDetails,
                    new AnimeDetailsPageNavigationArgs(id, link.Length == 4 ? link[3].Replace('_',' ').Trim() : null, null, null)
                    {
                        AnimeMode = link[1] == "anime"
                    });
            }
            if (uri.Contains("/profile/"))
            {
                return new Tuple<PageIndex, object>(PageIndex.PageProfile, new ProfilePageNavigationArgs { TargetUser = uri.Split('/').Last() });
            }


            return null;
        }
    }
}
