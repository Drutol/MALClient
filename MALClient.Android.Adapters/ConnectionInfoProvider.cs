using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MALClient.Adapters;

namespace MALClient.Android.Adapters
{
    public class ConnectionInfoProvider : IConnectionInfoProvider
    {
        public void Init()
        {
            var connectivityManager =
                (ConnectivityManager) Application.Context.GetSystemService(Context.ConnectivityService);
            HasInternetConnection = connectivityManager.ActiveNetworkInfo != null &&
                                    connectivityManager.ActiveNetworkInfo.IsConnectedOrConnecting;
        }

        public bool HasInternetConnection { get; set; }
    }
}