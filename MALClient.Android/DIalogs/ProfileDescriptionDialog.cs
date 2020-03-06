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
using Android.Webkit;
using Android.Widget;
using Com.Orhanobut.Dialogplus;
using GalaSoft.MvvmLight.Command;
using MALClient.Android.Listeners.DialogListeners;
using MALClient.Android.Resources;
using MALClient.Android.Web;
using MALClient.Models.Enums;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.Dialogs
{
    public class ProfileDescriptionDialog
    {
        #region Singleton

        private static ProfileDescriptionDialog _instance;
        private DialogPlus _dialog;

        private ProfileDescriptionDialog()
        {

        }

        public static ProfileDescriptionDialog Instance =>
            _instance ?? (_instance = new ProfileDescriptionDialog());

        #endregion

        public void ShowDialog(Context context,View rootView,string html)
        {
            var dialogBuilder = DialogPlus.NewDialog(context);
            dialogBuilder.SetGravity((int)GravityFlags.Center);
            dialogBuilder.SetContentHolder(new ViewHolder(Resource.Layout.ProfileDescriptionDialog));
            dialogBuilder.SetContentBackgroundResource(global::Android.Resource.Color.Transparent);
            dialogBuilder.SetOnDismissListener(
                new DialogDismissedListener(() => ViewModelLocator.NavMgr.ResetOneTimeOverride()));

            dialogBuilder.SetMargin(0, 0, 0, 0);
            _dialog = dialogBuilder.Create();

            var dialogView = _dialog.HolderView;
            dialogView.LayoutParameters.Height = rootView.Height - DimensionsHelper.DpToPx(16*2);
            dialogView.SetMargins(0,16,0,0);
            var webView = dialogView.FindViewById<WebView>(Resource.Id.WebView);
            var client = new ListenableWebClient { NavigationInterceptOpportunity = NavigationInterceptOpportunity };
            webView.Settings.JavaScriptEnabled = true;
            webView.Settings.SetSupportZoom(true);
            webView.Settings.DisplayZoomControls = false;
            webView.SetWebViewClient(client);
            webView.Settings.BuiltInZoomControls = true;
            webView.Settings.SetLayoutAlgorithm(WebSettings.LayoutAlgorithm.SingleColumn);
            webView.SetBackgroundColor(new Color(ResourceExtension.BrushAnimeItemInnerBackground));
            
            webView.Post(() =>
            {
                var scale = (100 - 735 * 100 / (dialogView.Width == 0 ? 300 : dialogView.Width) ) + 100;
                webView.SetInitialScale(scale);
                //WebView.SetPadding(DimensionsHelper.DpToPx(15),0,0,0);
            });

            webView.LoadDataWithBaseURL(null,
                ResourceLocator.CssManager.WrapWithCss(html, false, 800),
                "text/html; charset=utf-8", "UTF-8", null);
            ViewModelLocator.NavMgr.RegisterOneTimeMainOverride(new RelayCommand(() => _dialog.Dismiss()));
            _dialog.Show();
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
                        ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageProfile, ViewModelLocator.ProfilePage.PrevArgs);
                    }
                    else
                    {
                        var arg = navArgs.Item2 as AnimeDetailsPageNavigationArgs;
                        arg.Source = PageIndex.PageClubDetails;
                        arg.PrevPageSetup = ViewModelLocator.ProfilePage.PrevArgs;
                    }


                    ViewModelLocator.GeneralMain.Navigate(navArgs.Item1, navArgs.Item2);
                }
                else
                {
                    ResourceLocator.SystemControlsLauncherService.LaunchUri(new Uri(targetUrl));
                }
            }
            return null;
        }
    }
}