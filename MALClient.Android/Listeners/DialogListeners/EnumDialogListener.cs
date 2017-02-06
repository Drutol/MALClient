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
using Object = Java.Lang.Object;

namespace MALClient.Android.Listeners.DialogListeners
{
    public class EnumDialogListener<TEnum> : Java.Lang.Object, IOnItemClickListener where TEnum : struct 
    {
        public Action<DialogPlus,TEnum> OnItemClickAction { get; set; }

        public void OnItemClick(DialogPlus p0, Object p1, View p2, int p3)
        {
            OnItemClickAction?.Invoke(p0,p1.EnumCast<TEnum>());
        }
    }
}