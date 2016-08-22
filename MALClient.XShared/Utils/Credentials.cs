using System;
using System.Net;
using System.Xml.Linq;
using MALClient.Adapters;
using MALClient.Adapters.Credentails;
using MALClient.Models.AdapterModels;
using MALClient.XShared.Comm;
using MALClient.XShared.ViewModels;

namespace MALClient.XShared.Utils
{
    public static class Credentials
    {
        public static readonly IApplicationDataService ApplicationDataService;
        public static readonly IPasswordVault PasswordVault;

        static Credentials()
        {
            ApplicationDataService = ResourceLocator.ApplicationDataService;
        }

        public static string HummingbirdToken { get; private set; } =
            (string) (ApplicationDataService["HummingbirdToken"] ?? "");

        public static string UserName { get; private set; }

        public static string Password { get; set; }

        public static int Id { get; private set; } =
            (int) (ApplicationDataService["UserId"] ?? 0);

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
            PasswordVault.Reset();

            UserName = name;
            Password = passwd;

            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(passwd))
                PasswordVault.Add(new VaultCredential((type == ApiType.Mal ? "MALClient" : "MALClientHum"), UserName, Password));
        }

        public static void Reset()
        {
            PasswordVault.Reset();

            UserName = Password = string.Empty;
        }

        public static void SetAuthStatus(bool status)
        {
            Authenticated = status;
            ApplicationDataService["Auth"] = status.ToString();
            ViewModelLocator.GeneralHamburger.UpdateLogInLabel();
        }

        public static void SetId(int id)
        {
            ApplicationDataService["UserId"] = id;
            Id = id;
        }

        public static void SetAuthToken(string token)
        {
            var trimmedToken = token == "" ? "" : token.Substring(1, token.Length - 2);
            ApplicationDataService["HummingbirdToken"] = trimmedToken;
            HummingbirdToken = trimmedToken;
        }

        public static void Init()
        {
            try
            {
                var deductedApiType = ApiType.Mal;
                VaultCredential credential = null;
                try
                {
                    credential = PasswordVault.Get("MALClient");
                }
                catch (Exception)
                {
                    credential = PasswordVault.Get("MALClientHum");
                    deductedApiType = ApiType.Hummingbird;
                }
                if (credential != null)
                {
                    Settings.SelectedApiType = deductedApiType;
                    UserName = credential.UserName;
                    Password = credential.Password;
                    Authenticated = true;
                    if ((Settings.SelectedApiType == ApiType.Mal &&
                        ApplicationDataService["UserId"] == null) ||
                        (Settings.SelectedApiType == ApiType.Hummingbird &&
                        string.IsNullOrEmpty(ApplicationDataService["HummingbirdToken"] as string)))
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