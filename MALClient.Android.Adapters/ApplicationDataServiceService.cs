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
        private static readonly ISharedPreferences PreferenceManager;

        static ApplicationDataServiceService()
        {
            PreferenceManager = global::Android.Preferences.PreferenceManager.GetDefaultSharedPreferences(Application.Context);
        }

        public object this[string key]
        {
            get { return PreferenceManager.Contains(key) ? PreferenceManager.All[key] : null; }
            set
            {
                
                var editor =  PreferenceManager.Edit();
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
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                editor.Apply();
            }
        }
    }
}
