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
        private static readonly int PrefferedItemWidth = MainActivity.CurrentContext.Resources.DisplayMetrics.Density >= 2 ? 190 : 200;

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
            UpdateGrid(view,GetColumns(MainActivity.CurrentContext.Resources.Configuration));
        }

        private int GetColumns(Configuration newConfig)
        {
            var width = newConfig.ScreenWidthDp;
            var columns = (int)(width / DimensionsHelper.PxToDp(DimensionsHelper.DpToPx(PrefferedItemWidth)));
            columns = columns < 2 ? 2 : columns;
            return columns;
        }

        private void UpdateGrid(GridView grid,int columns)
        {
            grid.SetNumColumns(columns);
            var param = grid.LayoutParameters;
            param.Width = DimensionsHelper.DpToPx(PrefferedItemWidth) * columns;
            if (param.Width < 0)
                param.Width = ViewGroup.LayoutParams.MatchParent;

            grid.LayoutParameters = param;
        }

        public void OnConfigurationChanged(Configuration newConfig)
        {
            var columns = GetColumns(newConfig);
            _grids.ForEach(grid =>
            {
                UpdateGrid(grid,columns);
            });
        }
    }
}