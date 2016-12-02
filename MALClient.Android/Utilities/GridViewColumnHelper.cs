using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MALClient.Android.Activities;

namespace MALClient.Android
{
    public class GridViewColumnHelper
    {
        private readonly GridView _view;
        private static readonly int TwoHundred = DimensionsHelper.DpToPx(200);

        public GridViewColumnHelper(GridView view)
        {
            _view = view;
            OnConfigurationChanged(MainActivity.CurrentContext.Resources.Configuration);
        }

        public void OnConfigurationChanged(Configuration newConfig)
        {
            var width = newConfig.ScreenWidthDp / DimensionsHelper.PxToDp(2.1f);
            width = width > TwoHundred ? TwoHundred : width;
            _view.SetColumnWidth((int)width);
        }
    }
}