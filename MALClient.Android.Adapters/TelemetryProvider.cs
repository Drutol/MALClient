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
using HockeyApp.Android;
using HockeyApp.Android.Metrics;
using MALClient.Adapters;
using MALClient.Models.Enums;
using MALClient.XShared;

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
            CrashManager.Register(_context, Secrets.AndroidHockeyId);
            MetricsManager.Register(_app, Secrets.AndroidHockeyId);
            MetricsManager.EnableUserMetrics();
#endif
        }

        public void TelemetryTrackEvent(TelemetryTrackedEvents @event)
        {
#if !DEBUG
            MetricsManager.TrackEvent(@event.ToString());
#endif
        }

        public void TelemetryTrackEvent(TelemetryTrackedEvents @event, string arg)
        {
#if !DEBUG
            MetricsManager.TrackEvent($"{@event} {arg}");
#endif
        }

        public void LogEvent(string @event)
        {
#if !DEBUG
            MetricsManager.TrackEvent(@event);
#endif
        }

        public void TrackException(Exception e)
        {

        }

        public void TelemetryTrackNavigation(PageIndex page)
        {
#if !DEBUG
            //MetricsManager.TrackEvent($"Navigation: {page}");
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