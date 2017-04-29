using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Android.OS;
using Android.Support.V4.View;
using Android.Webkit;
using Android.Widget;
using Com.Astuetz;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.BindingConverters;
using MALClient.Android.PagerAdapters;
using MALClient.Android.Resources;
using MALClient.Android.Web;
using MALClient.Models.Enums;
using MALClient.Models.Models.MalSpecific;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;
using MALClient.Android.Web;

namespace MALClient.Android.Fragments.ArticlesPageFragments
{
    public class ArticlesPageFragment : MalFragmentBase
    {
        private MalArticlesViewModel ViewModel;
        private ListenableWebClient _listenableWebClient;

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = ViewModelLocator.MalArticles;
            ViewModel.Init(MalArticlesPageNavigationArgs.Articles);
            
            ViewModel.OpenWebView += ViewModelOnOpenWebView;

            _listenableWebClient = new ListenableWebClient();
            _listenableWebClient.PageReady += PageReady;
            _listenableWebClient.NavigationInterceptOpportunity += NavigationInterceptOpportunity;
        }

        protected override void InitBindings()
        {
            ArticlesPagePivot.Adapter = new ArticlesPagePagerAdapter(ChildFragmentManager);
            ArticlesPageTabStrip.SetViewPager(ArticlesPagePivot);
            ArticlesPageTabStrip.CenterTabs();


            ArticlesPageWebView.SetWebViewClient(_listenableWebClient);
            ArticlesPageWebView.Settings.JavaScriptEnabled = true;

            Bindings.Add(
                this.SetBinding(() => ViewModel.WebViewVisibility,
                    () => ArticlesPageWebView.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));

            Bindings.Add(
                this.SetBinding(() => ViewModel.LoadingVisibility,
                    () => ArticlesPageLoadingSpinner.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));         
        }

        public override int LayoutResourceId => Resource.Layout.ArticlesPage;

        private int _currentId;
        private void ViewModelOnOpenWebView(string html, int id)
        {
            //BackNav
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageArticles,
                ViewModel.Articles[id].Type == MalNewsType.Article
                    ? MalArticlesPageNavigationArgs.Articles
                    : MalArticlesPageNavigationArgs.News);
            //
            _currentId = id;
            var color = '#' + ResourceExtension.AccentColourHex.Substring(3);//uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Accent);
            var color1 = '#' + ResourceExtension.AccentColourDarkHex.Substring(3);//uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.AccentDark2);
            var color2 = '#' +ResourceExtension.AccentColourLightHex.Substring(3);//uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.AccentLight2);
            var css = Css.Replace("AccentColourBase", color).
                Replace("AccentColourLight", color2).
                Replace("AccentColourDark", color1)
                .Replace("BodyBackgroundThemeColor",
                    Settings.SelectedTheme == 1 ? "#2d2d2d" : "#e6e6e6")
                .Replace("BodyForegroundThemeColor",
                    Settings.SelectedTheme == 1 ? "white" : "black").Replace(
                "HorizontalSeparatorColor", Settings.SelectedTheme == 1 ? "#0d0d0d" : "#b3b3b3");
            //ArticlesPageWebView.NavigateToString();

