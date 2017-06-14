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
using MALClient.XShared.ViewModels;

namespace MALClient.Android.Resources
{
    public static class ResourceExtension
    {
        public static void Init()
        {
            if (Settings.SelectedTheme == 1)
            {
                BrushAnimeItemInnerBackground = ResourcesCompat.GetColor(MainActivity.CurrentContext.Resources, Resource.Color.DarkBrushAnimeItemInnerBackground, null);
                BrushAnimeItemBackground = ResourcesCompat.GetColor(MainActivity.CurrentContext.Resources, Resource.Color.DarkBrushAnimeItemBackground, null);
                BrushAppBars = ResourcesCompat.GetColor(MainActivity.CurrentContext.Resources, Resource.Color.DarkBrushAppBars, null);
                BrushFlyoutBackground = ResourcesCompat.GetColor(MainActivity.CurrentContext.Resources, Resource.Color.DarkBrushFlyoutBackground, null);
                BrushRowAlternate1 = ResourcesCompat.GetColor(MainActivity.CurrentContext.Resources, Resource.Color.DarkBrushRowAlternate1, null);
                BrushRowAlternate2 = ResourcesCompat.GetColor(MainActivity.CurrentContext.Resources, Resource.Color.DarkBrushRowAlternate2, null);
                //
                BrushText = ResourcesCompat.GetColor(MainActivity.CurrentContext.Resources, Resource.Color.DarkBrushText, null);
                BrushSelectedDialogItem = ResourcesCompat.GetColor(MainActivity.CurrentContext.Resources, Resource.Color.DarkBrushSelectedDialogItem, null);
                BrushTextRes = Resource.Color.DarkBrushText;
                //
                BrushAnimeItemBackgroundRes = Resource.Color.DarkBrushAnimeItemBackground;
                BrushNoSearchResultsRes = Resource.Color.DarkBrushNoSearchResults;
                BrushNoSearchResults = ResourcesCompat.GetColor(MainActivity.CurrentContext.Resources, Resource.Color.DarkBrushNoSearchResults, null);
                BrushFlyoutBackgroundRes = Resource.Color.DarkBrushFlyoutBackground;
                BrushHamburgerBackgroundRes = Resource.Color.DarkBrushHamburgerBackground;
                BrushRowAlternate1Res = Resource.Color.DarkBrushRowAlternate1;
                BrushRowAlternate2Res = Resource.Color.DarkBrushRowAlternate2;
                BrushRowAlternate2LighterRes = Resource.Color.DarkBrushRowAlternate2;
                BrushAnimeItemInnerBackgroundRes = Resource.Color.DarkBrushAnimeItemInnerBackground;
            }
            else
            {
                BrushAnimeItemInnerBackground = ResourcesCompat.GetColor(MainActivity.CurrentContext.Resources, Resource.Color.LightBrushAnimeItemInnerBackground, null);
                BrushAnimeItemBackground = ResourcesCompat.GetColor(MainActivity.CurrentContext.Resources, Resource.Color.LightBrushAnimeItemBackground, null);
                BrushAppBars = ResourcesCompat.GetColor(MainActivity.CurrentContext.Resources, Resource.Color.LightBrushAppBars, null);
                BrushFlyoutBackground = ResourcesCompat.GetColor(MainActivity.CurrentContext.Resources, Resource.Color.LightBrushFlyoutBackground, null);
                BrushRowAlternate1 = ResourcesCompat.GetColor(MainActivity.CurrentContext.Resources, Resource.Color.LightBrushRowAlternate1, null);
                BrushRowAlternate2 = ResourcesCompat.GetColor(MainActivity.CurrentContext.Resources, Resource.Color.LightBrushRowAlternate2, null);
                //
                BrushText = ResourcesCompat.GetColor(MainActivity.CurrentContext.Resources, Resource.Color.LightBrushText, null);
                BrushSelectedDialogItem = ResourcesCompat.GetColor(MainActivity.CurrentContext.Resources, Resource.Color.LightBrushSelectedDialogItem, null);
                BrushTextRes = Resource.Color.LightBrushText;
                //
                BrushAnimeItemBackgroundRes = Resource.Color.LightBrushAnimeItemBackground;
                BrushNoSearchResultsRes = Resource.Color.LightBrushNoSearchResults;
                BrushNoSearchResults = ResourcesCompat.GetColor(MainActivity.CurrentContext.Resources, Resource.Color.LightBrushNoSearchResults, null);
                BrushFlyoutBackgroundRes = Resource.Color.LightBrushFlyoutBackground;
                BrushHamburgerBackgroundRes = Resource.Color.LightBrushHamburgerBackground;
                BrushRowAlternate1Res = Resource.Color.LightBrushRowAlternate1;
                BrushRowAlternate2Res = Resource.Color.LightBrushRowAlternate2;
                BrushRowAlternate2LighterRes = Resource.Color.LightBrushRowAlternate2Lighter;
                BrushAnimeItemInnerBackgroundRes = Resource.Color.LightBrushAnimeItemInnerBackground;

            }
            switch (AndroidColourThemeHelper.CurrentTheme)
            {
                case AndroidColorThemes.Orange:
                    AccentColour = ResourcesCompat.GetColor(MainActivity.CurrentContext.Resources,
                        Resource.Color.OrangeAccentColour, null);
                    AccentColourDark = ResourcesCompat.GetColor(MainActivity.CurrentContext.Resources,
                        Resource.Color.OrangeAccentColourDark, null);
                    AccentColourContrast = Settings.SelectedTheme == 1 ? AccentColour : AccentColourDark;
                    AccentColourHex =
                        MainActivity.CurrentContext.Resources.GetString(Resource.Color.OrangeAccentColour);
                    AccentColourLightHex =
                        MainActivity.CurrentContext.Resources.GetString(Resource.Color.OrangeAccentColourLight);
                    AccentColourDarkHex =
                        MainActivity.CurrentContext.Resources.GetString(Resource.Color.OrangeAccentColourDark);
                    AccentColourRes = Resource.Color.OrangeAccentColour;
                    AccentColourDarkRes = Resource.Color.OrangeAccentColourDark;
                    OpaqueAccentColourRes = Resource.Color.OrangeOpaqueAccentColour;
                    OpaqueAccentColour = ResourcesCompat.GetColor(MainActivity.CurrentContext.Resources,
                        Resource.Color.OrangeOpaqueAccentColour, null);
                    break;
                case AndroidColorThemes.Purple:
                    AccentColour = ResourcesCompat.GetColor(MainActivity.CurrentContext.Resources,
                        Resource.Color.PurpleAccentColour, null);
                    AccentColourDark = ResourcesCompat.GetColor(MainActivity.CurrentContext.Resources,
                        Resource.Color.PurpleAccentColourDark, null);
                    AccentColourContrast = Settings.SelectedTheme == 1 ? AccentColour : AccentColourDark;
                    AccentColourHex =
                        MainActivity.CurrentContext.Resources.GetString(Resource.Color.PurpleAccentColour);
                    AccentColourLightHex =
                        MainActivity.CurrentContext.Resources.GetString(Resource.Color.PurpleAccentColourLight);
                    AccentColourDarkHex =
                        MainActivity.CurrentContext.Resources.GetString(Resource.Color.PurpleAccentColourDark);
                    AccentColourRes = Resource.Color.PurpleAccentColour;
                    AccentColourDarkRes = Resource.Color.PurpleAccentColourDark;
                    OpaqueAccentColourRes = Resource.Color.PurpleOpaqueAccentColour;
                    OpaqueAccentColour = ResourcesCompat.GetColor(MainActivity.CurrentContext.Resources,
                        Resource.Color.PurpleOpaqueAccentColour, null);
                    break;
                case AndroidColorThemes.Blue:
                    AccentColour = ResourcesCompat.GetColor(MainActivity.CurrentContext.Resources,
                        Resource.Color.BlueAccentColour, null);
                    AccentColourDark = ResourcesCompat.GetColor(MainActivity.CurrentContext.Resources,
                        Resource.Color.BlueAccentColourDark, null);
                    AccentColourContrast = Settings.SelectedTheme == 1 ? AccentColour : AccentColourDark;
                    AccentColourHex =
                        MainActivity.CurrentContext.Resources.GetString(Resource.Color.BlueAccentColour);
                    AccentColourLightHex =
                        MainActivity.CurrentContext.Resources.GetString(Resource.Color.BlueAccentColourLight);
                    AccentColourDarkHex =
                        MainActivity.CurrentContext.Resources.GetString(Resource.Color.BlueAccentColourDark);
                    AccentColourRes = Resource.Color.BlueAccentColour;
                    AccentColourDarkRes = Resource.Color.BlueAccentColourDark;
                    OpaqueAccentColourRes = Resource.Color.BlueOpaqueAccentColour;
                    OpaqueAccentColour = ResourcesCompat.GetColor(MainActivity.CurrentContext.Resources,
                        Resource.Color.BlueOpaqueAccentColour, null);
                    break;
                case AndroidColorThemes.Lime:
                    AccentColour = ResourcesCompat.GetColor(MainActivity.CurrentContext.Resources,
                        Resource.Color.LimeAccentColour, null);
                    AccentColourDark = ResourcesCompat.GetColor(MainActivity.CurrentContext.Resources,
                        Resource.Color.LimeAccentColourDark, null);
                    AccentColourContrast = Settings.SelectedTheme == 1 ? AccentColour : AccentColourDark;
                    AccentColourHex =
                        MainActivity.CurrentContext.Resources.GetString(Resource.Color.LimeAccentColour);
                    AccentColourLightHex =
                        MainActivity.CurrentContext.Resources.GetString(Resource.Color.LimeAccentColourLight);
                    AccentColourDarkHex =
                        MainActivity.CurrentContext.Resources.GetString(Resource.Color.LimeAccentColourDark);
                    AccentColourRes = Resource.Color.LimeAccentColour;
                    AccentColourDarkRes = Resource.Color.LimeAccentColourDark;
                    OpaqueAccentColourRes = Resource.Color.LimeOpaqueAccentColour;
                    OpaqueAccentColour = ResourcesCompat.GetColor(MainActivity.CurrentContext.Resources,
                        Resource.Color.LimeOpaqueAccentColour, null);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #region Accents

        public static  int AccentColour;
        public static  int AccentColourDark;
        public static  int AccentColourContrast;
        public static  int OpaqueAccentColour;
        public static string AccentColourHex;
        public static string AccentColourLightHex;
        public static string AccentColourDarkHex;
        public static  int AccentColourRes;
        public static  int AccentColourDarkRes;
        public static  int OpaqueAccentColourRes;

        #endregion

        #region Background

        public static  int BrushAnimeItemInnerBackground;
        public static  int BrushAnimeItemBackground;
        public static  int BrushAppBars;
        public static  int BrushFlyoutBackground;
        public static  int BrushRowAlternate1;
        public static  int BrushRowAlternate2;


        #endregion

        #region Text


        public static  int BrushText;
        public static  int BrushSelectedDialogItem;
        public static int BrushNoSearchResults;

        public static  string FontSizeLight =
            MainActivity.CurrentContext.Resources.GetString(Resource.String.font_family_light);

        public static int BrushTextRes;

        #endregion

        #region ResourceIds

        public static int BrushAnimeItemBackgroundRes;
        public static int BrushNoSearchResultsRes;
        public static int BrushFlyoutBackgroundRes;
        public static int BrushHamburgerBackgroundRes;


        public static  int BrushRowAlternate1Res;
        public static  int BrushRowAlternate2Res;
        public static  int BrushRowAlternate2LighterRes;
        public static  int BrushAnimeItemInnerBackgroundRes;

        #endregion






        private static int? _selectableItemBackground;

        public static int SelectableItemBackground
        {
            get
            {
                if (_selectableItemBackground.HasValue)
                    return _selectableItemBackground.Value;
                TypedValue outValue = new TypedValue();
                MainActivity.CurrentContext.Theme.ResolveAttribute(Resource.Attribute.selectableItemBackground, outValue,
                    true);
                _selectableItemBackground = outValue.ResourceId;
                return _selectableItemBackground.Value;
            }
        }
    }
}