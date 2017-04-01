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
using Com.Mikepenz.Iconics;
using Com.Shehabic.Droppy;
using MALClient.Android.Flyouts;
using MALClient.Models.Enums;
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Forums;

namespace MALClient.Android.Fragments.ForumFragments
{
    public class ForumMainPageFragment : MalFragmentBase
    {
        private readonly ForumsNavigationArgs _args;
        private ForumsMainViewModel ViewModel;

        private DroppyMenuPopup _moreMenu;
        private ForumsPinnedPostsFlyoutContext _pinnedPostsFlyoutContext;

        public ForumMainPageFragment(ForumsNavigationArgs args)
        {

            _args = args;
        }

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = ViewModelLocator.ForumsMain;
            ViewModel.NavigationRequested += ViewModelOnNavigationRequested;
            ViewModel.Init(_args);
        }

        private void ViewModelOnNavigationRequested(ForumsPageIndex page, object args)
        {
            Fragment fragment;
            switch (page)
            {
                case ForumsPageIndex.PageIndex:
                    fragment = new ForumIndexPageFragment();
                    break;
                case ForumsPageIndex.PageBoard:
                    fragment = new ForumBoardPageFragment(args as ForumsBoardNavigationArgs);
                    break;
                case ForumsPageIndex.PageTopic:
                    fragment = new ForumTopicPageFragment(args as ForumsTopicNavigationArgs);
                    break;
                case ForumsPageIndex.PageNewTopic:
                    fragment = new ForumNewTopicPageFragment(args as ForumsNewTopicNavigationArgs);
                    break;
                case ForumsPageIndex.PageStarred:
                    return;
                default:
                    throw new ArgumentOutOfRangeException(nameof(page), page, null);
            }
            
            var trans = ChildFragmentManager.BeginTransaction();
            trans.SetCustomAnimations(Resource.Animator.animation_slide_btm,
                Resource.Animator.animation_fade_out,
                Resource.Animator.animation_slide_btm,
                Resource.Animator.animation_fade_out);
            trans.Replace(Resource.Id.ForumsContentFrame, fragment);
            trans.Commit();
        }

        protected override void InitBindings()
        {
            ForumsMainPageMoreButton.Click += ForumsMainPageMoreButtonOnClick;

            ForumsMainPagePinnedPostsButton.Click += ForumsMainPagePinnedPostsButtonOnClick;
        }

        private void ForumsMainPagePinnedPostsButtonOnClick(object sender, EventArgs eventArgs)
        {
            if(_pinnedPostsFlyoutContext == null)
                _pinnedPostsFlyoutContext = new ForumsPinnedPostsFlyoutContext(ViewModel,ForumsMainPagePinnedPostsButton);
            _pinnedPostsFlyoutContext.Show();
        }

        private void ForumsMainPageMoreButtonOnClick(object sender, EventArgs eventArgs)
        {
            if (_moreMenu == null)
                _moreMenu = FlyoutMenuBuilder.BuildGenericFlyout(Activity, ForumsMainPageMoreButton, new List<string>
                {
                    "My recent topics",
                    "My watched topics",
                    "MALClient's topic",
                }, OnMoreMenuSelected);
            _moreMenu.Show();
        }

        private void OnMoreMenuSelected(int i)
        {
            switch (i)
            {
                case 0:
                    ViewModel.NavigateRecentTopicsCommand.Execute(null);
                    break;
                case 1:
                    ViewModel.NavigateWatchedTopicsCommand.Execute(null);
                    break;
                case 2:
                    ViewModel.NavigateMalClientTopicsCommand.Execute(null);
                    break;
            }
            _moreMenu.Dismiss(true);
        }

        public override int LayoutResourceId => Resource.Layout.ForumsMainPage;

        #region Views

        private ImageButton _forumsMainPagePinnedPostsButton;
        private LinearLayout _forumsMainPagePinnedBoardsList;
        private ImageButton _forumsMainPageMoreButton;
        private FrameLayout _forumsContentFrame;


        public ImageButton ForumsMainPagePinnedPostsButton => _forumsMainPagePinnedPostsButton ?? (_forumsMainPagePinnedPostsButton = FindViewById<ImageButton>(Resource.Id.ForumsMainPagePinnedPostsButton));

        public LinearLayout ForumsMainPagePinnedBoardsList => _forumsMainPagePinnedBoardsList ?? (_forumsMainPagePinnedBoardsList = FindViewById<LinearLayout>(Resource.Id.ForumsMainPagePinnedBoardsList));

        public ImageButton ForumsMainPageMoreButton => _forumsMainPageMoreButton ?? (_forumsMainPageMoreButton = FindViewById<ImageButton>(Resource.Id.ForumsMainPageMoreButton));

        public FrameLayout ForumsContentFrame => _forumsContentFrame ?? (_forumsContentFrame = FindViewById<FrameLayout>(Resource.Id.ForumsContentFrame));

        #endregion
    }
}