using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;

namespace MALClient.Comm
{
    public static class Creditentials
    {
        public static string UserName { get; set; } = (string)ApplicationData.Current.LocalSettings.Values["username"];
        public static string Password { get; set; } = (string)ApplicationData.Current.LocalSettings.Values["password"];

        internal static ICredentials GetHttpCreditentials()
        {
            return new NetworkCredential(UserName, Password);
        }

        public static void Update(string name, string passwd)
        {
            ApplicationData.Current.LocalSettings.Values["username"] = name;
            ApplicationData.Current.LocalSettings.Values["password"] = passwd;
        }
    }
}
