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
        private readonly int _prefferedItemWidth;
        private readonly int _minColumns;
        private static readonly int PrefferedItemWidth = MainActivity.CurrentContext.Resources.DisplayMetrics.Density >= 2 ? 190 : 200;


        public GridViewColumnHelper(GridView view,int? prefferedWidthDp = null,int? minCollumns = null)
        {
            _prefferedItemWidth = prefferedWidthDp ?? PrefferedItemWidth;
            _minColumns = minCollumns ?? 2;
            _grids = new List<GridView> {view};
            OnConfigurationChanged(MainActivity.CurrentContext.Resources.Configuration);
        }

        public GridViewColumnHelper(int? prefferedWidthDp = null)
        {
            _prefferedItemWidth = prefferedWidthDp ?? PrefferedItemWidth;
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
            var columns = width / _prefferedItemWidth;
            columns = columns < _minColumns ? _minColumns : columns;
            return columns;
        }

        private void UpdateGrid(GridView grid,int columns,Configuration config)
        {
            grid.SetNumColumns(columns);
            var param = grid.LayoutParameters;
            param.Width = DimensionsHelper.DpToPx(_prefferedItemWidth) * columns;
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