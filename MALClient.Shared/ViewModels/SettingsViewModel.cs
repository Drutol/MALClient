using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.System;
using GalaSoft.MvvmLight.Command;
using MALClient.Models.Enums;
using MALClient.Models.Models.Misc;
using MALClient.XShared.Delegates;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;

namespace MALClient.UWP.Shared.ViewModels
{
    public class SettingsViewModel : SettingsViewModelBase
    {
        public override event SettingsNavigationRequest NavigationRequest;

        public override ICommand ReviewCommand => _reviewCommand ?? (_reviewCommand = new RelayCommand(async () =>
        {
            Settings.RatePopUpEnable = false;
            await
                Launcher.LaunchUriAsync(
                    new Uri($"ms-windows-store:REVIEW?PFN={Package.Current.Id.FamilyName}"));
        }));

        public override List<SettingsPageEntry> SettingsPages =>
            base.SettingsPages.Where(entry => entry.PageType != SettingsPageIndex.Ads).ToList();

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

        public override async void LoadCachedEntries()
        {
            //if (_cachedItemsLoaded)
            //    return;
            //_cachedItemsLoaded = true;
            //var files = await ApplicationData.Current.LocalFolder.GetFilesAsync();
            //foreach (var file in files)
            //{
            //    if (file.FileType == ".json")
            //    {
            //        var data = await file.GetBasicPropertiesAsync();
            //        CachedEntries.Add(new CachedEntryModel
            //        {
            //            Date = data.DateModified.LocalDateTime.ToString("dd/MM/yyyy HH:mm"),
            //            FileName = file.Name,
            //            Size = Utilities.SizeSuffix((long)data.Size)
            //        });
            //    }
            //}
            //EmptyCachedListVisiblity = files.Count == 0;
            //try
            //{
            //    var folder = await ApplicationData.Current.LocalFolder.GetFolderAsync("AnimeDetails");
            //    var data = await folder.GetFilesAsync();
            //    TotalFilesCached = $"Remove all anime details data({data.Count}files)";
            //    RemoveAllCachedDataButtonVisibility = true;
            //}
            //catch (Exception)
            //{
            //    //No folder yet
            //    RemoveAllCachedDataButtonVisibility = false;
            //}
        }
    }
}
