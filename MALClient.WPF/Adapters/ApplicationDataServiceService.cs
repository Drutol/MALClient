using System;
using System.Collections.Generic;
using MALClient.Adapters;

namespace MALClient.WPF.Adapters
{
    public class ApplicationDataServiceService : IApplicationDataService
    {
        private Dictionary<string,object> _dictionary = new Dictionary<string, object>();

        public object this[string key]
        {
            get
            {
                if (!_dictionary.ContainsKey(key))
                    return null;
                return _dictionary[key];
            }
            set { _dictionary[key] = value; }
        }

        public object this[RoamingDataTypes key]
        {
            get
            {
                if (!_dictionary.ContainsKey(key.ToString()))
                    return null;
                return _dictionary[key.ToString()];
            }
            set { _dictionary[key.ToString()] = value; }
        }
    }
}
