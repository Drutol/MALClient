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
using Com.Shehabic.Droppy;

namespace Com.Shehabic.Droppy
{
    public partial class DroppyMenuPopup
    {
        public static float RequestedElevation { get; set; } = 0;

        public void Show()
        {
            ShowBase();
            MenuView.Elevation = RequestedElevation;
        }
    }
}