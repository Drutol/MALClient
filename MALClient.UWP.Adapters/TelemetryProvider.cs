using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MALClient.Adapters;
using MALClient.Models.Enums;
using MALClient.XShared.Utils;

namespace MALClient.UWP.Adapters
{
    public class TelemetryProvider : ITelemetryProvider
    {
        public void Init()
        {
#if !DEBUG
            HockeyClient.Current.Configure("b79e78858bdf44c4bfc3a1f37c8fd90c", new TelemetryConfiguration
            {
                Collectors = WindowsCollectors.Metadata | WindowsCollectors.Session | WindowsCollectors.UnhandledException,
            });
#endif
        }

        public void TelemetryTrackEvent(TelemetryTrackedEvents @event)
        {
#if !DEBUG
            //HockeyClient.Current.TrackEvent(@event.ToString());
#endif
        }

        public void TelemetryTrackEvent(TelemetryTrackedEvents @event, string arg)
        {
#if !DEBUG
            //HockeyClient.Current.TrackEvent(@event.ToString() + " " + arg);
#endif
        }
    }
}
