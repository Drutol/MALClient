using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.System;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MalClient.Shared.Comm;
using MalClient.Shared.Comm.Anime;
using MalClient.Shared.Comm.MagicalRawQueries;
using MalClient.Shared.NavArgs;
using MalClient.Shared.Utils;
using MalClient.Shared.Utils.Enums;
using MalClient.Shared.ViewModels;
using MalClient.Shared.ViewModels.Forums;
using MalClient.Shared.ViewModels.Main;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages.Forums
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ForumTopicPage : Page
    {
        private ForumsTopicNavigationArgs _args;
        private Uri _baseUri;
        private bool _navigatingRoot;
        private bool _lastpost;


        public ForumTopicViewModel ViewModel => ViewModelLocator.ForumsTopic;

        public ForumTopicPage()
        {
            this.InitializeComponent();
            Loaded += OnLoaded;
            SizeChanged += OnSizeChanged;
            _navigatingRoot = true;
            TopicWebView.DefaultBackgroundColor = Settings.SelectedTheme == ApplicationTheme.Dark ? Color.FromArgb(0xFF, 0x2f, 0x2f, 0x2f) : Color.FromArgb(0xFF,0xe6,0xe6,0xe6);
        }


        private async void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            await MalHttpContextProvider.InitializeContextForWebViews(true);
            ViewModel.WebViewTopicNavigationRequested += ViewTopicModelOnWebViewTopicNavigationRequested;
            ViewModel.WebViewNewTopicNavigationRequested += ViewModelOnWebViewNewTopicNavigationRequested;
            ViewModel.Init(_args);
        }

        private void ViewModelOnWebViewNewTopicNavigationRequested(string content, bool b)
        {
            _baseUri = new Uri($"http://myanimelist.net/forum/?action=post&boardid={content}");
            TopicWebView.Navigate(_baseUri);
        }

        private void ViewTopicModelOnWebViewTopicNavigationRequested(string content,bool arg)
        {
            _baseUri = new Uri($"http://myanimelist.net/forum/?topicid={content}{(arg ? "&goto=lastpost" : "")}");
            _lastpost = arg;
            TopicWebView.Navigate(_baseUri);
        }

        

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _args = e.Parameter as ForumsTopicNavigationArgs;
            base.OnNavigatedTo(e);
        }

        private async void TopicWebView_OnLoadCompleted(object sender, NavigationEventArgs e)
        {

        }


        private async void TopicWebView_OnDOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            var uiSettings = new UISettings();
            var color = uiSettings.GetColorValue(UIColorType.Accent);
            //this chain of commands will remove unnecessary stuff
            string bodyLight = Settings.SelectedTheme == ApplicationTheme.Dark ? "#3d3d3d" : "#d0d0d0";
            string bodyLighter = Settings.SelectedTheme == ApplicationTheme.Dark ? "#2f2f2f" : "#e6e6e6";
            string bodyDarker = Settings.SelectedTheme == ApplicationTheme.Dark ? "#212121" : "#cacaca";
            string fontColor = Settings.SelectedTheme == ApplicationTheme.Dark ? "white" : "black";
            string fontColorInverted = Settings.SelectedTheme == ApplicationTheme.Dark ? "black" : "white";

            var zoom = 100*ActualWidth/500;
            _prevSize = new Size(ActualWidth, ActualHeight);
            List<string> commands;
            if (_args.CreateNewTopic)
            {
                commands = new List<string>
                {
                    @"document.getElementById(""headerSmall"").outerHTML='';document.getElementById(""menu"").outerHTML='';document.getElementsByClassName(""js-sns-icon-container icon-block-small"")[0].outerHTML='';document.getElementsByTagName(""footer"")[0].innerHTML='';document.getElementsByClassName(""mauto clearfix pt24"")[0].outerHTML='';",
                    @"$(""#contentWrapper"").find('div:first').remove();",
                    $@"$(""#contentWrapper"").css(""background-color"", ""{bodyLighter}"").css(""width"", ""700px"");;",
                    $@"$(""body"").css(""font-family"", ""Segoe UI"").css(""color"", ""{fontColor}"").css(""background-color"", ""{bodyLighter}"").css(""width"", ""700px"");;",
                    @"$(""footer"").remove()",
                    $@"$(""textarea"").css(""background-color"",""{bodyDarker}"").css(""color"", ""{fontColor}"")",
                    $@"$(""td"").css(""color"", ""{fontColor}"")",
                    $@"$(""a"").css(""color"", ""#{color.ToString().Substring(3)}"");",
                    $@"$(""#content"").css(""border-color"", ""{bodyLighter}"").css(""background-color"",""{bodyLighter}"");",
                    $@"$(""html"").css(""zoom"", ""{Math.Floor(zoom)}%"").css(""background-color"", ""{bodyLighter}"").css(""width"", ""700px"");",
                    @"$(""iframe"").remove()",
                    $@"$(""#dialog"").css(""border-color"", ""{bodyLight}"")",
                    $@"$(""td"").css(""border-color"", ""{bodyDarker}"")",
                    $@"$("".inputtext"").css(""background-color"", ""{bodyDarker}"").css(""color"", ""{fontColor}"")",
                    $@"$("".normal_header"").css(""color"", ""{fontColor}"")",
                    $@"$("".inputButton"").css(""background-color"", ""{bodyLight}"").css(""border-color"",""{fontColorInverted}"");",
                    $@"$("".bgbdrContainer"").css(""background-color"", ""{bodyDarker}"").css(""border-color"",""{fontColorInverted}"");",
                };
            }
            else
            {
                commands = new List<string>
                {
                    @"$("".header"").remove();
                      $(""iframe"").remove();
                      $("".select.filter-sort"").remove();
                      $("".anchor-ad"").remove();
                      $(""footer"").remove();
                      $("".page-title"").remove();
                      $("".sns-unit"").remove();",
                    $@"$(""a"").css(""color"", ""#{color.ToString().Substring(3)}"");",
                    $@"$("".comment-title"").css(""background-color"",""{bodyLighter}"")",
                    $@"$("".breadcrumb"").css(""background-color"",""{bodyLighter}"")",       
                    $@"$("".num"").removeClass().css(""font-size"",""30px"").css(""color"", ""{fontColor}"").css(""margin"", ""13px"")",
                    $@"$("".icon-next"").not(""a.prev"").removeClass().css(""font-size"",""30px"").html(""»"")",
                    $@"$("".icon-next.prev"").css(""font-size"",""20px"").removeClass().html(""«"")",
                    $@"$("".db-ib"").removeClass().css(""font-size"",""30px"").css(""color"",""#{color.ToString().Substring(3)}"")",
                    $@"$("".btn-post-comment"").css(""background-color"",""{bodyLight}"").css(""color"", ""{fontColor}"").css(""border-color"", ""{bodyDarker}"")",
                    $@"$(""body"").css(""font-family"", ""Segoe UI"").css(""color"", ""{fontColor}"").css(""background-color"", ""{bodyLighter}"");",
                };
            }
            
            foreach (var command in commands)
            {
                try
                {
                    await TopicWebView.InvokeScriptAsync("eval", new string[] {command});
                }
                catch (Exception)
                {
                    //htm.. no it's javascript this time oh, how fun!
                }

            }
            ViewModel.LoadingTopic = Visibility.Collapsed;
        }

        private Size _prevSize;

        private async void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(Math.Abs(e.NewSize.Width - _prevSize.Width) < 10)
                return;
            _prevSize = e.NewSize;
            try
            {
                await TopicWebView.InvokeScriptAsync("eval", new string[] { $"$(\"html\").css(\"zoom\", \"{Math.Floor(100 * ActualWidth / 500)}%\");", });
            }
            catch (Exception)
            {
                //htm.. no it's javascript this time oh, how fun!
            }
        }

        private async void TopicWebView_OnNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            if (_navigatingRoot)
            {
                return;
            }
            if (args.Uri.ToString().Contains(_baseUri.ToString()))
                return;
            if (_lastpost && args.Uri.ToString().Contains("&show="))
                return;
            try
            {
                if (args.Uri != null)
                {
                    var uri = args.Uri.AbsoluteUri;
                    args.Cancel = true;
                    if (Regex.IsMatch(uri, @"http:\/\/myanimelist.net\/forum\/\?subboard=\d+"))
                    {
                        var id = uri.Split('=').Last();
                        ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageForumIndex, _args);
                        if (id == "1")
                            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageForumIndex, new ForumsBoardNavigationArgs(ForumBoards.AnimeSeriesDisc));
                        else if (id == "4")
                            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageForumIndex, new ForumsBoardNavigationArgs(ForumBoards.MangaSeriesDisc));
                        return;
                    }
                    if (Regex.IsMatch(uri, @"http:\/\/myanimelist.net\/forum\/\?board=\d+"))
                    {
                        ForumBoards board;
                        if (ForumBoards.TryParse(uri.Split('=').Last(), out board))
                        {
                            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageForumIndex, _args);
                            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageForumIndex, new ForumsBoardNavigationArgs(board));
                        }
                        return;
                    }
                    if (Regex.IsMatch(uri, @"http:\/\/myanimelist.net\/forum\/\?animeid=\d+"))
                    {
                        int id;
                        if (int.TryParse(uri.Split('=').Last(), out id))
                        {
                            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageForumIndex, _args);
                            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageForumIndex, new ForumsBoardNavigationArgs(id, "Anime Series Board", true));
                        }
                        return;
                    }
                    if (Regex.IsMatch(uri, @"http:\/\/myanimelist.net\/forum\/\?mangaid=\d+"))
                    {
                        int id;
                        if (int.TryParse(uri.Split('=').Last(), out id))
                        {
                            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageForumIndex, _args);
                            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageForumIndex, new ForumsBoardNavigationArgs(id, "Manga Series Board", false));
                        }
                        return;
                    }
                    if (Regex.IsMatch(uri, @"http:\/\/myanimelist\.net\/forum\/message\/\d+\?goto=topic"))
                    {
                        args.Cancel = false;
                        return;
                    }
                    if (Regex.IsMatch(uri, @"http:\/\/myanimelist.net\/forum\/\?topicid=\d+"))
                    {
                        var id = uri.Split('=').Last();
                        ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageForumIndex, _args);
                        ViewModelLocator.GeneralMain.Navigate(PageIndex.PageForumIndex, new ForumsTopicNavigationArgs(id, ForumBoards.Creative));
                        return;
                    }
                    if (uri == "http://myanimelist.net/forum/")
                    {
                        ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageForumIndex, _args);
                        ViewModelLocator.GeneralMain.Navigate(PageIndex.PageForumIndex, new ForumsNavigationArgs());
                        return;
                    }
                    var paramIndex = uri.IndexOf('?');
                    if (paramIndex != -1)
                        uri = uri.Substring(0, paramIndex);
                    if (Regex.IsMatch(uri, "anime\\/\\d") ||
                        (Settings.SelectedApiType != ApiType.Hummingbird && Regex.IsMatch(uri, "manga\\/\\d")))
                    {
                        var link = uri.Substring(7).Split('/');
                        if (link[3] == "")
                        {
                            if (Settings.ArticlesLaunchExternalLinks)
                            {
                                await Launcher.LaunchUriAsync(args.Uri);
                            }
                            return;
                        }
                        var id = int.Parse(link[2]);
                        if (Settings.SelectedApiType == ApiType.Hummingbird) //id switch            
                            id = await new AnimeDetailsHummingbirdQuery(id).GetHummingbirdId();
                        ViewModelLocator.GeneralMain.Navigate(PageIndex.PageAnimeDetails,
                            new AnimeDetailsPageNavigationArgs(id, link[3], null, null,_args)
                            {
                                AnimeMode = link[1] == "anime",
                                Source = PageIndex.PageForumIndex
                                
                            });
                    }
                    else if (uri.Contains("/profile/"))
                    {
                        ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageForumIndex, _args);
                        ViewModelLocator.GeneralMain.Navigate(PageIndex.PageProfile, new ProfilePageNavigationArgs { TargetUser = uri.Split('/').Last() });
                    }
                    else if (Settings.ArticlesLaunchExternalLinks)
                    {
                        await Launcher.LaunchUriAsync(args.Uri);
                    }
                }
            }
            catch (Exception)
            {
                args.Cancel = true;
            }
        }

        private void TopicWebView_OnFrameNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            //no no to iframes on mobile no freaking way
            args.Cancel = true;
        }

        private void TopicWebView_OnContentLoading(WebView sender, WebViewContentLoadingEventArgs args)
        {
            _navigatingRoot = false;
        }
    }
}
