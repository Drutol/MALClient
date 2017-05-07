using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MALClient.Android.Activities;
using MALClient.Android.CollectionAdapters;
using MALClient.Android.Resources;
using MALClient.Models.Enums;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Fragments.CalendarFragments
{
    public class CalendarPageTabFragment : MalFragmentBase
    {
        private readonly List<AnimeItemViewModel> _items;
        private GridViewColumnHelper _gridViewColumnHelper;
        private static readonly TimeZoneInfo _jstTimeZone = TimeZoneInfo.CreateCustomTimeZone("JST", TimeSpan.FromHours(9), "JST", "JST");

        public CalendarPageTabFragment(List<AnimeItemViewModel> items)
        {
            _items = items;
        }

        protected override void Init(Bundle savedInstanceState)
        {
            foreach (var animeItemViewModel in _items)
                animeItemViewModel.TimeTillNextAirCache = animeItemViewModel.GetTimeTillNextAir(_jstTimeZone);

        }

        protected override void InitBindings()
        {
            CalendarPageTabContentList.InjectAnimeListAdapter(Context,_items,AnimeListDisplayModes.IndefiniteGrid,OnItemClick,false,true);
            _gridViewColumnHelper = new GridViewColumnHelper(CalendarPageTabContentList);
        }

        private void OnItemClick(AnimeItemViewModel animeItemViewModel)
        {
            animeItemViewModel.NavigateDetails(PageIndex.PageCalendar);
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            _gridViewColumnHelper.OnConfigurationChanged(newConfig);
            base.OnConfigurationChanged(newConfig);
        }

        public override int LayoutResourceId => Resource.Layout.CalenarPageTabContent;

        #region Views

        private GridView _calendarPageTabContentList;

        public GridView CalendarPageTabContentList => _calendarPageTabContentList ?? (_calendarPageTabContentList = FindViewById<GridView>(Resource.Id.CalendarPageTabContentList));

        #endregion
    }
}