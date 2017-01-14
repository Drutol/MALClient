using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MALClient.Models.Enums;

namespace MALClient.Adapters
{
    public interface ITelemetryProvider
    {
        void Init();
        void TelemetryTrackEvent(TelemetryTrackedEvents @event);
        void TelemetryTrackEvent(TelemetryTrackedEvents @event,string arg);
        void LogEvent(string @event);
    }
}
