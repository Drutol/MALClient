using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using Android.Runtime;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Models.Enums;
using MALClient.Models.Models.Auth;
using MALClient.XShared.Comm;
using MALClient.XShared.Utils;
using PCLCrypto;
using static PCLCrypto.WinRTCrypto;
using HashAlgorithm = PCLCrypto.HashAlgorithm;

namespace MALClient.XShared.ViewModels.Main
{

    /// <summary>
    /// This class is literally the last in the process of mvvm conversion... lol. 
    /// Believe me or not but log in was all in code behind for whole 9 months.
    /// "Broken code gets fixed, bad code stays bad forever(almost)"
    /// 04.09.2016
    /// </summary>
    public class LogInViewModel : ViewModelBase
    {
        private ICommand _logOutCommand;
        private ICommand _logInCommand;
		private ICommand _problemsCommand;
        private ICommand _navigateRegister;
        private bool _authenticating;
        private bool _logOutButtonVisibility;
        private ApiType _currentApiType;
        private string _codeVerifier;

        public ApiType CurrentApiType
        {
            get { return _currentApiType; }
            private set
            {
                _currentApiType = value;
                RaisePropertyChanged(() => ProblemsButtonVisibility);
				RaisePropertyChanged(() => CurrentApiType);
            }
        }

        public bool LogOutButtonVisibility
        {
            get { return _logOutButtonVisibility; }
            set
            {
                _logOutButtonVisibility = value;
                RaisePropertyChanged(() => LogOutButtonVisibility);
            }
        }

        [Preserve]
        public string UserNameInput { get; set; }
        [Preserve]
        public string PasswordInput {
            get;
            set;

        }

        public bool Authenticating
        {
            get { return _authenticating; }
            set
            {
                _authenticating = value;
                RaisePropertyChanged(() => Authenticating);
            }
        }

        public bool ProblemsButtonVisibility => CurrentApiType == ApiType.Mal;

        public ICommand LogOutCommand => _logOutCommand ?? (_logOutCommand = new RelayCommand(() =>
                                         {
                                             Credentials.Reset();
                                             ResourceLocator.AnimeLibraryDataStorage.Reset();
                                             ResourceLocator.MalHttpContextProvider.Invalidate();
                                             ResourceLocator.DataCacheService.ClearAnimeListData();
                                             ViewModelLocator.GeneralMain.CurrentOffStatus = "Log In";
                                             ViewModelLocator.GeneralHamburger.UpdateLogInLabel();
                                             LogOutButtonVisibility = false;
                                         }));

        public ICommand LogInCommand => _logInCommand ?? (_logInCommand = new RelayCommand(AttemptAuthentication));

		public ICommand ProblemsCommand => _problemsCommand ?? (_problemsCommand = new RelayCommand(() =>
											{
												ResourceLocator.MessageDialogProvider.ShowMessageDialog("If you are experiencing constant error messages while trying to log in, resetting your password on MAL may solve this issue. Why you may ask... MAL api is just very very bad and it tends to do such things which are beyond my control.\n\nIf you've registered using Twitter or Facebook... MAL does not allow 3rd party apps to authenticate such accounts therefore I cannot sign you in. All you need to do though is to go to website and set your password manually over there and everything will work smoothly. Nothing I can do about it.","Something went wrong?™");
											}));



        public ICommand NavigateRegister => _navigateRegister ?? (_navigateRegister = new RelayCommand(() =>
        {
            ResourceLocator.SystemControlsLauncherService.LaunchUri(new Uri("https://myanimelist.net/register.php"));
        }));

        public void Init()
        {
            ViewModelLocator.GeneralMain
                .CurrentOffStatus = Credentials.Authenticated ? $"Logged in as {Credentials.UserName}" : "Log In";

            CurrentApiType = ApiType.Mal;
            LogOutButtonVisibility = Credentials.Authenticated;
        }

        private string CreateCodeChallenge()
        {
            _codeVerifier = CreateUniqueId();
            var sha256 = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Sha256);
            var challengeBuffer = sha256.HashData(
                CryptographicBuffer.CreateFromByteArray(Encoding.UTF8.GetBytes(_codeVerifier)));
            CryptographicBuffer.CopyToByteArray(challengeBuffer, out var challengeBytes);
            return Convert.ToBase64String(challengeBytes);
        }

        public static string CreateUniqueId(int length = 64)
        {
            var bytes = CryptographicBuffer.GenerateRandom(length);
            return ByteArrayToString(bytes);
        }

        private static string ByteArrayToString(byte[] array)
        {
            var hex = new StringBuilder(array.Length * 2);
            foreach (byte b in array)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }

