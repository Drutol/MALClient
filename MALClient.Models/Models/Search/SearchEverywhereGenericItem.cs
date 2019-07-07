using System;
using System.Collections.Generic;
using System.Text;
using MALClient.Models.Models.ApiResponses;

namespace MALClient.Models.Models.Search
{
    public abstract class SearchEverywhereGenericItem : ISearchEverywhereItem
    {
        public Item Item { get; set; }

        public SearchEverywhereGenericItem(Item item)
        {
            Item = item;
        }
    }

    public class SearchEverywhereAnimeItem : SearchEverywhereGenericItem
    {
        public SearchEverywhereAnimeItem(Item item) : base(item)
        {
        }
    }

    public class SearchEverywhereMangaItem : SearchEverywhereGenericItem
    {
        public SearchEverywhereMangaItem(Item item) : base(item)
        {
        }
    }

    public class SearchEverywhereCharacterItem : SearchEverywhereGenericItem
    {
        public SearchEverywhereCharacterItem(Item item) : base(item)
        {
        }
    }

    public class SearchEverywherePersonItem : SearchEverywhereGenericItem
    {
        public SearchEverywherePersonItem(Item item) : base(item)
        {
        }
    }

    public class SearchEverywhereUserItem : SearchEverywhereGenericItem
    {
        public SearchEverywhereUserItem(Item item) : base(item)
        {
        }
    }
}
