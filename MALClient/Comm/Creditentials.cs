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
        private static string Password { get; set; } = (string)ApplicationData.Current.LocalSettings.Values["password"];
        public static bool Authenticated { get; set; } = bool.Parse((string)ApplicationData.Current.LocalSettings.Values["Auth"] ?? "False");
        internal static ICredentials GetHttpCreditentials()
        {
            return new NetworkCredential(UserName, Password);
        }

        public static void Update(string name, string passwd)
        {
            UserName = name;
            Password = passwd;
            ApplicationData.Current.LocalSettings.Values["username"] = name;
            ApplicationData.Current.LocalSettings.Values["password"] = passwd;
        }

        public static void SetAuthStatus(bool status)
        {
            Authenticated = status;
            ApplicationData.Current.LocalSettings.Values["Auth"] = status.ToString();
        }
    }
}
