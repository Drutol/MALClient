using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
using MALClient.Android.DIalogs;
using MALClient.Android.Listeners;
using MALClient.Android.Resources;
using MALClient.Android.Web;
using MALClient.Models.Enums;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Forums.Items;

namespace MALClient.Android.UserControls.ForumItems
{
    public class ForumTopicItem : UserControlBase<ForumTopicMessageEntryViewModel, LinearLayout>
    {
        private class WebViewTag
        {

            public DataJavascriptInterface DataJavascriptInterface { get; }
            public int ContentHash { get; set; }

            public WebViewTag(DataJavascriptInterface dataJavascriptInterface)
            {
                DataJavascriptInterface = dataJavascriptInterface;
            }

        }

        private ListenableWebClient _client;
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

            if (double.IsNaN(ViewModel.ComputedHtmlHeight))
            {
                UpdateViewWithNewHeight(ForumTopicPageItemWebView, DimensionsHelper.DpToPx(200));
                ForumTopicPageItemWebView.Visibility = ViewStates.Invisible;
            }
            else
            {
                var tag = ForumTopicPageItemWebView.Tag.Unwrap<WebViewTag>();
                if (tag.ContentHash == 0 || tag.ContentHash != ViewModel.Data.HtmlContent.GetHashCode())
                {
                    ForumTopicPageItemWebView.LoadDataWithBaseURL(null, ResourceLocator.CssManager.WrapWithCss(ViewModel.Data.HtmlContent), "text/html; charset=utf-8", "UTF-8", null);
                    tag.ContentHash = ViewModel.Data.HtmlContent.GetHashCode();
                    ForumTopicPageItemWebView.Tag = tag.Wrap();
                }
            }

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


