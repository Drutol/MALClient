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
    public class OnLongClickListener : Java.Lang.Object , View.IOnLongClickListener
    {
        private readonly Action<View> _onLongClickAction;

        public OnLongClickListener(Action<View> onLongClickAction)
        {
            _onLongClickAction = onLongClickAction;
        }

        public bool OnLongClick(View v)
        {
            _onLongClickAction.Invoke(v);
            return true;
        }
    }
}