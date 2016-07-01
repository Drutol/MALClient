using System;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using Windows.Security.Credentials;
using Windows.Storage;
using MALClient.Comm;
using MALClient.ViewModels;

namespace MALClient
{
    public static class Credentials
    {
        public static string HummingbirdToken { get; private set; } =
            (string) (ApplicationData.Current.LocalSettings.Values["HummingbirdToken"] ?? "");

        public static string UserName { get; private set; }

        private static string Password { get; set; }

        public static int Id { get; private set; } =
            (int) (ApplicationData.Current.LocalSettings.Values["UserId"] ?? 0);

        public static bool Authenticated { get; private set; }

        internal static ICredentials GetHttpCreditentials()
        {
            return new NetworkCredential(UserName, Password);
        }

        internal static string GetHummingbirdCredentialChain()
        {
            return $"username={UserName}&password={Password}";
        }

        public static void Update(string name, string passwd, ApiType type)
        {
            var vault = new PasswordVault();

            foreach (var passwordCredential in vault.RetrieveAll())
                vault.Remove(passwordCredential);

            UserName = name;
            Password = passwd;

            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(passwd))
                vault.Add(new PasswordCredential(type == ApiType.Mal ? "MALClient" : "MALClientHum", UserName, Password));
        }

        public static void Reset()
        {
            var vault = new PasswordVault();

            UserName = Password = string.Empty;

            foreach (var passwordCredential in vault.RetrieveAll())
                vault.Remove(passwordCredential);
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

        public static void SetAuthToken(string token)
        {
            var trimmedToken = token == "" ? "" : token.Substring(1, token.Length - 2);
            ApplicationData.Current.LocalSettings.Values["HummingbirdToken"] = trimmedToken;
            HummingbirdToken = trimmedToken;
        }

        public static void Init()
        {
            var vault = new PasswordVault();
            // It has been quite a while since this format - safe to remove
            //if (bool.Parse((string) ApplicationData.Current.LocalSettings.Values["Auth"] ?? "False") &&
            //    ApplicationData.Current.LocalSettings.Values["Username"] != null) //check for old auth way
            //{
            //    vault.Add(new PasswordCredential("MALClient",
            //        ApplicationData.Current.LocalSettings.Values["Username"] as string, //they are not null
            //        ApplicationData.Current.LocalSettings.Values["password"] as string));

            //    //clean old resources
            //    ApplicationData.Current.LocalSettings.Values["Username"] = null;
            //    ApplicationData.Current.LocalSettings.Values["password"] = null;
            //}
            try
            {
                var deductedApiType = ApiType.Mal;
                PasswordCredential credential = null;
                try
                {
                    credential = vault.FindAllByResource("MALClient").FirstOrDefault();
                }
                catch (Exception)
                {
                    credential = vault.FindAllByResource("MALClientHum").FirstOrDefault();
                    deductedApiType = ApiType.Hummingbird;
                }
                if (credential != null)
                {
                    Settings.SelectedApiType = deductedApiType;
                    UserName = credential.UserName;
                    credential.RetrievePassword();
                    Password = credential.Password;
                    Authenticated = true;
                    if ((Settings.SelectedApiType == ApiType.Mal &&
                        string.IsNullOrEmpty(ApplicationData.Current.LocalSettings.Values["UserId"] as string)) ||
                        (Settings.SelectedApiType == ApiType.Hummingbird &&
                        string.IsNullOrEmpty(ApplicationData.Current.LocalSettings.Values["HummingbirdToken"] as string)))
                        //we have credentials without Id
                        FillInMissingIdData();
                }
                else
                    Authenticated = false;
            }
            catch (Exception)
            {
                Authenticated = false;
            }
        }

        private static async void FillInMissingIdData()
        {
            try
            {
                string response = null;
                switch (Settings.SelectedApiType)
                {
                    case ApiType.Mal:
                        response = await new AuthQuery(ApiType.Mal).GetRequestResponse(false);
                        if (string.IsNullOrEmpty(response))
                            throw new Exception();
                        var doc = XDocument.Parse(response);
                        SetId(int.Parse(doc.Element("user").Element("id").Value));
                        break;
                    case ApiType.Hummingbird:
                        response = await new AuthQuery(ApiType.Hummingbird).GetRequestResponse(false);
                        if (string.IsNullOrEmpty(response))
                            throw new Exception();
                        if (response.Contains("\"error\": \"Invalid credentials\""))
                            throw new Exception();
                        SetAuthToken(response);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception)
            {
                Authenticated = false;
            }
        }
    }
}