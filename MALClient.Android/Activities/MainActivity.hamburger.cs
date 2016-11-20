using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Mikepenz.Materialdrawer;
using Com.Mikepenz.Materialdrawer.Model;
using Com.Mikepenz.Materialdrawer.Model.Interfaces;
using FFImageLoading;
using MALClient.Android.Resources;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Activities
{
    public partial class MainActivity
    {
        private object GetAppropriateArgsForPage(PageIndex page)
        {
            switch (page)
            {
                case PageIndex.PageSeasonal:
                    return AnimeListPageNavigationArgs.Seasonal;
                case PageIndex.PageMangaList:
                    return AnimeListPageNavigationArgs.Manga;
                case PageIndex.PageMangaSearch:
                    return new SearchPageNavigationArgs { Anime = false };
                case PageIndex.PageSearch:
                    return new SearchPageNavigationArgs();
                case PageIndex.PageTopAnime:
                    return AnimeListPageNavigationArgs.TopAnime(TopAnimeType.General);
                case PageIndex.PageTopManga:
                    return AnimeListPageNavigationArgs.TopManga;
                case PageIndex.PageArticles:
                    return MalArticlesPageNavigationArgs.Articles;
                case PageIndex.PageNews:
                    return MalArticlesPageNavigationArgs.News;
                case PageIndex.PageProfile:
                    return new ProfilePageNavigationArgs { TargetUser = Credentials.UserName };
                case PageIndex.PageWallpapers:
                    return new WallpaperPageNavigationArgs();
                default:
                    return null;
            }
        }

        private PrimaryDrawerItem GetBasePrimaryItem()
        {
            var btn = new PrimaryDrawerItem();
            btn.WithIconTintingEnabled(true);
            btn.WithTextColorRes(Resource.Color.BrushText);
            btn.WithIconColorRes(Resource.Color.BrushNoSearchResults);
            btn.WithSelectedColorRes(Resource.Color.BrushAnimeItemBackground);
            btn.WithSelectedTextColorRes(Resource.Color.AccentColour);
            btn.WithSelectedIconColorRes(Resource.Color.AccentColourDark);
            return btn;
        }

        private SecondaryDrawerItem GetBaseSecondaryItem()
        {
            var btn = new SecondaryDrawerItem();
            btn.WithIconTintingEnabled(true);
            btn.WithTextColorRes(Resource.Color.BrushText);
            btn.WithIconColorRes(Resource.Color.BrushNoSearchResults);
            btn.WithSelectedColorRes(Resource.Color.BrushAnimeItemBackground);
            btn.WithSelectedTextColorRes(Resource.Color.AccentColour);
            btn.WithSelectedIconColorRes(Resource.Color.AccentColourDark);
            return btn;
        }

        private async void BuildDrawer()
        {
            var builder = new DrawerBuilder().WithActivity(this);
            builder.WithSliderBackgroundColorRes(Resource.Color.BrushHamburgerBackground);
            builder.WithStickyFooterShadow(true);


            var animeButton = GetBasePrimaryItem();
            animeButton.WithName("Anime list");
            animeButton.WithIdentifier((int)PageIndex.PageAnimeList);
            animeButton.WithIcon(Resource.Drawable.icon_list);

            var searchButton = GetBasePrimaryItem();
            searchButton.WithName("Anime search");
            searchButton.WithIdentifier((int)PageIndex.PageSearch);
            searchButton.WithIcon(Resource.Drawable.icon_search);

            var seasonalButton = GetBasePrimaryItem();
            seasonalButton.WithName("Seasonal anime");
            seasonalButton.WithIdentifier((int)PageIndex.PageSeasonal);
            seasonalButton.WithIcon(Resource.Drawable.icon_seasonal);

            var recomButton = GetBasePrimaryItem();
            recomButton.WithName("Recommendations");
            recomButton.WithIdentifier((int)PageIndex.PageRecomendations);
            recomButton.WithIcon(Resource.Drawable.icon_recom);

            var topAnimeButton = GetBasePrimaryItem();
            topAnimeButton.WithName("Top anime");
            topAnimeButton.WithIdentifier((int)PageIndex.PageTopAnime);
            topAnimeButton.WithIcon(Resource.Drawable.icon_fav_outline);

            var calendarButton = GetBasePrimaryItem();
            calendarButton.WithName("Calendar");
            calendarButton.WithIdentifier((int)PageIndex.PageCalendar);
            calendarButton.WithIcon(Resource.Drawable.icon_calendar);

            //

            var mangaListButton = GetBaseSecondaryItem();
            mangaListButton.WithName("Manga list");
            mangaListButton.WithIdentifier((int)PageIndex.PageMangaList);
            mangaListButton.WithIcon(Resource.Drawable.icon_books);

            var mangaSearchButton = GetBaseSecondaryItem();
            mangaSearchButton.WithName("Manga search");
            mangaSearchButton.WithIdentifier((int)PageIndex.PageMangaSearch);
            mangaSearchButton.WithIcon(Resource.Drawable.icon_search);

            var topMangaButton = GetBaseSecondaryItem();
            topMangaButton.WithName("Top manga");
            topMangaButton.WithIdentifier((int)PageIndex.PageTopManga);
            topMangaButton.WithIcon(Resource.Drawable.icon_fav_outline);

            //

            IDrawerItem accountButton;
            if (Credentials.Authenticated)
            {
                var btn = new ProfileDrawerItem();
                btn.WithName("Account");
                btn.WithTextColorRes(Resource.Color.BrushText);
                btn.WithSelectedColorRes(Resource.Color.BrushAnimeItemBackground);
                btn.WithSelectedTextColorRes(Resource.Color.AccentColour);
                btn.WithIdentifier((int)PageIndex.PageProfile);
                btn.WithIcon(Resource.Drawable.icon_account);
                accountButton = btn;
            }
            else
            {
                var btn = GetBaseSecondaryItem();
                btn.WithName("Sign in");
                btn.WithIdentifier((int)PageIndex.PageLogIn);
                btn.WithIcon(Resource.Drawable.icon_login);
                accountButton = btn;
            }

            var settingsButton = GetBaseSecondaryItem();
            settingsButton.WithName("Settings & more");
            settingsButton.WithIdentifier((int)PageIndex.PageSettings);
            settingsButton.WithIcon(Resource.Drawable.icon_settings);



            builder.AddStickyDrawerItems(accountButton, settingsButton);


            //

            var mangaSubHeader = new SectionDrawerItem();
            mangaSubHeader.WithName("Manga");
            mangaSubHeader.WithDivider(true);
            mangaSubHeader.WithTextColorRes(Resource.Color.BrushText);

            builder.WithDrawerItems(new List<IDrawerItem>()
            {
                animeButton,
                searchButton,
                seasonalButton,
                recomButton,
                topAnimeButton,
                calendarButton,
                mangaSubHeader,//
                mangaListButton,
                mangaSearchButton,
                topMangaButton,
            });

            _drawer = builder.Build();
            _drawer.StickyFooter.SetBackgroundColor(new Color(ResourceExtension.BrushAnimeItemInnerBackground));

            if (Credentials.Authenticated)
            {
                var bmp = await ImageService.Instance.LoadUrl(
                        $"https://myanimelist.cdn-dena.com/images/userimages/4952914.jpg")
                    .AsBitmapDrawableAsync();
                var btn = accountButton as ProfileDrawerItem;
                btn.WithIcon(bmp);
                _drawer.UpdateStickyFooterItem(btn);
            }
        }
    }
}