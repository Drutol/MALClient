using System.Net;
using Windows.Storage;
using MALClient.ViewModels;

namespace MALClient
{
    public static class Credentials
    {
        public static string UserName { get; private set; } =
            (string) ApplicationData.Current.LocalSettings.Values["Username"];

        private static string Password { get; set; } =
            (string) ApplicationData.Current.LocalSettings.Values["password"];

        public static int Id { get; private set; } =
            (int) (ApplicationData.Current.LocalSettings.Values["UserId"] ?? 0);

        public static bool Authenticated { get; private set; } =
            bool.Parse((string) ApplicationData.Current.LocalSettings.Values["Auth"] ?? "False");

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
            ViewModelLocator.Hamburger.UpdateLogInLabel();
        }

        public static void SetId(int id)
        {
            ApplicationData.Current.LocalSettings.Values["UserId"] = id;
            Id = id;
        }
    }
}