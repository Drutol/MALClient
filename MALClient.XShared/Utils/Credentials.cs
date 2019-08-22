using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using MALClient.Adapters;
using MALClient.Adapters.Credentials;
using MALClient.Models.AdapterModels;
using MALClient.Models.Enums;
using MALClient.Models.Models.Auth;
using MALClient.XShared.Comm;
using MALClient.XShared.ViewModels;
using Newtonsoft.Json;

namespace MALClient.XShared.Utils
{
    public static class Credentials
    {
        public static readonly IApplicationDataService ApplicationDataService;
        public static readonly IPasswordVault PasswordVault;
        private static string _userName;

        static Credentials()
        {
            ApplicationDataService = ResourceLocator.ApplicationDataService;
            PasswordVault = ResourceLocator.PasswordVaultProvider;

            var tokens = (string) ApplicationDataService[nameof(TokenResponse)];
            if (!string.IsNullOrEmpty(tokens))
            {
                Tokens = JsonConvert.DeserializeObject<TokenResponse>(tokens);
            }

            Id = (int) (ApplicationDataService["UserId"] ?? 0);
            Authenticated =
                bool.Parse(ApplicationDataService["Auth"] as string ?? "False"); //why? because I can? but I shouldn't
        }
        public static string UserName
        {
            get { return _userName; }
            private set { _userName = value?.Trim(); }
        }

        public static int Id { get; private set; }
        public static TokenResponse Tokens { get; set; }
        public static bool Authenticated { get; private set; }

        public static void Reset()
        {
            PasswordVault.Reset();
            SetAuthStatus(false);
            SetAuthTokenResponse(null);
            UserName = string.Empty;
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

        public static void SetAuthTokenResponse(TokenResponse tokens)
        {
            ApplicationDataService[nameof(TokenResponse)] = tokens == null ? null : JsonConvert.SerializeObject(tokens);
            Tokens = tokens;
            Query.RefreshClientAuthHeader();
        }

        public static async Task Init()
        {
            try
            {
                if (Tokens != null)
                {
                    if (DateTime.UtcNow < Tokens.ValidTill)
                    {
                        //obtain access token from refresh token
                        var response = await new AuthQuery(new FormUrlEncodedContent(new Dictionary<string, string>
                        {
                            {"grant_type", "refresh_token"},
                            {"refresh_token", Tokens.RefreshToken},
                            {"client_id", Secrets.OauthClientId},
                        })).GetRequestResponse();
                        var tokens = JsonConvert.DeserializeObject<TokenResponse>(response);
                        SetAuthTokenResponse(tokens);
                    }
                }

            }
            catch (Exception)
            {
                Authenticated = false;
            }
        }
    }
}