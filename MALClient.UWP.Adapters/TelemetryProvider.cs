using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MALClient.Adapters;
using MALClient.Models.Enums;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using Microsoft.HockeyApp;
using Microsoft.HockeyApp.DataContracts;

namespace MALClient.UWP.Adapters
{
    public class TelemetryProvider : ITelemetryProvider
    {
        public void Init()
        {
#if !DEBUG
            HockeyClient.Current.Configure("b79e78858bdf44c4bfc3a1f37c8fd90c", new TelemetryConfiguration
                {
                    Collectors =
                        WindowsCollectors.Metadata | WindowsCollectors.Session | WindowsCollectors.UnhandledException,
                })
                .SetExceptionDescriptionLoader(
                    exception =>
                            $"MainPage: {ViewModelLocator.GeneralMain.CurrentMainPage.ToString()} {(!ViewModelLocator.Mobile ? $"OffPage: {ViewModelLocator.GeneralMain.CurrentOffPage.ToString()}" : "")}");
#endif
        }

        public void TelemetryTrackEvent(TelemetryTrackedEvents @event)
        {
#if !DEBUG
            HockeyClient.Current.TrackEvent(@event.ToString());
#endif
        }

        public void TelemetryTrackEvent(TelemetryTrackedEvents @event, string arg)
        {
#if !DEBUG
            HockeyClient.Current.TrackEvent(@event + " " + arg);
#endif
        }

        public void TelemetryTrackNavigation(PageIndex page)
        {
#if !DEBUG
            HockeyClient.Current.TrackPageView(page.ToString());
#endif
        }

        public void TelemetryTrackNavigation(ForumsPageIndex page)
        {
#if !DEBUG
            HockeyClient.Current.TrackPageView(page.ToString());
#endif
        }

        public void LogEvent(string @event)
        {
#if !DEBUG
            HockeyClient.Current.TrackTrace(@event);
#endif
        }

        public void TrackException(Exception e)
        {
#if !DEBUG
            HockeyClient.Current.TrackException(e);
#endif
        }
    }
}
