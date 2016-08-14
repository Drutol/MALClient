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

        public CharacterDetailsData Data
        {
            get { return _data; }
            set
            {
                _data = value;
                RaisePropertyChanged(() => Data);
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

        public ICommand NavigateAnimeDetailsCommand
            =>
                _navigateAnimeDetailsCommand ??
                (_navigateAnimeDetailsCommand =
                    new RelayCommand<AnimeLightEntry>(
                        entry =>
                            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageAnimeDetails,
                                new AnimeDetailsPageNavigationArgs(entry.Id, entry.Title, null, null))));

        public ICommand NavigateMangaDetailsCommand
            =>
                _navigateMangaDetailsCommand ??
                (_navigateMangaDetailsCommand =
                    new RelayCommand<AnimeLightEntry>(
                        entry =>
                            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageAnimeDetails,
                                new AnimeDetailsPageNavigationArgs(entry.Id, entry.Title, null, null) {AnimeMode = false})));





        public async void Init(CharacterDetailsNavigationArgs args,bool force = false)
        {
            if(_prevArgs?.Equals(args) ?? false)
                return;

            _prevArgs = args;

            Data = await new CharacterDetailsQuery(args.Id).GetCharacterDetails();
            SpoilerButtonVisibility = !string.IsNullOrEmpty(Data.SpoilerContent);
            AnimeographyVisibility = Data.Animeography.Any();
            MangaographyVisibility = Data.Mangaography.Any();
        }

        public void RefreshData()
        {
            Init(_prevArgs,true);
        }
    }
}
