using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Models.Enums;
using MALClient.XShared.Comm;
using MALClient.XShared.Utils;
using MALClient.XShared.Utils.Enums;

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

        public string UserNameInput { get; set; }
        public string PasswordInput { get; set; }


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
                                             ViewModelLocator.GeneralMain.CurrentOffStatus = "Log In";
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

        public void Init()
        {
            ViewModelLocator.GeneralMain
                .CurrentOffStatus = Credentials.Authenticated ? $"Logged in as {Credentials.UserName}" : "Log In";

            CurrentApiType = Settings.SelectedApiType;
            LogOutButtonVisibility = Credentials.Authenticated;
        }

        private async void AttemptAuthentication()
        {
            if (Authenticating)
                return;
            Authenticating = true;
            Credentials.Update(UserNameInput, PasswordInput, CurrentApiType);
            PasswordInput = string.Empty;
            RaisePropertyChanged(() => PasswordInput);
            try
            {
                if (CurrentApiType == ApiType.Mal)
                {
                    var response = await new AuthQuery(ApiType.Mal).GetRequestResponse(false);
                    if (string.IsNullOrEmpty(response))
                        throw new Exception();
                    var doc = XDocument.Parse(response);
                    Settings.SelectedApiType = ApiType.Mal;
                    Credentials.SetId(int.Parse(doc.Element("user").Element("id").Value));
                    Credentials.SetAuthStatus(true);
                    Utilities.TelemetryTrackEvent(TelemetryTrackedEvents.LoggedInMyAnimeList);
                }
                else //hummingbird
                {
                    var response = await new AuthQuery(ApiType.Hummingbird).GetRequestResponse(false);
                    if (string.IsNullOrEmpty(response))
                        throw new Exception();
                    if (response.Contains("\"error\": \"Invalid credentials\""))
                        throw new Exception();
                    Settings.SelectedApiType = ApiType.Hummingbird;
                    Credentials.SetAuthToken(response);
                    Credentials.SetAuthStatus(true);
                    Utilities.TelemetryTrackEvent(TelemetryTrackedEvents.LoggedInHummingbird);

                }
            }
            catch (Exception)
            {
                Credentials.SetAuthStatus(false);
                Credentials.Update(string.Empty, string.Empty, ApiType.Mal);
                ResourceLocator.MessageDialogProvider.ShowMessageDialog("Unable to authorize with provided credentials.","Authorization failed.");              
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
