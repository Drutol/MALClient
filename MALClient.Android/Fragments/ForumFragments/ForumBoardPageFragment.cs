using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Com.Shehabic.Droppy;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.BindingConverters;
using MALClient.Android.Dialogs;
using MALClient.Android.DIalogs;
using MALClient.Android.Flyouts;
using MALClient.Android.Listeners;
using MALClient.Android.Resources;
using MALClient.Models.Enums;
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Forums;
using MALClient.XShared.ViewModels.Forums.Items;

namespace MALClient.Android.Fragments.ForumFragments
{
    public class ForumBoardPageFragment : MalFragmentBase
    {
        private ForumBoardViewModel ViewModel;
        private readonly ForumsBoardNavigationArgs _args;
        private View _prevHighlightedPageIndicator;
        private DroppyMenuPopup _menu;

        public ForumBoardPageFragment(ForumsBoardNavigationArgs args)
        {
            _args = args;
        }

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = ViewModelLocator.ForumsBoard;
            ViewModel.Init(_args);
        }

        protected override void InitBindings()
        {
            ForumBoardPageIcon.Typeface = FontManager.GetTypeface(Activity,FontManager.TypefacePath);

            Bindings.Add(
                this.SetBinding(() => ViewModel.LoadingTopics,
                    () => ForumBoardPageLoadingSpinner.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));

            Bindings.Add(
                this.SetBinding(() => ViewModel.Title,
                    () => ForumBoardPageTitle.Text));

            Bindings.Add(this.SetBinding(() => ViewModel.Icon).WhenSourceChanges(() =>
            {
                ForumBoardPageIcon.SetText(DummyFontAwesomeToRealFontAwesomeConverter.Convert(ViewModel.Icon));
            }));

            Bindings.Add(
                this.SetBinding(() => ViewModel.SearchButtonVisibility,
                    () => ForumBoardPageSearchButton.Visibility)
                    .ConvertSourceToTarget(Converters.BoolToVisibility));

            Bindings.Add(
                this.SetBinding(() => ViewModel.NewTopicButtonVisibility,
                    () => ForumBoardPageActionButton.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));
            //ForumBoardPageActionButton.Click += (sender, args) => ViewModel.CreateNewTopicCommand.Execute(null);


            Bindings.Add(this.SetBinding(() => ViewModel.Topics).WhenSourceChanges(() =>
            {
                ForumBoardPagePostsList.Adapter = ViewModel.Topics.GetAdapter(GetTopicTemplateDelegate);
            }));


            ForumBoardPageSearchButton.Click += async (sender, args) =>
            {
                var str = await TextInputDialogBuilder.BuildInputTextDialog(Context, "Search", "keyword...");
                if (!string.IsNullOrEmpty(str))
                {
                    ViewModel.SearchQuery = str;
                    ViewModel.SearchCommand.Execute(null);
                }
            };

            ForumBoardPageGotoPageButton.SetOnClickListener(new OnClickListener(OnGotoPageButtonClick));

            Bindings.Add(this.SetBinding(() => ViewModel.AvailablePages).WhenSourceChanges(UpdatePageSelection));
        }

        private async void OnGotoPageButtonClick(View view)
        {
            var result = await ForumDialogBuilder.BuildGoPageDialog(Context);
            if (result == null)
                return;

            if (result == -1)
            {
                ViewModel.GotoFirstPageCommand.Execute(null);
            }
            else if (result == -2)
            {
                ViewModel.GotoLastPageCommand.Execute(null);
            }
            else
            {
                if(result == 0)
                    ViewModel.GotoPageTextBind = result.ToString();
                else
                    ViewModel.GotoPageTextBind = (result-1).ToString();
                ViewModel.LoadGotoPageCommand.Execute(null);
            }
        }

        private void UpdatePageSelection()
        {
            ForumBoardPagePageList.SetAdapter(ViewModel.AvailablePages.GetAdapter(GetPageItemTemplateDelegate));
        }