        private async void OnOauthResponse(OAuthResponse obj)
        {
            MessengerInstance.Unregister<OAuthResponse>(this, OnOauthResponse);

            var dict = new Dictionary<string, string>
            {
                {"client_id", Secrets.OauthClientId},
                {"grant_type", "authorization_code"},
                {"redirect_url", Secrets.OauthRedirectUrl },
                {"code", obj.Code},
                {"code_verifier", _codeVerifier},
            };

            var formContent = new FormUrlEncodedContent(dict);

            var response = await new AuthQuery(formContent).GetRequestResponse();

            if (response == null)
            {
                ResourceLocator.MessageDialogProvider.ShowMessageDialog("Failed to authorize.", "Sign in");
            }
        }

        private async void AttemptAuthentication()
        {
            var challenge = CreateCodeChallenge();
            var authorizeRequest = new AuthorizeRequest("https://myanimelist.net/v1/oauth2/authorize");

            var dic = new Dictionary<string, string>
            {
                {"client_id", Secrets.OauthClientId},
                {"response_type", "code"},
                //{"redirect_uri", "malclient://oauth"},
                {"state", Guid.NewGuid().ToString("N")},
                {"code_challenge", _codeVerifier},
                {"code_challenge_method", "plain"}
            };

            var authorizeUri = authorizeRequest.Create(dic);
           
            ResourceLocator.SystemControlsLauncherService.LaunchUri(new Uri(authorizeUri));

            MessengerInstance.Register<OAuthResponse>(this, OnOauthResponse);


            //            if (Authenticating)
            //                return;
            //            Authenticating = true;
            //            Credentials.Update(UserNameInput, PasswordInput, CurrentApiType);
            //#if ANDROID
            //            Query.RefreshClientAuthHeader();
            //#endif
            //            PasswordInput = string.Empty;
            //            RaisePropertyChanged(() => PasswordInput);
            //            try
            //            {
            //                if (CurrentApiType == ApiType.Mal)
            //                {
            //                    var response = await new AuthQuery(ApiType.Mal).GetRequestResponse();
            //                    if (string.IsNullOrEmpty(response))
            //                        throw new Exception();
            //                    Settings.SelectedApiType = ApiType.Mal;
            //                    Credentials.SetId(123456);
            //                    Credentials.SetAuthStatus(true);
            //                    ResourceLocator.TelemetryProvider.TelemetryTrackEvent(TelemetryTrackedEvents.LoggedInMyAnimeList);
            //                }
            //                else //hummingbird
            //                {
            //                    var response = await new AuthQuery(ApiType.Hummingbird).GetRequestResponse();
            //                    if (string.IsNullOrEmpty(response))
            //                        throw new Exception();
            //                    if (response.Contains("\"error\": \"Invalid credentials\""))
            //                        throw new Exception();
            //                    Settings.SelectedApiType = ApiType.Hummingbird;
            //                    Credentials.SetAuthToken(response);
            //                    Credentials.SetAuthStatus(true);
            //                    ResourceLocator.TelemetryProvider.TelemetryTrackEvent(TelemetryTrackedEvents.LoggedInHummingbird);

            //                }
            //            }
            //            catch (Exception e)
            //            {
            //                Credentials.SetAuthStatus(false);
            //                Credentials.Update(string.Empty, string.Empty, ApiType.Mal);
            //                ResourceLocator.MessageDialogProvider.ShowMessageDialog("Unable to authorize with provided credentials. If problem persists please try to sign-in on website.","Authorization failed.");              
            //                Authenticating = false;
            //                return;
            //            }
            //            try
            //            {
            //                await ViewModelLocator.GeneralHamburger.UpdateProfileImg();
            //            }
            //            catch (Exception)
            //            {
            //                //
            //            }
            //ViewModelLocator.GeneralMain.HideOffContentCommand.Execute(null);
            //await DataCache.ClearApiRelatedCache();
            //ViewModelLocator.GeneralMain.Navigate(PageIndex.PageAnimeList);
            //ViewModelLocator.GeneralHamburger.SetActiveButton(HamburgerButtons.AnimeList);

            Authenticating = false;
        }

        public class AuthorizeRequest
        {
            readonly Uri _authorizeEndpoint;

            public AuthorizeRequest(string authorizeEndpoint)
            {
                _authorizeEndpoint = new Uri(authorizeEndpoint);
            }

            public string Create(IDictionary<string, string> values)
            {
                var queryString = string.Join("&", values.Select(kvp =>
                    $"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}").ToArray());
                return $"{_authorizeEndpoint.AbsoluteUri}?{queryString}";
            }
        }
    }
}
