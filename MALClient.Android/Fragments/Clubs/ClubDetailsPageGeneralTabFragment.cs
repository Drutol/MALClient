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
using Android.Support.V7.View.Menu;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Listeners;
using MALClient.Android.Resources;
using MALClient.Android.Web;
using MALClient.Models.Enums;
using MALClient.Models.Models;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Clubs;

namespace MALClient.Android.Fragments.Clubs
{
    public class ClubDetailsPageGeneralTabFragment : MalFragmentBase
    {
        private ClubDetailsViewModel ViewModel = ViewModelLocator.ClubDetails;

        protected override void Init(Bundle savedInstanceState)
        {
            
        }

        protected override void InitBindings()
        {
            var client = new ListenableWebClient { NavigationInterceptOpportunity = NavigationInterceptOpportunity };
            WebView.Settings.JavaScriptEnabled = true;
            WebView.Settings.SetSupportZoom(true);
            WebView.Settings.DisplayZoomControls = false;
            WebView.SetWebViewClient(client);
            WebView.Settings.SetLayoutAlgorithm(WebSettings.LayoutAlgorithm.SingleColumn);
            WebView.SetBackgroundColor(new Color(ResourceExtension.BrushAnimeItemInnerBackground));

            WebView.Post(() =>
            {
                var scale = (100 - 715 * 100 / RootView.Width) + 100;
                WebView.SetInitialScale(scale);
                //WebView.SetPadding(DimensionsHelper.DpToPx(15),0,0,0);
            });

            Bindings.Add(this.SetBinding(() => ViewModel.Details).WhenSourceChanges(() =>
            {
                if (ViewModel.Details == null)
                    return;

                Image.Into(ViewModel.Details.ImgUrl);
                Title.Text = ViewModel.Details.Name;

                StatsList.SetAdapter(ViewModel.Details.GeneralInfo.GetAdapter(GetTemplateDelegate));
                OfficersList.SetAdapter(ViewModel.Details.Officers.GetAdapter(GetOfficerTemplateDelegate));

                if (ViewModel.Details.Joined)
                {
                    ButtonLeave.Visibility = ViewStates.Visible;
                    ButtonLeave.Text = "Leave";
                    ButtonLeave.SetOnClickListener(
                        new OnClickListener(view => ViewModel.LeaveClubCommand.Execute(null)));
                }
                else if (ViewModel.Details.IsPublic)
                {
                    ButtonLeave.SetOnClickListener(
                        new OnClickListener(view => ViewModel.JoinClubCommand.Execute(null)));
                    ButtonLeave.Visibility = ViewStates.Visible;
                    ButtonLeave.Text = "Join";
                }
                else
                {
                    ButtonLeave.Visibility = ViewStates.Gone;
                }

                ButtonForum.SetOnClickListener(
                    new OnClickListener(view => ViewModel.NavigateForumCommand.Execute(null)));
                WebView.LoadDataWithBaseURL(null,
                    ResourceLocator.CssManager.WrapWithCss(ViewModel.Details.DescriptionHtml, false, 780),
                    "text/html; charset=utf-8", "UTF-8", null);

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

       


        private View GetTemplateDelegate(int i, (string name, string value) valueTuple, View arg3)
        {
            var view = Activity.LayoutInflater.Inflate(Resource.Layout.HeaderedTextBlock, null);

            view.FindViewById<TextView>(Resource.Id.Header).Text = valueTuple.name;
            view.FindViewById<TextView>(Resource.Id.Text).Text = valueTuple.value;

            return view;
        }

        private View GetOfficerTemplateDelegate(int i, (string name, string value) valueTuple, View arg3)
        {
            var view = Activity.LayoutInflater.Inflate(Resource.Layout.HeaderedTextBlock, null);

            view.FindViewById<TextView>(Resource.Id.Header).Text = valueTuple.value;
            view.FindViewById<TextView>(Resource.Id.Text).Text = valueTuple.name;

            return view;
        }

        public override int LayoutResourceId => Resource.Layout.ClubDetailsPageGeneralTab;


        #region Views

        private ImageViewAsync _image;
        private TextView _title;
        private LinearLayout _statsList;
        private LinearLayout _officersList;
        private Button _buttonForum;
        private Button _buttonLeave;
        private WebView _webView;

        public ImageViewAsync Image => _image ?? (_image = FindViewById<ImageViewAsync>(Resource.Id.Image));

        public TextView Title => _title ?? (_title = FindViewById<TextView>(Resource.Id.Title));

        public LinearLayout StatsList => _statsList ?? (_statsList = FindViewById<LinearLayout>(Resource.Id.StatsList));

        public LinearLayout OfficersList => _officersList ?? (_officersList = FindViewById<LinearLayout>(Resource.Id.OfficersList));

        public Button ButtonForum => _buttonForum ?? (_buttonForum = FindViewById<Button>(Resource.Id.ButtonForum));

        public Button ButtonLeave => _buttonLeave ?? (_buttonLeave = FindViewById<Button>(Resource.Id.ButtonLeave));

        public WebView WebView => _webView ?? (_webView = FindViewById<WebView>(Resource.Id.WebView));

        #endregion
    }
}