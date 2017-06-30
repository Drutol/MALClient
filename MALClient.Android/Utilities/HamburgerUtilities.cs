using System;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Com.Mikepenz.Materialdrawer.Model;
using Com.Mikepenz.Materialdrawer.Model.Interfaces;
using MALClient.Android.Listeners;
using MALClient.Android.Resources;
using MALClient.Models.Enums;
using Object = Java.Lang.Object;

namespace MALClient.Android
{
    public static class HamburgerUtilities
    {
        class BindListener : Java.Lang.Object, IOnPostBindViewListener
        {
            private readonly Action<View> _moreButtonAction;


            public BindListener(Action<View> moreButtonAction)
            {
                _moreButtonAction = moreButtonAction;
            }

            public void OnBindView(IDrawerItem p0, View p1)
            {
                p1.SetPadding(DimensionsHelper.DpToPx(16), 0, 0, 0);
                if (p1.FindViewById(123098) != null)
                    return;

                var param = new LinearLayout.LayoutParams(DimensionsHelper.DpToPx(50), -1);
                param.SetMargins(DimensionsHelper.DpToPx(10),0,0,0);

                var view = new FrameLayout(p1.Context)
                {
                    Clickable = true,
                    Focusable = true,
                    Id = 123098,
                    LayoutParameters = param
                };

                view.SetBackgroundResource(ResourceExtension.SelectableBorderlessItemBackground);
                var img = new ImageView(p1.Context);
                img.SetScaleType(ImageView.ScaleType.Center);
                img.SetImageResource(Resource.Drawable.icon_more_vertical);
                img.ImageTintList = global::Android.Content.Res.ColorStateList.ValueOf(new Color(ResourceExtension.BrushText));

                view.AddView(img);

                view.SetOnClickListener(new OnClickListener(_moreButtonAction));

                (p1 as ViewGroup).AddView(view);
            }
        }

        public static PrimaryDrawerItem GetBasePrimaryItem(Action<View> moreButtonAction = null)
        {
            var btn = new PrimaryDrawerItem();
            btn.WithIconTintingEnabled(true);
            btn.WithTextColorRes(ResourceExtension.BrushTextRes);
            btn.WithIconColorRes(ResourceExtension.BrushTextRes);
            btn.WithSelectedColorRes(ResourceExtension.BrushAnimeItemBackgroundRes);
            btn.WithSelectedTextColorRes(ResourceExtension.AccentColourRes);
            btn.WithSelectedIconColorRes(ResourceExtension.AccentColourDarkRes);
            if (moreButtonAction != null)
                btn.WithPostOnBindViewListener(new BindListener(moreButtonAction));
            return btn;
        }

        public static SecondaryDrawerItem GetBaseSecondaryItem()
        {
            var btn = new SecondaryDrawerItem();
            btn.WithIconTintingEnabled(true);
            btn.WithTextColorRes(ResourceExtension.BrushTextRes);
            btn.WithIconColorRes(ResourceExtension.BrushTextRes);
            btn.WithSelectedColorRes(ResourceExtension.BrushAnimeItemBackgroundRes);
            btn.WithSelectedTextColorRes(ResourceExtension.AccentColourRes);
            btn.WithSelectedIconColorRes(ResourceExtension.AccentColourDarkRes);
            return btn;
        }
    }
}