using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using GalaSoft.MvvmLight.Helpers;
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
            try
            {
                trans.Commit();
            }
            catch (Exception e)
            {
                Debugger.Break();
            }

        }

        protected override void InitBindings()
        {
            ForumsMainPageMoreButton.Click += ForumsMainPageMoreButtonOnClick;

            ForumsMainPagePinnedPostsButton.Click += ForumsMainPagePinnedPostsButtonOnClick;

            Bindings.Add(this.SetBinding(() => ViewModel.PinnedBoards)
                .WhenSourceChanges(() =>
                {
                    ViewModel.PinnedBoards.CollectionChanged += (sender, args) => UpdatePinnedBoards();
                    UpdatePinnedBoards();
                }));
        }

        private void UpdatePinnedBoards()
        {
            ForumsMainPagePinnedBoardsList.RemoveAllViews();
            ForumsMainPagePinnedBoardsList.SetAdapter(ViewModel.PinnedBoards.GetAdapter(GetTemplateDelegate));
        }

        private View GetTemplateDelegate(int i, ForumBoards forumBoards, View arg3)
        {

            var  view = Activity.LayoutInflater.Inflate(Resource.Layout.ForumMainPagePinnedBoardItem,null);
            var root = view.FindViewById(Resource.Id.ForumMainPagePinnedBoardItemRootContainer);
            root.Click += OnPinnedBoardClick;
            root.Tag = (int) forumBoards;

            var txt = forumBoards.GetDescription();
            if (txt.Length > 20 && txt.Length < 23)
            {
               txt = txt.Substring(0, 20) + "...";
            }
            view.FindViewById<TextView>(Resource.Id.ForumMainPagePinnedBoardItemText).Text = txt;
                

            return view;
        }

        private void OnPinnedBoardClick(object o, EventArgs eventArgs)
        {
            ViewModel.GotoPinnedBoardCommand.Execute((ForumBoards)(int)(o as View).Tag);
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

        private FrameLayout _forumsMainPagePinnedPostsButton;
        private LinearLayout _forumsMainPagePinnedBoardsList;
        private FrameLayout _forumsMainPageMoreButton;
        private FrameLayout _forumsContentFrame;

        public FrameLayout ForumsMainPagePinnedPostsButton => _forumsMainPagePinnedPostsButton ?? (_forumsMainPagePinnedPostsButton = FindViewById<FrameLayout>(Resource.Id.ForumsMainPagePinnedPostsButton));

        public LinearLayout ForumsMainPagePinnedBoardsList => _forumsMainPagePinnedBoardsList ?? (_forumsMainPagePinnedBoardsList = FindViewById<LinearLayout>(Resource.Id.ForumsMainPagePinnedBoardsList));

        public FrameLayout ForumsMainPageMoreButton => _forumsMainPageMoreButton ?? (_forumsMainPageMoreButton = FindViewById<FrameLayout>(Resource.Id.ForumsMainPageMoreButton));

        public FrameLayout ForumsContentFrame => _forumsContentFrame ?? (_forumsContentFrame = FindViewById<FrameLayout>(Resource.Id.ForumsContentFrame));


        #endregion
    }
}