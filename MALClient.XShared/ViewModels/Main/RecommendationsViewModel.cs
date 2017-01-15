using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Models.Enums;
using MALClient.Models.Models.AnimeScrapped;
using MALClient.Models.Models.Library;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.Utils;

namespace MALClient.XShared.ViewModels.Main
{
    public enum RecommendationsPageWorkMode
    {
        [EnumUtilities.Description("Anime Recommendations")]
        Anime,
        [EnumUtilities.Description("Manga Recommendations")]
        Manga,
        [EnumUtilities.Description("Anime Suggestions")]
        PersonalizedAnime,
        [EnumUtilities.Description("Manga Suggestions")]
        PersonalizedManga,
    }

    public class RecommendationsViewModel : ViewModelBase
    {
        public class XPivotItem
        {
            public object Header { get; set; }
            public object Content { get; set; }
        }

        private bool _loading;
        private int _pivotItemIndex;

        private List<XPivotItem> _recommendationAnimeItems;
        private List<XPivotItem> _recommendationMangaItems;

        private List<AnimeItemViewModel> _personalizedAnimeItems;
        private List<AnimeItemViewModel> _personalizedMangaItems;

        private RecommendationsPageWorkMode _currentWorkMode;
        private ICommand _switchWorkModeCommand;


        public List<XPivotItem> RecommendationAnimeItems
        {
            get { return _recommendationAnimeItems; }
            set
            {
                _recommendationAnimeItems = value;
                RaisePropertyChanged(() => RecommendationAnimeItems);
            }
        }

        public List<XPivotItem> RecommendationMangaItems
        {
            get { return _recommendationMangaItems; }
            set
            {
                _recommendationMangaItems = value;
                RaisePropertyChanged(() => RecommendationMangaItems);
            }
        }

        public List<AnimeItemViewModel> PersonalizedAnimeItems
        {
            get { return _personalizedAnimeItems; }
            set
            {
                _personalizedAnimeItems = value;
                RaisePropertyChanged(() => PersonalizedAnimeItems);
            }
        }

        public List<AnimeItemViewModel> PersonalizedMangaItems
        {
            get { return _personalizedMangaItems; }
            set
            {
                _personalizedMangaItems = value;
                RaisePropertyChanged(() => PersonalizedMangaItems);
            }
        }

        public bool Loading
        {
            get { return _loading; }
            set
            {
                _loading = value;
                RaisePropertyChanged(() => Loading);
            }
        }

        public int PivotItemIndex
        {
            get { return _pivotItemIndex; }
            set
            {
                _pivotItemIndex = value;
                if (!Loading)
                    RaisePropertyChanged(() => PivotItemIndex);
            }
        }

        public RecommendationsPageWorkMode CurrentWorkMode
        {
            get { return _currentWorkMode; }
            set
            {
                _currentWorkMode = value;
                RaisePropertyChanged(() => CurrentWorkMode);
                PopulateData();
            }
        }

        public ICommand SwitchWorkModeCommand => _switchWorkModeCommand ?? (_switchWorkModeCommand =
                                                     new RelayCommand<RecommendationsPageWorkMode>(
                                                         mode => CurrentWorkMode = mode));

        public double MaxWidth => AnimeItemViewModel.MaxWidth;

        public async void PopulateData(bool force = false)
        {
            ViewModelLocator.GeneralMain.CurrentStatus = CurrentWorkMode.GetDescription();

            if (Loading)
                return;

            if (!force)
            {
                switch (CurrentWorkMode)
                {
                    case RecommendationsPageWorkMode.Anime:
                        if (RecommendationAnimeItems != null)
                        {
                            RaisePropertyChanged(() => CurrentWorkMode);
                            return;
                        }
                        break;
                    case RecommendationsPageWorkMode.Manga:
                        if (RecommendationMangaItems != null)
                        {
                            RaisePropertyChanged(() => CurrentWorkMode);
                            return;
                        }
                        break;
                    case RecommendationsPageWorkMode.PersonalizedAnime:
                        if (PersonalizedAnimeItems != null)
                        {
                            RaisePropertyChanged(() => CurrentWorkMode);
                            return;
                        }
                        break;
                    case RecommendationsPageWorkMode.PersonalizedManga:
                        if (PersonalizedMangaItems != null)
                        {
                            RaisePropertyChanged(() => CurrentWorkMode);
                            return;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                RaisePropertyChanged(() => PivotItemIndex);
            }

            Loading = true;

            if (CurrentWorkMode == RecommendationsPageWorkMode.Anime ||
                CurrentWorkMode == RecommendationsPageWorkMode.Manga)
            {               
                var data =
                    await new AnimeRecomendationsQuery(CurrentWorkMode == RecommendationsPageWorkMode.Anime)
                        .GetRecomendationsData();

                if (data == null)
                {
                    Loading = false;
                    return;
                }

                var items = new List<XPivotItem>();
                var i = 0;
                foreach (var item in data)
                {
                    var pivot = new XPivotItem
                    {
                        Header = item.DependentTitle + "\n" + item.RecommendationTitle,
                        Content = new RecommendationItemViewModel(item, i++)
                    };
                    items.Add(pivot);
                }
                if (CurrentWorkMode == RecommendationsPageWorkMode.Anime)
                {
                    RecommendationAnimeItems = items;
                }
                else
                {
                    RecommendationMangaItems = items;
                }

                RaisePropertyChanged(() => PivotItemIndex);
            }
            else
            {
                var data =
                    await new AnimePersonalizedRecommendationsQuery(CurrentWorkMode ==
                                                                    RecommendationsPageWorkMode.PersonalizedAnime)
                        .GetPersonalizedRecommendations();
                if (CurrentWorkMode == RecommendationsPageWorkMode.PersonalizedAnime)
                {
                    PersonalizedAnimeItems =
                        data.Select(recommendationData => new AnimeItemAbstraction(false, new AnimeLibraryItemData
                        {
                            Title = recommendationData.Title,
                            Id = recommendationData.Id,
                            ImgUrl = recommendationData.ImgUrl
                        }).ViewModel).ToList();
                    PersonalizedAnimeItems.ForEach(model =>
                    {
                        model.UpdateButtonsVisibility = false;
                    });
                }
                else
                {
                    PersonalizedMangaItems =
                        data.Select(recommendationData => new AnimeItemAbstraction(false, new MangaLibraryItemData
                        {
                            Title = recommendationData.Title,
                            Id = recommendationData.Id,
                            ImgUrl = recommendationData.ImgUrl
                        }).ViewModel).ToList();
                    PersonalizedMangaItems.ForEach(model =>
                    {
                        model.UpdateButtonsVisibility = false;
                    });
                }
            }

            Loading = false;

        }
    }
}