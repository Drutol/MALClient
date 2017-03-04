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

        public ForumMainPageFragment(ForumsNavigationArgs args)
        {
            _args = args;
        }

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = ViewModelLocator.ForumsMain;
            ViewModel.Init(_args);

            ViewModel.NavigationRequested += ViewModelOnNavigationRequested;
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
            
            var trans = FragmentManager.BeginTransaction();
            trans.SetCustomAnimations(Resource.Animator.animation_slide_btm,
                Resource.Animator.animation_fade_out,
                Resource.Animator.animation_slide_btm,
                Resource.Animator.animation_fade_out);
            trans.Replace(Resource.Id.SearchPageContentFrame, fragment);
            trans.Commit();
        }

        protected override void InitBindings()
        {
            throw new NotImplementedException();
        }

        public override int LayoutResourceId { get; }
    }
}