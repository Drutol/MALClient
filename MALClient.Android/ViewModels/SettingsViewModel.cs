using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Command;
using MALClient.Android.Activities;
using MALClient.Models.Enums;
using MALClient.XShared.Delegates;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.ViewModels
{
    [Preserve(AllMembers = true)]
    public class SettingsViewModel : SettingsViewModelBase
    {
        public override event SettingsNavigationRequest NavigationRequest;

        public override ICommand ReviewCommand => new RelayCommand(() =>
        {
            MainActivity.CurrentContext.StartActivity(new Intent(Intent.ActionView,
                global::Android.Net.Uri.Parse($"market://details?id={MainActivity.CurrentContext.PackageName}")));
        });

        public override ICommand RequestNavigationCommand
            => _requestNavigationCommand ?? (_requestNavigationCommand = new RelayCommand<SettingsPageIndex>(page =>
            {
                //Get to account page from settings
                if (page == SettingsPageIndex.LogIn)
                {
                    ViewModelLocator.GeneralMain.Navigate(PageIndex.PageLogIn);
                    return;
                }
                NavigationRequest?.Invoke(page);
                if (page != SettingsPageIndex.Homepage)
                    ViewModelLocator.NavMgr.RegisterOneTimeOverride(
                        new RelayCommand(() => { NavigationRequest?.Invoke(SettingsPageIndex.Homepage); }));
            }));


        public override void LoadCachedEntries()
        {
            
        }
    }
}