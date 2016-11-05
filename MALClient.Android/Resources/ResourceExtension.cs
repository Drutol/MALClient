using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content.Res;
using Android.Views;
using Android.Widget;
using MALClient.Android.Activities;

namespace MALClient.Android.Resources
{
    public static class ResourceExtension
    {
        public static readonly int BrushText = ResourcesCompat.GetColor(MainActivity.CurrentContext.Resources,
            Resource.Color.BrushText, null);

        public static readonly int AccentColour = ResourcesCompat.GetColor(MainActivity.CurrentContext.Resources,
            Resource.Color.AccentColour, null);
    }
}