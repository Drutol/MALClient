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
using MALClient.XShared.ViewModels;

namespace MALClient.Android.Resources
{
    public enum AndroidColorThemes
    {
        Orange,
        Purple,
        Blue,
        Lime,
        Pink
    }

    public static class AndroidColourThemeHelper
    {
        public static AndroidColorThemes CurrentTheme
        {
            get
            {
                return
                    (AndroidColorThemes)
                    (ResourceLocator.ApplicationDataService[nameof(AndroidColorThemes)] ?? AndroidColorThemes.Orange);
            }
            set { ResourceLocator.ApplicationDataService[nameof(AndroidColorThemes)] = (int) value; }
        }
    }
}