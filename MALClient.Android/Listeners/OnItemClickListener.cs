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

namespace MALClient.Android.Listeners
{
    public class OnItemClickListener<T> : Java.Lang.Object , ListView.IOnItemClickListener where T : class
    {
        private readonly Action<T> _callback;

        public OnItemClickListener(Action<T> callback)
        {
            _callback = callback;
        }

        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            _callback.Invoke(view.Tag.Unwrap<T>());
        }
    }
}