using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.Animations;
using Android.Views.InputMethods;
using Android.Widget;
using Com.Mikepenz.Materialdrawer;
using Com.Mikepenz.Materialdrawer.Model;
using Com.Mikepenz.Materialdrawer.Model.Interfaces;
using Com.Shehabic.Droppy;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Dialogs;
using MALClient.Android.Flyouts;
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
        private bool _allowHamburgerNavigation = true;
        private View _accountHamburgerView;
        private bool _selectedProfileItem;
        private bool _selectedSettingsItem;
        private PopupMenu _supportMenu;
        private View _settingsHamburgerView;
        private View _footerView;
        private bool _mangaSectionVisbility;
        private DroppyMenuPopup _moreMenu;

        public event EventHandler HamburgerOpened;

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

            var animeButton = GetBasePrimaryItem(OnAnimeListMore);
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

            var topAnimeButton = GetBasePrimaryItem(OnTopAnimeMore);
            topAnimeButton.WithName("Top anime");
            topAnimeButton.WithIdentifier((int)PageIndex.PageTopAnime);
            topAnimeButton.WithIcon(Resource.Drawable.icon_fav_outline);

            var calendarButton = GetBasePrimaryItem();
            calendarButton.WithName("Calendar");
            calendarButton.WithIdentifier((int)PageIndex.PageCalendar);
            calendarButton.WithIcon(Resource.Drawable.icon_calendar);

            //



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

            //var messagingButton = GetBaseSecondaryItem();
            //messagingButton.WithName("Messaging");
            //messagingButton.WithIdentifier((int) PageIndex.PageMessanging);
            //messagingButton.WithIcon(Resource.Drawable.icon_message_alt);

            var historyButton = GetBaseSecondaryItem();
            historyButton.WithName("History");
            historyButton.WithIdentifier((int) PageIndex.PageHistory);
            historyButton.WithIcon(Resource.Drawable.icon_clock);

            //var notificationHubButton = GetBaseSecondaryItem();
            //notificationHubButton.WithName("Notification Hub");
            //notificationHubButton.WithIdentifier((int) PageIndex.PageNotificationHub);
            //notificationHubButton.WithIcon(Resource.Drawable.icon_notification);

            var wallpapersButton = GetBaseSecondaryItem();
            wallpapersButton.WithName("Images");
            wallpapersButton.WithIdentifier((int) PageIndex.PageWallpapers);
            wallpapersButton.WithIcon(Resource.Drawable.icon_image_alt);

            var clubsButton = GetBaseSecondaryItem();
            clubsButton.WithName("Clubs");
            clubsButton.WithIdentifier((int) PageIndex.PageClubIndex);
            clubsButton.WithIcon(Resource.Drawable.icon_club);

            //
            var othersSubHeader = new SectionDrawerItem();
            othersSubHeader.WithName("Other");
            othersSubHeader.WithDivider(true);
            othersSubHeader.WithTextColorRes(ResourceExtension.BrushTextRes);

            if (!Settings.HamburgerHideMangaSection)
            {
                var mangaSubHeader = new SectionDrawerItem();
                mangaSubHeader.WithName("Manga");
                mangaSubHeader.WithDivider(true);
                mangaSubHeader.WithIdentifier(55779988);
                mangaSubHeader.WithTextColorRes(ResourceExtension.BrushTextRes);

                var mangaListButton = GetBaseSecondaryItem(OnMangaMore);
                mangaListButton.WithName("Manga list");
                mangaListButton.WithIdentifier((int)PageIndex.PageMangaList);
                mangaListButton.WithIcon(Resource.Drawable.icon_list);

                var topMangaButton = GetBaseSecondaryItem();
                topMangaButton.WithName("Top manga");
                topMangaButton.WithIdentifier((int)PageIndex.PageTopManga);
                topMangaButton.WithIcon(Resource.Drawable.icon_fav_outline);

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
                    // messagingButton,
                    historyButton,
                    clubsButton,
                    // notificationHubButton,
                    wallpapersButton

                });
            }
            else
            {
                builder.WithDrawerItems(new List<IDrawerItem>()
                {
                    animeButton,
                    seasonalButton,
                    topAnimeButton,
                    searchButton,
                    recomButton,
                    calendarButton,
                    othersSubHeader,//
                    articlesButton,
                    videoButton,
                    feedsButton,
                    forumsButton,
                    // messagingButton,
                    historyButton,
                    // notificationHubButton,
                    wallpapersButton

                });
            }
            _drawer = builder.Build();

            UpdateLogInLabel();
            _drawer.StickyFooter.SetBackgroundColor(new Color(Settings.SelectedTheme == 1
                ? ResourceExtension.BrushAnimeItemInnerBackground
                : ResourceExtension.BrushAnimeItemBackground));

            _drawer.DrawerLayout.AddDrawerListener(new DrawerListener(OnClose, OnOpen));
        }

        private void OnMangaMore(View view)
        {
            _moreMenu = FlyoutMenuBuilder.BuildGenericFlyout(this, view,
                Enum.GetValues(typeof(AnimeStatus)).Cast<AnimeStatus>().Select(type => XShared.Utils.Utilities.StatusToString((int)type,true)).ToList(),
                i =>
                {
                    ViewModel.Navigate(PageIndex.PageTopAnime, new AnimeListPageNavigationArgs(i, AnimeListWorkModes.Manga));
                    _moreMenu.Dismiss(true);
                    _drawer.CloseDrawer();
                });
            _moreMenu.Show();
        }

        private void OnTopAnimeMore(View view)
        {
            _moreMenu = FlyoutMenuBuilder.BuildGenericFlyout(this, view,
                Enum.GetValues(typeof(TopAnimeType)).Cast<TopAnimeType>().Select(type => type.ToString()).ToList(),
                i =>
                {
                    ViewModel.Navigate(PageIndex.PageTopAnime, AnimeListPageNavigationArgs.TopAnime((TopAnimeType) i));
                    _moreMenu.Dismiss(true);
                    _drawer.CloseDrawer();
                });
            _moreMenu.Show();

        }

        private void OnAnimeListMore(View view)
        {
            _moreMenu = FlyoutMenuBuilder.BuildGenericFlyout(this, view,
                Enum.GetValues(typeof(AnimeStatus)).Cast<AnimeStatus>().Select(type => XShared.Utils.Utilities.StatusToString((int)type)).ToList(),
                i =>
                {
                    ViewModel.Navigate(PageIndex.PageTopAnime, new AnimeListPageNavigationArgs(i,AnimeListWorkModes.Anime));
                    _moreMenu.Dismiss(true);
                    _drawer.CloseDrawer();
                });
            _moreMenu.Show();
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
            UpdateLogInLabel();
        }

        public void SetActiveButton(HamburgerButtons val)
        {
            if (_footerView != null)
            {
                if (_selectedProfileItem)
                {
                    if (val == HamburgerButtons.Profile)
                        return;

                    _accountHamburgerView.SetBackgroundColor(Color.Transparent);
                    _accountHamburgerView.FindViewById<TextView>(Resource.Id.HamburgerProfileItemLabel).SetTextColor(new Color(ResourceExtension.BrushText));
                    _selectedProfileItem = false;
                }
                if (_selectedSettingsItem)
                {
                    if (val == HamburgerButtons.Settings)
                        return;

                    _settingsHamburgerView.SetBackgroundColor(Color.Transparent);
                    _settingsHamburgerView.FindViewById<TextView>(Resource.Id.HamburgerSettingsItemLabel).SetTextColor(new Color(ResourceExtension.BrushText));
                    _settingsHamburgerView.FindViewById<ImageView>(Resource.Id.HamburgerSettingsItemIcon).ImageTintList = ColorStateList.ValueOf(new Color(ResourceExtension.BrushText));
                    _selectedSettingsItem = false;
                }
            }
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
                    if (_settingsHamburgerView != null)
                    {
                        _settingsHamburgerView.SetBackgroundColor(Settings.SelectedTheme == 1
                            ? new Color(ResourceExtension.BrushAnimeItemBackground)
                            : Color.White);
                        _settingsHamburgerView.FindViewById<TextView>(Resource.Id.HamburgerSettingsItemLabel).SetTextColor(new Color(ResourceExtension.AccentColour));
                        _settingsHamburgerView.FindViewById<ImageView>(Resource.Id.HamburgerSettingsItemIcon).ImageTintList = ColorStateList.ValueOf(new Color(ResourceExtension.AccentColour));
                        _selectedSettingsItem = true;
                    }
                    _drawer.SetSelectionAtPosition(-1);
                    return;
                case HamburgerButtons.Profile:
                    _accountHamburgerView.SetBackgroundColor(Settings.SelectedTheme == 1
                        ? new Color(ResourceExtension.BrushAnimeItemBackground)
                        : Color.White);
                    _accountHamburgerView.FindViewById<TextView>(Resource.Id.HamburgerProfileItemLabel).SetTextColor(new Color(ResourceExtension.AccentColour));
                    _selectedProfileItem = true;
                    _drawer.SetSelectionAtPosition(-1);
                    return;
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
                    _drawer.SetSelectionAtPosition(-1);
                    return;
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
                    _drawer.SetSelectionAtPosition(-1);
                    return;
                case HamburgerButtons.Clubs:
                    id = (long) PageIndex.PageClubIndex;
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

        public void UpdateLogInLabel()
        {
            if (Credentials.Authenticated)
            {
                var btn = new ContainerDrawerItem();
                if (_accountHamburgerView == null)
                {
                    _footerView = LayoutInflater.Inflate(Resource.Layout.HamburgerProfileItem, null);
                    _accountHamburgerView = _footerView.FindViewById(Resource.Id.HamburgerBottomViewProfileItem);
                    _settingsHamburgerView = _footerView.FindViewById(Resource.Id.HamburgerBottomViewSettingsItem);

                    //
                    _accountHamburgerView.SetOnClickListener(new OnClickListener(view =>
                    {
                        SetActiveButton(HamburgerButtons.Profile);
                        OnHamburgerItemClick(PageIndex.PageProfile);
                    }));
                    var listener = new OnClickListener(OnProfileSubItemCLick);
                    _accountHamburgerView.FindViewById(Resource.Id.HamburgerProfileItemNotifications)
                        .SetOnClickListener(listener);
                    _accountHamburgerView.FindViewById(Resource.Id.HamburgerProfileItemMessages)
                        .SetOnClickListener(listener);
                    _accountHamburgerView.SetOnLongClickListener(new OnLongClickListener(view =>
                    {
                        var d = new PinnedUsersDialog(ResourceLocator.HandyDataStorage.PinnedUsers);
                        _drawer.CloseDrawer();
                    }));

                    var support = _settingsHamburgerView.FindViewById(Resource.Id.HamburgerProfileItemSupport);
                    support.SetOnClickListener(listener);
                    support.StartAnimation(AnimationUtils.LoadAnimation(this, Resource.Animator.animation_pulse));
                    //
                    _settingsHamburgerView.SetOnClickListener(new OnClickListener(view =>
                    {
                        SetActiveButton(HamburgerButtons.Settings);
                        OnHamburgerItemClick(PageIndex.PageSettings);
                    }));

                }

                ImageService.Instance
                    .LoadUrl($"https://cdn.myanimelist.net/images/userimages/{Credentials.Id}.webp",
                        TimeSpan.FromMinutes(10))
                    .FadeAnimation(false).Transform(new CircleTransformation())
                    .Error(e =>
                    {
                        ImageService.Instance
                            .LoadUrl($"https://cdn.myanimelist.net/images/userimages/{Credentials.Id}.jpg",
                                TimeSpan.FromMinutes(10))
                            .FadeAnimation(false).Transform(new CircleTransformation())
                            .Into(_accountHamburgerView.FindViewById<ImageViewAsync>(Resource.Id
                                .HamburgerProfileItemImage));
                    })
                    .Into(_accountHamburgerView.FindViewById<ImageViewAsync>(Resource.Id.HamburgerProfileItemImage));
                btn.WithView(_footerView);
                btn.WithSelectable(false);
                btn.WithDivider(false);

                _drawer.RemoveAllStickyFooterItems();
                _drawer.AddStickyFooterItem(btn);
            }
            else
            {
                var accountButton = GetBaseSecondaryItem();
                accountButton.WithName("Sign in");
                accountButton.WithIdentifier((int)PageIndex.PageLogIn);
                accountButton.WithIcon(Resource.Drawable.icon_login);

                var settingsButton = GetBaseSecondaryItem();
                settingsButton.WithName("Settings & more");
                settingsButton.WithIdentifier((int)PageIndex.PageSettings);
                settingsButton.WithIcon(Resource.Drawable.icon_settings);

                
                _drawer.RemoveAllStickyFooterItems();
                _drawer.AddStickyFooterItem(accountButton);
                _drawer.AddStickyFooterItem(settingsButton);
            }


            //var settingsButton = new ContainerDrawerItem();
            //settingsButton.WithSelectable(false);
            //settingsButton.WithDivider(false);


            if (_footerView != null)
            {
                var par = _footerView.Parent as View;
                par.SetPadding(0, 0, 0, 0);
                var param = par.LayoutParameters;
                param.Height = DimensionsHelper.DpToPx(130);
                par.LayoutParameters = param;
            }

        }

        private void OnProfileSubItemCLick(View view)
        {
            switch (view.Id)
            {
                case Resource.Id.HamburgerProfileItemNotifications:
                    OnHamburgerItemClick(PageIndex.PageNotificationHub);
                    break;
                case Resource.Id.HamburgerProfileItemMessages:
                    OnHamburgerItemClick(PageIndex.PageMessanging);
                    break;
                case Resource.Id.HamburgerProfileItemSupport:
                    _supportMenu = new PopupMenu(this, view);

                    _supportMenu.Menu.Add(0,0,0,"GitHub Feedback");
                    _supportMenu.Menu.Add(0,1,0,"Review");
                    _supportMenu.Menu.Add(0,2,0,"Donate Google");
                    //_supportMenu.Menu.Add(0,3,0,"Donate PayPal");
                    _supportMenu.Menu.Add(0,4,0,"Turn on ads");

                    _supportMenu.Show();

                    _supportMenu.DismissEvent += (sender, args) =>
                    {
                        ViewModelLocator.NavMgr.RegisterOneTimeOverride(
                            new RelayCommand(() => _drawer.CloseDrawer()));
                    };

                    _supportMenu.SetOnMenuItemClickListener(new AnimeItemFlyoutBuilder.MenuListener(item =>
                    {
                        OnSupportMenuSelection(item.ItemId);
                    }));

                    break;
            }
        }

        private void OnSupportMenuSelection(int i)
        {
            switch (i)
            {

                case 0:
                    ResourceLocator.SystemControlsLauncherService.LaunchUri(
                        new Uri("https://github.com/Drutol/MALClient/issues"));
                    break;
                case 1:
                    AndroidViewModelLocator.Settings.ReviewCommand.Execute(null);
                    break;
                case 2:
                    ViewModelLocator.GeneralMain.Navigate(PageIndex.PageSettings,SettingsPageIndex.About);
                    SetActiveButton(HamburgerButtons.Settings);
                    _drawer.CloseDrawer();
                    break;
                case 3:
                    ResourceLocator.SystemControlsLauncherService.LaunchUri(new Uri("https://www.paypal.me/drutol"));
                    break;
                case 4:
                    ViewModelLocator.GeneralMain.Navigate(PageIndex.PageSettings,SettingsPageIndex.Ads);
                    SetActiveButton(HamburgerButtons.Settings);
                    _drawer.CloseDrawer();
                    break;
                //case 6:
                //    FeedbackManager.ShowFeedbackActivity(ApplicationContext);
                //    break;
            }
            _supportMenu.Dismiss();
        }

        public bool MangaSectionVisbility
        {
            get { return _mangaSectionVisbility; }
            set
            {
                _mangaSectionVisbility = value;
                if (value)
                {
                    var mangaSubHeader = new SectionDrawerItem();
                    mangaSubHeader.WithName("Manga");
                    mangaSubHeader.WithDivider(true);
                    mangaSubHeader.WithIdentifier(55779988);
                    mangaSubHeader.WithTextColorRes(ResourceExtension.BrushTextRes);

                    var mangaListButton = GetBaseSecondaryItem(OnMangaMore);
                    mangaListButton.WithName("Manga list");
                    mangaListButton.WithIdentifier((int)PageIndex.PageMangaList);
                    mangaListButton.WithIcon(Resource.Drawable.icon_list);

                    var topMangaButton = GetBaseSecondaryItem();
                    topMangaButton.WithName("Top manga");
                    topMangaButton.WithIdentifier((int)PageIndex.PageTopManga);
                    topMangaButton.WithIcon(Resource.Drawable.icon_fav_outline);

                    _drawer.AddItemsAtPosition(6, mangaSubHeader, mangaListButton, topMangaButton);
                }
                else
                {
                    _drawer.RemoveItems(55779988, (long)PageIndex.PageMangaList, (long)PageIndex.PageTopManga);
                }
            }
        }

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