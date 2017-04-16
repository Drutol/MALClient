using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Activities;
using MALClient.Android.Resources;
using MALClient.Android.Web;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Forums.Items;

namespace MALClient.Android.UserControls.ForumItems
{
    public class ForumTopicItem : UserControlBase<ForumTopicMessageEntryViewModel, LinearLayout>
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

        private DataJavascriptInterface _dataJavascriptInterface;
        private readonly List<Binding> _bindings = new List<Binding>();

        #region Constructors

        public ForumTopicItem(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public ForumTopicItem(Context context) : base(context)
        {
        }

        public ForumTopicItem(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public ForumTopicItem(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        public ForumTopicItem(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }

        #endregion


        protected override int ResourceId => Resource.Layout.ForumTopicPageItem;

        protected override void BindModelFling()
        {
            if (ForumTopicPageItemAuthorImage.IntoIfLoaded(ViewModel.Data.Poster.MalUser.ImgUrl))
            {
                ForumTopicPageItemAuthorImage.Visibility = ViewStates.Visible;
            }
            else
            {
                ForumTopicPageItemAuthorImage.Visibility = ViewStates.Gone;
            }

            UpdateViewWithNewHeight(ForumTopicPageItemWebView, DimensionsHelper.DpToPx(200));
            ForumTopicPageItemWebView.Visibility = ViewStates.Invisible;
        }

        protected override void BindModelFull()
        {
            if (string.IsNullOrEmpty(ViewModel.Data.Poster.MalUser.ImgUrl))
                ForumTopicPageItemAuthorImage.Visibility = ViewStates.Gone;
            else
            {
                if (ForumTopicPageItemAuthorImage.Tag == null || (string)ForumTopicPageItemAuthorImage.Tag != ViewModel.Data.Poster.MalUser.ImgUrl)
                {
                    ForumTopicPageItemAuthorImage.Into(ViewModel.Data.Poster.MalUser.ImgUrl);
                }
                ForumTopicPageItemAuthorImage.Visibility = ViewStates.Visible;
            }
            ForumTopicPageItemWebView.Visibility = ViewStates.Visible;


            _bindings.Add(this.SetBinding(() => ViewModel.Data.HtmlContent).WhenSourceChanges(() =>
            {
                var tag = ForumTopicPageItemWebView.Tag.Unwrap<WebViewTag>();
                if (tag.ContentId == null || tag.ContentId != ViewModel.Data.Id)
                {
                    ForumTopicPageItemWebView.LoadDataWithBaseURL(null, ResourceLocator.CssManager.WrapWithCss(ViewModel.Data.HtmlContent), "text/html; charset=utf-8", "UTF-8", null);
                    tag.ContentId = ViewModel.Data.Id;
                    ForumTopicPageItemWebView.Tag = tag.Wrap();
                }
            }));

            _bindings.Add(this.SetBinding(() => ViewModel.ComputedHtmlHeight).WhenSourceChanges(() =>
            {
                if (ForumTopicPageItemWebView.Height < ViewModel.ComputedHtmlHeight)
                {
                    UpdateViewWithNewHeight(ForumTopicPageItemWebView, (int)ViewModel.ComputedHtmlHeight);
                }
                else
                {
                    UpdateViewWithNewHeight(ForumTopicPageItemWebView, DimensionsHelper.DpToPx(200));
                }
            }));
        }




        protected override void BindModelBasic()
        {
            ForumTopicPageItemPostAuthor.Text = ViewModel.Data.Poster.MalUser.Name;
            ForumTopicPageItemPostNumber.Text = ViewModel.Data.MessageNumber;
            ForumTopicPageItemPostDate.Text = ViewModel.Data.CreateDate;
            ForumTopicPageItemDetailOnline.Text = ViewModel.Data.Poster.Status;
            ForumTopicPageItemDetailsJoined.Text = ViewModel.Data.Poster.Joined;
            ForumTopicPageItemDetailsPostCount.Text = ViewModel.Data.Poster.Posts;
            if (string.IsNullOrEmpty(ViewModel.Data.EditDate))
                ForumTopicPageItemModifiedLabel.Visibility = ViewStates.Gone;
            else
            {
                ForumTopicPageItemModifiedLabel.Visibility = ViewStates.Visible;
                ForumTopicPageItemModifiedLabel.Text = ViewModel.Data.EditDate;
            }
        }

        protected override void RootContainerInit()
        {
            var jsInterface = new DataJavascriptInterface(Context);
            ForumTopicPageItemWebView.Settings.JavaScriptEnabled = true;
            ForumTopicPageItemWebView.AddJavascriptInterface(jsInterface, "android");
            ForumTopicPageItemWebView.Tag = new WebViewTag(jsInterface).Wrap();
            ForumTopicPageItemWebView.VerticalScrollBarEnabled = false;
            ForumTopicPageItemWebView.ScrollbarFadingEnabled = true;
            ForumTopicPageItemWebView.ScrollBarDefaultDelayBeforeFade = 0;
            ForumTopicPageItemWebView.SetBackgroundColor(Color.Transparent);
            _dataJavascriptInterface = jsInterface;

            _dataJavascriptInterface.NewResponse += DataJavascriptInterfaceOnNewResponse;
        }

        private void DataJavascriptInterfaceOnNewResponse(object sender, string s)
        {
            MainActivity.CurrentContext.RunOnUiThread(() =>
            {
                ViewModel.ComputedHtmlHeight = DimensionsHelper.DpToPx(int.Parse(s)) * 1.05;
            });
        }

        private void UpdateViewWithNewHeight(View view, int height)
        {
            var param = view.LayoutParameters;
            param.Height = height;
            view.LayoutParameters = param;
        }

        protected override void CleanupPreviousModel()
        {
            foreach (var binding in _bindings)
            {
                binding.Detach();
            }
            _bindings.Clear();
        }

        #region Views

        private TextView _forumTopicPageItemPostDate;
        private TextView _forumTopicPageItemPostNumber;
        private TextView _forumTopicPageItemPostAuthor;
        private ImageViewAsync _forumTopicPageItemAuthorImage;
        private TextView _forumTopicPageItemDetailOnline;
        private TextView _forumTopicPageItemDetailsJoined;
        private TextView _forumTopicPageItemDetailsPostCount;
        private WebView _forumTopicPageItemWebView;
        private TextView _forumTopicPageItemModifiedLabel;
        private TextView _forumTopicPageItemEditButton;
        private TextView _forumTopicPageItemDeleteButton;
        private TextView _forumTopicPageItemQuoteButton;

        public TextView ForumTopicPageItemPostDate => _forumTopicPageItemPostDate ?? (_forumTopicPageItemPostDate = FindViewById<TextView>(Resource.Id.ForumTopicPageItemPostDate));

        public TextView ForumTopicPageItemPostNumber => _forumTopicPageItemPostNumber ?? (_forumTopicPageItemPostNumber = FindViewById<TextView>(Resource.Id.ForumTopicPageItemPostNumber));

        public TextView ForumTopicPageItemPostAuthor => _forumTopicPageItemPostAuthor ?? (_forumTopicPageItemPostAuthor = FindViewById<TextView>(Resource.Id.ForumTopicPageItemPostAuthor));

        public ImageViewAsync ForumTopicPageItemAuthorImage => _forumTopicPageItemAuthorImage ?? (_forumTopicPageItemAuthorImage = FindViewById<ImageViewAsync>(Resource.Id.ForumTopicPageItemAuthorImage));

        public TextView ForumTopicPageItemDetailOnline => _forumTopicPageItemDetailOnline ?? (_forumTopicPageItemDetailOnline = FindViewById<TextView>(Resource.Id.ForumTopicPageItemDetailOnline));

        public TextView ForumTopicPageItemDetailsJoined => _forumTopicPageItemDetailsJoined ?? (_forumTopicPageItemDetailsJoined = FindViewById<TextView>(Resource.Id.ForumTopicPageItemDetailsJoined));

        public TextView ForumTopicPageItemDetailsPostCount => _forumTopicPageItemDetailsPostCount ?? (_forumTopicPageItemDetailsPostCount = FindViewById<TextView>(Resource.Id.ForumTopicPageItemDetailsPostCount));

        public WebView ForumTopicPageItemWebView => _forumTopicPageItemWebView ?? (_forumTopicPageItemWebView = FindViewById<WebView>(Resource.Id.ForumTopicPageItemWebView));

        public TextView ForumTopicPageItemModifiedLabel => _forumTopicPageItemModifiedLabel ?? (_forumTopicPageItemModifiedLabel = FindViewById<TextView>(Resource.Id.ForumTopicPageItemModifiedLabel));

        public TextView ForumTopicPageItemEditButton => _forumTopicPageItemEditButton ?? (_forumTopicPageItemEditButton = FindViewById<TextView>(Resource.Id.ForumTopicPageItemEditButton));

        public TextView ForumTopicPageItemDeleteButton => _forumTopicPageItemDeleteButton ?? (_forumTopicPageItemDeleteButton = FindViewById<TextView>(Resource.Id.ForumTopicPageItemDeleteButton));

        public TextView ForumTopicPageItemQuoteButton => _forumTopicPageItemQuoteButton ?? (_forumTopicPageItemQuoteButton = FindViewById<TextView>(Resource.Id.ForumTopicPageItemQuoteButton));

        #endregion
    }
}