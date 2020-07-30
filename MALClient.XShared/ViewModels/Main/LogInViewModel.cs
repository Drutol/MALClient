using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using Android.Runtime;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Models.Enums;
using MALClient.XShared.Comm;
using MALClient.XShared.Utils;

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
        private ICommand _focusMalCommand;
        private ICommand _focusHumCommand;
        private ICommand _logOutCommand;
        private ICommand _logInCommand;
		private ICommand _problemsCommand;
        private ICommand _navigateRegister;
        private bool _authenticating;
        private bool _logOutButtonVisibility;
        private bool _isHumToggleChecked;
        private bool _isMalToggleChecked;
        private ApiType _currentApiType;

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

        public ICommand FocusMalCommand => _focusMalCommand ?? (_focusMalCommand = new RelayCommand(() =>
                                           {                                 
                                               CurrentApiType = ApiType.Mal;
                                           }));

        public ICommand FocusHumCommand => _focusHumCommand ?? (_focusHumCommand = new RelayCommand(() =>
                                           {                                                                                        
                                               CurrentApiType = ApiType.Hummingbird;
                                           }));

        public ICommand LogInCommand => _logInCommand ?? (_logInCommand = new RelayCommand(AttemptAuthentication));

		public ICommand ProblemsCommand => _problemsCommand ?? (_problemsCommand = new RelayCommand(() =>
											{
												ResourceLocator.MessageDialogProvider.ShowMessageDialog("If you are experiencing constant error messages while trying to log in, resetting your password on MAL may solve this issue. Why you may ask... MAL api is just very very bad and it tends to do such things which are beyond my control.\n\nIf you've registered using Twitter or Facebook... MAL does not allow 3rd party apps to authenticate such accounts therefore I cannot sign you in. All you need to do though is to go to website and set your password manually over there and everything will work smoothly. Nothing I can do about it.","Something went wrong?™");
											}));



        public ICommand NavigateRegister => new RelayCommand(async () =>
        {
            //ResourceLocator.MessageDialogProvider.ShowMessageDialog(
            //    "Sorry, MyAnimeList.net has disabled registration for the time being. It's not something I can change, please go complain to MAL directly via support ticket.",
            //    "Registration disabled");
            try
            {
                ResourceLocator.SystemControlsLauncherService.LaunchUri(new Uri("https://myanimelist.net/register.php"));
            }
            catch (Exception e)
            {
                ResourceLocator.MessageDialogProvider.ShowMessageDialog("Looks like there's nothing that can launch this link... You can register at: https://myanimelist.net/register.php", "Error");
            }
        });


        public void Init()
        {
            ViewModelLocator.GeneralMain
                .CurrentOffStatus = Credentials.Authenticated ? $"Logged in as {Credentials.UserName}" : "Log In";

            CurrentApiType = ApiType.Mal;//Settings.SelectedApiType;
            LogOutButtonVisibility = Credentials.Authenticated;
        }

        private async void AttemptAuthentication()
        {
            if (Authenticating)
                return;
            Authenticating = true;
            Credentials.Update(UserNameInput, PasswordInput, CurrentApiType);
#if ANDROID
            Query.RefreshClientAuthHeader();
#endif
            PasswordInput = string.Empty;
            RaisePropertyChanged(() => PasswordInput);
            try
            {
                if (CurrentApiType == ApiType.Mal)
                {
                    var response = await new AuthQuery(ApiType.Mal).GetRequestResponse();
                    if (string.IsNullOrEmpty(response))
                        throw new Exception();
                    Settings.SelectedApiType = ApiType.Mal;
                    Credentials.SetId(123456);
                    Credentials.SetAuthStatus(true);
                    ResourceLocator.TelemetryProvider.TelemetryTrackEvent(TelemetryTrackedEvents.LoggedInMyAnimeList);
                }
                else //hummingbird
                {
                    var response = await new AuthQuery(ApiType.Hummingbird).GetRequestResponse();
                    if (string.IsNullOrEmpty(response))
                        throw new Exception();
                    if (response.Contains("\"error\": \"Invalid credentials\""))
                        throw new Exception();
                    Settings.SelectedApiType = ApiType.Hummingbird;
                    Credentials.SetAuthToken(response);
                    Credentials.SetAuthStatus(true);
                    ResourceLocator.TelemetryProvider.TelemetryTrackEvent(TelemetryTrackedEvents.LoggedInHummingbird);

                }
            }
            catch (Exception e)
            {
                Credentials.SetAuthStatus(false);
                Credentials.Update(string.Empty, string.Empty, ApiType.Mal);
                ResourceLocator.MessageDialogProvider.ShowMessageDialog("Unable to authorize with provided credentials. If problem persists please try to sign-in on website.","Authorization failed.");              
                Authenticating = false;
                return;
            }
            try
            {
                await ViewModelLocator.GeneralHamburger.UpdateProfileImg();
            }
            catch (Exception)
            {
                //
            }
            ViewModelLocator.GeneralMain.HideOffContentCommand.Execute(null);
            await DataCache.ClearApiRelatedCache();
            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageAnimeList);
            ViewModelLocator.GeneralHamburger.SetActiveButton(HamburgerButtons.AnimeList);

            Authenticating = false;
        }
    }
}
