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
    public class OnScrollListener : Java.Lang.Object , AbsListView.IOnScrollListener
    {
        private readonly Action<int, int> _action;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action">Int1 - firstVisibleIndex , Int2 - visible item count</param>
        public OnScrollListener(Action<int, int> action)
        {
            _action = action;
        }

        public void OnScroll(AbsListView view, int firstVisibleItem, int visibleItemCount, int totalItemCount)
        {
            _action.Invoke(firstVisibleItem,totalItemCount);
        }

        public void OnScrollStateChanged(AbsListView view, ScrollState scrollState)
        {
            
        }
    }
}