        private View GetPageItemTemplateDelegate(int i, Tuple<int, bool> tuple, View arg3)
        {

            var view = Activity.LayoutInflater.Inflate(Resource.Layout.PageIndicatorItem, null);

            view.Click += PageItemOnClick;
            view.Tag = tuple.Item1;

            var backgroundPanel = view.FindViewById(Resource.Id.PageIndicatorItemBackgroundPanel);
            var textView = view.FindViewById<TextView>(Resource.Id.PageIndicatorItemNumber);
            if (tuple.Item2)
            {
                textView.SetTextColor(Color.White);
                backgroundPanel.SetBackgroundResource(ResourceExtension.AccentColourRes);
            }
            else
            {
                textView.SetTextColor(new Color(ResourceExtension.BrushText));
                backgroundPanel.SetBackgroundResource(ResourceExtension.BrushAnimeItemInnerBackgroundRes);
            }     

            textView.Text = tuple.Item1.ToString();

            if (tuple.Item2)
                _prevHighlightedPageIndicator = view;

            return view;
        }

        private void PageItemOnClick(object sender, EventArgs eventArgs)
        {
            if(ViewModel.LoadingTopics)
                return;

            var view = sender as View;
            ViewModel.LoadPageCommand.Execute((int) view.Tag);
            //update it immediatelly
            view.FindViewById(Resource.Id.PageIndicatorItemBackgroundPanel)
                .SetBackgroundResource(ResourceExtension.AccentColourRes);
            view.FindViewById<TextView>(Resource.Id.PageIndicatorItemNumber)
                .SetTextColor(Color.White);
            _prevHighlightedPageIndicator.FindViewById(Resource.Id.PageIndicatorItemBackgroundPanel)
                .SetBackgroundResource(ResourceExtension.BrushAnimeItemInnerBackgroundRes);
            _prevHighlightedPageIndicator.FindViewById<TextView>(Resource.Id.PageIndicatorItemNumber)
                .SetTextColor(new Color(ResourceExtension.BrushText));
        }

        private View GetTopicTemplateDelegate(int i, ForumTopicEntryViewModel forumTopicEntryViewModel, View arg3)
        {
            var view = arg3;
            if (view == null)
            {
                view = Activity.LayoutInflater.Inflate(Resource.Layout.ForumBoardPagePostItem, null);

                var root = view.FindViewById(Resource.Id.ForumBordPagePostItemRootContainer);
                root.Click += PostOnClick;
                root.SetOnLongClickListener(new OnLongClickListener(OnLongClickAction));
                view.FindViewById(Resource.Id.ForumBordPagePostItemLastPostSection).Click += LastPostOnClick;
                view.FindViewById<TextView>(Resource.Id.ForumBordPagePostItemPollIcon).Typeface =
                    FontManager.GetTypeface(Activity, FontManager.TypefacePath);
            }
            view.Tag = forumTopicEntryViewModel.Wrap();
     
            view.FindViewById(Resource.Id.ForumBordPagePostItemRootContainer).SetBackgroundResource(i % 2 == 0 ? ResourceExtension.BrushRowAlternate1Res : ResourceExtension.BrushRowAlternate2Res);

            if (forumTopicEntryViewModel.FontAwesomeIcon == FontAwesomeIcon.None)
                view.FindViewById(Resource.Id.ForumBordPagePostItemPollIcon).Visibility = ViewStates.Gone;
            else
            {
                var icnView = view.FindViewById<TextView>(Resource.Id.ForumBordPagePostItemPollIcon);
                icnView.Visibility = ViewStates.Visible;
                icnView.SetText(DummyFontAwesomeToRealFontAwesomeConverter.Convert(forumTopicEntryViewModel.FontAwesomeIcon));
            }

            view.FindViewById<TextView>(Resource.Id.ForumBordPagePostItemTitle).Text =
                forumTopicEntryViewModel.Data.Title;
            view.FindViewById<TextView>(Resource.Id.ForumBordPagePostItemOpAndDate).Text =
                $"{forumTopicEntryViewModel.Data.Op} {forumTopicEntryViewModel.Data.Created}";
            view.FindViewById<TextView>(Resource.Id.ForumBordPagePostItemReplies).Text =
                forumTopicEntryViewModel.Data.Replies;
            view.FindViewById<TextView>(Resource.Id.ForumBordPagePostItemLastPostAuthor).Text =
                forumTopicEntryViewModel.Data.LastPoster;
            view.FindViewById<TextView>(Resource.Id.ForumBordPagePostItemLastPostDate).Text =
                forumTopicEntryViewModel.Data.LastPostDate;

            view.FindViewById(Resource.Id.ForumBordPagePostItemLastPostSection).Tag = forumTopicEntryViewModel.Wrap();

            return view;
        }

