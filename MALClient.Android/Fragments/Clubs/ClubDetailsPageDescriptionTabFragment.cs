using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Web;
using MALClient.Models.Enums;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Clubs;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Fragments.Clubs
{
    public class ClubDetailsPageDescriptionTabFragment : MalFragmentBase
    {
        private ClubDetailsViewModel ViewModel = ViewModelLocator.ClubDetails;

        protected override void Init(Bundle savedInstanceState)
        {
            
        }

        protected override void InitBindings()
        {
            var client = new ListenableWebClient();
            client.NavigationInterceptOpportunity = NavigationInterceptOpportunity;
            WebView.SetWebViewClient(client);
            WebView.Settings.JavaScriptEnabled = true;

            Bindings.Add(this.SetBinding(() => ViewModel.Details).WhenSourceChanges(() =>
            {
                if(ViewModel.Details != null)
                    WebView.LoadDataWithBaseURL(null, ResourceLocator.CssManager.WrapWithCss(ViewModel.Details.DescriptionHtml,false,780), "text/html; charset=utf-8", "UTF-8",null);
            }));
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
                        ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageClubDetails, ViewModel.LastArgs);
                    }
                    else
                    {
                        var arg = navArgs.Item2 as AnimeDetailsPageNavigationArgs;
                        arg.Source = PageIndex.PageClubDetails;
                        arg.PrevPageSetup = ViewModel.LastArgs;
                    }


                    ViewModelLocator.GeneralMain.Navigate(navArgs.Item1, navArgs.Item2);
                }
            }
            return null;
        }

        public override int LayoutResourceId => Resource.Layout.ClubDetailsPageDescriptionTab;

        #region Views

        private WebView _webView;

        public WebView WebView => _webView ?? (_webView = FindViewById<WebView>(Resource.Id.WebView));

        #endregion
    }
}