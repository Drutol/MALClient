using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Mikepenz.Fontawesome_typeface_library;
using Com.Mikepenz.Iconics.Typeface;
using MALClient.Models.Enums;

namespace MALClient.Android.BindingConverters
{
    public static class FontAwesomeToIconicsIcon
    {
        private static TypeInfo _typeInfo = typeof(FontAwesome.Icon).GetTypeInfo();

        public static IIcon Convert(FontAwesomeIcon icon)
        {
            return (IIcon)(_typeInfo.GetDeclaredProperty($"Faw{icon.ToString()}")).GetConstantValue();
        }
    }
}