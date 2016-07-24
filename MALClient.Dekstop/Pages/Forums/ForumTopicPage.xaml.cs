using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using MalClient.Shared.Comm.MagicalRawQueries;
using MalClient.Shared.NavArgs;
using MalClient.Shared.Utils;
using MalClient.Shared.ViewModels;
using MalClient.Shared.ViewModels.Forums;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages.Forums
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ForumTopicPage : Page
    {
        private ForumsTopicNavigationArgs _args;

        public ForumTopicViewModel ViewModel => ViewModelLocator.ForumsTopic;

        public ForumTopicPage()
        {
            this.InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            ViewModel.WebViewNavigationRequested += ViewModelOnWebViewNavigationRequested;
            ViewModel.Init(_args);
        }

        private async void ViewModelOnWebViewNavigationRequested(object content)
        {
            await MalHttpContextProvider.InitializeContextForWebViews();
            //TopicWebView.Navigate(new Uri("http://myanimelist.net/forum/?topicid=1499207"));
            TopicWebView.Navigate(new Uri($"http://myanimelist.net/forum/?topicid={content as string}"));
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
                 "$(\"html\").css(\"zoom\", \"150%\");",
                 "$(\".wrapper\").find(\".fl-r.ar\").remove()",
                 $"$(\".inputButton\").css(\"border-color\",\"{fontColorInverted}\").css(\"background-color\",\"{bodyLight}\")",
                 $"$(\"a\").css(\"color\", \"#{color.ToString().Substring(3)}\");",
                 $"$(\"#content\").css(\"border-color\", \"{bodyLighter}\").css(\"background-color\",\"{bodyLighter}\");",
                 $"$(\".forum_category,.forum_locheader\").css(\"color\",\"{fontColor}\");",
                 $"$(\".quotetext\").css(\"background-color\",\"{bodyLight}\").css(\"border-color\",\"{bodyLighter}\");",
            };
            foreach (var command in commands)
            {
                try
                {
                    await TopicWebView.InvokeScriptAsync("eval", new string[] { command });
                }
                catch (Exception)
                {
                    //htm.. no it's javascript this time oh, how fun!
                }

            }
            
        }


        private void TopicWebView_OnNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            
        }

        private void TopicWebView_OnFrameNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            //we don't like iframes
            args.Cancel = true;
        }
    }
}
