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

namespace Com.Astuetz
{
    public partial class PagerSlidingTabStrip
    {
        static IntPtr id_setAllCaps_Z;
        // Metadata.xml XPath method reference: path="/api/package[@name='com.astuetz']/class[@name='PagerSlidingTabStrip']/method[@name='setAllCaps' and count(parameter)=1 and parameter[1][@type='boolean']]"
        [Register("setAllCaps", "(Z)V", "GetSetAllCaps_ZHandler")]
        public virtual unsafe void SetAllCaps(bool textAllCaps)
        {
            if (id_setAllCaps_Z == IntPtr.Zero)
                id_setAllCaps_Z = JNIEnv.GetMethodID(class_ref, "setAllCaps", "(Z)V");
            try
            {
                JValue* __args = stackalloc JValue[1];
                __args[0] = new JValue(textAllCaps);

                if (((object)this).GetType() == ThresholdType)
                    JNIEnv.CallVoidMethod(((global::Java.Lang.Object)this).Handle, id_setAllCaps_Z, __args);
                else
                    JNIEnv.CallNonvirtualVoidMethod(((global::Java.Lang.Object)this).Handle, ThresholdClass, JNIEnv.GetMethodID(ThresholdClass, "setAllCaps", "(Z)V"), __args);
            }
            finally
            {
            }
        }
    }
}