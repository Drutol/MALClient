using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using MalClient.Shared.Comm.Search;
using MalClient.Shared.Models.Favourites;

namespace MalClient.Shared.ViewModels.Main
{
    public class CharacterSearchViewModel : ViewModelBase
    {
        private bool _queryHandler;
        private ObservableCollection<FavouriteViewModel> _foundCharacters;

        public ObservableCollection<FavouriteViewModel> FoundCharacters
        {
            get { return _foundCharacters; }
            set
            {
                _foundCharacters = value;
                RaisePropertyChanged(() => FoundCharacters);
            }
        }

        public async void Init()
        {
            if(!_queryHandler)
                ViewModelLocator.GeneralMain.OnSearchQuerySubmitted += OnOnSearchQuerySubmitted;
            _queryHandler = true;
        }

        private async void OnOnSearchQuerySubmitted(string query)
        {
            if(query.Length <= 2)
                return;

            FoundCharacters =
                new ObservableCollection<FavouriteViewModel>(
                    (await new CharactersSearchQuery(query).GetSearchResults()).Select(
                        character => new FavouriteViewModel(character)));

        }

        public void OnNavigatedFrom()
        {
            ViewModelLocator.GeneralMain.OnSearchQuerySubmitted -= OnOnSearchQuerySubmitted;
            _queryHandler = false;
        }
    }
}
