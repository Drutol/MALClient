using System;
using System.Linq;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.System;
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
        }


        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            ViewModel.WebViewNavigationRequested += ViewModelOnWebViewNavigationRequested;
            ViewModel.Init(_args);
        }

        private async void ViewModelOnWebViewNavigationRequested(string content,bool arg)
        {
            await MalHttpContextProvider.InitializeContextForWebViews();
            //TopicWebView.Navigate(new Uri("http://myanimelist.net/forum/?topicid=1499207"));
            _baseUri = new Uri($"http://myanimelist.net/forum/?topicid={content}{(arg ? "&goto=lastpost" : "")}");
            _lastpost = arg;
            TopicWebView.Navigate(_baseUri);
            ViewModel.LoadingTopic = Visibility.Visible;

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
            string bodyLight = Settings.SelectedTheme == ApplicationTheme.Dark ? "#3d3d3d" : "#e6e6e6";
            string bodyLighter = Settings.SelectedTheme == ApplicationTheme.Dark ? "#2f2f2f" : "#e6e6e6";
            string bodyDarker = Settings.SelectedTheme == ApplicationTheme.Dark ? "#212121" : "#e6e6e6";
            string fontColor = Settings.SelectedTheme == ApplicationTheme.Dark ? "white" : "black";
            string fontColorInverted = Settings.SelectedTheme == ApplicationTheme.Dark ? "black" : "white";

            var zoom = 100*ActualWidth/1060;
            _prevSize = new Size(ActualWidth, ActualHeight);

            string[] commands =
            {
                "document.getElementById(\"headerSmall\").outerHTML=\'\';document.getElementById(\"menu\").outerHTML=\'\';document.getElementsByClassName(\"js-sns-icon-container icon-block-small\")[0].outerHTML=\'\';document.getElementsByTagName(\"footer\")[0].innerHTML=\'\';document.getElementsByClassName(\"mauto clearfix pt24\")[0].outerHTML=\'\';",
                "$(\"#contentWrapper\").find(\'div:first\').remove();",
                $"$(\"body\").css(\"font-family\", \"Segoe UI\").css(\"color\", \"{fontColor}\").css(\"background-color\", \"{bodyLighter}\");",
                $"$(\"td\").css(\"background-color\", \"{bodyDarker}\").css(\"border-color\", \"{bodyDarker}\");",
                $"$(\".forum_boardrow2\").css(\"background-color\", \"{bodyDarker}\");",
                $"$(\".forum_boardrow1\").css(\"background-color\", \"{bodyLighter}\").css(\"border-color\",\"{fontColorInverted}\");",
                $"$(\".forum_category\").css(\"background-color\", \"{bodyLight}\");",
                $"$(\".forum_boardrowspacer\").css(\"background-color\", \"{bodyLighter}\");",
                $"$(\".btn-forum\").css(\"background-color\", \"{bodyLight}\").css(\"border-color\",\"{fontColorInverted}\");",
                $"$(\"html\").css(\"zoom\", \"{Math.Floor(zoom)}%\");",
                "$(\".wrapper\").find(\".fl-r.ar\").remove()",
                $"$(\".inputButton\").css(\"border-color\",\"{fontColorInverted}\").css(\"background-color\",\"{bodyLight}\")",
                $"$(\"a\").css(\"color\", \"#{color.ToString().Substring(3)}\");",
                $"$(\"#content\").css(\"border-color\", \"{bodyLighter}\").css(\"background-color\",\"{bodyLighter}\");",
                $"$(\".forum_category,.forum_locheader\").css(\"color\",\"{fontColor}\");",
                $"$(\".codetext\").css(\"background-color\",\"{bodyDarker}\");",
                $"$(\".quotetext\").css(\"background-color\",\"{bodyLight}\").css(\"border-color\",\"{bodyLighter}\");",
            };
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
                await TopicWebView.InvokeScriptAsync("eval", new string[] { $"$(\"html\").css(\"zoom\", \"{Math.Floor(100 * ActualWidth / 1060)}%\");", });
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
            if(args.Uri.ToString().Contains(_baseUri.ToString()))
                return;
            if(_lastpost && args.Uri.ToString().Contains("&show="))
                return;
            try
            {
                if (args.Uri != null)
                {
                    var uri = args.Uri.AbsoluteUri;
                    args.Cancel = true;
                    if (uri.Contains("?subboard="))
                    {
                        var id = uri.Split('=').Last();
                        if (id == "1")
                            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageForumIndex, new ForumsBoardNavigationArgs(ForumBoards.AnimeSeriesDisc));
                        else if (id == "4")
                            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageForumIndex, new ForumsBoardNavigationArgs(ForumBoards.MangaSeriesDisc));
                        return;
                    }
                    if (uri.Contains("?board="))
                    {
                        ForumBoards board;
                        if (ForumBoards.TryParse(uri.Split('=').Last(), out board)) ;
                        ViewModelLocator.GeneralMain.Navigate(PageIndex.PageForumIndex, new ForumsBoardNavigationArgs(board));
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
                            new AnimeDetailsPageNavigationArgs(id, link[3], null, null)
                            {
                                AnimeMode = link[1] == "anime"
                            });
                    }
                    else if (uri.Contains("/profile/"))
                    {
                        var vm = ViewModelLocator.MalArticles;
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
            //we don't like iframes
            //args.Cancel = true; or we do?
        }

        private void TopicWebView_OnContentLoading(WebView sender, WebViewContentLoadingEventArgs args)
        {
            _navigatingRoot = false;
        }
    }
}
