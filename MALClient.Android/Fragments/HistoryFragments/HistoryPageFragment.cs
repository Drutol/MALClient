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

namespace MALClient.Android.Fragments.HistoryFragments
{
    public class HistoryPageFragment : MalFragmentBase
    {
        private static HistoryNavigationArgs _args;
        public static HistoryNavigationArgs LastArgs => _args;
        private HistoryViewModel ViewModel;

        public HistoryPageFragment(HistoryNavigationArgs args)
        {
            _args = args;
        }

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = ViewModelLocator.History;
            ViewModel.Init(_args);
        }

        protected override void InitBindings()
        {
            Bindings.Add(this.SetBinding(() => ViewModel.History).WhenSourceChanges(() =>
            {
                if(ViewModel.History == null)
                    return;

                HistoryPagePivot.Adapter = new HistoryPagerAdapter(FragmentManager,ViewModel.History);
                HistoryPageTabStrip.SetViewPager(HistoryPagePivot);
            }));

            Bindings.Add(
                this.SetBinding(() => ViewModel.HistoryEmptyNoticeVisibility,
                    () => HistoryPageEmptyNotice.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));

            Bindings.Add(
                this.SetBinding(() => ViewModel.LoadingVisibility,
                    () => HistoryPageLoadingSpinner.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));
        }

        public override int LayoutResourceId => Resource.Layout.HistoryPage;

        #region Views

        private PagerSlidingTabStrip _historyPageTabStrip;
        private ViewPager _historyPagePivot;
        private ProgressBar _historyPageLoadingSpinner;
        private TextView _historyPageEmptyNotice;

        public PagerSlidingTabStrip HistoryPageTabStrip => _historyPageTabStrip ?? (_historyPageTabStrip = FindViewById<PagerSlidingTabStrip>(Resource.Id.HistoryPageTabStrip));

        public ViewPager HistoryPagePivot => _historyPagePivot ?? (_historyPagePivot = FindViewById<ViewPager>(Resource.Id.HistoryPagePivot));

        public ProgressBar HistoryPageLoadingSpinner => _historyPageLoadingSpinner ?? (_historyPageLoadingSpinner = FindViewById<ProgressBar>(Resource.Id.HistoryPageLoadingSpinner));

        public TextView HistoryPageEmptyNotice => _historyPageEmptyNotice ?? (_historyPageEmptyNotice = FindViewById<TextView>(Resource.Id.HistoryPageEmptyNotice));

        #endregion
    }
}