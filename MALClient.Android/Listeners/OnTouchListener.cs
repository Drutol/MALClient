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
    public class OnTouchListener : Java.Lang.Object, View.IOnTouchListener
    {
        private readonly Action<MotionEvent> _action;

        public OnTouchListener(Action<MotionEvent> action)
        {
            _action = action;
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            _action.Invoke(e);
            return true;
        }
    }
}
