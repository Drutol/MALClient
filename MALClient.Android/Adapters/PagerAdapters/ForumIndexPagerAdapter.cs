using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using com.refractored;
using MALClient.Android.Fragments;
using MALClient.Android.Fragments.ArticlesPageFragments;
using MALClient.Android.Fragments.ForumFragments.Tabs;
using MALClient.Android.Resources;

using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.PagerAdapters
{
    public class ForumIndexPagerAdapter : FragmentStatePagerAdapter, ICustomTabProvider
    {
        public ForumIndexPagerAdapter(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public ForumIndexPagerAdapter(FragmentManager fm) : base(fm)
        {
            _boardsFragment = new ForumIndexPageFragmentBoardsTabFragment();
            _recentsFragment = new ForumIndexPageFragmentRecentsTabFragment();
        }

        private readonly MalFragmentBase _boardsFragment;
        private readonly MalFragmentBase _recentsFragment;

        public override int Count => 2;

        public override Fragment GetItem(int p1)
        {
            switch (p1)
            {
                case 0:
                    return _boardsFragment;
                case 1:
                    return _recentsFragment;
            }
            throw new ArgumentException();
        }

        public void TabSelected(View p0)
        {
            var txt = p0 as TextView;
            txt.Alpha = 1f;
        }

        public void TabUnselected(View p0)
        {
            var txt = p0 as TextView;
            txt.Alpha = .7f;
        }

        public View GetCustomTabView(ViewGroup p0, int p1)
        {
            var txt = new TextView(p0.Context);
            txt.SetTextColor(new Color(ResourceExtension.BrushText));
            txt.Tag = p1;
            switch (p1)
            {
                case 0:
                    txt.Text = "Boards";
                    break;
                case 1:
                    txt.Text = "Recent";
                    break;
            }

            return txt;
        }
    }


}