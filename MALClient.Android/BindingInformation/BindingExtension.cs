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
using MALClient.Android.CollectionAdapters;

namespace MALClient.Android.BindingInformation
{
    public static class BindingExtension
    {
        public static void SetBinding<T>(this View view,IStaticBindingInfo<T> info,T model)
        {
            info.Bind(view, model);
        }
    }
}