        private void OnLongClickAction(View view)
        {
           

            _menu = FlyoutMenuBuilder.BuildGenericFlyout(Context, view,
                new List<string> { "Pin", "Pin to lastpost" }, i =>
                {
                    var vm = (view.Parent as View).Tag.Unwrap<ForumTopicEntryViewModel>();
                    if (i == 0)
                    {
                        vm.PinCommand.Execute(null);
                    }
                    else
                    {
                        vm.PinLastpostCommand.Execute(null);
                    }
                    _menu.Dismiss(true);
                });
            _menu.Show();
        }

        private void LastPostOnClick(object sender, EventArgs e)
        {
            ViewModel.GotoLastPostCommand.Execute((sender as View).Tag.Unwrap<ForumTopicEntryViewModel>());
        }

        private void PostOnClick(object sender, EventArgs eventArgs)
        {
            ViewModel.LoadTopic(((sender as View).Parent as View).Tag.Unwrap<ForumTopicEntryViewModel>());
        }

        public override int LayoutResourceId => Resource.Layout.ForumBoardPage;


        #region Views
        private TextView _forumBoardPageIcon;
        private TextView _forumBoardPageTitle;
        private Button _forumBoardPageSearchButton;
        private FrameLayout _forumBoardPageGotoPageButton;
        private LinearLayout _forumBoardPagePageList;
        private ListView _forumBoardPagePostsList;
        private FloatingActionButton _forumBoardPageActionButton;
        private ProgressBar _forumBoardPageLoadingSpinner;

        public TextView ForumBoardPageIcon => _forumBoardPageIcon ?? (_forumBoardPageIcon = FindViewById<TextView>(Resource.Id.ForumBoardPageIcon));

        public TextView ForumBoardPageTitle => _forumBoardPageTitle ?? (_forumBoardPageTitle = FindViewById<TextView>(Resource.Id.ForumBoardPageTitle));

        public Button ForumBoardPageSearchButton => _forumBoardPageSearchButton ?? (_forumBoardPageSearchButton = FindViewById<Button>(Resource.Id.ForumBoardPageSearchButton));

        public FrameLayout ForumBoardPageGotoPageButton => _forumBoardPageGotoPageButton ?? (_forumBoardPageGotoPageButton = FindViewById<FrameLayout>(Resource.Id.ForumBoardPageGotoPageButton));

        public LinearLayout ForumBoardPagePageList => _forumBoardPagePageList ?? (_forumBoardPagePageList = FindViewById<LinearLayout>(Resource.Id.ForumBoardPagePageList));

        public ListView ForumBoardPagePostsList => _forumBoardPagePostsList ?? (_forumBoardPagePostsList = FindViewById<ListView>(Resource.Id.ForumBoardPagePostsList));

        public FloatingActionButton ForumBoardPageActionButton => _forumBoardPageActionButton ?? (_forumBoardPageActionButton = FindViewById<FloatingActionButton>(Resource.Id.ForumBoardPageActionButton));

        public ProgressBar ForumBoardPageLoadingSpinner => _forumBoardPageLoadingSpinner ?? (_forumBoardPageLoadingSpinner = FindViewById<ProgressBar>(Resource.Id.ForumBoardPageLoadingSpinner));


        #endregion
    }
}