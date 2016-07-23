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
            var filter = new HttpBaseProtocolFilter();
            var httpContext = await MalHttpContextProvider.GetHttpContextAsync();
            var cookies = httpContext.Handler.CookieContainer.GetCookies(new Uri("http://myanimelist.net/"));
            foreach (var cookie in cookies.Cast<Cookie>())
            {
                var newCookie = new HttpCookie(cookie.Name, cookie.Domain, cookie.Path);
                newCookie.Value = cookie.Value;
                filter.CookieManager.SetCookie(newCookie);
            }

            filter.AllowAutoRedirect = true;
            
            HttpClient client = new HttpClient(filter);

            TopicWebView.Navigate(new Uri("http://myanimelist.net/forum/?topicid=1499207"));
        }

        

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _args = e.Parameter as ForumsTopicNavigationArgs;
            base.OnNavigatedTo(e);
        }

        private async void TopicWebView_OnLoadCompleted(object sender, NavigationEventArgs e)
        {
            TopicWebView.AllowedScriptNotifyUris.Add(new Uri("http://myanimelist.net"));
            string[] args = { "document.getElementById(\"headerSmall\").outerHTML=\'\';document.getElementById(\"menu\").outerHTML=\'\';document.getElementsByClassName(\"js-sns-icon-container icon-block-small\")[0].outerHTML=\'\';document.getElementsByTagName(\"footer\")[0].innerHTML=\'\';document.getElementsByClassName(\"mauto clearfix pt24\")[0].outerHTML=\'\';" };
            await TopicWebView.InvokeScriptAsync("eval", args);
        }


        private async void TopicWebView_OnDOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            
        }


        private void TopicWebView_OnNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            
        }
    }
}
