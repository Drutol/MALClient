using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MALClient.Adapters;
using MALClient.Models.Enums;
using MALClient.XShared;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace MALClient.Android.Adapters
{
    public class TelemetryProvider : ITelemetryProvider
    {
        private readonly Activity _context;
        private readonly Application _app;

        public TelemetryProvider(Activity context,Application app)
        {
            _context = context;
            _app = app;
        }

        public void Init()
        {
#if !DEBUG
            AppCenter.Start(Secrets.AndroidAppCenterKey,typeof(Crashes),typeof(Analytics));
#endif
        }

        public void TelemetryTrackEvent(TelemetryTrackedEvents @event)
        {
#if !DEBUG
            Analytics.TrackEvent(@event.ToString());
#endif
        }

        public void TelemetryTrackEvent(TelemetryTrackedEvents @event, params (string Key, string Param)[] args )
        {
#if !DEBUG
            Analytics.TrackEvent($"{@event}", args.ToDictionary(e => e.Key, e => e.Param));
#endif
        }

        public void LogEvent(string @event)
        {
#if !DEBUG
            Analytics.TrackEvent(@event);
#endif
        }

        public void TrackException(Exception e, [CallerMemberName] string caller = null)
        {
            Crashes.TrackError(e, new Dictionary<string, string>
            {
                {"Caller", caller}
            });
        }

        public void TelemetryTrackNavigation(PageIndex page)
        {
#if !DEBUG
            Analytics.TrackEvent("Navigation", new Dictionary<string, string>
            {
                {"Page", page.ToString()}
            });
#endif
        }

        public void TelemetryTrackNavigation(ForumsPageIndex page)
        {
#if !DEBUG
            //MetricsManager.TrackEvent($"Navigation: {page}");
#endif
        }
    }
}