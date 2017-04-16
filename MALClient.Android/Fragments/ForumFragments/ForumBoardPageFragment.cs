using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.BindingConverters;
using MALClient.Android.DIalogs;
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
                    () => ForumBoardPageNewTopicButton.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));

            Bindings.Add(this.SetBinding(() => ViewModel.Topics).WhenSourceChanges(() =>
            {
                ForumBoardPagePostsList.Adapter = ViewModel.Topics.GetAdapter(GetTopicTemplateDelegate);
            }));

            ViewModel.AvailablePages.CollectionChanged += (sender, args) => UpdatePageSelection();

            ForumBoardPageSearchButton.Click += async (sender, args) =>
            {
                var str = await TextInputDialogBuilder.BuildInputTextDialog(Context, "Search", "keyword...");
                if (!string.IsNullOrEmpty(str))
                {
                    ViewModel.SearchQuery = str;
                    ViewModel.SearchCommand.Execute(null);
                }
            };

            Bindings.Add(this.SetBinding(() => ViewModel.AvailablePages).WhenSourceChanges(UpdatePageSelection));
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

            view.FindViewById(Resource.Id.PageIndicatorItemBackgroundPanel)
                .SetBackgroundResource(tuple.Item2
                    ? ResourceExtension.AccentColourRes
                    : ResourceExtension.BrushAnimeItemInnerBackgroundRes);

            view.FindViewById<TextView>(Resource.Id.PageIndicatorItemNumber).Text = tuple.Item1.ToString();

            if (tuple.Item2)
                _prevHighlightedPageIndicator = view;

            return view;
        }

        private void PageItemOnClick(object sender, EventArgs eventArgs)
        {
            var view = sender as View;
            ViewModel.LoadPageCommand.Execute((int) view.Tag);
            //update it immediatelly
            view.FindViewById(Resource.Id.PageIndicatorItemBackgroundPanel)
                .SetBackgroundResource(ResourceExtension.AccentColourRes);
            _prevHighlightedPageIndicator.FindViewById(Resource.Id.PageIndicatorItemBackgroundPanel)
                .SetBackgroundResource(ResourceExtension.BrushAnimeItemInnerBackgroundRes);
        }

        private View GetTopicTemplateDelegate(int i, ForumTopicEntryViewModel forumTopicEntryViewModel, View arg3)
        {
            var view = arg3;
            if (view == null)
            {
                view = Activity.LayoutInflater.Inflate(Resource.Layout.ForumBoardPagePostItem, null);

                var root = view.FindViewById(Resource.Id.ForumBordPagePostItemRootContainer);
                root.Click += PostOnClick;
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



            return view;
        }

        private void LastPostOnClick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void PostOnClick(object sender, EventArgs eventArgs)
        {
            ViewModel.LoadTopic(((sender as View).Parent as View).Tag.Unwrap<ForumTopicEntryViewModel>());
        }

        public override int LayoutResourceId => Resource.Layout.ForumBoardPage;


        #region Views

        private TextView _forumBoardPageIcon;
        private TextView _forumBoardPageTitle;
        private Button _forumBoardPageNewTopicButton;
        private Button _forumBoardPageSearchButton;
        private LinearLayout _forumBoardPagePageList;
        private ImageButton _forumBoardPageGotoPageButton;
        private ListView _forumBoardPagePostsList;
        private ProgressBar _forumBoardPageLoadingSpinner;

        public TextView ForumBoardPageIcon => _forumBoardPageIcon ?? (_forumBoardPageIcon = FindViewById<TextView>(Resource.Id.ForumBoardPageIcon));

        public TextView ForumBoardPageTitle => _forumBoardPageTitle ?? (_forumBoardPageTitle = FindViewById<TextView>(Resource.Id.ForumBoardPageTitle));

        public Button ForumBoardPageNewTopicButton => _forumBoardPageNewTopicButton ?? (_forumBoardPageNewTopicButton = FindViewById<Button>(Resource.Id.ForumBoardPageNewTopicButton));

        public Button ForumBoardPageSearchButton => _forumBoardPageSearchButton ?? (_forumBoardPageSearchButton = FindViewById<Button>(Resource.Id.ForumBoardPageSearchButton));

        public LinearLayout ForumBoardPagePageList => _forumBoardPagePageList ?? (_forumBoardPagePageList = FindViewById<LinearLayout>(Resource.Id.ForumBoardPagePageList));

        public ImageButton ForumBoardPageGotoPageButton => _forumBoardPageGotoPageButton ?? (_forumBoardPageGotoPageButton = FindViewById<ImageButton>(Resource.Id.ForumBoardPageGotoPageButton));

        public ListView ForumBoardPagePostsList => _forumBoardPagePostsList ?? (_forumBoardPagePostsList = FindViewById<ListView>(Resource.Id.ForumBoardPagePostsList));

        public ProgressBar ForumBoardPageLoadingSpinner => _forumBoardPageLoadingSpinner ?? (_forumBoardPageLoadingSpinner = FindViewById<ProgressBar>(Resource.Id.ForumBoardPageLoadingSpinner));

        #endregion
    }
}