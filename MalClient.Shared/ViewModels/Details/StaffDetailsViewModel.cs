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
    public class StaffDetailsViewModel : ViewModelBase
    {
        private StaffDetailsData _data;
        private StaffDetailsNaviagtionArgs _prevArgs;
        private ICommand _navigateAnimeDetailsCommand;
        private ICommand _navigateCharacterDetailsCommand;

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
                                new AnimeDetailsPageNavigationArgs(entry.Id, entry.Title, null,null, _prevArgs) {Source = PageIndex.PageStaffDetails});
                        }));

        public ICommand NavigateCharacterDetailsCommand
            =>
                _navigateCharacterDetailsCommand ??
                (_navigateCharacterDetailsCommand =
                    new RelayCommand<AnimeCharacter>(
                        entry =>
                        {
                            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageStaffDetails, _prevArgs);
                            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageCharacterDetails,
                                new CharacterDetailsNavigationArgs { Id = int.Parse(entry.Id) });
                        }));

        public FavouriteViewModel FavouriteViewModel
            => Data == null ? null : new FavouriteViewModel(new AnimeStaffPerson {Id = Data.Id.ToString()});

        public async void Init(StaffDetailsNaviagtionArgs args, bool force = false)
        {
            if (Data != null)
                ViewModelLocator.GeneralMain.CurrentOffStatus = Data.Name;
            if (_prevArgs?.Equals(args) ?? false)
                return;

            _prevArgs = args;
            Data = await new StaffDetailsQuery(args.Id).GetStaffDetails(force);
            ViewModelLocator.GeneralMain.CurrentOffStatus = Data.Name;
        }

        public void RefreshData()
        {
            Init(_prevArgs,true);
        }
    }
}
