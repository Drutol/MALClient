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

namespace MALClient.Android.Listeners
{
    public class ScrollListener : Java.Lang.Object, AbsListView.IOnScrollChangeListener
    {
        private readonly Action<int> _callback;

        public ScrollListener(Action<int> callback)
        {
            _callback = callback;
        }

        public void OnScrollChange(View v, int scrollX, int scrollY, int oldScrollX, int oldScrollY)
        {
            _callback.Invoke(scrollY);
        }
    }
}