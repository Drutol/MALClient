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
using Com.Shehabic.Droppy.Views;
using MALClient.Android.Resources;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Flyouts
{
    public static class FlyoutMenuBuilder
    {
        public static DroppyMenuPopup BuildGenericFlyout(Context context, View parent, List<string> items, Action<int> callback,string header = null)
        {
            AnimeListPageFlyoutBuilder.ParamRelativeLayout = new ViewGroup.LayoutParams(DimensionsHelper.DpToPx(200), DimensionsHelper.DpToPx(38));

            var droppyBuilder = new DroppyMenuPopup.Builder(context, parent);
            AnimeListPageFlyoutBuilder.InjectAnimation(droppyBuilder);

            if (header != null)
            {
                droppyBuilder.AddMenuItem(new DroppyMenuCustomItem(AnimeListPageFlyoutBuilder.BuildItem(context, header,i => {}, 0, ResourceExtension.BrushRowAlternate2, null, false, GravityFlags.CenterHorizontal)));
                droppyBuilder.AddMenuItem(new DroppyMenuCustomItem(new DroppyMenuSeparatorView(context)));
            }

            for (int i = 0; i < items.Count; i++)
                droppyBuilder.AddMenuItem(new DroppyMenuCustomItem(AnimeListPageFlyoutBuilder.BuildItem(context, items[i], callback, i)));
            
            return droppyBuilder.Build();
        }
    }
}