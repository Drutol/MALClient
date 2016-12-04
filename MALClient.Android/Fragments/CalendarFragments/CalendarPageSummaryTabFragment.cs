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
using MALClient.Android.Activities;
using MALClient.Android.Adapters.CollectionAdapters;
using MALClient.Android.BindingInformation;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Fragments.CalendarFragments
{
    public class CalendarPageSummaryTabFragment : MalFragmentBase
    {
        private readonly List<Tuple<string, List<AnimeItemViewModel>>> _items;

        public CalendarPageSummaryTabFragment(List<Tuple<string, List<AnimeItemViewModel>>> items)
        {
            _items = items;
        }

        protected override void Init(Bundle savedInstanceState)
        {

        }

        protected override void InitBindings()
        {
            CalendarPageSummaryTabList.Adapter = _items.GetAdapter(GetTemplateDelegate);
        }

        private View GetTemplateDelegate(int i, Tuple<string, List<AnimeItemViewModel>> tuple, View convertView)
        {
            var view = convertView;
            if (view == null)
            {
                view = MainActivity.CurrentContext.LayoutInflater.Inflate(
                    Resource.Layout.CalendarPageSummaryTabContent, null);
            }

            view.FindViewById<TextView>(Resource.Id.CalendarPageSummaryTabContentHeader).Text = tuple.Item1;
            view.FindViewById<GridView>(Resource.Id.CalendarPageSummaryTabContentList).Adapter = new AnimeListItemsAdapter(MainActivity.CurrentContext,
                Resource.Layout.AnimeGridItem,tuple.Item2,(model, view1) => new AnimeGridItemBindingInfo(view1,model));

            return view;
        }

        public override int LayoutResourceId => Resource.Layout.CalendarPageSummaryTab;

        #region View

        private ListView _calendarPageSummaryTabList;

        public ListView CalendarPageSummaryTabList => _calendarPageSummaryTabList ?? (_calendarPageSummaryTabList = FindViewById<ListView>(Resource.Id.CalendarPageSummaryTabList));

        #endregion
    }
}