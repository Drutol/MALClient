using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Models.Enums;
using MALClient.Models.Models.AnimeScrapped;
using MALClient.Models.Models.Library;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.Delegates;
using MALClient.XShared.NavArgs;

namespace MALClient.XShared.ViewModels
{
    public class RecommendationItemViewModel : ViewModelBase
    {
        private bool _dataLoaded;
        private bool _loadingSpinnerVisibility;

        public RecommendationData Data { get; }

        public int Index { get; }

        public bool LoadingSpinnerVisibility
        {
            get { return _loadingSpinnerVisibility; }
            set
            {
                _loadingSpinnerVisibility = value;
                RaisePropertyChanged(() => LoadingSpinnerVisibility);
            }
        }

        public ObservableCollection<Tuple<string, string, string, string, string>> DetailItems { get; } =
            new ObservableCollection<Tuple<string, string, string, string, string>>();

        public RecommendationItemViewModel(RecommendationData data, int index)
        {
            Data = data;
            Index = index;
        }

        public async void PopulateData()
        {
            if (_dataLoaded)
                return;
            _dataLoaded = true;
            LoadingSpinnerVisibility = true;
            try
            {
                //Find for first
                Data.AnimeDependentData =
                    await
                        new AnimeGeneralDetailsQuery().GetAnimeDetails(false, Data.DependentId.ToString(), Data.DependentTitle, Data.IsAnime,
                            ApiType.Mal);

                //Find for second
                Data.AnimeRecommendationData =
                    await
                        new AnimeGeneralDetailsQuery().GetAnimeDetails(false, Data.RecommendationId.ToString(),
                            Data.RecommendationTitle, Data.IsAnime, ApiType.Mal);

                //If for some reason we fail
                if (Data.AnimeDependentData == null || Data.AnimeRecommendationData == null)
                    throw new ArgumentNullException(); // I'm to lazy to create my own so this will suffice     
            }
            catch (ArgumentNullException)
            {
                return; //umm tried to search for show with K as a title...
            }

            RaisePropertyChanged(() => Data);
            IAnimeData myDepItem, myRecItem;
            if (Data.IsAnime)
            {
                myDepItem = await ViewModelLocator.AnimeList.TryRetrieveAuthenticatedAnimeItem(Data.DependentId);
                myRecItem = await ViewModelLocator.AnimeList.TryRetrieveAuthenticatedAnimeItem(Data.RecommendationId);
            }
            else
            {
                myDepItem = await ViewModelLocator.AnimeList.TryRetrieveAuthenticatedAnimeItem(Data.DependentId,false);
                myRecItem = await ViewModelLocator.AnimeList.TryRetrieveAuthenticatedAnimeItem(Data.RecommendationId,false);
            }


            DetailItems.Add(new Tuple<string, string, string, string, string>($"{(Data.IsAnime ? "Episodes" : "Chapters")}:",
                Data.AnimeDependentData.AllEpisodes != 0 ? Data.AnimeDependentData.AllEpisodes.ToString() : "?",
                myDepItem?.MyEpisodes == null ? "" : myDepItem.MyEpisodes + $"/{(Data.AnimeDependentData.AllEpisodes == 0 ? "?" : Data.AnimeDependentData.AllEpisodes.ToString())}",
                Data.AnimeRecommendationData.AllEpisodes != 0 ? Data.AnimeRecommendationData.AllEpisodes.ToString() : "?",
                myRecItem?.MyEpisodes == null ? "" : myRecItem.MyEpisodes + $"/{(Data.AnimeRecommendationData.AllEpisodes == 0 ? "?" : Data.AnimeRecommendationData.AllEpisodes.ToString())}"));
            DetailItems.Add(new Tuple<string, string, string, string, string>("Score:",
                Data.AnimeDependentData.GlobalScore == 0 ? "N/A" : Data.AnimeDependentData.GlobalScore.ToString(),
                myDepItem?.MyScore == null ? "" : (myDepItem.MyScore == 0 ? "N/A" : $"{myDepItem.MyScore}/10"),
                Data.AnimeRecommendationData.GlobalScore == 0 ? "N/A" : Data.AnimeRecommendationData.GlobalScore.ToString(),
                myRecItem?.MyScore == null ? "" : (myRecItem.MyScore == 0 ? "N/A" : $"{myRecItem.MyScore}/10")));
            DetailItems.Add(new Tuple<string, string, string, string, string>("Type:", Data.AnimeDependentData.Type, "",
                Data.AnimeRecommendationData.Type, ""));
            DetailItems.Add(new Tuple<string, string, string, string, string>("Status:", Data.AnimeDependentData.Status, "",
                Data.AnimeRecommendationData.Status, ""));

            DetailItems.Add(new Tuple<string, string, string, string, string>("Start:",
                Data.AnimeDependentData.StartDate == AnimeItemViewModel.InvalidStartEndDate
                    ? "?"
                    : Data.AnimeDependentData.StartDate,
                myDepItem != null
                    ? (myDepItem.StartDate != AnimeItemViewModel.InvalidStartEndDate ? myDepItem.StartDate : "Not set")
                    : "",
                Data.AnimeRecommendationData.StartDate == AnimeItemViewModel.InvalidStartEndDate
                    ? "?"
                    : Data.AnimeRecommendationData.StartDate,
                myRecItem != null
                    ? (myRecItem.StartDate != AnimeItemViewModel.InvalidStartEndDate ? myRecItem.StartDate : "Not set")
                    : ""));

            DetailItems.Add(new Tuple<string, string, string, string, string>("End:",
                Data.AnimeDependentData.EndDate == AnimeItemViewModel.InvalidStartEndDate
                    ? "?"
                    : Data.AnimeDependentData.EndDate,
                myDepItem != null
                    ? (myDepItem.EndDate != AnimeItemViewModel.InvalidStartEndDate ? myDepItem.EndDate : "Not set")
                    : "",
                Data.AnimeRecommendationData.EndDate == AnimeItemViewModel.InvalidStartEndDate
                    ? "?"
                    : Data.AnimeRecommendationData.EndDate,
                myRecItem != null
                    ? (myRecItem.EndDate != AnimeItemViewModel.InvalidStartEndDate ? myRecItem.EndDate : "Not set")
                    : ""));
            LoadingSpinnerVisibility = false;
        }

