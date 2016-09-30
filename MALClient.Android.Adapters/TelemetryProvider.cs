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
using MALClient.Adapters;
using MALClient.Models.Enums;

namespace MALClient.Android.Adapters
{
    public class TelemetryProvider : ITelemetryProvider
    {
        public void Init()
        {
           // throw new NotImplementedException();
        }

        public void TelemetryTrackEvent(TelemetryTrackedEvents @event)
        {
           // throw new NotImplementedException();
        }

        public void TelemetryTrackEvent(TelemetryTrackedEvents @event, string arg)
        {
           // throw new NotImplementedException();
        }
    }
}