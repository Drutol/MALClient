using System.Net;
using Windows.Devices.Usb;
using Windows.Storage;

namespace MALClient.Comm
{
    public static class Creditentials
    {
        public static string UserName { get; private set; } = (string)ApplicationData.Current.LocalSettings.Values["Username"];
        private static string Password { get; set; } = (string)ApplicationData.Current.LocalSettings.Values["password"];
        public static int Id { get; private set; } = (int)(ApplicationData.Current.LocalSettings.Values["UserId"] ?? 0);
        public static bool Authenticated { get; private set; } = bool.Parse((string)ApplicationData.Current.LocalSettings.Values["Auth"] ?? "False");
        internal static ICredentials GetHttpCreditentials()
        {
            return new NetworkCredential(UserName, Password);
        }

        public static void Update(string name, string passwd)
        {
            UserName = name;
            Password = passwd;
            ApplicationData.Current.LocalSettings.Values["Username"] = name;
            ApplicationData.Current.LocalSettings.Values["password"] = passwd;
        }

        public static void SetAuthStatus(bool status)
        {
            Authenticated = status;
            ApplicationData.Current.LocalSettings.Values["Auth"] = status.ToString();
        }

        public static void SetId(int id)
        {
            ApplicationData.Current.LocalSettings.Values["UserId"] = id;
            Id = id;
        }
    }
}