        private ICommand _navigateDepDetailsCommand;
        public ICommand NavigateDepDetails
            => _navigateDepDetailsCommand ?? (_navigateDepDetailsCommand = new RelayCommand(NavigateDepDatails));

        private ICommand _navigateRecDetailsCommand;
        public ICommand NavigateRecDetails
            => _navigateRecDetailsCommand ?? (_navigateRecDetailsCommand = new RelayCommand(NavigateRecDatails));

        private void NavigateRecDatails()
        {
            if (ViewModelLocator.AnimeDetails.Id == Data.RecommendationId)
                return;
            ViewModelLocator.GeneralMain
                .Navigate(PageIndex.PageAnimeDetails,
                    new AnimeDetailsPageNavigationArgs(Data.RecommendationId, Data.RecommendationTitle,
                       Data.AnimeRecommendationData, null,
                        new RecommendationPageNavigationArgs { Index = Index })
                    { Source = PageIndex.PageRecomendations, AnimeMode = Data.IsAnime });
        }

        private void NavigateDepDatails()
        {
            if (ViewModelLocator.AnimeDetails.Id == Data.DependentId)
                return;
            ViewModelLocator.GeneralMain
                .Navigate(PageIndex.PageAnimeDetails,
                    new AnimeDetailsPageNavigationArgs(Data.DependentId, Data.DependentTitle,
                        Data.AnimeDependentData, null,
                        new RecommendationPageNavigationArgs { Index = Index })
                    { Source = PageIndex.PageRecomendations, AnimeMode = Data.IsAnime });
        }
    }
}
