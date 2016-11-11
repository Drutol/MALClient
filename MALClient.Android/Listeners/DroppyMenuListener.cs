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
using Com.Shehabic.Droppy;

namespace MALClient.Android.Listeners
{
    public class DroppyMenuListener : Java.Lang.Object,IDroppyClickCallbackInterface
    {
        private Action<int> _action;

        public DroppyMenuListener(Action<int> action)
        {
            _action = action;
        }

        public void Call(View p0, int p1)
        {
            _action.Invoke(p1);
        }
    }

    public class DroppyMenuDissmissListener : Java.Lang.Object, DroppyMenuPopup.IOnDismissCallback
    {
        private Action _action;

        public DroppyMenuDissmissListener(Action action)
        {
            _action = action;
        }

        public void Call()
        {
            _action.Invoke();
        }
    }
}