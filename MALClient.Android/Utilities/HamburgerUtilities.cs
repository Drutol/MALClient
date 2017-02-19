using Com.Mikepenz.Materialdrawer.Model;
using MALClient.Android.Resources;

namespace MALClient.Android
{
    public static class HamburgerUtilities
    {
        public static PrimaryDrawerItem GetBasePrimaryItem()
        {
            var btn = new PrimaryDrawerItem();
            btn.WithIconTintingEnabled(true);
            btn.WithTextColorRes(ResourceExtension.BrushTextRes);
            btn.WithIconColorRes(ResourceExtension.BrushTextRes);
            btn.WithSelectedColorRes(ResourceExtension.BrushAnimeItemBackgroundRes);
            btn.WithSelectedTextColorRes(Resource.Color.AccentColour);
            btn.WithSelectedIconColorRes(Resource.Color.AccentColourDark);
            return btn;
        }

        public static SecondaryDrawerItem GetBaseSecondaryItem()
        {
            var btn = new SecondaryDrawerItem();
            btn.WithIconTintingEnabled(true);
            btn.WithTextColorRes(ResourceExtension.BrushTextRes);
            btn.WithIconColorRes(ResourceExtension.BrushTextRes);
            btn.WithSelectedColorRes(ResourceExtension.BrushAnimeItemBackgroundRes);
            btn.WithSelectedTextColorRes(Resource.Color.AccentColour);
            btn.WithSelectedIconColorRes(Resource.Color.AccentColourDark);
            return btn;
        }
    }
}