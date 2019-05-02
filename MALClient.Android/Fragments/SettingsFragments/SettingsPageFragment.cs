using System;
using Android.OS;
using Android.Support.V4.App;
using Android.Widget;
using GalaSoft.MvvmLight.Command;
using MALClient.Android.Adapters;
using MALClient.Android.Resources;
using MALClient.Android.ViewModels;
using MALClient.Models.Enums;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.Fragments.SettingsFragments
{
    public class SettingsPageFragment : MalFragmentBase
    {
        private readonly SettingsPageIndex? _page;

        public SettingsPageFragment()
        {

        }

        public SettingsPageFragment(SettingsPageIndex? page = null)
        {
            _page = page;
        }

        private SettingsViewModel ViewModel;
        private bool _navigated;

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = AndroidViewModelLocator.Settings;
            ViewModel.NavigationRequest += ViewModelOnNavigationRequest;
            ViewModelLocator.NavMgr.DeregisterBackNav();
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageAnimeList, null);
            ViewModelLocator.GeneralMain.CurrentStatus = $"Settings - {ChangelogProvider.Version}";
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
                    fragment = new SettingsAboutFragment();
                    break;
                case SettingsPageIndex.LogIn:
                    break;
                case SettingsPageIndex.Misc:
                    fragment = new SettingsMiscFragment();
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
                case SettingsPageIndex.Info:
                    fragment = new SettingsInfoFragment();
                    break;
                case SettingsPageIndex.Discord:
                    ResourceLocator.SystemControlsLauncherService.LaunchUri(new Uri("https://discord.gg/5yETtFT"));
                    return;
                default:
                    throw new ArgumentOutOfRangeException(nameof(page), page, null);
            }

            try
            {
                var trans = ChildFragmentManager.BeginTransaction();
                trans.DisallowAddToBackStack();
                trans.SetCustomAnimations(Resource.Animator.animation_slide_btm,
                    Resource.Animator.animation_sink_in,
                    Resource.Animator.animation_slide_btm,
                    Resource.Animator.animation_sink_in);
                trans.Replace(Resource.Id.SearchPageContentFrame, fragment);
                trans.CommitAllowingStateLoss();
            }
            catch (Exception e)
            {

            }
            _navigated = true;

        }

        protected override void InitBindings()
        {
            if (!_navigated)
            {
                if (_page == null)
                    ViewModelOnNavigationRequest(SettingsPageIndex.Homepage);
                else
                {
                    ViewModel.RequestNavigationCommand.Execute(_page);
                }
            }
                
        }

        public override int LayoutResourceId => Resource.Layout.SettingsPage;

        #region Views

        private FrameLayout _searchPageContentFrame;

        public FrameLayout SearchPageContentFrame => _searchPageContentFrame ?? (_searchPageContentFrame = FindViewById<FrameLayout>(Resource.Id.SearchPageContentFrame));


        #endregion
    }
}