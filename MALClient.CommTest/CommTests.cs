using System;
using MalClient.Shared.Comm.Anime;
using MalClient.Shared.Comm.Articles;
using MalClient.Shared.Comm.CommUtils;
using MalClient.Shared.Models.Anime;
using MalClient.Shared.Models.MalSpecific;
using MalClient.Shared.Utils.Enums;
using Xunit;

namespace MalClientCommTest
{
    public abstract class TestResources
    {
        protected const int AnimeTestId = 9253;
        protected const int MangaTestId = 40171;

        protected TestResources()
        {
            HtmlClassMgr.Init();
        }
    }

    public class CommTests : TestResources
    {
        [Fact]
        public async void TestRecommendationsQuery()
        {
            var recoms = await new AnimeRecomendationsQuery().GetRecomendationsData();
            Assert.False(recoms.Count == 0);
        }
    }

    public class ArticleTests
    {
        [Theory]
        [InlineData(ArticlePageWorkMode.Articles)]
        [InlineData(ArticlePageWorkMode.News)]
        public async void TestArticlesIndexQuery(ArticlePageWorkMode mode)
        {
            var articles = await new MalArticlesIndexQuery(mode).GetArticlesIndex(true);
            Assert.False(articles.Count == 0);
        }

        [Theory]
        [InlineData(MalNewsType.Article, "http://myanimelist.net/featured/685")]
        [InlineData(MalNewsType.News, "http://myanimelist.net/news/46528454")]
        public async void TestArticlesIndexQuery(MalNewsType type,string url)
        {
            var articles = await new MalArticleQuery(url,"",type).GetArticleHtml();
            Assert.False(string.IsNullOrEmpty(articles));
        }


    }

    public class AnimeDetailsTests : TestResources
    {
        [Theory]
        [InlineData(AnimeTestId, true)]
        [InlineData(MangaTestId, false)]
        public async void TestAnimeRelatedQuery(int id, bool anime)
        {
            var related = await new AnimeRelatedQuery(id, anime).GetRelatedAnime(true);
            Assert.False(related.Count == 0);
        }

        [Theory]
        [InlineData(AnimeTestId, true)]
        [InlineData(MangaTestId, false)]
        public async void TestAnimeReviewsQuery(int id, bool anime)
        {
            var reviews = await new AnimeReviewsQuery(id, anime).GetAnimeReviews(true);
            Assert.False(reviews.Count == 0);
        }

        [Theory]
        [InlineData(AnimeTestId, true)]
        [InlineData(MangaTestId, false)]
        public async void TestDirectRecommendationsQuery(int id, bool anime)
        {
            var recoms = await new AnimeDirectRecommendationsQuery(id, anime).GetDirectRecommendations(true);
            Assert.False(recoms.Count == 0);
        }
    }

    public class AnimeTopTests
    {
        [Theory]
        [InlineData(TopAnimeType.Manga)]
        [InlineData(TopAnimeType.Airing)]
        [InlineData(TopAnimeType.Favourited)]
        [InlineData(TopAnimeType.General)]
        [InlineData(TopAnimeType.Movies)]
        [InlineData(TopAnimeType.Ovas)]
        [InlineData(TopAnimeType.Popular)]
        [InlineData(TopAnimeType.Tv)]
        [InlineData(TopAnimeType.Upcoming)]
        public async void TestTopAnime(TopAnimeType type)
        {
            var top = await new AnimeTopQuery(type).GetTopAnimeData(true);
            Assert.False(top.Count == 0);
        }
    }

    public class AnimeSeasonalTest
    {
        public AnimeSeasonalTest()
        {
            HtmlClassMgr.Init();
        }

        [Theory]
        [InlineData("http://myanimelist.net/anime/season/2016/spring")]
        [InlineData("http://myanimelist.net/anime/season/2016/fall")]
        public async void TestSeasonalAnime(string season)
        {
            var seasonal = await new AnimeSeasonalQuery(new AnimeSeason {Url = season}).GetSeasonalAnime(true);
            Assert.False(seasonal.Count == 0);
        }
    }
}
