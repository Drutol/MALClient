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
using MALClient.Android.Fragments.SettingsFragments;
using MALClient.Android.ViewModels;
using MALClient.Models.Enums;

namespace MALClient.Android.Fragments
{
    public class SettingsPageFragment : MalFragmentBase
    {
        private SettingsViewModel ViewModel;

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = AndroidViewModelLocator.Settings;
            ViewModel.NavigationRequest += ViewModelOnNavigationRequest;
        }

        private void ViewModelOnNavigationRequest(SettingsPageIndex page)
        {
            Fragment fragment = null;
            switch (page)
            {
                case SettingsPageIndex.General:
                    fragment = new SettingsGeneralFragment();
                    break;
                case SettingsPageIndex.Caching:
                    break;
                case SettingsPageIndex.Calendar:
                    break;
                case SettingsPageIndex.Articles:
                    break;
                case SettingsPageIndex.News:
                    break;
                case SettingsPageIndex.About:
                    break;
                case SettingsPageIndex.LogIn:
                    break;
                case SettingsPageIndex.Misc:
                    break;
                case SettingsPageIndex.Homepage:
                    fragment = new SettingsHomepageFragment();
                    break;
                case SettingsPageIndex.Notifications:
                    break;
                case SettingsPageIndex.Ads:
                    break;
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
            ViewModelOnNavigationRequest(SettingsPageIndex.Homepage);
        }

        public override int LayoutResourceId => Resource.Layout.SettingsPage;

        #region Views

        private FrameLayout _searchPageContentFrame;

        public FrameLayout SearchPageContentFrame => _searchPageContentFrame ?? (_searchPageContentFrame = FindViewById<FrameLayout>(Resource.Id.SearchPageContentFrame));


        #endregion
    }
}