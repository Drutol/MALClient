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
    public class IntegerDialogListener : Java.Lang.Object , IOnItemClickListener
    {
        public Action<DialogPlus, float> OnItemClickAction { get; set; }

        public void OnItemClick(DialogPlus p0, Java.Lang.Object p1, View p2, int p3)
        {
            OnItemClickAction.Invoke(p0,(float)p2.Tag);
        }
    }
}