using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Android.Content.Res;
using Android.Views;
using Android.Widget;
using MALClient.Android.Activities;
using MALClient.XShared.Comm.Anime;

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
            if(_grids.Contains(view))
                return;
            _grids.Add(view);
            var config = MainActivity.CurrentContext.Resources.Configuration;
            UpdateGrid(view,GetColumns(config),config);
        }

        private int GetColumns(Configuration newConfig)
        {
            var width = newConfig.ScreenWidthDp;
            var columns = (int)(width / DimensionsHelper.PxToDp(DimensionsHelper.DpToPx(PrefferedItemWidth)));
            columns = columns < 2 ? 2 : columns;
            return columns;
        }

        private void UpdateGrid(GridView grid,int columns,Configuration config)
        {
            grid.SetNumColumns(columns);
            var param = grid.LayoutParameters;
            param.Width = DimensionsHelper.DpToPx(PrefferedItemWidth) * columns;
            if (param.Width < 0)
                param.Width = ViewGroup.LayoutParams.MatchParent;
            if(param.Width > DimensionsHelper.DpToPx(config.ScreenWidthDp))
                param.Width = ViewGroup.LayoutParams.MatchParent;
            grid.LayoutParameters = param;
        }

        public void OnConfigurationChanged(Configuration newConfig)
        {
            var columns = GetColumns(newConfig);
            _grids.ForEach(grid =>
            {
                UpdateGrid(grid,columns,newConfig);
            });
        }
    }
}