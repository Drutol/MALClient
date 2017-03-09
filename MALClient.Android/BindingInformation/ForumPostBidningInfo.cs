using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Activities;
using MALClient.Android.Web;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Forums.Items;

namespace MALClient.Android.BindingInformation
{
    public class ForumPostBidningInfo : BindingInfo<ForumTopicMessageEntryViewModel>
    {
        private class WebViewTag
        {

            public DataJavascriptInterface DataJavascriptInterface { get; }
            public string ContentId { get; set; }

            public WebViewTag(DataJavascriptInterface dataJavascriptInterface)
            {
                DataJavascriptInterface = dataJavascriptInterface;
            }

        }

        public ForumPostBidningInfo(View container, ForumTopicMessageEntryViewModel viewModel, bool fling) : base(container, viewModel, fling)
        {
            PrepareContainer();
        }

        protected override void InitBindings()
        {
           if(Fling)
                return;

            Bindings.Add(this.SetBinding(() => ViewModel.Data.HtmlContent).WhenSourceChanges(() =>
            {
                var webView = Container.FindViewById<WebView>(Resource.Id.ForumTopicPageItemWebView);
                var tag = webView.Tag.Unwrap<WebViewTag>();
                if (tag.ContentId == null || tag.ContentId != ViewModel.Data.Id)
                {
                    webView.LoadDataWithBaseURL(null, ResourceLocator.CssManager.WrapWithCss(ViewModel.Data.HtmlContent), "text/html; charset=utf-8", "UTF-8", null);
                    tag.ContentId = ViewModel.Data.Id;
                    webView.Tag = tag.Wrap();
                }
            }));

            Bindings.Add(this.SetBinding(() => ViewModel.ComputedHtmlHeight).WhenSourceChanges(() =>
            {
                var webView = Container.FindViewById(Resource.Id.ForumTopicPageItemWebView);
                if (webView.Height < ViewModel.ComputedHtmlHeight)
                {
                    UpdateViewWithNewHeight(webView, (int)ViewModel.ComputedHtmlHeight);
                }
                else
                {
                    UpdateViewWithNewHeight(webView, DimensionsHelper.DpToPx(200));
                }
            }));
        }

        private DataJavascriptInterface _dataJavascriptInterface;

        protected override void InitOneTimeBindings()
        {
            Container.FindViewById<TextView>(Resource.Id.ForumTopicPageItemPostAuthor).Text =
                ViewModel.Data.Poster.MalUser.Name;
            Container.FindViewById<TextView>(Resource.Id.ForumTopicPageItemPostNumber).Text =
                ViewModel.Data.MessageNumber;
            Container.FindViewById<TextView>(Resource.Id.ForumTopicPageItemPostDate).Text =
                ViewModel.Data.CreateDate;
            Container.FindViewById<TextView>(Resource.Id.ForumTopicPageItemDetailOnline).Text =
                ViewModel.Data.Poster.Status;
            Container.FindViewById<TextView>(Resource.Id.ForumTopicPageItemDetailsJoined).Text =
                ViewModel.Data.Poster.Joined;
            Container.FindViewById<TextView>(Resource.Id.ForumTopicPageItemDetailsPostCount).Text =
                ViewModel.Data.Poster.Posts;
            var modifiedLabel = Container.FindViewById<TextView>(Resource.Id.ForumTopicPageItemModifiedLabel);
            if (string.IsNullOrEmpty(ViewModel.Data.EditDate))
                modifiedLabel.Visibility = ViewStates.Gone;
            else
            {
                modifiedLabel.Visibility = ViewStates.Visible;
                modifiedLabel.Text = ViewModel.Data.EditDate;
            }
            

            var webView = Container.FindViewById<WebView>(Resource.Id.ForumTopicPageItemWebView);
            if (!Fling)
            {
                if (webView.Tag == null)
                {
                    var jsInterface = new DataJavascriptInterface(Container.Context);
                    webView.Settings.JavaScriptEnabled = true;
                    webView.AddJavascriptInterface(jsInterface, "android");
                    webView.Tag = new WebViewTag(jsInterface).Wrap();
                    webView.VerticalScrollBarEnabled = false;
                    webView.ScrollbarFadingEnabled = true;
                    webView.ScrollBarDefaultDelayBeforeFade = 0;
                    _dataJavascriptInterface = jsInterface;
                }
                else
                    _dataJavascriptInterface = webView.Tag.Unwrap<WebViewTag>().DataJavascriptInterface;

                _dataJavascriptInterface.NewResponse += DataJavascriptInterfaceOnNewResponse;
                webView.Visibility = ViewStates.Visible;

                var img = Container.FindViewById<ImageViewAsync>(Resource.Id.ForumTopicPageItemAuthorImage);

                if (string.IsNullOrEmpty(ViewModel.Data.Poster.MalUser.ImgUrl))
                    img.Visibility = ViewStates.Gone;
                else
                {
                    img.Visibility = ViewStates.Visible;

                    if (img.Tag == null || (string) img.Tag != ViewModel.Data.Poster.MalUser.ImgUrl)
                    {
                        img.Into(ViewModel.Data.Poster.MalUser.ImgUrl);
                        img.Tag = ViewModel.Data.Poster.MalUser.ImgUrl;
                    }
                }
            }
            else if(Fling)
            {
                Container.FindViewById<ImageViewAsync>(Resource.Id.ForumTopicPageItemAuthorImage).Visibility = ViewStates.Gone;
                UpdateViewWithNewHeight(webView, DimensionsHelper.DpToPx(200));
                webView.Visibility = ViewStates.Invisible;
            }

        }

        private void UpdateViewWithNewHeight(View view,int height)
        {
            var param = view.LayoutParameters;
            param.Height = height;
            view.LayoutParameters = param;
        }

        private void DataJavascriptInterfaceOnNewResponse(object sender, string s)
        {
            MainActivity.CurrentContext.RunOnUiThread(() =>
            {
                ViewModel.ComputedHtmlHeight = DimensionsHelper.DpToPx(int.Parse(s))*1.05;
            });

        }

        protected override void DetachInnerBindings()
        {
            if(_dataJavascriptInterface != null)
            _dataJavascriptInterface.NewResponse -= DataJavascriptInterfaceOnNewResponse;
            _dataJavascriptInterface = null;
        }
    }
}