using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content.Res;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Shehabic.Droppy;
using Com.Shehabic.Droppy.Animations;
using MALClient.Android.Activities;
using MALClient.Android.Listeners;
using MALClient.Android.Resources;
using MALClient.Models.Enums;
using MALClient.Models.Enums.Enums;
using MALClient.XShared.Utils;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.Flyouts
{
    public enum AnimeGridItemMoreFlyoutButtons
    {
        CopyLink,
        OpenInBrowser,
        SetStatus,
        SetRating,
        SetWatched
    }

    public static class AnimeListPageFlyoutBuilder
    {
        public static RelativeLayout.LayoutParams ParamTextView { get; }
        public static ViewGroup.LayoutParams ParamRelativeLayout { get; set; }

        static AnimeListPageFlyoutBuilder()
        {
            ParamTextView = new RelativeLayout.LayoutParams(-1,-2); //wrap content
            ParamTextView.AddRule(LayoutRules.AlignParentLeft);
            ParamTextView.AddRule(LayoutRules.CenterVertical);
            ParamTextView.LeftMargin = 10;
        }

        public static void InjectAnimation(DroppyMenuPopup.Builder builder)
        {
            builder.SetPopupAnimation(new DroppyFadeInAnimation());
            builder.TriggerOnAnchorClick(false);
            builder.SetXOffset(5);
            builder.SetYOffset(5);
        }

        public static View BuildItem(Context context,string text,Action<int> callback,int id,int? background = null,int? foreground = null)
        {
            background = background ?? ResourceExtension.BrushFlyoutBackground;
            foreground = foreground ?? ResourceExtension.BrushText;

            var top = new FrameLayout(context);

            top.SetBackgroundColor(new Color(background.Value));
            var holder = new RelativeLayout(context) {LayoutParameters = ParamRelativeLayout};

            holder.SetBackgroundResource(ResourceExtension.SelectableItemBackground);

            holder.Clickable = true;
            holder.Focusable = true;

            var txt = new TextView(context) {LayoutParameters = ParamTextView};
            txt.SetTextColor(new Color(foreground.Value));
            txt.Text = text;

            holder.AddView(txt);
            top.AddView(holder);

            holder.Click += (sender, args) => callback.Invoke(id);

            return top;
        }

        

        public static DroppyMenuPopup BuildForAnimeStatusSelection(Context context, View parent,
            Action<AnimeStatus> callback,AnimeStatus currentStatus,bool manga)
        {
            ParamRelativeLayout = new ViewGroup.LayoutParams(300, 70);

            var droppyBuilder = new DroppyMenuPopup.Builder(context, parent);
            InjectAnimation(droppyBuilder);

            var listener = new Action<int>(i => callback.Invoke((AnimeStatus)i));

            foreach (AnimeStatus value in Enum.GetValues(typeof(AnimeStatus)))
            {
                if (value == currentStatus)
                    droppyBuilder.AddMenuItem(
                        new DroppyMenuCustomItem(BuildItem(context, Utilities.StatusToString((int)value,manga), listener, (int) value,
                            ResourceExtension.BrushSelectedDialogItem, ResourceExtension.AccentColour)));
                else //highlighted
                    droppyBuilder.AddMenuItem(
                        new DroppyMenuCustomItem(BuildItem(context, Utilities.StatusToString((int)value), listener, (int) value)));
            }

            return droppyBuilder.Build();
        }

        public static DroppyMenuPopup BuildForAnimeSortingSelection(Context context, View parent,
            Action<SortOptions> callback,SortOptions currentOption)
        {
            ParamRelativeLayout = new ViewGroup.LayoutParams(300, 70);

            var droppyBuilder = new DroppyMenuPopup.Builder(context, parent);
            InjectAnimation(droppyBuilder);


            var listener = new Action<int>(i => callback.Invoke((SortOptions)i));

            foreach (SortOptions value in Enum.GetValues(typeof(SortOptions)))
            {
                if (value == currentOption)
                    droppyBuilder.AddMenuItem(
                        new DroppyMenuCustomItem(BuildItem(context, value.GetDescription(), listener, (int) value,
                            ResourceExtension.BrushSelectedDialogItem, ResourceExtension.AccentColour)));
                else //highlighted
                    droppyBuilder.AddMenuItem(
                        new DroppyMenuCustomItem(BuildItem(context, value.GetDescription(), listener, (int) value)));
            }

            return droppyBuilder.Build();
        }

        public static DroppyMenuPopup BuildForAnimeListDisplayModeSelection(Context context, View parent,IEnumerable<Tuple<AnimeListDisplayModes,string>> items,
            Action<AnimeListDisplayModes> callback, AnimeListDisplayModes currentMode)
        {
            ParamRelativeLayout = new ViewGroup.LayoutParams(300, 70);

            var droppyBuilder = new DroppyMenuPopup.Builder(context, parent);
            InjectAnimation(droppyBuilder);


            var listener = new Action<int>(i => callback.Invoke((AnimeListDisplayModes)i));

            foreach (var item in items)
            {
                if (item.Item1 == currentMode)
                    droppyBuilder.AddMenuItem(
                        new DroppyMenuCustomItem(BuildItem(context, item.Item2, listener, (int)item.Item1,
                            ResourceExtension.BrushSelectedDialogItem, ResourceExtension.AccentColour)));
                else //highlighted
                    droppyBuilder.AddMenuItem(
                        new DroppyMenuCustomItem(BuildItem(context, item.Item2, listener, (int)item.Item1)));
            }

            return droppyBuilder.Build();
        }
    }    
}