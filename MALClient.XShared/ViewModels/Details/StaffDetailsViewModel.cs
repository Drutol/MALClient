using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Models.Enums;
using MALClient.Models.Models.Anime;
using MALClient.Models.Models.Favourites;
using MALClient.Models.Models.ScrappedDetails;
using MALClient.XShared.Comm.Details;
using MALClient.XShared.Delegates;
using MALClient.XShared.NavArgs;

namespace MALClient.XShared.ViewModels.Details
{
    public class StaffDetailsViewModel : ViewModelBase
    {
        private StaffDetailsData _data;
        private StaffDetailsNaviagtionArgs _prevArgs;
        private ICommand _navigateAnimeDetailsCommand;
        private ICommand _navigateCharacterDetailsCommand;
        private bool _loading;
        private ICommand _openInMalCommand;
        private bool _isNoVoiceActingRolesNoticeVisible;
        private bool _isNoProductionRolesNoticeVisible;

        public event PivotItemSelectionRequest OnPivotItemSelectionRequest;

        public StaffDetailsData Data
        {
            get { return _data; }
            set
            {
                _data = value;
                RaisePropertyChanged(() => Data);
                RaisePropertyChanged(() => FavouriteViewModel);
            }
        }

        public ICommand NavigateAnimeDetailsCommand
            =>
                _navigateAnimeDetailsCommand ??
                (_navigateAnimeDetailsCommand =
                    new RelayCommand<AnimeLightEntry>(
                        entry =>
                        {
                            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageAnimeDetails,
                                new AnimeDetailsPageNavigationArgs(entry.Id, entry.Title, null,null, _prevArgs) {AnimeMode = entry.IsAnime,Source = PageIndex.PageStaffDetails});
                        }));

        public ICommand NavigateCharacterDetailsCommand
            =>
                _navigateCharacterDetailsCommand ??
                (_navigateCharacterDetailsCommand =
                    new RelayCommand<AnimeCharacter>(
                        entry =>
                        {       
                            RegisterSelfBackNav();                    
                            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageCharacterDetails,
                                new CharacterDetailsNavigationArgs { Id = int.Parse(entry.Id) });
                        }));

        public ICommand OpenInMalCommand => _openInMalCommand ?? (_openInMalCommand = new RelayCommand(() =>
        {
            ResourceLocator.SystemControlsLauncherService.LaunchUri(new Uri($"https://myanimelist.net/people/{Data.Id}"));
        }));

        public FavouriteViewModel FavouriteViewModel
            => Data == null ? null : new FavouriteViewModel(new AnimeStaffPerson {Id = Data.Id.ToString()});

        public bool Loading
        {
            get { return _loading; }
            set
            {
                _loading = value;
                RaisePropertyChanged(() => Loading);
            }
        }

        public bool IsNoVoiceActingRolesNoticeVisible
        {
            get { return _isNoVoiceActingRolesNoticeVisible; }
            set
            {
                _isNoVoiceActingRolesNoticeVisible = value;
                RaisePropertyChanged(() => IsNoVoiceActingRolesNoticeVisible);
            }
        }

        public bool IsNoProductionRolesNoticeVisible
        {
            get { return _isNoProductionRolesNoticeVisible; }
            set
            {
                _isNoProductionRolesNoticeVisible = value;
                RaisePropertyChanged(() => IsNoProductionRolesNoticeVisible);
            }
        }


        public async void Init(StaffDetailsNaviagtionArgs args, bool force = false)
        {
            if (Data != null)
            {
                ViewModelLocator.GeneralMain.CurrentOffStatus = Data.Name;
                ViewModelLocator.GeneralMain.IsCurrentStatusSelectable = true;
            }
            if (!force && (_prevArgs?.Equals(args) ?? false))
                return;
            Loading = true;
            _prevArgs = args;
            Data = await new StaffDetailsQuery(args.Id).GetStaffDetails(force);
            if (Data.ShowCharacterPairs.Count == 0)
            {
                IsNoVoiceActingRolesNoticeVisible = true;
                OnPivotItemSelectionRequest?.Invoke(1);
            }
            IsNoProductionRolesNoticeVisible = Data.StaffPositions.Count == 0;
            ViewModelLocator.GeneralMain.CurrentOffStatus = Data.Name;
            ViewModelLocator.GeneralMain.IsCurrentStatusSelectable = true;
            Loading = false;
        }

        public void RefreshData()
        {
            Init(_prevArgs,true);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetId">Don't duplicate navigation</param>
        public void RegisterSelfBackNav(int targetId = 0)
        {
            if(targetId == Data.Id)
                return;
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageStaffDetails, _prevArgs);
        }
    }
}
