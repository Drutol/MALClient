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
using MALClient.Android.CollectionAdapters;
using MALClient.Android.Resources;
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Forums;

namespace MALClient.Android.Fragments.ForumFragments
{
    public class ForumTopicPageFragment : MalFragmentBase
    {
        private readonly ForumsTopicNavigationArgs _args;
        private ForumTopicViewModel ViewModel;

        private View _prevHighlightedPageIndicator;

        public ForumTopicPageFragment(ForumsTopicNavigationArgs args)
        {
            _args = args;
        }

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = ViewModelLocator.ForumsTopic;
            ViewModel.Init(_args);
        }

        protected override void InitBindings()
        {
            Bindings.Add(
                this.SetBinding(() => ViewModel.LoadingTopic,
                    () => ForumTopicPageLoadingSpinner.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));


            Bindings.Add(this.SetBinding(() => ViewModel.Messages).WhenSourceChanges(() =>
            {
                if(ViewModel.Messages != null)
                    ForumTopicPagePostsList.Adapter = new ForumPostItemsAdapter(Activity, Resource.Layout.ForumTopicPageItem, ViewModel.Messages);
            }));
            

            ViewModel.AvailablePages.CollectionChanged += (sender, args) => UpdatePageSelection();

            Bindings.Add(this.SetBinding(() => ViewModel.AvailablePages).WhenSourceChanges(UpdatePageSelection));
        }

        private void UpdatePageSelection()
        {
            ForumTopicPageList.SetAdapter(ViewModel.AvailablePages.GetAdapter(GetPageItemTemplateDelegate));
        }

        private View GetPageItemTemplateDelegate(int i, Tuple<int, bool> tuple, View arg3)
        {
            var view = Activity.LayoutInflater.Inflate(Resource.Layout.PageIndicatorItem, null);

            view.Click += PageItemOnClick;
            view.Tag = tuple.Item1;

            view.FindViewById(Resource.Id.PageIndicatorItemBackgroundPanel)
                .SetBackgroundResource(tuple.Item2
                    ? Resource.Color.AccentColour
                    : ResourceExtension.BrushAnimeItemInnerBackgroundRes);

            view.FindViewById<TextView>(Resource.Id.PageIndicatorItemNumber).Text = tuple.Item1.ToString();

            if (tuple.Item2)
                _prevHighlightedPageIndicator = view;

            return view;
        }

        private void PageItemOnClick(object sender, EventArgs eventArgs)
        {
            var view = sender as View;
            ViewModel.LoadPageCommand.Execute((int)view.Tag);
            //update it immediatelly
            view.FindViewById(Resource.Id.PageIndicatorItemBackgroundPanel)
                .SetBackgroundResource(Resource.Color.AccentColour);
            _prevHighlightedPageIndicator.FindViewById(Resource.Id.PageIndicatorItemBackgroundPanel)
                .SetBackgroundResource(ResourceExtension.BrushAnimeItemInnerBackgroundRes);
        }

        public override int LayoutResourceId => Resource.Layout.ForumTopicPage;


        #region Views

        private Button _forumTopicPageNewReplyButton;
        private Button _forumTopicPageToggleWatchingButton;
        private ImageButton _forumTopicPageGotoPageButton;
        private LinearLayout _forumTopicPageList;
        private ListView _forumTopicPagePostsList;
        private ProgressBar _forumTopicPageLoadingSpinner;

        public Button ForumTopicPageNewReplyButton => _forumTopicPageNewReplyButton ?? (_forumTopicPageNewReplyButton = FindViewById<Button>(Resource.Id.ForumTopicPageNewReplyButton));

        public Button ForumTopicPageToggleWatchingButton => _forumTopicPageToggleWatchingButton ?? (_forumTopicPageToggleWatchingButton = FindViewById<Button>(Resource.Id.ForumTopicPageToggleWatchingButton));

        public ImageButton ForumTopicPageGotoPageButton => _forumTopicPageGotoPageButton ?? (_forumTopicPageGotoPageButton = FindViewById<ImageButton>(Resource.Id.ForumTopicPageGotoPageButton));

        public LinearLayout ForumTopicPageList => _forumTopicPageList ?? (_forumTopicPageList = FindViewById<LinearLayout>(Resource.Id.ForumTopicPageList));

        public ListView ForumTopicPagePostsList => _forumTopicPagePostsList ?? (_forumTopicPagePostsList = FindViewById<ListView>(Resource.Id.ForumTopicPagePostsList));

        public ProgressBar ForumTopicPageLoadingSpinner => _forumTopicPageLoadingSpinner ?? (_forumTopicPageLoadingSpinner = FindViewById<ProgressBar>(Resource.Id.ForumTopicPageLoadingSpinner));



        #endregion
    }
}