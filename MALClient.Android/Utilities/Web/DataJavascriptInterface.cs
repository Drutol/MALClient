using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Java.Interop;

namespace MALClient.Android.Web
{
    public class DataJavascriptInterface : Java.Lang.Object
    {
        private Context _context;
        private string _lastResponse;

        public DataJavascriptInterface(Context context)
        {
            _context = context;
        }

        public event EventHandler<string> NewResponse;

        [Export("OnData")]
        [JavascriptInterface]
        public void OnData(string value)
        {
            NewResponse?.Invoke(this,value.ToString());
        }
    }
}