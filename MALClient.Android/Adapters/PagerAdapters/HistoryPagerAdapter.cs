using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V13.App;
using Android.Views;
using Android.Widget;
using Com.Astuetz;
using MALClient.Android.Fragments.HistoryFragments;
using MALClient.Android.Fragments.RecommendationsFragments;
using MALClient.Android.Resources;
using MALClient.Models.Models.MalSpecific;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.Adapters.PagerAdapters 
{
    public class HistoryPagerAdapter : FragmentStatePagerAdapter, PagerSlidingTabStrip.ICustomTabProvider
    {
        private readonly List<KeyValuePair<string, List<Tuple<AnimeItemViewModel, List<MalProfileHistoryEntry>>>>> _items;

        private readonly Dictionary<int, Fragment> _pageFragments = new Dictionary<int, Fragment>();


        public HistoryPagerAdapter(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public HistoryPagerAdapter(FragmentManager fm, Dictionary<string, List<Tuple<AnimeItemViewModel, List<MalProfileHistoryEntry>>>> items) : base(fm)
        {
            _items = items.ToList();
        }

        public override int Count => _items.Count;

        public override Fragment GetItem(int position)
        {
            if (!_pageFragments.ContainsKey(position))
            {
                _pageFragments.Add(position, new HistoryPageTabFragment(_items[position].Value));
            }
            return _pageFragments[position];
        }

        public View GetCustomTabView(ViewGroup p0, int p1)
        {
            var txt = new TextView(p0.Context);
            txt.SetTextColor(new Color(ResourceExtension.BrushText));
            txt.Text = _items[p1].Key;

            return txt;
        }

        public void TabSelected(View p0)
        {
            p0.Alpha = 1f;
        }

        public void TabUnselected(View p0)
        {
            p0.Alpha = .5f;
        }
    }
}