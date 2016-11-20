
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using MALClient.Android.Activities;

namespace MALClient.Android
{
    public static class DimensionsHelper
    {
        public static int DpToPx(float dp)
        {
            return (int) (dp* MainActivity.CurrentContext.Resources.DisplayMetrics.Density);
        }

        public static float PxToDp(float px)
        {
            return (px / MainActivity.CurrentContext.Resources.DisplayMetrics.Density);
        }
    }
}