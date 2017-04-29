using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Com.Mikepenz.Materialdrawer;
using Com.Mikepenz.Materialdrawer.Model;
using Com.Mikepenz.Materialdrawer.Model.Interfaces;
using FFImageLoading;
using GalaSoft.MvvmLight.Command;
using MALClient.Android.Listeners;
using MALClient.Android.Resources;
using MALClient.Models.Enums;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.Interfaces;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;
using static MALClient.Android.HamburgerUtilities;

namespace MALClient.Android.Activities
{
    public partial class MainActivity : IHamburgerViewModel
    {
        private bool _allowHamburgerNavigation;

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



        private void BuildDrawer()
        {
            var builder = new DrawerBuilder().WithActivity(this);
            builder.WithSliderBackgroundColorRes(ResourceExtension.BrushHamburgerBackgroundRes);
            builder.WithStickyFooterShadow(true);
            builder.WithDisplayBelowStatusBar(true);


            var animeButton = GetBasePrimaryItem();
            animeButton.WithName("Anime list");
            animeButton.WithIdentifier((int)PageIndex.PageAnimeList);
            animeButton.WithIcon(Resource.Drawable.icon_list);

            var searchButton = GetBasePrimaryItem();
            searchButton.WithName("Search");
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
            mangaListButton.WithIcon(Resource.Drawable.icon_list);

            var topMangaButton = GetBaseSecondaryItem();
            topMangaButton.WithName("Top manga");
            topMangaButton.WithIdentifier((int)PageIndex.PageTopManga);
            topMangaButton.WithIcon(Resource.Drawable.icon_fav_outline);

            var articlesButton = GetBaseSecondaryItem();
            articlesButton.WithName("Articles & News");
            articlesButton.WithIdentifier((int) PageIndex.PageArticles);
            articlesButton.WithIcon(Resource.Drawable.icon_newspaper);

            var videoButton = GetBaseSecondaryItem();
            videoButton.WithName("Promotional Videos");
            videoButton.WithIdentifier((int) PageIndex.PagePopularVideos);
            videoButton.WithIcon(Resource.Drawable.icon_video);

            var feedsButton = GetBaseSecondaryItem();
            feedsButton.WithName("Friends Feeds");
            feedsButton.WithIdentifier((int) PageIndex.PageFeeds);
            feedsButton.WithIcon(Resource.Drawable.icon_feeds);

            var forumsButton = GetBaseSecondaryItem();
            forumsButton.WithName("Forums");
            forumsButton.WithIdentifier((int) PageIndex.PageForumIndex);
            forumsButton.WithIcon(Resource.Drawable.icon_forum);

            var messagingButton = GetBaseSecondaryItem();
            messagingButton.WithName("Messaging");
            messagingButton.WithIdentifier((int) PageIndex.PageMessanging);
            messagingButton.WithIcon(Resource.Drawable.icon_message_alt);

            var historyButton = GetBaseSecondaryItem();
            historyButton.WithName("History");
            historyButton.WithIdentifier((int) PageIndex.PageHistory);
            historyButton.WithIcon(Resource.Drawable.icon_clock);

            var notificationHubButton = GetBaseSecondaryItem();
            notificationHubButton.WithName("Notification Hub");
            notificationHubButton.WithIdentifier((int) PageIndex.PageNotificationHub);
            notificationHubButton.WithIcon(Resource.Drawable.icon_notification);

            var wallpapersButton = GetBaseSecondaryItem();
            wallpapersButton.WithName("Images");
            wallpapersButton.WithIdentifier((int) PageIndex.PageWallpapers);
            wallpapersButton.WithIcon(Resource.Drawable.icon_image_alt);

            //
            var mangaSubHeader = new SectionDrawerItem();
            mangaSubHeader.WithName("Manga");
            mangaSubHeader.WithDivider(true);
            mangaSubHeader.WithTextColorRes(ResourceExtension.BrushTextRes);


            var othersSubHeader = new SectionDrawerItem();
            othersSubHeader.WithName("Other");
            othersSubHeader.WithDivider(true);
            othersSubHeader.WithTextColorRes(ResourceExtension.BrushTextRes);



            builder.WithDrawerItems(new List<IDrawerItem>()
            {
                animeButton,
                seasonalButton,
                topAnimeButton,
                searchButton,
                recomButton,
                calendarButton,
                mangaSubHeader,//
                mangaListButton,
                topMangaButton,
                othersSubHeader,//
                articlesButton,
                videoButton,
                feedsButton,
                forumsButton,
                messagingButton,
                historyButton,
                notificationHubButton,
                wallpapersButton

            });

            _drawer = builder.Build();
            UpdateLogInLabel();
            _drawer.StickyFooter.SetBackgroundColor(new Color(ResourceExtension.BrushAnimeItemInnerBackground));

            _drawer.DrawerLayout.AddDrawerListener(new DrawerListener(OnClose, OnOpen));

        }

