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
    public class DateSetListener : Java.Lang.Object, global::Android.App.DatePickerDialog.IOnDateSetListener
    {
        private Action<int, int, int> _callback;

        public DateSetListener(Action<int, int, int> callback)
        {
            _callback = callback;
        }


        public void OnDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth)
        {
            _callback.Invoke(year, monthOfYear+1, dayOfMonth);
        }
    }
}