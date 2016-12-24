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
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Fragments
{
    public class RecommendationsPageFragment : MalFragmentBase
    {
        private RecommendationsViewModel ViewModel;

        private RecommendationsPageFragment()
        {
            
        }

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = ViewModelLocator.Recommendations;
            ViewModel.PopulateData();
        }

        protected override void InitBindings()
        {
            Bindings.Add(RecommendationsPagePivot.Id, new List<Binding>());
            Bindings[RecommendationsPagePivot.Id].Add(
                this.SetBinding(() => ViewModel.RecommendationItems).WhenSourceChanges(() =>
                {
                    RecommendationsPagePivot.Adapter = new RecommandtionsPagerAdapter(FragmentManager,ViewModel.RecommendationItems.Select(item => item.Content as RecommendationItemViewModel));
                    RecommendationsPageTabStrip.SetViewPager(RecommendationsPagePivot);
                }));
        }

        public override int LayoutResourceId => Resource.Layout.RecommendationsPage;

        #region Views

        private ImageButton _recommendationsPageTypeChangeButton;
        private PagerSlidingTabStrip _recommendationsPageTabStrip;
        private ViewPager _recommendationsPagePivot;

        public ImageButton RecommendationsPageTypeChangeButton => _recommendationsPageTypeChangeButton ?? (_recommendationsPageTypeChangeButton = FindViewById<ImageButton>(Resource.Id.RecommendationsPageTypeChangeButton));

        public PagerSlidingTabStrip RecommendationsPageTabStrip => _recommendationsPageTabStrip ?? (_recommendationsPageTabStrip = FindViewById<PagerSlidingTabStrip>(Resource.Id.RecommendationsPageTabStrip));

        public ViewPager RecommendationsPagePivot => _recommendationsPagePivot ?? (_recommendationsPagePivot = FindViewById<ViewPager>(Resource.Id.RecommendationsPagePivot));

        #endregion

        public static RecommendationsPageFragment Instance => new RecommendationsPageFragment();
    }
}