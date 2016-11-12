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

namespace MALClient.Android.Listeners.DialogListeners
{
    public class DialogDismissedListener : Java.Lang.Object, IOnDismissListener
    {
        private readonly Action _onDismissAction;

        public DialogDismissedListener(Action action)
        {
            _onDismissAction = action;
        }

        public void OnDismiss(DialogPlus p0)
        {
            _onDismissAction.Invoke();
        }
    }
}