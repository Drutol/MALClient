using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MALClient.Android.Flyouts
{
    public class TextFlyoutItem : TextBasedFlyoutBase
    {
        public TextFlyoutItem(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public TextFlyoutItem(int id,string text) : base(id)
        {
            Text = text;
        }
    }
}