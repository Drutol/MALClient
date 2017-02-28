using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Storage;
using MALClient.Adapters;
using Microsoft.HockeyApp;

namespace MALClient.UWP.Adapters
{
    public class ApplicationDataServiceService : IApplicationDataService
    {
        public object this[string key]
        {
            get
            {
                try
                {
                    return ApplicationData.Current.LocalSettings.Values[key];
                }
                catch (Exception)
                {
                    return null;
                }
            }
            set { ApplicationData.Current.LocalSettings.Values[key] = value; }
        }

        public object this[RoamingDataTypes key]
        {
            get
            {
                var val = ApplicationData.Current.RoamingSettings.Values[key.ToString()] ??
                          ApplicationData.Current.LocalSettings.Values[key.ToString()];
                return val;
            }
            set
            {
                ApplicationData.Current.RoamingSettings.Values[key.ToString()] = value;
                if (ApplicationData.Current.RoamingSettings.Values[key.ToString()] == null)
                    this[key.ToString()] = value;
            }
        }
    }
}
