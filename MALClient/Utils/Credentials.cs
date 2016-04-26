using System;
using System.Linq;
using System.Net;
using Windows.Security.Credentials;
using Windows.Storage;
using MALClient.ViewModels;

namespace MALClient
{
    public static class Credentials
    {
        public static string UserName { get; private set; }

        private static string Password { get; set; }

        public static int Id { get; private set; } =
            (int) (ApplicationData.Current.LocalSettings.Values["UserId"] ?? 0);

        public static bool Authenticated { get; private set; }
            
        internal static ICredentials GetHttpCreditentials()
        {
            return new NetworkCredential(UserName, Password);
        }

        public static void Update(string name, string passwd)
        {
            var vault = new PasswordVault();

            if (!string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password))
                vault.Remove(new PasswordCredential("MALClient", UserName, Password));

            UserName = name;
            Password = passwd;

            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(passwd))
                vault.Add(new PasswordCredential("MALClient", UserName, Password));
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

        public static void Init()
        {
            var vault = new PasswordVault();
            if (bool.Parse((string)ApplicationData.Current.LocalSettings.Values["Auth"] ?? "False") && ApplicationData.Current.LocalSettings.Values["Username"] != null) //check for old auth way
            {               
                vault.Add(new PasswordCredential("MALClient",
                    ApplicationData.Current.LocalSettings.Values["Username"] as string, //they are not null
                    ApplicationData.Current.LocalSettings.Values["password"] as string));

                //clean old resources
                ApplicationData.Current.LocalSettings.Values["Username"] = null;
                ApplicationData.Current.LocalSettings.Values["password"] = null;
            }
            try
            {
                var credential = vault.FindAllByResource("MALClient").FirstOrDefault();
                if (credential != null)
                {
                    UserName = credential.UserName;
                    credential.RetrievePassword();
                    Password = credential.Password;
                    Authenticated = true;
                }
                else
                    Authenticated = false;
            }
            catch (Exception)
            {
                Authenticated = false;
            }

        }
    }
}