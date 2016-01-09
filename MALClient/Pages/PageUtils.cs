using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                default:
                    return false;
            }
        }
    }
}
