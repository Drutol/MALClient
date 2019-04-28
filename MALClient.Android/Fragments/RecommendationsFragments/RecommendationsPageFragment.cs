using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.OS;
using Android.Support.V4.View;
using Android.Widget;
using Com.Shehabic.Droppy;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.BindingConverters;
using MALClient.Android.Flyouts;
using MALClient.Android.PagerAdapters;
using MALClient.Android.Resources;

using MALClient.Models.Enums;
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Fragments.RecommendationsFragments
{
    public class RecommendationsPageFragment : MalFragmentBase
    {
        private readonly RecommendationPageNavigationArgs _args;
        private DroppyMenuPopup _menu;
        private RecommendationsViewModel ViewModel;

        public RecommendationsPageFragment(RecommendationPageNavigationArgs args)
        {
            _args = args;
        }

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = ViewModelLocator.Recommendations;
            ViewModel.PopulateData();
            if(_args != null)
                ViewModel.PivotItemIndex = _args.Index;
            ViewModelLocator.NavMgr.DeregisterBackNav();
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageAnimeList, null);
        }

        protected override void InitBindings()
        {

            Bindings.Add(
                this.SetBinding(() => ViewModel.RecommendationAnimeItems).WhenSourceChanges(() =>
                {
                    BindAnimeItems();
                }));
            Bindings.Add(
                this.SetBinding(() => ViewModel.RecommendationMangaItems).WhenSourceChanges(() =>
                {
                    BindMangaItems();
                }));
            Bindings.Add(this.SetBinding(() => ViewModel.CurrentWorkMode)
                .WhenSourceChanges(() =>
                {
                    if(ViewModel.CurrentWorkMode == RecommendationsPageWorkMode.Anime)
                        BindAnimeItems();
                    else if(ViewModel.CurrentWorkMode == RecommendationsPageWorkMode.Manga)
                        BindMangaItems();
                }));





            Bindings.Add(
                this.SetBinding(() => ViewModel.Loading,
                    () => RecommendationsPageLoading.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));

            RecommendationsPageTypeChangeButton.SetCommand("Click",new RelayCommand(OnTypeChangeButtonClick));
        }

        private void BindAnimeItems()
        {
            if (ViewModel.RecommendationAnimeItems?.Any() ?? false)
            {
                RecommendationsPagePivot.Adapter = new RecommandtionsPagerAdapter(ChildFragmentManager,
                    ViewModel.RecommendationAnimeItems.Select(
                        item => item.Content as RecommendationItemViewModel));
                RecommendationsPageTabStrip.SetViewPager(RecommendationsPagePivot);
                RecommendationsPagePivot.SetCurrentItem(ViewModel.PivotItemIndex, false);
            }
        }

        private void BindMangaItems()
        {
            if (ViewModel.RecommendationMangaItems?.Any() ?? false)
            {
                RecommendationsPagePivot.Adapter = new RecommandtionsPagerAdapter(ChildFragmentManager,
                    ViewModel.RecommendationMangaItems.Select(item => item.Content as RecommendationItemViewModel));
                RecommendationsPageTabStrip.SetViewPager(RecommendationsPagePivot);
                RecommendationsPagePivot.SetCurrentItem(ViewModel.PivotItemIndex, false);
            }
        }

        private void OnTypeChangeButtonClick()
        {
            _menu = RecommendationsFlyoutBuilder.BuildForRecommendationsPage(Activity, RecommendationsPageTypeChangeButton,
                ViewModel, i =>
               {
                   if(i == 0)
                     ViewModel.CurrentWorkMode = RecommendationsPageWorkMode.Anime;
                   else
                       ViewModel.CurrentWorkMode = RecommendationsPageWorkMode.Manga;
                   _menu.Dismiss(true);
               });
            _menu.Show();
        }

        public override int LayoutResourceId => Resource.Layout.RecommendationsPage;

        #region Views

        private ImageButton _recommendationsPageTypeChangeButton;
        private com.refractored.PagerSlidingTabStrip _recommendationsPageTabStrip;
        private ViewPager _recommendationsPagePivot;
        private RelativeLayout _recommendationsPageLoading;


        public ImageButton RecommendationsPageTypeChangeButton => _recommendationsPageTypeChangeButton ?? (_recommendationsPageTypeChangeButton = FindViewById<ImageButton>(Resource.Id.RecommendationsPageTypeChangeButton));

        public com.refractored.PagerSlidingTabStrip RecommendationsPageTabStrip => _recommendationsPageTabStrip ?? (_recommendationsPageTabStrip = FindViewById<com.refractored.PagerSlidingTabStrip>(Resource.Id.RecommendationsPageTabStrip));

        public ViewPager RecommendationsPagePivot => _recommendationsPagePivot ?? (_recommendationsPagePivot = FindViewById<ViewPager>(Resource.Id.RecommendationsPagePivot));

        public RelativeLayout RecommendationsPageLoading => _recommendationsPageLoading ?? (_recommendationsPageLoading = FindViewById<RelativeLayout>(Resource.Id.RecommendationsPageLoading));



        #endregion

    }
}