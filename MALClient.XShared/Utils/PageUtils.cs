
using MALClient.XShared.Utils.Enums;

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
                case PageIndex.PageRecomendations:
                case PageIndex.PageMangaSearch:
                case PageIndex.PageHistory:
                    return true;
                default:
                    return false;
            }
        }
    }
}