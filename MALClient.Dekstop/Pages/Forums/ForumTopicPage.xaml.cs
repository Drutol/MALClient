using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.System;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MALClient.Shared.Managers;
using MALClient.Models.Enums;
using MALClient.XShared.Comm;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.Comm.MagicalRawQueries;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Forums;

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
            TopicWebView.DefaultBackgroundColor = Settings.SelectedTheme == (int)ApplicationTheme.Dark ? Color.FromArgb(0xFF, 0x2f, 0x2f, 0x2f) : Color.FromArgb(0xFF,0xe6,0xe6,0xe6);
        }


        private async void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            await MalWebViewHttpContextInitializer.InitializeContextForWebViews(false);
            ViewModel.WebViewTopicNavigationRequested += ViewTopicModelOnWebViewTopicNavigationRequested;
            ViewModel.WebViewNewTopicNavigationRequested += ViewModelOnWebViewNewTopicNavigationRequested;
            Loaded -= OnLoaded;
            ViewModel.Init(_args);
        }

        private void ViewModelOnWebViewNewTopicNavigationRequested(string content, bool b)
        {
            _baseUri = new Uri($"https://myanimelist.net/forum/?action=post&boardid={content}");
            _newTopic = true;
            _navigatingRoot = true;
            TopicWebView.Navigate(_baseUri);
        }

        private void ViewTopicModelOnWebViewTopicNavigationRequested(string content,bool arg)
        {
            _baseUri = new Uri($"https://myanimelist.net/forum/?topicid={content}{(arg ? "&goto=lastpost" : "")}");
            _lastpost = arg;
            _navigatingRoot = true;
            TopicWebView.Navigate(_baseUri);
        }
      
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _args = e.Parameter as ForumsTopicNavigationArgs;
            base.OnNavigatedTo(e);
        }

        private void TopicWebView_OnLoadCompleted(object sender, NavigationEventArgs e)
        {

        }


        private async void TopicWebView_OnDOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            var uiSettings = new UISettings();
            var color = uiSettings.GetColorValue(UIColorType.Accent);
            //this chain of commands will remove unnecessary stuff
            string bodyLight = Settings.SelectedTheme == (int)ApplicationTheme.Dark ? "#3d3d3d" : "#d0d0d0";
            string bodyLighter = Settings.SelectedTheme == (int)ApplicationTheme.Dark ? "#2f2f2f" : "#e6e6e6";
            string bodyDarker = Settings.SelectedTheme == (int)ApplicationTheme.Dark ? "#212121" : "#cacaca";
            string fontColor = Settings.SelectedTheme == (int)ApplicationTheme.Dark ? "white" : "black";
            string fontColorInverted = Settings.SelectedTheme == (int)ApplicationTheme.Dark ? "black" : "white";

            var zoom = 100*ActualWidth/1060;
            _prevSize = new Size(ActualWidth, ActualHeight);
            List<string> commands;
            if (_args.CreateNewTopic)
            {
                commands = new List<string>
                {
                    @"document.getElementById(""headerSmall"").outerHTML='';document.getElementById(""menu"").outerHTML='';document.getElementsByClassName(""js-sns-icon-container icon-block-small"")[0].outerHTML='';document.getElementsByTagName(""footer"")[0].innerHTML='';document.getElementsByClassName(""mauto clearfix pt24"")[0].outerHTML='';",
                    @"$(""#contentWrapper"").find('div:first').remove();",
                    $@"$(""#contentWrapper"").css(""background-color"", ""{bodyLighter}"");",
                    $@"$(""body"").css(""font-family"", ""Segoe UI"").css(""color"", ""{fontColor}"").css(""background-color"", ""{bodyLighter}"");",
                    @"$(""footer"").remove()",
                    @"$(""input[name='preview']:first"").remove()",
                    $@"$(""textarea"").css(""background-color"",""{bodyDarker}"").css(""color"", ""{fontColor}"")",
                    $@"$(""td"").css(""color"", ""{fontColor}"")",
                    $@"$(""a"").css(""color"", ""#{color.ToString().Substring(3)}"");",
                    $@"$(""#content"").css(""border-color"", ""{bodyLighter}"").css(""background-color"",""{bodyLighter}"");",
                    $@"$(""html"").css(""zoom"", ""{Math.Floor(zoom)}%"").css(""background-color"", ""{bodyLighter}"");",
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
                    @"document.getElementById(""headerSmall"").outerHTML='';document.getElementById(""menu"").outerHTML='';document.getElementsByClassName(""js-sns-icon-container icon-block-small"")[0].outerHTML='';document.getElementsByTagName(""footer"")[0].innerHTML='';document.getElementsByClassName(""mauto clearfix pt24"")[0].outerHTML='';",
                    @"$(""#contentWrapper"").find('div:first').remove();",                 
                    @"$(""div[id^="" +""ad-skin"" + ""]"").remove();",
                    $@"$(""#contentWrapper"").css(""background-color"", ""{bodyLighter}"");",
                    $@"$(""body"").css(""font-family"", ""Segoe UI"").css(""color"", ""{fontColor}"").css(""background-color"", ""{bodyLighter}"");",
                    $@"$(""td"").css(""background-color"", ""{bodyDarker}"").css(""border-color"", ""{bodyDarker}"");",
                    $@"$("".forum_boardrow2"").css(""background-color"", ""{bodyDarker}"");",
                    $@"$("".forum_boardrow1"").css(""background-color"", ""{bodyLighter}"").css(""border-color"",""{fontColorInverted}"");",
                    $@"$("".forum_category"").css(""background-color"", ""{bodyLight}"");",
                    $@"$("".forum_boardrowspacer"").css(""background-color"", ""{bodyLighter}"");",
                    $@"$("".btn-forum"").css(""background-color"", ""{bodyLight}"").css(""border-color"",""{fontColorInverted}"");",
                    $@"$(""html"").css(""zoom"", ""{Math.Floor(zoom)}%"");",
                    @"$("".wrapper"").find("".fl-r.ar"").remove()",
                    $@"$("".inputButton"").css(""border-color"",""{fontColorInverted}"").css(""background-color"",""{bodyLight}"")",
                    $@"$(""a"").css(""color"", ""#{color.ToString().Substring(3)}"");",
                    $@"$(""#content"").css(""border-color"", ""{bodyLighter}"").css(""background-color"",""{bodyLighter}"");",
                    $@"$("".forum_category,.forum_locheader"").css(""color"",""{fontColor}"");",
                    $@"$("".codetext"").css(""background-color"",""{bodyDarker}"");",
                    $@"$("".quotetext"").css(""background-color"",""{bodyLight}"").css(""border-color"",""{bodyLighter}"");",
                    $@"$("".vote_container"").css(""background-color"",""#{color.ToString().Substring(3)}"")",
                    $@"$(""textarea"").css(""background-color"",""{bodyDarker}"").css(""color"", ""{fontColor}"")",
                    
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
            ViewModel.LoadingTopic = false;
        }

        private Size _prevSize;
        private bool _canChangeSize = true;
        private bool _newTopic;
        private bool _skipStyling;

        private async void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!_canChangeSize || Math.Abs(e.NewSize.Width - _prevSize.Width) < 10)
                return;
            _canChangeSize = false;
            _prevSize = e.NewSize;
            try
            {
                await TopicWebView.InvokeScriptAsync("eval", new string[] { $"$(\"html\").css(\"zoom\", \"{Math.Floor(100 * ActualWidth / 1060)}%\");", });
            }
            catch (Exception)
            {
                //htm.. no it's javascript this time oh, how fun!
            }
            await Task.Delay(1000);
            _canChangeSize = true;
        }


        private async void TopicWebView_OnNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            if (_navigatingRoot)
            {
                return;
            }
            if (Regex.IsMatch(args.Uri.ToString(), @"https:\/\/myanimelist.net\/forum\/index.php\?topic_id=.*"))
                return;
            if(Regex.IsMatch(args.Uri.ToString(), @"https:\/\/myanimelist.net\/forum\/\?topicid=.*"))
                return;
            if (args.Uri.ToString().Contains("&show="))
                return;
            if (_newTopic && Regex.IsMatch(args.Uri.ToString(), @"https:\/\/myanimelist\.net\/forum\/\?action=post&boardid=.*"))
            {
                TopicWebView.NavigationCompleted += TopicWebViewOnNavigationCompleted;
                return;
            }
            try
            {
                if (args.Uri != null)
                {
                    var uri = args.Uri.AbsoluteUri;
                    args.Cancel = true;
                    var navArgs = await MalLinkParser.GetNavigationParametersForUrl(uri);
                    if (navArgs != null)
                    {
                        ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageForumIndex, _args);
                        ViewModelLocator.GeneralMain.Navigate(navArgs.Item1, navArgs.Item2);
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

        private void TopicWebViewOnNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            TopicWebView.NavigationCompleted -= TopicWebViewOnNavigationCompleted;
            ViewModelLocator.ForumsBoard.ReloadOnNextLoad();
            ViewModelLocator.NavMgr.CurrentMainViewOnBackRequested();           
        }

        private void TopicWebView_OnFrameNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            //we don't like iframes
            //args.Cancel = true; or do we?
            if (_args.CreateNewTopic)
                args.Cancel = true;
        }

        private void TopicWebView_OnContentLoading(WebView sender, WebViewContentLoadingEventArgs args)
        {
            _navigatingRoot = false;
        }
    }
}
