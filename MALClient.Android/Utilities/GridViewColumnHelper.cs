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
using MALClient.XShared.Utils;

namespace MALClient.Android
{
    public class GridViewColumnHelper
    {
        private readonly List<GridView> _grids;

        public GridViewColumnHelper(GridView view)
        {
            _grids = new List<GridView> {view};
            OnConfigurationChanged(MainActivity.CurrentContext.Resources.Configuration);
        }

        public GridViewColumnHelper()
        {
            _grids = new List<GridView>();
        }

        public void RegisterGrid(GridView view)
        {
            _grids.Add(view);
            view.SetNumColumns(GetColumns(MainActivity.CurrentContext.Resources.Configuration));
        }

        private int GetColumns(Configuration newConfig)
        {
            var width = newConfig.ScreenWidthDp;
            //width = width > 200 ? 200 : width;
            var columns = (int)(width / DimensionsHelper.PxToDp(300));
            columns = columns < 2 ? 2 : columns;
            return columns;
        }

        public void OnConfigurationChanged(Configuration newConfig)
        {
            var columns = GetColumns(newConfig);
            _grids.ForEach(grid =>
            {
                grid.SetNumColumns(columns);
            });
        }
    }
}