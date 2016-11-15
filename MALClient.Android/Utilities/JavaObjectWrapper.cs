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

namespace MALClient.Android
{
    public class JavaObjectWrapper<TObj> : Java.Lang.Object where TObj : class
    {
        public TObj Instance { get; }

        public JavaObjectWrapper(TObj obj)
        {
            Instance = obj;
        }
    }
}