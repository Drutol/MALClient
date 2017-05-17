
using MALClient.Models.Enums;

namespace MALClient.XShared.Utils
{
    public static class PageUtils
    {
        public static bool PageRequiresAuth(PageIndex page)
        {
            switch (page)
            {
                case PageIndex.PageAnimeDetails:
                case PageIndex.PageSearch:
                case PageIndex.PageProfile:
#if ANDROID
                case PageIndex.PagePopularVideos: //because
#endif
                case PageIndex.PageRecomendations:
                case PageIndex.PageMangaSearch:
                case PageIndex.PageHistory:
                case PageIndex.PageNotificationHub:
                case PageIndex.PageForumIndex:
                case PageIndex.PageFeeds:
                    return true;
                default:
                    return false;
            }
        }
    }
}