            _bindings.Add(this.SetBinding(() => ViewModel.Data).WhenSourceChanges(() =>
            {
                var tag = ForumTopicPageItemWebView.Tag.Unwrap<WebViewTag>();
                if (tag.ContentHash == 0 || tag.ContentHash != ViewModel.Data.HtmlContent.GetHashCode())
                {
                    ForumTopicPageItemWebView.LoadDataWithBaseURL(null, ResourceLocator.CssManager.WrapWithCss(ViewModel.Data.HtmlContent), "text/html; charset=utf-8", "UTF-8", null);
                    tag.ContentHash = ViewModel.Data.HtmlContent.GetHashCode();
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

            _bindings.Add(this.SetBinding(() => ViewModel.Loading).WhenSourceChanges(() =>
            {
                if (ViewModel.Loading)
                {
                    ForumTopicPageItemLoadingOverlay.Visibility = ViewStates.Gone;
                }
                else
                {
                    ForumTopicPageItemLoadingOverlay.Visibility = ViewStates.Gone;
                }
            }));

            
        }




        protected override void BindModelBasic()
        {
            ForumTopicPageItemPostAuthor.Text = ViewModel.Data.Poster.MalUser.Name;
            ForumTopicPageItemPostNumber.Text = ViewModel.Data.MessageNumber;
            ForumTopicPageItemPostDate.Text = ViewModel.Data.CreateDate;
           // if (string.IsNullOrEmpty(ViewModel.Data.Poster.Status))
           //     ForumTopicPageItemDetailOnline.Visibility = ViewStates.Gone;
            //else
           // {
            //    ForumTopicPageItemDetailOnline.Visibility = ViewStates.Visible;
            //    ForumTopicPageItemDetailOnline.Text = ViewModel.Data.Poster.Status;
            //}
            if (string.IsNullOrEmpty(ViewModel.Data.Poster.Joined))
                ForumTopicPageItemDetailsJoined.Visibility = ViewStates.Gone;
            else
            {
                ForumTopicPageItemDetailsJoined.Visibility = ViewStates.Visible;
                ForumTopicPageItemDetailsJoined.Text = ViewModel.Data.Poster.Joined;
            }
            if (string.IsNullOrEmpty(ViewModel.Data.Poster.Posts))
                ForumTopicPageItemDetailsPostCount.Visibility = ViewStates.Gone;
            else
            {
                ForumTopicPageItemDetailsPostCount.Visibility = ViewStates.Visible;
                ForumTopicPageItemDetailsPostCount.Text = ViewModel.Data.Poster.Posts;
            }

            ForumTopicPageItemEditButton.Visibility = ViewModel.Data.CanEdit ? ViewStates.Visible : ViewStates.Gone;
            ForumTopicPageItemDeleteButton.Visibility = ViewModel.Data.CanDelete ? ViewStates.Visible : ViewStates.Gone;
            ForumTopicPageItemSendMessageButton.Visibility = ViewModel.MessagingVisible ? ViewStates.Visible : ViewStates.Gone;

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
            _client = new ListenableWebClient();
            _client.NavigationInterceptOpportunity = NavigationInterceptOpportunity;
            ForumTopicPageItemWebView.SetWebViewClient(_client);
            ForumTopicPageItemWebView.AddJavascriptInterface(jsInterface, "android");
            ForumTopicPageItemWebView.Tag = new WebViewTag(jsInterface).Wrap();
            ForumTopicPageItemWebView.VerticalScrollBarEnabled = false;
            ForumTopicPageItemWebView.ScrollbarFadingEnabled = true;
            ForumTopicPageItemWebView.HorizontalScrollBarEnabled = false;
            ForumTopicPageItemWebView.ScrollBarDefaultDelayBeforeFade = 0;
            ForumTopicPageItemWebView.SetBackgroundColor(Color.Transparent);
            _dataJavascriptInterface = jsInterface;

            _dataJavascriptInterface.NewResponse += DataJavascriptInterfaceOnNewResponse;

            ForumTopicPageItemEditButton.SetOnClickListener(new OnClickListener(async view =>
            {
                await ViewModel.StartEdit();
                var str = await TextInputDialogBuilder.BuildForumPostTextInputDialog(Context, TextInputDialogBuilder.ForumPostTextInputContext.Edit, ViewModel.BBcodeContent);
                if (!string.IsNullOrEmpty(str))
                {
                    ViewModel.BBcodeContent = str;
                    ViewModel.SubmitEditCommand.Execute(null);
                }
            }));

            ForumTopicPageItemQuoteButton.SetOnClickListener(new OnClickListener(async view =>
            {
                var semaphore = new SemaphoreSlim(0);
                var vm = ViewModelLocator.ForumsTopic;
                var deleg = new PropertyChangedEventHandler((sender, args) =>
                {
                    if (args.PropertyName == nameof(vm.ReplyMessage))
                        semaphore.Release();
                });
                vm.ReplyMessage = string.Empty;
                vm.PropertyChanged += deleg;
                ViewModel.QuoteCommand.Execute(null);
                await semaphore.WaitAsync();
                vm.PropertyChanged -= deleg;
                var str = await TextInputDialogBuilder.BuildForumPostTextInputDialog(Context, TextInputDialogBuilder.ForumPostTextInputContext.Reply, vm.ReplyMessage);
                if (!string.IsNullOrEmpty(str))
                {
                    vm.ReplyMessage = str;
                    vm.CreateReplyCommand.Execute(null);
                }
            }));

            ForumTopicPageItemDeleteButton.SetOnClickListener(new OnClickListener(view =>
            {
                ViewModel.DeleteCommand.Execute(null);
            }));

            ForumTopicPageItemSendMessageButton.SetOnClickListener(new OnClickListener(view =>
            {
                ViewModelLocator.ForumsTopic.NavigateMessagingCommand.Execute(ViewModel.Data.Poster.MalUser);
            }));

            ForumTopicPageItemSeeOtherPostsButton.SetOnClickListener(new OnClickListener(view =>
            {
                ViewModel.GoToPostersOtherPosts.Execute(null);
            }));
        }

        private async Task<string> NavigationInterceptOpportunity(string targetUrl)
        {
            try
            {
                if (targetUrl != null)
                {
                    var navArgs = MalLinkParser.GetNavigationParametersForUrl(targetUrl);
                    if (navArgs != null)
                    {


                        if (navArgs.Item1 != PageIndex.PageAnimeDetails)
                        {
                            ViewModelLocator.ForumsTopic.RegisterSelfBackNav();
                        }
                        else
                        {
                            var arg = navArgs.Item2 as AnimeDetailsPageNavigationArgs;
                            arg.Source = PageIndex.PageForumIndex;
                            arg.PrevPageSetup = ViewModelLocator.ForumsTopic.GetSelfBackNavArgs();
                        }

                        ViewModelLocator.GeneralMain.Navigate(navArgs.Item1, navArgs.Item2);
                    }
                    else if (Settings.ArticlesLaunchExternalLinks)
                    {
                        ResourceLocator.SystemControlsLauncherService.LaunchUri(new Uri(targetUrl));
                    }
                }
            }
            catch (Exception)
            {
            }
            return null;
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
        //private TextView _forumTopicPageItemDetailOnline;
        private TextView _forumTopicPageItemDetailsJoined;
        private TextView _forumTopicPageItemDetailsPostCount;
        private ImageButton _forumTopicPageItemSendMessageButton;
        private ImageButton _forumTopicPageItemSeeOtherPostsButton;
        private WebView _forumTopicPageItemWebView;
        private FrameLayout _forumTopicPageItemLoadingOverlay;
        private TextView _forumTopicPageItemModifiedLabel;
        private FrameLayout _forumTopicPageItemEditButton;
        private FrameLayout _forumTopicPageItemDeleteButton;
        private FrameLayout _forumTopicPageItemQuoteButton;

        public TextView ForumTopicPageItemPostDate => _forumTopicPageItemPostDate ?? (_forumTopicPageItemPostDate = FindViewById<TextView>(Resource.Id.ForumTopicPageItemPostDate));

        public TextView ForumTopicPageItemPostNumber => _forumTopicPageItemPostNumber ?? (_forumTopicPageItemPostNumber = FindViewById<TextView>(Resource.Id.ForumTopicPageItemPostNumber));

        public TextView ForumTopicPageItemPostAuthor => _forumTopicPageItemPostAuthor ?? (_forumTopicPageItemPostAuthor = FindViewById<TextView>(Resource.Id.ForumTopicPageItemPostAuthor));

        public ImageViewAsync ForumTopicPageItemAuthorImage => _forumTopicPageItemAuthorImage ?? (_forumTopicPageItemAuthorImage = FindViewById<ImageViewAsync>(Resource.Id.ForumTopicPageItemAuthorImage));

        //public TextView ForumTopicPageItemDetailOnline => _forumTopicPageItemDetailOnline ?? (_forumTopicPageItemDetailOnline = FindViewById<TextView>(Resource.Id.ForumTopicPageItemDetailOnline));

        public TextView ForumTopicPageItemDetailsJoined => _forumTopicPageItemDetailsJoined ?? (_forumTopicPageItemDetailsJoined = FindViewById<TextView>(Resource.Id.ForumTopicPageItemDetailsJoined));

        public TextView ForumTopicPageItemDetailsPostCount => _forumTopicPageItemDetailsPostCount ?? (_forumTopicPageItemDetailsPostCount = FindViewById<TextView>(Resource.Id.ForumTopicPageItemDetailsPostCount));

        public ImageButton ForumTopicPageItemSendMessageButton => _forumTopicPageItemSendMessageButton ?? (_forumTopicPageItemSendMessageButton = FindViewById<ImageButton>(Resource.Id.ForumTopicPageItemSendMessageButton));

        public ImageButton ForumTopicPageItemSeeOtherPostsButton => _forumTopicPageItemSeeOtherPostsButton ?? (_forumTopicPageItemSeeOtherPostsButton = FindViewById<ImageButton>(Resource.Id.ForumTopicPageItemSeeOtherPostsButton));

        public WebView ForumTopicPageItemWebView => _forumTopicPageItemWebView ?? (_forumTopicPageItemWebView = FindViewById<WebView>(Resource.Id.ForumTopicPageItemWebView));

        public FrameLayout ForumTopicPageItemLoadingOverlay => _forumTopicPageItemLoadingOverlay ?? (_forumTopicPageItemLoadingOverlay = FindViewById<FrameLayout>(Resource.Id.ForumTopicPageItemLoadingOverlay));

        public TextView ForumTopicPageItemModifiedLabel => _forumTopicPageItemModifiedLabel ?? (_forumTopicPageItemModifiedLabel = FindViewById<TextView>(Resource.Id.ForumTopicPageItemModifiedLabel));

        public FrameLayout ForumTopicPageItemEditButton => _forumTopicPageItemEditButton ?? (_forumTopicPageItemEditButton = FindViewById<FrameLayout>(Resource.Id.ForumTopicPageItemEditButton));

        public FrameLayout ForumTopicPageItemDeleteButton => _forumTopicPageItemDeleteButton ?? (_forumTopicPageItemDeleteButton = FindViewById<FrameLayout>(Resource.Id.ForumTopicPageItemDeleteButton));

        public FrameLayout ForumTopicPageItemQuoteButton => _forumTopicPageItemQuoteButton ?? (_forumTopicPageItemQuoteButton = FindViewById<FrameLayout>(Resource.Id.ForumTopicPageItemQuoteButton));


        #endregion
    }
}