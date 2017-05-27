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
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.BindingConverters;
using MALClient.Models.Models.Forums;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Forums;

namespace MALClient.Android.Fragments.ForumFragments.Tabs
{
    public class ForumIndexPageFragmentRecentsTabFragment : MalFragmentBase
    {
        private ForumIndexViewModel ViewModel;

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = ViewModelLocator.ForumsIndex;
        }


        protected override void InitBindings()
        {
            Bindings.Add(
                this.SetBinding(() => ViewModel.LoadingSideContentVisibility,
                    () => ForumIndexPageRecentsTabLoadingSpinner.Visibility)
                    .ConvertSourceToTarget(Converters.BoolToVisibility));
            Bindings.Add(this.SetBinding(() => ViewModel.ForumIndexContent).WhenSourceChanges(() =>
            {
                if (ViewModel.ForumIndexContent != null)
                {
                    ForumIndexPageRecentsTabListView.Adapter = new List<List<ForumPostEntry>>
                    {

                        ViewModel.ForumIndexContent.PopularNewTopics,
                        ViewModel.ForumIndexContent.RecentPosts,
                        ViewModel.ForumIndexContent.AnimeSeriesDisc,
                        ViewModel.ForumIndexContent.MangaSeriesDisc,
                    }.GetAdapter(GetRecentsSectionTemplateDelegate);
                }
            }));
        }

        private View GetRecentsSectionTemplateDelegate(int i, List<ForumPostEntry> forumPostEntries, View arg3)
        {
            var view = arg3 ?? Activity.LayoutInflater.Inflate(Resource.Layout.ForumsIndexSectionItem, null);

            view.FindViewById<TextView>(Resource.Id.ForumsIndexSectionItemListHeader).Text = IndexToSectionName();
            view.FindViewById<LinearLayout>(Resource.Id.ForumsIndexSectionItemList).SetAdapter(forumPostEntries.GetAdapter(GetRecentPostTemplateDelegate));

            return view;

            //hooray c#7
            string IndexToSectionName()
            {
                switch (i)
                {
                    case 0:
                        return "Popular New Topics";
                    case 1:
                        return "Recent Posts";
                    case 2:
                        return "Recent Anime Discussion";
                    case 3:
                        return "Recent Manga Discussion";
                }
                return "";
            }
        }

        private View GetRecentPostTemplateDelegate(int i, ForumPostEntry forumPostEntry, View arg3)
        {
            var view = arg3;
            if (view == null)
            {
                view = Activity.LayoutInflater.Inflate(Resource.Layout.ForumIndexPageRecentPostItem, null);
                view.FindViewById(Resource.Id.ForumIndexPageRecentPostItemRootContainer).Click += RecentPostOnClick;
            }

            view.FindViewById(Resource.Id.ForumIndexPageRecentPostItemRootContainer).Tag = forumPostEntry.Wrap();

            view.FindViewById<TextView>(Resource.Id.ForumIndexPageRecentPostItemTitle).Text = forumPostEntry.Title;
            view.FindViewById<TextView>(Resource.Id.ForumIndexPageRecentPostItemDate).Text = $"{forumPostEntry.Created} by {forumPostEntry.Op}";
            view.FindViewById<ImageViewAsync>(Resource.Id.ForumIndexPageRecentPostItemImage).Into(forumPostEntry.ImgUrl,new CircleTransformation());


            return view;
        }

        private void RecentPostOnClick(object sender, EventArgs eventArgs)
        {
            ViewModel.NavigateRecentPostCommand.Execute((sender as View).Tag.Unwrap<ForumPostEntry>());
        }

        public override int LayoutResourceId => Resource.Layout.ForumIndexPageRecentsTab;

        #region Views

        private ListView _forumIndexPageRecentsTabListView;
        private ProgressBar _forumIndexPageRecentsTabLoadingSpinner;

        public ListView ForumIndexPageRecentsTabListView => _forumIndexPageRecentsTabListView ?? (_forumIndexPageRecentsTabListView = FindViewById<ListView>(Resource.Id.ForumIndexPageRecentsTabListView));

        public ProgressBar ForumIndexPageRecentsTabLoadingSpinner => _forumIndexPageRecentsTabLoadingSpinner ?? (_forumIndexPageRecentsTabLoadingSpinner = FindViewById<ProgressBar>(Resource.Id.ForumIndexPageRecentsTabLoadingSpinner));

        #endregion
    }
}