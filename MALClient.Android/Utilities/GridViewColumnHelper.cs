using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Android.Content.Res;
using Android.Views;
using Android.Widget;
using MALClient.Android.Activities;
using MALClient.XShared.Comm.Anime;
using Orientation = Android.Content.Res.Orientation;

namespace MALClient.Android
{
    public class GridViewColumnHelper
    {
        private static readonly int DefaultPrefferedItemWidth = MainActivity.CurrentContext.Resources.DisplayMetrics.Density >= 2 ? 190 : 200;
        private readonly List<GridView> _grids;

        public int LastColmuns { get;  private set; }

        public int MinColumns { get; set; }
        public int? MinColumnsLandscape { get; set; }
        public int MinColumnsPortrait { get; set; }

        public int PrefferedItemWidth { get; set; }

        public GridViewColumnHelper(GridView view,int? prefferedWidthDp = null,int? minColumnsPortrait = null,int? minColumnsInLandscape = null)
        {
            PrefferedItemWidth = prefferedWidthDp ?? DefaultPrefferedItemWidth;
            MinColumnsPortrait = minColumnsPortrait ?? 2;
            MinColumnsLandscape = minColumnsInLandscape;
            _grids = new List<GridView> {view};
            OnConfigurationChanged(MainActivity.CurrentContext.Resources.Configuration);
        }

        public GridViewColumnHelper(int? prefferedWidthDp = null)
        {
            PrefferedItemWidth = prefferedWidthDp ?? DefaultPrefferedItemWidth;
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
            var columns = width / PrefferedItemWidth;
            columns = columns < MinColumns ? MinColumns : columns;
            LastColmuns = columns;
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
            if (newConfig.Orientation == Orientation.Landscape && MinColumnsLandscape.HasValue)
                MinColumns = MinColumnsLandscape.Value;
            else
                MinColumns = MinColumnsPortrait;

            var columns = GetColumns(newConfig);
            _grids.ForEach(grid =>
            {
                UpdateGrid(grid,columns,newConfig);
            });
        }
    }
}