        private void OnOpen()
        {
            var inputManager = (InputMethodManager)GetSystemService(Context.InputMethodService);
            inputManager.HideSoftInputFromWindow(MainPageSearchView.WindowToken, HideSoftInputFlags.None);
            ViewModelLocator.NavMgr.RegisterOneTimeMainOverride(new RelayCommand(() => _drawer.CloseDrawer()));
        }

        private void OnClose()
        {
            ViewModelLocator.NavMgr.ResetOneTimeMainOverride();
        }

        public async Task UpdateProfileImg(bool dl = true)
        {
            
        }

        public void SetActiveButton(HamburgerButtons val)
        {
            long id;
            switch (val)
            {
                case HamburgerButtons.AnimeList:
                    id = (long) PageIndex.PageAnimeList;
                    break;
                case HamburgerButtons.AnimeSearch:
                    id = (long) PageIndex.PageSearch;
                    break;
                case HamburgerButtons.LogIn:
                    id = (long) PageIndex.PageLogIn;
                    break;
                case HamburgerButtons.Settings:
                    id = (long) PageIndex.PageSettings;
                    break;
                case HamburgerButtons.Profile:
                    id = (long) PageIndex.PageProfile;
                    break;
                case HamburgerButtons.Seasonal:
                    id = (long) PageIndex.PageSeasonal;
                    break;
                case HamburgerButtons.About:
                    return;
                case HamburgerButtons.Recommendations:
                    id = (long) PageIndex.PageRecomendations;
                    break;
                case HamburgerButtons.MangaList:
                    id = (long) PageIndex.PageMangaList;
                    break;
                case HamburgerButtons.MangaSearch:
                    id = (long) PageIndex.PageSearch;
                    break;
                case HamburgerButtons.TopAnime:
                    id = (long) PageIndex.PageTopAnime;
                    break;
                case HamburgerButtons.TopManga:
                    id = (long) PageIndex.PageTopManga;
                    break;
                case HamburgerButtons.Calendar:
                    id = (long) PageIndex.PageCalendar;
                    break;
                case HamburgerButtons.Articles:
                    id = (long) PageIndex.PageArticles;
                    break;
                case HamburgerButtons.News:
                    id = (long) PageIndex.PageArticles;
                    break;
                case HamburgerButtons.Messanging:
                    id = (long) PageIndex.PageMessanging;
                    break;
                case HamburgerButtons.Forums:
                    id = (long) PageIndex.PageForumIndex;
                    break;
                case HamburgerButtons.History:
                    id = (long) PageIndex.PageHistory;
                    break;
                case HamburgerButtons.CharacterSearch:
                    id = (long) PageIndex.PageSearch;
                    break;
                case HamburgerButtons.Wallpapers:
                    id = (long) PageIndex.PageWallpapers;
                    break;
                case HamburgerButtons.PopularVideos:
                    id = (long) PageIndex.PagePopularVideos;
                    break;
                case HamburgerButtons.Feeds:
                    id = (long) PageIndex.PageFeeds;
                    break;
                case HamburgerButtons.Notifications:
                    id = (long) PageIndex.PageNotificationHub;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(val), val, null);
            }
            _allowHamburgerNavigation = false;
            _drawer.SetSelection(id);
            _allowHamburgerNavigation = true;
        }

        public void UpdateApiDependentButtons()
        {
            //
        }

        public void UpdateAnimeFiltersSelectedIndex()
        {
            //
        }

        public async void UpdateLogInLabel()
        {
            IDrawerItem accountButton;
            if (Credentials.Authenticated)
            {
                var btn = new ProfileDrawerItem();
                btn.WithName("Account");
                btn.WithTextColorRes(ResourceExtension.BrushTextRes);
                btn.WithSelectedColorRes(ResourceExtension.BrushAnimeItemBackgroundRes);
                btn.WithSelectedTextColorRes(ResourceExtension.AccentColourRes);
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

            _drawer.RemoveAllStickyFooterItems();
            _drawer.AddStickyFooterItem(accountButton);
            _drawer.AddStickyFooterItem(settingsButton);

            if (Credentials.Authenticated)
            {
                var btn = accountButton as ProfileDrawerItem;

                try
                {
                    var bmp = await ImageService.Instance.LoadUrl(
                            $"https://myanimelist.cdn-dena.com/images/userimages/{Credentials.Id}.jpg")
                        .AsBitmapDrawableAsync();
                    btn.WithIcon(bmp);
                }
                catch (Exception) // no image available
                {
                    btn.WithIcon(Resource.Drawable.icon_account);
                }

                _drawer.UpdateStickyFooterItem(btn);
            }
        }

        public bool MangaSectionVisbility { get; set; }

        public void SetActiveButton(TopAnimeType topType)
        {
           //
        }

        public void UpdatePinnedProfiles()
        {
            //
        }

        public void UpdateBottomMargin()
        {
           //
        }
    }
}