using System;
using Android.Runtime;

namespace MALClient.Android.Flyouts.FlyoutMenus
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