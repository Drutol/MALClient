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
using Com.Orhanobut.Dialogplus;

namespace MALClient.Android.Listeners
{
    public class OnBackPressListener : Java.Lang.Object, IOnBackPressListener
    {
        private readonly Action _action;

        public OnBackPressListener(Action action)
        {
            _action = action;
        }

        public void OnBackPressed(DialogPlus p0)
        {
            _action.Invoke();
        }
    }
}