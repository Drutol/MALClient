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
    [Preserve(AllMembers = true)]
    public class ShareProvider : IShareProvider
    {
        public void Share(string message)
        {
            var activity = SimpleIoc.Default.GetInstance<Activity>();
            var share = new Intent(Intent.ActionSend);
            share.SetType("text/plain");
            share.PutExtra(Intent.ExtraText, message);

            activity.StartActivity(Intent.CreateChooser(share, "Share your anime endeavours!"));
        }
    }
}