namespace MALClient.Pages
{
    public static class PageUtils
    {
        public static bool PageRequiresAuth(PageIndex page)
        {
            switch (page)
            {
                case PageIndex.PageAnimeDetails:
                    return true;
                case PageIndex.PageSearch:
                    return true;
                case PageIndex.PageProfile:
                    return true;
                case PageIndex.PageRecomendations:
                    return true;
                default:
                    return false;
            }
        }
    }
}