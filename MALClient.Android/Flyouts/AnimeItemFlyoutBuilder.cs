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
using MALClient.XShared.ViewModels;

namespace MALClient.Android.Flyouts
{
    public static class AnimeItemFlyoutBuilder
    {
        public static PopupMenu BuildForAnimeItem(Context context, View parent, AnimeItemViewModel viewModel, Action<AnimeGridItemMoreFlyoutButtons> callback,bool forceSmall = false)
        {
            var menu = new PopupMenu(context, parent);

            menu.Menu.Add(0,0,0,"Copy url");
            menu.Menu.Add(0,5,0,"Copy title");
            menu.Menu.Add(0,1,0,"Open in browser");
            if (!forceSmall && viewModel.Auth)
            {
                menu.Menu.Add(0,2,0,"Set status");
                menu.Menu.Add(0,3,0,"Set score");
                menu.Menu.Add(0,4,0,"Set watched");
            }
            menu.SetOnMenuItemClickListener(new MenuListener(item =>
            {
                callback.Invoke((AnimeGridItemMoreFlyoutButtons)item.ItemId);
            }));
            return menu;

            //AnimeListPageFlyoutBuilder.ParamRelativeLayout = new ViewGroup.LayoutParams(DimensionsHelper.DpToPx(150), DimensionsHelper.DpToPx(38));

            //var droppyBuilder = new DroppyMenuPopup.Builder(context, parent);
            //AnimeListPageFlyoutBuilder.InjectAnimation(droppyBuilder);


            //var listener = new Action<int>(i => callback.Invoke((AnimeGridItemMoreFlyoutButtons)i));

            ////droppyBuilder.AddMenuItem(new DroppyMenuCustomItem(AnimeListPageFlyoutBuilder.BuildItem(context, "Copy url", listener, 0)));
            ////droppyBuilder.AddMenuItem(new DroppyMenuCustomItem(AnimeListPageFlyoutBuilder.BuildItem(context, "Copy title", listener, 5)));
            ////droppyBuilder.AddMenuItem(new DroppyMenuCustomItem(AnimeListPageFlyoutBuilder.BuildItem(context, "Open in browser", listener, 1)));
            ////if (!forceSmall && viewModel.Auth)
            ////{
            ////    droppyBuilder.AddSeparator();
            ////    droppyBuilder.AddMenuItem(new DroppyMenuCustomItem(AnimeListPageFlyoutBuilder.BuildItem(context, "Set status", listener, 2)));
            ////    droppyBuilder.AddMenuItem(new DroppyMenuCustomItem(AnimeListPageFlyoutBuilder.BuildItem(context, "Set score", listener, 3)));
            ////    droppyBuilder.AddMenuItem(new DroppyMenuCustomItem(AnimeListPageFlyoutBuilder.BuildItem(context, "Set watched", listener, 4)));
            ////}
            //return droppyBuilder.Build();
        }

        public class MenuListener : Java.Lang.Object , PopupMenu.IOnMenuItemClickListener
        {
            private readonly Action<IMenuItem> _action;

            public MenuListener(Action<IMenuItem> action)
            {
                _action = action;
            }

            public bool OnMenuItemClick(IMenuItem item)
            {
                _action(item);
                return true;
            }
        }

        public static DroppyMenuPopup BuildForAnimeItemTags(Context context, View parent, AnimeItemViewModel viewModel, Action callback)
        {
            AnimeListPageFlyoutBuilder.ParamRelativeLayout = new ViewGroup.LayoutParams(DimensionsHelper.DpToPx(150), DimensionsHelper.DpToPx(38));

            var droppyBuilder = new DroppyMenuPopup.Builder(context, parent);
            AnimeListPageFlyoutBuilder.InjectAnimation(droppyBuilder);


            var listener = new Action<int>(i => callback.Invoke());

            droppyBuilder.AddMenuItem(new DroppyMenuCustomItem(AnimeListPageFlyoutBuilder.BuildItem(context, "Tags:", listener, 0,ResourceExtension.BrushRowAlternate2,null,false,GravityFlags.CenterHorizontal)));
            droppyBuilder.AddMenuItem(new DroppyMenuCustomItem(new DroppyMenuSeparatorView(context)));
            droppyBuilder.AddMenuItem(new DroppyMenuCustomItem(AnimeListPageFlyoutBuilder.BuildItem(context, viewModel.Notes , listener, 1, null, null, false, GravityFlags.CenterHorizontal,true)));

            return droppyBuilder.Build();
        }
    }
}