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
    public class ScrollChangedListener : Java.Lang.Object, AbsListView.IOnScrollListener
    {
        private readonly Action<AbsListView, ScrollState> _action;

        public ScrollChangedListener(Action<AbsListView, ScrollState> action)
        {
            _action = action;
        }

        public void OnScroll(AbsListView view, int firstVisibleItem, int visibleItemCount, int totalItemCount)
        {
            //
        }

        public void OnScrollStateChanged(AbsListView view, ScrollState scrollState)
        {
            _action.Invoke(view,scrollState);
        }
    }
}