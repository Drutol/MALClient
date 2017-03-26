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

namespace MALClient.Android.Listeners
{
    public class OnPageChangedListener : Java.Lang.Object, ViewPager.IOnPageChangeListener
    {
        private readonly Action<int> _onPageSelectedListener;

        public OnPageChangedListener(Action<int> onPageSelectedListener)
        {
            _onPageSelectedListener = onPageSelectedListener;
        }

        public void OnPageScrollStateChanged(int state)
        {

        }

        public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
        {

        }

        public void OnPageSelected(int position)
        {
            _onPageSelectedListener.Invoke(position);
        }
    }
}