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
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Adapters.PagerAdapters;
using MALClient.Android.BindingConverters;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Fragments
{
    public partial class CalendarPageFragment : MalFragmentBase
    {

        private CalendarPageViewModel ViewModel;

        private CalendarPageFragment()
        {
            
        }

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = ViewModelLocator.CalendarPage;
            ViewModel.Init();
        }

        protected override void InitBindings()
        {
            Bindings.Add(CalendarPageProgressBar.Id, new List<Binding>());
            Bindings[CalendarPageProgressBar.Id].Add(
                this.SetBinding(() => ViewModel.ProgressValue,
                    () => CalendarPageProgressBar.Progress));
            Bindings[CalendarPageProgressBar.Id].Add(
                this.SetBinding(() => ViewModel.MaxProgressValue,
                    () => CalendarPageProgressBar.Max));

            Bindings.Add(CalendarPageProgressBarGrid.Id, new List<Binding>());
            Bindings[CalendarPageProgressBarGrid.Id].Add(
                this.SetBinding(() => ViewModel.CalendarBuildingVisibility,
                    () => CalendarPageProgressBarGrid.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));

            Bindings.Add(CalendarPageViewPager.Id,new List<Binding>());
            Bindings[CalendarPageViewPager.Id].Add(this.SetBinding(() => ViewModel.CalendarData).WhenSourceChanges(() =>
            {
                CalendarPageViewPager.Adapter = new CalendarPagerAdapter(FragmentManager, ViewModel.CalendarData);
                CalendarPageTabStrip.SetViewPager(CalendarPageViewPager);
            }));

            Bindings.Add(CalendarPageContentGrid.Id, new List<Binding>());
            Bindings[CalendarPageContentGrid.Id].Add(
                this.SetBinding(() => ViewModel.CalendarVisibility,
                    () => CalendarPageContentGrid.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));
        }

        public override int LayoutResourceId => Resource.Layout.CalendarPage;

        public static CalendarPageFragment Instance => new CalendarPageFragment();
    }
}