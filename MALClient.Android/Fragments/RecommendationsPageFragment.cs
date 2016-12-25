using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Com.Astuetz;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Adapters.PagerAdapters;
using MALClient.Android.BindingConverters;
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Fragments
{
    public class RecommendationsPageFragment : MalFragmentBase
    {
        private readonly RecommendationPageNavigationArgs _args;
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
        }

        protected override void InitBindings()
        {
            Bindings.Add(RecommendationsPagePivot.Id, new List<Binding>());
            Bindings[RecommendationsPagePivot.Id].Add(
                this.SetBinding(() => ViewModel.RecommendationItems).WhenSourceChanges(() =>
                {
                    if (ViewModel.RecommendationItems.Any())
                    {
                        RecommendationsPagePivot.Adapter = new RecommandtionsPagerAdapter(FragmentManager,
                            ViewModel.RecommendationItems.Select(item => item.Content as RecommendationItemViewModel));
                        RecommendationsPageTabStrip.SetViewPager(RecommendationsPagePivot);
                        RecommendationsPagePivot.SetCurrentItem(ViewModel.PivotItemIndex, false);
                    }
                }));


            Bindings.Add(RecommendationsPageLoading.Id, new List<Binding>());
            Bindings[RecommendationsPageLoading.Id].Add(
                this.SetBinding(() => ViewModel.Loading,
                    () => RecommendationsPageLoading.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));
        }

        public override int LayoutResourceId => Resource.Layout.RecommendationsPage;

        #region Views

        private ImageButton _recommendationsPageTypeChangeButton;
        private PagerSlidingTabStrip _recommendationsPageTabStrip;
        private ViewPager _recommendationsPagePivot;
        private RelativeLayout _recommendationsPageLoading;

        public ImageButton RecommendationsPageTypeChangeButton => _recommendationsPageTypeChangeButton ?? (_recommendationsPageTypeChangeButton = FindViewById<ImageButton>(Resource.Id.RecommendationsPageTypeChangeButton));

        public PagerSlidingTabStrip RecommendationsPageTabStrip => _recommendationsPageTabStrip ?? (_recommendationsPageTabStrip = FindViewById<PagerSlidingTabStrip>(Resource.Id.RecommendationsPageTabStrip));

        public ViewPager RecommendationsPagePivot => _recommendationsPagePivot ?? (_recommendationsPagePivot = FindViewById<ViewPager>(Resource.Id.RecommendationsPagePivot));

        public RelativeLayout RecommendationsPageLoading => _recommendationsPageLoading ?? (_recommendationsPageLoading = FindViewById<RelativeLayout>(Resource.Id.RecommendationsPageLoading));



        #endregion

    }
}