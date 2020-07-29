using System;
using System.Collections.Generic;
using Android.Content;
using Android.Graphics;
using Android.Support.V4.Content.Res;

namespace MALClient.Android
{
    public static class FontManager
    {
        public const string TypefacePath =  "fonts/fontawesome-webfont.ttf";

        private static readonly Dictionary<string, Typeface> CacheTypefaces =new Dictionary<string, Typeface>();

        public static Typeface GetTypeface(Context context, string font)
        {
            if (CacheTypefaces.ContainsKey(font))
                return CacheTypefaces[font];

            var typeFace = ResourcesCompat.GetFont(context, Resource.Font.fontawesomewebfont);

            CacheTypefaces.Add(font,typeFace);

            return typeFace;
        }

    }
}