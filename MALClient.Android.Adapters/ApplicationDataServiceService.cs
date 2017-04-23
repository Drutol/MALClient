using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Preferences;
using MALClient.Adapters;

namespace MALClient.Android.Adapters
{
    public class ApplicationDataServiceService : IApplicationDataService
    {
        private static ISharedPreferences _preferenceManager;
        private static ISharedPreferences _preferenceManagerOriginal;

        static ApplicationDataServiceService()
        {
            _preferenceManagerOriginal = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
            _preferenceManager = _preferenceManagerOriginal;
        }

        public void OverridePreferenceManager(Context context)
        {
            _preferenceManager = PreferenceManager.GetDefaultSharedPreferences(context);
        }

        public void ResetPreferenceManagerOverride()
        {
            _preferenceManager = _preferenceManagerOriginal;
        }

        public object this[string key]
        {
            get { return _preferenceManager.Contains(key) ? _preferenceManager.All[key] : null; }
            set
            {
                
                var editor =  _preferenceManager.Edit();
                if (value != null)
                    switch (Type.GetTypeCode(value.GetType()))
                    {
                        case TypeCode.Boolean:
                            editor.PutBoolean(key, (bool) value);
                            break;
                        case TypeCode.String:
                            editor.PutString(key, (string) value);
                            break;
                        case TypeCode.Int32:
                            editor.PutInt(key, (int) value);
                            break;
                        case TypeCode.Int64:
                            editor.PutLong(key, (long) value);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                else
                    editor.Remove(key);
                editor.Apply();
            }
        }

        object IApplicationDataService.this[RoamingDataTypes key]
        {
            get { return this[key.ToString()]; }
            set { this[key.ToString()] = value; }
        }
    }
}
