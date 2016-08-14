using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MalClient.Shared.Comm.Details;
using MalClient.Shared.Models.Anime;
using MalClient.Shared.Models.Favourites;
using MalClient.Shared.Models.ScrappedDetails;
using MalClient.Shared.NavArgs;
using MalClient.Shared.Utils.Enums;

namespace MalClient.Shared.ViewModels.Details
{
    public class CharacterDetailsViewModel : ViewModelBase
    {
        private CharacterDetailsData _data;
        private bool _animeographyVisibility;
        private bool _spoilerButtonVisibility;
        private ICommand _navigateAnimeDetailsCommand;
        private CharacterDetailsNavigationArgs _prevArgs;
        private ICommand _navigateMangaDetailsCommand;
        private bool _mangaographyVisibility;
        private List<FavouriteViewModel> _voiceActors;
        private ICommand _navigateStaffDetailsCommand;

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
                    new RelayCommand<AnimeStaffPerson>(
                        entry =>
                        {
                            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageCharacterDetails,_prevArgs);
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
                                new AnimeDetailsPageNavigationArgs(entry.Id, entry.Title, null, null) {AnimeMode = false})));

        public FavouriteViewModel FavouriteViewModel => Data == null ? null : new FavouriteViewModel(new AnimeCharacter { Id = Data.Id.ToString()});

        public async void Init(CharacterDetailsNavigationArgs args,bool force = false)
        {
            if(Data != null)
                ViewModelLocator.GeneralMain.CurrentOffStatus = Data.Name;
            if (_prevArgs?.Equals(args) ?? false)
                return;

            _prevArgs = args;

            Data = await new CharacterDetailsQuery(args.Id).GetCharacterDetails();
            SpoilerButtonVisibility = !string.IsNullOrEmpty(Data.SpoilerContent);
            AnimeographyVisibility = Data.Animeography.Any();
            MangaographyVisibility = Data.Mangaography.Any();
            VoiceActors = Data.VoiceActors.Select(actor => new FavouriteViewModel(actor)).ToList();
            RaisePropertyChanged(() => FavouriteViewModel);
            ViewModelLocator.GeneralMain.CurrentOffStatus = Data.Name;
        }

        public void RefreshData()
        {
            Init(_prevArgs,true);
        }
    }
}
