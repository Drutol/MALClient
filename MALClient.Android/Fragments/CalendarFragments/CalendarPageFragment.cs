using System.Collections.Generic;
using System.Threading.Tasks;
using Android.OS;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.BindingConverters;
using MALClient.Android.PagerAdapters;
using MALClient.Android.Resources;
using MALClient.Models.Enums;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Fragments.CalendarFragments
{
    public partial class CalendarPageFragment : MalFragmentBase
    {

        private CalendarPageViewModel ViewModel;

        private CalendarPageFragment()
        {
            
        }

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModelLocator.NavMgr.DeregisterBackNav();
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageAnimeList, null);
            ViewModelLocator.AnimeList.AnimeItemsDisplayContext = AnimeItemDisplayContext.AirDay;
            ViewModel = ViewModelLocator.CalendarPage;
            ViewModel.Init();
        }

        protected override void InitBindings()
        {                   
            Bindings.Add(
                this.SetBinding(() => ViewModel.ProgressValue,
                    () => CalendarPageProgressBar.Progress));
            Bindings.Add(
                this.SetBinding(() => ViewModel.MaxProgressValue,
                    () => CalendarPageProgressBar.Max));

            
            Bindings.Add(
                this.SetBinding(() => ViewModel.CalendarBuildingVisibility,
                    () => CalendarPageProgressBarGrid.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));

            
            Bindings.Add(this.SetBinding(() => ViewModel.CalendarData).WhenSourceChanges( async () =>
            {
                CalendarPageViewPager.Adapter = new CalendarPagerAdapter(ChildFragmentManager, ViewModel.CalendarData);
                CalendarPageTabStrip.SetViewPager(CalendarPageViewPager);
                CalendarPageTabStrip.CenterTabs();

                await Task.Delay(30);
                CalendarPageViewPager.SetCurrentItem(ViewModel.CalendarPivotIndex,false);
            }));

            CalendarPageViewPager.OffscreenPageLimit = 7;

            Bindings.Add(
                this.SetBinding(() => ViewModel.CalendarVisibility,
                    () => CalendarPageContentGrid.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));
        }

        public override int LayoutResourceId => Resource.Layout.CalendarPage;

        public static CalendarPageFragment Instance => new CalendarPageFragment();
    }
}