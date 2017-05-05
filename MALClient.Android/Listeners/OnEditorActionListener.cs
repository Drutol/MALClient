using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;

namespace MALClient.Android.Listeners
{
    public class OnEditorActionListener : Java.Lang.Object, TextView.IOnEditorActionListener
    {
        private readonly Action<ImeAction> _action;

        public OnEditorActionListener(Action<ImeAction> action)
        {
            _action = action;
        }

        public bool OnEditorAction(TextView v, ImeAction actionId, KeyEvent e)
        {
            _action.Invoke(actionId);
            return false;
        }
    }
}