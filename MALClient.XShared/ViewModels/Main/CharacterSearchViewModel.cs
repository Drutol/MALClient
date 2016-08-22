using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.XShared.Comm.Search;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils.Enums;

namespace MALClient.XShared.ViewModels.Main
{
    public class CharacterSearchViewModel : ViewModelBase
    {
        private bool _queryHandler;
        private ObservableCollection<FavouriteViewModel> _foundCharacters;
        private bool _loading;
        private bool _isEmptyNoticeVisible;
        private ICommand _navigateCharacterDetailsCommand;
        private bool _isFirstVisitGridVisible = true;
        private string _prevQuery;

        public ObservableCollection<FavouriteViewModel> FoundCharacters
        {
            get { return _foundCharacters; }
            set
            {
                _foundCharacters = value;
                RaisePropertyChanged(() => FoundCharacters);
            }
        }

        public bool Loading
        {
            get { return _loading; }
            private set
            {
                _loading = value;
                RaisePropertyChanged(() => Loading);
            }
        }

        public bool IsEmptyNoticeVisible
        {
            get { return _isEmptyNoticeVisible; }
            private set
            {
                _isEmptyNoticeVisible = value;
                RaisePropertyChanged(() => IsEmptyNoticeVisible);
            }
        }

        public bool IsFirstVisitGridVisible
        {
            get { return _isFirstVisitGridVisible; }
            private set
            {
                _isFirstVisitGridVisible = value;
                RaisePropertyChanged(() => IsFirstVisitGridVisible);
            }
        }

        public ICommand NavigateCharacterDetailsCommand
            => _navigateCharacterDetailsCommand ?? (_navigateCharacterDetailsCommand = new RelayCommand<FavouriteViewModel>(
                        entry =>
                        {
                            if (ViewModelLocator.Mobile)
                                ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageCharacterSearch,null);
                            else if (ViewModelLocator.GeneralMain.OffContentVisibility == Visibility.Visible)
                            {
                                if (ViewModelLocator.GeneralMain.CurrentOffPage == PageIndex.PageStaffDetails)
                                    ViewModelLocator.StaffDetails.RegisterSelfBackNav(int.Parse(entry.Data.Id));
                                else if (ViewModelLocator.GeneralMain.CurrentOffPage == PageIndex.PageCharacterDetails)
                                    ViewModelLocator.CharacterDetails.RegisterSelfBackNav(int.Parse(entry.Data.Id));
                            }
                            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageCharacterDetails,
                                new CharacterDetailsNavigationArgs { Id = int.Parse(entry.Data.Id) });
                        }));

        public void Init()
        {
            if (!Loading && (FoundCharacters == null || !FoundCharacters.Any()))
            {
                IsFirstVisitGridVisible = true;
                IsEmptyNoticeVisible = false;
            }
            else
            {
                IsEmptyNoticeVisible = false;
                IsFirstVisitGridVisible = false;
            }

            if (!_queryHandler)
                ViewModelLocator.GeneralMain.OnSearchQuerySubmitted += OnOnSearchQuerySubmitted;
            _queryHandler = true;        

            OnOnSearchQuerySubmitted(ViewModelLocator.GeneralMain.CurrentSearchQuery);
        }

        private async void OnOnSearchQuerySubmitted(string query)
        {         
            if (Loading || (query?.Equals(_prevQuery, StringComparison.CurrentCultureIgnoreCase) ?? true) || string.IsNullOrEmpty(query))
                return;
            IsFirstVisitGridVisible = false;
            if (query.Length <= 2)
            {
                FoundCharacters?.Clear();
                IsEmptyNoticeVisible = false;
                IsFirstVisitGridVisible = true;
                return;
            }

            _prevQuery = query;

            FoundCharacters?.Clear();
            Loading = true;

            try
            {
                FoundCharacters =
                    new ObservableCollection<FavouriteViewModel>(
                        (await new CharactersSearchQuery(query).GetSearchResults()).Select(
                            character => new FavouriteViewModel(character)));

                IsEmptyNoticeVisible = !FoundCharacters.Any();
            }
            catch (Exception)
            {
                IsEmptyNoticeVisible = true;
            }
            Loading = false;
            

        }

        public void OnNavigatedFrom()
        {
            if (!ViewModelLocator.Mobile)
            {
                FoundCharacters?.Clear();
                _prevQuery = "";
            }
            ViewModelLocator.GeneralMain.OnSearchQuerySubmitted -= OnOnSearchQuerySubmitted;
            _queryHandler = false;
        }
    }
}
