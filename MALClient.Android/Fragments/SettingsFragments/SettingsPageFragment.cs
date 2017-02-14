using System;
using Android.App;
using Android.OS;
using Android.Widget;
using MALClient.Android.Resources;
using MALClient.Android.ViewModels;
using MALClient.Models.Enums;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.Fragments.SettingsFragments
{
    public class SettingsPageFragment : MalFragmentBase
    {
        private SettingsViewModel ViewModel;

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = AndroidViewModelLocator.Settings;
            ViewModel.NavigationRequest += ViewModelOnNavigationRequest;
            ViewModelLocator.NavMgr.DeregisterBackNav();
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageAnimeList, null);
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
                    fragment = new SettingsCalendarFragment();
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
                    fragment = new SettingsNotificationsFragment();
                    break;
                case SettingsPageIndex.Ads:
                    fragment = new SettingsAdsFragment();
                    break;
                case SettingsPageIndex.Feeds:
                    fragment = new SettingsFeedsFragment();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(page), page, null);
            }
            if (fragment == null)
            {
                Toast.MakeText(Activity, "Not implemented yet, traveller!", ToastLength.Short);
                return;
            }
            var trans = FragmentManager.BeginTransaction();
            trans.SetCustomAnimations(Resource.Animator.animation_slide_btm,
                Resource.Animator.animation_fade_out,
                Resource.Animator.animation_slide_btm,
                Resource.Animator.animation_fade_out);
            trans.Replace(Resource.Id.SearchPageContentFrame, fragment);
            trans.Commit();
        }

        protected override void Cleanup()
        {
            ViewModelLocator.NavMgr.ResetMainBackNav();
            base.Cleanup();
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