            ArticlesPageWebView.LoadDataWithBaseURL(null,css + Begin + html + "</div></body></html>", "text/html; charset=utf-8", "UTF-8",null);
        }


        private async Task<string> NavigationInterceptOpportunity(string targetUrl)
        {
            if (targetUrl != null)
            {
                var navArgs = MalLinkParser.GetNavigationParametersForUrl(targetUrl);
                if (navArgs != null)
                {
                    if (navArgs.Item1 != PageIndex.PageAnimeDetails)
                    {
                        ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageArticles, null);
                    }
                    else
                    {
                        var arg = navArgs.Item2 as AnimeDetailsPageNavigationArgs;
                        arg.Source = PageIndex.PageArticles;
                    }


                    ViewModelLocator.GeneralMain.Navigate(navArgs.Item1, navArgs.Item2);
                }
                else if (Settings.ArticlesLaunchExternalLinks)
                {
                    ResourceLocator.SystemControlsLauncherService.LaunchUri(new Uri(targetUrl));
                }
            }
            return null;
        }

        private void PageReady()
        {
            ViewModel.LoadingVisibility = false;
            ViewModel.WebViewVisibility = true;
        }

        #region Views

        private PagerSlidingTabStrip _articlesPageTabStrip;
        private ViewPager _articlesPagePivot;
        private WebView _articlesPageWebView;
        private ProgressBar _articlesPageLoadingSpinner;

        public PagerSlidingTabStrip ArticlesPageTabStrip => _articlesPageTabStrip ?? (_articlesPageTabStrip = FindViewById<PagerSlidingTabStrip>(Resource.Id.ArticlesPageTabStrip));

        public ViewPager ArticlesPagePivot => _articlesPagePivot ?? (_articlesPagePivot = FindViewById<ViewPager>(Resource.Id.ArticlesPagePivot));

        public WebView ArticlesPageWebView => _articlesPageWebView ?? (_articlesPageWebView = FindViewById<WebView>(Resource.Id.ArticlesPageWebView));

        public ProgressBar ArticlesPageLoadingSpinner => _articlesPageLoadingSpinner ?? (_articlesPageLoadingSpinner = FindViewById<ProgressBar>(Resource.Id.ArticlesPageLoadingSpinner));

        #endregion

        const string Begin = @"<html><body id='root'><div id='content'>";
        #region Css
        const string Css =
            @"<style type=""text/css"">@charset ""UTF-8"";
	        html, body
	        {
                zoom: 100%;
		        background-color: BodyBackgroundThemeColor;
		        color: BodyForegroundThemeColor;
                font-family: 'Segoe UI';
	        }
	        .userimg
	        {
		        display: block;
		        margin: 10px auto;
		        max-width: 100%;
		        height: auto;
		        -webkit-box-shadow: 0px 0px 67px 5px rgba(0,0,0,0.58);
		        -moz-box-shadow: 0px 0px 67px 5px rgba(0,0,0,0.58);
		        box-shadow: 0px 0px 67px 5px rgba(0,0,0,0.58);
	        }
	        a
	        {
		        font-weight: bold;
		        text-decoration: none;
	        }
            a:link{color:AccentColourBase}
            a:active{color:AccentColourBase}
            a:visited{color:AccentColourDark}
            a:hover{color:AccentColourLight}
        h1 {
	        font-family: 'Segoe UI', Frutiger, 'Frutiger Linotype', 'Dejavu Sans', 'Helvetica Neue', Arial, sans-serif;
	        font-size: 24px;
	        font-style: normal;
	        font-variant: normal;
	        font-weight: 500;
	        line-height: 26.4px;
        }
        h2 {
	        font-family: 'Segoe UI', Frutiger, 'Frutiger Linotype', 'Dejavu Sans', 'Helvetica Neue', Arial, sans-serif;
	        font-size: 24px;
	        font-style: normal;
	        font-variant: normal;
	        font-weight: 500;
	        line-height: 26.4px;
        }
        h3 {
	        font-family: 'Segoe UI', Frutiger, 'Frutiger Linotype', 'Dejavu Sans', 'Helvetica Neue', Arial, sans-serif;
	        font-size: 18px;
	        font-style: normal;
	        font-variant: normal;
	        position: relative;
	        text-align: center;
	        font-weight: 500;
	        line-height: 15.4px;
        }
        h4 {
	        font-family: 'Segoe UI', Frutiger, 'Frutiger Linotype', 'Dejavu Sans', 'Helvetica Neue', Arial, sans-serif;
	        font-size: 14px;
	        font-style: normal;
	        font-variant: normal;
	        font-weight: 500;
	        line-height: 15.4px;
        }
        hr {
            display: block;
            height: 2px;
	        background-color: HorizontalSeparatorColor;
	        border-radius: 10px 10px 10px 10px;
	        -moz-border-radius: 10px 10px 10px 10px;
	        -webkit-border-radius: 10px 10px 10px 10px;
	        border: 1px solid #1f1f1f;
            margin: 1em 0;
            margin-right: 20px;
            padding-right: 0;
        }
        p {
	        font-family: 'Segoe UI', Frutiger, 'Frutiger Linotype', 'Dejavu Sans', 'Helvetica Neue', Arial, sans-serif;
	        font-size: 14px;
	        font-style: normal;
	        font-variant: normal;
	        font-weight: 400;
	        line-height: 20px;
        }
        blockquote {
	        font-family: 'Segoe UI', Frutiger, 'Frutiger Linotype', 'Dejavu Sans', 'Helvetica Neue', Arial, sans-serif;
	        font-size: 21px;
	        font-style: normal;
	        font-variant: normal;
	        font-weight: 400;
	        line-height: 30px;
        }
        pre {
	        font-family: 'Segoe UI', Frutiger, 'Frutiger Linotype', 'Dejavu Sans', 'Helvetica Neue', Arial, sans-serif;
	        font-size: 13px;
	        font-style: normal;
	        font-variant: normal;
	        font-weight: 400;
	        line-height: 18.5714px;
        }

        .tags
        {
	        position: absolute;
            left: -9999px;
        }
        .js-sns-icon-container
        {
	        position: absolute;
            left: -9999px;
        }

        .news-info-block
        {
	        width: 100%;
	        border-style: solid;
            border-width: 0px 0px 2px 0px;
            border-color: rgba(0, 0, 0, 0);
        }

        .information
        {
	        font-family: 'Segoe UI', Frutiger, 'Frutiger Linotype', 'Dejavu Sans', 'Helvetica Neue', Arial, sans-serif;
	        font-size: 12px;
	        font-style: normal;
	        font-variant: normal;
	        font-weight: 500;
	
        }</style>";
        #endregion
    }
}