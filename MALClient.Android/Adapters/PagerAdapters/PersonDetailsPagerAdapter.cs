using System;
using Android.Graphics;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using MALClient.Android.Fragments;
using MALClient.Android.Fragments.DetailsFragments;
using MALClient.Android.Resources;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;
using PagerSlidingTab;

namespace MALClient.Android.PagerAdapters
{
    public class PersonDetailsPagerAdapter : FragmentStatePagerAdapter, ICustomTabProvider
    {
        private readonly MalFragmentBase _vaFragment;
        private readonly MalFragmentBase _prodFragment;

        public PersonDetailsPagerAdapter(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public PersonDetailsPagerAdapter(FragmentManager fm) : base(fm)
        {
            _vaFragment = new PersonDetailsPageVaTabFragment();
            _prodFragment = new PersonDetailsPageProdTabFragment();
        }

        public override int Count => 2;

        public override Fragment GetItem(int p1)
        {
            switch (p1)
            {
                case 0:
                    return _vaFragment;
                case 1:
                    return _prodFragment;
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
                    txt.Text = "Voice Acting Roles";
                    break;
                case 1:
                    txt.Text = "Production Roles";
                    break;
            }

            return txt;
        }
    }
}