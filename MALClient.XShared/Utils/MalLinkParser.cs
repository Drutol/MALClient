using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MALClient.Models.Enums;
using MALClient.Models.Models;
using MALClient.Models.Models.MalSpecific;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils.Enums;
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
            else if(Regex.IsMatch(uri, @"https:\/\/myanimelist\.net\/forum\/message\/\d+\?goto=topic"))
            {
                
            }
            else if(Regex.IsMatch(uri, @"https:\/\/myanimelist.net\/forum\/\?topicid=\d+.*"))
            {
                bool lastpost = false;
                if (uri.Contains("&goto=lastpost"))
                {
                    lastpost = true;
                    uri = uri.Replace("&goto=lastpost", "");
                }
                var id = uri.Split('=').Last();

                return new Tuple<PageIndex, object>(PageIndex.PageForumIndex, new ForumsTopicNavigationArgs(id, ForumBoards.Creative) {Lastpost = lastpost});
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
            else if (uri == "https://myanimelist.net/news")
            {
                return new Tuple<PageIndex, object>(PageIndex.PageNews, MalArticlesPageNavigationArgs.News);
            }
            else if (uri == "https://myanimelist.net/featured")
            {
                return new Tuple<PageIndex, object>(PageIndex.PageArticles, MalArticlesPageNavigationArgs.Articles);
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
                    new AnimeDetailsPageNavigationArgs(id, link[3].Replace('_',' ').Trim(), null, null)
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
