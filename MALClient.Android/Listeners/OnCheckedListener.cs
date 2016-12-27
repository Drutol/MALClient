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
    public class OnCheckedListener : Java.Lang.Object,RadioGroup.IOnCheckedChangeListener
    {
        private readonly Action<int> _callback;

        public OnCheckedListener(Action<int> callback)
        {
            _callback = callback;
        }

        public void OnCheckedChanged(RadioGroup group, int checkedId)
        {
            _callback.Invoke(checkedId);
        }
    }
}