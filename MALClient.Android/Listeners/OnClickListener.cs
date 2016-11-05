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
    public class OnClickListener : Java.Lang.Object , View.IOnClickListener
    {
        private readonly Action<View> _action;

        public OnClickListener(Action<View> action)
        {
            _action = action;
        }

        public void OnClick(View v)
        {
            _action.Invoke(v);
        }
    }
}