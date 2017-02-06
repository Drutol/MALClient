using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Models.Enums;
using MALClient.Models.Models.Anime;
using MALClient.Models.Models.Favourites;
using MALClient.Models.Models.ScrappedDetails;
using MALClient.XShared.Comm.Details;
using MALClient.XShared.NavArgs;

namespace MALClient.XShared.ViewModels.Details
{
    public class CharacterDetailsViewModel : ViewModelBase
    {
        private CharacterDetailsData _data;
        private bool _animeographyVisibility;
        private bool _spoilerButtonVisibility;
        private ICommand _navigateAnimeDetailsCommand;
        private ICommand _openInMalCommand;
        private CharacterDetailsNavigationArgs _prevArgs;
        private ICommand _navigateMangaDetailsCommand;
        private bool _mangaographyVisibility;
        private List<FavouriteViewModel> _voiceActors;
        private ICommand _navigateStaffDetailsCommand;
        private bool _loading;

        public CharacterDetailsData Data
        {
            get { return _data; }
            set
            {
                _data = value;
                RaisePropertyChanged(() => Data);
            }
        }

        public List<FavouriteViewModel> VoiceActors
        {
            get { return _voiceActors; }
            set
            {
                _voiceActors = value;
                RaisePropertyChanged(() => VoiceActors);
            }
        }


        public bool SpoilerButtonVisibility //one could say that this is first boolish visibility property in preparation for xamarin - 14.08.2016
        {
            get { return _spoilerButtonVisibility; }
            set
            {
                _spoilerButtonVisibility = value;
                RaisePropertyChanged(() => SpoilerButtonVisibility);
            }
        }


        public bool AnimeographyVisibility
        {
            get { return _animeographyVisibility; }
            set
            {
                _animeographyVisibility = value;
                RaisePropertyChanged(() => AnimeographyVisibility);
            }
        }

        public bool MangaographyVisibility
        {
            get { return _mangaographyVisibility; }
            set
            {
                _mangaographyVisibility = value;
                RaisePropertyChanged(() => MangaographyVisibility);
            }
        }

        public ICommand NavigateStaffDetailsCommand
            =>
                _navigateStaffDetailsCommand ??
                (_navigateStaffDetailsCommand =
                    new RelayCommand<FavouriteBase>(
                        entry =>
                        {
                            RegisterSelfBackNav();
                            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageStaffDetails,
                                new StaffDetailsNaviagtionArgs {Id = int.Parse(entry.Id)});
                        }));
                            

        public ICommand NavigateAnimeDetailsCommand
            =>
                _navigateAnimeDetailsCommand ??
                (_navigateAnimeDetailsCommand =
                    new RelayCommand<AnimeLightEntry>(
                        entry =>
                        {
                            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageAnimeDetails,
                                new AnimeDetailsPageNavigationArgs(entry.Id, entry.Title, null, null,_prevArgs) {Source = PageIndex.PageCharacterDetails});
                        }));

        public ICommand NavigateMangaDetailsCommand
            =>
                _navigateMangaDetailsCommand ??
                (_navigateMangaDetailsCommand =
                    new RelayCommand<AnimeLightEntry>(
                        entry =>
                            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageAnimeDetails,
                                new AnimeDetailsPageNavigationArgs(entry.Id, entry.Title, null,null, _prevArgs) { Source = PageIndex.PageCharacterDetails, AnimeMode = false})));


        public ICommand OpenInMalCommand => _openInMalCommand ?? (_openInMalCommand = new RelayCommand(() =>
        {
            ResourceLocator.SystemControlsLauncherService.LaunchUri(new Uri($"https://myanimelist.net/character/{Data.Id}"));
        }));

        public FavouriteViewModel FavouriteViewModel => Data == null ? null : new FavouriteViewModel(new AnimeCharacter { Id = Data.Id.ToString()});

        public bool Loading
        {
            get { return _loading; }
            set
            {
                _loading = value;
                RaisePropertyChanged(() => Loading);
            }
        }



        public async void Init(CharacterDetailsNavigationArgs args,bool force = false)
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

            Data = await new CharacterDetailsQuery(args.Id).GetCharacterDetails(force);
            SpoilerButtonVisibility = !string.IsNullOrEmpty(Data.SpoilerContent);
            AnimeographyVisibility = Data.Animeography.Any();
            MangaographyVisibility = Data.Mangaography.Any();
            VoiceActors = Data.VoiceActors.Select(actor => new FavouriteViewModel(actor)).ToList();
            RaisePropertyChanged(() => FavouriteViewModel);
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
        /// <param name="targetId">Don't duplicate nav back</param>
        public void RegisterSelfBackNav(int targetId = 0)
        {
            if (targetId == Data.Id)
                return;
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageCharacterDetails, _prevArgs);
        }
    }
}
