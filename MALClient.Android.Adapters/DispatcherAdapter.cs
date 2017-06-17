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
using GalaSoft.MvvmLight.Ioc;
using MALClient.Adapters;

namespace MALClient.Android.Adapters
{
    public class DispatcherAdapter : IDispatcherAdapter
    {
        public void Run(Action action)
        {
            SimpleIoc.Default.GetInstance<Activity>().RunOnUiThread(action);
        }
    }
}