using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content.Res;
using Android.Util;
using Android.Views;
using Android.Widget;
using MALClient.Android.Activities;
using MALClient.XShared.Utils;
namespace MALClient.Android.Resources
{
    public static class ResourceExtension
    {
        public static readonly int BrushText = ResourcesCompat.GetColor(MainActivity.CurrentContext.Resources,
            Settings.SelectedTheme == 1 ? Resource.Color.DarkBrushText : Resource.Color.LightBrushText, null);

        public static readonly int AccentColour = ResourcesCompat.GetColor(MainActivity.CurrentContext.Resources,Resource.Color.AccentColour, null);

        public static readonly int AccentColourDark = ResourcesCompat.GetColor(MainActivity.CurrentContext.Resources,Resource.Color.AccentColourDark, null);

        public static readonly int BrushAnimeItemInnerBackground = ResourcesCompat.GetColor(MainActivity.CurrentContext.Resources,
            Settings.SelectedTheme == 1 ? Resource.Color.DarkBrushAnimeItemInnerBackground : Resource.Color.LightBrushAnimeItemInnerBackground, null);

        public static readonly int BrushAnimeItemBackground = ResourcesCompat.GetColor(MainActivity.CurrentContext.Resources,
            Settings.SelectedTheme == 1 ? Resource.Color.DarkBrushAnimeItemBackground : Resource.Color.LightBrushAnimeItemBackground, null);

        public static readonly int BrushAppBars = ResourcesCompat.GetColor(MainActivity.CurrentContext.Resources,
            Settings.SelectedTheme == 1 ? Resource.Color.DarkBrushAppBars : Resource.Color.LightBrushAppBars, null);

        public static readonly int BrushSelectedDialogItem = ResourcesCompat.GetColor(MainActivity.CurrentContext.Resources,
            Settings.SelectedTheme == 1 ? Resource.Color.DarkBrushSelectedDialogItem : Resource.Color.LightBrushSelectedDialogItem, null);

        public static readonly int BrushFlyoutBackground = ResourcesCompat.GetColor(MainActivity.CurrentContext.Resources,
            Settings.SelectedTheme == 1 ? Resource.Color.DarkBrushFlyoutBackground : Resource.Color.LightBrushFlyoutBackground, null);

        public static readonly int BrushRowAlternate1 = ResourcesCompat.GetColor(MainActivity.CurrentContext.Resources,
            Settings.SelectedTheme == 1 ? Resource.Color.DarkBrushRowAlternate1 : Resource.Color.LightBrushRowAlternate1, null);

        public static readonly int BrushRowAlternate2 = ResourcesCompat.GetColor(MainActivity.CurrentContext.Resources,
            Settings.SelectedTheme == 1 ? Resource.Color.DarkBrushRowAlternate2 : Resource.Color.LightBrushRowAlternate2, null);

        public static readonly int BrushRowAlternate1Res =
            Settings.SelectedTheme == 1 ? Resource.Color.DarkBrushRowAlternate1 : Resource.Color.LightBrushRowAlternate1;

        public static readonly int BrushRowAlternate2Res =
            Settings.SelectedTheme == 1 ? Resource.Color.DarkBrushRowAlternate2 : Resource.Color.LightBrushRowAlternate2;

        public static readonly string FontSizeLight =
            MainActivity.CurrentContext.Resources.GetString(Resource.String.font_family_light);

        public static int BrushTextRes = Settings.SelectedTheme == 1
            ? Resource.Color.DarkBrushText
            : Resource.Color.LightBrushText;

        public static int BrushAnimeItemBackgroundRes = Settings.SelectedTheme == 1
            ? Resource.Color.DarkBrushAnimeItemBackground
            : Resource.Color.LightBrushAnimeItemBackground;

        public static int BrushNoSearchResultsRes = Settings.SelectedTheme == 1
            ? Resource.Color.DarkBrushNoSearchResults
            : Resource.Color.LightBrushNoSearchResults;

        public static int BrushFlyoutBackgroundRes = Settings.SelectedTheme == 1
            ? Resource.Color.DarkBrushFlyoutBackground
            : Resource.Color.LightBrushFlyoutBackground;

        public static int BrushHamburgerBackgroundRes = Settings.SelectedTheme == 1
            ? Resource.Color.DarkBrushHamburgerBackground
            : Resource.Color.LightBrushHamburgerBackground;

        public static string AccentColourHex =
            MainActivity.CurrentContext.Resources.GetString(Resource.Color.AccentColour);

        public static string AccentColourLightHex =
            MainActivity.CurrentContext.Resources.GetString(Resource.Color.AccentColourLight);

        public static string AccentColourDarkHex =
            MainActivity.CurrentContext.Resources.GetString(Resource.Color.AccentColourDark);

        private static int? _selectableItemBackground;

        public static int SelectableItemBackground
        {
            get
            {
                if (_selectableItemBackground.HasValue)
                    return _selectableItemBackground.Value;
                TypedValue outValue = new TypedValue();
                MainActivity.CurrentContext.Theme.ResolveAttribute(Resource.Attribute.selectableItemBackground, outValue, true);
                _selectableItemBackground = outValue.ResourceId;
                return _selectableItemBackground.Value;
            }
        }

    }
}